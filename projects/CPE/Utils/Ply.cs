using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CPE.Utils
{

    public enum PropertyType
    {
        Double,
        UChar,
        Unknown
    }
    public enum ElementType
    {
        Vertex,
        Unknown
    }

    public enum FormatType
    {
        ASCII_1_0,
        Unknown
    }

    public struct PlyHeader
    {
        public List<(PropertyType PType, string PValue)> Properties;
        public (ElementType EType, string EValue) Element;
        public FormatType Format;
    }

    public class Ply
    {
        private static readonly string END_HEAD_MARKER = "end_header";

        public PlyHeader Header;

        private string _data;

        public Ply(String Data, List<(PropertyType pType, string pValue)> Properties, (ElementType eType, string eValue) Element, FormatType Format)
        {
            Header = new PlyHeader
            {
                Element = Element,
                Properties = Properties,
                Format = Format,
            };

            _data = Data;

        }

        public ushort[] getUCharPropertyArray(string PValue)
        {
            int PropertyIndex = Header.Properties.FindIndex(prop => prop.PValue == PValue && prop.PType == PropertyType.UChar);

            if (PropertyIndex < 0)
            {
                throw new Exception("Property '" + PValue + "' of type 'uchar' not found");
            }

            return _data.Split("\n", StringSplitOptions.RemoveEmptyEntries).Select((row, i) => row.Split(" ")).Aggregate(new List<ushort>(), (list, row) => { list.Add(ushort.Parse(row[PropertyIndex])); return list; }).ToArray();
        }

        public double[] getDoublePropertyArray(string PValue)
        {
            int PropertyIndex = Header.Properties.FindIndex(prop => prop.PValue == PValue && prop.PType == PropertyType.Double);

            if (PropertyIndex < 0)
            {
                throw new Exception("Property '" + PValue + "' of type 'double' not found");
            }

            double[] data = _data.Split("\n", StringSplitOptions.RemoveEmptyEntries).Select((row, i) => row.Split(" ")).Aggregate(new List<double>(), (list, row) => { list.Add(double.Parse(row[PropertyIndex])); return list; }).ToArray();
            return data;
        }

        public static Ply Parse(string PlyFileContent)
        {
            string RawHeader = PlyFileContent.Split(END_HEAD_MARKER)[0];

            List<(PropertyType PType, string PValue)> Properties = new List<(PropertyType PType, string PValue)>();
            (ElementType EType, string EValue) Element = (ElementType.Unknown, "");
            FormatType Format = FormatType.Unknown;

            foreach (Match match in Regex.Matches(RawHeader, "property (double|uchar) (.*)"))
            {
                switch (match.Groups[1].Value)
                {
                    case "double":
                        Properties.Add((PropertyType.Double, match.Groups[2].Value));
                        break;
                    case "uchar":
                        Properties.Add((PropertyType.UChar, match.Groups[2].Value));
                        break;
                    default:
                        Properties.Add((PropertyType.Unknown, match.Groups[2].Value));
                        break;
                }
            }

            foreach (Match match in Regex.Matches(RawHeader, "format (.*)"))
            {
                switch (match.Groups[1].Value)
                {
                    case "ascii 1.0":
                        Format = FormatType.ASCII_1_0;
                        break;
                    default:
                        Format = FormatType.Unknown;
                        break;
                }
            }


            foreach (Match match in Regex.Matches(RawHeader, "element (vertex) (.*)"))
            {
                switch (match.Groups[1].Value)
                {
                    case "vertex":
                        Element = (ElementType.Vertex, match.Groups[2].Value);
                        break;
                    default:
                        Element = (ElementType.Unknown, match.Groups[2].Value);
                        break;
                }
            }

            Ply ply = new Ply(
                PlyFileContent.Split(END_HEAD_MARKER)[1],
                Properties,
                Element,
                Format
            );

            return ply;
        }


    }
}