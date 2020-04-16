using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using CocosSharp;
using DinoLingo.Games;
using Plugin.Connectivity;
using Xamarin.Forms;

namespace DinoLingo
{
    public partial class NumbersGame1_Page : ContentPage
    {
        NumbersGame1_ViewModel viewModel;        

        INavigation navigation;
        THEME_NAME theme;

        Object cardsLock = new Object();
        int firstCard, secondCard, closing1, closing2;
        bool forceClosing = false;

        bool[] isOpened;
        bool stopAllAnimation;
        List<string> allCards, randomCards, baseCards;
        int pairsOpened;
        Image[] small_dinos;

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
        

        VictoryView victoryView;
        string language = "english";
        bool firstStart = true;
        ListItemStarsInfo starsInfo;

        // cocos game
        GameScene_MemoryGame gameScene;
        Color yellowColor = (Color)Application.Current.Resources["YellowColor"];
        CocosSharpView gameView;

        public NumbersGame1_Page(INavigation navigation, THEME_NAME theme, ListItemStarsInfo starsInfo)
        {
            this.navigation = navigation;
            this.theme = theme;
            InitializeComponent();
            loadingView.BackgroundColor = LoadingView_Logic.GetRandomColor();
            loadingView.Padding = UI_Sizes.MainMargin;
            this.starsInfo = starsInfo;

            BindingContext = viewModel = new NumbersGame1_ViewModel(navigation, loadingView, theme, starsInfo);
            viewModel.OnLoadingEndedSuccess += OnLoadingEndedSuccess;

            int index = starsInfo.isSingleItem ? starsInfo.listViewItem.Index : (starsInfo.listViewItem as DoubleItem).Index2;
            viewModel.activityReport = new ActivityReport(ActivityReport.ACT_TYPE.GAME, starsInfo.data[index].id);

            AddCocosGameView(yellowSquare);
        }

        void OnLoadingEndedSuccess ()
        {
            while (gameScene == null)
            {
                Task.Delay(100).Wait();
            }
            OnStart();
            viewModel?.AnimateLoadingView(1000);
        }

        void AddCocosGameView(AbsoluteLayout yellowSquare)
        {
            // This hosts our game view.
            gameView = new CocosSharpView()
            {
                // Notice it has the same properties as other XamarinForms Views
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                // This gets called after CocosSharp starts up:
                ViewCreated = HandleViewCreated,
                BackgroundColor = yellowColor,
                
            };

            AbsoluteLayout.SetLayoutBounds(gameView, new Rectangle(0.5, 0.5, 0.98, 0.98));
            AbsoluteLayout.SetLayoutFlags(gameView, AbsoluteLayoutFlags.All);
            yellowSquare.Children.Add(gameView);

           
        }

        void DoOnOpenCard(int index)
        {
            try
            {
                App.Audio.SayWord(allCards[index], language);
            }
            catch (Exception ex)
            {

            }           
        }

        void HandleViewCreated(object sender, EventArgs e)
        {
            var gameView = sender as CCGameView;
            if (gameView != null)
            {
                // This sets the game "world" resolution to 100x100:
                gameView.DesignResolution = new CCSizeI(100, 100);
                // GameScene is the root of the CocosSharp rendering hierarchy:
                gameScene = new GameScene_MemoryGame(gameView);
                gameScene.memoryGameModel.DoOnMatchesThePair = DoOnMatchesThePair;
                // gameScene.memoryGameModel.DoOnMissedThePair =
                gameScene.memoryGameModel.DoOnOpenCard = DoOnOpenCard;

                // Starts CocosSharp:
                gameView.RunWithScene(gameScene);
            }
        }

        void DoOnMatchesThePair()
        {
            viewModel.SoundRight();
            viewModel.activityReport.ReportProgress(gameScene.memoryGameModel.pairsDone / 8.0 * 100.0);

            Device.BeginInvokeOnMainThread(() => {
                try
                {
                    small_dinos[gameScene.memoryGameModel.pairsDone - 1].IsVisible = true;
                    viewModel.ScoreUp();
                }
                catch (Exception)
                {

                }
            });
            
            
            if (gameScene.memoryGameModel.pairsDone >= 8)
            { // VICTORY
                viewModel.PauseTimer();
                App.Audio.playerBackground.Stop();
                Device.BeginInvokeOnMainThread(() => {
                    victoryView.EndOfGameVictory(starsInfo, viewModel.post, viewModel.activityReport);
                });
                
            }
        }
        


        async void OnStart()
        {
            Debug.WriteLine("NumbersGame_Page -> OnStart");
            language = (await CacheHelper.GetAsync(CacheHelper.CURRENT_LANGUAGE)).Data;
            HideSmallDinos();
            await InitGameModel();           
           
            for (int i = 0; i < 16; i++)
            {                
                allCards.Add(randomCards[i]);                
            }

            
            gameScene.Restart(theme, randomCards);           

            Debug.WriteLine("NumbersGame_Page -> OnStart - OK");
        }


        void HideSmallDinos()
        {
            foreach (View v in small_dinos)
            {
                v.IsVisible = false;
            }
        }

        async Task InitGameModel()
        {
            Debug.WriteLine("NumbersGame_Page -> InitGameModel");
            isOpened = new bool[16];
            allCards = new List<string>();
            await SetBaseCards();
            SetRandomCards();
            closing1 = closing2 = firstCard = secondCard = -1;
            pairsOpened = 0;
            stopAllAnimation = false;
            Debug.WriteLine("NumbersGame_Page -> InitGameModel - END");
        }

        void SetRandomCards()
        {
            Debug.WriteLine("NumbersGame_Page -> SetRandomCards");
            
            randomCards = new List<string>();
            Random random = new Random();
            for (int i = 0; i < 8; i++)
            {
                int randomIndex = random.Next(0, baseCards.Count);
                string value = baseCards[randomIndex];
                baseCards.RemoveAt(randomIndex);
                randomCards.Insert(random.Next(0, randomCards.Count), value);
                randomCards.Insert(random.Next(0, randomCards.Count), value);
            }
            Debug.WriteLine("NumbersGame_Page -> SetRandomCards - END");
        }

        async Task SetBaseCards()
        {
            Debug.WriteLine("NumbersGame_Page -> SetBaseCards");
            List<string> tmp = new List<string>(Theme.Resources[(int)theme].Item.Keys);
            List<string> source = new List<string>(Theme.Resources[(int)theme].Item.Keys);
           
            // check if audiofile exists
            Debug.WriteLine("NumbersGame_Page -> SetBaseCards");
            for (int i = 0; i < tmp.Count; i ++)
            {
                string key = tmp[i];
                if (!(await PCLHelper.IsFileExistAsync(key + "_" + language + ".mp3")))
                {                    
                    source.Remove(key);
                    Debug.WriteLine("removed from list key: " + key);
                }
                else
                {
                    int j = 0;
                    j++;
                }
            }
            baseCards = new List<string>(source);
            Random random = new Random();
            while (baseCards.Count < 8)
            { // not enough items for game
                int randomIndex = random.Next(0, source.Count);
                baseCards.Add(source[randomIndex]);
                source.RemoveAt(randomIndex);
            }
            Debug.WriteLine("NumbersGame_Page -> SetBaseCards - END");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            viewModel.isVisible = true;
            if (firstStart)
            {                
                Device.StartTimer(TimeSpan.FromMilliseconds(500), FastStart);
                LoadingView_Logic.ShowCloseBtnTimer(LoadingCloseBtn);
            }
            
        }

        bool FastStart()
        {

            firstStart = false;
            small_dinos = new Image[] { small_dino_01, small_dino_02, small_dino_03, small_dino_04, small_dino_05, small_dino_06, small_dino_07, small_dino_08 };
                        

            IsAnimating = false; 

            //add vicrory view
            
            victoryView = new VictoryView(navigation, OnClose, starsInfo);
            viewModel.victoryView = victoryView;


            AbsoluteLayout.SetLayoutBounds(victoryView, new Rectangle(0, 0, 1, 1));
            AbsoluteLayout.SetLayoutFlags(victoryView, AbsoluteLayoutFlags.All);
            mainRootLayout.Children.Add(victoryView);

            // set victory view
            victoryView.SetSizesAndStartY(mainRootLayout.Height, 0.621, 0.05, -App.Current.MainPage.Height);            
            
            viewModel.StartLoading();            
            return false;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            gameView.IsVisible = false;
            Debug.WriteLine("OnDisappearing()");
            viewModel.OnClose();
            viewModel.activityReport.OnDisappearing();           
        }


        void OnClose()
        {
            Debug.WriteLine("viewModel.OnClose();");
            viewModel.OnClose();
        }

        public async void MenuButton_Tapped(object sender, System.EventArgs e)
        {
            if (IsAnimating) return;

            View view = sender as View;
            Debug.WriteLine("view.ClassId = " + view.ClassId);
            if (view.ClassId == "CloseBtn")
            {

                this.OnClose();
                await AnimateImage(view, 250);
                await navigation.PopModalAsync();
            }
            else if (view.ClassId == "RestartBtn")
            {
                await AnimateImage(view, 250);
                //restart here
                Debug.WriteLine("Restart");

                OnStart();
                viewModel.Restart();
            }
        }

        public Task AnimateImage(View view, uint time)
        {
            IsAnimating = true;
            return Task.Run(async () =>
            {
                await view.ScaleTo(0.8, time / 2);
                await view.ScaleTo(1.0, time / 2);
                IsAnimating = false;
                return;
            });
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();
            if (Parent == null)
            {
                viewModel.OnLoadingEndedSuccess -= OnLoadingEndedSuccess;
                viewModel.Dispose();
                Dispose();
                GC.Collect();
                MemoryLeak.TrackMemory();
            }
        }   
        
        void Dispose()
        {
            Debug.WriteLine("NumbersGame_Page -> Dispose");
            
            BindingContext = null;
            Content = null;

            viewModel = null;


            navigation = null;  
            cardsLock = null;
            isOpened = null;
           
            allCards = randomCards = baseCards = null; // list            
            small_dinos = null; // arr

            victoryView.Dispose();
            victoryView = null;  
            
            starsInfo = null;

            gameScene = null;
            gameView = null;
        }
    }
}
