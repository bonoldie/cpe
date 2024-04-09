using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace CPE.Zephyr
{

    public class Camera
    {
        public XMP XMPFile;

        public Camera()
        {
        }

        public void LoadXMPFromFile(string XMPFilePath) {
            this.XMPFile = XMP.Parse(File.ReadAllText(XMPFilePath));
        }
    }
}