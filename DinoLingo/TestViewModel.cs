using System;
using System.Net;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Connectivity;
using Xamarin.Forms;
using System.Text;
using System.IO;
using FFImageLoading;

namespace DinoLingo
{
    public class TestViewModel: INotifyPropertyChanged
    {
        static int ERROR_PREFIX = 140;

        public double SubViewBtnSize { get; set; }
       public event PropertyChangedEventHandler PropertyChanged;
       public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public MainPage_ViewModel rootViewModel;

        string cur_cats = ""; // list of categories
        int targetCatId = -1;       

        List<ListViewItemData> dataForListView;
        ListView listView;

        ObservableCollection<ListViewItem> tempListItems;
        ObservableCollection<ListViewItem> listItems;
        public ObservableCollection<ListViewItem> ListItems { 
            get {
                return listItems;
            } 
            set{
                listItems = value;
                OnPropertyChanged();
            }
        }

        public FavoriteListCoords viewType;

        int [][] curPosition = new int[][] {
           new int[] { 0 }, // LESSONS_AND_GAMES
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }, // BOOKS
            new int[] { 0 }, // STORIES
            new int[] { 0 }, // SONGS
        };
        int curPos = 0;
        DateTime firstItemAppearingTime;

        List<ListViewItem> visibleItems = new List<ListViewItem>();        

        bool isAnimating;
        Object animLock = new Object();
        public bool IsAnimating
        {
            get
            {
                if (animLock != null) lock (animLock)
                    {
                        return isAnimating;
                    }
                else return true;
            }
            set
            {
                if (animLock != null) lock (animLock)
                {
                    isAnimating = value;
                }

            }
        }

        public bool isBlocked;

        // local data
        //double height = -1;
        //
        bool isLoading = false;

        // === UI visual resources ===
        // ***************************
        public static ImageSource heart_color_img = Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.COMMON.heart_color.png");
        //ImageSource heart_preloader_img = Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.COMMON.preloader_favorites.gif");
        public static ImageSource heart_preloader_img = Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.COMMON.heart_color.png");
        public static ImageSource heart_gray_img = Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.COMMON.heart_gray.png");
        static ImageSource loading_img = "my_loading_512.gif";

        static ImageSource background_book = Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.cloud_white.png");
        public static ImageSource background_lessons = Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.img_back_gray_02.png");

        Thickness mainImagePadding_book, mainImagePadding_lessons;

        public static ImageSource[] stars_imgs = {    null, 
                                        Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.COMMON.small_star_1.png"),
                                        Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.COMMON.small_star_2.png"),
                                        Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.COMMON.small_star_3.png"),
                                        Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.COMMON.small_star_4.png"),
                                        Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.COMMON.small_star_5.png"),
                                        };
        string[] headers;  //= new string[] { " Lessons and Games", " Books", " Stories", " Songs"};
        

        Color HEADER_COLOR_NORMAL;
        Color HEADER_COLOR_BRIGHT;

        // sub cats bar
        StackLayout rootStack, subCatsStack;
        Image[] subCatsImgs;
        static ImageSource[] subCats = {
            Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.btn_books_lv_1.png"),
            Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.btn_books_lv_2.png"),
            Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.btn_books_lv_3.png"),
            Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.btn_books_lv_4.png"),
            Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.btn_books_multilevel.png"),
            Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.btn_books_fun.png"),
            Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.btn_books_dinosaur.png"),
        };
        static ImageSource[] subCatsP = {
            Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.btn_books_lv_1_p.png"),
            Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.btn_books_lv_2_p.png"),
            Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.btn_books_lv_3_p.png"),
            Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.btn_books_lv_4_p.png"),
            Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.btn_books_multilevel_p.png"),
            Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.btn_books_fun_p.png"),
            Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.btn_books_dinosaur_p.png"),
        };
        

        public TestViewModel(FavoriteListCoords centralViewType, MainPage_ViewModel rootViewModel, ListView listView, StackLayout rootStack, StackLayout subCatsStack)
        {
            viewType = centralViewType;            

            Device.StartTimer(TimeSpan.FromMilliseconds(100), FillTheViewOnStartAsync);
            this.rootViewModel = rootViewModel;
            this.rootViewModel.testViewModel = this;
            this.listView = listView;
            this.rootStack = rootStack;
            this.subCatsStack = subCatsStack;
            
            string lang_ = LANGUAGES.CAT_INFO[UserHelper.Lang_cat].Name.Replace('-', '_');
            //= new string[] { " Lessons and Games", " Books", " Stories", " Songs"};
            headers = new string[]
            {
                string.Format(Translate.GetString("header_for_kids"), LANGUAGES.CAT_INFO[UserHelper.Lang_cat].VisibleName).FirstLetterToUpperCase(),
                Translate.GetString("header_books_" + lang_),
                Translate.GetString("header_stories_" + lang_),
                Translate.GetString("header_songs_" + lang_),
            };
            Debug.WriteLine("TEST -> Translate.GetString(header_for_kids) = " + Translate.GetString("header_for_kids"));
            Debug.WriteLine("TEST -> headers[0] " + headers[0]);

            HEADER_COLOR_NORMAL = (Color) Application.Current.Resources["BackgroundBlueHeaderColor"];
            HEADER_COLOR_BRIGHT = (Color) Application.Current.Resources["BackgroundBrightBlueHeaderColor"];

            mainImagePadding_book = new Thickness(UI_Sizes.ScreenHeightX_UNIFORMED_TO_1_78 * 0.06);
            mainImagePadding_lessons = new Thickness(0);

           
            // set SetSubCatBar
            CreateSubCatBar();
            SwitchSubCatBar();

            isBlocked = false;
        }

        public void CreateSubCatBar()
        {
            SubViewBtnSize = UI_Sizes.CloseBtnSize * 0.9;
            subCatsImgs = new Image[subCatsStack.Children.Count];


            for (int i = 0; i < subCatsStack.Children.Count; i++)
            {
                subCatsImgs[i] = subCatsStack.Children[i] as Image;
                if (viewType.SubView == i) subCatsImgs[i].Source = subCatsP[i];
                else subCatsImgs[i].Source = subCats[i];
                if (LANGUAGES.CAT_INFO[UserHelper.Lang_cat].HasBookInSubCats[i] == 0)
                {
                    subCatsImgs[i].IsEnabled = subCatsImgs[i].IsVisible = false;
                }
                else
                {
                    subCatsImgs[i].IsEnabled = subCatsImgs[i].IsVisible = true;
                }

                Debug.WriteLine("TestViewModel -> Created subCat indx = " + i);
            }
        }

        public void SwitchSubCatBar()
        {
            if (viewType.CentralView == MainPage_ViewModel.CENTRAL_VIEWS.BOOKS)
            {
                subCatsStack.IsEnabled = subCatsStack.IsVisible = true;
                SubViewBtnSize = UI_Sizes.CloseBtnSize * 0.9;
            }
            else
            {
                subCatsStack.IsEnabled = subCatsStack.IsVisible = false;
                SubViewBtnSize = 0;
            }
            
        }

        public void Handle_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
           // check if it is first item
           if (visibleItems.Count == 0)
           {
               firstItemAppearingTime = DateTime.Now;
               Debug.WriteLine("TestViewModel -> Handle_ItemAppearing -> first item appeared, index = " + (e.Item as ListViewItem).listIndex);
                curPos = (e.Item as ListViewItem).listIndex;
           }
           else if ((DateTime.Now - firstItemAppearingTime).CompareTo(TimeSpan.FromMilliseconds(400)) > 0)
           {
                Debug.WriteLine($"TestViewModel -> Handle_ItemAppearing ->dt, msec= {(DateTime.Now - firstItemAppearingTime).TotalMilliseconds}, last item appeared, index = {(e.Item as ListViewItem).listIndex}");
                curPos = (e.Item as ListViewItem).listIndex;
            }

            visibleItems.Add(e.Item as ListViewItem);
            //curPosition[(int)viewType] = visibleItems[0];
            //Debug.WriteLine("curPosition = " + curPosition[(int)viewType].listIndex);
            Debug.WriteLine("visibleItems.Count = " + visibleItems.Count);
            if (isLoading || ListItems.Count == 0)
                return;
            
            //preload images ...
            int nextIndex = (e.Item as ListViewItem).listIndex + 1;
            if (nextIndex < ListItems.Count - 1) {
                //preload images here ...
                string url;
                if (e.Item.GetType() == typeof(SingleItem)) {
                    url = (e.Item as SingleItem).ImageUrl;
                    if (!string.IsNullOrEmpty(url)) {
                        Debug.WriteLine("PRELOAD: "+ url);
                        ImageService.Instance.LoadUrl(url).Preload();
                    }
                }
                else if (e.Item.GetType() == typeof(DoubleItem))
                {
                    url = (e.Item as DoubleItem).ImageUrl;
                    if (!string.IsNullOrEmpty(url)) {
                        //Debug.WriteLine("PRELOAD + DOWNLOAD: " + url);
                        ImageService.Instance.LoadUrl(url).Preload(); //.Configuration.TryToReadDiskCacheDurationFromHttpHeaders = false;
                        //ImageService.Instance.LoadUrl(url).DownloadOnly();
                    }
                    url = (e.Item as DoubleItem).ImageUrl2;
                    if (!string.IsNullOrEmpty(url)) {
                        //Debug.WriteLine("PRELOAD + DOWNLOAD : " + url);
                        ImageService.Instance.LoadUrl(url).Preload(); //.Configuration.TryToReadDiskCacheDurationFromHttpHeaders = false;
                        //ImageService.Instance.LoadUrl(url).DownloadOnly();
                    }
                }
            }


            if (e.Item == ListItems[ListItems.Count - 1] && viewType.CentralView == MainPage_ViewModel.CENTRAL_VIEWS.BOOKS) {
                if (dataForListView.Count > 0 && dataForListView[dataForListView.Count -1].id == "downloading...")
                {
                    //***15.02.19 if (dataForListView[ListItems[ListItems.Count - 1].Index].id == "downloading...") {
                    Debug.WriteLine("try to download books...");
                    LoadItems(ListItems.Count * 2);
                }

            }
        }
        public void Handle_ItemDisappearing(object sender, ItemVisibilityEventArgs e)
        {
           // Debug.WriteLine("item disappearing, listIndex = " + (e.Item as ListViewItem).listIndex);
            visibleItems.Remove(e.Item as ListViewItem);
        }
        async Task LoadItems(int bookOffset)
        {
            isLoading = true;
            //download more books here
            TryToDownloadContent(bookOffset);
        }

        bool FillTheViewOnStartAsync () {
            Debug.WriteLine("FillTheViewOnStartAsync() ... ");
            //ListItems = null;
            TryToShowContent();
            return false;
        }

        public async void TryToShowContent () {
            Debug.WriteLine("TryToShowContent...");
            string lang = UserHelper.Language;
            cur_cats = UserHelper.Lang_cat;
            // check, if we have cached content FOR THE current central view
            string viewTypeKey = CacheHelper.POST_LIST_ITEM_DATA + "_" + cur_cats + "_" + UserHelper.Login.user_id + "_" + viewType.CentralView + "_" + viewType.SubView;

            if (await CacheHelper.Exists(viewTypeKey)) { // we Do have the content - just show it
                Debug.WriteLine("Test view -> we have content data: " + (await CacheHelper.GetAsync(viewTypeKey)).Data);

                dataForListView = new List<ListViewItemData>(await CacheHelper.GetAsync<ListViewItemData[]>(viewTypeKey));
                // and now we can update favorites data
                Favorites.allListsData[(int)viewType.CentralView][viewType.SubView] = dataForListView.ToArray();

                Debug.WriteLine("Test view -> UpdateObservableCollectionFull");
                UpdateObservableCollectionFull(dataForListView);
                Debug.WriteLine("Test view -> ShowList(true)");
                ShowList(true);
            }
            else // we need connection here 
            {
                Debug.WriteLine("Test view -> we do not have cats data...");
                TryToDownloadContent();
            }
        }

        Task DownloadContentAsync(FavoriteListCoords oldView) {
            return Task.Run(async ()=> {
                Debug.WriteLine("Test view -> DownloadContentAsync...");
                //check login here
                int downloadResult = await CheckDownload(0);

                isLoading = false;
                //Debug.WriteLine("downloadResult = " + downloadResult + ", total items in list :" + ListItems.Count);
                switch (downloadResult)
                {
                    case 0: // no connection
                        if (AsyncMessages.CheckConnectionTimeout())
                        {
                            AsyncMessages.DisplayAlert(POP_UP.OOPS,
                            POP_UP.NO_CONNECTION
                            //+ POP_UP.GetCode(null, ERROR_PREFIX + 1)
                            , POP_UP.OK);
                            //("Error!", "No internet connection", "ОK");
                        }

                        break;

                    case 1: // CategoryResponse read with null result! - some error
                        if (AsyncMessages.CheckDisplayAlertTimeout())
                        {
                            AsyncMessages.DisplayAlert(POP_UP.OOPS,
                        POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(null, ERROR_PREFIX + 2), POP_UP.OK);
                            //("Error!", "CategoryResponse read with null result!", "ОK");
                        }

                        break;
                    case 2: // error in response
                        if (AsyncMessages.CheckDisplayAlertTimeout())
                        {
                            AsyncMessages.DisplayAlert(POP_UP.OOPS,
                        POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(null, ERROR_PREFIX + 3), POP_UP.OK);
                            //("Error!", "Error in response from server", "ОK");
                        }

                        break;

                    case 3: // download - ok
                        Debug.WriteLine("download - ok");

                        if (viewType.CentralView == oldView.CentralView && viewType.SubView == oldView.SubView) {
                            Device.BeginInvokeOnMainThread(DownloadOKAlert);
                        }
                        else {
                            Debug.WriteLine("oldView = " + oldView.CentralView + "/" + oldView.SubView);
                            Debug.WriteLine("viewType = " + viewType.CentralView + "/" + viewType.SubView);
                        }
                        break;
                    default: break;
                }
            });
        }

        async void DownloadOKAlert() {
            Debug.WriteLine("TestViewModel -> DownloadOKAlert -> Async Download ok");
            

            // need to check favorites here
            if (!Favorites.ListsAreBuisy)
            {
                 UpdateObservableCollectionFull(dataForListView);
                //ShowList();
                visibleItems = new List<ListViewItem>();
                ListItems = tempListItems;
            }
            else
            {
                Debug.WriteLine("TestViewModel -> DownloadOKAlert -> SKIPPED UPDATE *******************************************");
                Debug.WriteLine("TestViewModel -> DownloadOKAlert -> LISTS ARE BUISY *******************************************");
            }
            
        }

        async void TryToDownloadContent(int bookOffset = 0) {
            //check login here
            
            int downloadResult = await CheckDownload(bookOffset);
            isLoading = false;
            //Debug.WriteLine("downloadResult = " + downloadResult + ", total items in list :" + ListItems.Count);
            switch (downloadResult)
            {
                case 0: // no connection
                    if (AsyncMessages.CheckConnectionTimeout())
                    {
                        await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                        POP_UP.NO_CONNECTION
                        //+ POP_UP.GetCode(null, ERROR_PREFIX + 4)
                        , POP_UP.OK);
                        //("Error!", "No internet connection", "ОK");
                    }
                    break;

                case 1: // CategoryResponse read with null result! - some error
                    await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                        POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(null, ERROR_PREFIX + 5), POP_UP.OK);
                    //("Error!", "CategoryResponse read with null result!", "ОK");
                    break;                
                case 2: // error in response - postlist_resp == null - i do not know what the fuck
                    await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                       POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(null, ERROR_PREFIX + 6), POP_UP.OK);
                    //("Error!", "Error in response from server", "ОK");
                    break;

                case 3: // download - ok
                    Debug.WriteLine("download - ok");
                    UpdateObservableCollectionFull(dataForListView);
                    ShowList(bookOffset == 0, false);
                    
                    break;
   
                case 4: // download, but user switche the cat- ok
                    Debug.WriteLine("download - ok, BUT USER CHANGED THE CATEGORY");
                    break;
                default: break;
            }
        }

        Task<int> CheckDownload(int bookOffset)
        {
            Debug.WriteLine("CheckDownload() here...");
            return Task.Run(async () =>
            {
                // 0 - check internet connection              

                if (!CrossConnectivity.Current.IsConnected || !DownloadHelper.DownloadHelper.CheckInternetConnectionProgressive())
                {
                    return 0; // no connection
                }
                // 2 - do post request
                else {
                    CategoryResponse catResp = await CacheHelper.GetAsync<CategoryResponse>(CacheHelper.CATEGORYS_RESPONSE + cur_cats);

                    Debug.WriteLine("downloaded cats... ");
                    if (catResp != null) {
                        Debug.WriteLine($"cat_id: {cur_cats}, cats: {JsonConvert.SerializeObject(catResp)}");
                        if (catResp.error != null) return 2; // error in response from server

                        // make from it PostCategoryIds
                        PostCategoryIds postCategoryIds = new PostCategoryIds(catResp);
                        //and save it for the lang_cat
                        await CacheHelper.Add<PostCategoryIds>(CacheHelper.POST_CATEGORY_IDS + cur_cats, postCategoryIds);

                        FavoriteListCoords oldView = new FavoriteListCoords { CentralView = viewType.CentralView, SubView = viewType.SubView };
                        //try to download postlist for the catId
                        string postData;
                        PostListResponse postlistResp;
                        string url;
                        if (oldView.CentralView == MainPage_ViewModel.CENTRAL_VIEWS.BOOKS) {
                            postData = $"lang_id={LANGUAGES.CAT_INFO[cur_cats].Id}";
                            postData += $"&sort=3";
                            postData += $"&offset={bookOffset}";
                            postData += $"&limit={20}";
                            postData += $"&cat={MainPage_ViewModel.SUBCATS[(int)oldView.CentralView][oldView.SubView]}";
                            url = ServerApi.BOOK_LIST_URL;
                            
                        }
                        else {
                            // get needed category id from viewType
                            targetCatId = postCategoryIds.GetIdFromViewType(oldView.CentralView);
                            Debug.WriteLine("targetCatId = " + targetCatId);

                            postData = $"cat={targetCatId}";
                            postData += $"&user_id={UserHelper.Login.user_id}";
                            postData += $"&lang_id={Translate.LangId}";
                            url = ServerApi.URL_POSTLIST;                            
                        }
                        postlistResp = await ServerApi.PostRequestProgressive<PostListResponse>(url, postData, null);

                        Debug.WriteLine("downloaded postlistResp... ");
                        if (postlistResp != null)
                        {
                            Debug.WriteLine($"targetCatId: {targetCatId}, postListResponse: {JsonConvert.SerializeObject(postlistResp)}");
                            if (postlistResp.error != null)
                            {
                                Analytics.SendResultsRegular("TestViewModel", postlistResp, postlistResp?.error, url, postData);
                                return 2; // error in response from server
                            }

                            //everything is OK here - convert postListResponse into  and save the data...


                            if (oldView.CentralView == MainPage_ViewModel.CENTRAL_VIEWS.BOOKS & bookOffset > 0)
                            {
                                // restore data
                                string viewTypeKey = CacheHelper.POST_LIST_ITEM_DATA + "_" + cur_cats + "_" + UserHelper.Login.user_id + "_" + oldView.CentralView + "_" + oldView.SubView;

                                if (await CacheHelper.Exists(viewTypeKey))
                                { // we Do have cached data
                                    Debug.WriteLine("Test view -> we have precached data: " + (await CacheHelper.GetAsync(viewTypeKey)).Data);

                                    dataForListView = new List<ListViewItemData>(await CacheHelper.GetAsync<ListViewItemData[]>(viewTypeKey));
                                }
                                else // we need connection here 
                                {
                                    Debug.WriteLine("Test view -> we do not have precached data...");
                                    dataForListView = new List<ListViewItemData>();
                                }


                                if (dataForListView.Count > 0) dataForListView.RemoveAt(dataForListView.Count - 1);
                                if (postlistResp.result != null & postlistResp.result.Length > 0)
                                {                                    
                                    List<ListViewItemData> AdditionaldataForListView = ConvertFromPostListResponse(postlistResp);
                                    dataForListView.AddRange(AdditionaldataForListView);
                                }                                
                            }

                            else dataForListView = ConvertFromPostListResponse(postlistResp);

                            // save list to cache
                            Debug.WriteLine("try to write dataForListView[] to cache ...");
                            await CacheHelper.Add<ListViewItemData[]>(CacheHelper.POST_LIST_ITEM_DATA + "_" + cur_cats + "_" + UserHelper.Login.user_id + "_" + oldView.CentralView + "_" + oldView.SubView, dataForListView.ToArray());
                            Debug.WriteLine("TestViewModel -> saved to cache successfully? dataForListView[] :" + await CacheHelper.Exists(CacheHelper.POST_LIST_ITEM_DATA + "_" + cur_cats + "_" + UserHelper.Login.user_id + "_" + oldView.CentralView + "_" + oldView.SubView));
                            Debug.WriteLine("saved data is: " + (await CacheHelper.GetAsync(CacheHelper.POST_LIST_ITEM_DATA + "_" + cur_cats + "_" + UserHelper.Login.user_id + "_" + oldView.CentralView + "_" + oldView.SubView)).Data);

                            // and now we can update favorites data
                            Favorites.allListsData[(int)oldView.CentralView][oldView.SubView] = dataForListView.ToArray();

                            // check if user changed the category
                            if (oldView.CentralView == viewType.CentralView && oldView.SubView == viewType.SubView) return 3;
                            else return 4; // user changed a category
                        }
                        else
                        {
                            Analytics.SendResultsRegular("TestViewModel", postlistResp, postlistResp?.error, url, postData);
                            //postlistResp == null

                            return 2;
                        }
                        
                    }
                    else {
                        return 1; // CategoryResponse read with null result! - some error
                    }
                    //int postsListDownloaded = await PostRequest<PostList>(URL_POST_LIST, cat_id);
                }

            });
        }

        

        public void UpdateCurrentPosition()
        {
            curPosition[(int)this.viewType.CentralView][this.viewType.SubView] = GetCurrentPosition_startDelay();
        }

        public void SwitchCenterView (FavoriteListCoords viewType) {
           
            curPosition[(int)this.viewType.CentralView][this.viewType.SubView] = GetCurrentPosition_startDelay();

            Debug.WriteLine("old curPosition[(int)viewType][viewType.SubView] = " + curPosition[(int)this.viewType.CentralView][this.viewType.SubView]);


            this.viewType = new FavoriteListCoords {CentralView = viewType.CentralView, SubView = MainPage_ViewModel.subCatIndex[(int)viewType.CentralView] } ;

            visibleItems = new List<ListViewItem>();

            //
            SwitchSubCatBar();

            TryToShowContent();
        }

        int GetCurrentPosition_startDelay()
        {
            return curPos;
        }

            int GetCurrentPosition()
        {
            int pos;

            Debug.WriteLine("TestViewModel -> GetCurrentPosition -> visibleItems:");
            foreach (ListViewItem item in visibleItems)
            {
                Debug.Write(item.listIndex + ", ");
            }

            if (visibleItems.Count > 2)
            {
                int pos1, pos2, pos3;
                pos1 = visibleItems[visibleItems.Count - 3].listIndex;
                pos2 = visibleItems[visibleItems.Count - 2].listIndex;
                pos3 = visibleItems[visibleItems.Count - 1].listIndex;

                if (pos3 > pos2 & pos2 > pos1)
                { // go down
                    pos = pos3 - 1;
                    Debug.WriteLine("GO DOWN...");
                }
                else if (pos3 < pos2 & pos2 < pos1)
                { // go up
                    pos = pos3 + 1;
                    Debug.WriteLine("GO UP...");
                }
                else
                {
                    pos = (visibleItems[visibleItems.Count - 1].listIndex + visibleItems[visibleItems.Count - 2].listIndex + visibleItems[visibleItems.Count - 3].listIndex) / 3;
                    Debug.WriteLine("AVERAGE...");
                }
            }
            else if (visibleItems.Count > 0)
            {
                pos = visibleItems[visibleItems.Count - 1].listIndex;
                Debug.WriteLine("LAST INDEX...");
            }
            else pos = 0;
            
            
            // detect min index!
            foreach (ListViewItem item in visibleItems)
            {
                if (item.listIndex < pos) pos = item.listIndex;
            }

            if (pos < 0) pos = 0;
            else if (pos >= ListItems.Count) pos = ListItems.Count - 1;

            return pos;
        }

        void UpdateObservableCollectionFull (List<ListViewItemData> dataForListView) {
            //List<ListViewItemData> dataForListView_loc = new List<ListViewItemData>(dataForListView);

            Debug.WriteLine("TestViewModel --> UpdateObservableCollection: dataForListView.Count=" + dataForListView.Count);
            tempListItems = new ObservableCollection<ListViewItem>();

            double cellH = UI_Sizes.MediumTextSize * 10;
            double headerH = cellH * 0.15;
            double fontSize = cellH * 0.047;
            double cellWidth;
            ImageSource background;
            Thickness imgPadding;
            GridLength fistRowHeight;           
            GridLength lastRowHeight;
           // double nameTranslationY; 

            Favorites.WaitIfListsAreBuisy("Testview -> UpdateObservableCollectionFull");
            
            //Debug.WriteLine("TestViewModel --> UpdateObservableCollection -> fill the list ...");
            string fontFamily = null;
            FontAttributes fontAttributes = FontAttributes.None;

            if (viewType.CentralView == MainPage_ViewModel.CENTRAL_VIEWS.LESSONS_AND_GAMES) {
                fontFamily = Translate.fontFamily;
                fontAttributes = Translate.fontAttributes;
            }

            
            if (viewType.CentralView != MainPage_ViewModel.CENTRAL_VIEWS.LESSONS_AND_GAMES && dataForListView.Count > 0 && dataForListView[0].Type == ListViewItemData.VISUAL_TYPE.HEADER) {
                dataForListView.RemoveAt(0);
            }
            Debug.WriteLine("TestViewModel --> UpdateObservableCollection (2*): dataForListView.Count=" + dataForListView.Count);

            if (viewType.CentralView != MainPage_ViewModel.CENTRAL_VIEWS.LESSONS_AND_GAMES) dataForListView.Insert(0, new ListViewItemData {
                Type = ListViewItemData.VISUAL_TYPE.SPACE,
                Name = string.Empty, TransName = string.Empty,
            });

            if (viewType.CentralView == MainPage_ViewModel.CENTRAL_VIEWS.LESSONS_AND_GAMES) 
                dataForListView.Insert(0, new ListViewItemData
                {
                    Type = ListViewItemData.VISUAL_TYPE.HEADER_WITH_IMAGE,
                    Name = headers[(int)viewType.CentralView], TransName = headers[(int)viewType.CentralView],
                });
            else
                dataForListView.Insert(0, new ListViewItemData
                {
                    Type = ListViewItemData.VISUAL_TYPE.HEADER,
                    Name = headers[(int)viewType.CentralView],                    TransName = headers[(int)viewType.CentralView],
                });

            // set some proportions
            if (viewType.CentralView == MainPage_ViewModel.CENTRAL_VIEWS.BOOKS) {
                cellWidth = cellH * 0.8;
                cellH = UI_Sizes.MediumTextSize * 7;
                background = background_book;
                imgPadding = mainImagePadding_book;
                fistRowHeight = new GridLength(cellH * 0.0 , GridUnitType.Absolute);
                lastRowHeight = new GridLength(0.5, GridUnitType.Star);
            }
            else {
                cellWidth = cellH * 0.8;
                background = viewType.CentralView == MainPage_ViewModel.CENTRAL_VIEWS.LESSONS_AND_GAMES? background_lessons : null;
                imgPadding = mainImagePadding_lessons;
                fistRowHeight = new GridLength(1, GridUnitType.Star);
                lastRowHeight = new GridLength(1, GridUnitType.Star);
            }

            for (int i = 0; i < dataForListView.Count; i++)
            {
                bool isNotAGame = Game.GetTypeFromTitle(dataForListView[i].Name, dataForListView[i].GameType) == GAME_TYPE.NOT_A_GAME;
                // check REAL type
                if (string.IsNullOrEmpty(dataForListView[i].id)) dataForListView[i].id = string.Empty;
                if (dataForListView[i].Type == ListViewItemData.VISUAL_TYPE.DOUBLE) {
                    if (i < dataForListView.Count -1 && dataForListView[i + 1].Type != ListViewItemData.VISUAL_TYPE.DOUBLE) {
                        
                        dataForListView[i].Type = ListViewItemData.VISUAL_TYPE.SINGLE;
                        Debug.WriteLine("id = " + dataForListView[i].id + ", SET TO SINGLE");
                    }
                } 

                switch (dataForListView[i].Type)
                {
                    case ListViewItemData.VISUAL_TYPE.HEADER:
                        double hFactor = 1;
                        //if (dataForListView[i].Name.Length > 15) hFactor = 2.2;
                        tempListItems.Add(
                                new HeaderItem {
                                    
                                    Name = System.Net.WebUtility.HtmlDecode(Uri.UnescapeDataString(string.IsNullOrEmpty(dataForListView[i].TransName)? dataForListView[i].Name: dataForListView[i].TransName)),
                                    Name_slug = System.Net.WebUtility.HtmlDecode(Uri.UnescapeDataString(dataForListView[i].Name)),
                                    Index = i,
                                    listIndex = tempListItems.Count,
                                    CellHeight = headerH * hFactor,
                                    //FontSize = headerH * 0.7,
                                    FontSize = headerH * 0.5,
                                    HeaderColor = (viewType.CentralView == MainPage_ViewModel.CENTRAL_VIEWS.LESSONS_AND_GAMES && i == 0) ? HEADER_COLOR_BRIGHT : HEADER_COLOR_NORMAL,
                                }
                            );

                        //Debug.WriteLine("Added templistItem, Name = " + dataForListView[i].Name + ", tempListItems.Count = " + tempListItems.Count);
                        break;

                    case ListViewItemData.VISUAL_TYPE.SINGLE:
                        Debug.WriteLine("single STARS = " + dataForListView[i].Stars);
                        tempListItems.Add( 
                            new SingleItem
                            {
                                
                                Name = System.Net.WebUtility.HtmlDecode(Uri.UnescapeDataString(string.IsNullOrEmpty(dataForListView[i].TransName) ? dataForListView[i].Name : dataForListView[i].TransName)),
                                Name_slug = System.Net.WebUtility.HtmlDecode(Uri.UnescapeDataString(dataForListView[i].Name)),
                                Index = i,
                                listIndex = tempListItems.Count,
                                Imagesource = dataForListView[i].ImgResource,
                                ImageUrl = dataForListView[i].ImgResource,
                                CellHeight = cellH,
                            CellWidth = cellWidth,
                            FontSize = fontSize,
                                // favorites
                                FavoritesSize = cellH * 0.2,
                                IsFavoritesVisible = isNotAGame,
                                FavoritesImagesource = Favorites.SetVisualSource(dataForListView[i].id, heart_color_img, heart_gray_img, heart_preloader_img),
                                StarsImagesource = dataForListView[i].Stars > 0 ? stars_imgs[dataForListView[i].Stars] : null,
                                MyFont = fontFamily,
                                FontAttributes_ = fontAttributes,
                            }
                        );

                        // if needs to be downloaded 
                        if (Favorites.IsAdding(dataForListView[i].id)) {
                            Favorites.AddToAddFavoritesItems(dataForListView[i].id, viewType, this, tempListItems.Count - 1, true);
                        }

                        //Debug.WriteLine("Added templistItem, Name = " + dataForListView[i].Name + ", tempListItems.Count = " + tempListItems.Count + " , image = " + dataForListView[i].ImgResource);
                        break;

                    case ListViewItemData.VISUAL_TYPE.DOUBLE:
                        if (dataForListView[i].id == "downloading...") {
                            tempListItems.Add(
                                new DoubleItem
                                {
                                    Index = i,
                                    listIndex = tempListItems.Count,
                                Imagesource = loading_img,
                                
                                 
                                    Index2 = i + 1,
                                Imagesource2 = loading_img,
                                    CellHeight = cellH,
                                CellWidth = cellWidth,
                                    FontSize = cellH * 0.04,
                                    FistRowHeight = fistRowHeight,
                                    LastRowHeight = lastRowHeight,
                                    //NameTranslationY = nameTranslationY,
                                }
                            );
                        }
                        else if (i < dataForListView.Count-1) {
                            bool isNotAGame2 = Game.GetTypeFromTitle(dataForListView[i + 1].Name, dataForListView[i + 1].GameType) == GAME_TYPE.NOT_A_GAME;
                            tempListItems.Add(
                                new DoubleItem
                                {
                                    //Name = dataForListView[i].Name.Replace("&#8211;", "-").Replace("&#8217;", "'"),
                                    Name = System.Net.WebUtility.HtmlDecode(Uri.UnescapeDataString(string.IsNullOrEmpty(dataForListView[i].TransName) ? dataForListView[i].Name : dataForListView[i].TransName)),
                                    Name_slug = System.Net.WebUtility.HtmlDecode(Uri.UnescapeDataString(dataForListView[i].Name)),
                                    Index = i,
                                    listIndex = tempListItems.Count,
                                    Imagesource = dataForListView[i].ImgResource,
                                    ImageUrl = dataForListView[i].ImgResource,
                                    // favorites
                                    FavoritesSize = cellH * 0.2,
                                    IsFavoritesVisible = isNotAGame,
                                    FavoritesImagesource = Favorites.SetVisualSource(dataForListView[i].id, heart_color_img, heart_gray_img, heart_preloader_img),
                                    StarsImagesource = dataForListView[i].Stars > 0 ? stars_imgs[dataForListView[i].Stars] : null,

                                    //Name2 = dataForListView[i + 1].Name.Replace("&#8211;", "-").Replace("&#8217;", "'"),
                                    Name2 = System.Net.WebUtility.HtmlDecode(Uri.UnescapeDataString(string.IsNullOrEmpty(dataForListView[i+1].TransName) ? dataForListView[i+1].Name : dataForListView[i+1].TransName)),
                                    Name2_slug = System.Net.WebUtility.HtmlDecode(Uri.UnescapeDataString(dataForListView[i+1].Name)),
                                    Index2 = i + 1,
                                    Imagesource2 = dataForListView[i + 1].ImgResource,
                                    ImageUrl2 = dataForListView[i + 1].ImgResource,
                                    CellHeight = cellH,
                                CellWidth = cellWidth,
                                FontSize = fontSize,
                                    // favorites
                                    IsFavoritesVisible2 = isNotAGame2,
                                    FavoritesImagesource2 = Favorites.SetVisualSource(dataForListView[i+1].id, heart_color_img, heart_gray_img, heart_preloader_img),
                                    StarsImagesource2 = dataForListView[i + 1].Stars > 0 ? stars_imgs[dataForListView[i + 1].Stars] : null,
                                    
                                MyFont = fontFamily,
                                    FontAttributes_ = fontAttributes,
                                    BackgroundImageSource = background,
                                    MainImagePadding = imgPadding,
                                    FistRowHeight = fistRowHeight,
                                    LastRowHeight = lastRowHeight,
                                }
                            );

                            // if needs to be downloaded 
                            if (Favorites.IsAdding(dataForListView[i].id))
                            {
                                Favorites.AddToAddFavoritesItems(dataForListView[i].id, viewType, this, tempListItems.Count - 1, true);
                            } 
                            // if needs to be downloaded 
                            if (Favorites.IsAdding(dataForListView[i+1].id))
                            {
                                Favorites.AddToAddFavoritesItems(dataForListView[i+1].id, viewType, this, tempListItems.Count - 1, false);
                            }
                            //Debug.WriteLine("Added templistItem, Name = " + dataForListView[i].Name + ", tempListItems.Count = " + tempListItems.Count + " , image = " + dataForListView[i].ImgResource);
                            //Debug.WriteLine("Added templistItem, Name = " + dataForListView[i+1].Name + ", tempListItems.Count = " + tempListItems.Count + " , image = " + dataForListView[i+1].ImgResource);
                        
                        
                        }
                            
                        else {
                            tempListItems.Add(
                                new DoubleItem
                                {
                                    //Name = dataForListView[i].Name.Replace("&#8211;", "-").Replace("&#8217;", "'"),
                                    Name = System.Net.WebUtility.HtmlDecode(Uri.UnescapeDataString(string.IsNullOrEmpty(dataForListView[i].TransName) ? dataForListView[i].Name : dataForListView[i].TransName)),
                                    Name_slug = System.Net.WebUtility.HtmlDecode(Uri.UnescapeDataString(dataForListView[i].Name)),
                                    Index = i,
                                    listIndex = tempListItems.Count,
                                    Imagesource = dataForListView[i].ImgResource,
                                    ImageUrl = dataForListView[i].ImgResource,
                                    CellHeight = cellH,
                                CellWidth = cellWidth,
                                FontSize = fontSize,
                                    // favorites
                                    FavoritesSize = cellH * 0.2,
                                    IsFavoritesVisible = isNotAGame,
                                    FavoritesImagesource = Favorites.SetVisualSource(dataForListView[i].id, heart_color_img, heart_gray_img, heart_preloader_img),
                                    StarsImagesource = dataForListView[i].Stars > 0 ? stars_imgs[dataForListView[i].Stars] : null,
                                    
                                    Index2 = -1,
                                MyFont = fontFamily,
                                    FontAttributes_ = fontAttributes,
                                    BackgroundImageSource = background,
                                    MainImagePadding = imgPadding,
                                    FistRowHeight = fistRowHeight,
                                    LastRowHeight = lastRowHeight,
                                }
                                
                            ); 

                            // if needs to be downloaded 
                            if (Favorites.IsAdding(dataForListView[i].id))
                            {
                                Favorites.AddToAddFavoritesItems(dataForListView[i].id, viewType, this, tempListItems.Count - 1, true);
                            } 
                        }
                              
                        //Debug.WriteLine("Added templistItem, Name = " + dataForListView[i].Name + ", tempListItems.Count = " + tempListItems.Count + " , image = " + dataForListView[i].ImgResource);
                        i++;
                        break;

                    case ListViewItemData.VISUAL_TYPE.SPACE:

                        tempListItems.Add(
                            new SpaceItem
                            {
                                CellHeight = headerH,
                            }
                        );

                        //Debug.WriteLine("Added templistItem, Name = " + dataForListView[i].Name + ", tempListItems.Count = " + tempListItems.Count);
                        break;

                    case ListViewItemData.VISUAL_TYPE.HEADER_WITH_IMAGE:
                        double hFactor_2 = 2.5;
                        //if (dataForListView[i].Name.Length > 15) hFactor = 2.2;
                        tempListItems.Add(
                                new HeaderWithImageItem
                                {
                                    Name = dataForListView[i].Name,
                                    Index = i,
                                    listIndex = tempListItems.Count,
                                    CellHeight = headerH * hFactor_2,                                    
                                    FontSize = headerH * 0.5,                                    
                                }
                            );

                        //Debug.WriteLine("Added templistItem, Name = " + dataForListView[i].Name + ", tempListItems.Count = " + tempListItems.Count);
                        break;
                }
            }

            //Debug.WriteLine("TestViewModel --> UpdateObservableCollection -> fill the list FINISHED");

            Favorites.ListsAreBuisy = false;
            Debug.WriteLine("Favorites_Page -> UpdateObservableCollectionFull -> ListsAreBuisy = false");
        }


        void ShowList(bool DownLoadAsyncAfter = false, bool scrollTo = true) {
           
            Debug.WriteLine("void ShowList -> ");
            FavoriteListCoords current = new FavoriteListCoords() { CentralView = viewType.CentralView, SubView = viewType.SubView };
            //ListItems = new ObservableCollection<ListViewItem>();
            int pos = curPosition[(int)viewType.CentralView][viewType.SubView];

            visibleItems = new List<ListViewItem>();
            if (!scrollTo)
            {
                ListItems = tempListItems;               
            }            
            else {
                
                listView.IsVisible = false;
                ListItems = tempListItems;                              

                if (pos < ListItems.Count)
                {
                    Debug.WriteLine("ScrollToPosition curPosition[(int)viewType] = " + curPosition[(int)viewType.CentralView][viewType.SubView]);
                    Debug.WriteLine("pos = " + pos + ", Name = " + ListItems[pos].Name);

                    //listView.ScrollTo(curPosition[(int)viewType], ScrollToPosition.End, false);
                    listView.ScrollTo(ListItems[pos], ScrollToPosition.Center, false);
                    //listView.ScrollTo(curPosition[(int)viewType], ScrollToPosition.Center, false);

                    Device.StartTimer(TimeSpan.FromMilliseconds(25), () => {
                        listView.ScrollTo(ListItems[pos], ScrollToPosition.Center, false);
                        if (visibleItems.Count > 1) visibleItems.RemoveRange(1, visibleItems.Count - 1);
                        listView.IsVisible = true;
                        return false;
                    });
                }  
            }

            

            // try to download async ... // only once
            Debug.WriteLine("void ShowList -> OK");
            if (viewType.CentralView != MainPage_ViewModel.CENTRAL_VIEWS.BOOKS && DownLoadAsyncAfter && current.CentralView == viewType.CentralView) {
                Debug.WriteLine("TestViewModel -> ShowList ->  DownloadContentAsync");
                DownloadContentAsync(current);
            }

        }

        List<ListViewItemData> ConvertFromPostListResponse(PostListResponse postlistResp)
        {
            Debug.WriteLine("List<ListViewItemData> ConvertFromPostListResponse(PostListResponse postlistResp)");
            List<ListViewItemData> listViewItemData = new List<ListViewItemData>();
            for (int i = 0; i < postlistResp.result.Length; i++) {
                PostListResponse.PostListItem item = postlistResp.result[i];
                string thumb = string.IsNullOrEmpty(item.thumbnail) ? item.cover_image : item.thumbnail;
                string itemId = string.IsNullOrEmpty(item.book_id) ? item.id : item.book_id;
                bool isAnimalsSAS = viewType.CentralView == MainPage_ViewModel.CENTRAL_VIEWS.LESSONS_AND_GAMES &&
                                                                  Game.GetTypeFromTitle(item.title, item.game_type) == GAME_TYPE.SEE_AND_SAY &&
                                                                  Theme.GetThemeOfGame(item.title) == THEME_NAME.ANIMALS;

                //Debug.WriteLine($"viewType = {viewType}, thumbnail = " + thumb);
                if (!string.IsNullOrEmpty(item.text_above)) {
                    // add header
                    listViewItemData.Add(new ListViewItemData { Type = ListViewItemData.VISUAL_TYPE.HEADER, Name = item.text_above });
                    // add body
                    // single

                    if (item.wide == 1)
                    {
                        listViewItemData.Add(new ListViewItemData
                        {
                            id = itemId,
                            Type = ListViewItemData.VISUAL_TYPE.SINGLE,
                            Name = item.title,
                            GameType = item.game_type,
                            TransName = item.trans_title,
                            ImgResource = thumb,
                            Stars = item.repeated,
                        }
                        );
                    }
                    // double
                    else
                    {
                        
                        listViewItemData.Add(new ListViewItemData
                        {
                            id = itemId,
                            Type = ListViewItemData.VISUAL_TYPE.DOUBLE,
                            Name = item.title,
                            GameType = item.game_type,
                            TransName = item.trans_title,
                            ImgResource = thumb,
                            Stars = item.repeated,

                        }
                    );
                    }
                }
                else {
                    if (item.wide == 1)
                    {
                        listViewItemData.Add(new ListViewItemData
                            {
                                id = itemId,
                                Type = ListViewItemData.VISUAL_TYPE.SINGLE,
                                Name = item.title,
                            GameType = item.game_type,
                            TransName = item.trans_title,
                            ImgResource = thumb,
                            Stars = item.repeated,
                            }
                            );
                    }
                    // double
                    else
                    {
                        
                        listViewItemData.Add(new ListViewItemData
                            {
                                id = itemId,
                                Type = ListViewItemData.VISUAL_TYPE.DOUBLE,
                                Name = item.title,
                            GameType = item.game_type,
                            TransName = item.trans_title,
                            ImgResource = thumb,
                            Stars = item.repeated,
                            }
                        );
                    }
                }
            }

            if (viewType.CentralView == MainPage_ViewModel.CENTRAL_VIEWS.BOOKS)
            {
                listViewItemData.Add(new ListViewItemData
                {
                    id = "downloading...",
                    Type = ListViewItemData.VISUAL_TYPE.DOUBLE,
                    Name = string.Empty,
                    TransName = string.Empty,

                });
            };

            return listViewItemData;
        }



        public void ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null) Debug.WriteLine("Item selected = " + (e.SelectedItem as ListViewItem).Name);
        }

        public void ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item != null)
                Debug.WriteLine("Item tapped = " + e.Item.ToString());
            ((ListView)sender).SelectedItem = null;
        }

        public async void SubCat_Tapped(object sender, ItemTappedEventArgs e)
        {
            if (IsAnimating) return;
            IsAnimating = true;

            View view = sender as View;
            int index = int.Parse(view.ClassId);

            Debug.WriteLine("SubCat_OnTapped, ClassId =  " + index);
            if (viewType.SubView == index)
            {
                IsAnimating = false;
                return;
            }
            
            await AnimateImage(sender as View, 250);

            // set new subcats here!
            subCatsImgs[viewType.SubView].Source = subCats[viewType.SubView];           
            subCatsImgs[index].Source = subCatsP[index];
            MainPage_ViewModel.subCatIndex[(int)MainPage_ViewModel.CENTRAL_VIEWS.BOOKS] = index;

            SwitchCenterView(new FavoriteListCoords {CentralView = viewType.CentralView, SubView = index });
            IsAnimating = false;
        }

        public async void  Name_OnTapped(object sender, ListViewItem listViewItem)       
        {
            if (isBlocked) return;
            isBlocked = true;

            if (IsAnimating) return;
            IsAnimating = true;
            Debug.WriteLine("TestViewModel -> Name_OnTapped");
            
            if (listViewItem == null) return;
            // now you can fully access the listview item here via the variable "item"
            // ...
            await AnimateImage(sender as View, 250);
            Debug.WriteLine("Name_OnTapped,  e.MyItem.Name= " + listViewItem.Name + ", e.MyItem.Index=" + listViewItem.Index);
            int index = listViewItem.Index;

            OnTapped1or2(listViewItem, index, listViewItem.Name_slug);

        }

        public async void Favorites_OnTapped(object sender, ListViewItem listViewItem)
        {
            if (isBlocked) return;
            isBlocked = true;

            if (IsAnimating) return;
            IsAnimating = true;
            Debug.WriteLine("Favorites_OnTapped");
            
            if (listViewItem == null) return;

            await AnimateImage(sender as View, 250);
            Debug.WriteLine("Favorites_OnTapped,  e.MyItem.Name= " + listViewItem.Name + ", e.MyItem.Index=" + listViewItem.Index);

            OnFavoritesTapped(listViewItem, listViewItem.Index);
        }

        public async void Favorites_OnTapped2(object sender, ListViewItem listViewItem)
        {
            if (isBlocked) return;
            isBlocked = true;
            
            /*
            if (IsAnimating)
            {
                isBlocked = false;
                return;
            }
            */

            Debug.WriteLine("Favorites_OnTapped");
            var item = listViewItem as DoubleItem;
            if (item == null)
            {
                isBlocked = true;
                return;
            }
            await AnimateImage(sender as View, 250);
            Debug.WriteLine("TestViewModel -> Favorites_OnTapped,  e.MyItem.Name= " + item.Name2 + ", e.MyItem.Index=" + item.Index2);

            OnFavoritesTapped(listViewItem, item.Index2, false);
        }

        async void OnFavoritesTapped (ListViewItem listViewItem, int index, bool isSingle = true) {
            
            // close side panels
            rootViewModel.OnBackButtonPressed();

            // check if has subscription
            //Debug.WriteLine("TestViewModel -> OnFavoritesTapped -> check user");

            if (UserHelper.IsFree)
            {
                var answ = await App.Current.MainPage.DisplayAlert(Translate.GetString("main_page_free_trial"),
                    Translate.GetString("main_page_please_sign_up"), Translate.GetString("login_sign_up"), Translate.GetString("main_page_continue"));

                if (answ)
                {
                    isBlocked = true;
                    await App.Current.MainPage.Navigation.PushModalAsync(new ParentCheck_Page(ParentCheck_Page.WhatToDoNext.WANT_TO_SIGN_UP));
                    IsAnimating = false;
                    return;
                }
                else
                {
                    isBlocked = false;
                    IsAnimating = false;
                    return;
                }
                
            }

            //Debug.WriteLine("TestViewModel -> OnFavoritesTapped -> Favorites.WaitIfListsAreBuisy");

            Favorites.WaitIfListsAreBuisy("TestView -> OnFavoritesTapped");
            int lIndex = listViewItem.listIndex;
            string id = dataForListView[index].id;
           // Debug.WriteLine("TestViewModel -> OnFavoritesTapped -> Favorites.IsVisualFavorite ?");
            Debug.WriteLine("TestViewModel -> OnFavoritesTapped -> Favorites.IsVisualFavorite -> index = " + index);
            Debug.WriteLine("TestViewModel -> OnFavoritesTapped -> Favorites.IsVisualFavorite -> dataForListView[index].id = " + dataForListView[index].id);
            
            
            bool isFavorite = Favorites.IsVisualFavorite(id); 

            bool answer;
            //Debug.WriteLine("OnFavoritesTapped -> isFavorite = " + isFavorite);
            if (isFavorite) {                
                answer = await App.Current.MainPage.DisplayAlert(Translate.GetString("favorites"), Translate.GetString("favorites_remove_post_from_favorites"), POP_UP.YES, POP_UP.NO);
                Debug.WriteLine("TestViewModel -> OnFavoritesTapped -> pop-up result =" + answer);
            }
            else {
                Debug.WriteLine("OnFavoritesTapped -> !isFavorite");
                // check first if we can add
                if (viewType.CentralView == MainPage_ViewModel.CENTRAL_VIEWS.BOOKS) {
                    Debug.WriteLine("OnFavoritesTapped -> (viewType == MainPage_ViewModel.CENTRAL_VIEWS.BOOKS)");
                    if (Favorites.IsMaxBooks()) {
                        await App.Current.MainPage.DisplayAlert(Translate.GetString("favorites"),
                            Translate.GetString("favorites_cant_add_more_books") + "\n" + Translate.GetString("favorites_total_books") + "\n" + Favorites.Books, POP_UP.OK);
                        Favorites.ListsAreBuisy = false;
                        Debug.WriteLine("TestViewModel -> OnFavoritesTapped = books -> ListsAreBuisy = false");
                        isBlocked = false;
                        IsAnimating = false;
                        return;
                    }
                }
                else {
                    Debug.WriteLine("OnFavoritesTapped -> (viewType != MainPage_ViewModel.CENTRAL_VIEWS.BOOKS)");
                    if (Favorites.IsMaxVideos())
                    {
                        await App.Current.MainPage.DisplayAlert(Translate.GetString("favorites"),
                            Translate.GetString("favorites_cant_add_more_videos") + "\n" + Translate.GetString("favorites_total_videos") + "\n" + Favorites.Videos, POP_UP.OK);
                        Favorites.ListsAreBuisy = false;
                        Debug.WriteLine("TestViewModel -> OnFavoritesTapped = not books -> ListsAreBuisy = false");
                        isBlocked = false;
                        IsAnimating = false;
                        return;
                    }    
                }
                answer = await App.Current.MainPage.DisplayAlert(Translate.GetString("favorites"), Translate.GetString("favorites_add_post"), POP_UP.YES, POP_UP.NO);
            }



            if (answer && isFavorite)
            {
                Debug.WriteLine("answer && isFavorite");
                bool removed;
                if (isSingle) {
                    removed = Favorites.Remove(id, viewType, this, lIndex, isSingle);
                    if (removed) listViewItem.FavoritesImagesource = heart_gray_img;
                    Debug.WriteLine($"item added to remove list, Books = {Favorites.Books}, Videos = {Favorites.Videos}");
                }
                else {
                    removed = Favorites.Remove(id, viewType, this , lIndex, isSingle);
                    if (removed) (listViewItem as DoubleItem).FavoritesImagesource2 = heart_gray_img;
                    Debug.WriteLine($"item added to remove list, Books = {Favorites.Books}, Videos = {Favorites.Videos}");
                }
                await Favorites.InstantRemoveAll();

                // try to remove from favorites
                Favorites.StartLoading();
                // *** save to cache all lists ***
                await Favorites.SaveFavorites_unprotected(UserHelper.Lang_cat, UserHelper.Login.user_id);

                // set favorites button
                rootViewModel.SetFavoritesButton();
            }
            else if (answer && !isFavorite) {
                Debug.WriteLine("answer && !isFavorite");
                if (isSingle)
                {
                    listViewItem.FavoritesImagesource = heart_preloader_img;
                    bool addedToAddList = Favorites.Add(id, viewType, this, lIndex, isSingle);
                    Debug.WriteLine($"item added to add list, Books = {Favorites.Books}, Videos = {Favorites.Videos}");
                }
                else
                {
                    (listViewItem as DoubleItem).FavoritesImagesource2 = heart_preloader_img;
                    bool addedToAddList = Favorites.Add(id, viewType, this, lIndex, isSingle);
                    Debug.WriteLine($"item added to add list, Books = {Favorites.Books}, Videos = {Favorites.Videos}");
                }

                // item added to favorites, download it !
                Favorites.StartLoading();
                // *** save to cache all lists ***
                await Favorites.SaveFavorites_unprotected(UserHelper.Lang_cat, UserHelper.Login.user_id);

                // set favorites button
                rootViewModel.SetFavoritesButton();
            }

            Favorites.ListsAreBuisy = false;
            Debug.WriteLine("TestViewModel -> OnFavoritesTapped finish -> ListsAreBuisy = false");

            isBlocked = false;
            IsAnimating = false;
        }

        public async void Name_OnTapped2(object sender, ListViewItem listViewItem)
        {
            if (isBlocked) return;
            isBlocked = true;

            if (IsAnimating) return;
            IsAnimating = true;

            Debug.WriteLine("Name_OnTapped2");
            var item = listViewItem as DoubleItem;
            if (item == null) return;
            int index = item.Index2;

            if (index < 0) return;

            await AnimateImage(sender as View, 250);
            Debug.WriteLine("Name_OnTapped2,  item.Name2= " + item.Name2 + ", item.Index2=" + item.Index2);

            OnTapped1or2(listViewItem, index, item.Name2_slug, false);
        }

        public void OnAppearing()
        {
            isBlocked = false;
        }

        async void OnTapped1or2(ListViewItem listViewItem_, int index, string title, bool isSingle = true) {
           
            //check if user expired

            // close side panels
            rootViewModel.OnBackButtonPressed();

            CheckUserExpired();
            ListItemStarsInfo starsInfo = new ListItemStarsInfo
            {
                id = dataForListView[index].id,
                favoriteListCoords = new FavoriteListCoords() { CentralView = viewType.CentralView, SubView = viewType.SubView, },
                isSingleItem = isSingle,
                data = dataForListView,
                listViewItem = listViewItem_,
                POST_LIST_ITEM_DATA_key = CacheHelper.POST_LIST_ITEM_DATA + "_" + cur_cats + "_" + UserHelper.Login.user_id + "_" + viewType.CentralView + "_" + viewType.SubView,
                rootVM = rootViewModel,
            };
                       

            switch (viewType.CentralView) {
                case MainPage_ViewModel.CENTRAL_VIEWS.LESSONS_AND_GAMES:
                    Debug.WriteLine("Tapped item :" + title);
                    // is game ?
                    GAME_TYPE gameT = Game.GetTypeFromTitle(title, dataForListView[index].GameType);
                    if (gameT == GAME_TYPE.NEW_GAME) // it's a new game
                    {
                        Debug.WriteLine("TEstViewModel ->  new game!, title= " + title);

                        isBlocked = false;
                        IsAnimating = false;
                        return;
                    }
                    else if (gameT != GAME_TYPE.NOT_A_GAME) { // it is a game
                        // check if has subscription
                        if (UserHelper.IsFree)
                        {
                            var answer = await App.Current.MainPage.DisplayAlert(Translate.GetString("main_page_free_trial"), Translate.GetString("main_page_first_lesson_is_free"),
                                Translate.GetString("login_sign_up"), Translate.GetString("main_page_continue"));

                            if (answer)
                            {                                
                                await App.Current.MainPage.Navigation.PushModalAsync(new ParentCheck_Page(ParentCheck_Page.WhatToDoNext.WANT_TO_SIGN_UP));
                            }
                            else isBlocked = false;

                            IsAnimating = false;
                            return;
                        }

                        THEME_NAME themeName = Theme.GetThemeOfGame(title);
                        Debug.WriteLine ("it's a game! theme: " + themeName + ", gameT: " + gameT);

                        // launch game here  

                        // check cache if we have 1st item for game
                        int haveFirstItem = await DownloadHelper.DownloadHelper.IsFirstKeyWordSaved(themeName, UserHelper.Language);
                        switch (haveFirstItem)
                        {
                            case 1: // have 1st item, do nothing, go ahead
                                break;

                            case 0: // have no data for the key in gameobjects
                                if (AsyncMessages.CheckDisplayAlertTimeout())
                                {
                                    await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                                    Translate.GetString("popup_something_went_wrong") + "\n" +
                                    Translate.GetString("popup_try_again") +
                                    POP_UP.GetCode(null, ERROR_PREFIX + 7), POP_UP.OK);
                                }
                                
                                isBlocked = false;
                                IsAnimating = false;
                                return;
                                
                            case -1: // have no 1st item, should check connection
                                if (!CrossConnectivity.Current.IsConnected || !DownloadHelper.DownloadHelper.CheckInternetConnectionProgressive())
                                {
                                    if (AsyncMessages.CheckConnectionTimeout())
                                    {
                                        await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                                        POP_UP.NO_CONNECTION
                                        //+ POP_UP.GetCode(null, ERROR_PREFIX + 8)
                                        , POP_UP.OK);
                                    };
                                    isBlocked = false;
                                    IsAnimating = false;
                                    return;                                    
                                }                                
                                break;

                        }
                        // if not - check if we have good connection

                        switch (gameT) {
                            case GAME_TYPE.SPRINT:
                            await App.Current.MainPage.Navigation.PushModalAsync(new SprintGame_Page(themeName, starsInfo));
                                break;

                            case GAME_TYPE.FIND_THE_PAIR:
                                //await App.Current.MainPage.Navigation.PushModalAsync(new SprintGame_Page(themeName, starsInfo));

                                await rootViewModel.FindThePairGame(themeName, starsInfo);
                                break;
                            case GAME_TYPE.CAROUSEL: 
                                await rootViewModel.CarouselGame(themeName, starsInfo);
                                break;
                            case GAME_TYPE.SEE_AND_SAY: 
                                await rootViewModel.SASGame(themeName, 0, starsInfo);
                                break;
                            case GAME_TYPE.QUIZ: 
                                await rootViewModel.QuizGame(themeName, starsInfo);
                                break;
                            case GAME_TYPE.SEE_AND_SAY_2: 
                                await rootViewModel.SASGame(themeName, 1, starsInfo);
                                break;
                            default: isBlocked = false;
                                break;
                        }
                        break;
                    }

                    //it is some lesson
                    Debug.WriteLine("some lesson (video) goes here =>, id = " + dataForListView[index].id);
                   
                    // check if has subscription
                    if (UserHelper.IsFree && index > 2)
                    {
                        var answ = await App.Current.MainPage.DisplayAlert(Translate.GetString("main_page_free_trial"), Translate.GetString("main_page_first_lesson_is_free"),
                                Translate.GetString("login_sign_up"), Translate.GetString("main_page_continue"));

                        if (answ)
                        {
                            
                            await App.Current.MainPage.Navigation.PushModalAsync(new ParentCheck_Page(ParentCheck_Page.WhatToDoNext.WANT_TO_SIGN_UP));
                        }
                        else isBlocked = false;

                        IsAnimating = false;
                        return;
                    }

                    isBlocked = true;
                    await rootViewModel.WatchLesson(dataForListView[index].id, starsInfo);
                    break;
                    
                case MainPage_ViewModel.CENTRAL_VIEWS.BOOKS:
                    // check if has subscription
                    if (UserHelper.IsFree && index > 2)
                    {
                        var answ = await App.Current.MainPage.DisplayAlert(Translate.GetString("main_page_free_trial"), Translate.GetString("main_page_first_lesson_is_free"),
                                Translate.GetString("login_sign_up"), Translate.GetString("main_page_continue"));
                        if (answ)
                        {
                            
                            await App.Current.MainPage.Navigation.PushModalAsync(new ParentCheck_Page(ParentCheck_Page.WhatToDoNext.WANT_TO_SIGN_UP));
                        }
                        else isBlocked = false;
                        IsAnimating = false;
                        return;
                    }

                    
                    await rootViewModel.ReadBook(dataForListView[index].id);
                    break;
                    
                case MainPage_ViewModel.CENTRAL_VIEWS.STORIES:
                    
                    Debug.WriteLine("some story (video) goes here =>, id = " + dataForListView[index].id);
                    // check if has subscription
                    if (UserHelper.IsFree && index > 2)
                    {
                        

                        var answ = await App.Current.MainPage.DisplayAlert(Translate.GetString("main_page_free_trial"), Translate.GetString("main_page_first_lesson_is_free"),
                                Translate.GetString("login_sign_up"), Translate.GetString("main_page_continue"));

                        if (answ)
                        {
                            await App.Current.MainPage.Navigation.PushModalAsync(new ParentCheck_Page(ParentCheck_Page.WhatToDoNext.WANT_TO_SIGN_UP));
                        }
                        else isBlocked = false;
                        IsAnimating = false;
                        return;
                    }

                    
                    await rootViewModel.WatchLesson(dataForListView[index].id, starsInfo);
                    break;
                    
                case MainPage_ViewModel.CENTRAL_VIEWS.SONGS: 
                    Debug.WriteLine("some song (video) goes here =>, id = " + dataForListView[index].id);
                    // check if has subscription
                    if (UserHelper.IsFree && index > 2)
                    {
                        var answ = await App.Current.MainPage.DisplayAlert(Translate.GetString("main_page_free_trial"), Translate.GetString("main_page_first_lesson_is_free"),
                                Translate.GetString("login_sign_up"), Translate.GetString("main_page_continue"));
                        if (answ)
                        {
                            
                            await App.Current.MainPage.Navigation.PushModalAsync(new ParentCheck_Page(ParentCheck_Page.WhatToDoNext.WANT_TO_SIGN_UP));
                        }
                        else isBlocked = false;
                        IsAnimating = false;
                        return;
                    }

                    
                    await rootViewModel.WatchLesson(dataForListView[index].id, starsInfo);
                    break;    
            }
            IsAnimating = false;
        }

        async void CheckUserExpired() {
            bool userExpired = await CacheHelper.IsExpiredAsync(CacheHelper.LOGIN);
            Debug.WriteLine("userExpired ? : " + userExpired);

            // if user expired -  try to update his info

        }

        public Task AnimateImage(View view, uint time)
        {
            
            return Task.Run(async () =>
            {
                try
                {
                    await view.ScaleTo(0.8, time / 2);
                    await view.ScaleTo(1.0, time / 2);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("TestViewModel -> AnimateImage -> ex:" + ex.Message);
                }
                
                return;
            });
        }

        public void Dispose()
        {
            PropertyChanged = null;
            rootViewModel = null;
            dataForListView = null;

            listView.BindingContext = null;
            listView.ItemsSource = null;
            listView = null;

            tempListItems = null;
            listItems = null;
            ListItems = null;

            visibleItems = null;
            animLock = null;

            rootStack = subCatsStack = null;

            foreach (Image img in subCatsImgs) img.Source = null;
            subCatsImgs = null;           
        }



    }


    public class MyListItemEventArgs : EventArgs
    {
        public ListViewItem MyItem { get; set; }
        public MyListItemEventArgs(ListViewItem item)
        {
            this.MyItem = item;
        }
    }



    // ================================================
    public class ListViewItem : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Name_slug { get; set; }

        public int listIndex { get; set; }
        public int Index { get; set; } // index in dataForListView
        public ImageSource Imagesource { get; set; }
        public string ImageUrl { get; set; }
        public ImageSource StarsImagesource { get; set; }
        public double CellHeight { get; set; }
        public double CellWidth { get; set; }


        public GridLength FistRowHeight { get; set; }
        public GridLength LastRowHeight { get; set; }
        //public double NameTranslationY { get; set; }
        public double FontSize { get; set; }

        public ImageSource FavoritesImagesource { get; set; }
        public double FavoritesSize { get; set; }
        public bool IsFavoritesVisible { get; set; }
        public string MyFont { get; set; }
        public FontAttributes FontAttributes_ { get; set; }

        public ImageSource BackgroundImageSource { get; set; }
        public Thickness MainImagePadding { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class HeaderWithImageItem : ListViewItem
    {        
    }

    public class HeaderItem: ListViewItem {
        public Color HeaderColor { get; set; }
    }

    public class SpaceItem : ListViewItem
    {

    }

    public class SingleItem: ListViewItem {
        
    }

    public class DoubleItem: ListViewItem {
        public string Name2 { get; set; }
        public string Name2_slug { get; set; }
        public int Index2 { get; set; } // index in dataForListView
        public ImageSource Imagesource2 { get; set; }
        public string ImageUrl2 { get; set; }
        public ImageSource StarsImagesource2 { get; set; }

        public ImageSource FavoritesImagesource2 { get; set; }
        public bool IsFavoritesVisible2 { get; set; }

    }
   
    public class PostListResponse {
        public PostListItem[] result { get; set; }
        public Login_Response.Error error { get; set; }

        public class PostListItem {
            public string id { get; set; }
            public string book_id { get; set; } // for books
            public string lang_id { get; set; } // for books
            public string parent_cat_id { get; set; } // for books
            public string book_order { get; set; } // for books
            public string slug { get; set; } // for books
            public string cover_image { get; set; } // for books
            public int wide { get; set; }
            public string text_above { get; set; }
            public int progress { get; set; }
            public int repeated { get; set; }
            public string thumbnail { get; set; }
            public string title { get; set; }
            public string trans_title { get; set; }
            public string game_type { get; set; }
        }
    }

    public class ListItemStarsInfo {
        public string id { get; set; }
        public FavoriteListCoords favoriteListCoords { get; set; }
        public Favorites_Page favorites_Page { get; set; }
        public bool isSingleItem { get; set; }
        public ListViewItem listViewItem { get; set; }
        public List<ListViewItemData> data { get; set; }
        public string POST_LIST_ITEM_DATA_key { get; set; }
        public MainPage_ViewModel rootVM;
    }

}
