using System;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading.Tasks;
using Plugin.Connectivity;
using Newtonsoft.Json;
using Plugin.GoogleAnalytics;

namespace DinoLingo
{
    public class MainPage_ViewModel : INotifyPropertyChanged
    {
        static int ERROR_PREFIX = 80;

        double SLIDER_SIZE_LEFT, SLIDER_SIZE_RIGHT;
        double SLIDER_VISIBLE_PART_LEFT, SLIDER_VISIBLE_PART_RIGHT;

        public event PropertyChangedEventHandler PropertyChanged;
        public enum CENTRAL_VIEWS {LESSONS_AND_GAMES, BOOKS, STORIES, SONGS};
        public static int MAIN_CATS_COUNT = 4;
        public static int BOOKS_SUBCATS_COUNT = 4;
        public static string[][] SUBCATS = new string[][] {
            new string[] { "0" }, // LESSONS_AND_GAMES
            new string[] { "level-1", "level-2", "level-3", "level-4", "multilevel", "fun", "dinosaur-books"}, // BOOKS
            new string[] { "0" }, // STORIES
            new string[] { "0" }, // SONGS
        };

        public static int[] subCatIndex =  {
            0, // LESSONS_AND_GAMES
            0, // BOOKS
            0, // STORIES
            0, // SONGS
        };


        //public CENTRAL_VIEWS centralView;
        //public int subViewIndex = 0;
        public FavoriteListCoords centralView;
        
        // ===  ===

        // ===  ===
        public INavigation navigation;

        MySlideView slideViewLeft, slideViewRight;
        MyListView centerListView;
        LeftMenuView leftMenuView;
        RightMenuView rightMenuView;
        RelativeLayout rootRelativeLayout;

        // 
        CategoryResponse categoryResponse;
        ShortReportResponse.ShortReport shortReport;
        bool firstLaunch = true;
        public TestViewModel testViewModel;
        bool needToSignUp;
        int id = -1;

        public MainPage_ViewModel(INavigation navigation, FavoriteListCoords centralView, int id = -1, bool needToSignUp = false)
        {
            this.navigation = navigation;
            this.centralView = centralView;
            this.needToSignUp = needToSignUp;
            App.OnSleepEvent += OnSleep;
            App.OnResumeEvent += OnResume;
            subCatIndex[(int)centralView.CentralView] = centralView.SubView;
            this.id = id;
        }

        public bool OnBackButtonPressed() {
            // check if any panel is opened
            Debug.WriteLine("MainPage_ViewModel -> OnBackButtonPressed");
            if (slideViewLeft.IsOpened)
            {
                slideViewLeft.ForceClose(200);
                return false;
            }
            else if (slideViewRight.IsOpened)
            {
                slideViewRight.ForceClose(200);
                return false;
            }
            else return true;
        }

        void OnSleep()
        {
            Debug.WriteLine("MainPage_ViewModel -- > OnSleep");
        }
        
        void OnResume()
        {
            Debug.WriteLine("MainPage_ViewModel -- > OnResume");
            try
            {
                App.Audio.SayWordFromRes("harp_3_sec.mp3");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MainPage_ViewModel -- > OnResume - > Exception in App.Audio.SayWordFromRes(harp_3_sec.mp)");
            }

            centerListView.viewModel.isBlocked = false;
        }

        public async void OnAppearing() {
            centerListView.viewModel.isBlocked = false;
            id = id;
            if (testViewModel == null) return;
            
                Debug.WriteLine("MainPage_ViewModel -- > OnAppearing");
                categoryResponse = await CacheHelper.GetAsync<CategoryResponse>(CacheHelper.CATEGORYS_RESPONSE + UserHelper.Lang_cat);
                UpdateMenuInfo();

                if (firstLaunch)
                {
                    firstLaunch = false;
                    OnResume();
                }

                GoogleAnalytics.Current.Tracker.SendView("MainPage");

                if (needToSignUp)
                {
                    needToSignUp = false;
                    Debug.WriteLine("MainPage_ViewModel -> OnAppearing -> needToSignUp -> show sign up page");
                    await App.Current.MainPage.Navigation.PushModalAsync(new SignUp_Page());
                }

            if (testViewModel == null) return;
            else {
                testViewModel.OnAppearing();
                Debug.WriteLine("MainPage_ViewModel -> OnAppearing -> App.Audio.playerBackground.Stop ?");
                try
                {
                    if (App.Audio.playerBackground.IsPlaying) App.Audio.playerBackground.Stop();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("MainPage_ViewModel -> OnAppearing -> Exception in - App.Audio.playerBackground.Stop ?");
                }
                
            }
            
            
           
        }

        public void OnDisappearing()
        {
            App.OnSleepEvent -= OnSleep;
            App.OnResumeEvent -= OnResume;
            if (App.Audio.sayWord.IsPlaying) App.Audio.sayWord.Stop();
        }

        public async void SwitchCentralView (int  btn_index, int subView = 0) {
            
            if (centralView.CentralView == categoryResponse.result[btn_index].viewType && centralView.SubView == subView) return;
            centralView = new FavoriteListCoords { CentralView = categoryResponse.result[btn_index].viewType, SubView = subView };
            Debug.WriteLine("MainPage_ViewModel --> SwitchCentralView -->" + centralView.CentralView + "/" + centralView.SubView);
            centerListView.viewModel.SwitchCenterView(centralView);
        }

		public void AddCenterView (ContentView view) {			
			double widthFactor = (UI_Sizes.ScreenWidthAspect > 1.78)? 0.017 + 0.008 + 0.026 + 0.188 : 0.008+0.017;
            double width = UI_Sizes.ScreenWidthX - UI_Sizes.ScreenHeightX * widthFactor * 2;
           
            view.Content = centerListView = new MyListView(centralView, this) { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center,
                WidthRequest =  width,
                HeightRequest = UI_Sizes.ScreenHeightX,
            };
        }


        
        public void AddSlideViews(RelativeLayout root) {
            rootRelativeLayout = root;
            double sliderSizeLeft = UI_Sizes.ScreenHeightX * (0.5+0.27+0.017+0.008+0.0268+0.188);
            double sliderVisiblePartLeft = UI_Sizes.ScreenHeightX * (0.017 + 0.008 + 0.026 + 0.188);

            double sliderSizeRight = UI_Sizes.ScreenHeightX * (0.188+0.026+0.008+0.017+0.500);
            double sliderVisiblePartRight = UI_Sizes.ScreenHeightX * (0.188 + 0.026 + 0.008 + 0.017);

            SLIDER_SIZE_LEFT = sliderSizeLeft / UI_Sizes.ScreenWidthX;
            SLIDER_VISIBLE_PART_LEFT = sliderVisiblePartLeft / sliderSizeLeft;
            SLIDER_SIZE_RIGHT = sliderSizeRight / UI_Sizes.ScreenWidthX;
            SLIDER_VISIBLE_PART_RIGHT = sliderVisiblePartRight / sliderSizeRight;

            slideViewLeft = new MySlideView(rootRelativeLayout,     SLIDER_SIZE_LEFT,    -SLIDER_SIZE_LEFT + SLIDER_SIZE_LEFT * SLIDER_VISIBLE_PART_LEFT, 0.0, this);
            slideViewRight = new MySlideView(rootRelativeLayout,    SLIDER_SIZE_RIGHT,    1 - SLIDER_SIZE_RIGHT * SLIDER_VISIBLE_PART_RIGHT, 1 - SLIDER_SIZE_RIGHT * 0.995, this);

            leftMenuView = new LeftMenuView(slideViewLeft, this);
            rightMenuView = new RightMenuView(slideViewRight, this);

            slideViewLeft.AddContent(leftMenuView);
            slideViewRight.AddContent(rightMenuView);

            slideViewLeft.AddToParent();
            slideViewRight.AddToParent();
            slideViewLeft.pairedView = slideViewRight;
            slideViewRight.pairedView = slideViewLeft;

            slideViewLeft.OnSlideOpened += OnLeftMenuOpened;
            slideViewLeft.OnSlideClosed += OnLeftMenuClosed;
            slideViewRight.OnSlideOpened += OnRightMenuOpened;
            slideViewRight.OnSlideClosed += OnRightMenuClosed;
        }

        async void UpdateMenuInfo () {

            //rightMenuView
            
            try
            {
                id = id;
                rightMenuView.SetCategoriesButtons();
                rightMenuView.SetAdditionalButtons();

                // leftMenuView
                //check if we have SHORT_REPORT
                if (!UserHelper.IsFree && UserHelper.Login != null && !string.IsNullOrEmpty(UserHelper.Login.user_id))
                {
                    if (await CacheHelper.Exists(CacheHelper.SHORT_REPORT + UserHelper.Lang_cat + UserHelper.Login.user_id))
                    {
                        ShowReportAsync(UserHelper.Lang_cat, UserHelper.Login.user_id);
                        // make lazy request for ShortReportResponse
                        ShortReportResponseAsync(UserHelper.Lang_cat, UserHelper.Login.user_id);
                    }
                    else
                    {
                        // clear all
                        leftMenuView.ClearAllStats();
                        // make lazy request for ShortReportResponse
                        ShortReportResponseAsync(UserHelper.Lang_cat, UserHelper.Login.user_id);
                    }
                }
                else
                { // user is not logged in - no dinos, starts ...
                  // clear all
                    leftMenuView.ClearAllStats();
                }
                SetFavoritesButton();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MainPage_ViewModel -> UpdateMenuInfo -> ex: " + ex.Message);
            }
            
        }



        public Task ShortReportResponseAsync(string Lang_Cat, string user_id)
        {
            Debug.WriteLine($"lasy request for ShortReportResponse ... lang_cat :{Lang_Cat}, user_id: {user_id}");
            return Task.Run(async () => {
                var postData = $"cat={Lang_Cat}";
                postData += $"&user_id={user_id}";
                //check connection
                if (!CrossConnectivity.Current.IsConnected || !DownloadHelper.DownloadHelper.CheckInternetConnectionProgressive()) {
                    /*
                    Device.BeginInvokeOnMainThread(async () => {
                        //await App.Current.MainPage.DisplayAlert(POP_UP.OOPS, POP_UP.NO_CONNECTION, POP_UP.OK);
                        //("Error!", "No internet connection! Can't update dinosaurs", "ОK");
                        // START TIMER TO RECONNECT and try again
                        
                    });
                    */

                    Debug.WriteLine("MainPage_ViewModel -> ShortReportResponseAsync -> NO_CONNECTION");
                    return;
                }

                ShortReportResponse shortReportResponse = await ServerApi.PostRequestProgressive<ShortReportResponse>(ServerApi.SHORT_REPORT_URL, postData, null);
                // wait for response
                if (shortReportResponse == null) {
                    Device.BeginInvokeOnMainThread(async () => {
                        Analytics.SendResultsRegular("MainPage_ViewModel", shortReportResponse, shortReportResponse?.error, ServerApi.SHORT_REPORT_URL, postData);
                        /*
                        await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                            POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(shortReportResponse?.error, ERROR_PREFIX + 1), POP_UP.OK);
                            */
                        //("Error!", "Error while updating report...", "ОK");
                        // START TIMER TO RECONNECT and try again
                    });

                        return;
                }

                else if (shortReportResponse.error != null) {
                    Device.BeginInvokeOnMainThread(async () => {
                        Analytics.SendResultsRegular("MainPage_ViewModel", shortReportResponse, shortReportResponse?.error, ServerApi.SHORT_REPORT_URL, postData);
                        if (AsyncMessages.CheckDisplayAlertTimeout())
                        {
                            await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                             POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(shortReportResponse?.error, ERROR_PREFIX + 2), POP_UP.OK);
                            //("Error!", "Error in ShortReportResponse: " + shortReportResponse.error.message, "ОK");
                            // START TIMER TO RECONNECT and try again
                        }

                    });
                   
                    return;
                }
                else if (shortReportResponse.result == null) {
                    Device.BeginInvokeOnMainThread(async () => {
                        Analytics.SendResultsRegular("MainPage_ViewModel", shortReportResponse, shortReportResponse?.error, ServerApi.SHORT_REPORT_URL, postData);
                        if (AsyncMessages.CheckDisplayAlertTimeout())
                        {
                            await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                            POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(shortReportResponse?.error, ERROR_PREFIX + 3), POP_UP.OK);
                            //("Error!", "Error in ShortReportResponse: null result ", "ОK");
                            // START TIMER TO RECONNECT and try again
                        }
                    });
                   
                    return;
                }
                // we have good response !
                await Task.Delay(50);
                shortReport = shortReportResponse.result;
                Debug.WriteLine("shortReport = " + JsonConvert.SerializeObject(shortReport));
                // save result
                await CacheHelper.Add<ShortReportResponse.ShortReport>(CacheHelper.SHORT_REPORT + Lang_Cat + user_id, shortReport);
                // and show result
                ShowReportAsync(Lang_Cat, user_id);
                return;
            });
        }

   

        Task ShowReportAsync(string Lang_Cat, string user_id) {
            Debug.WriteLine("ShowReportAsync()");
            return Task.Run(async ()=> {
                shortReport = await CacheHelper.GetAsync<ShortReportResponse.ShortReport>(CacheHelper.SHORT_REPORT + Lang_Cat + user_id);
                if (leftMenuView != null) Device.BeginInvokeOnMainThread(ShowReport);
                return;
            });
        }

        void ShowReport(){
            leftMenuView.ShowReport(shortReport);
        }

        public void SetFavoritesButton()
        {
            leftMenuView.SetFavoritesButton();
        }

        void OnLeftMenuOpened()
        {
            Debug.WriteLine("OnLeftMenuOpened ()");
            leftMenuView.ChangeOpenButtonImage(true);
        }

        void OnLeftMenuClosed()
        {
            Debug.WriteLine("OnLeftMenuClosed ()");
            leftMenuView.ChangeOpenButtonImage(false);
        }

        void OnRightMenuOpened()
        {
            Debug.WriteLine("OnRightMenuOpened ()");
            rightMenuView.ChangeOpenButtonImage(true);
        }

        void OnRightMenuClosed()
        {
            Debug.WriteLine("OnRightMenuClosed ()");
            rightMenuView.ChangeOpenButtonImage(false);
        }

        public async Task SASGame(THEME_NAME ThemeName, int gameIndex, ListItemStarsInfo starsInfo)
        {
            Debug.WriteLine("Starting navigation.PushModalAsync(new SASGamePage(navigation, THEME_NAME.NUMBERS))");
            //App.Current.MainPage = new NavigationPage(App.Current.MainPage);
            //Page newPage = new SASGamePage(navigation, ThemeName, gameIndex, starsInfo);
            //await App.Current.MainPage.Navigation.PushAsync (newPage);
            await navigation.PushModalAsync(new SASGamePage(navigation, ThemeName, gameIndex, starsInfo));
        }

        public async Task FindThePairGame(THEME_NAME ThemeName, ListItemStarsInfo starsInfo)
        {
            Debug.WriteLine("FindThePairGame");

            await navigation.PushModalAsync(new NumbersGame1_Page(navigation, ThemeName, starsInfo));
        }

        public async Task CarouselGame(THEME_NAME ThemeName, ListItemStarsInfo starsInfo)
        {
            Debug.WriteLine("CarouselGame");
            await navigation.PushModalAsync(new CarouselGame_Page(navigation, ThemeName, starsInfo));
        }

        public async Task QuizGame(THEME_NAME ThemeName, ListItemStarsInfo starsInfo)
        {
            Debug.WriteLine("CarouselGame");
            await navigation.PushModalAsync(new QuizGamePage(navigation, ThemeName, starsInfo));
        }

        public async Task WatchLesson(string id, ListItemStarsInfo starsInfo) {
            Debug.WriteLine("VideoPage, id = " + id);
            await navigation.PushModalAsync(new VideoPage(navigation, id, starsInfo));
        }

        public async Task ReadBook(string book_id)
        {
            Debug.WriteLine("BookPage");
            await navigation.PushModalAsync(new BookPage(navigation, book_id));
        }

        public void Dispose()
        {
            PropertyChanged = null;
            centralView = null;
            navigation = null;

            slideViewLeft.Dispose(); slideViewRight.Dispose();
            slideViewLeft = slideViewRight = null;

            centerListView.Dispose();
            centerListView = null;

            leftMenuView.Dispose();
            leftMenuView = null;

            rightMenuView.Dispose();
            rightMenuView = null;

            rootRelativeLayout = null;        
            categoryResponse = null;
            shortReport = null;        
            testViewModel = null;        
        }
    }
}
