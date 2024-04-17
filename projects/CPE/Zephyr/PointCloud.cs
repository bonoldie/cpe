using System.Collections.Generic;
using System.IO;
using CPE.Utils;

namespace CPE.Zephyr
{
    public class PointCloud
    {

        string PlyFilePath;

        public Ply ply;

        public Dictionary<string, Visibility> VisibilityMaps;

        public PointCloud()
        {
        }

        /// <summary>
        /// Method <c>LoadPlyFromFile</c> loads the Zephyr point cloud
        /// </summary>
        public void LoadPlyFromFile(string _PlyFilePath)
        {
            PlyFilePath = _PlyFilePath;

            ply = Ply.Parse(File.ReadAllText(_PlyFilePath));
        }
        // TODO: overload LoadPlyFromFile function with StreamReader
    }
}
