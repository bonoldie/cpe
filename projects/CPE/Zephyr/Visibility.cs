using System;
using System.Collections.Generic;
using System.IO;
using CPE.Utils;

namespace CPE.Zephyr
{
    public class Visibility
    {

        string VisibilityFilePath;

        public VisibilityFile visibilityFile;

        public Visibility()
        {

        }

        public VisibilityMap GetVisibilityMapByCameraName(string CameraName) {
            return visibilityFile.VisibilityMaps[CameraName];
        }
        
        /// <summary>
        /// Method <c>LoadVisibilityFromFile</c> loads the Zephyr point cloud
        /// </summary>
        public void LoadVisibilityFromFile(string _VisibilityFilePath)
        {
            VisibilityFilePath = _VisibilityFilePath;

            visibilityFile = VisibilityFile.Parse(File.ReadAllText(_VisibilityFilePath));
        }

        // TODO: overload LoadVisibilityFromFile function with StreamReader
    }
}
