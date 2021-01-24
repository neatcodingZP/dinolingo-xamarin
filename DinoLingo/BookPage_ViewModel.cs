using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using FormsVideoLibrary;
using System.ComponentModel;
using FFImageLoading;
using Plugin.Connectivity;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using CarouselView.FormsPlugin.Abstractions;

namespace DinoLingo
{
    public class BookPage_ViewModel: INotifyPropertyChanged
    {
        static int ERROR_PREFIX = 20;

        public event PropertyChangedEventHandler PropertyChanged;
        public ImageSource CashedImageSource { get; set; }
        public ImageSource MainImageSource { get; set; }

        public bool IsMainImageEnabled { get; set; }
        //public GridLength UIMenuWidth { get; set; }

        public Rectangle CloseBtnRect { get; set; }
        public Rectangle SoundBtnRect { get; set; }
        public Rectangle SwitchBtnRect { get; set; }
        public Rectangle LeftBtnRect { get; set; }
        public Rectangle RightBtnRect { get; set; }
        public Rectangle BottomTextRect { get; set; }

        public bool IsPrevBtnVisible { get; set; }
        public bool IsPrevBtnEnabled { get; set; }
        public bool IsNextBtnVisible { get; set; }
        public bool IsNextBtnEnabled { get; set; }
        public bool IsHomeBtnVisible { get; set; }
        public bool IsHomeBtnEnabled { get; set; }

        public bool IsAudioVisible { get; set; }
        public bool IsAudioPlaying { get; set; }
        public bool IsSwitchLangVisible { get; set; }
        public bool IsEngVisible { get; set; }

        public string Text2 { get; set; }
        public double TextFontSize { get; set; }
        public bool BottomFrameEnabled { get; set; }
        public bool BottomFrameVisible { get; set; }

        // ===
        public string LoadingWaitText { get; set; }
        public bool IsGifVisible { get; set; }
        public ImageSource GifSource { get; set; }

        public double CloseBtnSize { get; set; }

        public int myPosition { get; set; }
        public ObservableCollection<BookPageResponse.BookPage.Data> myItemsSource { get; set; }
        public bool IsSwipeEnabled { get; set; }
        
       


        INavigation navigation;
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

        string book_id;
        string lang_cat;
        bool firstStart = true;
        View loadingView, bottomTextView;
        ScrollView scrollView;
        //int page = 0;
        int maxReadPages = -1;
        bool mainLanguage = true;

        List<BookPageResponse.BookPage> bookPages = new List<BookPageResponse.BookPage>();

        bool isFavorite = false;
        ActivityReport activityReport;

        int soundLoadingTasks;
        Object soundLoadingTasksLock = new Object();

        public bool CanAddSoundLoadingTask
        {
            get
            {
                if (soundLoadingTasksLock != null) lock (soundLoadingTasksLock)
                    {
                        soundLoadingTasks++;
                        if (soundLoadingTasks > 20) soundLoadingTasks = 20;
                        return soundLoadingTasks < 20;
                    }
                else return false;
            }            
        }

        public int RemoveSoundLoadingTask       {
            
            set
            {
                if (soundLoadingTasksLock != null) lock (soundLoadingTasksLock)
                    {
                        soundLoadingTasks -= value;
                    }
            }
        }

        public int SoundLoadingTasks
        {
            get
            {
                if (soundLoadingTasksLock != null) lock (soundLoadingTasksLock)
                    {
                        return soundLoadingTasks;
                    }
                else return 0;
            }
            set
            {
                if (soundLoadingTasksLock != null) lock (soundLoadingTasksLock)
                    {
                        soundLoadingTasks = value;
                    }
            }
        }

        private bool hadStarted = false;
        private int audioPlayingPosition = -1;

        public BookPage_ViewModel(INavigation navigation, View loadingView, View bottomTextView, ScrollView scrollView, string book_id)
        {
            this.navigation = navigation;
            this.loadingView = loadingView;
            this.bottomTextView = bottomTextView;
            this.book_id = book_id;
            this.scrollView = scrollView;

            App.OnSleepEvent += OnSleep;
            App.OnResumeEvent += OnResume;

            activityReport = new ActivityReport(ActivityReport.ACT_TYPE.BOOK, book_id);
            IsGifVisible = true;
            CloseBtnSize = UI_Sizes.CloseBtnSize;

            myItemsSource = new ObservableCollection<BookPageResponse.BookPage.Data>();
            IsSwipeEnabled = true;
           
        }

        public async void MenuButton_Tapped(object sender, System.EventArgs e)
        {
            

            Debug.WriteLine("MenuButton_Tapped");
            try
            {
                if (IsAnimating) return;
                IsAnimating = true;
                View view = sender as View;
                if (view.ClassId == "CloseBtn" || view.ClassId == "HomeBtn")
                {
                    await AnimateClick(view, 250);
                    await navigation.PopModalAsync();
                }
                else if (view.ClassId == "NextBtn")
                {
                    IsSwipeEnabled = false;
                    await AnimateClick(view, 250);
                    myPosition++;
                    GoToNextPage();
                    IsSwipeEnabled = true;
                }
                else if (view.ClassId == "PrevBtn")
                {
                    IsSwipeEnabled = false;
                    await AnimateClick(view, 250);
                    myPosition--;
                    GoToPrevPage();
                    IsSwipeEnabled = true;
                }
                else if (view.ClassId == "SoundBtn")
                {
                    if (!IsAudioVisible && !IsAudioPlaying) return;

                    await AnimateClick(view, 250);
                    Debug.WriteLine("say word");
                    TryToSpellPage();
                }
                else if (view.ClassId == "SwitchLangBtn")
                {
                    if (IsSwitchLangVisible)
                    {
                        await AnimateClick(view, 200);
                        await AnimateTextFrame(250);

                        mainLanguage = !mainLanguage;
                        IsAudioPlaying = false;
                        if (App.Audio.sayWord.IsPlaying) App.Audio.sayWord.Stop();

                        // show text2
                        if (mainLanguage) Text2 = bookPages[myPosition].data.content;
                        else Text2 = bookPages[myPosition].engTrans.content;

                        // check if we have audio
                        if (mainLanguage)
                        {
                            IsAudioVisible = !string.IsNullOrEmpty(bookPages[myPosition].data.audio);
                            IsEngVisible = false;
                        }
                        else
                        {
                            IsAudioVisible = !string.IsNullOrEmpty(bookPages[myPosition].engTrans.audio);
                            IsEngVisible = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Analytics.SendResultsRegular("BookPage_ViewModel -> MenuButton_Tapped", null, null, "", "Ex: " + ex.Message);
                Debug.WriteLine("BookPage_ViewModel -> MenuButton_Tapped -> Ex: " + ex.Message);
                await App.Current.MainPage.Navigation.PopModalAsync();
                
            }

            IsAnimating = false;
        }

        void GoToPrevPage()
        {
            //await ImageService.Instance.InvalidateCacheEntryAsync(ImageUrl(page), FFImageLoading.Cache.CacheType.All, true);
            //page--;
            scrollView.ScrollToAsync(0, 0, false);
            IsAudioPlaying = false;
            if (App.Audio.sayWord.IsPlaying) App.Audio.sayWord.Stop();

            ShowCurrentPage();
        }

        void GoToNextPage()
        {
            //page++;
            scrollView.ScrollToAsync(0, 0, false);

            IsAudioPlaying = false;
            if (App.Audio.sayWord.IsPlaying) App.Audio.sayWord.Stop();

            ShowCurrentPage();

            /*
            if (page == bookPages.Count - 1 && bookPages[page].next_page > 0) // it is last downloaded page
            {// we have page to download  
                Debug.WriteLine("download next page, nextpage = " + (page + 1));
                DownloadPage(page + 1);
            }
            else
            {
                Debug.WriteLine("do not download next page, nextpage = " + (page + 1));
            }
            */
        }

        public void OnPositionSelected(object sender, CarouselView.FormsPlugin.Abstractions.PositionSelectedEventArgs e)
        {
            try
            {
                IsSwipeEnabled = false;
                GoToNextPage();
                /*
                if (e.NewValue > page)
                {
                    Debug.WriteLine("BookPage_ViewModel -> OnPositionSelected -> NEXT PAGE");
                    page = e.NewValue;
                    GoToNextPage();
                }
                else if (e.NewValue < page)
                {
                    Debug.WriteLine("BookPage_ViewModel -> OnPositionSelected -> PREV PAGE");
                    page = e.NewValue;
                    GoToPrevPage();
                }
                else
                {
                    Debug.WriteLine("BookPage_ViewModel -> OnPositionSelected -> SAME PAGE");

                    // check if itemsource is not updated
                    Debug.WriteLine($"BookPage_ViewModel -> OnPositionSelected -> SAME PAGE -> myItemsSource.Count = {myItemsSource.Count}, bookPages= {bookPages.Count}");

                    //check if it is pre-last page
                   
                }
                */
                IsSwipeEnabled = true;
                Debug.WriteLine("BookPage_ViewModel -> OnPositionSelected -> EXIT");
            }
            catch (Exception ex)
            {
                Analytics.SendResultsRegular("BookPage_ViewModel -> OnPositionSelected", null, null, "", "Ex: " + ex.Message);
                Debug.WriteLine("BookPage_ViewModel -> OnPositionSelected -> Ex: " + ex.Message);
                Device.BeginInvokeOnMainThread(async ()=> {
                   await App.Current.MainPage.Navigation.PopModalAsync();
                });
                
            }
            
        }
        

        bool TryToSpellPage()
        {
            Debug.WriteLine("BookPage_ViewModel -> TryToSpellPage, page= " + myPosition);
            try
            {
                if (!App.Audio.sayWord.IsPlaying)
                {
                    Debug.WriteLine("if (!App.Audio.sayWord.IsPlaying)");
                    IsAudioPlaying = true;
                    audioPlayingPosition = myPosition;
                    if (isFavorite)
                    {
                        if (mainLanguage)
                        {
                            if (!string.IsNullOrEmpty(bookPages[audioPlayingPosition].data.audio))
                                App.Audio.SayWord("BOOK" + UserHelper.Lang_cat + "_" + book_id + "_" + audioPlayingPosition + ".mp3");
                        }
                        else
                        {
                            if (bookPages[audioPlayingPosition].engTrans != null && !string.IsNullOrEmpty(bookPages[audioPlayingPosition].engTrans.audio))
                                App.Audio.SayWord("BOOK" + UserHelper.Lang_cat + "_" + book_id + "_" + audioPlayingPosition + "transl.mp3");
                        }
                    }
                    else
                    {
                        if (mainLanguage)
                        {
                            if (!string.IsNullOrEmpty(bookPages[audioPlayingPosition].data.audio))
                                App.Audio.SayWord(audioPlayingPosition + "_main_.mp3");
                        }
                        else
                        {
                            if (bookPages[audioPlayingPosition].engTrans != null && !string.IsNullOrEmpty(bookPages[audioPlayingPosition].engTrans.audio))
                                App.Audio.SayWord(audioPlayingPosition + "_trans_.mp3");
                        }
                    }
                }
                else if (audioPlayingPosition != myPosition)
                {
                    Debug.WriteLine("pause audio ...");
                    audioPlayingPosition = -1;
                    IsAudioPlaying = false;
                    App.Audio.sayWord.Pause();
                }
            }
            catch (Exception ex)
            {
                if (navigation != null)
                {
                    Analytics.SendResultsRegular("BookPage_ViewModel -> TryToSpellPage", null, null, "", "Ex: " + ex.Message);
                    Debug.WriteLine("BookPage_ViewModel -> TryToSpellPage -> Ex: " + ex.Message);
                    Device.BeginInvokeOnMainThread(async () => {
                        await App.Current.MainPage.Navigation.PopModalAsync();
                    });
                }               
            }
            
            return false;
        }

        void Current_MediaFinished(object sender, EventArgs e)
        {
            Debug.WriteLine("Current_MediaFinished(), e" + e.ToString());
            IsAudioPlaying = false;
        }

        public Task AnimateClick(View view, uint time)
        {
           
            return Task.Run(async () =>
            {
                try
                {
                    await view.ScaleTo(0.8, time / 2);
                    await view.ScaleTo(1.0, time / 2);
                }
                catch
                {

                }                
                return;
            });
        }

        public Task AnimateTextFrame(uint time)
        {
            IsAnimating = true;
            return Task.Run(async () =>
            {
                await bottomTextView.RotateXTo(90, time / 2);
                await bottomTextView.RotateXTo(0, time / 2);
                IsAnimating = false;
                return;
            });
        }

        void OnLoadingAppeared()
        {            
            IsGifVisible = true;
            GifSource = LoadingView_Logic.GetRandomDancingDinoImg();
        }

        void OnLoadingStarted_InBckground()
        {
            //LoadingWaitText = "Please wait, downloading audio...";
            
        }

        async void OnLoadingEnded_InBckground_OnStart()
        {
            Debug.WriteLine("OnLoadingEnded_InBckground_OnStart(), result_InBckground =" + DownloadHelper.DownloadHelper.result_InBckground);
            try
            {
                switch (DownloadHelper.DownloadHelper.result_InBckground)
                {
                    case DownloadHelper.DownloadHelper.RESULT.NONE:
                    case DownloadHelper.DownloadHelper.RESULT.ERROR:
                    case DownloadHelper.DownloadHelper.RESULT.DOWNLOAD_STARTED:
                        if (AsyncMessages.CheckDisplayAlertTimeout())
                        {
                            AsyncMessages.DisplayAlert(POP_UP.OOPS, POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(null, ERROR_PREFIX + 1), POP_UP.CANCEL);
                        }
                        

                        break;
                    case DownloadHelper.DownloadHelper.RESULT.ERROR_NO_CONNECTION:

                        if (AsyncMessages.CheckDisplayAlertTimeout())
                        {
                            AsyncMessages.CheckConnectionTimeout();
                            AsyncMessages.DisplayAlert(POP_UP.OOPS, POP_UP.NO_CONNECTION, POP_UP.CANCEL);
                        }                            

                        break;
                    case DownloadHelper.DownloadHelper.RESULT.NOTHING_TO_DOWNLOAD:
                        Debug.WriteLine("nothing to download");
                        // WHEN DOWNLOADED 

                        //myItemsSource.Add(bookPages[0].data);
                        Debug.WriteLine("BookPage_ViewModel -> AnimateLoadingView -> myItemsSource.Count =" + myItemsSource.Count);

                        AnimateLoadingView(1000);
                        break;
                    case DownloadHelper.DownloadHelper.RESULT.DOWNLOADED_OK:
                        Debug.WriteLine("downloaded - ok, total files = " + DownloadHelper.DownloadHelper.totalFilesDownloaded);
                        // WHEN DOWNLOADED

                        //myItemsSource.Add(bookPages[0].data);
                        Debug.WriteLine("BookPage_ViewModel -> AnimateLoadingView -> myItemsSource.Count =" + myItemsSource.Count);

                        AnimateLoadingView(1000);
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("BookPage_ViewModel -> OnLoadingEnded_InBckground_OnStart -> ex: " + ex.Message);
            }
        }

        bool StartLoading()
        {
            Debug.WriteLine("StartLoading ()");
            double Xoffset = UI_Sizes.ScreenWidthX * 0.05;
            double Yoffset = UI_Sizes.ScreenHeightX * 0.05;

            CloseBtnRect = new Rectangle(UI_Sizes.ScreenWidthX - Xoffset*0.8 - UI_Sizes.CloseBtnSize, Yoffset, UI_Sizes.CloseBtnSize, UI_Sizes.CloseBtnSize);
            SwitchBtnRect = new Rectangle(Xoffset, Yoffset, UI_Sizes.CloseBtnSize, UI_Sizes.CloseBtnSize);
            SoundBtnRect = new Rectangle(Xoffset + UI_Sizes.CloseBtnSize + Yoffset, Yoffset, UI_Sizes.CloseBtnSize, UI_Sizes.CloseBtnSize);
            LeftBtnRect = new Rectangle(Xoffset, (UI_Sizes.ScreenHeightX - UI_Sizes.CloseBtnSize) * 0.5, UI_Sizes.CloseBtnSize, UI_Sizes.CloseBtnSize);
            RightBtnRect = new Rectangle(UI_Sizes.ScreenWidthX - Xoffset*0.8 - UI_Sizes.CloseBtnSize, (UI_Sizes.ScreenHeightX - UI_Sizes.CloseBtnSize) * 0.5, UI_Sizes.CloseBtnSize, UI_Sizes.CloseBtnSize);
            double textFrameH = 0.3;
            BottomTextRect = new Rectangle(Xoffset, UI_Sizes.ScreenHeightX * (1 - textFrameH), UI_Sizes.ScreenWidthX - 2 * Xoffset, UI_Sizes.ScreenHeightX * textFrameH - Yoffset * 0.5);

            IsMainImageEnabled = false;

            IsPrevBtnVisible = false;
            IsPrevBtnEnabled = false;

            IsNextBtnVisible = false;
            IsNextBtnEnabled = true;

            bottomTextView.BackgroundColor = Color.FromRgba(0, 0, 1, 0.2);

            Text2 = "";
            TextFontSize = UI_Sizes.SmallTextSize;

            IsAudioVisible = true;
            IsAudioPlaying = false;
            IsSwitchLangVisible = true;
            IsEngVisible = false;

            //DownloadHelper.DownloadHelper.OnLoadingStarted_InBckground = OnLoadingStarted_InBckground;
            //DownloadHelper.DownloadHelper.OnLoadingEnded_InBckground = OnLoadingEnded_InBckground_OnStart;

            activityReport.Start();

            //download data for page [0]

            // CHECK IF FAVORITE !


            CheckIfFavorite();

            return false;
        }

        async void CheckIfFavorite () {
            //check all pages
            //check all audios
            Favorites.WaitIfListsAreBuisy("BookPage_ViewModel");
            bool mayBeFavorite = Favorites.IsFavorite(book_id);
            Favorites.ListsAreBuisy = false;
            Debug.WriteLine("BookPage_ViewModel -> ListsAreBuisy = false");

            if (mayBeFavorite && await CacheHelper.Exists(CacheHelper.BOOK + UserHelper.Lang_cat + "_" + book_id)) {
                Debug.WriteLine("BookPage_ViewModel -> book is favorite..., check resources (all pages)...");
                BookPageResponse.BookPage[] pages = await CacheHelper.GetAsync<BookPageResponse.BookPage[]>(CacheHelper.BOOK + UserHelper.Lang_cat + "_" + book_id);
                Debug.WriteLine("BookPage_ViewModel -> we have cached pages: " + pages.Length);

                for (int i = 0; i < pages.Length; i++)
                {
                    BookPageResponse.BookPage page = pages[i];
                    isFavorite = true;
                    // delete all audio 
                    if (page.engTrans != null && !string.IsNullOrEmpty(page.engTrans.audio))
                    {
                        if (! await PCLHelper.IsFileExistAsync("BOOK" + UserHelper.Lang_cat + "_" + book_id + "_"+ i + "transl.mp3")) {
                            Debug.WriteLine("BOOK" + UserHelper.Lang_cat +"_" + book_id + "_" + i + "transl.mp3" + " - does not Exist!");
                            isFavorite = false;
                            break;
                        };
                    }
                    if (!string.IsNullOrEmpty(page.data.audio))
                    {
                        if (! await PCLHelper.IsFileExistAsync("BOOK" + UserHelper.Lang_cat + "_" + book_id + "_" + i + ".mp3")) {
                            Debug.WriteLine("BOOK" + UserHelper.Lang_cat + "_" + book_id + "_" + i + ".mp3" + " - does not Exist!");
                            isFavorite = false;
                            break;
                        };
                    }
                    bookPages.Add(page);
                    
                }

                if (isFavorite) { // check last page
                    Debug.WriteLine("BookPage_ViewModel -> all pages are ok, check last page ...");
                    if (bookPages[pages.Length - 1].next_page != 0) { // it's not las page - have some error
                        isFavorite = false;
                    }
                }
            }
            Debug.WriteLine("BookPage_ViewModel -> CheckIfFavorite -> isFavorite = " + isFavorite);

            if (!isFavorite) { // reset pages
                //bookPages.Clear();
                //DownloadPage(page);
                DownloadAllThePagesData();
            }
            else {
                List<BookPageResponse.BookPage.Data> pagesDataList = new List<BookPageResponse.BookPage.Data>();
                foreach (BookPageResponse.BookPage page in bookPages)
                {
                    pagesDataList.Add(page.data);
                }
                myItemsSource = new ObservableCollection<BookPageResponse.BookPage.Data>(pagesDataList);
                Debug.WriteLine("BookPage_ViewModel -> CheckIfFavorite -> myItemsSource.Count =" + myItemsSource.Count);
                AnimateLoadingView(1000);
            }
        }

        async void DownloadAllThePagesData()
        {
            Debug.WriteLine("BookPage_ViewModel -> DownloadAllThePagesData");
            bookPages.Clear();

            if (CrossConnectivity.Current.IsConnected && DownloadHelper.DownloadHelper.CheckInternetConnectionProgressive())
            {
                // download the first page
                int lang_id_ = -1;
                int book_id_ = -1;
                Int32.TryParse(LANGUAGES.CAT_INFO[lang_cat].Id, out lang_id_);
                Int32.TryParse(book_id, out book_id_);

                BookPageResponse firstPageResponse = null;
                string postPrefix = $"lang_id={LANGUAGES.CAT_INFO[lang_cat].Id}";
                postPrefix += $"&book_id={book_id}";

                // check cached first page
                BookPageResponse.BookPage firstPageCached = await BookPageCacheHelper.GetBookPageCached(lang_id_, book_id_, 0);

                if (firstPageCached == null)
                {
                    Debug.WriteLine("BookPage_ViewModel -> DownloadAllThePagesData, firstPageCached == null");
                    int numberOfTries = 2;
                    for (int i = 0; i < numberOfTries; i++)
                    {
                        firstPageResponse = await DownloadPageAsyncNew(0, postPrefix);
                        if (animLock == null) return;

                        if (firstPageResponse != null && firstPageResponse.error == null && firstPageResponse.result != null)
                        {
                            try
                            {
                                string result = JsonConvert.SerializeObject(firstPageResponse);
                                Debug.WriteLine("BookPage_ViewModel -> DownloadAllThePagesData, firstPageResponse= " + result);
                                // add to cache
                                await BookPageCacheHelper.Add(lang_id_, book_id_, 0, firstPageResponse.result);
                                break;
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("BookPage_ViewModel -> DownloadAllThePagesData, firstPageResponse, exception= " + ex);
                                firstPageResponse = null;
                            }

                        }
                        // else try to download again
                        if (animLock == null) return;
                    }
                }
                else
                {
                    Debug.WriteLine("BookPage_ViewModel -> DownloadAllThePagesData, firstPageCached != null");
                    firstPageResponse = new BookPageResponse()
                    {
                        result = firstPageCached,
                        error = null
                    };
                }
                
                // check the result here
                if (firstPageResponse == null || firstPageResponse.error != null)
                {
                    Debug.WriteLine("BookPage_ViewModel -> DownloadAllThePagesData, firstPageResponse - ERROR, can't procede");
                    Device.BeginInvokeOnMainThread(async () => {
                        if (navigation != null) await navigation.PopModalAsync();
                    });
                    return;
                }


                // then download all the pages
               
                int totalPages = 1;
                Int32.TryParse(firstPageResponse.result.page_count, out totalPages);
                bookPages.Add(firstPageResponse.result);

                // populate bookPages with empty pages

                ObservableCollection<BookPageResponse.BookPage.Data>  myItemsSourceTmp = new ObservableCollection<BookPageResponse.BookPage.Data>();
                myItemsSourceTmp.Add(new BookPageResponse.BookPage.Data());
                for (int i = 1; i < totalPages; i++)
                {
                    bookPages.Add(new BookPageResponse.BookPage());
                    myItemsSourceTmp.Add(new BookPageResponse.BookPage.Data());
                }

                // set data here
                myItemsSourceTmp[0] = bookPages[0].data;



                int pagesCount = 0;
                Int32.TryParse(firstPageResponse.result.page_count, out pagesCount);
                if (pagesCount <= 1)
                {
                    await OnPagesDataDownloaded();
                    return;
                }


                Debug.WriteLine("BookPage_ViewModel -> DownloadAllThePagesData, downloadRestThePages");
                int pagesToDownload = pagesCount - 1;
                List<int> indexesToDownload = new List<int>();
                int requestGroupSize = 9;
                Task<BookPageResponse>[] tasks = new Task<BookPageResponse>[requestGroupSize];

                

                List<BookPageResponse.BookPage> cachedPages =
                    await BookPageCacheHelper.GetBookPagesCached(lang_id_, book_id_);
                int cp = cachedPages == null ? 0 : cachedPages.Count;
                Debug.WriteLine("BookPage_ViewModel -> DownloadAllThePagesData, cachedPages.Count= " + cp);

                for (int i = 1; i <= pagesToDownload; i++)
                {
                    indexesToDownload.Add(i);                    
                }

                // process the cached pages here 
                foreach (var cachedPage in cachedPages)
                {
                    int page = -1;
                    Int32.TryParse(cachedPage.data.page_num, out page);

                    if (indexesToDownload.Contains(page))
                    {
                        indexesToDownload.Remove(page);
                        bookPages[page] = cachedPage;
                        myItemsSourceTmp[page] = cachedPage.data;
                    }
                }

                // array of requests
                if (indexesToDownload.Count > 0)
                {
                    int allPagesTries = 3;
                    int[] requestIndexes = new int[requestGroupSize];

                    for (int t = 0; t < allPagesTries; t++)
                    {
                        Debug.WriteLine("BookPage_ViewModel -> DownloadAllThePagesData, try= " + t);
                        if (animLock == null) return;
                        if (indexesToDownload.Count == 0) break;

                        List<int> indexesToDownloadTemp = new List<int>(indexesToDownload);
                        int itemsToDownload = indexesToDownloadTemp.Count;
                        int waves = (itemsToDownload - 1) / requestGroupSize + 1;
                        for (int w = 0; w < waves; w++)
                        {
                            // create request group here
                            if (animLock == null) return;

                            for (int r = 0; r < requestGroupSize; r++)
                            {
                                int currentIndex = w * requestGroupSize + r;
                                if (currentIndex < indexesToDownloadTemp.Count)
                                {
                                    requestIndexes[r] = indexesToDownloadTemp[currentIndex];
                                }
                                else
                                {
                                    requestIndexes[r] = -1;
                                } 
                            }
                            // here we have a group (wave)
                            for (int ta = 0; ta < tasks.Length; ta++)  
                            {
                                tasks[ta] = requestIndexes[ta] > 0 ? DownloadPageAsyncNew(requestIndexes[ta], postPrefix) : NullableTask();
                            }

                            if (CrossConnectivity.Current.IsConnected)
                            {
                                await Task.WhenAll(tasks[0], tasks[1], tasks[2], tasks[3], tasks[4],
                                tasks[5], tasks[6], tasks[7], tasks[8]
                                );

                                // process the responses
                                for (int ta = 0; ta < tasks.Length; ta++)
                                {

                                    if (animLock == null) return;


                                    BookPageResponse bookPageResponse = await tasks[ta];
                                    // process the result here
                                    if (bookPageResponse != null && bookPageResponse.error == null && bookPageResponse.result != null)
                                    {
                                        try
                                        {
                                            string result = JsonConvert.SerializeObject(bookPageResponse);
                                            int page_ = requestIndexes[ta];
                                            Debug.WriteLine("BookPage_ViewModel -> DownloadAllThePagesData, page= " + page_ + " , bookPageResponse= " + result);
                                            indexesToDownload.Remove(page_);


                                            bookPages[page_] = bookPageResponse.result;

                                            myItemsSourceTmp[requestIndexes[ta]] = bookPageResponse.result.data;

                                            // add to cache
                                            await BookPageCacheHelper.Add(lang_id_, book_id_, page_, bookPageResponse.result);
                                        }
                                        catch (Exception ex)
                                        {
                                            Debug.WriteLine("BookPage_ViewModel -> DownloadAllThePagesData, page= " + requestIndexes[ta] + ", exception= " + ex);

                                        }

                                    }

                                    // else try to download again
                                    if (animLock == null) return;

                                }
                            }

                        }

                    }
                }
                

                // here we can chek the result
                Debug.WriteLine("BookPage_ViewModel -> DownloadAllThePagesData, indexesToDownload.Count= " + indexesToDownload.Count);
                
                if (animLock != null) myItemsSource = new ObservableCollection<BookPageResponse.BookPage.Data>(myItemsSourceTmp);

                if (indexesToDownload.Count > 0)
                {
                    await App.Current.MainPage.DisplayAlert(POP_UP.OOPS, POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(null, ERROR_PREFIX + 5), POP_UP.OK);
                    await navigation?.PopModalAsync();
                    return;
                }

                await OnPagesDataDownloaded();
            }
            else
            {               
                AsyncMessages.CheckConnectionTimeout();
                await App.Current.MainPage.DisplayAlert(POP_UP.OOPS, POP_UP.NO_CONNECTION, POP_UP.OK);
                await navigation.PopModalAsync();
            }
        }

        private async Task OnPagesDataDownloaded()
        {
            if (animLock == null) return;
            Debug.WriteLine("BookPage_ViewModel -> OnPagesDataDownloaded");

            // remove all the sounds if have
            for (int i = 0; i < bookPages.Count; i++)               
            {
                if (IsSoundPresent(i))
                {
                    if (await PCLHelper.IsFileExistAsync(i + "_main_.mp3"))
                    {
                        await PCLHelper.DeleteFile(i + "_main_.mp3");
                    }
                }

                if ((IsSoundPresentTranslated(i)))
                {
                    if (await PCLHelper.IsFileExistAsync(i + "_trans_.mp3"))
                    {
                        await PCLHelper.DeleteFile(i + "_trans_.mp3");
                    }
                }
            }


            if (animLock == null) return;

            // download sounds for first page only

            if (CrossConnectivity.Current.IsConnected)
            {
                for (int p = 0; p < 1; p++)
                {
                    var taskSound1 = IsSoundPresent(p) ? DownloadHelper.DownloadHelper.SimpleAudioLoader(bookPages[p].data.audio, 0 + "_main_.mp3") : DownloadHelper.DownloadHelper.SimpleAudioLoaderNull();
                    var taskSound2 = IsSoundPresentTranslated(p) ? DownloadHelper.DownloadHelper.SimpleAudioLoader(bookPages[p].engTrans.audio, 0 + "_trans_.mp3") : DownloadHelper.DownloadHelper.SimpleAudioLoaderNull();

                    await Task.WhenAll(taskSound1, taskSound2);
                    // process the result here
                    Debug.WriteLine("BookPage_ViewModel -> OnPagesDataDownloaded, SimpleAudioLoader, taskSound1= " + (await taskSound1) + ", taskSound2= " + (await taskSound2));
                }
            }
                




            // start book
            AnimateLoadingView(800);

        }

        private Boolean IsSoundPresent(int page_)
        {
            return bookPages[page_] != null && bookPages[page_].data != null
                && bookPages[page_].data.audio != null && bookPages[page_].data.audio != string.Empty;
        }

        private Boolean IsSoundPresentTranslated(int page_)
        {
            return bookPages[page_] != null && bookPages[page_].engTrans != null && bookPages[page_].engTrans.audio != null
                && bookPages[page_].engTrans.audio != string.Empty;
        }

        private async Task<BookPageResponse> DownloadPageAsyncNew(int page_, string postPrefix)
        {
            //string postData = $"lang_id={LANGUAGES.CAT_INFO[lang_cat].Id}";
            //postData += $"&book_id={book_id}";
            Debug.WriteLine("BookPage_ViewModel -> DownloadPageAsyncNew, page =" + page_);
            BookPageResponse bookPageResponse = await ServerApi.PostRequestProgressive<BookPageResponse>(ServerApi.BOOK_URL, postPrefix + $"&page={page_}", null);
            return bookPageResponse;
        }

        private async Task<BookPageResponse> NullableTask()
        {            
            return null;
        }

        async void DownloadPage (int page) {
            if (animLock == null) return;
            if (CrossConnectivity.Current.IsConnected && DownloadHelper.DownloadHelper.CheckInternetConnectionProgressive())
            {
                DownloadPageAsync(page);
                Debug.WriteLine("CrossConnectivity.Current.IsConnected = true");
            }
            else {
                Debug.WriteLine("CrossConnectivity.Current.IsConnected = false");
                AsyncMessages.CheckConnectionTimeout();
                await App.Current.MainPage.DisplayAlert(POP_UP.OOPS, POP_UP.NO_CONNECTION, POP_UP.OK);
                await navigation.PopModalAsync();
            }
       }
         

        Task DownloadPageAsync(int pageIndex)
        {
            Debug.WriteLine("download pageIndex : " + pageIndex);
            return Task.Run(async () =>
            {
                // IF IT IS FAVORITE - NO NEED TO DOWNLOAD !
                if (isFavorite) return;
                
                    
                try
                {
                    //do we need BookPageResponse ?
                    if (bookPages.Count <= pageIndex)
                    { // we do not have page info yet
                        string postData = $"lang_id={LANGUAGES.CAT_INFO[lang_cat].Id}";
                        postData += $"&book_id={book_id}";
                        postData += $"&page={pageIndex}";
                        BookPageResponse bookPageResponse = await ServerApi.PostRequestProgressive<BookPageResponse>(ServerApi.BOOK_URL, postData, null);
                        if (bookPageResponse == null)
                        {
                            Analytics.SendResultsRegular("BookPage_ViewModel", bookPageResponse, bookPageResponse?.error, ServerApi.BOOK_URL, postData);
                            Debug.WriteLine("BookPage_ViewModel -> " + POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(bookPageResponse?.error, ERROR_PREFIX + 2));
                            
                            ProcessSomeErrorWhileDownloading(pageIndex);
                            return;
                        }
                        else if (bookPageResponse.error != null)
                        {
                            Analytics.SendResultsRegular("BookPage_ViewModel", bookPageResponse, bookPageResponse?.error, ServerApi.BOOK_URL, postData);
                            Debug.WriteLine("BookPage_ViewModel -> " + POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(bookPageResponse?.error, ERROR_PREFIX + 3));
                            ProcessSomeErrorWhileDownloading(pageIndex);
                            return;
                        }
                        else if (bookPageResponse.result == null)
                        {
                            Analytics.SendResultsRegular("BookPage_ViewModel", bookPageResponse, bookPageResponse?.error, ServerApi.BOOK_URL, postData);
                            Debug.WriteLine("BookPage_ViewModel -> " + POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(bookPageResponse?.error, ERROR_PREFIX + 4));
                            ProcessSomeErrorWhileDownloading(pageIndex);
                            return;
                        }

                        try
                        {
                            string result = JsonConvert.SerializeObject(bookPageResponse);
                            Debug.WriteLine("bookPageResponse = " + result);
                            int totalPages = 1;
                            Int32.TryParse(bookPageResponse.result.page_count, out totalPages);
                            bookPages.Add(bookPageResponse.result);

                            if (pageIndex == 0)
                            {
                                // populate bookPages with empty pages
                                Debug.WriteLine("BookPage_ViewModel -> DownloadPageAsync, populate bookPages with empty pages, totalPages=" + totalPages);
                                myItemsSource = new ObservableCollection<BookPageResponse.BookPage.Data>();
                                myItemsSource.Add(new BookPageResponse.BookPage.Data());
                                for (int i = 1; i < totalPages; i++)
                                {
                                    bookPages.Add(new BookPageResponse.BookPage());
                                    myItemsSource.Add(new BookPageResponse.BookPage.Data()); 
                                }                               

                            }
                            else
                            {
                                Debug.WriteLine("BookPage_ViewModel -> DownloadPageAsync, pageData downloaded, pageIndex= " + pageIndex);
                                bookPages[pageIndex] = bookPageResponse.result;
                            }

                            // set data here
                            myItemsSource[pageIndex] = bookPages[pageIndex].data;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("BookPage_ViewModel -> DownloadPageAsync ex: " + ex.Message);
                        }                     
                        

                    }

                    // download 2 audios here
                    string transUrlAudio = string.Empty;
                    if (bookPages[pageIndex].engTrans != null) transUrlAudio = bookPages[pageIndex].engTrans.audio;

                    if (animLock == null) return;

                    DownloadHelper.DownloadHelper.Download2AudioAsync_InBckground(bookPages[pageIndex].data.audio, pageIndex + "_main_.mp3",
                                                                                  transUrlAudio, pageIndex + "_trans_.mp3");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("BookPage_ViewModel -> DownloadPageAsync -> ex:" + ex.Message);
                }                    
                
                return;
            });
        }


        void ProcessSomeErrorWhileDownloading(int pageIndex) {
            Debug.WriteLine("ProcessSomeErrorWhileDownloading");
            if (pageIndex == 0)
            {
                Device.BeginInvokeOnMainThread(async () => {
                    if (navigation != null) await navigation.PopModalAsync();
               });

            }
            else
            {
                // try again in 5 sec ...
                // ***
                // ***  
                if (navigation != null) Device.StartTimer(TimeSpan.FromSeconds(5), RetryDownload);
            }
        }

        bool RetryDownload () {
            if (myPosition == bookPages.Count - 1) { // if we on last page
                DownloadPage(myPosition + 1);
            }
            return false;
        }
                                    


        void OnLoadingNextPageStarted_InBckground()
        {
            Debug.WriteLine("OnLoadingNextPageStarted_InBckground()");
        }

        async void OnLoadingNextPageEnded_InBckground()
        {
            Debug.WriteLine("OnLoadingNextPageEnded_InBckground(), result =" + DownloadHelper.DownloadHelper.result_InBckground);
            switch (DownloadHelper.DownloadHelper.result_InBckground)
            {
                case DownloadHelper.DownloadHelper.RESULT.NONE:
                case DownloadHelper.DownloadHelper.RESULT.ERROR:
                case DownloadHelper.DownloadHelper.RESULT.DOWNLOAD_STARTED:
                    if (AsyncMessages.CheckDisplayAlertTimeout())
                    {
                        await App.Current.MainPage.DisplayAlert(POP_UP.OOPS, POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(null, ERROR_PREFIX + 5), POP_UP.OK);
                    } 
                    

                    break;
                case DownloadHelper.DownloadHelper.RESULT.ERROR_NO_CONNECTION:
                    if (AsyncMessages.CheckDisplayAlertTimeout())
                    {
                        AsyncMessages.CheckConnectionTimeout();
                        await App.Current.MainPage.DisplayAlert(POP_UP.OOPS, POP_UP.NO_CONNECTION, POP_UP.CANCEL);
                    }                    

                    break;
                case DownloadHelper.DownloadHelper.RESULT.NOTHING_TO_DOWNLOAD:
                    Debug.WriteLine("nothing to download");

                    break;
                case DownloadHelper.DownloadHelper.RESULT.DOWNLOADED_OK:
                    Debug.WriteLine("downloaded - ok, total files = " + DownloadHelper.DownloadHelper.totalFilesDownloaded);
                    // WHEN DOWNLOADED 
                    OnNextPageDownloaded();
                    break;
            }           
        }

        void OnNextPageDownloaded () {
            Debug.WriteLine("OnNextPageDownloaded()");
            try
            {
                //are we on pre-last page?
                if (myPosition == bookPages.Count - 2)
                {
                    // update cached image
                    CashedImageSource = bookPages[myPosition + 1].data.image;
                    // update next button
                    IsNextBtnEnabled = IsNextBtnVisible = true;

                   
                    //myItemsSource.Add(bookPages[page + 1].data);
                    Debug.WriteLine($"BookPage_ViewModel -> myItemsSource.Add({myPosition + 1}) - OK");
                    Debug.WriteLine("BookPage_ViewModel -> OnNextPageDownloaded -> myItemsSource.Count =" + myItemsSource.Count);
                }
            }
            catch (Exception ex)
            {
                if (navigation != null)
                {
                    Analytics.SendResultsRegular("BookPage_ViewModel -> OnNextPageDownloaded", null, null, "", "Ex: " + ex.Message);
                    Debug.WriteLine("BookPage_ViewModel -> OnNextPageDownloaded -> Ex: " + ex.Message);
                    Device.BeginInvokeOnMainThread(async () => {

                        await App.Current.MainPage.Navigation.PopModalAsync();
                    });
                }
                
            }
            
        }


        async void AnimateLoadingView(uint time)
        {
            try
            {
                IsAnimating = true;

                Text2 = bookPages[0].data.content;
                MainImageSource = bookPages[0].data.image;


                await Task.Delay(250);
                await loadingView.TranslateTo(0, -App.Current.MainPage.Height, time);
                loadingView.IsVisible = false;
                loadingView.InputTransparent = true;
                IsAnimating = false;

                /*
                DownloadHelper.DownloadHelper.OnLoadingStarted_InBckground -= OnLoadingStarted_InBckground;
                DownloadHelper.DownloadHelper.OnLoadingEnded_InBckground -= OnLoadingEnded_InBckground_OnStart;

                DownloadHelper.DownloadHelper.OnLoadingStarted_InBckground = OnLoadingNextPageStarted_InBckground;
                DownloadHelper.DownloadHelper.OnLoadingEnded_InBckground = OnLoadingNextPageEnded_InBckground;
                */

                // CONTINUE DOWNLOADING !!!
                // show current page
                hadStarted = true;
                ShowCurrentPage();

                // download next page (if we have one)
                /*
                if (!isFavorite && bookPages[0].next_page > 0)
                { // we have page to download
                  //download data for page [0]
                    Debug.WriteLine("next page for page 0 : " + 1);
                    DownloadPage(1);
                }
                */
            }
            catch (Exception ex)
            {
                Debug.WriteLine("BookPage_ViewModel -> AnimateLoadingView -> ex:" + ex.Message);
            }            
        }

        void ShowCurrentPage() {


            // here we have at least [0] page
            // report activity here

            //check page here
            if (bookPages.Count == 0 || !hadStarted) return;


            if (!isFavorite && CrossConnectivity.Current.IsConnected) CheckPageDataAndSounds(myPosition);

            if (myPosition > maxReadPages) {
                maxReadPages = myPosition;
                double totalPages = double.Parse(bookPages[0].page_count);
                activityReport.ReportProgress((maxReadPages + 1) / totalPages * 100);
            }


            Debug.WriteLine("ShowCurrentPage, page = " + myPosition);
            // check if we have audio & text
            BottomFrameEnabled = BottomFrameVisible = ((!string.IsNullOrEmpty(bookPages[myPosition].data.audio) && !string.IsNullOrWhiteSpace(bookPages[myPosition].data.audio)) || 
                                                       (!string.IsNullOrEmpty(bookPages[myPosition].data.content) && !string.IsNullOrWhiteSpace(bookPages[myPosition].data.content)) );

            // check if we have audio
            if (mainLanguage) {
                IsAudioVisible = !string.IsNullOrEmpty(bookPages[myPosition].data.audio);
                Debug.WriteLine("do not have native lang audio");
            }
            else {
                IsAudioVisible = !string.IsNullOrEmpty(bookPages[myPosition].engTrans.audio);
            }


            // check if we have translation
            IsSwitchLangVisible = bookPages[myPosition].engTrans != null;
            if (!IsSwitchLangVisible) IsEngVisible = false;

            // check previous page 
            IsPrevBtnEnabled = IsPrevBtnVisible = myPosition > 0;

            // check next page
            IsNextBtnEnabled = IsNextBtnVisible = myPosition < bookPages.Count - 1;

            //check home button
            IsHomeBtnEnabled = IsHomeBtnVisible = (bookPages[myPosition].next_page == 0);

            // show text2
            if (mainLanguage) Text2 = bookPages[myPosition].data.content;
            else Text2 = bookPages[myPosition].engTrans.content;

            // main image
            Debug.WriteLine("Image : " + bookPages[myPosition].data.image);
            MainImageSource = bookPages[myPosition].data.image;
            

            // try to spell the page
            if (IsAudioVisible) Device.StartTimer(TimeSpan.FromMilliseconds(300), TryToSpellPage);
        }

        private async Task CheckPageDataAndSounds(int page_)
        {
            Debug.WriteLine("BookPageViewModel --> CheckPageDataAndSounds, page= " + page_);
            // check current page
            // and x pages after current
            if (animLock == null) return;

            // download sounds for current page only
            if (animLock == null) return;

            if (CrossConnectivity.Current.IsConnected)
            {
                await CheckAndLoadSounds(page_);
            }
            

            int pagesToPreload = 3;
            int maxForwardDistance = 10;

            int preloadingPage = page_ + 1;
            while (preloadingPage < page_ + maxForwardDistance && pagesToPreload > 0 && preloadingPage < bookPages.Count)
            {
                Debug.WriteLine("BookPageViewModel --> CheckPageDataAndSounds, SoundLoadingTasks= " + SoundLoadingTasks);
                if (animLock == null) return;

                if (CrossConnectivity.Current.IsConnected)
                {
                    if (CanAddSoundLoadingTask && await CheckAndLoadSounds(preloadingPage))
                    {
                        pagesToPreload--;
                    }
                    RemoveSoundLoadingTask = 1;
                }
                


                // dosome
                preloadingPage++;
            }
        }

        private async Task<Boolean> CheckAndLoadSounds(int page_)
        {
            Debug.WriteLine("BookPageViewModel --> CheckAndLoadSounds, page= " + page_);
            bool needToLoadSoundFile = IsSoundPresent(page_) && !(await PCLHelper.IsFileExistAsync(page_ + "_main_.mp3"));
            bool needToLoadSoundFileTranslated = IsSoundPresentTranslated(page_) && !(await PCLHelper.IsFileExistAsync(page_ + "_trans_.mp3"));

            if (!needToLoadSoundFile && !needToLoadSoundFileTranslated) return false;

            var taskSound1 = needToLoadSoundFile ? DownloadHelper.DownloadHelper.SimpleAudioLoader(bookPages[page_].data.audio, page_ + "_main_.mp3") : DownloadHelper.DownloadHelper.SimpleAudioLoaderNull();
            var taskSound2 = needToLoadSoundFileTranslated ? DownloadHelper.DownloadHelper.SimpleAudioLoader(bookPages[page_].engTrans.audio, page_ + "_trans_.mp3") : DownloadHelper.DownloadHelper.SimpleAudioLoaderNull();

            await Task.WhenAll(taskSound1, taskSound2);

            // process the result here
            Debug.WriteLine("BookPage_ViewModel -> CheckAndLoadSounds, SimpleAudioLoader, taskSound1= " + (await taskSound1) + ", taskSound2= " + (await taskSound2));

            if (IsAudioVisible && page_ == myPosition && needToLoadSoundFile) TryToSpellPage();
            return true;
        }

        public void OnAppearing()
        {
            App.Audio.sayWord.PlaybackEnded += Current_MediaFinished;

            if (firstStart)
            {
                firstStart = false;
                OnLoadingAppeared();
                lang_cat = UserHelper.Lang_cat;
                Device.StartTimer(TimeSpan.FromMilliseconds(500), StartLoading);
            }
        }

        public async void OnDisappearing()
        {
            App.OnSleepEvent -= OnSleep;
            App.OnResumeEvent -= OnResume;
            App.Audio.sayWord.PlaybackEnded -= Current_MediaFinished;
            if (App.Audio.sayWord.IsPlaying) App.Audio.sayWord.Stop();

            DownloadHelper.DownloadHelper.OnLoadingStarted_InBckground -= OnLoadingNextPageStarted_InBckground;
            DownloadHelper.DownloadHelper.OnLoadingEnded_InBckground -= OnLoadingNextPageEnded_InBckground;

            // clear all ffimageloading cache
            if (!isFavorite)
            foreach (BookPageResponse.BookPage bp in bookPages) {
                if (bp != null && bp.data != null && bp.data.image != null)
                    {
                        await ImageService.Instance.InvalidateCacheEntryAsync(bp.data.image, FFImageLoading.Cache.CacheType.All, true);
                    }
                
            }
            activityReport.OnDisappearing();
        }


        void OnSleep()
        {
            Debug.WriteLine("BookPageViewModel --> OnSleep()");
            activityReport.OnSleep();
        }

        void OnResume()
        {
            Debug.WriteLine("BookPageViewModel --> OnResume()");
            activityReport.OnResume();
        }

        public void Dispose()
        {
            PropertyChanged = null;
            CashedImageSource = null;
            MainImageSource = null;
            GifSource = null;
            myItemsSource = null;
            navigation = null;
            animLock = null;

            loadingView = bottomTextView = null;
            scrollView = null;
            bookPages = null;
            
            activityReport = null;
        }
    }
}
