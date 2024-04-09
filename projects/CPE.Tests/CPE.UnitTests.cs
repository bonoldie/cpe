using CPE.Zephyr;
using Xunit.Abstractions;
using CPE.Utils;
using CPE.CV;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Data.Text;
using MathNet.Numerics.Data.Matlab;

namespace CPE.UnitTests;

public class CPE_Tests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public CPE_Tests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        Logger.Setup();
    }

    [Fact]
    public void Test_Camera()
    {
        Camera cam = new Camera();



        cam.LoadXMPFromFile("xmp/_SAM1020.xmp");

        Assert.True(cam.XMPFile.Calibration.Lense == 25, "XMP calibration(lense) parsing not working");

        Assert.True(cam.XMPFile.Rotation.ColumnCount == cam.XMPFile.Rotation.RowCount && cam.XMPFile.Rotation[0, 0] == -0.008710670784014551 && cam.XMPFile.Rotation[2, 1] == -0.03062245974281945, "XMP rotation matrix parsing not working");
        Assert.True(cam.XMPFile.Translation.Count() == 3 && cam.XMPFile.Translation[1] == 9.756892735915628, "XMP translation vector parsing not working");
    }

    [Fact]
    public void Test_Ply()
    {

        string plyFile = File.ReadAllText("ply/SamPointCloud.ply");

        Ply ply = Ply.Parse(plyFile);

        var x = ply.GetDoubleVector("x");
        var r = ply.GetUCharVector("red");

        Assert.True(ply.Header.Properties.Count == 6, "Header parsing error(properties)");
        Assert.True(ply.Header.Properties.FindIndex(prop => prop.PType == PropertyType.Unknown) < 0, "Unkown properties in header");
        Assert.True(r.Count == int.Parse(ply.Header.Element.EValue) && x.Count == int.Parse(ply.Header.Element.EValue), "Vertices number mismatch");
    }

    [Fact]
    public void Test_PointCloud()
    {

        PointCloud pc = new PointCloud();

        pc.LoadPlyFromFile("ply/SamPointCloud.ply");

        Assert.Throws<FileNotFoundException>(() => pc.LoadPlyFromFile("ply/NotExists.ply"));
    }

    [Fact]
    public void Test_Visibility()
    {

        Visibility vis = new Visibility();

        vis.LoadVisibilityFromFile("vis/Visibility.txt");

        VisibilityMap visMap = vis.GetVisibilityMapByCameraName("_SAM1002.JPG");

        Assert.Throws<FileNotFoundException>(() => vis.LoadVisibilityFromFile("vis/NotExists.ply"));

        Assert.NotNull(visMap);
        Assert.Equal(3894, visMap.Data.GetLength(0));
    }

    [Fact]
    public void Test_Features2D()
    {

    }

    [Fact]
    public void Test_PoseEstimation()
    {
        PoseEstimation pe = new();

        Matrix<double> Points2D = DelimitedReader.Read<double>("mat/points2D.csv", false, ",");
        Matrix<double> Points3D = DelimitedReader.Read<double>("mat/points3D.csv", false, ",");
        Matrix<double> K = DelimitedReader.Read<double>("mat/K.csv", false, ",");

        pe.EstimatePose(Points2D.SubMatrix(0, 100, 0, Points2D.ColumnCount), Points3D.SubMatrix(0, 100, 0, Points3D.ColumnCount), K);
    }
}
