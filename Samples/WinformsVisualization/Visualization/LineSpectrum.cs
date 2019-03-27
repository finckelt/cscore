using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using CSCore.DSP;

namespace WinformsVisualization.Visualization
{
    public class LineSpectrum : SpectrumBase
    {
        private int _barCount;
        private double _barSpacing;
        private double _barWidth;
        private Size _currentSize;
        public double FirstPointValue { get; set; } = 0.0;
        public double LastPointValue { get; set; } = 0.0;
        public double Average { get; set; } = 0.0;
        public double Max{ get; set; } = 0.0;
        public double MaxPercent { get; set; } = 0.0;
        public int MaxIndex { get; set; } = 0;
        public int DelayMS { get; set; } = 0;
        public double Sensitivity { get; set; } = 0;
        public bool EnableLights { get; set; }
        public Color StartColor { get; set; } = Color.FromArgb(255, 255, 255, 255);
        public bool UseFlow { get; set; }



        public LineSpectrum(FftSize fftSize)
        {
            FftSize = fftSize;
        }

        [Browsable(false)]
        public double BarWidth
        {
            get { return _barWidth; }
        }

        public double BarSpacing
        {
            get { return _barSpacing; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value");
                _barSpacing = value;
                UpdateFrequencyMapping();

                RaisePropertyChanged("BarSpacing");
                RaisePropertyChanged("BarWidth");
            }
        }

        public int BarCount
        {
            get { return _barCount; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");
                _barCount = value;
                SpectrumResolution = value;
                UpdateFrequencyMapping();

                RaisePropertyChanged("BarCount");
                RaisePropertyChanged("BarWidth");
            }
        }

        [BrowsableAttribute(false)]
        public Size CurrentSize
        {
            get { return _currentSize; }
            protected set
            {
                _currentSize = value;
                RaisePropertyChanged("CurrentSize");
            }
        }

        public Bitmap CreateSpectrumLine(Size size, Brush brush, Color background, bool highQuality)
        {
            if (!UpdateFrequencyMappingIfNessesary(size))
                return null;

            var fftBuffer = new float[(int) FftSize];

            //get the fft result from the spectrum provider
            if (SpectrumProvider.GetFftData(fftBuffer, this))
            {
                using (var pen = new Pen(brush, (float) _barWidth))
                {
                    var bitmap = new Bitmap(size.Width, size.Height);

                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        PrepareGraphics(graphics, highQuality);
                        graphics.Clear(background);

                        CreateSpectrumLineInternal(graphics, pen, fftBuffer, size);
                    }

                    return bitmap;
                }
            }
            return null;
        }

        public Bitmap CreateSpectrumLine(Size size, Color color1, Color color2, Color background, bool highQuality)
        {
            if (!UpdateFrequencyMappingIfNessesary(size))
                return null;

            using (
                Brush brush = new LinearGradientBrush(new RectangleF(0, 0, (float) _barWidth, size.Height), color2,
                    color1, LinearGradientMode.Vertical))
            {
                return CreateSpectrumLine(size, brush, background, highQuality);
            }
        }

        private void CreateSpectrumLineInternal(Graphics graphics, Pen pen, float[] fftBuffer, Size size)
        {
            int height = size.Height;
            //prepare the fft result for rendering 
            SpectrumPointData[] spectrumPoints = CalculateSpectrumPoints(height, fftBuffer);

            if (spectrumPoints != null && spectrumPoints.Length > 0)
            {
                FirstPointValue = spectrumPoints[0].Value;
                LastPointValue = spectrumPoints[spectrumPoints.Length - 1].Value;
                Average = spectrumPoints.Average(s => s.Value);
                Max = spectrumPoints.Max(s => s.Value);
                
                MaxPercent = 1 - ((height - Max) / height);
                if (Sensitivity > 0 && MaxPercent + Sensitivity <= 1)
                    MaxPercent = MaxPercent + Sensitivity;

                MaxIndex = spectrumPoints.ToList().IndexOf(spectrumPoints.First(s => s.Value == Max));
            }

            Console.WriteLine("First: {0}, Last: {1}, Average: {2}, Max: {3}, MaxPercent: {4}", FirstPointValue, LastPointValue, Average, Max, MaxPercent);

            //connect the calculated points with lines
            for (int i = 0; i < spectrumPoints.Length; i++)
            {
                SpectrumPointData p = spectrumPoints[i];
                int barIndex = p.SpectrumPointIndex;
                double xCoord = BarSpacing * (barIndex + 1) + (_barWidth * barIndex) + _barWidth / 2;

                var p1 = new PointF((float) xCoord, height);
                var p2 = new PointF((float) xCoord, height - (float) p.Value - 1);

                Console.WriteLine("Height: {0}, PValue:{1}, Calc: {2}", height, p.Value, height - (float)p.Value - 1);

                graphics.DrawLine(pen, p1, p2);
            }
        }

        protected override void UpdateFrequencyMapping()
        {
            _barWidth = Math.Max(((_currentSize.Width - (BarSpacing * (BarCount + 1))) / BarCount), 0.00001);
            base.UpdateFrequencyMapping();
        }

        private bool UpdateFrequencyMappingIfNessesary(Size newSize)
        {
            if (newSize != CurrentSize)
            {
                CurrentSize = newSize;
                UpdateFrequencyMapping();
            }

            return newSize.Width > 0 && newSize.Height > 0;
        }

        private void PrepareGraphics(Graphics graphics, bool highQuality)
        {
            if (highQuality)
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.CompositingQuality = CompositingQuality.AssumeLinear;
                graphics.PixelOffsetMode = PixelOffsetMode.Default;
                graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            }
            else
            {
                graphics.SmoothingMode = SmoothingMode.HighSpeed;
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.PixelOffsetMode = PixelOffsetMode.None;
                graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
            }
        }
    }
}