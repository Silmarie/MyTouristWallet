using System;
using SQLite;

namespace MyTouristWallet
{
	public interface ISQLite
	{
		SQLiteConnection GetConnection();
	}
}
