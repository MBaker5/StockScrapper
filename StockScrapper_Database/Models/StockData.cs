using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockScrapper_Database.Models
{
    public class StockData
    {
        private string _date;
		private string _openPrice;
		private string _highPrice;
		private string _lowPrice;
		private string _closePrice;
		public string Date
        {
            get { return _date; }
            set
            {
                _date = value;
            }
        }
        
        public string OpenPrice
        {
            get { return _openPrice; }
            set
            {
                _openPrice = value;
            }
        }
        
        public string HighPrice
        {
            get { return _highPrice; }
            set
            {
                _highPrice = value;
            }
        }
        
        public string LowPrice
        {
            get { return _lowPrice; }
            set
            {
                _lowPrice = value;
            }
        }
        
        public string ClosePrice
        {
            get { return _closePrice; }
            set
            {
                _closePrice = value;
            }
        }
    }
}
