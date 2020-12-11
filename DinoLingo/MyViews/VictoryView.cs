using FFImageLoading;
using FFImageLoading.Forms;
using Newtonsoft.Json;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DinoLingo.MyViews
{
    public class VictoryView: AbsoluteLayout
    {
        static int ERROR_PREFIX = 250;

        static float HEIGHT_RATIO = 0.9F;
        static float WIDTH_RATIO = 1.0F;
        static double TIMESPAN_RETURN_TO_PARENT_PAGE = 10.0;

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

        AbsoluteLayout dialogLayout_;
        //Image victoryImage_;
        CachedImage victoryImage_;
        MyViews.MyLabel victoryTextLabel_;
        MyViews.ButtonWithImage okBtn_;

        public Action OnClose;


        public VictoryView()
        {
            SetLayoutBounds(this, new Rectangle(0.5, 0.5, 1, 1));
            SetLayoutFlags(this, AbsoluteLayoutFlags.All);
            BackgroundColor = Color.FromRgba(0, 0, 0, 0.75);

            // add background here
            CachedImage confettiImage = new CachedImage
            {
                Source = Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.COMMON.confetti.gif"),
                Aspect = Aspect.Fill,
            };
            SetLayoutBounds(confettiImage, new Rectangle(0.5, 0.5, 1, 1));
            SetLayoutFlags(confettiImage, AbsoluteLayoutFlags.All);



            Children.Add(confettiImage);

            // create dialog layout
            AbsoluteLayout dialogLayout = new AbsoluteLayout();
            SetLayoutBounds(dialogLayout, new Rectangle(0.5, 0.5, 1, 1));
            SetLayoutFlags(dialogLayout, AbsoluteLayoutFlags.All);
            dialogLayout_ = dialogLayout;

            Frame shadowFrame = new Frame
            {
                CornerRadius = UI_Sizes.BigFrameCornerRadius,
                BorderColor = Color.Transparent,
                HasShadow = false,
                BackgroundColor = MyColors.ShadowLoginColor,
                TranslationX = UI_Sizes.BigFrameShadowTranslationX,
                TranslationY = UI_Sizes.BigFrameShadowTranslationX
            };
            SetLayoutBounds(shadowFrame, new Rectangle(0.5, 0.5, UI_Sizes.ScreenHeightX * HEIGHT_RATIO * WIDTH_RATIO, UI_Sizes.ScreenHeightX * HEIGHT_RATIO));
            SetLayoutFlags(shadowFrame, AbsoluteLayoutFlags.PositionProportional);
            dialogLayout.Children.Add (shadowFrame);

            Frame mainFrame = new Frame
            {
                CornerRadius = UI_Sizes.BigFrameCornerRadius,
                BorderColor = Color.Transparent,
                HasShadow = false,
                BackgroundColor = Color.White,
                Padding = 10
            };
            SetLayoutBounds(mainFrame, new Rectangle(0.5, 0.5, UI_Sizes.ScreenHeightX * HEIGHT_RATIO * WIDTH_RATIO, UI_Sizes.ScreenHeightX * HEIGHT_RATIO));
            SetLayoutFlags(mainFrame, AbsoluteLayoutFlags.PositionProportional);

            // add grid to main frame
            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(4, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            // add top text
            MyViews.MyLabel congratsLabel = new MyViews.MyLabel()
            {                
                Text = Translate.GetString("victory_congratulations"),
                FontSize = UI_Sizes.MediumTextSize * 1.2,
                TextColor = MyColors.YellowColor,                
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,                
            };            
            grid.Children.Add(congratsLabel, 0, 0);

            // add victory text
            MyViews.MyLabel victoryTextLabel = new MyViews.MyLabel()
            {
                Text = string.Empty,
                FontSize = UI_Sizes.MediumTextSize,
                TextColor = MyColors.BlueTextLoginColor,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
            };
            grid.Children.Add(victoryTextLabel, 0, 2);
            victoryTextLabel_ = victoryTextLabel;

            // add victory image
            CachedImage victoryImage = new CachedImage
            {
                Source = Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.win_cup.png"),
                Aspect = Aspect.AspectFit,
            };
            grid.Children.Add(victoryImage, 0, 1);
            victoryImage_ = victoryImage;

            // add button
            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (sender, e) =>
            {
                View view = (View)sender;
                OnButtonTapped(view);
            };

            // ok button
            MyViews.ButtonWithImage okBtn = new MyViews.ButtonWithImage(
                "OkBtn",
                new Rectangle(0.5, 0.5, 0.75, 1.0),
                "DinoLingo.Resources.UI.btnblue_x2.png",
                Translate.GetString("victory_great"),
                UI_Sizes.SmallTextSize,
                MyColors.ButtonBlueTextColor,
                tapGestureRecognizer
                );
            grid.Children.Add(okBtn, 0, 3);
            okBtn_ = okBtn;



            dialogLayout.Children.Add(mainFrame);
            mainFrame.Content = grid;

            Children.Add (dialogLayout);

            IsEnabled = IsVisible = false;
            InputTransparent = true;
            dialogLayout.TranslationY = -UI_Sizes.ScreenHeightX;

            

        }

        async void OnButtonTapped(View view)
        {
            if (IsAnimating) return;
            IsAnimating = true;
            await AnimateView(view, 250);

            if (view.ClassId == "OkBtn")
            {
                OnClose?.Invoke();
                OnClose = null;
                await App.Current.MainPage.Navigation.PopModalAsync();
            }
        }

        public Task AnimateView(View view, uint time)
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
                    Debug.WriteLine("VictoryView -> AnimateView -> ex:" + ex.Message);
                }
                return;
            });
        }

        async void AnimateVictory()
        {            
            await Task.Delay(200);
            
            await dialogLayout_.TranslateTo(0, 0, 700);
            IsEnabled = true;

            Debug.WriteLine("VictoryView -> IsEnabled = true");
        }

        public async void EndOfGameVictory(ListItemStarsInfo starsInfo, PostResponse.Post post, ActivityReport report)
        {
            App.Audio.playerVictory.Play();

            // set-up content
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
                    stars = starsInfo.data[starsInfo.listViewItem.Index].Stars;
                }
                else
                {
                    stars = starsInfo.data[(starsInfo.listViewItem as DoubleItem).Index2].Stars;
                }
            }

            // check if not registered
            if (UserHelper.IsFree || string.IsNullOrEmpty(UserHelper.Login.user_id)) // free or unregistered user
            {
                NoConnectionSetUp();
            }
            else if (stars < 5)
            {
                victoryTextLabel_.Text = Translate.GetString("victory_you_earned_star");
                victoryImage_.Source = Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.win_big_star.png");
                okBtn_.Text = Translate.GetString("victory_great");
            }
            else
            {
                NoConnectionSetUp();
            }


            IsVisible = true;
            InputTransparent = false;

            // change stars here !
            Debug.WriteLine("EndOfGameVictory --> post == null ?: " + (post == null));

            // get saved data_list
            ListViewItemData[] savedData = await CacheHelper.GetAsync<ListViewItemData[]>(starsInfo.POST_LIST_ITEM_DATA_key);
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
            if (starsInfo.favorites_Page == null) index = starsInfo.isSingleItem ? starsInfo.listViewItem.Index : (starsInfo.listViewItem as DoubleItem).Index2;
            stars = savedData[savedDataIndex].Stars;

            if (stars < 0) stars = 0;
            Debug.WriteLine("stars old = " + stars);
            // check connection & report status 
            bool reportToServerOK = true;

            // check if not registered
            if (UserHelper.IsFree || string.IsNullOrEmpty(UserHelper.Login.user_id)) // free or unregistered user
            {
                // JUST DO NOTHING

            }
            else if (!report.IsCompleted && CrossConnectivity.Current.IsConnected)
            { // if we connected
                // just try to wait
                await Task.Delay(500);
                if (!report.IsCompleted && CrossConnectivity.Current.IsConnected) await Task.Delay(500);
                if (!report.IsCompleted && CrossConnectivity.Current.IsConnected) await Task.Delay(500);
                if (!report.IsCompleted && CrossConnectivity.Current.IsConnected) await Task.Delay(500);
                

                if (!report.IsCompleted)
                {
                    reportToServerOK = false;
                    /*
                    await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                        POP_UP.NO_CONNECTION + POP_UP.GetCode(null, ERROR_PREFIX + 1), POP_UP.OK);
                        */
                    //("Error!", "Can't send victory data to the server", "ОK");
                    NoConnectionSetUp();
                }
            }
            else if (!report.IsCompleted && !CrossConnectivity.Current.IsConnected)
            {
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
                    Debug.WriteLine("my views -> check a dino for win...");
                    if (!string.IsNullOrEmpty(post.after_finish) && post.after_finish != "star")
                    {
                        Debug.WriteLine("You've earned a new dinosaur!");
                        victoryImage_.Source = post.after_finish;
                        victoryTextLabel_.Text = Translate.GetString("victory_you_earned_dinosaur");
                        dinoEarned = true;

                        if (starsInfo.favorites_Page == null)
                        {
                            if (starsInfo.isSingleItem)
                            {
                                starsInfo.data[index].ImgResource = post.after_finish;
                                starsInfo.listViewItem.Imagesource = post.after_finish;

                            }
                            else
                            {
                                starsInfo.data[index].ImgResource = post.after_finish;
                                (starsInfo.listViewItem as DoubleItem).Imagesource2 = post.after_finish;
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
                        starsInfo.listViewItem.StarsImagesource = TestViewModel.stars_imgs[stars];
                        Debug.WriteLine("STARS ADDED to item 1: " + starsInfo.listViewItem.Name);
                    }
                    else
                    {
                        (starsInfo.listViewItem as DoubleItem).StarsImagesource2 = TestViewModel.stars_imgs[stars];
                        Debug.WriteLine("STARS ADDED to item 2: " + (starsInfo.listViewItem as DoubleItem).Name2);
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

        bool ReturnToParentPage()
        {
            if (IsAnimating) return false;

            IsAnimating = true;
            try
            {
                
               OnClose?.Invoke();
               OnClose = null;
               App.Current.MainPage.Navigation.PopModalAsync();
                
            }
            catch
            {

            }

            return false;
        }

        void NoConnectionSetUp()
        {
            victoryImage_.Source = Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.win_cup.png"); ;
            victoryTextLabel_.Text = Translate.GetString("victory_you_won");            
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
                    int index = starsInfo.isSingleItem ? starsInfo.listViewItem.Index : (starsInfo.listViewItem as DoubleItem).Index2;
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
                            Device.BeginInvokeOnMainThread(async () => {
                                if (AsyncMessages.CheckDisplayAlertTimeout())
                                {
                                    await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                                    POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(postResponse?.error, ERROR_PREFIX + 2), POP_UP.OK);
                                    //("Error!", postResponse.error.message, "ОK");
                                }

                                await App.Current.MainPage.Navigation.PopModalAsync();
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
                                
                                return post;
                            }
                        }
                    }
                    else
                    {
                        Analytics.SendResultsRegular("VictoryView", postResponse, postResponse?.error, ServerApi.POST_URL, postData);                        
                        return post;
                    }
                }
                else
                {
                    
                    return post;
                }
            });
        }

        public void Dispose()
        {
            BindingContext = null;
            dialogLayout_ = null;
            victoryImage_ = null;
            victoryTextLabel_ = null;
            animLock = null;
        }
    }
}
