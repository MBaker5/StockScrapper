using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockScrapper.Models
{
	public class CurrencyData : INotifyPropertyChanged
	{
		private string _currencyCode;
		public string CurrencyCode
		{
			get { return _currencyCode; }
			set
			{
				_currencyCode = value;
				OnPropertyChanged("CurrencyCode");
			}
		}

		private string _exchangeRate;
		public string ExchangeRate
		{
			get { return _exchangeRate; }
			set
			{
				_exchangeRate = value;
				OnPropertyChanged("ExchangeRate");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
