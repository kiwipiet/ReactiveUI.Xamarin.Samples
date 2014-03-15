using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using SQLite;

namespace ReactiveUI.Sample.Routing
{
    public interface IAppDb : ISQLiteConnection
    {
         
    }

    public class AppDb : SQLiteConnection, IAppDb
    {
        public static object locker = new object();

        #region public AppDb(): this("app.db")

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDb"/> class.
        /// </summary>
        public AppDb()
            : this("app.db")
        {

        }

        #endregion

        #region public AppDb(string databasePath)

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDb" /> class.
        /// if the database doesn't exist, it will create the database and all the tables.
        /// </summary>
        /// <param name="databasePath">The database path.</param>
        public AppDb(string databasePath)
            : base(App.Current.GetFilePath(databasePath), SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite, true)
        {
            // Create Tables
            CreateTable<WatchListItem>();

        }

        #endregion

        // Methods


    }
}