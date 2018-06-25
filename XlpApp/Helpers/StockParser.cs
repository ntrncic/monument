using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TT.StockQuoteSource.Contracts;

namespace XlpApp.Helpers
{
    class StockParser
    {

        private static SeriesCollection Series
        {
            get
            {
                return new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Open",
                        Values = new ChartValues<double>()
                    },
                    new LineSeries
                    {
                        Title = "Close",
                        Values = new ChartValues<double>()
                    },
                    new LineSeries
                    {
                        Title = "High",
                        Values = new ChartValues<double>()
                    },
                    new LineSeries
                    {
                        Title = "Low",
                        Values = new ChartValues<double>()
                    }
                };
            }
        }

        public static (SeriesCollection ValueSeries, List<string> Labels) GetChartData(IReadOnlyList<IStockQuoteFromDataSource> stockQuoteFromDataSources)
        {
            List<string> labels = new List<string>();

            SeriesCollection series = Series;

            foreach (var item in stockQuoteFromDataSources)
            {
                series[0].Values.Add(Convert.ToDouble(item.OpenPrice));
                series[1].Values.Add(Convert.ToDouble(item.ClosePrice));
                series[2].Values.Add(Convert.ToDouble(item.HighPrice));
                series[3].Values.Add(Convert.ToDouble(item.LowPrice));

                labels.Add(item.TradeDateTime.Date.ToString());
            }

            return (series, labels);
        }

        public static (SeriesCollection ValueSeries, List<string> Labels) GetChartData(DataTable stockQuoteFromDataSources)
        {
            List<string> labels = new List<string>();

            SeriesCollection series = Series;

            foreach (DataRow item in stockQuoteFromDataSources.Rows)
            {
                series[0].Values.Add(Convert.ToDouble(item[1].ToString()));
                series[1].Values.Add(Convert.ToDouble(item[2].ToString()));
                series[2].Values.Add(Convert.ToDouble(item[3].ToString()));
                series[3].Values.Add(Convert.ToDouble(item[4].ToString()));

                labels.Add(item[0].ToString());
            }

            return (series, labels);
        }

        public static (SeriesCollection ValueSeries, List<string> Labels) GetChartData(Dictionary<string, CsvData> dataFromFile, int numberOfDays, DateTime lastDay)
        {
            List<string> labels = new List<string>();

            Series series = new LineSeries
            {
                Title = "Prediction",
                Values = new ChartValues<double>()
            };

            for (int i = 0; i<numberOfDays; i++)
            {
                series.Values.Add(Convert.ToDouble(0));
            }
            DateTime dateTime = lastDay;

            foreach (var item in dataFromFile["Prediction"].Values)
            {
                if (item != 0)
                {
                    series.Values.Add(Convert.ToDouble(item.ToString()));
                    lastDay = lastDay.AddDays(1);
                    labels.Add(lastDay.ToShortDateString());
                }

                //series[1].Values.Add(Convert.ToDouble(item[2].ToString()));
                //series[2].Values.Add(Convert.ToDouble(item[3].ToString()));
                //series[3].Values.Add(Convert.ToDouble(item[4].ToString()));

            }

            return (new SeriesCollection { series }, labels);
        }
    }
}