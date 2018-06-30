using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
        public List<string> Columns { get; set; } = new List<string>();
    }
    class ParseCVSFile
    {
        public static Dictionary<string, CsvData> ReadFile(string filePath)
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

                    for (int i = 2; i < fields.Length; i++)
                    {
                        csvData.Values.Add(Convert.ToDecimal(fields[i]));
                    }
                    data.Add(fields[0], csvData);
                }
            }

            return data;
        }

        public static Dictionary<string, CsvData> UploadReadFile(string filePath)
        {
            Dictionary<string, CsvData> data = new Dictionary<string, CsvData>();
            char[] delimiters = new char[] { ',' };
            using (StreamReader reader = new StreamReader(filePath))
            {
                while (true)
                {

                    string line = reader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }
                    string[] parts = line.Split(delimiters);
                    var csvData = new CsvData();
                    for (int i = 1; i < parts.Length; i++)
                    {
                        if (parts[i] != "")
                        {
                            try
                            {
                                csvData.Values.Add(Convert.ToDecimal(parts[i]));
                            }
                            catch
                            {
                                csvData.Columns.Add(parts[i]);
                            }
                        }
                        csvData.Values.Add(Convert.ToDecimal(parts[i]));
                    }
                    data.Add(parts[0], csvData);
                    // Console.WriteLine("{0} field(s)", parts.Length);
                }
                return data;
            }
        }

        public static DataTable ConvertCSVtoDataTable(string strFilePath)
        {
            DataTable dt = new DataTable();
            using (StreamReader sr = new StreamReader(strFilePath))
            {
                string[] headers = sr.ReadLine().Split(',');
                foreach (string header in headers)
                {
                    dt.Columns.Add(header);
                }
                while (!sr.EndOfStream)
                {
                    string[] rows = sr.ReadLine().Split(',');
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        dr[i] = rows[i];
                    }
                    dt.Rows.Add(dr);
                }

            }

            return dt;
        }

            //Dictionary<string, CsvData> data = new Dictionary<string, CsvData>();

            //using (TextFieldParser parser = new TextFieldParser(filePath))
            //{
            //    parser.TextFieldType = FieldType.Delimited;
            //    parser.SetDelimiters(",");
            //    while (!parser.EndOfData)
            //    {
            //        string[] fields = parser.ReadFields();
            //        var csvData = new CsvData();

            //        for (int i = 1; i < fields.Length; i++)
            //        {
            //            csvData.Values.Add(Convert.ToDecimal(fields[i]));
            //        }
            //        data.Add(fields[0], csvData);
            //    }
            //}

            //return data;
        
    }
}