using System;
using SQLite;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DinoLingo
{
    public class LocalDatabase : SQLiteAsyncConnection
    {


        public LocalDatabase(string dbPath) : base(dbPath)
        {
            CreateTableAsync<Word>().Wait();
            //DeleteAllAsync<Word>().Wait();

            //Debug.WriteLine("Word Table - OK");
            //DropTableAsync<CacheData>().Wait();
            CreateTableAsync<CacheData>().Wait();
            //DeleteAllAsync<CacheData>().Wait();

            Debug.WriteLine("LocalDatabase -> Initiated");
        }

        /*
        public async Task<List<string>> GetUnsavedWordKeys(List<string> words, string language, string theme) {
            Debug.WriteLine("GetUnsavedWordKeys, theme = " + theme);
            List<string> savedWords_s = (await Table<Word>().Where(i => i.Theme == theme && i.Language == language).ToListAsync()).Select(i => i.Key).ToList();
            Debug.WriteLine("savedWords.Count = " + savedWords_s.Count);
            var list = words.Except(savedWords_s);
            return list.ToList();
        }
*/

        public Task<List<Word>> GetItemsAsync()
        {
            Debug.WriteLine("GetItemsAsync()");
            return this.Table<Word>().ToListAsync();
        }

        public Task<List<Word>> GetItemsNotDoneAsync()
        {
            return this.QueryAsync<Word>("SELECT * FROM [TodoItem] WHERE [Done] = 0");
        }

        public Task<Word> GetItemAsync(int id)
        {
            Debug.WriteLine("GetItemAsync(int id =" + id);
            return this.Table<Word>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveItemAsync(Word item)
        {
            Debug.WriteLine("SaveItemAsync(Word item, id=" + item.Id + " , Key =" + item.Key);
            if (item.Id != 0)
            {
                return this.UpdateAsync(item);
            }
            else
            {
                return this.InsertAsync(item);
            }
        }

        public async Task<int> SaveItemAsyncByKey(Word item)
        {
            Debug.WriteLine("SaveItemAsyncByKey(Word item, id=" + item.Id + " , Key =" + item.Key);

            List<Word> haveKeys = await Table<Word>().Where(w => w.Key == item.Key).ToListAsync();

            if (haveKeys.Count == 0)
            {
                Debug.WriteLine("ADD ITEM TO TABLE count = 0");
                if (item.Id != 0)
                {
                    return await UpdateAsync(item);
                }
                else
                {
                    return await InsertAsync(item);
                }
            }
            else {
                Debug.WriteLine("We already have keys, item.Key =" + item.Key + ", Count=" + haveKeys.Count);
                return -1;
            }

        }

        public Task<int> DeleteItemAsync(Word item)
        {
            return this.DeleteAsync(item);
        }

    }

    [Table("Words")]
    public class Word
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        [Column("_Key")]
        public string Key { get; set; }

        [Column("_Theme")]
        public string Theme { get; set; }

        [Column("_Language")]
        public string Language { get; set; }
        public string FilePath { get; set; }
   }

    [Table("Cache")]
    public class CacheData
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        [Column("_Key")]
        public string Key { get; set; }

        [Column("_Data")]
        public string Data { get; set; }

        [Column("_Date")]
        public string DateTime { get; set; }

        [Column("_TimeSpan")]
        public string TimeSpan { get; set; }
    }
   
}
