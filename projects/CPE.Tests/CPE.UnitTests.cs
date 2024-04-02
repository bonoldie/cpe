using CPE.Zephyr;
using Xunit.Abstractions;
using CPE.Utils;

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

        cam.LoadXMPFromFile(@"<?xml version=""1.0"" encoding=""utf-8""?>
                                <camera name=""_SAM1001.JPG"">
                        <calibration cameraMaker=""SAMSUNG"" cameraModel=""NX3000"" lense=""25.0 mm"" ccdwidth=""0"" w=""5472"" h=""3648"" fx=""5794.23954825"" fy=""5794.23954825"" cx=""2792.29091924"" cy=""1817.35747811"" k1=""-0.049194332605"" k2=""0.0277098399156"" p1=""0"" p2=""0"" k3=""0.12963081329"" skew=""5.00502053223e-12"" name=""""/>
                        <extrinsics>
                            <rotation>
                    -0.1526064710434736  0.02478783062041083   0.9879772326429888
                    -0.06149714385805169  -0.9979880323252579  0.01553978641167059
                    0.9863732810618633 -0.05838727668360567   0.1538235124154451
                    </rotation>
                            <translation>
                    -68.59414976387103
                    62.94846928976456
                    -7.650801008848024
                    </translation>
                        </extrinsics>
                    </camera>"
                    );

            Assert.True(cam.Calibration.Lense == 25, "XMP calibration(lense) parsing not working");

            Assert.True(cam.Rotation.GetLength(0) == cam.Rotation.GetLength(1) && cam.Rotation[0,0] == -0.1526064710434736 && cam.Rotation[2,1] == -0.05838727668360567, "XMP rotation matrix parsing not working");
            Assert.True(cam.Translation.Count() == 3 && cam.Translation[1] == 62.94846928976456, "XMP translation vector parsing not working");
    } 

    [Fact]
    public void Test_Ply()
    {

        string plyFile = File.ReadAllText("ply/SamPointCloud.ply");

        Ply ply = Ply.Parse(plyFile);

        double [] x = ply.getDoublePropertyArray("x");
        ushort [] r = ply.getUCharPropertyArray("red");

        Assert.True(ply.Header.Properties.Count == 6,"Header parsing error(properties)");
        Assert.True(ply.Header.Properties.FindIndex(prop => prop.PType == PropertyType.Unknown) < 0, "Unkown properties in header");
        Assert.True(r.Length == int.Parse(ply.Header.Element.EValue) && x.Length == int.Parse(ply.Header.Element.EValue), "Vertices number mismatch");
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
}