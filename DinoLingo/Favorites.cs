using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using FFImageLoading;
using Newtonsoft.Json;
using Plugin.Connectivity;
using Xamarin.Forms;
using System.Linq;
using System.Collections.ObjectModel;
using Plugin.GoogleAnalytics;

namespace DinoLingo
{
    public static class Favorites
    {
        
        static int MAX_BOOKS = 20;
        static int MAX_VIDEOS = 12;
        static int DOWNLOAD_RETRY_TIMESPAN_CONNECTION = 20;
        static int DOWNLOAD_RETRY_TIMESPAN_NO_CONNECTION = 120;

        static int books = 0;

        public static int Books {
            get {
                    return books;
            }
            set {
                books = value;
            }
        }

        static int videos = 0;
        public static int Videos
        {
            get
            {
                return videos;
            }
            set
            {
                videos = value;
            }
        }

        static bool isDownloading = false;
        public static bool IsDownloading
        {
            get
            {
                return isDownloading;
            }
            set
            {
                isDownloading = value;
            }
        }

        static bool isLoaded = false;
        public static bool IsLoaded
        {
            get
            {
                return isLoaded;
            }
            set
            {
                isLoaded = value;
            }
        }

        static bool isTimerOn = false;
        public static bool IsTimerOn
        {
            get
            {
                return isTimerOn;               
            }
            set
            {
                isTimerOn = value;
            }
        }

        static bool listsAreBuisy = false;
        static Object ListsAreBuisyLock = new Object();
        public static bool ListsAreBuisy
        {
            get
            {
                lock (ListsAreBuisyLock)
                {
                    return listsAreBuisy;
                }
            }
            set
            {
                lock (ListsAreBuisyLock)
                {
                    listsAreBuisy = value;
                }
            }
        }

        public static ListViewItemData[][][] allListsData = new ListViewItemData[MainPage_ViewModel.MAIN_CATS_COUNT][][];

        public static Dictionary<string, FavoriteListCoords> add = new Dictionary<string, FavoriteListCoords>();
        static Dictionary<string, FavoritVisualItem> add_items = new Dictionary<string, FavoritVisualItem>();

        public static void AddToAddFavoritesItems (string id, FavoriteListCoords centralView, TestViewModel testViewModel, int listIndex, bool isSingle) {
            if (!add_items.ContainsKey(id)) {
                add_items.Add(id, new FavoritVisualItem { FavListCoords = new FavoriteListCoords() {
                    CentralView = centralView.CentralView, SubView = centralView.SubView},
                    TestViewModel = testViewModel, ListIndex = listIndex, IsSingle = isSingle });
            }
        }

        public static Dictionary<string, FavoriteListCoords> current = new Dictionary<string, FavoriteListCoords>();
        public static Dictionary<string, FavoriteListCoords> remove = new Dictionary<string, FavoriteListCoords>();

        public static bool IsAdding(string id)
        {
            return add.ContainsKey(id);
        }

        public static void WaitIfListsAreBuisy(string s) {
            
                while (ListsAreBuisy)
                {
                    Task.Delay(75).Wait();
                    Debug.WriteLine("WAIT, from ->" + s);
                }
                ListsAreBuisy = true;
                Debug.WriteLine(s + "ListsAreBuisy = true");
        }

        /*
        static Task ReleaseLists() {
            return Task.Run(async ()=> {
                await Task.Delay(50);
                ListsAreBuisy = false;
                return;
            });
        }
*/
        public static bool IsVisualFavorite(string id) {
            if (string.IsNullOrEmpty(id)) return false;
            if (remove.ContainsKey(id)) return false;
            if (add.ContainsKey(id) || current.ContainsKey(id)) return true;
            return false;
        }

        public static bool IsFavorite (string id) {
            return (current.ContainsKey(id) && !remove.ContainsKey(id)) ;
        }

        static void WriteLineAllLists() {
            Debug.WriteLine("*******");
            Debug.Write("current: ");
            if (current != null) foreach (string key in current.Keys) Debug.Write(key + "/" + current[key].CentralView + ", " );

            Debug.Write("\nadd: ");
            if (add != null) foreach (string key in add.Keys) Debug.Write(key + "/" + add[key].CentralView + ", ");

            Debug.Write("\nremove: ");
            if (remove != null) foreach (string key in remove.Keys) Debug.Write(key + "/" + remove[key].CentralView + ", ");
            Debug.WriteLine("");
        }

        public static async void AddToCurrentFinal(string id, FavoriteListCoords favoriteListCoords)
        {
            Debug.WriteLine("Favorites -> AddToCurrentFinal ->, id: " + id);

            // ***
            WaitIfListsAreBuisy("AddToCurrentFinal");
            //check if we have item in add list
            if (!add.ContainsKey(id))
            {
                Debug.WriteLine("Favorites -> AddToCurrentFinal -> WE NOT have it in add list, do nothing, id: " + id);
                ListsAreBuisy = false;
                Debug.WriteLine("Favorites -> AddToCurrentFinal -> ListsAreBuisy = false");
                return;
            }

            // remove from add dict
            if (add.ContainsKey(id))
                  {
                        add.Remove(id);
                        if (favoriteListCoords.CentralView == MainPage_ViewModel.CENTRAL_VIEWS.BOOKS) Books--;
                        else Videos--;
                        if (add_items.ContainsKey(id)) {
                            add_items[id].OnDownloadedOK();
                            add_items.Remove(id);
                        }
                  }

             if (!current.ContainsKey(id))
                    {
                        current.Add(id, favoriteListCoords);
                        if (favoriteListCoords.CentralView == MainPage_ViewModel.CENTRAL_VIEWS.BOOKS) Books++;
                        else Videos++;

                    }
             WriteLineAllLists();


            await SaveFavorites_unprotected(UserHelper.Lang_cat, UserHelper.Login.user_id);

            ListsAreBuisy = false;
            Debug.WriteLine("Favorites -> AddToCurrentFinal finish -> ListsAreBuisy = false");
            // ***
        }



        static Task DeleteItem (string id, FavoriteListCoords favoriteListCoords) {
            return Task.Run(async() => {
                Debug.WriteLine("Favorites -> DeleteItem ->, type = " + favoriteListCoords.CentralView);
                if (favoriteListCoords.CentralView != MainPage_ViewModel.CENTRAL_VIEWS.BOOKS)
                {

                    // do nothing ? - no 
                    // DELETE FILE
                    Debug.WriteLine("Favorites -> AddToCurrentFinal -> we don't need this video, delete it..., file exists? " + await PCLHelper.IsFileExistAsync(id + ".mp4"));
                    await PCLHelper.DeleteFile(id + ".mp4");
                    Debug.WriteLine("Favorites -> AddToCurrentFinal -> file deleted, video exists? " + await PCLHelper.IsFileExistAsync(id + ".mp4"));
                    await PCLHelper.IsFileExistAsync(id + ".mp4");

                }
                else
                {
                    Debug.WriteLine("Favorites -> Delete book -> ..., id = " + id);
                    if (await CacheHelper.Exists(CacheHelper.BOOK + UserHelper.Lang_cat + "_" + id)) { // we can delete book
                        BookPageResponse.BookPage[] bookPages = await CacheHelper.GetAsync<BookPageResponse.BookPage[]>(CacheHelper.BOOK + UserHelper.Lang_cat + "_" + id);
                        for (int i = 0; i < bookPages.Length; i ++) {
                            BookPageResponse.BookPage page = bookPages[i];
                            // delete all ImageService 
                            Debug.WriteLine("page == null ?" + (page == null));
                            Debug.WriteLine("page = " + JsonConvert.SerializeObject(page));
                            if (page.data != null && !string.IsNullOrEmpty(page.data.image)) await ImageService.Instance.InvalidateCacheEntryAsync(page.data.image, FFImageLoading.Cache.CacheType.All, true);
                            if (page.engTrans != null && !string.IsNullOrEmpty(page.engTrans.image)) await ImageService.Instance.InvalidateCacheEntryAsync(page.engTrans.image, FFImageLoading.Cache.CacheType.All, true);
                            // delete all audio 
                            if (page.engTrans != null && !string.IsNullOrEmpty(page.engTrans.audio))
                            {
                                await PCLHelper.DeleteFile("BOOK" + UserHelper.Lang_cat + "_" + id + "_" + i + "transl.mp3");
                            }
                            if (!string.IsNullOrEmpty(page.data.audio))
                            {
                                await PCLHelper.DeleteFile("BOOK" + UserHelper.Lang_cat + "_" + id + "_" + i + ".mp3");
                            }
                            Debug.WriteLine("Favorites -> Delete book -> deleted page = " + i);
                        }

                        // clear cache ...
                        await CacheHelper.Delete(await CacheHelper.GetAsync(CacheHelper.BOOK + UserHelper.Lang_cat + "_" + id));
                        Debug.WriteLine("book still cached ? :" + await CacheHelper.Exists(CacheHelper.BOOK + UserHelper.Lang_cat + "_" + id));
                    }
                    else {
                        Debug.WriteLine("Favorites -> Delete book -> HAVE NO CACHE TO DELETE BOOK ..., id = " + id);
                    }
                }
                return;
            });
        }

        public static bool Add (string id, FavoriteListCoords favListCoords, TestViewModel testViewModel, int listIndex, bool isSingle) {
            if (string.IsNullOrEmpty(id))  
            {
                WriteLineAllLists();
                return false;
            }
           
            // ==============
            if (add_items.ContainsKey(id)) add_items.Remove(id);

            if (allListsData[(int)favListCoords.CentralView][favListCoords.SubView] == null) return false;

            // we should find its index in allListsData
            FavoriteListCoords favoriteListCoords = new FavoriteListCoords() { CentralView = favListCoords.CentralView,
            SubView = favListCoords.SubView, index = -1,};
            for (int i = 0; i < allListsData[(int)favListCoords.CentralView][favListCoords.SubView].Length; i++)
            {
                if (allListsData[(int)favListCoords.CentralView][favListCoords.SubView][i].id == id)
                {
                    favoriteListCoords.index = i;
                    break;
                }
            }
            if (favoriteListCoords.index < 0) return false;

            // ==============
            if (remove.ContainsKey(id))
            {
                remove.Remove(id);

                if (favoriteListCoords.CentralView == MainPage_ViewModel.CENTRAL_VIEWS.BOOKS) Books++;
                else Videos++;

                if (!add.ContainsKey(id) && !current.ContainsKey(id))
                {
                    add.Add(id, favoriteListCoords);
                    add_items.Add(id, new FavoritVisualItem { FavListCoords = favoriteListCoords, TestViewModel = testViewModel, ListIndex = listIndex, IsSingle = isSingle });
                    add_items[id].OnAdded();
                }
                else if (add.ContainsKey(id))
                {
                    add_items.Add(id, new FavoritVisualItem { FavListCoords = favoriteListCoords, TestViewModel = testViewModel, ListIndex = listIndex, IsSingle = isSingle });
                    add_items[id].OnAdded();
                }
                else if (current.ContainsKey(id))
                {
                    add_items.Add(id, new FavoritVisualItem { FavListCoords = favoriteListCoords, TestViewModel = testViewModel, ListIndex = listIndex, IsSingle = isSingle });
                    add_items[id].OnDownloadedOK();
                    add_items.Remove(id);
                }

                WriteLineAllLists();
                return true;
            }
            // ===============
            else if (!add.ContainsKey(id) && !current.ContainsKey(id))
            {
                    if (remove.ContainsKey(id))
                    {
                        remove.Remove(id);
                    }
                    add.Add(id, favoriteListCoords);
                    if (favoriteListCoords.CentralView == MainPage_ViewModel.CENTRAL_VIEWS.BOOKS) Books++;
                    else Videos++;


                add_items.Add(id, new FavoritVisualItem { FavListCoords = favoriteListCoords, TestViewModel = testViewModel, ListIndex = listIndex, IsSingle = isSingle });
                add_items[id].OnAdded();

                WriteLineAllLists();
                return true;
            }
            else if (add.ContainsKey(id)) {


                add_items.Add(id, new FavoritVisualItem { FavListCoords = favoriteListCoords, TestViewModel = testViewModel, ListIndex = listIndex, IsSingle = isSingle });
                add_items[id].OnAdded();

                WriteLineAllLists();
                return false;
            }

            else if (current.ContainsKey(id)) {
                add_items.Add(id, new FavoritVisualItem { FavListCoords = favoriteListCoords, TestViewModel = testViewModel, ListIndex = listIndex, IsSingle = isSingle });
                add_items[id].OnDownloadedOK();
                add_items.Remove(id);

                WriteLineAllLists();
                return false;   
            }

            WriteLineAllLists();
            return false;
        }

        public static bool Remove(string id, FavoriteListCoords favoriteListCoords, TestViewModel testViewModel,  int listIndex, bool isSingle) {
            if (string.IsNullOrEmpty(id))
            {
                WriteLineAllLists();
                return false;
            }

            if (add_items.ContainsKey(id)) add_items.Remove(id);

            if (remove.ContainsKey(id)) {
                add_items.Add(id, new FavoritVisualItem { FavListCoords = favoriteListCoords, TestViewModel = testViewModel, ListIndex = listIndex, IsSingle = isSingle });
                add_items[id].OnRemoved();
                add_items.Remove(id);

                WriteLineAllLists();
                return false;
            }

            else if (add.ContainsKey(id)) {
                if (add.Keys.ElementAt(0) == id) {
                    remove.Add(id, favoriteListCoords);
                }
                else { // just remove from add
                    add.Remove(id);                   
                }

                if (favoriteListCoords.CentralView == MainPage_ViewModel.CENTRAL_VIEWS.BOOKS) Books--;
                else Videos--;

                add_items.Add(id, new FavoritVisualItem { FavListCoords = favoriteListCoords, TestViewModel = testViewModel, ListIndex = listIndex, IsSingle = isSingle });
                add_items[id].OnRemoved();
                add_items.Remove(id);

                WriteLineAllLists();
                return true;
            }
            else if (current.ContainsKey(id))
            {
                remove.Add(id, favoriteListCoords);

                if (favoriteListCoords.CentralView == MainPage_ViewModel.CENTRAL_VIEWS.BOOKS) Books--;
                else Videos--;

                add_items.Add(id, new FavoritVisualItem { FavListCoords = favoriteListCoords, TestViewModel = testViewModel, ListIndex = listIndex, IsSingle = isSingle });
                add_items[id].OnRemoved();
                add_items.Remove(id);

                WriteLineAllLists();               
                return true;
            }

            WriteLineAllLists();
            return false;
        }

        public static bool IsMaxBooks() {

            Debug.WriteLine("total books = " + books);
            bool maxBooks = Books >= MAX_BOOKS;
            return maxBooks;
        }

        public static bool IsMaxVideos()
        {
            Debug.WriteLine("total videos = " + videos);
            bool maxVideos =Videos >= MAX_VIDEOS;
            return maxVideos;
        }


        public static ImageSource  SetVisualSource(string id, ImageSource heart_color_img, ImageSource heart_gray_img, ImageSource heart_preloader_img) {
            if (id == "30345")
            {
                Debug.WriteLine("Favorites -> SetVisualSource - id = 30345");
                WriteLineAllLists();

                if (remove.ContainsKey(id))
                {
                    Debug.WriteLine("Favorites -> SetVisualSource - return GRAY");
                    return heart_gray_img;
                }
                if (current.ContainsKey(id))
                {
                    Debug.WriteLine("Favorites -> SetVisualSource - return COLOR");
                    return heart_color_img;
                }
                if (add.ContainsKey(id))
                {
                    Debug.WriteLine("Favorites -> SetVisualSource - return PRELOADER");
                    return heart_preloader_img;
                }

                Debug.WriteLine("Favorites -> SetVisualSource - FINALLY return GRAY");
                return heart_gray_img;

            }
            if (remove.ContainsKey(id)) return heart_gray_img;
            if (current.ContainsKey(id)) return heart_color_img;
            if (add.ContainsKey(id)) return heart_preloader_img;
            return heart_gray_img;
        }
        
        /*
        public static KeyValuePair<MainPage_ViewModel.CENTRAL_VIEWS, int> GetCentralViewTypeAndIndex(string id)
        {
            Debug.WriteLine("Favorites -> GetCentralViewTypeAndIndex -> id=" + id);
            KeyValuePair<MainPage_ViewModel.CENTRAL_VIEWS, int> pair = new KeyValuePair<MainPage_ViewModel.CENTRAL_VIEWS, int>(MainPage_ViewModel.CENTRAL_VIEWS.NONE, -1);
            //check if we have id 
            if (IsVisualFavorite(id))
            {
                // we have such a favorite - try to get it from allListsData
                MainPage_ViewModel.CENTRAL_VIEWS type;
                if (current.ContainsKey(id)) type = current[id];
                else type = add[id];

                if (type == MainPage_ViewModel.CENTRAL_VIEWS.BOOKS)
                {
                    // check only in book-list                        
                    for (int i = 0; i < MainPage_ViewModel.BOOKS_SUBCATS_COUNT; i++)
                    {
                        if (allListsData[(int)MainPage_ViewModel.CENTRAL_VIEWS.BOOKS][i] != null)
                            foreach (ListViewItemData item in allListsData[(int)MainPage_ViewModel.CENTRAL_VIEWS.BOOKS][i])
                            {
                                if (item != null && item.id == id)
                                {
                                    pair = new KeyValuePair<MainPage_ViewModel.CENTRAL_VIEWS, int>(MainPage_ViewModel.CENTRAL_VIEWS.BOOKS, i);
                                    break;
                                }
                            }
                        if (pair.Value >=0) break;
                    }
                }
                else
                {
                    // check only in other categories                        
                    for (int i = 0; i < MainPage_ViewModel.MAIN_CATS_COUNT; i++)
                    {
                        if (i == (int)MainPage_ViewModel.CENTRAL_VIEWS.BOOKS) continue;

                        if (allListsData[i][0] != null)
                            foreach (ListViewItemData item in allListsData[i][0])
                            {
                                if (item != null && item.id == id)
                                {
                                    pair = new KeyValuePair<MainPage_ViewModel.CENTRAL_VIEWS, int>((MainPage_ViewModel.CENTRAL_VIEWS) i, 0);
                                    break;
                                }
                            }
                    }
                }
            }
            else
            {
                Debug.WriteLine("Favorites -> GetCentralViewTypeAndIndex -> id=" + id + " - is not in favorites list !!!");
            }


            return pair;
        }

            

        public static ListViewItemData[] GetDataSForFavorite_unprotected(string id)
        {
            Debug.WriteLine("Favorites -> GetDataSForFavorite_unprotected -> id=" + id);
            ListViewItemData[] listViewItemDataS = null;

            //check if we have id 
            if (IsVisualFavorite(id))
            {
                // we have such a favorite - try to get it from allListsData
                TYPE type;
                if (current.ContainsKey(id)) type = current[id];
                else type = add[id];

                if (type == TYPE.BOOK)
                {
                    // check only in book-list                        
                    for (int i = 0; i < MainPage_ViewModel.BOOKS_SUBCATS_COUNT; i++)
                    {
                        if (allListsData[(int)MainPage_ViewModel.CENTRAL_VIEWS.BOOKS][i] != null)
                            foreach (ListViewItemData item in allListsData[(int)MainPage_ViewModel.CENTRAL_VIEWS.BOOKS][i])
                            {
                                if (item != null && item.id == id)
                                {
                                    listViewItemDataS = allListsData[(int)MainPage_ViewModel.CENTRAL_VIEWS.BOOKS][i];
                                    break;
                                }
                            }
                        if (listViewItemDataS != null) break;
                    }
                }
                else
                {
                    // check only in other categories                        
                    for (int i = 0; i < MainPage_ViewModel.MAIN_CATS_COUNT; i++)
                    {
                        if (i == (int)MainPage_ViewModel.CENTRAL_VIEWS.BOOKS) continue;

                        if (allListsData[i][0] != null)
                            foreach (ListViewItemData item in allListsData[i][0])
                            {
                                if (item != null && item.id == id)
                                {
                                    listViewItemDataS = allListsData[i][0];
                                    break;
                                }
                            }
                    }
                }
            }
            else
            {
                Debug.WriteLine("Favorites -> GetDataSForFavorite -> id=" + id + " - is not in favorites list !!!");
            }

            return listViewItemDataS;
        }

        public static ListViewItemData GetDataForFavorite_unprotected(string id, MainPage_ViewModel.CENTRAL_VIEWS)
        {            
                Debug.WriteLine("Favorites -> GetDataForFavorite -> id=" + id);
                ListViewItemData listViewItemData = null;
               
                //check if we have id 
                if (IsVisualFavorite(id))
                {
                    // we have such a favorite - try to get it from allListsData
                    TYPE type;
                    if (current.ContainsKey(id)) type = current[id];
                    else type = add[id];

                    if (type == TYPE.BOOK)
                    {
                        // check only in book-list                        
                        for (int i = 0; i < MainPage_ViewModel.BOOKS_SUBCATS_COUNT; i ++)
                        {
                            if (allListsData[(int) MainPage_ViewModel.CENTRAL_VIEWS.BOOKS][i] != null)
                                foreach (ListViewItemData item in allListsData[(int)MainPage_ViewModel.CENTRAL_VIEWS.BOOKS][i])
                                {
                                    if (item != null && item.id == id)
                                    {
                                        listViewItemData = item;
                                        break;
                                    }
                                }
                            if (listViewItemData != null) break;
                        }
                    }
                    else
                    {
                        // check only in other categories                        
                        for (int i = 0; i < MainPage_ViewModel.MAIN_CATS_COUNT; i++)
                        {
                            if (i == (int)MainPage_ViewModel.CENTRAL_VIEWS.BOOKS) continue;

                            if (allListsData[i][0] != null)
                                foreach (ListViewItemData item in allListsData[i][0])
                                {
                                    if (item != null && item.id == id)
                                    {
                                        listViewItemData = item;
                                        break;
                                    }
                                }                            
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("Favorites -> GetDataForFavorite -> id=" + id + " - is not in favorites list !!!");
                }
                
                return listViewItemData;  
        }
        */

        public static void SetBooksAndVideosCount_unprotected()
        {
            Debug.WriteLine("Favorites -> SetBooksAndVideosCount_unprotected -> ");
            Books = Videos = 0;
            if (current!=null) foreach (KeyValuePair<string, FavoriteListCoords> pair in current)
                {
                    if (pair.Value.CentralView == MainPage_ViewModel.CENTRAL_VIEWS.BOOKS) Books++;
                    else Videos++;
                }
            if (add != null) foreach (KeyValuePair<string, FavoriteListCoords> pair in add)
                {
                    if (pair.Value.CentralView == MainPage_ViewModel.CENTRAL_VIEWS.BOOKS) Books++;
                    else Videos++;
                }
            if (remove != null) foreach (KeyValuePair<string, FavoriteListCoords> pair in remove)
                {
                    if (pair.Value.CentralView == MainPage_ViewModel.CENTRAL_VIEWS.BOOKS) Books--;
                    else Videos--;
                }
            if (Books < 0) Books = 0;
            if (Videos < 0) Videos = 0;
        }

        static string FAVORITES = "FAVORITES";
        public static Task LoadFavorites_unprotected(string lang_cat, string user_id, bool IsFree) {
            return Task.Run(async () =>
            {
                // reset all allListsData
                Debug.WriteLine("Favorites -> LoadFavorites_unprotected -> ");
                for (int i = 0; i < allListsData.Length; i++)
                {
                    Debug.WriteLine("Favorites -> LoadFavorites_unprotected -> int i =" + i);
                    string viewTypeKey = CacheHelper.POST_LIST_ITEM_DATA + "_" + UserHelper.Lang_cat + "_" + UserHelper.Login.user_id + "_" + ((MainPage_ViewModel.CENTRAL_VIEWS)i).ToString() + "_";
                    Debug.WriteLine("Favorites -> LoadFavorites_unprotected -> viewTypeKey =" + viewTypeKey);
                    allListsData[i] = new ListViewItemData[MainPage_ViewModel.SUBCATS[i].Length][];
                    Debug.WriteLine("Favorites -> LoadFavorites_unprotected -> allListsData[i].Length =" + allListsData[i].Length);
                    for (int j = 0; j < allListsData[i].Length; j++)
                    {
                        Debug.WriteLine("Favorites -> LoadFavorites_unprotected -> int j =" + j);
                        string viewTypeKeyFinal = viewTypeKey + j.ToString();                    
                        if (await CacheHelper.Exists(viewTypeKeyFinal))
                        { // we Do have the content - just show it 
                            Favorites.allListsData[i][j] = await CacheHelper.GetAsync<ListViewItemData[]>(viewTypeKeyFinal);
                        }
                    }
                }
                Debug.WriteLine("Favorites -> LoadFavorites_unprotected -> allListsData created ...");
                add = new Dictionary<string, FavoriteListCoords>();
                current = new Dictionary<string, FavoriteListCoords>();
                remove = new Dictionary<string, FavoriteListCoords>();
                add_items = new Dictionary<string, FavoritVisualItem>();

                if (!IsFree && await CacheHelper.Exists(FAVORITES + "add" + lang_cat + "_" + user_id))
                {
                    add = await CacheHelper.GetAsync<Dictionary<string, FavoriteListCoords>>(FAVORITES + "add" + lang_cat  + "_" + user_id);
                }

                if (!IsFree && await CacheHelper.Exists(FAVORITES + "current" + lang_cat + "_" + user_id))
                {
                    current = await CacheHelper.GetAsync<Dictionary<string, FavoriteListCoords>>(FAVORITES + "current" + lang_cat + "_" + user_id);
                }
                if (!IsFree && await CacheHelper.Exists(FAVORITES + "remove" + lang_cat + "_" + user_id))
                {
                    remove = await CacheHelper.GetAsync<Dictionary<string, FavoriteListCoords>>(FAVORITES + "remove" + lang_cat + "_" + user_id);
                }               

                SetBooksAndVideosCount_unprotected();

                IsLoaded = true;

                Debug.WriteLine("Favorites -> list loaded: ");
                WriteLineAllLists();               
                
            });            
        }

        public static Task SaveFavorites_unprotected(string langCat, string user_id)
        {
            return Task.Run(async() =>{
                if (!IsLoaded) return; // we have nothing to save
                await CacheHelper.Add<Dictionary<string, FavoriteListCoords>>(FAVORITES + "add" + langCat + "_" + user_id, add);
                await CacheHelper.Add<Dictionary<string, FavoriteListCoords>>(FAVORITES + "current" + langCat + "_" + user_id, current);
                await CacheHelper.Add<Dictionary<string, FavoriteListCoords>>(FAVORITES + "remove" + langCat + "_" + user_id, remove);
                                
                Debug.WriteLine("Favorites -> list saved: ");
                WriteLineAllLists();

                
            } );

        }

        
        public static KeyValuePair<string, FavoriteListCoords> GetElementToAdd() {
            KeyValuePair<string, FavoriteListCoords> element;
            if (add != null && add.Count > 0)
                {
                    element = new KeyValuePair<string, FavoriteListCoords>(add.Keys.ElementAt(0),add.Values.ElementAt(0));
                }
                else element = new KeyValuePair<string, FavoriteListCoords> ("", null);
            return element;
        }
        

        public static Task InstantRemoveAll() {

            return Task.Run(async() => {
                Debug.WriteLine("Favorites -> InstantRemoveAll");
                while (remove.Count > 0) {

                    KeyValuePair<string, FavoriteListCoords> pairToRemove = remove.ElementAt(0);
                    await DeleteItem(pairToRemove.Key, pairToRemove.Value);

                    remove.Remove(pairToRemove.Key);
                    if (add.ContainsKey(pairToRemove.Key)) add.Remove(pairToRemove.Key);
                    if (current.ContainsKey(pairToRemove.Key)) current.Remove(pairToRemove.Key);
                    await SaveFavorites_unprotected(UserHelper.Lang_cat, UserHelper.Login.user_id);
                }
                Debug.WriteLine("Favorites -> InstantRemoveAll -> Exit");
                return;
            });
        }

        public static Task Refresh(string lang_cat, string user_id, bool IsFree, string new_lang_cat, string new_user_id, bool new_IsFree)
        {
            Debug.WriteLine("Favorites -> Refresh()");
            
            return Task.Run(async () =>
            {               
                WaitIfListsAreBuisy("Favorites -> Refresh()");
                if (remove == null) remove = new Dictionary<string, FavoriteListCoords>();
                if (add != null)
                {
                    foreach (KeyValuePair<string, FavoriteListCoords> pair in add)
                    {
                        if (!remove.ContainsKey(pair.Key))
                        {
                            remove.Add(pair.Key, pair.Value);
                        }
                    }

                    add = new Dictionary<string, FavoriteListCoords>();
                }

                //
                
                // await InstantRemoveAll();

               // if (!IsFree && !string.IsNullOrEmpty(user_id)) await SaveFavorites_unprotected(lang_cat, user_id);

                Debug.WriteLine("Favorites ->Refresh-> LoadFavorites -> new favorites()");
                await LoadFavorites_unprotected(new_lang_cat, new_user_id, new_IsFree);
                ListsAreBuisy = false;
                Debug.WriteLine("Favorites ->Refresh-> LoadFavorites -> ListsAreBuisy = false");

                return;
            });
        }

        public static Task StartLoading(){
            Debug.WriteLine("Favorites -> StartLoading()...");
            return Task.Run(async () =>
            {
                // check if IsDownloading ?
                if (IsDownloading) {
                    Debug.WriteLine("allready downloading -> return");
                    return;
                }
                IsDownloading = true;

                // ***
                WaitIfListsAreBuisy("StartLoading");

                // *** save to cache all lists ***
                await SaveFavorites_unprotected(UserHelper.Lang_cat, UserHelper.Login.user_id);
                // *** OUR SAVE POINT ***

                // INSTANTLY REMOVE ALL WE HAVE to Remove...
                await InstantRemoveAll();

                Debug.WriteLine("Favorites -> InstantRemoveAll -> StartLoading");
                // get first element from addToFavorites
                KeyValuePair<string, FavoriteListCoords> elementToAdd = GetElementToAdd();
                Debug.WriteLine("Favorites -> StartLoading -> add element: " + elementToAdd.Key);
                if (string.IsNullOrEmpty(elementToAdd.Key)) {
                    // we have nothing to Do 
                    IsDownloading = false;
                    Debug.WriteLine("Favorites -> StartLoading() -> WE HAVE NOTHING TO DOWNLOAD AND DELETE (empty key)");
                    ListsAreBuisy = false;
                    Debug.WriteLine("Favorites -> StartLoading() -> ListsAreBuisy = false");
                    // ***
                    return;
                }             

                ListsAreBuisy = false;
                Debug.WriteLine("Favorites -> StartLoading() 2 -> ListsAreBuisy = false");
                // *** 
                // download item

                // and check if it's a video
                if (elementToAdd.Value.CentralView != MainPage_ViewModel.CENTRAL_VIEWS.BOOKS)
                {
                     // 1) download post
                     await DownloadPost(elementToAdd);
                }
                else
                {
                    // 2 ) download book
                    await DownloadBook(elementToAdd);
                }
                return;
            });        
        }

        static Task DownloadBook(KeyValuePair<string, FavoriteListCoords> elementToAdd)
        {
            return Task.Run(async () =>
            {
                // download all the pages (in while loop)
                Debug.WriteLine("Favorites -> loop download all the pages...");
                List<BookPageResponse.BookPage> allPages = new List<BookPageResponse.BookPage>();
                bool proceedDownloading = true;
                int pageIndex = 0;
                while (proceedDownloading) {
                    BookPageResponse.BookPage somePage = await DownloadPage(pageIndex, elementToAdd.Key);
                    if (somePage == null) {
                        Debug.WriteLine("Favorites -> error while loading pages...");
                        //either error in request // or no connection
                        TimeSpan timeSpan;
                        if (CrossConnectivity.Current.IsConnected && DownloadHelper.DownloadHelper.CheckInternetConnectionProgressive())
                        {
                            timeSpan = TimeSpan.FromSeconds(DOWNLOAD_RETRY_TIMESPAN_CONNECTION);
                        }
                        else
                        {
                            timeSpan = TimeSpan.FromSeconds(DOWNLOAD_RETRY_TIMESPAN_NO_CONNECTION);
                        }
                        // and start timer with task ONLY IF WE HAVE NO ACTIVE TIMER !!!
                        IsDownloading = false;
                        if (!IsTimerOn)
                        {
                            IsTimerOn = true;
                            Debug.WriteLine("Favorites -> DownloadPost -> Device.StartTimer(timeSpan, DoOnTimer)");
                            Device.StartTimer(timeSpan, DoOnTimer);
                        }
                        return; // EXIT
                    }

                    // we have some page, add it to pages
                    allPages.Add(somePage);
                    proceedDownloading = somePage.next_page > 0;
                    pageIndex++;
                }

                Debug.WriteLine("Favorites -> DownloadPost -> downloaded  allPages.Count = " + allPages.Count);

                // download all the content
                for (int i = 0; i < allPages.Count; i++) {
                    Debug.WriteLine("Favorites -> for loop");
                    // download 2 audios here
                    // translation
                    bool result = true;
                    if (allPages[i].engTrans != null && !string.IsNullOrEmpty(allPages[i].engTrans.audio)) {
                        Debug.WriteLine("Favorites -> for loop2");
                        result = await GetAudioAsync(allPages[i].engTrans.audio, "BOOK" + UserHelper.Lang_cat + "_" +  elementToAdd.Key + "_" + i + "transl.mp3");
                    }
                    Debug.WriteLine("Favorites -> for loop3");
                    if (!string.IsNullOrEmpty(allPages[i].data.audio)) {
                        result = result & (await GetAudioAsync(allPages[i].data.audio, "BOOK" + UserHelper.Lang_cat + "_"+ elementToAdd.Key + "_"  + i + ".mp3"));
                    }
                    if (!result)
                    {
                        Debug.WriteLine("Favorites -> error while loading audios for pages..., cur page = " + i);
                        //either error in request // or no connection
                        TimeSpan timeSpan;
                        if (CrossConnectivity.Current.IsConnected && DownloadHelper.DownloadHelper.CheckInternetConnectionProgressive())
                        {
                            timeSpan = TimeSpan.FromSeconds(DOWNLOAD_RETRY_TIMESPAN_CONNECTION);
                        }
                        else
                        {
                            timeSpan = TimeSpan.FromSeconds(DOWNLOAD_RETRY_TIMESPAN_NO_CONNECTION);
                        }
                        // and start timer with task ONLY IF WE HAVE NO ACTIVE TIMER !!!
                        IsDownloading = false;
                        if (!IsTimerOn)
                        {
                            IsTimerOn = true;
                            Debug.WriteLine("Favorites -> DownloadBook -> Device.StartTimer(timeSpan, DoOnTimer)");
                            Device.StartTimer(timeSpan, DoOnTimer);
                        }
                        return; // EXIT
                    }

                    // preload ffimages 
                    Debug.WriteLine("Favorites -> DownloadBook -> preload image: " + allPages[i].data.image);
                    ImageService.Instance.LoadUrl(allPages[i].data.image).Preload().Configuration.DiskCacheDuration = TimeSpan.FromDays(30);
                }
                // everything is OK- cache all data, and 
                await CacheHelper.Add<BookPageResponse.BookPage[]>(CacheHelper.BOOK + UserHelper.Lang_cat + "_" + elementToAdd.Key, allPages.ToArray());
                AddToCurrentFinal(elementToAdd.Key, elementToAdd.Value);

                // continue downloading 
                if (!IsTimerOn)
                {
                    IsTimerOn = true;
                    Debug.WriteLine("Favorites -> DownloadBook -> continue downloading - start next timer");
                    Device.StartTimer(TimeSpan.FromSeconds(DOWNLOAD_RETRY_TIMESPAN_NO_CONNECTION), DoOnTimer);
                }
                IsDownloading = false;
            });
        }

        static Task<bool> GetAudioAsync(string uri, string filename)
        {
            Debug.WriteLine("Favorites -> GetAudioAsync -> Try my HttpWebRequest ..., uri = " + uri);
            return Task.Run(async () =>
            {
                bool result = false;
                HttpWebRequest webReq;
                Uri url = new Uri(uri);

                byte[] bytes = default(byte[]); //null;
                var memstream = new MemoryStream();
                int totalBytes = 0;
                try
                {
                webReq = (HttpWebRequest)HttpWebRequest.Create(url);
                    webReq.Timeout = 120000;

                    Debug.WriteLine("Favorites -> GetAudioAsync -> Try my HttpWebRequest --- ..., uri = " + uri);
                    webReq.CookieContainer = new CookieContainer();
                    webReq.Method = "GET";
                    using (WebResponse response = webReq.GetResponse())
                    {
                        totalBytes = Int32.Parse(response.Headers[HttpResponseHeader.ContentLength]);
                        Debug.WriteLine("totalBytes = " + totalBytes);
                        using (Stream stream = response.GetResponseStream())
                        {
                            // === OLD WAY
                            await stream.CopyToAsync(memstream);
                            bytes = memstream.ToArray();
                            Debug.WriteLine("Favorites -> GetAudioAsync -> Try to save audio to file..." + filename);
                            if (bytes.Length == totalBytes) await PCLHelper.SaveImage(bytes, filename);
                            if (await PCLHelper.IsFileExistAsync(filename))
                            {
                                Debug.WriteLine("Favorites -> GetAudioAsync -> file successfully saved! " + filename);
                                result = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    GoogleAnalytics.Current.Tracker.SendEvent("Favorites -> GetAudioAsync", $"Exception, url= {url}", "GetResponse().ex: " + ex.Message, 3);
                    Debug.WriteLine("Favorites -> GetAudioAsync -> HttpWebRequest EXCEPTION :(, audio not loaded _InBckground, uri : " + uri);
                }
                return result;
            });
        }

        static Task<BookPageResponse.BookPage> DownloadPage(int pageIndex, string book_id)
        {
            Debug.WriteLine("Favorites -> download pageIndex : " + pageIndex);
            return Task.Run(async () =>
            {
                BookPageResponse bookPageResponse = null;
                if (CrossConnectivity.Current.IsConnected && DownloadHelper.DownloadHelper.CheckInternetConnectionProgressive())
                {
                    string postData = $"lang_id={LANGUAGES.CAT_INFO[UserHelper.Lang_cat].Id}";
                   postData += $"&book_id={book_id}";
                   postData += $"&page={pageIndex}";
                        bookPageResponse = await ServerApi.PostRequestProgressive<BookPageResponse>(ServerApi.BOOK_URL, postData, null);
                        if (bookPageResponse == null)
                        {
                            Analytics.SendResultsRegular("Favorites", bookPageResponse, bookPageResponse?.error, ServerApi.BOOK_URL, postData);
                            Debug.WriteLine("Favorites -> Error while getting data from server!");
                            return null;
                        }
                        else if (bookPageResponse.error != null)
                        {
                            Analytics.SendResultsRegular("Favorites", bookPageResponse, bookPageResponse?.error, ServerApi.BOOK_URL, postData);
                            Debug.WriteLine("Favorites -> Error while getting data from server! Error: " + bookPageResponse.error.message);
                            return null;
                        }
                        else if (bookPageResponse.result == null)
                        {
                            Analytics.SendResultsRegular("Favorites", bookPageResponse, bookPageResponse?.error, ServerApi.BOOK_URL, postData);
                            Debug.WriteLine("Favorites -> Error while getting data from server! null page info!" + bookPageResponse.error.message);
                            return null;
                        }

                        string result = JsonConvert.SerializeObject(bookPageResponse);
                        Debug.WriteLine("Favorites -> bookPageResponse = " + result);

                }
                else
                {
                    Debug.WriteLine("Favorites -> DownloadPage -> No internet connection");
                    return null;
                }
                return bookPageResponse.result;
            });
        }

        static Task DownloadPost(KeyValuePair<string, FavoriteListCoords> elementToAdd)
        {
            return Task.Run(async () =>
            {
                // check if we have post (cashed)
                PostResponse.Post post = null;
                PostResponse postResponse;
                bool connection = true;
                if (await CacheHelper.Exists(CacheHelper.POST + elementToAdd.Key)) {
                    Debug.WriteLine("Favorites -> DownloadPost -> we have the post cashed: " + CacheHelper.POST + elementToAdd.Key);
                    post = await CacheHelper.GetAsync<PostResponse.Post>(CacheHelper.POST + elementToAdd.Key);
                }
                else if (CrossConnectivity.Current.IsConnected && DownloadHelper.DownloadHelper.CheckInternetConnectionProgressive()) 
                { // if we don't have it - try to download
                    connection = false;
                    Debug.WriteLine("Favorites -> DownloadPost -> don't have the post, try to download, id: " + elementToAdd.Key);
                    string userId = UserHelper.Login.user_id;
                    var postData = $"id={elementToAdd.Key}";
                    postData += $"&user_id={userId}";
                    postData += $"&type={""}";
                    postResponse = await ServerApi.PostRequestProgressive<PostResponse>(ServerApi.POST_URL, postData, null);
                    Debug.WriteLine("Favorites -> DownloadPost -> post = " + JsonConvert.SerializeObject(postResponse));
                    if (postResponse != null)
                    {
                        if (postResponse.error != null)
                        {
                            Debug.WriteLine("Favorites -> DownloadPost -> Error!" + postResponse.error.message);
                            Analytics.SendResultsRegular("Favorites", postResponse, postResponse?.error, ServerApi.POST_URL, postData);
                        }
                        else
                        {
                            Debug.WriteLine("Favorites -> DownloadPost -> Process response...");
                            if (postResponse.result != null)
                            {
                                post = postResponse.result;
                                Debug.WriteLine("got result from server, post == null? : " + (post == null));
                                // and cache it
                                await CacheHelper.Add<PostResponse.Post>(CacheHelper.POST + elementToAdd.Key, post);
                                Debug.WriteLine($"POST id = {elementToAdd.Key} successfully cached ?: {await CacheHelper.Exists(CacheHelper.POST + elementToAdd.Key)}");                                
                            }
                            else
                            {
                                Debug.WriteLine("Favorites -> DownloadPost -> Error! - Response from server is NULL");
                                Analytics.SendResultsRegular("Favorites", postResponse, postResponse?.error, ServerApi.POST_URL, postData);
                            }
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Favorites -> DownloadPost -> Error! - No response from server");
                        Analytics.SendResultsRegular("Favorites", postResponse, postResponse?.error, ServerApi.POST_URL, postData);
                    }
                }
                else
                {
                    Debug.WriteLine("Favorites -> DownloadPost -> No internet connection"); 
                }
                
                // here we must analyze rezults, an make some choices 
                if (post == null) {
                    //either error in request // or no connection
                    TimeSpan timeSpan;
                    if (connection) {
                        timeSpan = TimeSpan.FromSeconds(DOWNLOAD_RETRY_TIMESPAN_CONNECTION);
                    }
                    else {
                        timeSpan = TimeSpan.FromSeconds(DOWNLOAD_RETRY_TIMESPAN_NO_CONNECTION);
                    }
                    // and start timer with task ONLY IF WE HAVE NO ACTIVE TIMER !!!
                    IsDownloading = false;
                    if (!IsTimerOn) 
                    {
                        IsTimerOn = true;
                        Debug.WriteLine("Favorites -> DownloadPost -> Device.StartTimer(timeSpan, DoOnTimer)"); 
                        Device.StartTimer(timeSpan, DoOnTimer);
                    }
                }
                else { // everything is OK, try to download and save content
                    await DownloadVideoFile(post, elementToAdd);
                }

                return;
            });
        }

        static Task DownloadVideoFile(PostResponse.Post post, KeyValuePair<string, FavoriteListCoords> elementToAdd) {
            Debug.WriteLine("Favorites -> DownloadPost -> DownloadVideoFile, url: " + post.content); 
            return Task.Run(async () => {
                bool isOK = true;

                await Task.Delay(10);

                string uri = post.content;
                int index1 = uri.IndexOf("\"]");
                if (index1 != -1)
                {
                    uri = uri.Remove(index1);
                }
                uri = uri.Replace("[dlol_player url=\"", string.Empty).Replace(" ", "%20").Replace($"'", "%27");
                Debug.WriteLine("Try to download uri = " + uri);

                if (!CrossConnectivity.Current.IsConnected || !DownloadHelper.DownloadHelper.CheckInternetConnectionProgressive()) {
                    // have no connection here
                    if (!IsTimerOn)
                    {
                        IsTimerOn = true;
                        Debug.WriteLine("Favorites -> DownloadPost -> Device.StartTimer(no connection)");
                        Device.StartTimer(TimeSpan.FromSeconds(DOWNLOAD_RETRY_TIMESPAN_NO_CONNECTION), DoOnTimer);
                    }
                }
                else {
                    isOK = await DownloadHelper.DownloadHelper.GetVideoAsync_InBckground(uri, post.id + ".mp4");
                    if (isOK)
                    { // downloaded OK
                      Debug.WriteLine("Favorites -> DownloadPost -> DownloadVideoFile -> video file downloaded, and saved, -> update lists");
                      AddToCurrentFinal(elementToAdd.Key, elementToAdd.Value);
                    }
                    // continue downloading 
                    if (!IsTimerOn)
                    {
                        IsTimerOn = true;
                        Debug.WriteLine("Favorites -> DownloadPost -> continue downloading - start next timer");
                                Device.StartTimer(TimeSpan.FromSeconds(DOWNLOAD_RETRY_TIMESPAN_NO_CONNECTION), DoOnTimer);
                    }
                }
                IsDownloading = false;
                return;
            });
        }

        static bool DoOnTimer () {
            Debug.WriteLine("Favorites -> DownloadPost -> DoOnTimer"); 
            IsTimerOn = false;
            if (IsDownloading) {
                // allready is downloading - do nothing ...
            }
            else {
                if (!UserHelper.IsFree) StartLoading();
            }

            return false;
        }       
    }

    public class FavoriteListCoords
    {
        public MainPage_ViewModel.CENTRAL_VIEWS CentralView { get; set; }
        public int SubView { get; set; }
        public int index { get; set; }
    }

    public class FavoritVisualItem
    {
        static ImageSource heart_color_img = Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.COMMON.heart_color.png");
        // static ImageSource heart_preloader_img = Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.COMMON.preloader_favorites.gif");
        static ImageSource heart_preloader_img = heart_color_img;
        static ImageSource heart_gray_img = Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.COMMON.heart_gray.png");

        ListViewItem listViewItem;
        public FavoriteListCoords FavListCoords { get; set; } 
        public TestViewModel TestViewModel { get; set; }  
        public int ListIndex { get; set; }
        public bool IsSingle { get; set;  }

        public void OnDownloadedOK () {
            if (FavListCoords.CentralView != TestViewModel.viewType.CentralView || FavListCoords.SubView != TestViewModel.viewType.SubView) return;
            listViewItem = TestViewModel.ListItems[ListIndex];

                if (IsSingle)
                {
                    listViewItem.FavoritesImagesource = heart_color_img;
                }
                else
                {
                    (listViewItem as DoubleItem).FavoritesImagesource2 = heart_color_img;
                }


        } 

        public void OnAdded () {
            if (FavListCoords.CentralView != TestViewModel.viewType.CentralView || FavListCoords.SubView != TestViewModel.viewType.SubView) return;
            listViewItem = TestViewModel.ListItems[ListIndex];

            if (IsSingle)
            {
                listViewItem.FavoritesImagesource = heart_preloader_img;
            }
            else
            {
                (listViewItem as DoubleItem).FavoritesImagesource2 = heart_preloader_img;
            } 
        }

        public void OnRemoved () {
            if (FavListCoords.CentralView != TestViewModel.viewType.CentralView || FavListCoords.SubView != TestViewModel.viewType.SubView) return;
            listViewItem = TestViewModel.ListItems[ListIndex];

            if (IsSingle)
            {
                listViewItem.FavoritesImagesource = heart_gray_img;
            }
            else
            {
                (listViewItem as DoubleItem).FavoritesImagesource2 = heart_gray_img;
            } 
        }

    }
}
