using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Diagnostics;
using FFImageLoading;
using Plugin.Connectivity;
using Newtonsoft.Json;

namespace DinoLingo
{
    public partial class VictoryView : ContentView
    {
        static int ERROR_PREFIX = 150;

        public double VictoryDialogHeight { get; set; }
        public double VictoryDialogWidth { get; set; }
        public double TopTextSize { get; set; }
        public double VictoryFontSize {get; set;}
        public double BtnTextSize { get; set; }
        public double CornerRadius { get; set; }

        public string VictoryText { get; set; }
        public string BtnText { get; set; }
        public ImageSource VictoryImage {get; set;}
        public bool isAnimating = false;
        bool IsClosedByUser = false;
        static double TIMESPAN_RETURN_TO_PARENT_PAGE = 10.0;

		INavigation navigation;
        public Action OnClose;

        public VictoryView(INavigation navigation, Action onClose, ListItemStarsInfo starsInfo = null)
        {
            Debug.WriteLine("VictoryView -> ");
            Debug.WriteLine("VictoryView -> starsInfo == null?:" + (starsInfo == null));
            InitializeComponent();
			BindingContext = this;
			this.navigation = navigation;
            this.BackgroundColor = Color.FromRgba(0, 0, 0, 0.75);
            int stars;
            //============================================================
            if (starsInfo.favorites_Page != null)
            {
                FavoriteListCoords favoriteListCoords = starsInfo.favoriteListCoords;

                stars = Favorites.allListsData[(int)favoriteListCoords.CentralView][favoriteListCoords.SubView][favoriteListCoords.index].Stars;
            }
            else
            {
                if (starsInfo.isSingleItem)
                {
                    stars = starsInfo.data[starsInfo.eventArgs.MyItem.Index].Stars;
                }
                else
                {
                    stars = starsInfo.data[(starsInfo.eventArgs.MyItem as DoubleItem).Index2].Stars;
                }
            }

            // check if not registered
            if (UserHelper.IsFree || string.IsNullOrEmpty(UserHelper.Login.user_id)) // free or unregistered user
            {
                NoConnectionSetUp();
            }
            else if (stars < 5) {
                VictoryText = Translate.GetString("victory_you_earned_star");
                VictoryImage = Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.win_big_star.png");
                BtnText = Translate.GetString("victory_great");
            }
            else {
                NoConnectionSetUp();
            }

            OnClose = onClose;
            Debug.WriteLine("VictoryView -> created OK, stars = " + stars);
        }

        public void Dispose()
        {
            Content = null;
            BindingContext = null;
            VictoryImage = null;
            navigation = null;
            OnClose = null;
        }

        void NoConnectionSetUp () {
            VictoryImage = Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.win_cup.png");;
            VictoryText = Translate.GetString("victory_you_won"); 
            BtnText = Translate.GetString("victory_great");
        }

		public async void Button_Tapped(object sender, System.EventArgs e)
        {
            if (isAnimating) return;
            isAnimating = true;
            View view = sender as View;
            if (view.ClassId == "OkBtn") {
                IsClosedByUser = true;
                OnClose();
                await AnimateImage(view, 250);
                await navigation.PopModalAsync();
            }
            else if (view.ClassId == "Background") {
                IsClosedByUser = true;
                OnClose();
                await navigation.PopModalAsync();
            }
            isAnimating = false;
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
                catch
                {

                }                             
                return;
            });
        }

		public void SetStartPositionForDialog (double translationY) {
			victoryDialog.TranslationY = translationY;
		}

		async void AnimateVictory() {            
                IsVisible = true;
                InputTransparent = false;
                await Task.Delay(500);
                victoryDialog.IsVisible = true;
                await victoryDialog.TranslateTo(0, 0, 700);
                IsEnabled = true;
                Debug.WriteLine ("VictoryView -> IsEnabled = true");                      
		}

        public void SetSizesAndStartY (double height, double widtFactor, double fontFactor, double offsetY) {
            VictoryDialogHeight = height;
            CornerRadius = height * 0.15;
            VictoryDialogWidth = height * 1.2; //height * widtFactor;
            TopTextSize = UI_Sizes.MediumTextSize * 1.2;
            VictoryFontSize = UI_Sizes.MediumTextSize;
            BtnTextSize = UI_Sizes.SmallTextSize;

            SetStartPositionForDialog(offsetY);
        }

        public async void EndOfGameVictory(ListItemStarsInfo starsInfo, PostResponse.Post post, ActivityReport report)
        {
            App.Audio.playerVictory.Play();
            // change stars here !
            Debug.WriteLine("EndOfGameVictory --> post == null ?: " + (post == null));

            // get saved data_list
            ListViewItemData[] savedData =  await CacheHelper.GetAsync<ListViewItemData[]>(starsInfo.POST_LIST_ITEM_DATA_key);
            // get saved data index
            int savedDataIndex = -1;

            for (int i = 0; i < savedData.Length; i++)
            {
                if (savedData[i].id == starsInfo.id)
                {
                    savedDataIndex = i;
                    break;
                }
            }
            Debug.WriteLine("EndOfGameVictory --> savedDataIndex =  " + savedDataIndex);
            int index = -1;
            if (starsInfo.favorites_Page == null) index = starsInfo.isSingleItem ? starsInfo.eventArgs.MyItem.Index : (starsInfo.eventArgs.MyItem as DoubleItem).Index2;
            int stars = savedData[savedDataIndex].Stars;

            if (stars < 0) stars = 0;
            Debug.WriteLine("stars old = " + stars);
            // check connection & report status 
            bool reportToServerOK = true;
            
            // check if not registered
            if (UserHelper.IsFree || string.IsNullOrEmpty(UserHelper.Login.user_id)) // free or unregistered user
            {
                // JUST DO NOTHING

            }
            else if (!report.IsCompleted && CrossConnectivity.Current.IsConnected) { // if we connected
                // just try to wait
                await Task.Delay(500);
                if (!report.IsCompleted && CrossConnectivity.Current.IsConnected) await Task.Delay(500);
                if (!report.IsCompleted && CrossConnectivity.Current.IsConnected) await Task.Delay(500);
                if (!report.IsCompleted && CrossConnectivity.Current.IsConnected) await Task.Delay(500);
                if (!report.IsCompleted && CrossConnectivity.Current.IsConnected) await Task.Delay(500);
                if (!report.IsCompleted && CrossConnectivity.Current.IsConnected) await Task.Delay(500);

                if (!report.IsCompleted) {
                    reportToServerOK = false;
                    /*
                    await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                        POP_UP.NO_CONNECTION + POP_UP.GetCode(null, ERROR_PREFIX + 1), POP_UP.OK);
                        */
                   //("Error!", "Can't send victory data to the server", "ОK");
                    NoConnectionSetUp();
                }
            }
            else if (!report.IsCompleted && !CrossConnectivity.Current.IsConnected) {
                Debug.WriteLine("victory -> no connection -> regular victory");
                reportToServerOK = false;
                NoConnectionSetUp();
            }

            Debug.WriteLine("report.IsCompleted = " + report.IsCompleted);
            if (UserHelper.IsFree || string.IsNullOrEmpty(UserHelper.Login.user_id)) // free or unregistered user
            {
                // JUST DO NOTHING

            }
            else if (stars < 5 && report.IsCompleted && reportToServerOK)
            {

                // update victory dino 
                bool dinoEarned = false;
                if (stars == 0)
                {
                    Debug.WriteLine("check a dino for win...");
                    if (!string.IsNullOrEmpty(post.after_finish) && post.after_finish != "star")
                    {
                        Debug.WriteLine("You've earned a new dinosaur!");
                        VictoryImage = post.after_finish;
                        VictoryText = Translate.GetString("victory_you_earned_dinosaur");
                        dinoEarned = true;

                        if (starsInfo.favorites_Page == null) {
                            if (starsInfo.isSingleItem)
                            {
                                starsInfo.data[index].ImgResource = post.after_finish;
                                starsInfo.eventArgs.MyItem.Imagesource = post.after_finish;

                            }
                            else
                            {
                                starsInfo.data[index].ImgResource = post.after_finish;
                                (starsInfo.eventArgs.MyItem as DoubleItem).Imagesource2 = post.after_finish;
                            }
                        }
                        savedData[savedDataIndex].ImgResource = post.after_finish;
                        
                    }
                }
                Debug.WriteLine("victoryView -> EndOfGameVictory -> update stars");
                stars++;
                if (starsInfo.favorites_Page == null)
                {
                    if (starsInfo.isSingleItem)
                    {
                        starsInfo.eventArgs.MyItem.StarsImagesource = TestViewModel.stars_imgs[stars];
                        Debug.WriteLine("STARS ADDED to item 1: " + starsInfo.eventArgs.MyItem.Name);
                    }
                    else
                    {
                        (starsInfo.eventArgs.MyItem as DoubleItem).StarsImagesource2 = TestViewModel.stars_imgs[stars];
                        Debug.WriteLine("STARS ADDED to item 2: " + (starsInfo.eventArgs.MyItem as DoubleItem).Name2);
                    }
                }
                Debug.WriteLine("victoryView -> EndOfGameVictory -> stars = " + stars);
                // cache starsInfo.data 
                if (starsInfo.favorites_Page == null) starsInfo.data[index].Stars = stars;

                savedData[savedDataIndex].Stars = stars;

                //WRONG - need to cache raw post_list_item_data
                Debug.WriteLine("victoryView -> EndOfGameVictory -> cashe saveddata ");
                await CacheHelper.Add<ListViewItemData[]>(starsInfo.POST_LIST_ITEM_DATA_key, savedData);

                Debug.WriteLine($"victoryView -> EndOfGameVictory -> Favorites.allListsData: starsInfo.favoriteListCoords.CentralView = {starsInfo.favoriteListCoords.CentralView}, starsInfo.favoriteListCoords.SubView ");
                // and now we can update favorites data
                Favorites.allListsData[(int)starsInfo.favoriteListCoords.CentralView][starsInfo.favoriteListCoords.SubView] = savedData;

                Debug.WriteLine("victoryView -> EndOfGameVictory -> starsInfo.favorites_Page?.UpdateFavoritesList() ");
                starsInfo.favorites_Page?.UpdateFavoritesList();
                

                // if dinoearned - update short report & left menu
                if (CrossConnectivity.Current.IsConnected)
                {
                    if (dinoEarned) starsInfo.rootVM.ShortReportResponseAsync(UserHelper.Lang_cat, UserHelper.Login.user_id);
                }
            }

            AnimateVictory();

            Device.StartTimer(TimeSpan.FromSeconds(TIMESPAN_RETURN_TO_PARENT_PAGE), ReturnToParentPage);
        }

        bool ReturnToParentPage () {
            isAnimating = true;
            try
            {
                if (!IsClosedByUser)
                {
                    OnClose();
                    navigation.PopModalAsync();
                };
            }
            catch
            {

            }

            return false;
        }

        

        public Task<PostResponse.Post> DownloadPost(ListItemStarsInfo starsInfo)
        {
            return Task.Run(async () => {
                // check if need to download anything...
                PostResponse.Post post = null;
            //============================================================            
            int stars = 0;
            if (starsInfo.favorites_Page != null)
                {
                    FavoriteListCoords favoriteListCoords = starsInfo.favoriteListCoords;
                    stars = Favorites.allListsData[(int)favoriteListCoords.CentralView][favoriteListCoords.SubView][favoriteListCoords.index].Stars;
                }
            else
                {
                    int index = starsInfo.isSingleItem ? starsInfo.eventArgs.MyItem.Index : (starsInfo.eventArgs.MyItem as DoubleItem).Index2;
                    stars = starsInfo.data[index].Stars;
                }    //if (stars > 0) return true;

                // check if we already have the post
                PostResponse postResponse;
                if (await CacheHelper.Exists(CacheHelper.POST + starsInfo.id))
                {
                    post = await CacheHelper.GetAsync<PostResponse.Post>(CacheHelper.POST + starsInfo.id);
                    Debug.WriteLine("we have cached post, id = " + starsInfo.id + ", post == null? : " + (post == null));
                    // preload image
                    if (!string.IsNullOrEmpty(post.after_finish)) Debug.WriteLine("AFTER_FINISH_FROM CACHE = " + post.after_finish);
                    if (!string.IsNullOrEmpty(post.after_finish) && post.after_finish != "star") ImageService.Instance.LoadUrl(post.after_finish).Preload();
                    return post;
                }
                else if (CrossConnectivity.Current.IsConnected && DownloadHelper.DownloadHelper.CheckInternetConnectionProgressive())
                { // if we don't have it - try to download
                    string userId = UserHelper.Login.user_id;
                    var postData = $"id={starsInfo.id}";
                    postData += $"&user_id={userId}";
                    postData += $"&type={""}";
                    Debug.WriteLine("postData = " + postData);
                    postResponse = await ServerApi.PostRequestProgressive<PostResponse>(ServerApi.POST_URL, postData, null);
                    Debug.WriteLine("post = " + JsonConvert.SerializeObject(postResponse));
                    if (postResponse != null)
                    {
                        if (postResponse.error != null)
                        {
                            Analytics.SendResultsRegular("VictoryView", postResponse, postResponse?.error, ServerApi.POST_URL, postData);
                            Device.BeginInvokeOnMainThread(async()=> {
                                if (AsyncMessages.CheckDisplayAlertTimeout())
                                {
                                    await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                                    POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(postResponse?.error, ERROR_PREFIX + 2), POP_UP.OK);
                                    //("Error!", postResponse.error.message, "ОK");
                                }

                                await navigation.PopModalAsync();
                            });
                            
                            return post;
                        }
                        else
                        {
                            Debug.WriteLine("Process response...");
                            if (postResponse.result != null)
                            {
                                post = postResponse.result;
                                Debug.WriteLine("got result from server, post == null? : " + (post == null));
                                if (!string.IsNullOrEmpty(post.after_finish)) Debug.WriteLine("AFTER_FINISH_FROM WEB = " + post.after_finish);
                                // and cache it
                                await CacheHelper.Add<PostResponse.Post>(CacheHelper.POST + starsInfo.id, post);
                                Debug.WriteLine($"POST id = {starsInfo.id} successfully cached ?: {await CacheHelper.Exists(CacheHelper.POST + starsInfo.id)}");
                                // preload image
                                if (!string.IsNullOrEmpty(post.after_finish) && post.after_finish != "star") ImageService.Instance.LoadUrl(post.after_finish).Preload();
                                return post;
                            }
                            else
                            {
                                Analytics.SendResultsRegular("VictoryView", postResponse, postResponse?.error, ServerApi.POST_URL, postData);
                                /*
                                Device.BeginInvokeOnMainThread(async () =>
                                {
                                    await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                                    POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(postResponse?.error, ERROR_PREFIX + 3), POP_UP.OK);
                                    //("Error!", "Response from server is NULL", "ОK");
                                    //await navigation.PopModalAsync();
                                });
                                */
                                return post;
                            }
                        }
                    }
                    else 
                    {
                        Analytics.SendResultsRegular("VictoryView", postResponse, postResponse?.error, ServerApi.POST_URL, postData);
                        /*
                        Device.BeginInvokeOnMainThread(async () => {
                            await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                            POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(postResponse?.error, ERROR_PREFIX + 4), POP_UP.OK);
                            //("Error!", "No response from server", "ОK");
                            //await navigation.PopModalAsync();
                        });
                        */
                        return post;
                    }
                }
                else 
                {
                    /*
                    Device.BeginInvokeOnMainThread(async () => {
                        await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                            POP_UP.NO_CONNECTION + POP_UP.GetCode(null, ERROR_PREFIX + 5), POP_UP.OK);
                        //("Error!", "No internet connection", "ОK");
                        //await navigation.PopModalAsync();
                    });
                    */
                    return post;
                }
            });
        }
    }
}
