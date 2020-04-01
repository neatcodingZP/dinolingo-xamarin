using CocosSharp;
using DinoLingo.Games;
using DinoLingo.MyViews;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DinoLingo
{
    public class SprintGame_Page: ContentPage
    {
        static int ERROR_PREFIX = 300;
        Color backgroundBlueColor = (Color)Application.Current.Resources["BackgroundBlueColor"];

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

        
        public PostResponse.Post post = null;
        ListItemStarsInfo starsInfo;
        public ActivityReport activityReport;

        MyViews.VictoryView victoryView_new_;

        MyViews.TwoButtonsView playAgainView_;

        LoadingView loadingView_;
        ContentView viewForGame_;
        SprintGameModel gameModel;

        // cocos game
        GameScene_SprintGame gameScene;        
        CocosSharpView gameView;

        THEME_NAME themeName;

        public SprintGame_Page (THEME_NAME themeName, ListItemStarsInfo starsInfo)
        {
            this.themeName = themeName;
            var absLayout = new AbsoluteLayout() { BackgroundColor = backgroundBlueColor };

            int index = starsInfo.isSingleItem ? starsInfo.eventArgs.MyItem.Index : (starsInfo.eventArgs.MyItem as DoubleItem).Index2;
            activityReport = new ActivityReport(ActivityReport.ACT_TYPE.GAME, starsInfo.data[index].id);

            LoadingView loadingView = new LoadingView();
            loadingView_ = loadingView;
            AbsoluteLayout.SetLayoutBounds(loadingView, new Rectangle(0.5, 0.5, 1, 1));
            AbsoluteLayout.SetLayoutFlags(loadingView, AbsoluteLayoutFlags.All);

            loadingView.BackgroundColor = LoadingView_Logic.GetRandomColor();
            loadingView.mainImage_.IsVisible = true;
            loadingView.mainImage_.Source = LoadingView_Logic.GetRandomDancingDinoImg();

            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (sender, e) =>
            {
                View view = (View)sender;
                OnButtonTapped(view);
            };

            loadingView.closeBtn_.GestureRecognizers.Add(tapGestureRecognizer);

            ContentView viewForGame = new ContentView();
            viewForGame_ = viewForGame;
            AbsoluteLayout.SetLayoutBounds(viewForGame, new Rectangle(0.5, 0.5, 1, 1));
            AbsoluteLayout.SetLayoutFlags(viewForGame, AbsoluteLayoutFlags.All);

            this.starsInfo = starsInfo;                    

            MyViews.VictoryView victoryView_new = new MyViews.VictoryView();
            victoryView_new.OnClose += OnClose;
            victoryView_new_ = victoryView_new;

            MyViews.TwoButtonsView playAgainView = new MyViews.TwoButtonsView(Translate.GetString("popup_oops"),
                Translate.GetString("purchase_try_again"),
                Translate.GetString("popup_cancel"));
            playAgainView.OnButton1Clicked += OnButton1Clicked;
            playAgainView.OnButton2Clicked += OnButton2Clicked;
            playAgainView_ = playAgainView;


            absLayout.Children.Add(viewForGame);

            absLayout.Children.Add(loadingView);
            LoadingView_Logic.ShowCloseBtnTimer(loadingView.closeBtn_);

            
            absLayout.Children.Add(victoryView_new);
            absLayout.Children.Add(playAgainView);

            StartLoading();

            Content = absLayout;

            App.OnSleepEvent += OnSleep;
            App.OnResumeEvent += OnResume;
        }

        void OnButton1Clicked ()
        {
            Debug.WriteLine("SprintGame_Page -> OnButton1Clicked");
            Device.BeginInvokeOnMainThread(async () => {
                playAgainView_.Hide();
                gameScene.Restart = true;
            });
        }

        void OnButton2Clicked()
        {            
            Debug.WriteLine("SprintGame_Page -> OnButton2Clicked");
            Device.BeginInvokeOnMainThread(async () => {
                OnClose();
                await App.Current.MainPage.Navigation.PopModalAsync();
            }); 
        }

        void OnSleep()
        {
            /*
            if (App.Audio.playerBackground != null && App.Audio.playerBackground.IsPlaying)
            {
                Debug.WriteLine("try to --> playerBackground.Pause()");
                App.Audio.playerBackground.Pause();
            }
            */
            activityReport.OnSleep();
        }

        void OnResume()
        {            
            //*if (App.Audio.playerBackground != null && gameModel.state != SprintGameModel.State.TIME_FINISHED) App.Audio.playerBackground.Play();
            activityReport.OnResume();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            activityReport.OnDisappearing();
        }

        public void StartLoading()
        {
            Debug.WriteLine("SprintGame_Page -> StartLoading");

            //get list from gameObjects
            gameModel = new SprintGameModel(themeName);
            List<string> wordsToDownload1 = gameModel.GetWordsToPlay();
            DownloadHelper.DownloadHelper.OnLoadingEnded += OnLoadingEnded;            
            DownloadHelper.DownloadHelper.DownLoadWords(wordsToDownload1, themeName, null, themeName);
        }

        async void OnLoadingEnded()
        {
            Debug.WriteLine("SprintGame_Page() -> OnLoadingEnded");

           switch (DownloadHelper.DownloadHelper.result)
            {
                case DownloadHelper.DownloadHelper.RESULT.NONE:
                case DownloadHelper.DownloadHelper.RESULT.ERROR:
                case DownloadHelper.DownloadHelper.RESULT.DOWNLOAD_STARTED:
                    if (AsyncMessages.CheckDisplayAlertTimeout())
                    {
                        await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                        POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(null, ERROR_PREFIX + 1), POP_UP.OK);
                    }
                    
                    
                    await App.Current.MainPage.Navigation.PopModalAsync();
                    break;
                case DownloadHelper.DownloadHelper.RESULT.ERROR_NO_CONNECTION:
                    if (AsyncMessages.CheckConnectionTimeout())
                    {
                        await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                        POP_UP.NO_CONNECTION
                        //+ POP_UP.GetCode(null, ERROR_PREFIX + 2)
                        , POP_UP.OK);
                    };
                    await App.Current.MainPage.Navigation.PopModalAsync();
                    break;

                case DownloadHelper.DownloadHelper.RESULT.NOTHING_TO_DOWNLOAD:
                    //Download post if we need it
                    post = await victoryView_new_.DownloadPost(starsInfo);
                    /*
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

    */
                    OnLoadingEndedSuccess();
                    break;

                case DownloadHelper.DownloadHelper.RESULT.DOWNLOADED_OK:
                    Debug.WriteLine("downloaded - ok, total files = " + DownloadHelper.DownloadHelper.totalFilesDownloaded);
                    post = await victoryView_new_.DownloadPost(starsInfo);
                    //Download post if we need it
                    /*
                     
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

                    */
                    OnLoadingEndedSuccess();
                    break;
            }
        }

        void OnLoadingEndedSuccess()
        {
            Debug.WriteLine("SprintGame_Page() -> OnLoadingEndedSuccess");
            Device.BeginInvokeOnMainThread(()=> {
                AddCocosGameView(viewForGame_);
            }); 
            
        }

        void AddCocosGameView(ContentView contentView)
        {
            Debug.WriteLine("SprintGame_Page() -> AddCocosGameView");
            // This hosts our game view.
            gameView = new CocosSharpView()
            {
                // Notice it has the same properties as other XamarinForms Views
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                // This gets called after CocosSharp starts up:
                ViewCreated = HandleViewCreated,
                BackgroundColor = backgroundBlueColor,
            };
            gameView.IsVisible = false;
            contentView.Content = gameView;
        }

        async void HandleViewCreated(object sender, EventArgs e)
        {
            Debug.WriteLine("SprintGame_Page() -> HandleViewCreated");
            var gameView = sender as CCGameView;
            if (gameView != null)
            {
                // This sets the game "world" resolution to 100x100:
                gameView.DesignResolution = new CCSizeI(UI_Sizes.ScreenWidth, UI_Sizes.ScreenHeight);
                // GameScene is the root of the CocosSharp rendering hierarchy:
                gameScene = new GameScene_SprintGame(gameView, gameModel, this.gameView);

                gameScene.OnEndGame += OnEndGame;
                gameScene.OnMadeClick += OnMadeClick;

                //gameScene.memoryGameModel.DoOnMatchesThePair = DoOnMatchesThePair;
                // gameScene.memoryGameModel.DoOnMissedThePair =
                //gameScene.memoryGameModel.DoOnOpenCard = DoOnOpenCard;

                // Starts CocosSharp:
                gameView.RunWithScene(gameScene);



                Device.BeginInvokeOnMainThread(async ()=> {                    
                    this.gameView.IsVisible = true;
                    // animate loading view
                    await loadingView_.AnimateUp(1000);
                    activityReport?.Start();
                });                
                
            }
        }

        public void OnEndGame ()
        {            
            Device.BeginInvokeOnMainThread(() => {
                if (gameModel.correct >= gameModel.MINIMUM_CORRECT)
                {
                    Debug.WriteLine("SprintGame_Page() -> victoryView_new_.EndOfGameVictory");
                    victoryView_new_.EndOfGameVictory(starsInfo, post, activityReport); // victory
                }
                else
                {
                    Debug.WriteLine("SprintGame_Page() -> playAgainView_.Show");
                    playAgainView_.Show(); // show replay dialog
                }                              
            });
        }

        public void OnMadeClick ()
        {
            Device.BeginInvokeOnMainThread(() => {

                double progress = (double)gameModel.correct / gameModel.MINIMUM_CORRECT * 100.0;

                if (progress > 100) progress = 100;
                if (!activityReport.IsCompleted)
                    //activityReport.ReportProgress((double) gameModel.correct / gameModel.TOTAL_WORDS_IN_SPRINT * 100.0);

                activityReport.ReportProgress(progress);
            });
        }

        public void OnClose()
        {
            Debug.WriteLine("try to dispose sounds");            
            if (App.Audio.playerBackground != null)
            {
                if (App.Audio.playerBackground.IsPlaying) App.Audio.playerBackground.Stop();
            }
            if (App.Audio.playerVictory.IsPlaying) App.Audio.playerVictory.Stop();

            //App.OnSleepEvent -= OnSleep;
            //App.OnResumeEvent -= OnResume;

            DownloadHelper.DownloadHelper.BreakDownloading = true;            
            DownloadHelper.DownloadHelper.OnLoadingEnded -= OnLoadingEnded;
        }

        async void OnButtonTapped(View view)
        {
            if (IsAnimating) return;
            IsAnimating = true;
            await AnimateView(view, 250);
            Debug.WriteLine("SprintGame_Page -> OnButtonTapped -> ClassId =" + view.ClassId);

            if (view.ClassId == CloseButton.CLASS_ID)
            {

                await App.Current.MainPage.Navigation.PopModalAsync();
            }            
        }

        Task AnimateView(View view, uint time)
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
                    
                }
                return;
            });
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();
            if (Parent == null)
            {
                Dispose();
                GC.Collect();
                MemoryLeak.TrackMemory();
            }
        }

        void Dispose()
        {
            Debug.WriteLine("SprintGame_Page -> Dispose");

            DownloadHelper.DownloadHelper.OnLoadingEnded -= OnLoadingEnded;

            gameScene.OnEndGame -= OnEndGame;
            gameScene.OnMadeClick -= OnMadeClick;
            gameScene = null;

            gameView = null;
            gameModel = null;

            if (loadingView_ != null) loadingView_.Dispose();
            loadingView_ = null;

            viewForGame_.Content = null;
            viewForGame_ = null;

            Content = null;
            BindingContext = null;
            animLock = null;
                       
            post = null;
            starsInfo = null;
            activityReport = null;

            if (victoryView_new_ != null) victoryView_new_.OnClose -= OnClose;
            victoryView_new_?.Dispose();
            victoryView_new_ = null;

            if (playAgainView_ != null)
            {
                playAgainView_.OnButton1Clicked -= OnButton1Clicked;
                playAgainView_.OnButton2Clicked -= OnButton2Clicked;
            }
            playAgainView_?.Dispose();
            playAgainView_ = null;

            App.OnSleepEvent -= OnSleep;
            App.OnResumeEvent -= OnResume;
        }
    }
}
