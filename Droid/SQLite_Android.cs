using System;
using System.IO;
using MyTouristWallet;
using MyTouristWallet.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLite_Android))]

namespace MyTouristWallet.Droid
{

	public class SQLite_Android : ISQLite
	{
		public SQLite_Android()
		{
		}

		public SQLite.SQLiteConnection GetConnection()
		{
			var sqliteFilename = "DB.db3";
			string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
			var path = Path.Combine(documentsPath, sqliteFilename);
			Console.WriteLine(path);
			if (!File.Exists(path)) File.Create(path);
			var conn = new SQLite.SQLiteConnection(path);
			// Return the database connection 
			return conn;
		}
	}
}
