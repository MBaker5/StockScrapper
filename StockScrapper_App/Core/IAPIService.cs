using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockScrapper_App.Core
{
	public interface IAPIService
	{
		Task GetCurrenciesByFrankfuterAPIAsync();
	}
}
