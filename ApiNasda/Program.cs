using System;
using System.Net.Http;
using System.Threading.Tasks;

class Program
{
	static async Task Main()
	{
		string apiUrl = "https://data.nasdaq.com/api/v3/datatables/QUOTEMEDIA/TICKERS?api_key=jnyyaAToQhVxKbwHG2g3&qopts.export=true";

		using (HttpClient client = new HttpClient())
		{
			try
			{
				HttpResponseMessage response = await client.GetAsync(apiUrl);
				response.EnsureSuccessStatusCode();

				string json = await response.Content.ReadAsStringAsync();
				Console.WriteLine(json); // Display the raw JSON response

				// Add your code to parse and process the JSON response here
				// For example, you can use Newtonsoft.Json to deserialize the JSON
			}
			catch (HttpRequestException ex)
			{
				Console.WriteLine($"Error: {ex.Message}");
			}
		}
	}
}