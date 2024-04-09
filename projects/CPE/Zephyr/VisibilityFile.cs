using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CPE.Zephyr
{

    public struct VisibilityHeader
    {
        public string CameraName;
        public int PointsCount;
    }


    public class VisibilityMap
    {
        public int[] PointIndexes;
        public float[,] Data;
        public VisibilityHeader Header;
        public VisibilityMap(int[] _PointIndexes, float[,] _Data, VisibilityHeader _Header)
        {
            PointIndexes = _PointIndexes;
            Data = _Data;
            Header = _Header;
        }
    }

    public class VisibilityFile
    {
        public Dictionary<string, VisibilityMap> VisibilityMaps = new Dictionary<string, VisibilityMap>();

        public VisibilityFile(List<VisibilityMap> VisibilityMapsList)
        {
            foreach (VisibilityMap visibilityMap in VisibilityMapsList)
            {
                VisibilityMaps.Add(visibilityMap.Header.CameraName, visibilityMap);
            }
        }

        public static VisibilityFile Parse(string VisibilityFileContent)
        {
            List<VisibilityMap> _VisibilityMaps = new List<VisibilityMap>();

            string[] Blocks = Regex.Split(VisibilityFileContent, "Visibility for camera ");


            foreach (string Block in Blocks.Skip(1))
            {
                int[] pointsIndexes = { };
                VisibilityHeader header = new VisibilityHeader();

                string[] rows = Block.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

                header.CameraName = rows[0];
                header.PointsCount = int.Parse(rows[1]);
                float[,] data = new float[header.PointsCount, 2];

                foreach (var row in rows.Skip(2).Select((value, index) => new { value, index }))
                {
                    string[] rowElements = row.value.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    pointsIndexes.Append(int.Parse(rowElements[0]));

                    data[row.index, 0] = float.Parse(rowElements[1]);
                    data[row.index, 1] = float.Parse(rowElements[2]);
                }

                _VisibilityMaps.Add(new VisibilityMap(pointsIndexes, data, header));
            }

            VisibilityFile vis = new VisibilityFile(_VisibilityMaps);

            return vis;
        }
    }
}