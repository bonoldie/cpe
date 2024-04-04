using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace CPE.Zephyr
{

    public struct Calibration
    {
        public string CameraMaker;
        public string CameraModel;
        public float Lense;
        public float CCDWidth;
        public int W;
        public int H;
        public double Fx;
        public double Fy;
        public double Cx;
        public double Cy;
        public double K1;
        public double K2;
        public double K3;
        public int P1;
        public int P2;
        public double Skew;
        public string Name;
    }

    public class Camera
    {
        private readonly XmlDocument XMP = new XmlDocument();

        public string Name = "";

        public Calibration Calibration = new Calibration();


        public double[,] Rotation = new double[3,3];

        public double[] Translation = new double[3];

        public Camera()
        {
        }

        public void LoadXMPFromFile(string XMPFileContent)
        {
            XMP.Load(new StringReader(XMPFileContent));

            // TODO: validation??

            // Logger.NLogger.Info(XMPFileContent);
            // Logger.NLogger.Info(JsonConvert.SerializeObject(this.Translation));
            // Logger.NLogger.Info(JsonConvert.SerializeObject(this.Rotation));
            
            // Unpacking the data

            Name = XMP.SelectNodes("/camera")[0]?.Attributes["name"]?.Value ?? "";

            Calibration.CameraMaker = XMP.SelectNodes("/camera/calibration")[0]?.Attributes["cameraMaker"].Value ?? "";
            Calibration.CameraModel = XMP.SelectNodes("/camera/calibration")[0]?.Attributes["cameraModel"].Value ?? "";
            Calibration.Lense = float.Parse(Regex.Matches(XMP.SelectNodes("/camera/calibration")[0]?.Attributes["lense"].Value, @"[+-]?([0-9]*[.])?[0-9]+").First().Value ?? "");
            Calibration.CCDWidth = float.Parse(XMP.SelectNodes("/camera/calibration")[0]?.Attributes["ccdwidth"].Value ?? "");
            Calibration.W = int.Parse(XMP.SelectNodes("/camera/calibration")[0]?.Attributes["w"].Value ?? "");
            Calibration.H = int.Parse(XMP.SelectNodes("/camera/calibration")[0]?.Attributes["h"].Value ?? "");
            Calibration.Fx = double.Parse(XMP.SelectNodes("/camera/calibration")[0]?.Attributes["fx"].Value ?? "");
            Calibration.Fy = double.Parse(XMP.SelectNodes("/camera/calibration")[0]?.Attributes["fy"].Value ?? "");
            Calibration.Cx = double.Parse(XMP.SelectNodes("/camera/calibration")[0]?.Attributes["cx"].Value ?? "");
            Calibration.Cy = double.Parse(XMP.SelectNodes("/camera/calibration")[0]?.Attributes["cy"].Value ?? "");
            Calibration.K1 = double.Parse(XMP.SelectNodes("/camera/calibration")[0]?.Attributes["k1"].Value ?? "");
            Calibration.K2 = double.Parse(XMP.SelectNodes("/camera/calibration")[0]?.Attributes["k2"].Value ?? "");
            Calibration.K3 = double.Parse(XMP.SelectNodes("/camera/calibration")[0]?.Attributes["k3"].Value ?? "");
            Calibration.P1 = int.Parse(XMP.SelectNodes("/camera/calibration")[0]?.Attributes["p1"].Value ?? "");
            Calibration.P2 = int.Parse(XMP.SelectNodes("/camera/calibration")[0]?.Attributes["p2"].Value ?? "");
            Calibration.Skew = double.Parse(XMP.SelectNodes("/camera/calibration")[0]?.Attributes["skew"].Value ?? "");
            Calibration.Name = XMP.SelectNodes("/camera/calibration")[0]?.Attributes["name"].Value ?? "";

            Rotation =  Regex
                .Matches(XMP.SelectNodes("/camera/extrinsics/rotation")[0].InnerText, @"[+-]?([0-9]*[.])?[0-9]+")
                .Select((val, index) => new { Value = double.Parse(val.Value), Index = index })
                // Array to matrix
                .Aggregate(new double[3, 3], (acc, val) => { acc[val.Index / 3, val.Index % 3] = val.Value; return acc; });
            
            Translation = Regex
                .Matches(XMP.SelectNodes("/camera/extrinsics/translation")[0].InnerText, @"[+-]?([0-9]*[.])?[0-9]+")
                .Select(val => double.Parse(val.Value))
                .ToArray();
        }
    }
}