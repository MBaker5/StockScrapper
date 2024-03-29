using HtmlAgilityPack;
using StockScrapper_App.Core;
using StockScrapper_Database.Models;
using System.Diagnostics;

namespace StockScrapper_App.Services
{
    public class HtmlScrappService : IHtmlScrappService
    {
		public List<CurrencyModel> GetDataFromNBP()
		{
			string url = "https://nbp.pl/statystyka-i-sprawozdawczosc/kursy/tabela-a/";
			HtmlWeb web = new HtmlWeb();
			List<CurrencyModel> currencyList = new List<CurrencyModel>();

			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			HtmlDocument doc = web.Load(url);

			string tableSelector = "//table[@class='table table-hover table-striped table-bordered']";
			string rowSelector = "//table[@class='table table-hover table-striped table-bordered']//tbody//tr";

			HtmlNode tableNode = doc.DocumentNode.SelectSingleNode(tableSelector);
			HtmlNodeCollection? rowNodes = tableNode?.SelectNodes(rowSelector);

			if (tableNode != null && rowNodes != null)
			{
				foreach (var rowNode in rowNodes)
				{
					HtmlNodeCollection cellNodes = rowNode.SelectNodes("td");

					if (cellNodes != null && cellNodes.Count == 3)
					{
						
						string currencyCode = cellNodes[1].InnerText.Trim();
						string exchangeRate = cellNodes[2].InnerText.Trim();

						currencyList.Add(new CurrencyModel { CurrencyCode = currencyCode, ExchangeRate = exchangeRate });
					}
				}

				return currencyList;
			}
			else
			{
				Console.WriteLine("Data not found or selectors need adjustment.");
				return null;
			}

			stopwatch.Stop();
			Console.WriteLine($"Czas wykonania operacji: {stopwatch.ElapsedMilliseconds} ms");
		}


        public List<CurrencyModel> ScrapYahoo(string companyShortcut)
        {
            List<CurrencyModel> currencyList = new List<CurrencyModel>();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Example URL for historical stock data on Yahoo Finance (AAPL)
            string url = $"https://finance.yahoo.com/quote/{companyShortcut}/history?p={companyShortcut}";
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);

            // Replace these selectors with the actual ones from the Yahoo Finance page
            string dateSelector = "//td[@class='Py(10px) Ta(start) Pend(10px)']"; // Selector for date
            string openSelector = "//td[@class='Py(10px) Pstart(10px)']"; // Selector for open value
            string highSelector = "//td[@class='Py(10px) Pstart(10px)']"; // Selector for high value
            string lowSelector = "//td[@class='Py(10px) Pstart(10px)']"; // Selector for low value
            string closeSelector = "//td[@class='Py(10px) Pstart(10px)']"; // Selector for close value

            // Use the appropriate selectors to extract data
            HtmlNodeCollection dateNodes = doc.DocumentNode.SelectNodes(dateSelector);
            HtmlNodeCollection openNodes = doc.DocumentNode.SelectNodes(openSelector);
            HtmlNodeCollection highNodes = doc.DocumentNode.SelectNodes(highSelector);
            HtmlNodeCollection lowNodes = doc.DocumentNode.SelectNodes(lowSelector);
            HtmlNodeCollection closeNodes = doc.DocumentNode.SelectNodes(closeSelector);

            if (dateNodes != null && openNodes != null && highNodes != null && lowNodes != null && closeNodes != null)
            {
                for (int i = 0; i < dateNodes.Count; i++)
                {
                    string date = dateNodes[i].InnerText.Trim();
                    string open = openNodes[i].InnerText.Trim();
                    string high = highNodes[i].InnerText.Trim();
                    string low = lowNodes[i].InnerText.Trim();
                    string close = closeNodes[i].InnerText.Trim();

                    // Creating a new CurrencyModel instance and populating it with scraped data
                    CurrencyModel currency = new CurrencyModel();
                    currency.CurrencyCode = date; // Example: Using date as currency code
                    currency.ExchangeRate = open; // Example: Using open value as exchange rate

                    currencyList.Add(currency);

                    Console.WriteLine($"Date: {date}, Open: {open}, High: {high}, Low: {low}, Close: {close}");
                }
            }
            else
            {
                Console.WriteLine("Data not found or selectors need adjustment.");
            }

            stopwatch.Stop();
            Console.WriteLine($"Czas wykonania operacji: {stopwatch.ElapsedMilliseconds} ms");

            return currencyList;
        }
    }
}

