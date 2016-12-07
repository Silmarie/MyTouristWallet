using System;
using MyTouristWallet;
using Xamarin.Forms;
using MyTouristWallet.iOS;
using System.IO;
using SQLite;

[assembly: Dependency(typeof(SQLite_iOS))]

namespace MyTouristWallet.iOS
{
	public class SQLite_iOS : ISQLite
	{
		public SQLite_iOS()
		{
		}

		public SQLite.SQLiteConnection GetConnection()
		{
			var sqliteFilename = "DB.db3";
			var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
			var libraryPath = Path.Combine(documentsPath, "..", "Library"); // Library folder
			var path = Path.Combine(libraryPath, sqliteFilename);

			if (!File.Exists(path))
			{
				File.Create(path);
			}
			// Create the connection
			var conn = new SQLiteConnection(path);
			// Return the database connection
			return conn;
		}

	}
}