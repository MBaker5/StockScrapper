using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockScrapper.Models
{
    public class StockData : INotifyPropertyChanged
    {
        private DateTime _date;
        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
                OnPropertyChanged("Date");
            }
        }

        private double _openPrice;
        public double OpenPrice
        {
            get { return _openPrice; }
            set
            {
                _openPrice = value;
                OnPropertyChanged("OpenPrice");
            }
        }

        private double _highPrice;
        public double HighPrice
        {
            get { return _highPrice; }
            set
            {
                _highPrice = value;
                OnPropertyChanged("HighPrice");
            }
        }

        private double _lowPrice;
        public double LowPrice
        {
            get { return _lowPrice; }
            set
            {
                _lowPrice = value;
                OnPropertyChanged("LowPrice");
            }
        }

        private double _closePrice;
        public double ClosePrice
        {
            get { return _closePrice; }
            set
            {
                _closePrice = value;
                OnPropertyChanged("ClosePrice");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
