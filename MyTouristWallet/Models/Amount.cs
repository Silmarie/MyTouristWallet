using System;
using System.ComponentModel;
using SQLite;

namespace MyTouristWallet
{
	public class Amount : INotifyPropertyChanged
	{
		public Amount()
		{
			
		}

		[PrimaryKey, AutoIncrement]
		public int ID { get; set; }
		public string currency { get; set; }
		public string description { get; set; }
		public decimal amountValue;
		public string color { get; set; }

		public decimal AmountValue
		{
			set
			{
				if (amountValue != value)
				{
					amountValue = value;
					OnPropertyChanged("AmountValue");
				}
			}
			get
			{
				return amountValue;
			}
		}

		public Amount(string currency, string description, decimal value, string color)
		{
			this.currency = currency;
			this.description = description;
			this.amountValue = value;
			this.color = color;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		void OnPropertyChanged(string v)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this,
					new PropertyChangedEventArgs(v));
			}
		}
	}
}
