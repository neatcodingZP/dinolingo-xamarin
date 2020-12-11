using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DinoLingo
{
    public static class BookPageCacheHelper {

        public static async Task<List<BookPageResponse.BookPage>> GetBookPagesCached(int LangId, int BookId)
        {
            Task<List<BookPageCache>> t = App.Database.Table<BookPageCache>().Where(i => i.BookId == BookId && i.LangId == LangId).ToListAsync();
            var result = await t;

            if (result == null || result.Count == 0) return null;

            List<BookPageResponse.BookPage> list = new List<BookPageResponse.BookPage>();
            foreach (var bookPageCached in result)
            {
                try
                {
                    list.Add(JsonConvert.DeserializeObject<BookPageResponse.BookPage>(bookPageCached.Data));
                }
                catch
                {

                }
            }
            return list;
        }

        public static async Task<BookPageResponse.BookPage> GetBookPageCached(int LangId, int BookId, int PageId)
        {
            Task<List<BookPageCache>> t = App.Database.Table<BookPageCache>().Where(i => i.BookId == BookId && i.LangId == LangId && i.PageId == PageId).ToListAsync();
            List<BookPageCache> result = await t;

            if (result == null || result.Count == 0) return null;

            try
            {
                return JsonConvert.DeserializeObject<BookPageResponse.BookPage>(result[0].Data);
            }
            catch
            {
                return null;
            }
        }

        public static Task<bool> Exists(int LangId, int BookId, int PageId)
        {
            return Task.Run(async () =>
            {
                List<BookPageCache> temp = await App.Database.Table<BookPageCache>().Where(i => i.BookId == BookId 
                && i.LangId == LangId 
                && i.PageId == PageId).ToListAsync();
                if (temp != null && temp.Count > 0) return true;
                else return false;
            });
        }

        public static async Task<int> Add(int LangId_, int BookId_, int PageId_, BookPageResponse.BookPage bookPageData, TimeSpan timeSpan = default(TimeSpan))
        {
            int n = -1;

            string data = JsonConvert.SerializeObject(bookPageData);

            BookPageCache bookPage = new BookPageCache() { 
                DateTime = string.Empty, 
                TimeSpan = timeSpan.ToString("c", CultureInfo.InvariantCulture),
                LangId = LangId_,
                BookId = BookId_,
                PageId = PageId_,
                Data = data
                
            };


            if (timeSpan != default(TimeSpan))
            {
                bookPage.DateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                bookPage.TimeSpan = timeSpan.ToString("c", CultureInfo.InvariantCulture);
            }

            Debug.WriteLine("Try to save item to cache ... BookId, PageId = " + bookPage.BookId + "/" + bookPage.PageId + ", data = " + bookPage.Data);

            if (!(await Exists(LangId_, BookId_, PageId_)))
            { 
                n = await App.Database.InsertAsync(bookPage);

                return n;
            }
            else
            {

                bookPage = await App.Database.Table<BookPageCache>().Where(i => i.BookId == BookId_
                && i.LangId == LangId_
                && i.PageId == PageId_).FirstOrDefaultAsync();

                bookPage.Data = data;

                n = await App.Database.UpdateAsync(bookPage);
                
                return n;
            }
        }
    }
}
