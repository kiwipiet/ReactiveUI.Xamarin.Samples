using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using SQLite;
using SQLite.Net.Async;

namespace StockWatch.Advandced
{

    public class AppDb : SQLiteAsyncConnection
    {
        
        public AppDb(Func<SQLite.Net.SQLiteConnectionWithLock> connection)
            : base(connection)
        {
            WatchListItemChange = new Subject<Tuple<DbChangeTypeEnum, WatchListItem>>();
        }

        public async Task SetupDatabaseAsync()
        {
            //TODO: Step 4 - Create tables
            // create the SQLite database tables based on the Model classes
            await CreateTableAsync<Settings>().ConfigureAwait(false);
            await CreateTableAsync<WatchListItem>().ConfigureAwait(false);
        }
        

        // Methods

        /// <summary>
        /// Informs subscribers about an WatchListItem Update. Sample usage of an Subject for notification.
        /// </summary>
        public Subject<Tuple<DbChangeTypeEnum, WatchListItem>> WatchListItemChange { get; private set; }

        /// <summary>
        /// Saves the watch list item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public async Task<int> SaveWatchListItem(WatchListItem item)
        {
            if (item == null) return -1;
            if (item.Id <= 0)
            {
                var id = await InsertAsync(item).ConfigureAwait(false);

                // Publish Create
                WatchListItemChange.OnNext(Tuple.Create(DbChangeTypeEnum.Create, item));

                return id;
            }

            await UpdateAsync(item).ConfigureAwait(false);

            // Publish Update
            WatchListItemChange.OnNext(Tuple.Create(DbChangeTypeEnum.Update, item));

            return item.Id;

        }

        /// <summary>
        /// Deletes the watch list item.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public async void DeleteWatchListItem(int id)
        {
            if (id <= 0) return;

            var item = await Table<WatchListItem>().Where(s => s.Id == id).FirstOrDefaultAsync().ConfigureAwait(false);
            if (item != null)
            {
                await DeleteAsync(item).ConfigureAwait(false);

                // Publish Delete
                WatchListItemChange.OnNext(Tuple.Create(DbChangeTypeEnum.Delete, item));
            }
        }


        /// <summary>
        /// Saves the settings and informs about the Update via the MessageBus.
        /// </summary>
        /// <param name="item">The item.</param>
        public async void SaveSettings(Settings item)
        {
            if (item == null) return;
            if (item.Id <= 0)
            {
                var id = await InsertAsync(item).ConfigureAwait(false);

                // Publish via MessageBus
                MessageBus.Current.SendMessage(item);

            }

            await UpdateAsync(item).ConfigureAwait(false);

            // Publish via MessageBus
            MessageBus.Current.SendMessage(item);
            
        }

    }
}