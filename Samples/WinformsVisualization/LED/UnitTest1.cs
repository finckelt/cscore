using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using MagicLedLibrary;
using MagicLedLibrary.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace MagicLedTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var lightControl = new LightsController();

            lightControl.Scan();

            lightControl.Lights.ForEach(l => Debug.WriteLine(l.ToString()));

            var color = lightControl.Lights[0].CurrentColor;
            var goingUp = 2;
            var incRandomRange = 3;
            var rand = new Random();
            bool run = true;
            var r = new ColorChannelShifter(color.R);
            var g = new ColorChannelShifter(color.G);
            var b = new ColorChannelShifter(color.B);

            var shifters = new List<ColorChannelShifter> { r, g, b };



            var count = 0;
            var type = "flow";
            var lastType = "";

            while (run)
            {
                switch (type)
                {
                    case "flow":
                        if (lastType != "flow")
                        {
                            r.Unit = 50;
                            g.Unit = 200;
                            b.Unit = 180;

                            goingUp = rand.Next(0, 2);
                            shifters[goingUp].GoingUp = true;
                        }
                        color = Flow(shifters, lightControl, goingUp);
                        lastType = "flow";
                        break;
                    case "increment":
                        if (lastType != "increment")
                        {
                            shifters.ForEach(s => s.Unit = 1);
                            shifters[goingUp].GoingUp = true;
                        }
                        color = Flow(shifters, lightControl, goingUp);
                        lastType = "increment";
                        break;
                    case "random":
                        if (lastType != "random")
                        {
                            shifters.ForEach(s => s.Unit = 180);

                            goingUp = rand.Next(0, 2);
                            incRandomRange = rand.Next(3, 10);
                            shifters[goingUp].GoingUp = true;
                        }

                        color = Random(shifters, lightControl, goingUp, incRandomRange);
                        lastType = "random";
                        break;
                }


                count++;

                if (count > 10000000)
                    run = false;
            }


            lightControl.Dispose();
        }

        public static Color Flow(List<ColorChannelShifter> shifters, LightsController lightControl, int goingUp)
        {

            
            shifters.ForEach(s => s.ShiftBy(1));
            var color = ColorChannelShifter.FromShifters(shifters[0], shifters[1], shifters[2]);
            //lightControl.GetLightsByPowerStatus(true).SetColors(color);
            

            Debug.WriteLine(string.Format("{0};{1};{2}", color.R, color.G, color.B));
            //lightControl.GetLightsByPowerStatus(true).ForEach(l => Debug.WriteLine(l.ToString()));
            return color;
        }

        private static Color Increment(List<ColorChannelShifter> shifters, LightsController lightControl)
        {

          
            shifters.ForEach(s => s.ShiftBy(1));
            var color = ColorChannelShifter.FromShifters(shifters[0], shifters[1], shifters[2]);
            lightControl.GetLightsByPowerStatus(true).SetColors(color);
            Task.Delay(10);

            Debug.WriteLine(string.Format("{0};{1};{2}", color.R, color.G, color.B));
            //lightControl.GetLightsByPowerStatus(true).ForEach(l => Debug.WriteLine(l.ToString()));
            return color;
        }

        private static Color Random(List<ColorChannelShifter> shifters, LightsController lightControl, int goingUp, int randomRange)
        {

            
            shifters.ForEach(s => s.ShiftBy(new Random().Next(0, randomRange)));
            var color = ColorChannelShifter.FromShifters(shifters[0], shifters[1], shifters[2]);
            lightControl.GetLightsByPowerStatus(true).SetColors(color);
            Task.Delay(10);

            Debug.WriteLine(string.Format("{0};{1};{2}", color.R, color.G, color.B));
            //lightControl.GetLightsByPowerStatus(true).ForEach(l => Debug.WriteLine(l.ToString()));
            return color;
        }
    }

    public class ColorChannelShifter
    {
        public byte Unit { get; set; }
        public bool GoingUp { get; set; }

        public ColorChannelShifter(byte unit)
        {
            Unit = unit;
        }

        public ColorChannelShifter(int unit)
        {
            Unit = Convert.ToByte(unit);
        }

        public void ShiftBy(int units)
        {
            if (GoingUp && Convert.ToInt32(Unit) + units <= 255)
            {
                Unit = Convert.ToByte(Convert.ToInt32(Unit) + units);
            }
            else
            {
                GoingUp = false;
                var amount = Convert.ToInt32(Unit) - units;

                amount = amount > 0 ? amount : 0;

                Unit = Convert.ToByte(amount);
            }

            if (GoingUp)
            {
                GoingUp = Convert.ToInt32(Unit) < 255 && GoingUp;
            }
            else
            {
                GoingUp = Convert.ToInt32(Unit) <= 0 || GoingUp;
            }

        }

        public static Color FromShifters(ColorChannelShifter r, ColorChannelShifter g, ColorChannelShifter b,
            ColorChannelShifter a = null)
        {
            a = a ?? new ColorChannelShifter(255);
            return Color.FromArgb(a.Unit, r.Unit, g.Unit, b.Unit);
        }
    }

    public class LightsController
    {
        readonly LedLibrary _ledLibrary = new LedLibrary();
        public List<FoundBulbModels> FoundBulbs { get; set; } = new List<FoundBulbModels>();
        public List<Light> Lights { get; set; } = new List<Light>();

        Socket _socket;
        bool scanClicked = false;

        private void LoadFoundBulbs(List<string> foundList)
        {
            foreach (var item in foundList)
            {
                var splitItem = item.Trim().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (splitItem.Length == 3)
                {
                    FoundBulbModels model = new FoundBulbModels
                    {
                        IPAddress = splitItem[0],
                        Id = splitItem[1],
                        Model = splitItem[2].Trim('\0')
                    };
                    if (FoundBulbs.All(m => m.IPAddress != model.IPAddress))
                        FoundBulbs.Add(model);
                }
            }

            FoundBulbs.ForEach(b =>
            {
                Lights.Add(new Light(b));

                Task.Delay(10).Wait();


            });
        }

        public async Task ScanAsync()
        {
            if (!scanClicked)
            {
                scanClicked = true;
                Debug.WriteLine("Scanning...");
                await Task.Delay(1);
                // Send message
                var sendModels = _ledLibrary.Send(CommonHelpers.DISCOVERY_PORT);
                Debug.WriteLine(sendModels.Status);
                _socket = sendModels.Socket;
                // Receive message
                await Task.Delay(1);
                var progress = new Progress<string>(status => { Debug.WriteLine(status); });
                LoadFoundBulbs(await _ledLibrary.Receive(progress, _socket, CommonHelpers.DISCOVERY_PORT,
                    CommonHelpers.TIMEOUT));
                Debug.WriteLine("Ready");
                scanClicked = false;
            }
        }


        public void Scan()
        {
            var scanTask = ScanAsync();
            scanTask.Wait();
        }

        public void Refresh()
        {
            Lights.ForEach(l =>
            {
                l.Refresh();
                Task.Delay(1).Wait();
            });
        }


        public List<Light> GetLightsByPowerStatus(bool on)
        {
            return Lights.FindAll(l => l.PowerState == on);
        }

        public void Dispose()
        {
            Lights.ForEach(l => l.Dispose());
        }

    }

    [DataContract]
    public class Light
    {
        public FoundBulbModels FoundBulb;
        readonly LedLibrary _ledLibrary = new LedLibrary();
        [DataMember]
        public bool Selected { get; set; }

        [DataMember]
        public bool EnableColorFrequencies { get; set; }

        [DataMember]
        public string DisplayName => HostName ?? "NULL" + " / " + IpAddress ?? "NULL";
        [DataMember]
        public string HostName { get; set; }
        [DataMember]
        public string IpAddress { get; set; }
        public ConnectModels ConnectModel { get; set; }
        public Socket Socket => ConnectModel?.Socket;
        [DataMember]
        public string Status
        {
            get => ConnectModel?.Status;
            set
            {
                if (ConnectModel != null)
                    ConnectModel.Status = value;
            }
        }

        [DataMember]
        public RefreshModels RefreshModel { get; set; }

        [DataMember]
        public bool PowerState
        {
            get => RefreshModel?.PowerState ?? false;
            set
            {
                List<byte> msg = new List<byte>();

                msg = !value ? new List<byte> { 0x71, 0x23, 0x0f } : new List<byte> { 0x71, 0x24, 0x0f };

                if (Socket != null && !Disposed)
                    _ledLibrary.Write(Socket, msg);

                if (RefreshModel != null)
                    RefreshModel.PowerState = value;
            }
        }

        public Color LowColor { get; set; } = Color.FromArgb(Convert.ToByte(255), Convert.ToByte(255),
            Convert.ToByte(255), Convert.ToByte(255));

        public Color MidColor { get; set; } = Color.FromArgb(Convert.ToByte(255), Convert.ToByte(255),
            Convert.ToByte(255), Convert.ToByte(255));

        public Color HighColor { get; set; } = Color.FromArgb(Convert.ToByte(255), Convert.ToByte(255),
            Convert.ToByte(255), Convert.ToByte(255));


        public Color CurrentColor
        {
            get => RefreshModel?.CurrentColor ?? Color.FromArgb(Convert.ToByte(255), Convert.ToByte(255),
                       Convert.ToByte(255), Convert.ToByte(255));
            set
            {
                if (Socket != null && !Disposed)
                    _ledLibrary.SetRGB(value.R, value.G, value.B, Socket);

                if (RefreshModel != null)
                    RefreshModel.CurrentColor = value;
            }
        }

        [DataMember]
        public string CurrentColorString
        {
            get => string.Format("{0};{1};{2}", CurrentColor.R, CurrentColor.G, CurrentColor.B);
            set
            {
                var vals = value.Split(';');
                Console.WriteLine(vals);
            }
        }



        public bool Disposed = false;



        public Light(FoundBulbModels bulbInfo)
        {
            FoundBulb = bulbInfo;
            var dnsTask = Dns.GetHostEntryAsync(FoundBulb.IPAddress);
            dnsTask.Wait();
            HostName = dnsTask.Result.HostName;
            IpAddress = FoundBulb.IPAddress;
            ConnectModel = _ledLibrary.Connect(FoundBulb.IPAddress, CommonHelpers.CONNECT_PORT);
            Refresh();
        }

        public void Refresh()
        {
            if (Socket != null && !Disposed)
                RefreshModel = _ledLibrary.Refresh(Socket, CommonHelpers.MAX_BUFFER_SIZE);
        }



        public void Dispose()
        {
            if (!Disposed)
                Socket?.Dispose();

            Disposed = true;
        }

        public override string ToString()
        {
            return this.SerializeToJson();
        }
    }

    public static class LightExtensions
    {

        public static void SetColors(this List<Light> lights, Color color)
        {
            lights.ForEach(l =>
            {
                l.CurrentColor = color;
                Task.Delay(1).Wait();
            });
        }

        public static Color AdjustPercentage(this Color color, double percent)
        {
            return Color.FromArgb(255, Convert.ToInt32(color.R * percent), Convert.ToInt32(color.G * percent),
                Convert.ToInt32(color.B * percent));
        }
    }


    public static class Serializer
    {
        /// <summary>
        /// Returns the serialized JSON Representation of an object
        /// </summary>
        /// <param name="input">Object to serialize</param>
        /// <returns>Serialized JSON String</returns>
        public static string Serialize(object input)
        {
            string returnValue;

            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(input.GetType());
                serializer.WriteObject(stream, input);

                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    returnValue = reader.ReadToEnd();
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Deserializes a JSON String to the outputObject
        /// </summary>
        /// <typeparam name="T">Type of the Serialized Object</typeparam>
        /// <param name="serializedObject">JSON String</param>
        /// <returns>Deserialized Object</returns>
        public static T Deserialize<T>(string serializedObject)
        {
            T returnValue;
            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                var uniEncoding = new UnicodeEncoding();

                // Create the data to write to the stream.
                byte[] serializedBytes = uniEncoding.GetBytes(serializedObject);
                stream.Write(serializedBytes, 0, serializedBytes.Length);

                stream.Position = 0;
                returnValue = (T)serializer.ReadObject(stream);
            }

            return returnValue;
        }
    }

    public static class SerializerExtensions
    {
        /// <summary>
        /// Extension method for Json object serialization.
        /// Usage: var string serializedObject = someObject.serializeToJson();
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string SerializeToJson(this object input)
        {
            return Serializer.Serialize(input);
        }

        /// <summary>
        /// Extension method for Json string deserialization.
        /// Usage: var List<string> deserializedObject = serializedString.DeserializeFromJson<List<string>>();
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializedObject"></param>
        /// <returns></returns>
        public static T DeserializeFromJson<T>(this string serializedObject)
        {
            return Serializer.Deserialize<T>(serializedObject);
        }

    }
}
