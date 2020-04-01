using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DinoLingo
{
    public static class CacheHelper
    {
        public static readonly string DATE_TO_CHECK_NEW_VERSION = "NEW_VERSION_CHECKED_DATE"; // --> date string
        public static readonly string RATE_STATE = "RATE_STATE"; // --> RateWidget.RateState

        public static readonly string COUPONS = "COUPONS"; // + user_id + product_id --> string

        public static readonly string MEMORY_GAMEOBJECTS = "MEMORY_GAMEOBJECTS"; // + lang_cat_id --> GameObjects
        public static readonly string SAS_GAMEOBJECTS = "SAS_GAMEOBJECTS"; // + lang_cat_id --> GameObjects 

        public static readonly string LOGIN = "LOGIN"; // --> Login_Response.Login
        public static readonly string CURRENT_LANGUAGE = "CURRENT_LANGUAGE";
        public static readonly string CURRENT_LANGUAGE_CAT = "CURRENT_LANGUAGE_CAT";

        public static readonly string POST_LIST = "POST_LIST"; // class PostList
        public static readonly string POST_LIST_ITEM_DATA = "POST_LIST_ITEM_DATA"; // + cur_cats + user_id + viewType.ToString() // class ListViewItemData []
        public static readonly string CATEGORYS_RESPONSE = "CATEGORYS_RESPONSE"; // class CategoryResponse

        public static readonly string GAME_OBJECTS = "GAME_OBJECTS"; // + currentLanguage from LANGUAGES  // class GameObjects 
       
        public static readonly string POST_CATEGORY_IDS = "POST_CATEGORY_IDS"; //class PostCategoryIds + cat_id

        public static readonly string SHORT_REPORT = "SHORT_REPORT"; // + lang_cat + user_id --> class ShortReportResponse.ShortReport

        public static readonly string BADGE_LIST = "BADGE_LIST"; // + lang_cat + user_id --> BadgeListResponse.Badge[]

        //public static readonly string REPORT = "REPORT"; // + lang_cat + user_id --> ReportResponse.Report
        public static readonly string REPORT = "REPORTV2";

        public static readonly string POST = "POST"; // + post_id --> PostResponse.Post

        public static readonly string BOOK = "BOOK"; // + lang_cat_ + book_id --> BookPageResponse.BookPage[]

        
        // ================================ some urls



        static CacheData tmp = new CacheData();

        public static Task<CacheData> GetAsync(string key)
        {
            Debug.WriteLine("Task<CacheData> GetAsync(string key), key =" + key);
            return App.Database.Table<CacheData>().Where(i => i.Key == key).FirstOrDefaultAsync();
        }



        public static Task<T> GetAsync<T>(string key)
        {
            return Task.Run(async () =>
            {
                Debug.WriteLine("Task<T> GetAsync<T>, T =" + typeof(T));
                tmp = await App.Database.Table<CacheData>().Where(i => i.Key == key).FirstOrDefaultAsync();
                if (string.IsNullOrEmpty(tmp.Data)) return default(T);
                T obj = JsonConvert.DeserializeObject<T>(tmp.Data);
                return obj;
            });
        }

        public static Task<bool> IsExpiredAsync(string key)
        {
            return Task.Run(async () =>
            {
                CacheData c = await App.Database.Table<CacheData>().Where(i => i.Key == key).FirstOrDefaultAsync();
                if (string.IsNullOrEmpty(c.DateTime)) return false;
                Debug.WriteLine("CacheHelper -> IsExpiredAsync -> c.DateTime = " + c.DateTime);

                
                TimeSpan timeSpan = DateTime.Now - DateTime.ParseExact(c.DateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

                TimeSpan targetSpan = TimeSpan.ParseExact(c.TimeSpan, "c", CultureInfo.InvariantCulture);
                return (targetSpan < timeSpan);
            });
        }

        public static Task<bool> Exists (string key) {
            return Task.Run(async () =>
            {
                List<CacheData> temp = await App.Database.Table<CacheData>().Where(i => i.Key == key).ToListAsync();
                if (temp != null && temp.Count > 0) return true;
                else return false;
            });
        }

        public static async Task<int> Add(string key, string data, TimeSpan timeSpan = default(TimeSpan))
        {
            int n;
            if (tmp == null) {
                tmp = new CacheData() {DateTime = string.Empty, TimeSpan = timeSpan.ToString("c", CultureInfo.InvariantCulture) };
            }
            tmp.Key = key;
            tmp.Data = data;
            if (timeSpan != default(TimeSpan)) {
                tmp.DateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                tmp.TimeSpan = timeSpan.ToString("c", CultureInfo.InvariantCulture);
            }

            Debug.WriteLine("Try to save item to cache ... key = " + CacheHelper.tmp.Key + ", data = " + CacheHelper.tmp.Data);

            if (!(await Exists(key)))
            { 
                Debug.WriteLine("App.Database.InsertAsync(tmp)");
                //return await App.Database.InsertAsync(tmp);
                n = await App.Database.InsertAsync(tmp);

                /*
                List<CacheData> c = await App.Database.Table<CacheData>().ToListAsync();
                foreach (CacheData el in c)
                {
                    Debug.WriteLine(el.Key);
                }
*/
                return n;
            }
            else {
                Debug.WriteLine("App.Database.UpdateAsync(tmp);");
                //return await App.Database.UpdateAsync(tmp);

                tmp = await App.Database.Table<CacheData>().Where(i => i.Key == key).FirstOrDefaultAsync();
                tmp.Key = key;
                tmp.Data = data;

                n = await App.Database.UpdateAsync(tmp);
                /*
                List<CacheData> c = await App.Database.Table<CacheData>().ToListAsync();
                foreach (CacheData el in c)
                {
                    Debug.WriteLine(el.Key);
                }
                */
                return n;
            }           
        }


        public static async Task<int> Add<T>(string key, T obj, TimeSpan timeSpan = default(TimeSpan)) {
            string data;
            Debug.WriteLine("try to add key :" + key);
            if (obj == null) { data = null; }
            else {
                data = JsonConvert.SerializeObject(obj);
            }
            Debug.WriteLine("key almost added");

            return await Add(key, data, timeSpan);
        }

        public static Task<int> Delete(CacheData cache)
        {
            return App.Database.DeleteAsync(cache);
        }


    }


    public class PostCategoryIds {
        public int LESSONS_AND_GAMES { get; set; }
        public int BOOKS { get; set; }
        public int STORIES { get; set; }
        public int SONGS { get; set; }

        public int GetIdFromViewType (MainPage_ViewModel.CENTRAL_VIEWS viewType) {
            switch (viewType) {
                case MainPage_ViewModel.CENTRAL_VIEWS.LESSONS_AND_GAMES: return LESSONS_AND_GAMES;
                case MainPage_ViewModel.CENTRAL_VIEWS.BOOKS: return BOOKS;
                case MainPage_ViewModel.CENTRAL_VIEWS.STORIES: return STORIES;
                case MainPage_ViewModel.CENTRAL_VIEWS.SONGS: return SONGS;    
            }
            return 0;
        }

        public PostCategoryIds (CategoryResponse catResp) {
            LESSONS_AND_GAMES = -1;
            BOOKS = -1;
            STORIES = -1;
            SONGS = -1;

            int i;
            foreach (CategoryResponse.Category c in catResp.result) {
                if (!int.TryParse(c.term_id, out i)) {
                    Debug.WriteLine("error parsing : " + c.term_id);
                    i = -1;
                }

                if (c.viewType == MainPage_ViewModel.CENTRAL_VIEWS.LESSONS_AND_GAMES) {
                    LESSONS_AND_GAMES = i;
                }
                else if (c.viewType == MainPage_ViewModel.CENTRAL_VIEWS.BOOKS)
                {
                    BOOKS = i;
                }
                else if (c.viewType == MainPage_ViewModel.CENTRAL_VIEWS.STORIES)
                {
                    STORIES = i;
                }
                else if (c.viewType == MainPage_ViewModel.CENTRAL_VIEWS.SONGS)
                {
                    SONGS = i;
                }
            }
        }
    }
}
