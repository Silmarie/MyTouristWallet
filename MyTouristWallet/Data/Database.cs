using SQLite;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace MyTouristWallet
{
	public class Database
	{
		//static object locker = new object();

		SQLiteConnection database;

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

		public IEnumerable<CurrencyCall> GetCurrencyCallRate(string currs)
		{
            return database.Query<CurrencyCall>("SELECT * FROM [CurrencyCall] WHERE [currencies] = ?", currs);
		}

		public int SaveAmount(Amount item)
		{
				if (item.ID != 0 /*|| database.Find<Amount>(item) != null*/)
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
			List<CurrencyCall> calls = database.Query<CurrencyCall>("SELECT * FROM [CurrencyCall] WHERE [currencies] = ?", item.currencies);
			if (item.ID != 0 || calls.Count() > 0)
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