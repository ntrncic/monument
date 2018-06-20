using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TT.StockQuoteSource;
using TT.StockQuoteSource.Contracts;
using System.Windows.Controls;
using System.Windows.Data;
using System.IO;
using FastMember;
using System.ComponentModel;



namespace XlpApp
{
    public class StockQuoteTask
    {
        public static IStockQuoteProvider _provider { get; set; }
        public static IConfiguration _config { get; set; }
        public static Country _country { get; set; }

        public static IConfiguration GetConfiguration()
        {
            IConfigurationBuilder configBuilder = new ConfigurationBuilder()
                                                      .SetBasePath(Environment.CurrentDirectory)
                                                      .AddJsonFile("StockQuoteSourceConfig.json");

            return configBuilder.Build();
        }

        public static IStockQuoteDataSource GetYahooDataSource(IStockQuoteProvider provider)
        {
            return provider.GetStockDataSources().FirstOrDefault(a => a.Source == StockQuoteSource.Yahoo);
        }

        public static IStockQuoteDataSource GetAlphaVantageDataSource(IStockQuoteProvider provider)
        {
            return provider.GetStockDataSources().FirstOrDefault(a => a.Source == StockQuoteSource.AlphaVantage);
        }

         //just for tests
        public static async Task<IReadOnlyList<IStockQuoteFromDataSource>> GetFromYahooSourceAsList(string stockId, DateTime start, DateTime end)
        {
            _country = Country.USA;
            _config = GetConfiguration();
            _provider = new StockQuoteSourceProvider(_config, _country);
         

            IStockQuoteDataSource yahooDataSource = GetYahooDataSource(_provider);

            if (yahooDataSource == null)
            {
                Console.WriteLine("Error : Yahoo data source object is null");
                return new List<IStockQuoteFromDataSource>();
            }

            var results = await yahooDataSource
                .GetHistoricalQuotesAsync(_country, stockId, start, end, WriteToError);

            return results;
        }

        public static async Task<DataTable> RunYahooSource(string stockId, DateTime start, DateTime end)
        {
            IStockQuoteDataSource yahooDataSource = GetYahooDataSource(_provider);

            if (yahooDataSource == null)
            {
                Console.WriteLine("Error : Yahoo data source object is null");
                return new DataTable();
            }

            IReadOnlyList<IStockQuoteFromDataSource> results = await yahooDataSource.GetHistoricalQuotesAsync(_country, stockId, start, end, WriteToError);

            DataTable DtStocks = new DataTable();
            DtStocks.Columns.Add("TradeDateTime", typeof(String));
            DtStocks.Columns.Add("OpenPrice", typeof(String));
            DtStocks.Columns.Add("ClosePrice", typeof(Double));
            DtStocks.Columns.Add("HighPrice", typeof(String));
            DtStocks.Columns.Add("LowPrice", typeof(String));
            DtStocks.Columns.Add("Volume", typeof(Double));

            foreach (IStockQuoteFromDataSource quote in results)
            {
                DtStocks.Rows.Add(new Object[] { quote.TradeDateTime, quote.OpenPrice, quote.ClosePrice, quote.HighPrice, quote.LowPrice, quote.Volume });
            }

            PrintToCsv(results);
            //foreach (IStockQuoteFromDataSource data in results)
            //{
            //    PrintQuote(data);
            //}

            //IStockQuoteFromDataSource quote = await yahooDataSource.GetMostRecentQuoteAsync(_country, stockId, WriteToError).ConfigureAwait(false);

            //if (quote != null)
            //{
            //    PrintQuote(quote);
            //}
            return DtStocks;
        }

        public static async Task RunAlphaVantageSource(string stockId)
        {
            IStockQuoteDataSource alphaVantageDataSource = GetAlphaVantageDataSource(_provider);

            if (alphaVantageDataSource == null)
            {
                Console.WriteLine("Error : Yahoo data source object is null");
                return;
            }

            IStockQuoteFromDataSource quote = await alphaVantageDataSource.GetMostRecentQuoteAsync(_country, stockId, WriteToError).ConfigureAwait(false);

            if (quote != null)
            {
                PrintQuote(quote);
            }
        }

        public static void PrintQuote(IStockQuoteFromDataSource quote)
        {
            if (quote == null)
            {
                return;
            }

            Console.WriteLine("Date: " + quote.TradeDateTime);
            Console.WriteLine("Open: " + quote.OpenPrice);
            Console.WriteLine("Close: " + quote.ClosePrice);
            Console.WriteLine("High: " + quote.HighPrice);
            Console.WriteLine("Low: " + quote.LowPrice);
            Console.WriteLine("Volume: " + quote.Volume);
            Console.WriteLine();
        }

        public static void PrintToCsv(IReadOnlyList<IStockQuoteFromDataSource> results)
        {
            string filePath = System.AppDomain.CurrentDomain.BaseDirectory + "/Algos/" + results[0].StockId.ToLower() + ".csv";
            var csv = new StringBuilder();

            //using (var reader = ObjectReader.Create(results, "Date", "Open", "Close", "High", "Low", "Volume"))
            //{
            //    pg2.dt_stocks.Load(reader);
            //}
            //pg2.grid_stock_data.DataContext = pg2.dt_stocks.DefaultView;
            csv.AppendLine(string.Format("{0},{1},{2},{3},{4},{5}","Date", "Open", "Close", "High", "Low", "Volume"));

            foreach (IStockQuoteFromDataSource quote in results)
            {
                var newLine = string.Format("{0},{1},{2},{3},{4},{5}", quote.TradeDateTime, quote.OpenPrice, quote.ClosePrice, quote.HighPrice, quote.LowPrice, quote.Volume);
                csv.AppendLine(newLine);
            }
            File.WriteAllText(filePath, csv.ToString());
        }

        static void WriteToError(Exception ex)
        {
            if (ex != null)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);
                    Console.WriteLine(ex.StackTrace);
                }
                Console.WriteLine();
            }
        }
    }
}
