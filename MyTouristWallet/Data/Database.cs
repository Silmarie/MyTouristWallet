using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using MyTouristWallet;

namespace MyTouristWallet
{
	public class Database
	{
		//static object locker = new object();

		SQLiteConnection database;

		/// <summary>
		/// Initializes a new instance of the <see cref="Tasky.DL.TaskDatabase"/> TaskDatabase. 
		/// if the database doesn't exist, it will create the database and all the tables.
		/// </summary>
		/// <param name='path'>
		/// Path.
		/// </param>
		public Database()
		{
			database = DependencyService.Get<ISQLite>().GetConnection();
			// create the tables
			database.CreateTable<Amount>();
			database.CreateTable<CurrencyCall>();
		}

		public IEnumerable<Amount> GetAmounts()
		{
				return (from i in database.Table<Amount>() select i).ToList();
		}

		public IEnumerable<CurrencyCall> GetCurrencyCalls()
		{
				return (from i in database.Table<CurrencyCall>() select i).ToList();
		}

		public Amount GetAmount(int id)
		{
				return database.Table<Amount>().FirstOrDefault(x => x.ID == id);	
		}

		public CurrencyCall GetCurrencyCall(int id)
		{
				return database.Table<CurrencyCall>().FirstOrDefault(x => x.ID == id);
		}

		public int SaveAmount(Amount item)
		{
				if (item.ID != 0)
				{
					database.Update(item);
					return item.ID;
				}
				else {
					return database.Insert(item);
				}
		}

		public int SaveCurrencyCall(CurrencyCall item)
		{
				if (item.ID != 0)
				{
					database.Update(item);
					return item.ID;
				}
				else {
					return database.Insert(item);
				}
		}

		public int DeleteAmount(int id)
		{
			return database.Delete<Amount>(id);
		}

		public int DeleteCurrencyCall(int id)
		{
			return database.Delete<CurrencyCall>(id);
		}

		public int DeleteAllAmounts()
		{
			return database.DeleteAll<Amount>();
		}

		public int DeleteAllCurrencyCalls()
		{
			return database.DeleteAll<CurrencyCall>();
		}
	}
}