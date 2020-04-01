using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using FFImageLoading.Forms;
using Newtonsoft.Json;
using Plugin.Connectivity;
using Xamarin.Forms;

namespace DinoLingo
{
    public class BadgeList_Page_ViewModel: INotifyPropertyChanged
    {

        static int ERROR_PREFIX = 10;

        public double MyDinosTextSize { get; set; }

        BackButtonView backButtonView;
        INavigation navigation;
        ScrollView centerListView;
        List<BadgeListResponse.Badge> dataForListView;
        bool isLoading = false;

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

        public BadgeList_Page_ViewModel(INavigation navigation)
        {
            this.navigation = navigation;
            MyDinosTextSize = UI_Sizes.MediumTextSize * 0.7;
        }

        public void AddCloseButton(RelativeLayout totalLayout)
        {
            backButtonView = new BackButtonView();
            backButtonView.AddToTopRight(totalLayout, 0.1, 5, OnClickedClose);
        }

        public void AddCenterView(Frame frameForList)
        {
            centerListView = new ScrollView();
            Device.StartTimer(TimeSpan.FromMilliseconds(10), FillTheViewOnStartAsync);

            frameForList.Content = centerListView;
        }

        bool FillTheViewOnStartAsync()
        {
            Debug.WriteLine("FillTheViewOnStartAsync() ... ");
            //ListItems = null;
            TryToShowContent();
            return false;
        }

        async void TryToShowContent()
        {
            Debug.WriteLine("TryToShowContent...");
            // check, if we have cached content FOR THE current central view
            string cacheKey = CacheHelper.BADGE_LIST + UserHelper.Lang_cat + UserHelper.Login.user_id;
            if (await CacheHelper.Exists(cacheKey))
            { // we Do have the content - just show it
                Debug.WriteLine("we have content data...");
                dataForListView = new List<BadgeListResponse.Badge>(await CacheHelper.GetAsync<BadgeListResponse.Badge[]>(cacheKey));
                ShowList();
            }

            // ANYWAY !!! --> try to download
            Debug.WriteLine("try to download content anyway...");
            TryToDownloadContent();
        }

        void ShowList()
        {
            Debug.WriteLine("try to show list");
            Debug.WriteLine("total items in list: dataForListView.Count = " + dataForListView.Count);
            int fullRows = dataForListView.Count / 4;
            double cellsize = UI_Sizes.MediumTextSize * 6;
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (sender, e) =>
            {
                View view = (View) sender;
                if (view != null) OnClickedBadge(view);
            };
            StackLayout totalStack = new StackLayout();
            string fontFamily = null;

            if (Device.RuntimePlatform == Device.iOS) fontFamily = "Arial Rounded MT Bold";
            else if (Device.RuntimePlatform == Device.Android) fontFamily = "Arial_Rounded_MT_Bold.ttf#Arial Rounded MT Bold";


                for (int i = 0; i < fullRows; i++)
                {
                    StackLayout stackLayout = new StackLayout() { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Center, };
                    for (int j = i * 4; j < i * 4 + 4; j++)
                    {
                    var grid = new Grid() { WidthRequest = cellsize, HeightRequest = cellsize, ClassId = dataForListView[j].id };
                        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(4, GridUnitType.Star) });
                        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

                        Label label = new Label
                        {
                            Text = dataForListView[j].title,
                            TextColor = Color.FromHex("#0275c4"),
                            FontSize = UI_Sizes.MediumTextSize * 0.85,
                            FontFamily = fontFamily,
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalTextAlignment = TextAlignment.Center
                        };
                        CachedImage cachedImage = new CachedImage { Source = dataForListView[j].thumbnail, Aspect = Aspect.AspectFit };

                        grid.Children.Add(label, 0, 2);
                        grid.Children.Add(cachedImage, 0, 1);
                        grid.GestureRecognizers.Add(tapGestureRecognizer);
                        stackLayout.Children.Add(grid);
                        totalStack.Children.Add(stackLayout);
                    }
                }

                //add remainings
                StackLayout stackLayout2 = new StackLayout() { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Center, };
                for (int j = fullRows * 4; j < dataForListView.Count; j++)
                {
                var grid = new Grid() { WidthRequest = cellsize, HeightRequest = cellsize, ClassId = dataForListView[j].id };
                    grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(4, GridUnitType.Star) });
                    grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

                    Label label = new Label
                    {
                        Text = dataForListView[j].title,
                        TextColor = Color.FromHex("#0275c4"),
                        FontSize = UI_Sizes.MediumTextSize * 0.85,
                        FontFamily = fontFamily,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center
                    };
                CachedImage cachedImage = new CachedImage { Source = dataForListView[j].thumbnail, Aspect = Aspect.AspectFit };

                    grid.Children.Add(label, 0, 2);
                    grid.Children.Add(cachedImage, 0, 1);
                    grid.GestureRecognizers.Add(tapGestureRecognizer);
                    stackLayout2.Children.Add(grid);
                    totalStack.Children.Add(stackLayout2);
                }
           
                centerListView.Content = totalStack;
        }

        Task TryToDownloadContent()
        {
            Debug.WriteLine("try to update content...");
            return Task.Run(async () => { 
                isLoading = true;
                // check connection here
                if (!CrossConnectivity.Current.IsConnected || !DownloadHelper.DownloadHelper.CheckInternetConnectionProgressive())
                { // check connectivity
                    if (AsyncMessages.CheckConnectionTimeout())
                    {
                        if (dataForListView != null)
                        {
                            AsyncMessages.DisplayAlert(POP_UP.INFO, POP_UP.NO_CONNECTION, POP_UP.OK);
                            isLoading = false;
                            return;
                        }
                        else
                        {
                            AsyncMessages.DisplayAlert(POP_UP.INFO, POP_UP.NO_CONNECTION, POP_UP.OK);
                            isLoading = false;
                            return;
                        }
                    }
                    else
                    {
                        isLoading = false;
                        return;
                    }
                }

                // get BadgeListResponse
                var postData = $"cat={UserHelper.Lang_cat}";
                postData += $"&user_id={UserHelper.Login.user_id}";
                BadgeListResponse badgeListResponse = await ServerApi.PostRequestProgressive<BadgeListResponse>(ServerApi.BADGE_LIST_URL, postData, null);

                // process response
                if (badgeListResponse == null)
                {
                    Analytics.SendResultsRegular("BadgeList_Page_ViewModel", badgeListResponse, badgeListResponse?.error, ServerApi.BADGE_LIST_URL, postData);
                    if (AsyncMessages.CheckDisplayAlertTimeout())
                    {
                        AsyncMessages.DisplayAlert(POP_UP.OOPS,
                                                POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(badgeListResponse?.error, ERROR_PREFIX + 1), POP_UP.OK);
                    }
                    

                    isLoading = false;
                    return;
                }
                if (badgeListResponse.error != null)
                {
                    Analytics.SendResultsRegular("BadgeList_Page_ViewModel", badgeListResponse, badgeListResponse?.error, ServerApi.BADGE_LIST_URL, postData);
                    if (AsyncMessages.CheckDisplayAlertTimeout()) {
                        AsyncMessages.DisplayAlert(POP_UP.OOPS,
                       POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(badgeListResponse?.error, ERROR_PREFIX + 2), POP_UP.OK);
                    }
                   
                    isLoading = false;
                    return;
                }
                if (badgeListResponse.result == null)
                {
                    Analytics.SendResultsRegular("BadgeList_Page_ViewModel", badgeListResponse, badgeListResponse?.error, ServerApi.BADGE_LIST_URL, postData);
                    if (AsyncMessages.CheckDisplayAlertTimeout())
                    {
                        AsyncMessages.DisplayAlert(POP_UP.OOPS,
                        POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(badgeListResponse?.error, ERROR_PREFIX + 3), POP_UP.OK);
                    }
                        
                    isLoading = false;
                    return;
                }

                // add to cache
                Debug.WriteLine("badgeListResponse downloaded ok, badgeListResponse.result.L = " + badgeListResponse.result.Length);
                await CacheHelper.Add<BadgeListResponse.Badge[]>(CacheHelper.BADGE_LIST + UserHelper.Lang_cat + UserHelper.Login.user_id, badgeListResponse.result);
                dataForListView = new List<BadgeListResponse.Badge>(badgeListResponse.result);

                Device.BeginInvokeOnMainThread(ShowList);
                isLoading = false;
                return;
            });
        }

        async void OnClickedClose(View view)
        {
            if (IsAnimating) return;
            IsAnimating = true;
            await AnimateImage(view, 250);
            await navigation.PopModalAsync();
            IsAnimating = false;
        }

        async void OnClickedBadge(View view)
        {
            if (IsAnimating) return;
            IsAnimating = true;
            Debug.WriteLine("Tapped badge, ClassId = " + view.ClassId);
            // open dino page -->


            //Page page = new DinoPage(navigation, view.ClassId);
                await AnimateImage(view, 250);
                //await navigation.PushModalAsync(new DinoPage(navigation, view.ClassId));
                await App.Current.MainPage.Navigation.PushModalAsync(new DinoPage(navigation, view.ClassId));
            IsAnimating = false;
        }

        public void OnAppearing() {
            // fill the list with badges
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Task AnimateImage(View view, uint time)
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

        public void Dispose()
        {
            if (backButtonView != null) backButtonView.Dispose();
            backButtonView = null;
            navigation = null;
            centerListView.Content = null;
            centerListView = null;
            dataForListView = null;
            animLock = null;
        }
    }
}
