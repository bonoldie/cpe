using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Providers.LinearAlgebra;
using NLog.LayoutRenderers;

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

        private Dictionary<(PropertyType PType, string PValue), MathNet.Numerics.LinearAlgebra.Vector<double>> DoubleData = new Dictionary<(PropertyType PType, string PValue),  MathNet.Numerics.LinearAlgebra.Vector<double>>();
        private Dictionary<(PropertyType PType, string PValue),  MathNet.Numerics.LinearAlgebra.Vector<float>> FloatData = new Dictionary<(PropertyType PType, string PValue),  MathNet.Numerics.LinearAlgebra.Vector<float>>();
    
        public Ply(string Data, List<(PropertyType pType, string pValue)> Properties, (ElementType eType, string eValue) Element, FormatType Format)
        {
            Header = new PlyHeader
            {
                Element = Element,
                Properties = Properties,
                Format = Format,
            };  

          
            string[] Rows = Data.Split("\n", StringSplitOptions.RemoveEmptyEntries);

              foreach(var Property in Properties) {
                if(Property.pType == PropertyType.Double) {
                    DoubleData[Property] = CreateVector.Dense(new double[Rows.Length]);
                }
                if(Property.pType == PropertyType.UChar) {
                    FloatData[Property] = CreateVector.Dense(new float[Rows.Length]);
                }
            }



            foreach(var Row in Rows.Select((Row, Index) => new {RowValues=Row.Split(" "), RowIndex=Index}) ) {
                foreach(var RowVal in Row.RowValues.Select((RowValue, RowValueIndex) => new{RowValue,RowValueIndex,RowIndex=Row.RowIndex})){
                    if(Properties[RowVal.RowValueIndex].pType == PropertyType.Double) {

                        DoubleData[Properties[RowVal.RowValueIndex]][RowVal.RowIndex] = double.Parse(RowVal.RowValue);
                    } else  if(Properties[RowVal.RowValueIndex].pType == PropertyType.UChar) {
                        FloatData[Properties[RowVal.RowValueIndex]][RowVal.RowIndex] = float.Parse(RowVal.RowValue);
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

            return  FloatData[Header.Properties[PropertyIndex]];
        }

        public MathNet.Numerics.LinearAlgebra.Vector<double> GetDoubleVector(string PValue)
        {
            int PropertyIndex = Header.Properties.FindIndex(prop => prop.PValue == PValue && prop.PType == PropertyType.Double);

            if (PropertyIndex < 0)
            {
                throw new Exception("Property '" + PValue + "' of type 'double' not found");
            }

              return  DoubleData[Header.Properties[PropertyIndex]];
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