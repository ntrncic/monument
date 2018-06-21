using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace XlpApp.Helpers
{
    class CsvData
    {
        public Color Color { get; set; }
        public List<decimal> Values { get; set; } = new List<decimal>();
    }
    class ParseCVSFile
    {
        public static Dictionary<string,CsvData> ReadFile(string filePath)
        {
            Dictionary<string, CsvData> data = new Dictionary<string, CsvData>();

            using (TextFieldParser parser = new TextFieldParser(filePath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    var csvData = new CsvData();
                    csvData.Color = (Color)ColorConverter.ConvertFromString(fields[1]);
                    
                    for (int i = 2; i < fields.Length;i++)
                    {
                        csvData.Values.Add(Convert.ToDecimal(fields[i]));
                    }
                    data.Add(fields[0], csvData);
                }
            }

            return data;
        }
    }
}
