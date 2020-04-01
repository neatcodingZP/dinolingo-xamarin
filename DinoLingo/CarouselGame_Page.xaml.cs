using CocosSharp;
using DinoLingo.Games;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DinoLingo
{
    public partial class CarouselGame_Page : ContentPage
    {
        public CarouselGame_ViewModel viewModel;
        //bool firstStart = true;

        // cocos game
        GameScene_CarouselGame gameScene;        
        CocosSharpView gameView;

        public CarouselGame_Page()
        {
            InitializeComponent();
        }

        public CarouselGame_Page(INavigation navigation, THEME_NAME themeName, ListItemStarsInfo starsInfo): this() {
            loadingView.BackgroundColor = LoadingView_Logic.GetRandomColor();
            loadingView.Padding = UI_Sizes.MainMargin;

            viewModel = new CarouselGame_ViewModel(navigation, themeName, loadingView, starsInfo);
            BindingContext = viewModel;

            viewModel.AddShotsImages(shotsGrid);

            viewModel.SetUpSizes();
            AddCocosGameView(rootLayout);

            viewModel.AddVictoryView(absoluteRoot);
            viewModel.AddGameOverView(absoluteRoot);
            LoadingView_Logic.ShowCloseBtnTimer(LoadingCloseBtn);
        }

        void AddCocosGameView(ContentView view)
        {
            // This hosts our game view.

            gameView = new CocosSharpView()
            {
                // Notice it has the same properties as other XamarinForms Views
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = (int)(viewModel.GameWidth - 0.05f * viewModel.GameWidth),
                HeightRequest = (int)(viewModel.GameHeight - 0.05f * viewModel.GameWidth),
                // This gets called after CocosSharp starts up:
                ViewCreated = HandleViewCreated, 
            };

            view.Content = gameView;
        }



        void HandleViewCreated(object sender, EventArgs e)
        {
            var gameView = sender as CCGameView;
            if (gameView != null)
            {
                // This sets the game "world" resolution to 100x100:
                gameView.DesignResolution = new CCSizeI((int)(viewModel.GameWidth - 0.05f * viewModel.GameWidth), (int)(viewModel.GameHeight - 0.05f * viewModel.GameWidth));
                // GameScene is the root of the CocosSharp rendering hierarchy:
                gameScene = new GameScene_CarouselGame(gameView);

                gameScene.gameModel.DoOnMatchesThePair = DoOnMatchesThePair;
                gameScene.gameModel.DoOnMissedThePair = DoOnMissedThePair;
                //gameScene.memoryGameModel.DoOnOpenCard = DoOnOpenCard;

                // Starts CocosSharp:
                gameView.RunWithScene(gameScene);
            }
            viewModel.gameScene = gameScene;
        }

        void DoOnMatchesThePair()
        {
            viewModel.DoOnMatchesThePair();
        }

        void DoOnMissedThePair()
        {
            viewModel.DoOnMissedThePair();
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();            
            viewModel.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            viewModel.OnClose();
            Debug.WriteLine("Carousel page --> OnDisappearing");
        }

        public void MenuButton_Tapped(object sender, System.EventArgs e)
        {
            viewModel.MenuButton_Tapped(sender, e);
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();
            if (Parent == null)
            {
                viewModel.Dispose();
                Dispose();
                GC.Collect();
                MemoryLeak.TrackMemory();
            }
        }

        void Dispose()
        {
            Debug.WriteLine("CarouselGame_Page -> Dispose");

            Content = null;
            BindingContext = null;
            viewModel = null;            
        }
    }
}
