using System;
using System.Net;
using SQLite;

namespace MyTouristWallet
{
	public class CurrencyCall
	{
		public CurrencyCall()
		{

		}

		[PrimaryKey, AutoIncrement]
		public int ID { get; set; }
		public string currencies { get; set; }
		public decimal rate { get; set; }
		public string date { get; set; }
		public string time { get; set; }

		public CurrencyCall(string currencies, decimal rate, string date, string time)
		{
			this.currencies = currencies;
			this.rate = rate;
			this.date = date;
			this.time = time;
		}
	}
}
