using StockScrapper_App.Core;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
namespace StockScrapper_App.Services
{
	public class APIService : IAPIService
	{
		public async Task GetCurrenciesByFrankfuterAPIAsync()
		{
			string[] currencies = { "USD", "EUR", "GBP", "JPY", "AUD" };
			string baseCurrency = "PLN";

			Stopwatch totalStopwatch = new Stopwatch();
			totalStopwatch.Start();

			double totalValue = 0;

			foreach (var currency in currencies)
			{
				string apiUrl = $"https://api.frankfurter.app/latest?from={baseCurrency}&to={currency}";

				using (HttpClient client = new HttpClient())
				{
					try
					{
						Stopwatch stopwatch = new Stopwatch();
						stopwatch.Start();

						HttpResponseMessage response = await client.GetAsync(apiUrl);
						response.EnsureSuccessStatusCode();

						string json = await response.Content.ReadAsStringAsync();
						JObject data = JObject.Parse(json);

						stopwatch.Stop();
						Debug.WriteLine($"Time elapsed for {currency}: {stopwatch.ElapsedMilliseconds} ms");

						if (data.ContainsKey("rates") && data["rates"].ToObject<JObject>().ContainsKey(currency))
						{
							double value = data["rates"][currency].ToObject<double>();
							totalValue += value;

							Debug.WriteLine($"Value of 1 {baseCurrency} in {currency}: {value}");
						}
						else
						{
							Debug.WriteLine($"Value not available for {currency}");
						}
					}
					catch (HttpRequestException ex)
					{
						Debug.WriteLine($"Error: {ex.Message}");
					}
				}
			}

			totalStopwatch.Stop();
			Debug.WriteLine($"Total Time elapsed for all currencies: {totalStopwatch.ElapsedMilliseconds} ms");
		}
	}
}
