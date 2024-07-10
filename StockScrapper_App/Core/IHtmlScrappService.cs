using StockScrapper_Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockScrapper_App.Core
{
	public interface IHtmlScrappService
	{
		List<CurrencyModel> GetDataFromNBP();
		List<string> GetMostActiveOnMarket();
		List<StockData> ScrapYahoo(string companyShortcut);

    }
}
