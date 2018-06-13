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
using System.Windows.Data;
using System.Data;
using JEXEServerLib;

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
        private DataSet _ds;
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

        }

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }

        public DataTable dt_stocks { get; set; }

        private async void btn_run_Click(object sender, RoutedEventArgs e)
        {
            object result;
            Session s = new Session();
            //s.Load("/Resources/script.ijs");


            JEXEServerClass j = new JEXEServerClass();

            // Create instance of JEXEServer
            try
            {
                JEXEServer jobject = new JEXEServer();
                // QueryInterface for the IJEXEServer interface:
                IJEXEServer js = (IJEXEServer)jobject;
                int rc;  // return code, 0 = success
                rc = js.DoR("2| !/~i.8", out result);
                Console.WriteLine(
                string.Format("J DoR ended with status {0} and result\n{1}",
                    rc, result));
            }
            catch
            {

            }




            //StockQuoteTask._country = Country.USA;
            //StockQuoteTask._config = StockQuoteTask.GetConfiguration();
            //StockQuoteTask._provider = new StockQuoteSourceProvider(StockQuoteTask._config, StockQuoteTask._country);
            //string stockId = "SPY";
            //Console.WriteLine($"Getting the most recent data of {stockId}");

            //Console.WriteLine("Yahoo Finance:");
            //await StockQuoteTask.RunYahooSource(stockId, this.start_date.SelectedDate.Value, this.end_date.SelectedDate.Value);
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btn_plus_stock_Click(object sender, RoutedEventArgs e)
        {
            add_stock_popup.IsOpen = true;
        }

        private async void btn_add_Click(object sender, RoutedEventArgs e)
        {
            //Get Stock Data
            StockQuoteTask._country = Country.USA;
            StockQuoteTask._config = StockQuoteTask.GetConfiguration();
            StockQuoteTask._provider = new StockQuoteSourceProvider(StockQuoteTask._config, StockQuoteTask._country);
            string stockId = txt_stock.Text;
            Console.WriteLine($"Getting the most recent data of {stockId}");
            Console.WriteLine("Yahoo Finance:");

            dt_stocks = StockQuoteTask.RunYahooSource(stockId, start_date.SelectedDate.Value, end_date.SelectedDate.Value).Result;
            _ds = new DataSet();
            _ds.Tables.Add(dt_stocks);
            lst_stocks.DataContext = _ds.Tables[0].DefaultView;


            add_stock_popup.IsOpen = false;

            //populate grid
            // Add columns
            //gridView.Columns.Add(new GridViewColumn
            //{
            //    Header = "Id",
            //    DisplayMemberBinding = new Binding("Id")
            //});
            //gridView.Columns.Add(new GridViewColumn
            //{
            //    Header = "Name",
            //    DisplayMemberBinding = new Binding("Name")
            //});

            //// Populate list
            //this.lstv_stock_data.Items.Add(new MyItem { Id = 1, Name = "David" });
        }

        private void btn_add_exit_Click(object sender, RoutedEventArgs e)
        {
            add_stock_popup.IsOpen = false;
        }

        public void StocksToDataTable(DataTable dt)
        {
            
        }
    }

}
