using FFImageLoading.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DinoLingo
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Favorites_Page : ContentPage
	{

        bool IsAnimating = false;        

        TapGestureRecognizer gestureRecognizerHeart, gestureRecognizerItem;
        double itemHeight, itemWidth;
        Dictionary<string, FavoriteListCoords> videosFavoritesId, booksFavoritesId;
        MainPage_ViewModel rootViewModel;

        Color LABEL_COLOR = (Color)Application.Current.Resources["BlueTextLoginColor"];
        bool postLaunched = false;
        Label[] videosLoading, booksLoading;
        string LOADING, favorites, favorite_books;
        bool needToUpdateState = true;

        public Favorites_Page (MainPage_ViewModel rootViewModel)
		{
			InitializeComponent ();
            BindingContext = this;
            this.rootViewModel = rootViewModel;

            absMainGrid.Margin = UI_Sizes.MainMargin;
            shadowFrame.CornerRadius = mainFrame.CornerRadius = UI_Sizes.BigFrameCornerRadius;
            shadowFrame.TranslationX = UI_Sizes.BigFrameShadowTranslationX;
            shadowFrame.TranslationY = UI_Sizes.BigFrameShadowTranslationY;           

            CloseBtn.WidthRequest = CloseBtn.HeightRequest = UI_Sizes.CloseBtnSize;
            

            gestureRecognizerHeart = new TapGestureRecognizer();
            gestureRecognizerHeart.Tapped += (sender, e) =>
            {
                View view = (View)sender;
                if (view != null) OnTappedHeart(view);
                // now you have a reference to the image
                Debug.WriteLine("tapped back");
            };

            gestureRecognizerItem = new TapGestureRecognizer();
            gestureRecognizerItem.Tapped += (sender, e) =>
            {
                View view = (View)sender;
                if (view != null) OnTappedItem(view);
                // now you have a reference to the image
                Debug.WriteLine("tapped back");
            };

            itemWidth = UI_Sizes.ScreenWidthX * 0.2;
            itemHeight = itemWidth / 0.8;

            LOADING = Translate.GetString("favorites_loading");
            favorites = Translate.GetString("favorites");
            favorite_books = Translate.GetString("favorite_books");

            UpdateFavoritesList();
        }

        protected override void OnAppearing ()
        {
            base.OnAppearing();
            Debug.WriteLine("Favorites_Page -> OnAppearing ->");
            if (postLaunched) postLaunched = false;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Debug.WriteLine("Favorites_Page -> OnDisappearing ->");
            if (!postLaunched)
            {
                // set favorites button
                rootViewModel.SetFavoritesButton();

                // UpdateLists !
                Debug.WriteLine("Favorites_Page -> TryToShowContent()");
                rootViewModel.testViewModel.UpdateCurrentPosition();
                rootViewModel.testViewModel.TryToShowContent();
            }
            needToUpdateState = false;
        }

        async void OnTappedHeart(View view)
        {
            if (IsAnimating) return;
            IsAnimating = true;
            Debug.WriteLine("Favorites_Page -> OnTappedHeart -> id=" + view.ClassId);
            await AnimateView(view, 250);

            //try to remove from favorites
            bool answer = await App.Current.MainPage.DisplayAlert(favorites, Translate.GetString("favorites_remove_post_from_favorites"), POP_UP.YES, POP_UP.NO);
            if (answer)
            {
                Debug.WriteLine("Favorites_Page -> remove");
                FavoriteListCoords flc = booksFavoritesId.ContainsKey(view.ClassId) ? booksFavoritesId[view.ClassId] : videosFavoritesId[view.ClassId];

                Favorites.WaitIfListsAreBuisy("Favorites_Page -> OnTappedHeart -> remove");

                Favorites.remove.Add(view.ClassId, flc);
                await Favorites.InstantRemoveAll();
                Favorites.SetBooksAndVideosCount_unprotected();
                Favorites.StartLoading();

                Favorites.ListsAreBuisy = false;
                Debug.WriteLine("Favorites_Page -> OnTappedHeart -> ListsAreBuisy = false");

                if (Favorites.Books + Favorites.Videos == 0) await App.Current.MainPage.Navigation.PopModalAsync();
                else
                {
                    // Favorites.StartLoading();
                    UpdateFavoritesList();
                }   
            }
            IsAnimating = false;            
        }



        async void OnTappedItem(View view)
        {
            if (IsAnimating) return;
            IsAnimating = true;
            Debug.WriteLine("Favorites_Page -> OnTappedItem -> id=" + view.ClassId);
            await AnimateView(view, 250);

            // get data for listview
            FavoriteListCoords flc = booksFavoritesId.ContainsKey(view.ClassId) ? booksFavoritesId[view.ClassId] : videosFavoritesId[view.ClassId];
           
            Debug.WriteLine($"Favorites_Page -> OnTappedItem -> flc.CentralView={flc.CentralView}");
            postLaunched = true;
            if (flc.CentralView == MainPage_ViewModel.CENTRAL_VIEWS.BOOKS)
                {
                    Debug.WriteLine("Favorites_Page -> OnTappedItem -> try to start book, id = " + view.ClassId);
                    await rootViewModel.ReadBook(view.ClassId);
                    
                }
           else
           { 
                Debug.WriteLine("Favorites_Page -> OnTappedItem -> try to start video, id = " + view.ClassId);
                ListItemStarsInfo starsInfo = new ListItemStarsInfo
                {
                    favorites_Page = this,
                    id = view.ClassId,

                    favoriteListCoords = flc,
                    POST_LIST_ITEM_DATA_key = CacheHelper.POST_LIST_ITEM_DATA + "_" + UserHelper.Lang_cat + "_" + UserHelper.Login.user_id + "_" + flc.CentralView + "_" + flc.SubView,
                    //CacheHelper.POST_LIST_ITEM_DATA + "_" + cur_cats + "_" + UserHelper.Login.user_id + "_" + viewType.CentralView + "_" + viewType.SubView;
                    rootVM = rootViewModel,
                };
                await rootViewModel.WatchLesson(view.ClassId, starsInfo);
           }
            

            IsAnimating = false;
        }

        public void UpdateFavoritesList()
        {
            Favorites.WaitIfListsAreBuisy("Favorites_Page -> UpdateFavoritesList()");
            IsAnimating = true;
            favoritesStack.Children.Clear();

            videosFavoritesId = new Dictionary<string, FavoriteListCoords>();
            booksFavoritesId = new Dictionary<string, FavoriteListCoords>();

            
            foreach (KeyValuePair<string, FavoriteListCoords> pair in Favorites.current)
            {
                if (pair.Value.CentralView == MainPage_ViewModel.CENTRAL_VIEWS.BOOKS)
                {
                    booksFavoritesId.Add(pair.Key, pair.Value);
                }
                else
                {
                    videosFavoritesId.Add(pair.Key, pair.Value);
                }
            }

            foreach (KeyValuePair<string, FavoriteListCoords> pair in Favorites.add)
            {
                if (pair.Value.CentralView == MainPage_ViewModel.CENTRAL_VIEWS.BOOKS)
                {
                    booksFavoritesId.Add(pair.Key, pair.Value);
                }
                else
                {
                    videosFavoritesId.Add(pair.Key, pair.Value);
                }
            }

            videosLoading = new Label[videosFavoritesId.Count];
            booksLoading = new Label[booksFavoritesId.Count];

            

            // add to stack
            if (videosFavoritesId.Count > 0)
            {
                // add title
                favoritesStack.Children.Add(new Label
                {
                    Text = Translate.GetString("favorites"),
                    TextColor = LABEL_COLOR,
                    FontSize = UI_Sizes.MediumTextSize,
                    FontFamily = Translate.fontFamily,
                    FontAttributes = Translate.fontAttributes,
                    HorizontalOptions = LayoutOptions.Center,
                });
                // add all items
                int row = 4;
                for (int i = 0; i < videosFavoritesId.Count; i += row)
                {
                    int lastIndex = i + row;
                    if (lastIndex > videosFavoritesId.Count) lastIndex = videosFavoritesId.Count;
                    // create horizontal stack

                    StackLayout hStack = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Center };
                    for (int j = i; j < lastIndex; j++)
                    { // add each element
                        Debug.WriteLine("Favorites_Page -> add to hStack, j = " + j);
                        var grid = new Grid() { WidthRequest = itemWidth, HeightRequest = itemHeight,
                            ClassId = videosFavoritesId.ElementAt(j).Key,
                        };
                        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(4, GridUnitType.Star) });
                        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                        grid.GestureRecognizers.Add(gestureRecognizerItem);

                        //get item data
                        ListViewItemData itemData =
                            Favorites.allListsData[(int)videosFavoritesId.ElementAt(j).Value.CentralView][videosFavoritesId.ElementAt(j).Value.SubView][videosFavoritesId.ElementAt(j).Value.index]; 
                        if (itemData != null)
                        {
                            // add stars
                            if (itemData.Stars > 0)
                            {
                                grid.Children.Add(new Image
                                {
                                    Source = TestViewModel.stars_imgs[itemData.Stars],
                                    Aspect = Aspect.AspectFit
                                }, 0, 0);
                            }
                            // add main image
                            CachedImage c = new CachedImage
                            {                                
                                Source = itemData.ImgResource,
                                Aspect = Aspect.AspectFit
                            };
                           
                            grid.Children.Add(c, 0, 1);

                            // add state label
                            Label loading = new Label
                            {                                
                                TextColor = Color.Gray,
                                FontSize = itemHeight * 0.1,
                                HorizontalOptions = LayoutOptions.Center,
                                HorizontalTextAlignment = TextAlignment.Center,
                                VerticalOptions = LayoutOptions.Center,

                            };

                            if (!Favorites.current.ContainsKey(videosFavoritesId.Keys.ElementAt(j)))
                            {
                                // add name
                                loading.Text = LOADING;
                                Debug.WriteLine("Favorites_Page -> *** LOADING *** videosFavoritesId.Keys.ElementAt(i) = " + videosFavoritesId.Keys.ElementAt(j));
                            }
                            else
                            {
                                Debug.WriteLine("Favorites_Page -> we have in currents -> videosFavoritesId.Keys.ElementAt(i) = " + videosFavoritesId.Keys.ElementAt(j));
                            }
                            videosLoading[j] = loading;
                            grid.Children.Add(loading, 0, 1);

                            // add favorites heart
                            Image img = new Image
                            {
                                ClassId = videosFavoritesId.ElementAt(j).Key,
                                Source = Favorites.SetVisualSource(videosFavoritesId.ElementAt(j).Key, TestViewModel.heart_color_img, TestViewModel.heart_gray_img, TestViewModel.heart_preloader_img),
                                Aspect = Aspect.AspectFit,
                                WidthRequest = itemWidth * 0.2,
                                HeightRequest = itemWidth * 0.2,
                                HorizontalOptions = LayoutOptions.End,
                                VerticalOptions = LayoutOptions.Start,

                            };
                            img.GestureRecognizers.Add(gestureRecognizerHeart);
                            grid.Children.Add(img, 0, 1);

                            // add name
                            Label name = new Label
                            {
                                Text = System.Net.WebUtility.HtmlDecode(Uri.UnescapeDataString(string.IsNullOrEmpty(itemData.TransName) ? itemData.Name : itemData.TransName)),                                
                                TextColor = LABEL_COLOR,
                                FontSize = itemHeight * 0.05,
                                HorizontalOptions = LayoutOptions.Center,
                                HorizontalTextAlignment = TextAlignment.Center,
                            };
                           // name.GestureRecognizers.Add(gestureRecognizerItem);
                            grid.Children.Add(name, 0, 2);

                            hStack.Children.Add(grid);
                        }
                    }

                    favoritesStack.Children.Add(hStack);
                }
            }

            // add books
            if (booksFavoritesId.Count > 0)
            {
                // add title
                favoritesStack.Children.Add (new Label {
                    Text = favorite_books, TextColor = LABEL_COLOR, FontSize = UI_Sizes.MediumTextSize,
                    FontFamily = Translate.fontFamily,
                    FontAttributes = Translate.fontAttributes,
                    HorizontalOptions  = LayoutOptions.Center,
                });
                // add all items
                int row = 4;
               for (int i = 0; i < booksFavoritesId.Count; i += row)
               {
                    int lastIndex = i + row;
                    if (lastIndex > booksFavoritesId.Count) lastIndex = booksFavoritesId.Count;
                    // create horizontal stack

                    StackLayout hStack = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Center };
                    for (int j = i; j < lastIndex; j ++)
                    { // add each element
                        var grid = new Grid() { WidthRequest = itemWidth, HeightRequest = itemHeight,
                            ClassId = booksFavoritesId.ElementAt(j).Key,
                        };
                        //grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });                       
                        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(4, GridUnitType.Star) });
                        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                        grid.GestureRecognizers.Add(gestureRecognizerItem);

                        //get item data
                        ListViewItemData itemData = Favorites.allListsData[(int)booksFavoritesId.ElementAt(j).Value.CentralView][booksFavoritesId.ElementAt(j).Value.SubView][booksFavoritesId.ElementAt(j).Value.index];
                        if (itemData != null)
                        {
                            // add stars
                            /*
                            if (itemData.Stars > 0)
                            {
                                grid.Children.Add(new Image {
                                    Source = TestViewModel.stars_imgs[itemData.Stars], Aspect = Aspect.AspectFit
                                }, 0,0);
                            }
                            */
                            // add main image
                            CachedImage c = new CachedImage
                            {
                                Source = itemData.ImgResource,
                                Aspect = Aspect.AspectFit
                            };
                            //c.GestureRecognizers.Add(gestureRecognizerItem);
                            grid.Children.Add(c, 0, 0);

                            // add state label
                            Label loading = new Label
                            {
                                TextColor = Color.Gray,
                                FontSize = itemHeight * 0.1,
                                HorizontalOptions = LayoutOptions.Center,
                                HorizontalTextAlignment = TextAlignment.Center,
                                VerticalOptions = LayoutOptions.Center,

                            };
                            if (!Favorites.current.ContainsKey(booksFavoritesId.Keys.ElementAt(j)))
                            {
                                // add name
                                loading.Text = LOADING;
                            }
                            booksLoading[j] = loading;
                            grid.Children.Add(loading, 0, 0);

                            // add favorites heart
                            Image img = new Image
                            {
                                ClassId = booksFavoritesId.ElementAt(j).Key,
                                Source = Favorites.SetVisualSource(booksFavoritesId.ElementAt(j).Key, TestViewModel.heart_color_img, TestViewModel.heart_gray_img, TestViewModel.heart_preloader_img),
                                Aspect = Aspect.AspectFit,
                                WidthRequest = itemWidth * 0.2,
                                HeightRequest = itemWidth * 0.2,
                                HorizontalOptions = LayoutOptions.End,
                                VerticalOptions = LayoutOptions.Start,

                            };
                            Debug.WriteLine($"Favorites_Page ->  BOOKS -> id = {booksFavoritesId.ElementAt(j).Key}, img.Source = {img.Source.ToString()}");
                            img.GestureRecognizers.Add(gestureRecognizerHeart);
                            grid.Children.Add(img, 0, 0);

                            // add name
                            Label name = new Label
                            {
                                Text = System.Net.WebUtility.HtmlDecode(Uri.UnescapeDataString(string.IsNullOrEmpty(itemData.TransName) ? itemData.Name: itemData.TransName)),
                                TextColor = LABEL_COLOR,
                                FontSize = itemHeight * 0.05,
                                HorizontalOptions = LayoutOptions.Center,
                                HorizontalTextAlignment = TextAlignment.Center,
                            };
                            Debug.WriteLine($"Favorites_Page ->  BOOKS -> id = {booksFavoritesId.ElementAt(j).Key}, name = {itemData.Name}");
                            //name.GestureRecognizers.Add(gestureRecognizerItem);
                            grid.Children.Add(name, 0, 1);

                            hStack.Children.Add(grid);
                        }
                    }

                    favoritesStack.Children.Add(hStack);
               }
            }

            IsAnimating = false;
            Favorites.ListsAreBuisy = false;
            Debug.WriteLine("Favorites_Page -> UpdateFavoritesList -> ListsAreBuisy = false");

            //DoOnUpdateState();
        }


        Task DoOnUpdateState ()
        {
            return Task.Run(async () =>
            {
                Debug.WriteLine("Favorites_Page -> DoOnUpdateState ->...");
                await Task.Delay(15000);
                if (needToUpdateState) UpdateLoadingState();               
            });             
        }

        void UpdateLoadingState()
        {
            Favorites.WaitIfListsAreBuisy("Favorites_Page -> UpdateLoadingState()");
            Debug.WriteLine("Favorites_Page -> UpdateLoadingState -> ");
            for (int i = 0; i < videosLoading.Length; i++)
            {
                Debug.WriteLine("Favorites_Page -> UpdateLoadingState -> videosLoading.Length = " + videosLoading.Length);
                if (Favorites.current.ContainsKey(videosFavoritesId.ElementAt(i).Key))
                {
                    videosLoading[i].Text = string.Empty;
                }
                else
                {
                    videosLoading[i].Text = LOADING;
                }
            }

            Debug.WriteLine("Favorites_Page -> UpdateLoadingState -> booksLoading.Length = " + booksLoading.Length);
            for (int i = 0; i < booksLoading.Length; i++)
            {
                Debug.WriteLine("Favorites_Page -> UpdateLoadingState -> booksLoading.Length = " + booksLoading.Length);
                if (Favorites.current.ContainsKey(booksFavoritesId.ElementAt(i).Key))
                {
                    booksLoading[i].Text = string.Empty;
                }
                else
                {
                    booksLoading[i].Text = LOADING;
                }
            }   

            Favorites.ListsAreBuisy = false;
            Debug.WriteLine("Favorites_Page -> UpdateLoadingState -> ListsAreBuisy = false");

            if (needToUpdateState) DoOnUpdateState();
        }

        private async void MenuButton_Tapped(object sender, System.EventArgs e)
        {
            Debug.WriteLine("SignUp_Page -> MenuButton_Tapped");
            if (IsAnimating) return;
            IsAnimating = true;
            View view = sender as View;
            await AnimateView(view, 250);

            if (view.ClassId == "CloseBtn")
            {
                await App.Current.MainPage.Navigation.PopModalAsync();
            }            

            IsAnimating = false;
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
            Debug.WriteLine("Favorites_Page -> Dispose");

            Content = null;
            BindingContext = null;

            gestureRecognizerHeart = gestureRecognizerItem = null;
            
            videosFavoritesId = booksFavoritesId = null;
            rootViewModel = null;

            
            videosLoading = booksLoading = null;
        }
    }
}