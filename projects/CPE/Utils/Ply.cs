using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MathNet.Numerics.LinearAlgebra;

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
        private static readonly string END_HEADER_MARKER = "end_header";

        public PlyHeader Header;

        private Dictionary<(PropertyType PType, string PValue), MathNet.Numerics.LinearAlgebra.Vector<double>> DoubleData = new Dictionary<(PropertyType PType, string PValue), MathNet.Numerics.LinearAlgebra.Vector<double>>();
        private Dictionary<(PropertyType PType, string PValue), MathNet.Numerics.LinearAlgebra.Vector<float>> FloatData = new Dictionary<(PropertyType PType, string PValue), MathNet.Numerics.LinearAlgebra.Vector<float>>();

        private string RawData = "";

        private Dictionary<string, string> PCachePropertyNamesMap = new Dictionary<string, string>(){
            {"red", "r"},
            {"green","g"},
            {"blue","b"},
        }  ;

        public Ply(string Data, List<(PropertyType pType, string pValue)> Properties, (ElementType eType, string eValue) Element, FormatType Format)
        {
            Header = new PlyHeader
            {
                Element = Element,
                Properties = Properties,
                Format = Format,
            };

            string[] Rows = Data.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            RawData = string.Join("\n", Rows);

            foreach (var Property in Properties)
            {
                if (Property.pType == PropertyType.Double)
                {
                    DoubleData[Property] = CreateVector.Dense(new double[Rows.Length]);
                }
                if (Property.pType == PropertyType.UChar)
                {
                    FloatData[Property] = CreateVector.Dense(new float[Rows.Length]);
                }
            }

            foreach (var Row in Rows.Select((Row, Index) => new { RowValues = Row.Split(" "), RowIndex = Index }))
            {
                foreach (var RowVal in Row.RowValues.Select((RowValue, RowValueIndex) => new { RowValue, RowValueIndex, Row.RowIndex }))
                {
                    var Property = Properties[RowVal.RowValueIndex];

                    switch (Property.pType)
                    {
                        case PropertyType.Double:
                            DoubleData[Property][RowVal.RowIndex] = double.Parse(RowVal.RowValue);
                            break;
                        case PropertyType.UChar:
                            FloatData[Property][RowVal.RowIndex] = float.Parse(RowVal.RowValue);
                            break;
                        case PropertyType.Unknown:
                        default:
                            break;
                    }
                }
            }
        }

        public MathNet.Numerics.LinearAlgebra.Vector<float> GetUCharVector(string PValue)
        {
            int PropertyIndex = Header.Properties.FindIndex(prop => prop.PValue == PValue && prop.PType == PropertyType.UChar);

            if (PropertyIndex < 0)
            {
                throw new Exception("Property '" + PValue + "' of type 'uchar' not found");
            }

            return FloatData[Header.Properties[PropertyIndex]];
        }

        public MathNet.Numerics.LinearAlgebra.Vector<double> GetDoubleVector(string PValue)
        {
            int PropertyIndex = Header.Properties.FindIndex(prop => prop.PValue == PValue && prop.PType == PropertyType.Double);

            if (PropertyIndex < 0)
            {
                throw new Exception("Property '" + PValue + "' of type 'double' not found");
            }

            return DoubleData[Header.Properties[PropertyIndex]];
        }

        public static Ply Parse(string PlyFileContent)
        {
            string RawHeader = PlyFileContent.Split(END_HEADER_MARKER)[0];

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
                PlyFileContent.Split(END_HEADER_MARKER)[1],
                Properties,
                Element,
                Format
            );

            return ply;
        }


        public string ToPCacheFormat()
        {
            List<string> PCacheFileContent = new List<string>() { "pcache" };

            switch (Header.Format)
            {
                case FormatType.ASCII_1_0:
                case FormatType.Unknown:
                default:
                    PCacheFileContent.Add("format ascii 1.0");
                    break;
            }

            if (Header.Element.EType == ElementType.Vertex)
            {
                PCacheFileContent.Add("elements " + int.Parse(Header.Element.EValue));
            }

            PCacheFileContent.Add(string.Join("\n", Header.Properties.Select((Property, Index) => "property " + Property.PType.ToString().ToLower() + (Index < 3 ? " position." : " color.")  + (PCachePropertyNamesMap.ContainsKey(Property.PValue.ToString()) ? PCachePropertyNamesMap[Property.PValue.ToString()] : Property.PValue.ToString()))));

            PCacheFileContent.Add(END_HEADER_MARKER);

            PCacheFileContent.Add(RawData);

            return string.Join("\n", PCacheFileContent);
        }
    }

}