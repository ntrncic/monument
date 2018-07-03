using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using LiveCharts.Geared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TT.StockQuoteSource.Contracts;

namespace XlpApp.Helpers
{
    class StockParser
    {

        private static SeriesCollection Series { get; set; }

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

            SeriesCollection series = new SeriesCollection();

            int rownum = 0;
            int seriesNum = 0;
            foreach (DataRow item in stockQuoteFromDataSources.Rows)
            {
                series.Add(new LineSeries
                {
                    Title = item[0].ToString(),
                    Values = new ChartValues<double>()
                });
                
                for(int i=1; i < item.ItemArray.Length; i++)
                    series[seriesNum].Values.Add(Convert.ToDouble(item[i].ToString()));

                seriesNum++;
                //series[1].Values.Add(Convert.ToDouble(item[2].ToString()));
                //series[2].Values.Add(Convert.ToDouble(item[3].ToString()));
                //series[3].Values.Add(Convert.ToDouble(item[4].ToString()));  
            }

            foreach(DataColumn dc in stockQuoteFromDataSources.Columns)
            {
                if (dc.ColumnName == "Stock")
                    continue;
                labels.Add(dc.ColumnName);
            }

            return (series, labels);
        }

        public static (SeriesCollection ValueSeries, List<string> Labels) GetChartDataGeared(DataTable stockQuoteFromDataSources)
        {
            List<string> labels = new List<string>();
            Series = new SeriesCollection();

            foreach (DataRow item in stockQuoteFromDataSources.Rows) //series
            {
                var values = new double [item.ItemArray.Length-1];
                for (int i = 1; i < item.ItemArray.Length; i++)
                {
                    values[i - 1] = Convert.ToDouble(item[i].ToString());
                }
                var gseries = new GLineSeries
                {
                    Values = values.AsGearedValues().WithQuality(Quality.Low),
                    Fill = Brushes.Transparent,
                    StrokeThickness = .5,
                    PointGeometry = null //use a null geometry when you have many series
                };

                Series.Add(gseries);

            }

            foreach (DataColumn dc in stockQuoteFromDataSources.Columns)
            {
                if (dc.ColumnName == "Stock")
                    continue;
                labels.Add(dc.ColumnName);
            }

            return (Series, labels);
        }

            public static (SeriesCollection ValueSeries, List<string> Labels) GetOhlcChartData(IReadOnlyList<IStockQuoteFromDataSource> stockQuoteFromDataSources)
        {
            var stockSeries = new ChartValues<OhlcPoint>();
            var labels = new List<string>();

            foreach (IStockQuoteFromDataSource item in stockQuoteFromDataSources)
            {
                stockSeries.Add(new OhlcPoint
                {
                    Open = (double)item.OpenPrice,
                    High = (double)item.HighPrice,
                    Low = (double)item.LowPrice,
                    Close = (double)item.ClosePrice
                });
                labels.Add(item.TradeDateTime.ToShortDateString());
            }

            var seriesCol = new SeriesCollection
            {
                 new OhlcSeries()
                 {
                     Title = stockQuoteFromDataSources[0].StockId,
                     Values = stockSeries
                 },
                 new LineSeries
                 {
                     Title = stockQuoteFromDataSources[0].StockId,
                     Values = new ChartValues<double>(stockSeries.Select(x => x.Close)),
                     Fill = Brushes.Transparent
                 }
            };

            return (seriesCol, labels);
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