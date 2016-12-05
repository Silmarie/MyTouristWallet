using System;
using SQLite;

namespace MyTouristWallet
{
	public class Amount
	{
		public Amount()
		{
			
		}

		[PrimaryKey, AutoIncrement]
		public int ID { get; set; }
		public string currency { get; set; }
		public string description { get; set; }
		public decimal value { get; set; }
		public string color { get; set; }

		public Amount(string currency, string description, decimal value, string color)
		{
			this.currency = currency;
			this.description = description;
			this.value = value;
			this.color = color;
		}
	}
}
