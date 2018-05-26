using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Extensions.Configuration;
using TT.StockQuoteSource;
using TT.StockQuoteSource.Contracts;
using System.Threading.Tasks;


class Stock
{
    public string Name { get; set; }
}


namespace XlpApp
{
    /// <summary>
    /// Interaction logic for Page2.xaml
    /// </summary>
    public partial class Page2 : Page
    {
        public Page2()
        {
            InitializeComponent();

            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Series 1",
                    Values = new ChartValues<double> { 4, 6, 5, 2 ,4 }
                },
                new LineSeries
                {
                    Title = "Series 2",
                    Values = new ChartValues<double> { 6, 7, 3, 4 ,6 },
                    PointGeometry = null
                },
                new LineSeries
                {
                    Title = "Series 3",
                    Values = new ChartValues<double> { 4,2,7,2,7 },
                    PointGeometry = DefaultGeometries.Square,
                    PointGeometrySize = 15
                }
            };

            Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May" };
            YFormatter = value => value.ToString("C");

            //modifying the series collection will animate and update the chart
            SeriesCollection.Add(new LineSeries
            {
                Title = "Series 4",
                Values = new ChartValues<double> { 5, 3, 2, 4 },
                LineSmoothness = 0, //0: straight lines, 1: really smooth lines
                PointGeometry = Geometry.Parse("m 25 70.36218 20 -28 -20 22 -8 -6 z"),
                PointGeometrySize = 50,
                PointForeground = Brushes.Gray
            });

            //modifying any series values will also animate and update the chart
            SeriesCollection[3].Values.Add(5d);

            DataContext = this;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ListView1.Items.Add(new Stock() { Name = "" });
            ListView1.Items.Add(new Stock() { Name = "" });
            ListView1.Items.Add(new Stock() { Name = "" });
            ListView1.Items.Add(new Stock() { Name = "" });
        }


        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }

        private async void btn_run_Click(object sender, RoutedEventArgs e)
        {
            StockQuoteTask._country = Country.USA;
            StockQuoteTask._config = StockQuoteTask.GetConfiguration();
            StockQuoteTask._provider = new StockQuoteSourceProvider(StockQuoteTask._config, StockQuoteTask._country);
            string stockId = "SPY";
            Console.WriteLine($"Getting the most recent data of {stockId}");

            Console.WriteLine("Yahoo Finance:");
            await StockQuoteTask.RunYahooSource(stockId);
        }

    }

}
