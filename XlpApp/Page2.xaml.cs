using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Data;

//using JEXEServerLib;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TT.StockQuoteSource;
using TT.StockQuoteSource.Contracts;
using XlpApp.Helpers;

namespace XlpApp
{
    /// <summary>
    /// Interaction logic for Page2.xaml
    /// </summary>
    public partial class Page2 : Page
    {
        #region Private Fields

        private DataSet dataSet;

        #endregion Private Fields

        #region Public Constructors

        public Page2()
        {
            InitializeComponent();
            end_date.SelectedDate = DateTime.Now;

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

            Labels = new List<string> { "Jan", "Feb", "Mar", "Apr", "May" };
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

        #endregion Public Constructors

        #region Eventhandlers

        private void btnAddStock_Click(object sender, RoutedEventArgs e)
        {
            addStockPopup.IsOpen = true;
        }

        private async void btnPopupAddStock_Click(object sender, RoutedEventArgs e)
        {
            //Get Stock Data
            StockQuoteTask._country = Country.USA;
            StockQuoteTask._config = StockQuoteTask.GetConfiguration();
            StockQuoteTask._provider = new StockQuoteSourceProvider(StockQuoteTask._config, StockQuoteTask._country);
            string stockId = txt_stock.Text;
            Console.WriteLine($"Getting the most recent data of {stockId}");
            Console.WriteLine("Yahoo Finance:");

            addStockPopup.IsOpen = false;

            var previousCursor = Cursor;
            Cursor = Mouse.OverrideCursor;
            Mouse.OverrideCursor = Cursors.Wait;

            DataTableStocks = await StockQuoteTask
                .RunYahooSource(stockId, start_date.SelectedDate.Value, end_date.SelectedDate.Value);
            dataSet = new DataSet();
            dataSet.Tables.Add(DataTableStocks);
            DataView dv = dataSet.Tables[0].DefaultView;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(dv);
            view.SortDescriptions.Add(new SortDescription("TradeDateTime", ListSortDirection.Descending));
            lst_stocks.DataContext = dv;
            UpdateChart(StockParser.GetChartData(DataTableStocks));

            Mouse.OverrideCursor = previousCursor;
        }

        private void btnPopupExit_Click(object sender, RoutedEventArgs e)
        {
            addStockPopup.IsOpen = false;
        }

        private async void btnRun_Click(object sender, RoutedEventArgs e)
        {
            Process b_process = new Process();
            b_process.StartInfo.FileName = "test.bat";
            b_process.StartInfo.CreateNoWindow = true;
            string path = Environment.CurrentDirectory + @"\Algos";
            b_process.StartInfo.WorkingDirectory = path;
            b_process.Start();

            System.Threading.Thread.Sleep(1000);
            var data = ParseCVSFile.ReadFile(Environment.CurrentDirectory + @"\Algos\out.csv");

            UpdateDataTable(data);
            UpdateChart(StockParser.GetChartData(DataTableStocks));

            //AppendChartValues(StockParser.GetChartData(data, SeriesCollection[0].Values.Count, end_date.DisplayDate));


            ////Process process = new Process();
            ////process.StandardInput.Write(@"load 'C:\JFiles\hello.ijs'");
            //ProcessStartInfo psi = new ProcessStartInfo
            //{
            //    FileName = "jconsole.cmd",
            //    WorkingDirectory = @"C:\Users\ntrncic\j64-806",
            //    RedirectStandardInput = true,
            //    UseShellExecute = false,

            //    Arguments = @"/k load 'C:\JFiles\hello.ijs'",
            //    CreateNoWindow = false
            //};
            //var prc = Process.Start(psi);
            //prc.StandardInput.WriteLine(@"/k load 'C:\JFiles\hello.ijs'");


            #region JEXESERVERLIB



            //object result;
            //Session s = new Session();
            //s.Load("/Resources/script.ijs");

            //JEXEServerClass js = new JEXEServerClass();

            //            int rc;  // return code, 0 = success
            //            // rc = js.Quit();       // uncomment to close IDE automatically
            //            rc = js.Show(1);       // show Term
            //            rc = js.Log(1);        // log input
            //            string script = "1!:1 < 'C:\\JFiles\\script.ijs'";
            //            script =
            //@"load 'csv'
            //IBMPATH =: 'C:\JFiles\ibm.csv'
            //IBMPATH2 =: 'C:\JFiles\ibmout.csv'
            //dat =: readcsv IBMPATH
            //stock =:> "".&.>}.4{|:dat
            //pred =: stock , 140 +? 10#20
            //out =: stock ,: pred
            //out writecsv IBMPATH2
            //";

            //            try
            //            {
            //                // Execute the command
            //                rc = js.DoR(script, out result);
            //                Console.WriteLine(
            //                string.Format("J DoR ended with status {0} and result\n{1}",
            //                    rc, result));

            //                if (rc > 0)
            //                {
            //                    // Throw the correct error message
            //                    object errorMessage;
            //                    jObject.ErrorTextB(rc, out errorMessage);
            //                    Exception eoe = new Exception(Convert.ToString(errorMessage));
            //                    throw eoe;
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                throw ex;
            //            }

            //rc = js.DoR(script, out result);

            //Console.WriteLine(
            //string.Format("J DoR ended with status {0} and result\n{1}",
            //    rc, result));

            //StockQuoteTask._country = Country.USA;
            //StockQuoteTask._config = StockQuoteTask.GetConfiguration();
            //StockQuoteTask._provider = new StockQuoteSourceProvider(StockQuoteTask._config, StockQuoteTask._country);
            //string stockId = "SPY";
            //Console.WriteLine($"Getting the most recent data of {stockId}");

            //Console.WriteLine("Yahoo Finance:");

            //await StockQuoteTask.RunYahooSource(stockId, this.start_date.SelectedDate.Value, this.end_date.SelectedDate.Value);

            #endregion ???
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        #endregion Eventhandlers

        #region Public Properties

        public DataTable DataTableStocks { get; set; }
        public List<string> Labels { get; set; }
        public SeriesCollection SeriesCollection { get; set; }
        public Func<double, string> YFormatter { get; set; }

        #endregion Public Properties

        #region Helper methods

        public void StocksToDataTable(DataTable dt)
        {
        }

        private void UpdateDataTable(Dictionary<string, CsvData> dataFromFile)
        {
            DateTime dateTime = end_date.DisplayDate;
            foreach (var item in dataFromFile["Prediction"].Values)
            {
                if (item != 0)
                {
                    dateTime = dateTime.AddDays(1);
                    DataRow dr = DataTableStocks.Rows[DataTableStocks.Rows.Count-1];
                    DataTableStocks.Rows.Add(
                        new Object[]
                        {
                            dateTime.ToShortDateString(),
                            dr["OpenPrice"],
                            item,
                            dr["HighPrice"],
                            dr["LowPrice"],
                            dr["Volume"]
                        });
                }
            }

        }

        private void UpdateChart((SeriesCollection ValueSeries, List<string> Labels) dataForChart)
        {
            SeriesCollection.Clear();
            SeriesCollection.AddRange(dataForChart.ValueSeries);
            Chart1.AxisX[0].Labels = dataForChart.Labels;
        }
        //just for testing

        private void AppendChartValues((SeriesCollection ValueSeries, List<string> Labels) dataForChart)
        {
            SeriesCollection.AddRange(dataForChart.ValueSeries);
            foreach (var item in Labels)
            {
                Chart1.AxisX[0].Labels.Add(item);
            }
        }

        #endregion Helper methods

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged members
    }
}

internal class Stock
{
    #region Public Properties

    public string Name { get; set; }

    #endregion Public Properties
}
