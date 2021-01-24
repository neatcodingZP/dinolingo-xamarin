using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Connectivity;
using Xamarin.Forms;

namespace DinoLingo
{
    public class ChooseLanguage_ViewModel: INotifyPropertyChanged
    {
        static int ERROR_PREFIX = 40;
        INavigation navigation;
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<LangItem> ListItemSource { get; set; }
        public bool IsListEnabled { get; set; }
        public double DinoSize { get; set; }
        public Thickness ListPadding { get; set; }
        Thickness listItemsMargin;
        
        public double HeaderFontSize { get; set; }
        public double  BigCornerRadius { get; set; }

        Color BACKGROUND_NOT_PURCHASED = (Color)Application.Current.Resources["ButtonBlueBackgroundColor"];
        Color BACKGROUND_NOT_PURCHASED_SELECTED = Color.Red;
        Color BACKGROUND_PURCHASED = (Color)Application.Current.Resources["ReportOrangeColor"];
        Color BACKGROUND_PURCHASED_SELECTED = Color.Green;

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
        bool IsDownloading = false;


        public ChooseLanguage_ViewModel(Login_Response.Login loginResult, INavigation navigation)
        {
            int maxLenght = 15;
            double lenghtCoeff = 0.7;
            IsListEnabled = !IsDownloading; 
            this.navigation = navigation;
            DinoSize = UI_Sizes.ScreenHeightX * 0.5;
            HeaderFontSize = UI_Sizes.MediumTextSize * 0.8;
            BigCornerRadius = UI_Sizes.BigFrameCornerRadius;
            ListPadding = new Thickness(BigCornerRadius * 0.27, BigCornerRadius * 0.35, BigCornerRadius * 0.27, BigCornerRadius * 0.35);
            listItemsMargin = new Thickness(8, 4, 8, 4);

            ObservableCollection<LangItem> items = new ObservableCollection<LangItem>();
            List<string> all_lang_cats = new List<string>(LANGUAGES.CAT_INFO.Keys);
            
            if (loginResult.products != null && loginResult.products.Length > 0)
            {
                foreach (Login_Response.Product p in loginResult.products)
                {
                    string lang_cat = p.cat_id.ToString();
                    if (all_lang_cats.Contains(lang_cat)) {
                        all_lang_cats.Remove(lang_cat);
                        string name = LANGUAGES.CAT_INFO[lang_cat].GetVisibleName();
                        items.Add(new LangItem
                        {
                            LanguageName = name,
                            product_id = p.product_id,
                            cat_id = lang_cat,
                            CellColor = BACKGROUND_PURCHASED,
                            TextColor = Color.White,
                            CellHeight = UI_Sizes.MediumTextSize * 1.5,
                            FontSize = name.Length < maxLenght ? UI_Sizes.MediumTextSize: UI_Sizes.MediumTextSize * lenghtCoeff,
                            ListItemsMargin = listItemsMargin,
                        });                        
                    }
                }
            }

            if (loginResult.IAPproducts != null && loginResult.IAPproducts.Length > 0)
            {
                foreach (Login_Response.Product p in loginResult.IAPproducts)
                {
                    string lang_cat = p.cat_id.ToString();
                    if (all_lang_cats.Contains(lang_cat))
                    {
                        all_lang_cats.Remove(lang_cat);
                        string name = LANGUAGES.CAT_INFO[lang_cat].GetVisibleName();
                        items.Add(new LangItem
                        {
                            LanguageName = name,
                            product_id = p.product_id,
                            cat_id = lang_cat,
                            CellColor = BACKGROUND_PURCHASED,
                            TextColor = Color.White,
                            CellHeight = UI_Sizes.MediumTextSize * 1.5,
                            FontSize = name.Length < maxLenght ? UI_Sizes.MediumTextSize : UI_Sizes.MediumTextSize * lenghtCoeff,
                            ListItemsMargin = listItemsMargin,
                        });
                    }
                }
            }


            foreach (string lang_cat in all_lang_cats) {
                string name = LANGUAGES.CAT_INFO[lang_cat].GetVisibleName();
                items.Add(new LangItem
                {
                    LanguageName = name,
                    product_id = "",
                    cat_id = lang_cat,
                    CellColor = BACKGROUND_NOT_PURCHASED,
                    TextColor = Color.White,
                    CellHeight = UI_Sizes.MediumTextSize * 1.5,
                    FontSize = name.Length < maxLenght ? UI_Sizes.MediumTextSize : UI_Sizes.MediumTextSize * lenghtCoeff,
                    ListItemsMargin = listItemsMargin,
                });
            }

            ListItemSource = items;
        }

        public async void MenuButton_Tapped(object sender, System.EventArgs e)
        {
            Debug.WriteLine("ChooseLanguage_Page -> MenuButton_Tapped");
            if (IsAnimating) return;
            View view = sender as View;
            await AnimateView(view, 250);

            if (view.ClassId == "DinoLingoLink")
            {
                Debug.WriteLine("ChooseLanguage_Page -> MenuButton_Tapped -> DinoLingoLink");
                Device.OpenUri(new System.Uri("https://dinolingo.com/"));
            }
            else if (view.ClassId == "CloseBtn")
            {
                Debug.WriteLine("ChooseLanguage_Page -> MenuButton_Tapped -> CloseBtn");
                if (App.Current.MainPage.Navigation.ModalStack.Count > 0) await App.Current.MainPage.Navigation.PopModalAsync();
            }

            IsAnimating = false;
        }

        public void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (IsDownloading) return;
            Debug.WriteLine("ChooseLanguage_Page -> OnItemSelected ...");
            if (e.SelectedItem != null)
            {
                Debug.WriteLine("ChooseLanguage_Page -> OnItemSelected -> name = " + (e.SelectedItem as LangItem).LanguageName);
                if ((e.SelectedItem as LangItem).CellColor == BACKGROUND_NOT_PURCHASED) (e.SelectedItem as LangItem).CellColor = BACKGROUND_NOT_PURCHASED_SELECTED;
                else (e.SelectedItem as LangItem).CellColor = BACKGROUND_PURCHASED_SELECTED;

                ((ListView)sender).SelectedItem = null;
            }            
        }

        public async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (IsDownloading) return;
            Debug.WriteLine("ChooseLanguage_Page -> OnItemTapped ...");
            IsDownloading = true; IsListEnabled = !IsDownloading;
            if (e.Item != null) {
                Debug.WriteLine("Tapped :" + (e.Item as LangItem).LanguageName + ", product_id:" + (e.Item as LangItem).product_id);
                ((ListView)sender).SelectedItem = null;
                //add current language 
                string old_lang_cat = UserHelper.Lang_cat;
                string old_user_id = UserHelper.Login.user_id;
                bool old_IsFree = UserHelper.IsFree;


                UserHelper.Language = (e.Item as LangItem).LanguageName.ToLower();
                UserHelper.Lang_cat = (e.Item as LangItem).cat_id;
                UserHelper.SetIsFree();

                await CacheHelper.Add(CacheHelper.CURRENT_LANGUAGE, UserHelper.Language);
                await CacheHelper.Add(CacheHelper.CURRENT_LANGUAGE_CAT, UserHelper.Lang_cat);


                //add memory gamebjects //add sas gameobjects
                GameHelper.memory_GameObjects = await CacheHelper.GetAsync<GameObjects>(CacheHelper.MEMORY_GAMEOBJECTS + UserHelper.Lang_cat);
                GameHelper.sas_GameObjects = await CacheHelper.GetAsync<GameObjects>(CacheHelper.SAS_GAMEOBJECTS + UserHelper.Lang_cat);

                             

                await Favorites.Refresh(old_lang_cat, old_user_id, old_IsFree, UserHelper.Lang_cat, UserHelper.Login.user_id, UserHelper.IsFree);
                if (!UserHelper.IsFree) Favorites.StartLoading();

                GetCategoriesAndLaunchMainPage((e.Item as LangItem).cat_id);
            }
            else {
                IsDownloading = false; IsListEnabled = !IsDownloading;
            }
        }



        public async void GetCategoriesAndLaunchMainPage(string cat_id)
        {
            try
            {
                if (await CacheHelper.Exists(CacheHelper.CATEGORYS_RESPONSE + cat_id))
                {
                    //we do not need to download cats here...
                    Debug.WriteLine("we have the categories, cat_id = " + cat_id);
                    CategoryResponse categoryResponse = await CacheHelper.GetAsync<CategoryResponse>(CacheHelper.CATEGORYS_RESPONSE + cat_id);
                    Debug.WriteLine("Choose Lang -> categories:" + (await CacheHelper.GetAsync(CacheHelper.CATEGORYS_RESPONSE + cat_id)).Data);

                    //block parent menu page
                    ParentMenu_Page.IsBlocked = true;

                    Page current = App.Current.MainPage;
                    
                    while (current.Navigation.ModalStack.Count > 0)
                    {
                        await current.Navigation.PopModalAsync();
                    };                   

                    Debug.WriteLine("ChooseLanguage_ViewModel -> new MainPage_");
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        App.Current.MainPage = new MainPage_(categoryResponse.result[0].viewType, 1);
                    });                    

                    //await App.Current.MainPage.Navigation.PushAsync(new MainPage_(categoryResponse.result[0].viewType, 1));
                    //App.Current.MainPage.Navigation.RemovePage(App.Current.MainPage.Navigation.NavigationStack[0]);

                    // ??? navigation.RemovePage(current);
                }
                else
                { // download categories...
                    if (!CrossConnectivity.Current.IsConnected || !DownloadHelper.DownloadHelper.CheckInternetConnectionProgressive())
                    { // check connectivity
                        AsyncMessages.CheckConnectionTimeout();                        
                        await App.Current.MainPage.DisplayAlert(POP_UP.OOPS, POP_UP.NO_CONNECTION, POP_UP.OK);

                        IsDownloading = false; IsListEnabled = !IsDownloading;
                        return;
                    }

                    var postData = $"cat={cat_id}";
                    CategoryResponse categoryResponse = await ServerApi.PostRequestProgressive<CategoryResponse>(ServerApi.CATS_URL, postData, null);
                    Debug.WriteLine("Choose Lang -> categoryResponse = " + JsonConvert.SerializeObject(categoryResponse));
                    if (categoryResponse != null && categoryResponse.result != null && categoryResponse.result.Length > 0)
                    {
                        if (categoryResponse.error == null)
                        {
                            categoryResponse.ReorderForGame(UserHelper.Lang_cat);
                            await CacheHelper.Add(CacheHelper.CATEGORYS_RESPONSE + cat_id, categoryResponse);

                            IsDownloading = false; IsListEnabled = !IsDownloading;

                            //block parent menu page
                            ParentMenu_Page.IsBlocked = true;

                            Page current = App.Current.MainPage;
                            while (current.Navigation.ModalStack.Count > 0)
                            {
                                await current.Navigation.PopModalAsync();
                            };
                            Debug.WriteLine("Choose Lang -> categoryResponse = " + JsonConvert.SerializeObject(categoryResponse));
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                App.Current.MainPage = new MainPage_(categoryResponse.result[0].viewType);
                            });
                            
                            //await App.Current.MainPage.Navigation.PushAsync(new MainPage_(categoryResponse.result[0].viewType));
                            //App.Current.MainPage.Navigation.RemovePage(App.Current.MainPage.Navigation.NavigationStack[0]);
                            //current.Navigation.RemovePage(current);
                            // ??? navigation.RemovePage(current);

                        }
                        else
                        {
                            Analytics.SendResultsRegular("ChooseLanguage_ViewModel", categoryResponse, categoryResponse?.error, ServerApi.CATS_URL, postData);
                            if (AsyncMessages.CheckDisplayAlertTimeout())
                            {
                                await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                                POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(categoryResponse?.error, ERROR_PREFIX + 1), POP_UP.OK);
                            }
                            
                            //("Error in categories!", categoryResponse.error.message, "ОK");                        
                        }
                    }
                    else
                    {
                        Analytics.SendResultsRegular("ChooseLanguage_ViewModel", categoryResponse, categoryResponse?.error, ServerApi.CATS_URL, postData);
                        if (AsyncMessages.CheckDisplayAlertTimeout())
                        {
                            await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                            POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(categoryResponse?.error, ERROR_PREFIX + 2), POP_UP.OK);
                        }
                        
                        //("Error!", "Can not get categories from server! Check Your internet connection!", "ОK");                   
                    }
                    IsDownloading = false; IsListEnabled = !IsDownloading;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ChooseLanguage_ViewModel -> GetCategoriesAndLaunchMainPage -> ex:" + ex.Message);
                IsDownloading = false; IsListEnabled = !IsDownloading;
            }
            
        }

        Task AnimateView(View view, uint time)
        {
            IsAnimating = true;
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

        public class LangItem {
            public string LanguageName { get; set; }
            public string product_id { get; set; }
            public string cat_id { get; set; }
            public Color CellColor { get; set; }
            public Color TextColor { get; set; }
            public double CellHeight { get; set; }
            public double FontSize { get; set; }
            public Thickness ListItemsMargin { get; set; }
        }

        public void Dispose()
        {
            navigation = null;
            PropertyChanged = null;
            ListItemSource = null;
            animLock = null;
        }

    }   
}
