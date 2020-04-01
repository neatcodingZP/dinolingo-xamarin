using System;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DownloadHelper;
using FFImageLoading;
using Plugin.Connectivity;
using FFImageLoading.Forms;
using DinoLingo.Games;

namespace DinoLingo
{
    public class CarouselGame_ViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        static int ERROR_PREFIX = 30;

        public double MenuFontSize { get; set; }
        public double TotalWidth { get; set; }
        public double TotalMenuesWidth { get; set; }
        public double GameWidth { get; set; }
        public double GameHeight { get; set; }

        public Thickness GamePadding { get; set; }
        public double CloseBtnSize { get; set; }
        public double ShotsWidth { get; set; }

        public string TargetsText { get; set; }
        public string AimedText { get; set; }
        public string MissesText { get; set; }



        public string LoadingWaitText { get; set; }       
        public bool IsStartBtnEnabled { get; set; }
        public bool IsStartBtnVisible { get; set; }
        public ImageSource StartBtnSource { get; set; }


        INavigation navigation;
        THEME_NAME theme;

        bool isAnimating;
        Object animLock = new Object();
        public bool IsAnimating
        {
            get
            {
                lock (animLock)
                {
                    return isAnimating;
                }
            }
            set
            {
                lock (animLock)
                {
                    isAnimating = value;
                }

            }
        }        
        
        List<string> randomKeys;  
       
        Random random = new Random();
        Image[] shotsImages;
        VictoryView victoryView;
        GameOverView gameoverView;
        View loadingView;

        string imgResPrefix = "DinoLingo.Resources.CAROUSEL.";             

        bool firstStart = true;
        ListItemStarsInfo starsInfo;
        PostResponse.Post post;
        ActivityReport activityReport;
        public GameScene_CarouselGame gameScene;

        public CarouselGame_ViewModel(INavigation navigation, THEME_NAME theme, View loadingView, ListItemStarsInfo starsInfo)
        {
            this.navigation = navigation;
            this.theme = theme;
            this.loadingView = loadingView;
            this.starsInfo = starsInfo;
            
            App.OnSleepEvent += OnSleep;
            App.OnResumeEvent += OnResume;

            int index = starsInfo.isSingleItem ? starsInfo.eventArgs.MyItem.Index : (starsInfo.eventArgs.MyItem as DoubleItem).Index2;
            activityReport = new ActivityReport(ActivityReport.ACT_TYPE.GAME, starsInfo.data[index].id);       
        }


        bool StartLoading()
        {
            Debug.WriteLine("StartLoading ()");
            //get list from gameObjects
            List<string> wordsToDownload1 = new List<string>(Theme.Resources[(int)theme].Item.Keys);
            foreach (string key in Theme.Resources[(int)theme].Item.Keys)
            {
                if (GameHelper.memory_GameObjects.HasKey(theme, key) || GameHelper.sas_GameObjects.HasKey(theme, key))
                {
                    continue;
                }
                wordsToDownload1.Remove(key);
                Debug.WriteLine("removed from downloading : " + key);
            }

            DownloadHelper.DownloadHelper.OnLoadingStarted = OnLoadingStarted;
            DownloadHelper.DownloadHelper.OnLoadingProgress = OnLoadingProgress;
            DownloadHelper.DownloadHelper.OnLoadingEnded = OnLoadingEnded;

            DownloadHelper.DownloadHelper.DownLoadWords(wordsToDownload1, theme,
                                                        null, theme);
            return false;
        }

        void OnLoadingAppeared()
        {
            IsStartBtnEnabled = false;            
        }

        void OnLoadingStarted()
        {
            Debug.WriteLine("CarouselGame_ViewModel -> OnLoadingStarted -> set .GIF");
            Random r = new Random();
            StartBtnSource = LoadingView_Logic.GetRandomDancingDinoImg();
            Debug.WriteLine($"CarouselGame_ViewModel -> OnLoadingStarted -> StartBtnSource = {StartBtnSource.ToString()}, is null? = {StartBtnSource == null}");
            IsStartBtnVisible = true;
            //LoadingWaitText = "Please wait... dowloading audio ...";
        }

        void OnLoadingProgress()
        {
            Debug.WriteLine("OnLoadingProgress() --> progress = " + DownloadHelper.DownloadHelper.progress);
            //ProgressValue = DownloadHelper.DownloadHelper.progress;
            //LoadingPercentText = ((int)(ProgressValue * 100)).ToString() + " %";
        }

        async void OnLoadingEnded()
        {
            switch (DownloadHelper.DownloadHelper.result)
            {
                case DownloadHelper.DownloadHelper.RESULT.NONE:
                case DownloadHelper.DownloadHelper.RESULT.ERROR:
                case DownloadHelper.DownloadHelper.RESULT.DOWNLOAD_STARTED:
                    if (AsyncMessages.CheckDisplayAlertTimeout())
                    {
                        await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                                                POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(null, ERROR_PREFIX + 1), POP_UP.OK);
                        //await App.Current.MainPage.DisplayAlert("Error!", "Error, while loading audio from server..., download.result = " + DownloadHelper.DownloadHelper.result, "Cancel");

                    }
                    await navigation.PopModalAsync();
                    break;
                case DownloadHelper.DownloadHelper.RESULT.ERROR_NO_CONNECTION:
                    if (AsyncMessages.CheckConnectionTimeout())
                    {
                        await App.Current.MainPage.DisplayAlert(POP_UP.OOPS, POP_UP.NO_CONNECTION
                            //+ POP_UP.GetCode(null, ERROR_PREFIX + 2)
                            , POP_UP.OK);
                    }
                    
                    await navigation.PopModalAsync();
                    break;
                case DownloadHelper.DownloadHelper.RESULT.NOTHING_TO_DOWNLOAD:
                    Debug.WriteLine("Carousel_ViewModel -> nothing to download");
                    // WHEN DOWNLOADED 
                    if ((post = await victoryView.DownloadPost(starsInfo)) != null)
                    { // post downloaded ok
                        SetAllAfterLoading();
                    }
                    else
                    {
                        //await App.Current.MainPage.DisplayAlert("Error!", "Some error in post data", "Cancel");
                        SetAllAfterLoading();
                        //await navigation.PopModalAsync();
                    }
                    break;

                case DownloadHelper.DownloadHelper.RESULT.DOWNLOADED_OK:
                    Debug.WriteLine("downloaded - ok, total files = " + DownloadHelper.DownloadHelper.totalFilesDownloaded);
                    if ((post = await victoryView.DownloadPost(starsInfo)) != null)
                    { // post downloaded ok
                        SetAllAfterLoading();
                    }
                    else
                    {
                        //await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                        //POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(null, ERROR_PREFIX + 3), POP_UP.OK);
                        SetAllAfterLoading();
                        //await navigation.PopModalAsync();
                    }
                    break;
            }
        }
        
        public Task AddAllItems () 
        {
            return Task.Run(async () =>
            {
                List<string> tmp = new List<string>(Theme.Resources[(int)theme].Item.Keys);
                List<string> source = new List<string>(Theme.Resources[(int)theme].Item.Keys);
                List<string> baseCards = new List<string>(Theme.Resources[(int)theme].Item.Keys);

                // check if audiofile exists
                foreach (string key in tmp)
                {
                    if (!(await PCLHelper.IsFileExistAsync(key + "_" + UserHelper.Language + ".mp3")))
                    {
                        baseCards.Remove(key);
                        source.Remove(key);
                        Debug.WriteLine("removed from list key: " + key);
                    }
                }

                Random random = new Random();
                while (baseCards.Count < 10)
                { // not enough items for gameCheck coords
                    Debug.WriteLine("source.Count = " + source.Count);
                    int randomIndex = random.Next(0, source.Count);
                    baseCards.Add(source[randomIndex]);
                    source.RemoveAt(randomIndex);
                }

                randomKeys = new List<string>();
                for (int i = 0; i < 10; i++)
                {
                    int index = random.Next(0, baseCards.Count);
                    randomKeys.Add(baseCards[index]);
                    baseCards.RemoveAt(index);
                }

                Debug.WriteLine("total keys: " + randomKeys.Count);
            });                        
        }


        async void SetAllAfterLoading() {

            StartBtnSource = Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.btn_right.png"); //Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.CAROUSEL.arrow1b.png");
            IsStartBtnEnabled = true;
            //IsProgressVisible = false;
            await AddAllItems();
            InitOnStart();
            // *** AddTouchView();

            while (gameScene == null)
            {
                Task.Delay(200).Wait();
            }
            
             gameScene.CreateAllSprites(theme, randomKeys);
             gameScene.Restart(randomKeys);

             UpdateUIInfo();

             IsStartBtnEnabled = true;
             IsStartBtnVisible = true;
             IsAnimating = false;
             Debug.WriteLine("CarouselGame_ViewModel -> SetAllAfterLoading -> done");
            
            
        }

        async void AnimateLoadingView(uint time)
        {
            Debug.WriteLine("CarouselGame_ViewModel -> AnimateLoadingView");
            await loadingView.TranslateTo(0, -App.Current.MainPage.Height, time);
            loadingView.IsVisible = false;
            loadingView.InputTransparent = true;           
            if (!App.Audio.playerBackground.IsPlaying) App.Audio.playerBackground.Play();
        }

        public void DoOnMatchesThePair()
        {
            if (gameScene.gameModel.aimed + gameScene.gameModel.misses > 16) shotsImages[0].IsVisible = false;
            else
            {
                Debug.WriteLine("shot number : " + (16 - (gameScene.gameModel.aimed + gameScene.gameModel.misses)));
                shotsImages[16 - (gameScene.gameModel.aimed + gameScene.gameModel.misses)].IsVisible = false;
            }
            UpdateUIInfo();

            App.Audio.playerCorrect.Play();
            activityReport.ReportProgress((8 - gameScene.gameModel.targets) / 8.0 * 100.0);                      
            
            if (gameScene.gameModel.targets <= 0)
            {
                Debug.WriteLine("YOU WON!!!");
                App.Audio.playerCorrect.Stop();
                App.Audio.playerBackground.Stop();
                                
                victoryView.EndOfGameVictory(starsInfo, post, activityReport);
            }
            else
            {
                if (gameScene.gameModel.playingItems.Count > 0)
                {
                    Device.StartTimer(TimeSpan.FromMilliseconds(500), DoAfterClickCorrect);  
                }
            }
        }

        bool DoAfterClickCorrect()
        {
            App.Audio.SayWord(gameScene.gameModel.targetItemKey, UserHelper.Language);
            gameScene.gameModel.canClick = true;
            return false;
        }

        public void DoOnMissedThePair()
        {
            Debug.WriteLine("MISSED");            

            if (gameScene.gameModel.aimed + gameScene.gameModel.misses - 1 > -1 && gameScene.gameModel.aimed + gameScene.gameModel.misses - 1 < 16)
            {
                UpdateUIInfo();
                shotsImages[16 - (gameScene.gameModel.aimed + gameScene.gameModel.misses)].IsVisible = false;
                App.Audio.playerWrong.Play();
                

                if (gameScene.gameModel.aimed + gameScene.gameModel.misses >= 16)
                {
                    Debug.WriteLine("Out of bullets - game over !!!");
                    gameScene.pause = true;
                    gameoverView.AnimateDown();                    
                }
                else
                {
                    Device.StartTimer(TimeSpan.FromMilliseconds(250), RepeatWordIfWrong);
                    IsAnimating = false;
                }
            }
        }

        bool RepeatWordIfWrong()
        {
            App.Audio.SayWord(gameScene.gameModel.targetItemKey, UserHelper.Language);            
            return false;
        }

        public void InitOnStart () {

            Debug.WriteLine("CarouselGame_ViewModel -> InitOnStart");            
            IsAnimating = false;
            foreach (Image img in shotsImages) {
                img.IsVisible = true;
            }   
        }

        void UpdateUIInfo() {
            Debug.WriteLine("CarouselGame_ViewModel -> UpdateUIInfo");
            TargetsText = gameScene.gameModel.targets.ToString();
            AimedText = gameScene.gameModel.aimed.ToString();
            MissesText = gameScene.gameModel.misses.ToString();
        }

        public void AddVictoryView(AbsoluteLayout absRoot) {
            Debug.WriteLine("CarouselGame_ViewModel -> AddVictoryView");

            victoryView = new VictoryView(navigation, OnClose, starsInfo);
            AbsoluteLayout.SetLayoutBounds(victoryView, new Rectangle(0, 0, 1, 1));
            AbsoluteLayout.SetLayoutFlags(victoryView, AbsoluteLayoutFlags.All);

            victoryView.SetSizesAndStartY(UI_Sizes.ScreenHeightX, 0.621, 0.05, -UI_Sizes.ScreenHeightX);

            absRoot.Children.Add(victoryView);
        }



        public void AddGameOverView(AbsoluteLayout absRoot)
        {
            Debug.WriteLine("CarouselGame_ViewModel -> AddGameOverView");
            gameoverView = new GameOverView(navigation, OnClose, OnRestart);
            AbsoluteLayout.SetLayoutBounds(gameoverView, new Rectangle(0, 0, 1, 1));
            AbsoluteLayout.SetLayoutFlags(gameoverView, AbsoluteLayoutFlags.All);
            gameoverView.SetSizesAndStartY(UI_Sizes.ScreenHeightX * 0.75, 0.621, 0.05, -UI_Sizes.ScreenHeightX);
            absRoot.Children.Add(gameoverView);
        }

        void OnRestart () {
            Debug.WriteLine("CarouselGame_ViewModel --> OnRestart ()");
            gameoverView.AnimateUp();

            InitOnStart();

            gameScene.Restart(randomKeys);

            UpdateUIInfo();

            gameScene.pause = false;

            App.Audio.playerBackground.Play();
            
            App.Audio.SayWord(gameScene.gameModel.targetItemKey, UserHelper.Language);
            IsAnimating = false;
        }

        public void AddShotsImages(Grid shotsGrid) {
            Debug.WriteLine("CarouselGame_ViewModel -> AddShotsImages");
            shotsImages = new Image[16];
            for (int i = 0; i < 16; i++) {
                int col = i / 8;
                int row = ( i % 8 ) * 2;
                shotsImages[i] = new Image {
                    Source = Forms9Patch.ImageSource.FromResource(imgResPrefix + "shot_color.png"),
                    Aspect = Aspect.AspectFit,
                    IsEnabled = false,
                };

                Image grayShot = new Image
                {
                    Source = Forms9Patch.ImageSource.FromResource(imgResPrefix + "shot_gray.png"),
                    Aspect = Aspect.AspectFit,
                    IsEnabled = false,
                };

                shotsGrid.Children.Add(grayShot, col, row);
                shotsGrid.Children.Add(shotsImages[i], col, row);
            }
        }

        public void SetUpSizes () {
            // check aspect ratio
            Debug.WriteLine("CarouselGame_ViewModel -> AddGameLayout");
            TotalWidth = UI_Sizes.ScreenWidthX;

            GameHeight = UI_Sizes.ScreenHeightX_UNIFORMED_TO_1_78;

            Debug.WriteLine("App.ScreenHeight = " + UI_Sizes.ScreenHeightX);
            Debug.WriteLine("GameHeight = " + GameHeight);     

            MenuFontSize = GameHeight * 0.05;
            GameWidth = GameHeight * 1.36;
            TotalMenuesWidth = (TotalWidth - GameWidth) * 0.5;
            GamePadding = new Thickness (GameHeight * 0.016);
            CloseBtnSize = GameHeight * 0.15;
            ShotsWidth = GameHeight * 0.11; 

        }
        
        public async void MenuButton_Tapped(object sender, System.EventArgs e) {
            
            Debug.WriteLine("MenuButton_Tapped");
            if (IsAnimating) return;
            IsAnimating = true;

            View view = sender as View;
            if (view.ClassId == "CloseBtn")
            {
                this.OnClose();
                await AnimateImage(view, 250);
                OnClose();
                await navigation.PopModalAsync();
            }
            else if (view.ClassId == "RepeatBtn")
            {
                await AnimateImage(view, 250);
                //repeat here
                App.Audio.SayWord(gameScene.gameModel.targetItemKey, UserHelper.Language);
                Debug.WriteLine("REPEAT, target is -->" + gameScene.gameModel.targetItemKey);
                IsAnimating = false;
            }
            else if (view.ClassId == "StartBtn")
            {
                await AnimateImage(view, 250);
                //START here
                Debug.WriteLine("START");
                
                AnimateStart(1000);
            }
            else
            {
                IsAnimating = false;
            }
        }

       public async void AnimateStart(uint time) {
            Debug.WriteLine("CarouselGame_ViewModel -> AnimateStart");

            try
            {                
                IsAnimating = true;
                await loadingView.TranslateTo(0, -App.Current.MainPage.Height, time);

                if (loadingView == null) return;
                loadingView.IsVisible = false;
                loadingView.InputTransparent = true;                

                App.Audio.playerBackground.Play();
                gameScene.pause = false;

                if (activityReport == null) return;
                activityReport.Start();
                // init game
                
                Debug.WriteLine("Target Item = " + gameScene.gameModel.targetItemKey);
                App.Audio.SayWord(gameScene.gameModel.targetItemKey, UserHelper.Language);

                IsAnimating = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("CarouselGame_ViewModel -> AnimateStart -> ex" + ex.Message);
            }            
        }

        public Task AnimateImage(View view, uint time)
        {
            
            return Task.Run(async () =>
            {
                await view.ScaleTo(0.8, time / 2);
                await view.ScaleTo(1.0, time / 2);
               
                return;
            });
        }

        public async void OnAppearing () {

            Debug.WriteLine("CarouselGame_ViewModel -> OnAppearing");
            if (loadingView !=null) {
                if (loadingView.IsVisible == false && gameoverView.IsVisible == false && victoryView.IsVisible == false) {
                    App.Audio.playerBackground.Play();
                }
            } 
            if (firstStart)
            {
                firstStart = false;
                OnLoadingAppeared();
                Device.StartTimer(TimeSpan.FromMilliseconds(500), StartLoading);
            }
        }

        public void OnClose()
        {
            Debug.WriteLine("CarouselGame_ViewModel -> OnClose");
            if (App.Audio.playerVictory != null && App.Audio.playerVictory.IsPlaying) App.Audio.playerVictory.Stop();
            if (App.Audio.playerBackground != null && App.Audio.playerBackground.IsPlaying) App.Audio.playerBackground.Stop();

            App.OnSleepEvent -= OnSleep;
            App.OnResumeEvent -= OnResume;

            DownloadHelper.DownloadHelper.OnLoadingStarted -= OnLoadingStarted;
            DownloadHelper.DownloadHelper.OnLoadingProgress -= OnLoadingProgress;
            DownloadHelper.DownloadHelper.OnLoadingEnded -= OnLoadingEnded;
            DownloadHelper.DownloadHelper.BreakDownloading = true;

            activityReport.OnDisappearing();
        }


        void OnSleep() {
            Debug.WriteLine("CarouselGame_ViewModel -> OnSleep");
            if (App.Audio.playerVictory != null && App.Audio.playerVictory.IsPlaying) App.Audio.playerVictory.Stop();
            if (App.Audio.playerBackground != null && App.Audio.playerBackground.IsPlaying) App.Audio.playerBackground.Pause();
            activityReport.OnSleep();
        }

        void OnResume() {
            Debug.WriteLine("CarouselGame_ViewModel -> OnResume");
            if (!victoryView.IsVisible)
            if (!loadingView.IsVisible) 
            if (!gameoverView.IsVisible)
            if (App.Audio.playerBackground != null) App.Audio.playerBackground.Play();
            activityReport.OnResume();
        }


        public void Dispose()
        {
            PropertyChanged = null;
            StartBtnSource = null;
            navigation = null;
            animLock = null;

            foreach (Image img in shotsImages)
            {
                img.Source = null;
            }
            shotsImages = null;

            victoryView.Dispose();
            victoryView = null;

            gameoverView.Dispose();
            gameoverView = null;

            loadingView = null;

            starsInfo = null;
            post = null;
            activityReport = null;
        }
    }
}
