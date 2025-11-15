using System;
using System.IO;
using System.Collections.Generic;

using SQLite;

namespace door
{
	public static class LocalDB
	{
		static SQLiteConnection db;

		static LocalDB()
		{
            string databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MyData.db");

            db = new SQLiteConnection(databasePath);
            db.CreateTable<Lock>();
        }

		public static List<Lock> GetLocks()
		{
			return db.Query<Lock>("SELECT * FROM Lock");
		}

		public static void AddLock(Lock l)
		{
			db.InsertOrReplace(l);
		}
	}
}
