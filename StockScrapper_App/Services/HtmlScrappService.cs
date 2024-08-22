using HtmlAgilityPack;
using StockScrapper_App.Core;
using StockScrapper_Database.Models;
using System;
using System.Diagnostics;
using System.Globalization;

namespace StockScrapper_App.Services
{
    public class HtmlScrappService : IHtmlScrappService
    {
		public List<string> GetMostActiveOnMarket()
		{
			try
			{
				string url = "https://finance.yahoo.com/most-active/?offset=0&count=50";
				HtmlWeb web = new HtmlWeb();
				List<string> companyShortcuts = new List<string>();
				int maxRetries = 3;

				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();

				for (int attempt = 0; attempt < maxRetries; attempt++)
				{
					try
					{
						HtmlDocument doc = web.Load(url);

						string rowSelector = "//tr[contains(@class, 'row')]//td[1]//span[contains(@class, 'symbol')]";

						HtmlNodeCollection? rowNodes = doc.DocumentNode.SelectNodes(rowSelector);

						if (rowNodes != null)
						{
							foreach (var rowNode in rowNodes)
							{
								string companyShortcut = rowNode.InnerText.Trim();
								companyShortcuts.Add(companyShortcut);
							}

							stopwatch.Stop();
							Debug.WriteLine($"Operation completed in: {stopwatch.ElapsedMilliseconds} ms");
							return companyShortcuts;
						}
						else
						{
							throw new InvalidOperationException("No company shortcuts found!");
						}
					}
					catch (Exception ex)
					{
						Debug.WriteLine($"Attempt {attempt + 1} failed: {ex.Message}. Retrying...");
					}
				}

				Debug.WriteLine("Failed to fetch data after multiple attempts.");
				return null;
			}
			catch(Exception ex)
			{
				return null;
			}
			
		}


		public List<CurrencyModel> GetDataFromNBP()
		{
			try
			{
				string url = "https://nbp.pl/statystyka-i-sprawozdawczosc/kursy/tabela-a/";
				HtmlWeb web = new HtmlWeb();
				List<CurrencyModel> currencyList = new List<CurrencyModel>();
				int maxRetries = 3;

				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();

				for (int attempt = 0; attempt < maxRetries; attempt++)
				{
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
								string currencyName = cellNodes[0].InnerText.Trim();
								string currencyCode = cellNodes[1].InnerText.Trim();
								string exchangeRate = cellNodes[2].InnerText.Trim();

								currencyList.Add(new CurrencyModel { CurrencyName = currencyName, CurrencyCode = currencyCode, ExchangeRate = exchangeRate });
							}
						}

						stopwatch.Stop();
						Debug.WriteLine($"Operation completed in: {stopwatch.ElapsedMilliseconds} ms");
						return currencyList;
					}

					Debug.WriteLine($"Attempt {attempt + 1} failed. Retrying...");
				}

				Debug.WriteLine("Failed to fetch data after multiple attempts.");
				return null;
			}
			catch(Exception ex)
			{
				return null;
			}
			
		}


		public List<StockData> ScrapYahooAsync(string url)
        {
			try
			{
				List<StockData> stockDataList = new List<StockData>();

				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();

				HtmlWeb web = new HtmlWeb();
				HtmlDocument doc = web.Load(url);

				string dateSelector = "//tr[contains(@class, 'yf-ewueuo')]/td[1]";
				string openSelector = "//tr[contains(@class, 'yf-ewueuo')]/td[2]";
				string highSelector = "//tr[contains(@class, 'yf-ewueuo')]/td[3]";
				string lowSelector = "//tr[contains(@class, 'yf-ewueuo')]/td[4]";
				string closeSelector = "//tr[contains(@class, 'yf-ewueuo')]/td[5]";


				HtmlNodeCollection dateNodes = doc.DocumentNode.SelectNodes(dateSelector);
				HtmlNodeCollection openNodes = doc.DocumentNode.SelectNodes(openSelector);
				HtmlNodeCollection highNodes = doc.DocumentNode.SelectNodes(highSelector);
				HtmlNodeCollection lowNodes = doc.DocumentNode.SelectNodes(lowSelector);
				HtmlNodeCollection closeNodes = doc.DocumentNode.SelectNodes(closeSelector);

				if (dateNodes != null && openNodes != null && highNodes != null && lowNodes != null && closeNodes != null)
				{
					int minCount = Math.Min(dateNodes.Count, Math.Min(openNodes.Count, Math.Min(highNodes.Count, Math.Min(lowNodes.Count, closeNodes.Count))));

					Console.WriteLine($"dateNodes.Count: {dateNodes.Count}, openNodes.Count: {openNodes.Count}, highNodes.Count: {highNodes.Count}, lowNodes.Count: {lowNodes.Count}, closeNodes.Count: {closeNodes.Count}");

					for (int i = 0; i < minCount; i++)
					{
						string date = dateNodes[i].InnerText.Trim();
						string open = openNodes[i].InnerText.Trim();
						string high = highNodes[i].InnerText.Trim();
						string low = lowNodes[i].InnerText.Trim();
						string close = closeNodes[i].InnerText.Trim();

						StockData stock = new StockData
						{
							Date = date,
							OpenPrice = open,
							HighPrice = high,
							LowPrice = low,
							ClosePrice = close
						};

						double lowCheck = 0;
						double openCheck = 0;
						double.TryParse(low, NumberStyles.Float, CultureInfo.InvariantCulture, out lowCheck);
						double.TryParse(open, NumberStyles.Float, CultureInfo.InvariantCulture, out openCheck);
						if (lowCheck < openCheck )
						{
							stockDataList.Add(stock);
						}
						

						Console.WriteLine($"Date: {date}, Open: {open}, High: {high}, Low: {low}, Close: {close}");
					}

				}
				else
				{
					Console.WriteLine("Data not found or selectors need adjustment.");
				}

				stopwatch.Stop();
				Console.WriteLine($"Czas wykonania operacji: {stopwatch.ElapsedMilliseconds} ms");

				return stockDataList;
			}
			catch(Exception ex )
			{
				return null;
			}
           
        }

		public string ConstructUrl(string companyShortcut, DateTime startDate, DateTime endDate)
		{
			try
			{
				long period1 = (long)(startDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
				long period2 = (long)(endDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
				DateTimeOffset dateTimeOffset = new DateTimeOffset(startDate);
				DateTimeOffset dateTimeOffset1 = new DateTimeOffset(endDate);


				return $"https://finance.yahoo.com/quote/{companyShortcut}/history?period1={period1}&period2={period2}";
			}
			catch(Exception ex)
			{
				return null;
			}
			
		}
	}
}

