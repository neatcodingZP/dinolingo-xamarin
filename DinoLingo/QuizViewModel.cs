using System;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading.Tasks;
using FFImageLoading.Forms;
using System.IO;
using System.Reflection;
using Plugin.SimpleAudioPlayer;
using Plugin.Connectivity;
using Newtonsoft.Json;
using FFImageLoading;

namespace DinoLingo
{
    public class QuizViewModel: INotifyPropertyChanged
    {
        static int ERROR_PREFIX = 100;

        public event PropertyChangedEventHandler PropertyChanged;

        public INavigation navigation;
        public THEME_NAME theme;

        public double GameZoneWidth { get; set; }
        public double GameWidth { get; set; }
        public double GameHeight { get; set; }
        public double MenuWidth { get; set; }
        public double MenuFontSize { get; set; }
        public double SmallMenuFontSize { get; set; }

        public double BorderSize { get; set; }
        public double SpacingSize { get; set; }
        public double CloseBtnSize { get; set; }
        public double StrokeLineWidth1 { get; set; }
        public double StrokeLineWidth2 { get; set; }
        public double CornerRadius1 { get; set; }
        public double CornerRadius2 { get; set; }


        public string ScoreText { get; set; }
        public string TimeText { get; set; }
        public string QuestionsText { get; set; } 

        public string LoadingWaitText { get; set; }
        public bool IsStartBtnEnabled { get; set; }
        public bool IsStartBtnVisible { get; set; }
        public ImageSource StartBtnSource { get; set; }


        VictoryView victoryView;
        GameOverView gameoverView;

        // === Game Model ===
        View[] cellViews;
        Image[] cellImages;
        Dictionary<string, string> additionalKeys = new Dictionary<string, string> {
            ["HELLOW"] = "DinoLingo.Resources.QUIZGAME.hel.jpg",
            ["HOWAREYOU"] = "DinoLingo.Resources.QUIZGAME.hay.jpg",
            ["GOODBYE"] = "DinoLingo.Resources.QUIZGAME.bye.jpg",
        };

        List<string> additionalCards;
        Queue<string> itemsFor20Questions;
        int TOTAL_QUESTIONS = 20;
        int WIN_SCORE = 16;

        List<string> baseCards, current4Cards;

        int targetCardIndex;
        Random random = new Random();
        int score;
        double time;
        public int questions;

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

        bool timerJustStarted = false;
        bool timerPaused = false;
        bool firstStart = true;
        View loadingView;
        string language;
        ListItemStarsInfo starsInfo;
        PostResponse.Post post;
        ActivityReport activityReport;

        public QuizViewModel(INavigation navigation, THEME_NAME ThemeName, View[] cellViews, Image[] cellImages, ListItemStarsInfo starsInfo)
        {
            this.navigation = navigation;
            theme = ThemeName;
            this.cellViews = cellViews;
            this.cellImages = cellImages;
            this.starsInfo = starsInfo;

            RestartUI();

            App.OnSleepEvent += OnSleep;
            App.OnResumeEvent += OnResume;

            int index = starsInfo.isSingleItem ? starsInfo.eventArgs.MyItem.Index : (starsInfo.eventArgs.MyItem as DoubleItem).Index2;
            activityReport = new ActivityReport(ActivityReport.ACT_TYPE.GAME, starsInfo.data[index].id);
            //activityReport.OnCompleted += ActivityReport_OnCompleted;
        }

        public void AddVictoryView(AbsoluteLayout absRoot)
        {
            victoryView = new VictoryView(navigation, OnClose, starsInfo);
            AbsoluteLayout.SetLayoutBounds(victoryView, new Rectangle(0, 0, 1, 1));
            AbsoluteLayout.SetLayoutFlags(victoryView, AbsoluteLayoutFlags.All);
            absRoot.Children.Add(victoryView);
        }

        public void AddGameOverView(AbsoluteLayout absRoot)
        {
            gameoverView = new GameOverView(navigation, OnClose, OnRestart);
            AbsoluteLayout.SetLayoutBounds(gameoverView, new Rectangle(0, 0, 1, 1));
            AbsoluteLayout.SetLayoutFlags(gameoverView, AbsoluteLayoutFlags.All);
            absRoot.Children.Add(gameoverView);
        }


        async void OnRestart()
        {
            Debug.WriteLine("QuizGame_ViewModel --> OnRestart ()");

            timerJustStarted = true;
            timerPaused = false;

            RestartUI();
            InitDataOnRestart();

            UpdateUIInfo();

            gameoverView.AnimateUp();

            SetCardsAndTarget();

            await FadeOutAllCards();

            await Task.Delay(250);
            App.Audio.SayWord(current4Cards[targetCardIndex], language);
        }

        async Task InitDataForGame() {
            additionalCards = new List<string>(additionalKeys.Keys);
            List<string> tmp = new List<string>(Theme.Resources[(int)theme].Item.Keys);
            baseCards = new List<string>(Theme.Resources[(int)theme].Item.Keys);
            // check if audiofile exists
            foreach (string key in tmp)
            {
                if (!(await PCLHelper.IsFileExistAsync(key + "_" + language + ".mp3")))
                {
                    baseCards.Remove(key);
                    Debug.WriteLine("removed from list key: " + key);
                }
            }

        }

        void InitDataOnRestart () {
            // init list of 4 * 20 items

            List<string> tempItemsFor20Questions = new List<string>();

            List<string> tempAddCards = new List<string>(additionalCards);
            List<string> tempBaseCards = new List<string>(baseCards);


            for (int i = 0; i < TOTAL_QUESTIONS; i++) {
                bool haveAdditionalCard = false;
                for (int j = 0; j < 4; j++) {
                    // chance of additional card - 20%
                    if (!haveAdditionalCard && random.NextDouble() < 0.20) { // add additional card
                        haveAdditionalCard = true;
                        int index = random.Next(0, tempAddCards.Count);
                        tempItemsFor20Questions.Add(tempAddCards[index]);
                        tempAddCards.RemoveAt(index);
                        if (tempAddCards.Count == 0) tempAddCards = new List<string>(additionalCards);
                    }
                    else { // add regular card
                        int index;
                        string cardToAdd;
                        // check for duplicates
                        bool isDuplicate;
                        do
                        {
                            isDuplicate = false;
                            index = random.Next(0, tempBaseCards.Count);
                            cardToAdd = tempBaseCards[index];
                            tempBaseCards.RemoveAt(index);
                            for (int k = 0; k < j; k ++) {
                                if (cardToAdd == tempItemsFor20Questions[tempItemsFor20Questions.Count - 1 - k]) {
                                    isDuplicate = true;
                                    break;
                                }
                            }
                        }
                        while (isDuplicate);

                        tempItemsFor20Questions.Add(cardToAdd);

                        if (tempBaseCards.Count == 0) tempBaseCards = new List<string>(baseCards);
                    }
                }


            }

           

            // finally
            itemsFor20Questions = new Queue<string>(tempItemsFor20Questions);

            Debug.WriteLine("list of items: ");
            foreach (string s in itemsFor20Questions) {
                Debug.Write(s + ", ");
            }
        }

        void RestartUI () {
            score = 0;
            time = 0;
            questions = 0;
            ShowTime();
            UpdateUIInfo();
        }

       
        void ShowTime () {
            TimeText = ((int)(time / 60)).ToString("D2") + ":" + ((int)((int)time % 60)).ToString("D2");
        }

        void UpdateUIInfo(){
            ScoreText = score.ToString();
            QuestionsText = questions.ToString();
        }

        void SetCardsAndTarget()
        {
            
                current4Cards = new List<string>();
                for (int i = 0; i < 4; i++)
                {
                    current4Cards.Add(itemsFor20Questions.Dequeue());
                    if (additionalCards.Contains(current4Cards[i]))
                    { // it is additional card
                        Debug.WriteLine("Additional card, source = " + additionalKeys[current4Cards[i]]);
                        cellImages[i].Source = Forms9Patch.ImageSource.FromResource(additionalKeys[current4Cards[i]]);
                        cellImages[i].Aspect = Aspect.Fill;
                    }
                    else
                    { // it is main card
                        Debug.WriteLine("base card, source = " + Theme.GetTileImageSourceForPairsGame(theme, current4Cards[i]));
                        cellImages[i].Source = Forms9Patch.ImageSource.FromResource(Theme.GetTileImageSourceForPairsGame(theme, current4Cards[i]));
                        cellImages[i].Aspect = Aspect.AspectFit;
                    }
                }

                targetCardIndex = random.Next(0, 4);

            // *** DEBUG INFO ***
            for (int i = 0; i < current4Cards.Count; i++)
            {
                Debug.WriteLine($"card #{i + 1} = {current4Cards[i]}");
            }
            Debug.WriteLine("target = " + current4Cards[targetCardIndex]);
           
        }

        public Task FadeOutAllCards()
        {
            Debug.WriteLine("FadeOutAllCards()");
            foreach (View cell in cellViews)
            {
                cell.Opacity = 0;
                cell.IsVisible = true;
            };

            return Task.Run(async () =>
            {
                await Task.WhenAll(
                    ViewExtensions.FadeTo(cellViews[0], 1, 250),
                    ViewExtensions.FadeTo(cellViews[1], 1, 250),
                    ViewExtensions.FadeTo(cellViews[2], 1, 250),
                    ViewExtensions.FadeTo(cellViews[3], 1, 250)
                );
                IsAnimating = false;
            });
        }

        public Task FadeInAllCards()
        {
            IsAnimating = true;
            return Task.Run(async () =>
            {
                try
                {
                    await Task.WhenAll(
                    ViewExtensions.FadeTo(cellViews[0], 0, 200),
                    ViewExtensions.FadeTo(cellViews[1], 0, 200),
                    ViewExtensions.FadeTo(cellViews[2], 0, 200),
                    ViewExtensions.FadeTo(cellViews[3], 0, 200) );
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("QuizViewModel -> FadeInAllCards -> ex:" + ex.Message);
                }                
            });
        }

        public async void MenuButton_Tapped(object sender, System.EventArgs e)
        {

            Debug.WriteLine("MenuButton_Tapped");
            if (IsAnimating) return;
            View view = sender as View;
            if (view.ClassId == "CloseBtn")
            {
                await AnimateImage(view, 250);
                await navigation.PopModalAsync();
            }
            else if (view.ClassId == "RepeatBtn")
            {
                await AnimateImage(view, 250);
                //repeat here
                Debug.WriteLine("REPEAT, target is -->" + current4Cards[targetCardIndex]);

                App.Audio.SayWord(current4Cards[targetCardIndex], language);
                IsAnimating = false;
            }
            else if (view.ClassId == "cell_0")
            {
                ProcessOnCellTap(view, 0);
            }
            else if (view.ClassId == "cell_1")
            {
                ProcessOnCellTap(view, 1);
            }
            else if (view.ClassId == "cell_2")
            {
                ProcessOnCellTap(view, 2);
            }
            else if (view.ClassId == "cell_3")
            {
                ProcessOnCellTap(view, 3);
            }
            else if (view.ClassId == "StartBtn")
            {
                await AnimateImage(view, 250);
                //START here

                await InitDataForGame();
                InitDataOnRestart();

                UpdateUIInfo();
                Debug.WriteLine("START");
                SetCardsAndTarget();
                AnimateStart(1000);

                await FadeOutAllCards();

                await Task.Delay(250);
                App.Audio.SayWord(current4Cards[targetCardIndex], language);

                Device.StartTimer(TimeSpan.FromSeconds(1), OnTimerTick);
                IsAnimating = false;
            };
        }

        private bool OnTimerTick()
        {
            Debug.WriteLine("QuizViewModel -> OnTimerTick");
            if (victoryView == null) return false;

            if (timerJustStarted)
            {
                timerJustStarted = false;
                return true;
            }

            if (timerPaused) return true;

            time++;
            TimeText = ((int)(time / 60)).ToString("D2") + ":" + ((int)((int)time % 60)).ToString("D2");

            return true;
        }

        public void PauseTimer()
        {
            timerPaused = true;
        }

        async void ProcessOnCellTap(View cell, int index) {
            try
            {
                Debug.WriteLine($"Tapped i= {index}, key = {current4Cards[index]}");

                questions++;
                if (index == targetCardIndex)
                { //correct card
                    score++;
                    activityReport.ReportProgress(score / (double)WIN_SCORE * 100.0);
                    App.Audio.playerCorrect.Play();

                }
                else
                { // wrong card
                    App.Audio.playerWrong.Play();
                    //animate correct answer !!!
                }
                await AnimateImage(cell, 250);

                UpdateUIInfo();
                if (questions >= TOTAL_QUESTIONS && score >= WIN_SCORE) // victory
                {
                    IsAnimating = true;
                    PauseTimer();
                    if (App.Audio.playerCorrect.IsPlaying) App.Audio.playerCorrect.Stop();
                    if (App.Audio.playerWrong.IsPlaying) App.Audio.playerWrong.Stop();
                    victoryView.EndOfGameVictory(starsInfo, post, activityReport);
                }
                else if (questions >= TOTAL_QUESTIONS) // game over
                {
                    PauseTimer();
                    gameoverView.AnimateDown();
                }
                else
                { // next round
                    await FadeInAllCards();
                    SetCardsAndTarget();

                    await FadeOutAllCards();

                    await Task.Delay(250);
                    App.Audio.SayWord(current4Cards[targetCardIndex], language);
                }
                IsAnimating = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("QuizViewMode -> ProcessOnCellTap -> ex:" + ex.Message);
            }            
        }



        public Task AnimateStart_async(uint time)
        {
            IsAnimating = true;
            return Task.Run(async () =>
            {
                try
                {
                    await loadingView.TranslateTo(0, -loadingView.Height, time);
                    IsAnimating = false;
                    return;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("QuizViewModel -> AnimateStart_async -> ex:" + ex.Message);
                }                
            });
        }

        public async void AnimateStart(uint time)
        {
            try
            {
                await AnimateStart_async(time);
                loadingView.IsVisible = false;
                loadingView.InputTransparent = true;

                activityReport.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("QuizViewModel -> AnimateStart -> ex:" + ex.Message);
            }
        }

        public Task AnimateImage(View view, uint time)
        {
            IsAnimating = true;
            return Task.Run(async () =>
            {
                await view.ScaleTo(0.8, time / 2);
                await view.ScaleTo(1.0, time / 2);

                return;
            });
        }

        public void OnClose() {
            
        }

        bool StartLoading () { 
            // check aspect ratio
            GameHeight = UI_Sizes.ScreenHeightX_UNIFORMED_TO_1_78;

            GameWidth = GameHeight * 1.1;
            MenuWidth = GameHeight * 0.5;
            SpacingSize = (UI_Sizes.ScreenWidthX - GameWidth - MenuWidth) * 0.2;
            BorderSize = SpacingSize * 2;

            MenuFontSize = GameHeight * 0.06;
            SmallMenuFontSize = MenuFontSize * 0.8;
            CloseBtnSize = UI_Sizes.CloseBtnSize;

            CornerRadius1 = GameHeight * 0.025;
            StrokeLineWidth1 = GameHeight * 0.01;

            CornerRadius2 = GameHeight * 0.035;
            StrokeLineWidth2 = GameHeight * 0.0125;

            victoryView.SetSizesAndStartY(UI_Sizes.ScreenHeightX, 0.621, 0.05, -UI_Sizes.ScreenHeightX);
            gameoverView.SetSizesAndStartY(GameHeight * 0.75, 0.621, 0.05, -UI_Sizes.ScreenHeightX);

            Debug.WriteLine("StartLoading ()");

            // DO DOWNLOAD HERE

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

            List<string> wordsToDownload2 = new List<string>(additionalKeys.Keys);

            DownloadHelper.DownloadHelper.OnLoadingStarted += OnLoadingStarted;
            DownloadHelper.DownloadHelper.OnLoadingProgress += OnLoadingProgress;
            DownloadHelper.DownloadHelper.OnLoadingEnded += OnLoadingEnded;

            DownloadHelper.DownloadHelper.DownLoadWords(wordsToDownload1, theme,
                                                        wordsToDownload2, theme);
            return false;
        }

        async void OnLoadingAppeared()
        {
            IsStartBtnEnabled = false;
            //LoadingPercentText = "Checking data for game";
            //ProgressValue = 0;
            language = UserHelper.Language;
        }

        void OnLoadingStarted()
        {
            //IsStartBtnVisible = false;
            //IsProgressVisible = true;            
            StartBtnSource = LoadingView_Logic.GetRandomDancingDinoImg();
            IsStartBtnVisible = true;
            //LoadingWaitText = "Please wait... dowloading audio ...";
            //LoadingPercentText = "0 %";
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
                        //("Error!", "Error, while loading audio from server..., download.result = " + DownloadHelper.DownloadHelper.result, "Cancel");
                    }

                    await navigation.PopModalAsync();
                    break;
                case DownloadHelper.DownloadHelper.RESULT.ERROR_NO_CONNECTION:
                    if (AsyncMessages.CheckConnectionTimeout())
                    {
                        await App.Current.MainPage.DisplayAlert(POP_UP.OOPS, POP_UP.NO_CONNECTION
                            //+ POP_UP.GetCode(null, ERROR_PREFIX + 2)
                            , POP_UP.OK);
                        //("Error!", "No internet! Check your internet connection.", "Cancel");
                    }

                    await navigation.PopModalAsync();
                    break;
                case DownloadHelper.DownloadHelper.RESULT.NOTHING_TO_DOWNLOAD:
                    Debug.WriteLine("nothing to download");
                    // WHEN DOWNLOADED 
                    if ((post = await victoryView.DownloadPost(starsInfo)) != null)
                    { // post downloaded ok
                        SetAllAfterLoading();
                    }
                    else
                    {
                        SetAllAfterLoading();
                        //await App.Current.MainPage.DisplayAlert("Error!", "Some error in post data", "Cancel");
                        //await navigation.PopModalAsync();
                    }
                    break;
                case DownloadHelper.DownloadHelper.RESULT.DOWNLOADED_OK:
                    Debug.WriteLine("downloaded - ok, total files = " + DownloadHelper.DownloadHelper.totalFilesDownloaded);
                    // WHEN DOWNLOADED 
                    if ((post = await victoryView.DownloadPost(starsInfo)) != null)
                    { // post downloaded ok
                        SetAllAfterLoading();
                    }
                    else
                    {
                        SetAllAfterLoading();
                        //await App.Current.MainPage.DisplayAlert("Error!", "Some error in post data", "Cancel");
                        //await navigation.PopModalAsync();
                    }
                    break;
            }
        }

        void SetAllAfterLoading() {
            StartBtnSource = Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.btn_right.png"); 
            //IsProgressVisible = false;
            IsStartBtnEnabled = true;
            IsStartBtnVisible = true;
            IsAnimating = false;
        }



        public async void OnAppearing(View loadingView) {
            if (firstStart)
            {
                this.loadingView = loadingView;
                OnLoadingAppeared();
                Device.StartTimer(TimeSpan.FromMilliseconds(500), StartLoading);
                firstStart = false;
            }
        }

        public void OnDisappearing()
        {
            if (App.Audio.playerVictory.IsPlaying) App.Audio.playerVictory.Stop();
            App.OnSleepEvent -= OnSleep;
            App.OnResumeEvent -= OnResume;

            DownloadHelper.DownloadHelper.OnLoadingStarted -= OnLoadingStarted;
            DownloadHelper.DownloadHelper.OnLoadingProgress -= OnLoadingProgress;
            DownloadHelper.DownloadHelper.OnLoadingEnded -= OnLoadingEnded;

            activityReport.OnDisappearing();
        }


        void OnSleep()
        {
            if (App.Audio.playerVictory != null && App.Audio.playerVictory.IsPlaying) App.Audio.playerVictory.Stop();
            activityReport.OnSleep();
        }

        void OnResume()
        {
            activityReport.OnResume();
        }

        public void Dispose()
        {
            PropertyChanged = null;
            navigation = null;
            StartBtnSource = null;

            victoryView.Dispose();
            victoryView = null;

            gameoverView.Dispose();
            gameoverView = null;

            cellViews = null;
            cellImages = null;

            animLock = null;
            loadingView = null;
        
            starsInfo = null;
            post = null;
            activityReport = null;
        }
    }
}
