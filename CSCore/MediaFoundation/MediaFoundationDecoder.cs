﻿using CSCore.Win32;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace CSCore.MediaFoundation
{
    /// <summary>
    /// The <see cref="MediaFoundationDecoder"/> is a generic decoder for all installed Mediafoundation codecs.
    /// </summary>
    public class MediaFoundationDecoder : IWaveSource
    {
        private MFByteStream _byteStream;
        private MFSourceReader _reader;
        private WaveFormat _waveFormat;
        private Stream _stream;
        private readonly Object _lockObj = new Object();

        private long _length;
        private long _position; //could not find a possibility to find out the position -> we have to track the position ourselves.
        private readonly bool _hasFixedLength;

        private byte[] _decoderBuffer;
        private int _decoderBufferOffset;
        private int _decoderBufferCount;

        static MediaFoundationDecoder()
        {
            MediaFoundationCore.Startup(); //make sure that the MediaFoundation is started up.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaFoundationDecoder"/> class.
        /// </summary>
        /// <param name="url">Uri which points to an audio source which can be decoded.</param>
        public MediaFoundationDecoder(string url)
        {
            if (String.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");

            _hasFixedLength = true;

            _reader = Initialize(MediaFoundationCore.CreateSourceReaderFromUrl(url));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaFoundationDecoder"/> class.
        /// </summary>
        /// <param name="stream">Stream which provides the audio data to decode.</param>
        public MediaFoundationDecoder(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (!stream.CanRead)
                throw new ArgumentException("Stream is not readable.", "stream");

            stream = new ComStream(stream);
            _stream = stream;
            _byteStream = MediaFoundationCore.IStreamToByteStream((IStream)stream);
            _reader = Initialize(_byteStream);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaFoundationDecoder"/> class.
        /// </summary>
        /// <param name="byteStream">Stream which provides the audio data to decode.</param>
        public MediaFoundationDecoder(MFByteStream byteStream)
        {
            if (byteStream == null)
                throw new ArgumentNullException("byteStream");
            _byteStream = byteStream;
            _reader = Initialize(_byteStream);
        }

        private MFSourceReader Initialize(MFByteStream byteStream)
        {
            return Initialize(MediaFoundationCore.CreateSourceReaderFromByteStream(byteStream.BasePtr, IntPtr.Zero));
        }

        private MFSourceReader Initialize(MFSourceReader reader)
        {
            try
            {
                reader.SetStreamSelection(NativeMethods.MF_SOURCE_READER_ALL_STREAMS, false);
                reader.SetStreamSelection(NativeMethods.MF_SOURCE_READER_FIRST_AUDIO_STREAM, true);

                using (var mediaType = MFMediaType.CreateEmpty())
                {
                    mediaType.MajorType = AudioSubTypes.MediaTypeAudio;
                    mediaType.SubType = AudioSubTypes.Pcm; //variable??

                    reader.SetCurrentMediaType(NativeMethods.MF_SOURCE_READER_FIRST_AUDIO_STREAM, mediaType);
                }

                using (var currentMediaType = reader.GetCurrentMediaType(NativeMethods.MF_SOURCE_READER_FIRST_AUDIO_STREAM))
                {
                    if (currentMediaType.MajorType != AudioSubTypes.MediaTypeAudio)
                        throw new InvalidOperationException(String.Format("Invalid Majortype set on sourcereader: {0}.", currentMediaType.MajorType));

                    AudioEncoding encoding = AudioSubTypes.EncodingFromMediaType(currentMediaType.SubType);

                    ChannelMask channelMask;
                    if (currentMediaType.TryGet(MediaFoundationAttributes.MF_MT_AUDIO_CHANNEL_MASK, out channelMask))
                        //check whether the attribute is available
                    {
                        _waveFormat = new WaveFormatExtensible(currentMediaType.SampleRate,
                            currentMediaType.BitsPerSample, currentMediaType.Channels, currentMediaType.SubType,
                            channelMask);
                    }
                    else
                    {
                        _waveFormat = new WaveFormat(currentMediaType.SampleRate, currentMediaType.BitsPerSample,
                            currentMediaType.Channels, encoding);
                    }
                }

                reader.SetStreamSelection(NativeMethods.MF_SOURCE_READER_FIRST_AUDIO_STREAM, true);

                if (_hasFixedLength)
                    _length = GetLength(reader);

                return reader;
            }
            catch (Exception)
            {
                DisposeInternal(true);
                throw;
            }
        }

        private long GetLength(MFSourceReader reader)
        {
            lock (_lockObj)
            {
                try
                {
                    if (reader == null)
                        return 0;

                    using (
                        PropertyVariant value =
                            reader.GetPresentationAttribute(NativeMethods.MF_SOURCE_READER_MEDIASOURCE,
                                MediaFoundationAttributes.MF_PD_DURATION))


                    {
                        //bug: still, depending on the decoder, this returns imprecise values.
                        return ((value.HValue) * _waveFormat.BytesPerSecond) / 10000000L;
                    }
                }
                catch (Exception)
                {
                    //if (e.Result == (int)HResult.MF_E_ATTRIBUTENOTFOUND)
                    //    return 0;
                    //throw;
                    return 0;
                }
            }
        }

        private void SetPosition(long value)
        {
            if (CanSeek)
            {
                lock (_lockObj)
                {
                    long hnsPos = (10000000L * value) / WaveFormat.BytesPerSecond;
                    var propertyVariant = new PropertyVariant() { HValue = hnsPos, DataType = VarEnum.VT_I8 };
                    _reader.SetCurrentPosition(Guid.Empty, propertyVariant);
                    _decoderBufferCount = 0;
                    _decoderBufferOffset = 0;
                    _position = value;
                }
            }
        }

        /// <summary>
        ///     Reads a sequence of bytes from the <see cref="MediaFoundationDecoder" /> and advances the position within the stream by the
        ///     number of bytes read.
        /// </summary>
        /// <param name="buffer">
        ///     An array of bytes. When this method returns, the <paramref name="buffer" /> contains the specified
        ///     byte array with the values between <paramref name="offset" /> and (<paramref name="offset" /> +
        ///     <paramref name="count" /> - 1) replaced by the bytes read from the current source.
        /// </param>
        /// <param name="offset">
        ///     The zero-based byte offset in the <paramref name="buffer" /> at which to begin storing the data
        ///     read from the current stream.
        /// </param>
        /// <param name="count">The maximum number of bytes to read from the current source.</param>
        /// <returns>The total number of bytes read into the buffer.</returns>
        public int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (buffer.Length < count)
                throw new ArgumentException("Length is too small.", "buffer");

            lock (_lockObj)
            {
                int read = 0;

                if (_reader == null || _disposed)
                    return read;

                if (_decoderBufferCount > 0)
                {
                    read += CopyDecoderBuffer(buffer, offset + read, count - read);
                }

                while (read < count)
                {
                    MFSourceReaderFlags flags;
                    long timestamp;
                    int actualStreamIndex;
                    using (var sample = _reader.ReadSample(NativeMethods.MF_SOURCE_READER_FIRST_AUDIO_STREAM, 0, out actualStreamIndex, out flags, out timestamp))
                    {
                        if (flags != MFSourceReaderFlags.None)
                            break;

                        using (MFMediaBuffer mediaBuffer = sample.ConvertToContiguousBuffer())
                        {
                            using (var @lock = mediaBuffer.Lock())
                            {
                                _decoderBuffer = _decoderBuffer.CheckBuffer(@lock.CurrentLength);
                                Marshal.Copy(@lock.Buffer, _decoderBuffer, 0, @lock.CurrentLength);
                                _decoderBufferCount = @lock.CurrentLength;
                                _decoderBufferOffset = 0;

                                int tmp = CopyDecoderBuffer(buffer, offset + read, count - read);
                                read += tmp;
                            }
                        }
                    }
                }

                _position += read;

                return read;
            }
        }

        private int CopyDecoderBuffer(byte[] destBuffer, int offset, int count)
        {
            count = Math.Min(count, _decoderBufferCount);
            Array.Copy(_decoderBuffer, _decoderBufferOffset, destBuffer, offset, count);
            _decoderBufferCount -= count;
            _decoderBufferOffset += count;

            if (_decoderBufferCount == 0)
                _decoderBufferOffset = 0;

            return count;
        }

        private bool _disposed;

        /// <summary>
        /// Disposes the <see cref="MediaFoundationDecoder"/>.
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Disposes the <see cref="MediaFoundationDecoder"/> and its internal resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            lock (_lockObj)
            {
                DisposeInternal(disposing);
            }
        }

        private void DisposeInternal(bool disposing)
        {
            if (_reader != null)
            {
                _reader.Dispose();
                _reader = null;
            }
            if (_byteStream != null)
            {
                _byteStream.Dispose();
                _byteStream = null;
            }
            if (_stream != null)
            {
                _stream.Dispose();
                _stream = null;
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="MediaFoundationDecoder"/> class.
        /// </summary>
        ~MediaFoundationDecoder()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets the format of the decoded audio data provided by the <see cref="Read"/> method.
        /// </summary>
        public WaveFormat WaveFormat
        {
            get { return _waveFormat; }
        }

        /// <summary>
        /// Gets or sets the position of the output stream, in bytes.
        /// </summary>
        public long Position
        {
            get
            {
                return _position;
            }
            set
            {
                SetPosition(value);
            }
        }

        /// <summary>
        /// Gets the total length of the decoded audio, in bytes.
        /// </summary>
        public long Length
        {
            get
            {
                if (_hasFixedLength)
                    return _length;
                return GetLength(_reader);
            }
        }

        /// <summary>
        /// Gets a value which indicates whether the seeking is supported. True means that seeking is supported. False means that seeking is not supported.
        /// </summary>
        public bool CanSeek
        {
            get { return _reader.CanSeek; }
        }
    }
}