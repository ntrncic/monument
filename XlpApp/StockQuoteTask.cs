using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TT.StockQuoteSource;
using TT.StockQuoteSource.Contracts;

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

        public static async Task RunYahooSource(string stockId)
        {
            IStockQuoteDataSource yahooDataSource = GetYahooDataSource(_provider);

            if (yahooDataSource == null)
            {
                Console.WriteLine("Error : Yahoo data source object is null");
                return;
            }

            DateTime start = new DateTime(2018, 5, 12);
            DateTime end = new DateTime(2018, 5, 25);

            IReadOnlyList<IStockQuoteFromDataSource> results = yahooDataSource.GetHistoricalQuotesAsync(_country, stockId, start, end, WriteToError).Result;

            foreach (IStockQuoteFromDataSource data in results)
            {
                PrintQuote(data);
            }
            IStockQuoteFromDataSource quote = await yahooDataSource.GetMostRecentQuoteAsync(_country, stockId, WriteToError).ConfigureAwait(false);

            if (quote != null)
            {
                //PrintQuote(quote);
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
