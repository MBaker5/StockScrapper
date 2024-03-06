using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

class Program
{
	static async Task Main()
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
					Console.WriteLine($"Time elapsed for {currency}: {stopwatch.ElapsedMilliseconds} ms");

					if (data.ContainsKey("rates") && data["rates"].ToObject<JObject>().ContainsKey(currency))
					{
						double value = data["rates"][currency].ToObject<double>();
						totalValue += value;

						Console.WriteLine($"Value of 1 {baseCurrency} in {currency}: {value}");
					}
					else
					{
						Console.WriteLine($"Value not available for {currency}");
					}
				}
				catch (HttpRequestException ex)
				{
					Console.WriteLine($"Error: {ex.Message}");
				}
			}

			Console.WriteLine(); // Add a line break between each currency for better readability
		}

		totalStopwatch.Stop();
		Console.WriteLine($"Total Time elapsed for all currencies: {totalStopwatch.ElapsedMilliseconds} ms");

		if (currencies.Length > 0)
		{
			// Calculate and display the average value
			double averageValue = totalValue / currencies.Length;
			Console.WriteLine($"Average Value for all currencies: {averageValue:F4} {baseCurrency}");
		}
		else
		{
			Console.WriteLine("No currencies to calculate average value.");
		}
	}
}
