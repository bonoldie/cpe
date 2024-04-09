using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using CPE.Utils;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;

namespace CPE.Zephyr
{

    public struct CameraCalibration
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

    public class XMP
    {
        public string Name = "";

        public CameraCalibration Calibration;

        public Matrix<double> Rotation;

        public MathNet.Numerics.LinearAlgebra.Vector<double> Translation;

        public XMP(string _Name, CameraCalibration _Calibration, Matrix<double> _Rotation, MathNet.Numerics.LinearAlgebra.Vector<double> _Translation)
        {
            this.Name = _Name;
            this.Calibration = _Calibration;
            this.Rotation = _Rotation;
            this.Translation = _Translation;

        }

        public static XMP Parse(string XMPFileContent)
        {
            XmlDocument XMPFile = new XmlDocument();

            XMPFile.Load(new StringReader(XMPFileContent));

            // TODO: validation??

            // Logger.NLogger.Info(XMPFileContent);
            // Logger.NLogger.Info(JsonConvert.SerializeObject(this.Translation));
            // Logger.NLogger.Info(JsonConvert.SerializeObject(this.Rotation));

            // Unpacking the data

            string Name = XMPFile.SelectNodes("/camera")[0]?.Attributes["name"]?.Value ?? "";


            CameraCalibration Cal = new CameraCalibration();

            Matrix<double> Rotation;
            MathNet.Numerics.LinearAlgebra.Vector<double> Translation;

            Cal.CameraMaker = XMPFile.SelectNodes("/camera/calibration")[0]?.Attributes["cameraMaker"].Value ?? "";
            Cal.CameraModel = XMPFile.SelectNodes("/camera/calibration")[0]?.Attributes["cameraModel"].Value ?? "";
            Cal.Lense = float.Parse(Regex.Matches(XMPFile.SelectNodes("/camera/calibration")[0]?.Attributes["lense"].Value, @"[+-]?([0-9]*[.])?[0-9]+").First().Value ?? "");
            Cal.CCDWidth = float.Parse(XMPFile.SelectNodes("/camera/calibration")[0]?.Attributes["ccdwidth"].Value ?? "");
            Cal.W = int.Parse(XMPFile.SelectNodes("/camera/calibration")[0]?.Attributes["w"].Value ?? "");
            Cal.H = int.Parse(XMPFile.SelectNodes("/camera/calibration")[0]?.Attributes["h"].Value ?? "");
            Cal.Fx = double.Parse(XMPFile.SelectNodes("/camera/calibration")[0]?.Attributes["fx"].Value ?? "");
            Cal.Fy = double.Parse(XMPFile.SelectNodes("/camera/calibration")[0]?.Attributes["fy"].Value ?? "");
            Cal.Cx = double.Parse(XMPFile.SelectNodes("/camera/calibration")[0]?.Attributes["cx"].Value ?? "");
            Cal.Cy = double.Parse(XMPFile.SelectNodes("/camera/calibration")[0]?.Attributes["cy"].Value ?? "");
            Cal.K1 = double.Parse(XMPFile.SelectNodes("/camera/calibration")[0]?.Attributes["k1"].Value ?? "");
            Cal.K2 = double.Parse(XMPFile.SelectNodes("/camera/calibration")[0]?.Attributes["k2"].Value ?? "");
            Cal.K3 = double.Parse(XMPFile.SelectNodes("/camera/calibration")[0]?.Attributes["k3"].Value ?? "");
            Cal.P1 = int.Parse(XMPFile.SelectNodes("/camera/calibration")[0]?.Attributes["p1"].Value ?? "");
            Cal.P2 = int.Parse(XMPFile.SelectNodes("/camera/calibration")[0]?.Attributes["p2"].Value ?? "");
            Cal.Skew = double.Parse(XMPFile.SelectNodes("/camera/calibration")[0]?.Attributes["skew"].Value ?? "");
            Cal.Name = XMPFile.SelectNodes("/camera/calibration")[0]?.Attributes["name"].Value ?? "";

            Rotation = Matrix<double>.Build.Dense(3, 3, Regex
                .Matches(XMPFile.SelectNodes("/camera/extrinsics/rotation")[0].InnerText, @"[+-]?([0-9]*[.])?[0-9]+")
                .Select(val => double.Parse(val.Value)).ToArray());

            Translation = CreateVector.Dense(Regex
                .Matches(XMPFile.SelectNodes("/camera/extrinsics/translation")[0].InnerText, @"[+-]?([0-9]*[.])?[0-9]+")
                .Select(val => double.Parse(val.Value))
                .ToArray());

            Logger.NLogger.Info(Rotation.ToMatrixString());
            
            return new XMP(Name, Cal, Rotation, Translation);
        }
    }
}