using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockScrapper_Database.Models
{
	public class CurrencyModel
	{
		private string _currencyName;
		public string CurrencyName
		{
			get { return _currencyName; }
			set
			{
				_currencyName = value;
			}
		}

		private string _currencyCode;
		public string CurrencyCode
		{
			get { return _currencyCode; }
			set
			{
				_currencyCode = value;
			}
		}

		private string _exchangeRate;
		public string ExchangeRate
		{
			get { return _exchangeRate; }
			set
			{
				_exchangeRate = value;
			}
		}
	}
}
