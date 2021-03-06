﻿using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

//using JEXEServerLib;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using TT.StockQuoteSource;
using TT.StockQuoteSource.Contracts;
using XlpApp.Helpers;

namespace XlpApp.UserControls
{
    /// <summary>
    /// Interaction logic for ChartUserControl.xaml
    /// </summary>
    public partial class ChartUserControl : UserControl
    {
        #region Public Constructors

        public ChartUserControl()
        {
            InitializeComponent();
            DataTableStocks = new DataTable();
            DataTableStocks.Columns.Add("Stock");
            DataColumn[] keys = new DataColumn[1];
            keys[0] = DataTableStocks.Columns[0];
            DataTableStocks.PrimaryKey = keys;

            end_date.SelectedDate = DateTime.Now;
            DataContext = this;
            SeriesCollection = new SeriesCollection();
            //SeriesCollection = new SeriesCollection
            //{
            //    new LineSeries
            //    {
            //        Title = "Series 1",
            //        Values = new ChartValues<double> { 4, 6, 5, 2 ,4 }
            //    },
            //    new LineSeries
            //    {
            //        Title = "Series 2",
            //        Values = new ChartValues<double> { 6, 7, 3, 4 ,6 },
            //        PointGeometry = null
            //    },
            //    new LineSeries
            //    {
            //        Title = "Series 3",
            //        Values = new ChartValues<double> { 4,2,7,2,7 },
            //        PointGeometry = DefaultGeometries.Square,
            //        PointGeometrySize = 15
            //    }
            //};

            //Labels = new List<string> { "Jan", "Feb", "Mar", "Apr", "May" };
            //YFormatter = value => value.ToString("C");

            ////modifying the series collection will animate and update the chart
            //SeriesCollection.Add(new LineSeries
            //{
            //    Title = "Series 4",
            //    Values = new ChartValues<double> { 5, 3, 2, 4 },
            //    LineSmoothness = 0, //0: straight lines, 1: really smooth lines
            //    PointGeometry = Geometry.Parse("m 25 70.36218 20 -28 -20 22 -8 -6 z"),
            //    PointGeometrySize = 50,
            //    PointForeground = Brushes.Gray
            //});

            ////modifying any series values will also animate and update the chart
            //SeriesCollection[3].Values.Add(5d);
        }

        #endregion Public Constructors

        #region Public Properties

        public DataTable DataTableStocks { get; set; }
        public List<string> Labels { get; set; }
        public SeriesCollection SeriesCollection { get; set; }
        public Func<double, string> YFormatter { get; set; }

        #endregion Public Properties

        #region Eventhandlers

        private void btnAddStock_Click(object sender, RoutedEventArgs e)
        {
            addStockPopup.IsOpen = true;
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                DataTableStocks = ParseCVSFile.ConvertCSVtoDataTable(openFileDialog.FileName);
                addStockPopup.IsOpen = false;
                LoadData(DataTableStocks);
                UpdateChart(StockParser.GetChartDataGeared(DataTableStocks));
                //var data = ParseCVSFile.UploadReadFile(openFileDialog.FileName);
                //DataTableStocks.Rows.Add(data.Values);
                //UpdateDataTable(data);
            }

            //txtEditor.Text = File.ReadAllText(openFileDialog.FileName);
        }


        //private void UpdateChart(IReadOnlyList<IStockQuoteFromDataSource> stockQuoteFromDataSources)
        //{
        //    SeriesCollection.Clear();
        //    var series = StockParser.GetOhlcChartData(stockQuoteFromDataSources);

        //    SeriesCollection.AddRange(series.ValueSeries);
        //    Chart1.AxisX[0].Labels = series.Labels;

        //}

        private void AddToChart(CartesianChart cartesianChart, SeriesCollection seriesCollection, (SeriesCollection ValueSeries, List<string> Labels) chartData)
        {
            if (cartesianChart.Series.Count > 0)
            {
                seriesCollection[0].Values.Add(chartData.ValueSeries);
            }
            else
            {
                
                seriesCollection.AddRange(chartData.ValueSeries);
                cartesianChart.AxisX[0].Labels = chartData.Labels;
                cartesianChart.Series[0].ScalesYAt = 0;
            }
        }

        private async void btnPopupAddStock_Click(object sender, RoutedEventArgs e)
        {
            //SeriesCollection.Clear();
            //Get Stock Data

            //StockQuoteTask._country = Country.USA;
            //StockQuoteTask._config = StockQuoteTask.GetConfiguration();
            //StockQuoteTask._provider = new StockQuoteSourceProvider(StockQuoteTask._config, StockQuoteTask._country);

            string stockId = txt_stock.Text;
            Console.WriteLine($"Getting the most recent data of {stockId}");
            Console.WriteLine("Yahoo Finance:");

            addStockPopup.IsOpen = false;

            //var tmp =
             //   await StockQuoteTask.GetFromYahooSourceAsList(stockId, start_date.SelectedDate.Value, end_date.SelectedDate.Value);
            //UpdateChart(tmp);

            var previousCursor = Cursor;
            Cursor = Mouse.OverrideCursor;
            Mouse.OverrideCursor = Cursors.Wait;



            DataTableStocks = await StockQuoteTask
                .RunYahooSource(stockId, start_date.SelectedDate.Value, end_date.SelectedDate.Value, DataTableStocks);

            LoadData(DataTableStocks);

            //dataSet = new DataSet();
            //dataSet.Tables.Add(DataTableStocks);
            //DataView dv = dataSet.Tables[0].DefaultView;

            //CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(dv);
            //view.SortDescriptions.Add(new SortDescription("TradeDateTime", ListSortDirection.Descending));
            //lst_stocks.DataContext = dv;

            UpdateChart(StockParser.GetChartDataGeared(DataTableStocks));

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
            UpdateChart(StockParser.GetChartDataGeared(DataTableStocks));

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

            #endregion JEXESERVERLIB
        }

        private void btnUppload_Click(object sender, RoutedEventArgs e)
        {
        }

        #endregion Eventhandlers

        #region Helper methods

        public void StocksToDataTable(DataTable dt)
        {
        }

        //private void AppendChartValues((SeriesCollection ValueSeries, List<string> Labels) dataForChart)
        //{
        //    SeriesCollection.AddRange(dataForChart.ValueSeries);
        //    foreach (var item in Labels)
        //    {
        //        Chart1.AxisX[0].Labels.Add(item);
        //    }
        //}

        private void LoadData(DataTable TestTable)
        {
            int n = 0;
            var testoc = new ObservableCollection<object>();
            foreach (var row in TestTable.Rows)
            {
                n = ((System.Data.DataRow)(row)).ItemArray.Length;

                testoc.Add(((System.Data.DataRow)(row)).ItemArray);
            }

            //Auto-generate Columns with binding
            StockDataGrid.Columns.Clear();
            for (int i = 0; i < n; i++)
            {
                StockDataGrid.Columns.Add
                    (
                    new DataGridTextColumn() { Header = TestTable.Columns[i].ColumnName, Binding = new Binding(".[" + i.ToString() + "]") }
                    );
            }
            StockDataGrid.ItemsSource = testoc;

            //this.DataContext = testoc;
        }

        private void UpdateChart((SeriesCollection ValueSeries, List<string> Labels) dataForChart)
        {
            //updates chart
            SeriesCollection.Clear();
            SeriesCollection.AddRange(dataForChart.ValueSeries);
            Chart1.AxisX[0].Labels = dataForChart.Labels;
        }

        private void UpdateDataTable(Dictionary<string, CsvData> dataFromFile)
        {
            DateTime dateTime = end_date.DisplayDate;
            var testoc = new ObservableCollection<object>();
            int n = 0;
            List<string> keyList = new List<string>(dataFromFile.Keys);
            string stockId = keyList[0].ToUpper();
            int predictionId = 1;
            foreach (var item in dataFromFile["Prediction"].Values)
            {
                if (item != 0)
                {
                    dateTime = dateTime.AddDays(1);
                    DataTableStocks.Columns.Add("Prediction " + predictionId);
                    predictionId++;
                    DataRow dr = DataTableStocks.Rows.Find(stockId);
                    dr[DataTableStocks.Columns.Count - 1] = item;
                }
            }
            foreach (var row in DataTableStocks.Rows)
            {
                n = ((System.Data.DataRow)(row)).ItemArray.Length;
                testoc.Add(((System.Data.DataRow)(row)).ItemArray);
            }
            //Auto-generate Columns with binding
            StockDataGrid.Columns.Clear();
            for (int i = 0; i < n; i++)
            {
                StockDataGrid.Columns.Add
                    (
                    new DataGridTextColumn() { Header = DataTableStocks.Columns[i].ColumnName, Binding = new Binding(".[" + i.ToString() + "]") }
                    );
            }
            StockDataGrid.ItemsSource = testoc;
        }

        #endregion Helper methods
    }
}