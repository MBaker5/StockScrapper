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
	}
}

