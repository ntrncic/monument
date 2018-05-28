using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TT.StockQuoteSource;
using TT.StockQuoteSource.Contracts;
using System.Windows.Controls;
using System.Windows.Data;
using System.IO;

namespace XlpApp
{
    class StockQuoteTask
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

        public static async Task RunYahooSource(string stockId, DateTime start, DateTime end)
        {
            IStockQuoteDataSource yahooDataSource = GetYahooDataSource(_provider);

            if (yahooDataSource == null)
            {
                Console.WriteLine("Error : Yahoo data source object is null");
                return;
            }

            IReadOnlyList<IStockQuoteFromDataSource> results = yahooDataSource.GetHistoricalQuotesAsync(_country, stockId, start, end, WriteToError).Result;
            PrintToCsv(results);

            foreach (IStockQuoteFromDataSource data in results)
            {
                PrintQuote(data);
            }

            IStockQuoteFromDataSource quote = await yahooDataSource.GetMostRecentQuoteAsync(_country, stockId, WriteToError).ConfigureAwait(false);

            if (quote != null)
            {
                PrintQuote(quote);
            }
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
            string filePath = System.AppDomain.CurrentDomain.BaseDirectory + "/" + results[0].StockId +".csv";
            var csv = new StringBuilder();

            foreach (IStockQuoteFromDataSource quote in results)
            {
                var newLine = string.Format("{0},{1},{2},{3},{4},{5}", quote.TradeDateTime, quote.OpenPrice, quote.ClosePrice, quote.HighPrice, quote.LowPrice, quote.Volume);
                csv.AppendLine(newLine);
            }
            File.WriteAllText(filePath, csv.ToString());
        }

        public static void PrintGrid(IStockQuoteFromDataSource quote)
        {
            
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
