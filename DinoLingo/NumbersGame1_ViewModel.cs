using System;
using System.Windows.Input;

using System.Collections;
using System.Collections.Generic;
using Xamarin.Forms;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using Plugin.SimpleAudioPlayer;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using DownloadHelper;
using Plugin.Connectivity;
using Newtonsoft.Json;
using FFImageLoading;

namespace DinoLingo
{
    public class NumbersGame1_ViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        //   === Navigation ===
        static int ERROR_PREFIX = 90;

        private INavigation navigation;

        //public ICommand ItemTappedCommand { get; set; }
        //public IEnumerable Items { get; set; }
        public string ScoreText { get; set; }
        public string LevelText { get; set; }
        public string TimeText { get; set; }
        public double GameHeight { get; set; }
        public double MenuWidth { get; set; }
        public double ScoreFontSize { get; set; }
        public double SmallScoreFontSize { get; set; }
        public double UIWidth { get; set; }
        public double CloseBtnSize { get; set; }
        public double SmallTextSize { get; set; }


        public string LoadingWaitText { get; set; }
        public bool IsGifVisible { get; set; }
        public ImageSource GifSource { get; set; }

        int score;
        double time;
        int level;

        int SCORE_INCREMENT = 100;
        bool timerJustStarted = false;
        bool timerPaused = false;
        View loadingView;   

        bool gameStarted = false;
        public bool isVisible = false;
        THEME_NAME themeName;
        ListItemStarsInfo starsInfo;
        public PostResponse.Post post;
        public VictoryView victoryView;
        public ActivityReport activityReport;             

        public NumbersGame1_ViewModel(INavigation navigation, View loadingView, THEME_NAME themeName, ListItemStarsInfo starsInfo)
        {
            this.navigation = navigation;

            this.loadingView = loadingView;
            this.themeName = themeName;
            this.starsInfo = starsInfo;

            App.OnSleepEvent += OnSleep;
            App.OnResumeEvent += OnResume;

            SetSizes();
        }

        public void Dispose ()
        {
            Debug.WriteLine("NumbersGame_ViewModel -> Dispose");
            App.OnSleepEvent -= OnSleep;
            App.OnResumeEvent -= OnResume;

            navigation = null;
           
            ScoreText = null;
            LevelText = null;
            TimeText = null;
            LoadingWaitText = null;
            GifSource = null;
        
            loadingView = null;        
            starsInfo = null;
            post = null;
            victoryView = null;
            activityReport = null;
            PropertyChanged = null;
    }

        public Action OnLoadingEndedSuccess;

        public async void StartLoading () {
            Debug.WriteLine("StartLoading ()");

            Restart();
            
            //get list from gameObjects
            List<string> wordsToDownload1 = new List<string>(Theme.Resources[(int)themeName].Item.Keys);            
            foreach (string key in Theme.Resources[(int)themeName].Item.Keys) {
                if (GameHelper.memory_GameObjects.HasKey(themeName, key) || GameHelper.sas_GameObjects.HasKey(themeName, key)) {
                    continue;
                }
                wordsToDownload1.Remove(key);
                Debug.WriteLine("removed from downloading : " + key);
            }

            DownloadHelper.DownloadHelper.OnLoadingStarted += OnLoadingStarted;
            DownloadHelper.DownloadHelper.OnLoadingProgress += OnLoadingProgress;
            DownloadHelper.DownloadHelper.OnLoadingEnded += OnLoadingEnded;

            //await Task.Delay(500);
            DownloadHelper.DownloadHelper.DownLoadWords(wordsToDownload1, themeName, null, themeName);
            

            
        }

        
        void OnLoadingStarted()
        {
            //LoadingWaitText = "Please wait... dowloading audio ...";
           
            IsGifVisible = true;
            GifSource = LoadingView_Logic.GetRandomDancingDinoImg();
        }

        void OnLoadingProgress()
        {
            Debug.WriteLine("OnLoadingProgress() --> progress = " + DownloadHelper.DownloadHelper.progress);
        }

        async void OnLoadingEnded(){
            Debug.WriteLine("OnLoadingEnded() --> progress = " + DownloadHelper.DownloadHelper.progress);
            
            switch (DownloadHelper.DownloadHelper.result) {
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
                        await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                        POP_UP.NO_CONNECTION
                        //+ POP_UP.GetCode(null, ERROR_PREFIX + 2)
                        , POP_UP.OK);
                        //("Error!", "No internet! Check your internet connection.", "Cancel");
                    }

                    await navigation.PopModalAsync();
                    break;
                case DownloadHelper.DownloadHelper.RESULT.NOTHING_TO_DOWNLOAD:
                    Debug.WriteLine("nothing to download");
                    //Download post if we need it
                    
                    if ((post = await victoryView.DownloadPost(starsInfo)) != null)
                    { // post downloaded ok
                        OnLoadingEndedSuccess?.Invoke();                        
                    }
                    else
                    {
                        OnLoadingEndedSuccess?.Invoke();
                        
                        //await App.Current.MainPage.DisplayAlert("Error!", "Some error in post data", "Cancel");
                        //await navigation.PopModalAsync();
                    }
                    
                    break;

                case DownloadHelper.DownloadHelper.RESULT.DOWNLOADED_OK:
                    Debug.WriteLine("downloaded - ok, total files = " + DownloadHelper.DownloadHelper.totalFilesDownloaded);
                    //Download post if we need it
                    if ((post = await victoryView.DownloadPost(starsInfo)) != null)
                    { // post downloaded ok
                        OnLoadingEndedSuccess?.Invoke();
                        
                    }
                    else
                    {
                        OnLoadingEndedSuccess?.Invoke();                        
                        //await App.Current.MainPage.DisplayAlert("Error!", "Some error in post data", "Cancel");
                        //await navigation.PopModalAsync();
                    }
                    break;
            }
            

        }

        public async void AnimateLoadingView(uint time) {
            await loadingView.TranslateTo(0, -App.Current.MainPage.Height, time);
            if (loadingView != null) loadingView.IsVisible = false;
            if (loadingView != null)  loadingView.InputTransparent = true;
            if (loadingView != null) Device.StartTimer(TimeSpan.FromSeconds(1), OnTimerTick);
            gameStarted = true;
            if (loadingView != null) activityReport.Start();
            if (loadingView != null) if (!App.Audio.playerBackground.IsPlaying & isVisible) App.Audio.playerBackground.Play();
            
        }

        void OnSleep()
        {
            isVisible = false;
            if (App.Audio.playerBackground != null && App.Audio.playerBackground.IsPlaying) {
                Debug.WriteLine("try to --> playerBackground.Pause()"); 
                App.Audio.playerBackground.Pause();
            }
            activityReport.OnSleep();
        }

        void OnResume ()
        {
            isVisible = true;
            if (!timerPaused && App.Audio.playerBackground!= null & gameStarted) App.Audio.playerBackground.Play();
            activityReport.OnResume();
        }

        public void Restart() {
            Debug.WriteLine("NumbersGame_ViewModel -> Restart");
            score = 0;
            ScoreText = score.ToString();

            time = 0;
            TimeText = ((int)(time / 60)).ToString("D2") + ":" + ((int)((int)time % 60)).ToString("D2");

            level = 1;
            LevelText = level.ToString();

            timerJustStarted = true;
            timerPaused = false;
            Debug.WriteLine("NumbersGame_ViewModel -> Restart - OK");
        }

        public void ScoreUp () {
            score += SCORE_INCREMENT;
            ScoreText = score.ToString();
        }

        private bool OnTimerTick()
        {
            Debug.WriteLine("NumbersGame_ViewModel -> OnTimerTick");
            if (loadingView == null) return false;

            if (timerJustStarted) {
                timerJustStarted = false;
                return true;
            }

            if (timerPaused) return true;
                
            time++;
            TimeText = ((int)(time / 60)).ToString("D2") + ":" + ((int)((int)time % 60)).ToString("D2");

            return true;
        }

        public void PauseTimer () {
            timerPaused = true;
        }

        public void SoundVictory() {
            App.Audio.playerVictory.Play();
        }

        public void SoundRight () {
            App.Audio.playerCorrect.Play();
        }

        public void OnClose () {
            Debug.WriteLine("try to dispose sounds");
            isVisible = false;
            if (App.Audio.playerBackground != null) {
                if (App.Audio.playerBackground.IsPlaying) App.Audio.playerBackground.Stop();
            }
            if (App.Audio.playerVictory.IsPlaying) App.Audio.playerVictory.Stop();

            App.OnSleepEvent -= OnSleep;
            App.OnResumeEvent -= OnResume;

            DownloadHelper.DownloadHelper.BreakDownloading = true;
            DownloadHelper.DownloadHelper.OnLoadingStarted -= OnLoadingStarted;
            DownloadHelper.DownloadHelper.OnLoadingProgress -= OnLoadingProgress;
            DownloadHelper.DownloadHelper.OnLoadingEnded -= OnLoadingEnded;
        }

        void SetSizes() {   
            GameHeight = UI_Sizes.ScreenHeightX_UNIFORMED_TO_1_78;

            MenuWidth = (UI_Sizes.ScreenWidthX - GameHeight) * 0.5;
            UIWidth = MenuWidth * 0.75;
            CloseBtnSize = GameHeight * 0.15;
            ScoreFontSize = GameHeight * 0.06;
            SmallScoreFontSize = ScoreFontSize * 0.8; 
            SmallTextSize = GameHeight * 0.04;
        } 

    }
}
