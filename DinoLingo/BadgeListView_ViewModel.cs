using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Plugin.Connectivity;
using Xamarin.Forms;

namespace DinoLingo
{
    public class BadgeListView_ViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

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

        List<BadgeListResponse.Badge> dataForListView;
        ObservableCollection<ListViewItem> tempListItems;
        ObservableCollection<ListViewItem> listItems;
        public ObservableCollection<ListViewItem> ListItems
        {
            get
            {
                return listItems;
            }
            set
            {
                listItems = value;
                OnPropertyChanged();
            }
        }
        bool isLoading = false;

        public BadgeListView_ViewModel()
        {
            // check, if we already have the BadgeList for current Lag_cat
            // if have - instantly show it
            // else start lazy load
            Device.StartTimer(TimeSpan.FromMilliseconds(100), FillTheViewOnStartAsync);
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
                UpdateObservableCollectionFull(dataForListView);
                ShowList();
            }
            else // we need connection here 
            {
                Debug.WriteLine("we do not have cats data...");
                TryToDownloadContent();
            }
        }

        async void TryToDownloadContent () {
            isLoading = true;
            // check connection here
            if (!CrossConnectivity.Current.IsConnected) { // check connectivity
                await App.Current.MainPage.DisplayAlert("Error!", "No internet connection!", "ОK");
                isLoading = false;
                return;
            }

            // get BadgeListResponse
            var postData = $"cat={UserHelper.Lang_cat}";
            postData += $"&user_id={UserHelper.Login.user_id}";
            BadgeListResponse badgeListResponse = await ServerApi.PostRequest<BadgeListResponse>(ServerApi.BADGE_LIST_URL, postData, null);

            // process response
            if (badgeListResponse == null)
            {
                await App.Current.MainPage.DisplayAlert("Error!", "Error in response from server!", "ОK");
                isLoading = false;
                return;
            }
            if (badgeListResponse.error != null)
            {
                await App.Current.MainPage.DisplayAlert("Error!", "Message: " + badgeListResponse.error.message, "ОK");
                isLoading = false;
                return;
            }
            if (badgeListResponse.result == null)
            {
                await App.Current.MainPage.DisplayAlert("Error!", "Result from server is null!", "ОK");
                isLoading = false;
                return;
            }

            // add to cache
            Debug.WriteLine("badgeListResponse downloaded ok, badgeListResponse.result.L = " + badgeListResponse.result.Length);
            await CacheHelper.Add<BadgeListResponse.Badge[]>(CacheHelper.BADGE_LIST + UserHelper.Lang_cat + UserHelper.Login.user_id, badgeListResponse.result);
            dataForListView = new List<BadgeListResponse.Badge>(badgeListResponse.result);
            UpdateObservableCollectionFull(dataForListView);
            ShowList();
            isLoading = false;
        }

        void ShowList()
        {
            Debug.WriteLine("try to show list");
            Debug.WriteLine("total items in list: tempListItems.Count = " + tempListItems.Count);
            ListItems = tempListItems;
        }
        /*
        void UpdateObservableCollectionFull(List<BadgeListResponse.Badge> dataForListView)
        { 
            double cellH = App.Current.MainPage.Height * 0.5;
            tempListItems = new ObservableCollection<ListViewItem>();
            tempListItems.Add(
                new SingleItem
                {
                    Name = dataForListView[0].title,
                    Index = 0,
                    Imagesource = dataForListView[0].thumbnail,
                    CellHeight = cellH,
                    FontSize = cellH * 0.04
                }
                        );
            tempListItems.Add(
                new DoubleItem
                {
                    Name = dataForListView[0].title,
                    Index = 0,
                    Imagesource = dataForListView[0].thumbnail,
                    CellHeight = cellH,
                    FontSize = cellH * 0.04
                }
                        );
            tempListItems.Add(
                new TrioItem
                {
                    Name = dataForListView[0].title,
                    Index = 0,
                    Imagesource = dataForListView[0].thumbnail,
                    CellHeight = cellH,
                    FontSize = cellH * 0.04
                }
                        );
            tempListItems.Add(
                new QuadroItem
                            {
                                Name4 = dataForListView[0].title,
                                Index4 = 0,
                                Imagesource4 = dataForListView[0].thumbnail,
                                CellHeight = cellH,
                                FontSize = cellH * 0.04
                            }
                        );
        }
*/


        void UpdateObservableCollectionFull(List<BadgeListResponse.Badge> dataForListView)
        {
            Debug.WriteLine("BadgeList ViewModel --> UpdateObservableCollection: dataForListView.Count=" + dataForListView.Count);
            tempListItems = new ObservableCollection<ListViewItem>();
            double cellH = App.Current.MainPage.Height * 0.5;

            int fullRows = dataForListView.Count / 4;
            if (fullRows > 0)
            for (int i = 0; i < dataForListView.Count; i += 4) {
                tempListItems.Add(
                            new QuadroItem
                            {
                                Name = dataForListView[0].title,
                                Index = i,
                                Imagesource = dataForListView[0].thumbnail,
                                CellHeight = cellH,
                                FontSize = cellH * 0.04,
                                
                                Name2 = dataForListView[i+1].title,
                                Index2 = i+1,
                                Imagesource2 = dataForListView[i+1].thumbnail,
                                
                                Name3 = dataForListView[i + 2].title,
                                Index3 = i + 2,
                                Imagesource3 = dataForListView[i + 2].thumbnail,

                                Name4 = dataForListView[i + 3].title,
                                Index4 = i + 3,
                                Imagesource4 = dataForListView[i + 3].thumbnail,
                                
                            }
                        );
            }
            Debug.WriteLine("fullRows = " + fullRows);

            switch (dataForListView.Count % 4) {
                case 1: 
                    tempListItems.Add(
                            new SingleItem
                            {
                        Name = dataForListView[fullRows*4].title,
                        Index = fullRows*4,
                        Imagesource = dataForListView[fullRows*4].thumbnail,
                                CellHeight = cellH,
                                FontSize = cellH * 0.04
                            }
                        );
                    break;

                case 2: 
                    tempListItems.Add(
                            new DoubleItem
                            {
                        Name = dataForListView[fullRows*4].title,
                        Index = fullRows*4,
                        Imagesource = dataForListView[fullRows*4].thumbnail,
                                CellHeight = cellH,
                                FontSize = cellH * 0.04,
                                
                        Name2 = dataForListView[fullRows * 4 + 1].title,
                        Index2 = fullRows * 4 + 1,
                        Imagesource2 = dataForListView[fullRows * 4 + 1].thumbnail,
                            }
                        );
                    break;

                case 3:
                    Debug.WriteLine("Switch 3 is here");
                    tempListItems.Add(
                            new TrioItem
                            {
                        
                        Name = dataForListView[fullRows * 4].title,

                        Index = fullRows * 4,
                        Imagesource = dataForListView[fullRows * 4].thumbnail,
                                CellHeight = cellH,
                                FontSize = cellH * 0.04,

                        Name2 = dataForListView[fullRows * 4 + 1].title,
                        Index2 = fullRows * 4 + 1,
                        Imagesource2 = dataForListView[fullRows * 4 + 1].thumbnail,

                        Name3 = dataForListView[fullRows * 4 + 2].title,
                        Index3 = fullRows * 4 + 2,
                        Imagesource3 = dataForListView[fullRows * 4 + 2].thumbnail,

                            }
                        );
                    break;
                default: break;
            }


            Debug.WriteLine("UpdateObservableCollectionFull done.");
        }
       
        public async void Name_OnTapped(object sender, MyListItemEventArgs e)
        {
            if (IsAnimating) return;
            Debug.WriteLine("Name_OnTapped");
            var item = e.MyItem;
            if (item == null) return;
            // now you can fully access the listview item here via the variable "item"
            // ...
            await AnimateImage(sender as View, 250);
            Debug.WriteLine("Name_OnTapped,  e.MyItem.Name= " + e.MyItem.Name + ", e.MyItem.Index=" + e.MyItem.Index);
            int index = e.MyItem.Index;
            OnTapped1or2(index, e.MyItem.Name);
        }

        public async void Name_OnTapped2(object sender, MyListItemEventArgs e)
        {
            if (IsAnimating) return;
            Debug.WriteLine("Name_OnTapped2");

            var item = e.MyItem as DoubleItem;
            if (item == null) return;
            // now you can fully access the listview item here via the variable "item"
            // ...
            await AnimateImage(sender as View, 250);
            Debug.WriteLine("Name_OnTapped2,  item.Name2= " + item.Name2 + ", item.Index2=" + item.Index2);
            int index = item.Index2;
            OnTapped1or2(index, item.Name2);
        }

        public async void Name_OnTapped3(object sender, MyListItemEventArgs e)
        {
            if (IsAnimating) return;
            Debug.WriteLine("Name_OnTapped3");
            var item = e.MyItem as TrioItem;
            if (item == null) return;
            // now you can fully access the listview item here via the variable "item"
            // ...
            await AnimateImage(sender as View, 250);
            Debug.WriteLine("Name_OnTapped3,  item.Name3= " + item.Name3 + ", item.Index3=" + item.Index3);
            int index = item.Index3;
            OnTapped1or2(index, item.Name3);
        }

        public async void Name_OnTapped4(object sender, MyListItemEventArgs e)
        {
            if (IsAnimating) return;
            Debug.WriteLine("Name_OnTapped4");
            var item = e.MyItem as QuadroItem;
            if (item == null) return;
            // now you can fully access the listview item here via the variable "item"
            // ...
            await AnimateImage(sender as View, 250);
            Debug.WriteLine("Name_OnTapped4,  item.Name4= " + item.Name4 + ", item.Index4=" + item.Index4);
            int index = item.Index4;
            OnTapped1or2(index, item.Name4);
        }

        async void OnTapped1or2(int index, string title)
        {
            Debug.WriteLine("OnTapped1or2, title = " + title);
        }

        Task AnimateImage(View view, uint time)
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
    }
}
