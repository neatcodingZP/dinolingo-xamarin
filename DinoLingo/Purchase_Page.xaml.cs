using Newtonsoft.Json;
using Plugin.Connectivity;
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
	public partial class Purchase_Page : ContentPage
	{
        static int ERROR_PREFIX = 200;

        public double InfoFontSize { get; set; }
        public double BtnCornerRadius { get; set; }
        public double BtnCornerRadiusInner { get; set; }
        public double BtnShadowTranslationY { get; set; }
        public Thickness StrokeLineMarginBtn { get; set; }
        public double  BtnTextSize { get; set; }

        bool IsAnimating = false;
        string langCat;
        int termIndex;
        string[] plans;// = new string[] { "6 months", "1 year" };

        /*
        string[][] month_prices_ios = new string[][] {
            new string[] { "16.50", "13.30" },
            new string[] { "9.80", "8.20" },
        };

        string[][] month_prices_androids = new string[][] {
            new string[] { "16.70", "13.30" },
            new string[] { "10", "8.30" },
        };
        
        string[][] month_prices;

        */
        private double _loginBtnWidthFactor = 420.0 / 125.0;
        private string _blueBtnRes = "DinoLingo.Resources.UI.btnblue_new.png";

        public Purchase_Page ()
		{
			InitializeComponent ();
            FlowDirection = Translate.FlowDirection_;
            BindingContext = this;

            //month_prices = Device.RuntimePlatform == Device.iOS ? month_prices_ios : month_prices_androids;
            infoScrollView.IsEnabled = infoScrollView.IsVisible = Device.RuntimePlatform == Device.iOS;

            plans = new string[] {
                Translate.GetString("purchase_6_months_of_full_access"),
                Translate.GetString("purchase_1_year_of_full_access"),
                Translate.GetString("purchase_1_month_of_full_access")};

            absMainGrid.Margin = UI_Sizes.MainMargin;
            shadowFrame.CornerRadius = mainFrame.CornerRadius = UI_Sizes.BigFrameCornerRadius;
            shadowFrame.TranslationX =  UI_Sizes.BigFrameShadowTranslationX;
            shadowFrame.TranslationY = UI_Sizes.BigFrameShadowTranslationY;

            Title.FontSize = UI_Sizes.SmallTextSize;
            Length.FontSize = UI_Sizes.SmallTextSize;
            Price.FontSize = UI_Sizes.MediumTextSize;
            Description.FontSize = UI_Sizes.SmallTextSize * 0.75;
            InfoFontSize = UI_Sizes.MicroTextSize * 0.9;
            TermsOfUse.FontSize = PrivacyPolicy.FontSize = UI_Sizes.SmallTextSize * 0.8;

            BtnCornerRadius = UI_Sizes.ButtonCornerRadius;
            BtnCornerRadiusInner = BtnCornerRadius - UI_Sizes.ButtonOutlineWidth;
            BtnShadowTranslationY = UI_Sizes.ButtonShadowTranslationY;
            StrokeLineMarginBtn = new Thickness(UI_Sizes.ButtonOutlineWidth);
            BtnTextSize = UI_Sizes.SmallTextSize;

            CloseBtn.WidthRequest = CloseBtn.HeightRequest = UI_Sizes.CloseBtnSize;
            Purchaser.OnPurchased += OnPurchased;

            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += MenuButton_Tapped;

            // purchase button            
            MyViews.ButtonWithImage purchaseBtn = new MyViews.ButtonWithImage(
                "PurchaseBtn",
                new Rectangle(0.5, 0.5, UI_Sizes.ButtonHeight * _loginBtnWidthFactor, UI_Sizes.ButtonHeight),
                _blueBtnRes,
                Translate.GetString("purchase"),
                UI_Sizes.SmallTextSize,
                Color.White,
                tapGestureRecognizer,
                AbsoluteLayoutFlags.PositionProportional
                );

            robustGrid.Children.Add(purchaseBtn, 0, 5);
        }

        public Purchase_Page(string langCat, int termIndex): this() {
            this.langCat = langCat;
            this.termIndex = termIndex;

            Title.Text = Purchaser.GetProductNameByCatAndTerm(langCat, termIndex);
            Length.Text = Purchaser.TermToLenght(termIndex);
            Price.Text = Purchaser.Price(termIndex, langCat);

            string productName = string.Format(Translate.GetString("header_for_kids"), LANGUAGES.CAT_INFO[langCat].VisibleName).FirstLetterToUpperCase();

            //Description.Text = string.Format(plans[termIndex], productName, month_prices[Purchaser.PriceCat(langCat)][termIndex]);           
            Description.Text = string.Format(plans[termIndex], productName);
        }

        protected override void OnDisappearing()
        {
            Purchaser.OnPurchased -= OnPurchased;
        }



        async void OnPurchased()
        {
            Debug.WriteLine("Purchase_Page -> OnPurchased...");
            // check if we need to sign up         
            
            // GO TO THE MAIN PAGE
           
            //add current language 
            string old_lang_cat = UserHelper.Lang_cat;
            string old_user_id = UserHelper.Login.user_id;
            bool old_IsFree = UserHelper.IsFree;

            UserHelper.Lang_cat = Purchaser.Lang_cat;
            UserHelper.Language = LANGUAGES.CAT_INFO[UserHelper.Lang_cat].Name;
            UserHelper.SetIsFree();

            await CacheHelper.Add(CacheHelper.CURRENT_LANGUAGE, UserHelper.Language);
            await CacheHelper.Add(CacheHelper.CURRENT_LANGUAGE_CAT, UserHelper.Lang_cat);

            GameHelper.memory_GameObjects = await CacheHelper.GetAsync<GameObjects>(CacheHelper.MEMORY_GAMEOBJECTS + UserHelper.Lang_cat);
            GameHelper.sas_GameObjects = await CacheHelper.GetAsync<GameObjects>(CacheHelper.SAS_GAMEOBJECTS + UserHelper.Lang_cat);

            await Favorites.Refresh(old_lang_cat, old_user_id, old_IsFree, UserHelper.Lang_cat, UserHelper.Login.user_id, UserHelper.IsFree);
            if (!UserHelper.IsFree) Favorites.StartLoading();

            // cache the widget
            RateWidget.State.justPurchased = 1;
            await CacheHelper.Add<RateWidget.RateState>(CacheHelper.RATE_STATE, RateWidget.State);

            GetCategoriesAndLaunchNextPage(UserHelper.Lang_cat, string.IsNullOrEmpty(UserHelper.Login.user_id));            
            
        }

        async void GetCategoriesAndLaunchNextPage(string cat_id, bool needToSignUp)
        {
            if (await CacheHelper.Exists(CacheHelper.CATEGORYS_RESPONSE + cat_id))
            {
                //we do not need to download cats here...
                Debug.WriteLine("we have the categories, cat_id = " + cat_id);
                CategoryResponse categoryResponse = await CacheHelper.GetAsync<CategoryResponse>(CacheHelper.CATEGORYS_RESPONSE + cat_id);
                Debug.WriteLine("categories:" + (await CacheHelper.GetAsync(CacheHelper.CATEGORYS_RESPONSE + cat_id)).Data);

                if (App.DEBUG) await App.Current.MainPage.DisplayAlert("DEBUG", "we have cached categories -> go to main page", "ОK");
                Page current = App.Current.MainPage;

                while (App.Current.MainPage.Navigation.ModalStack.Count > 0)
                {
                   await App.Current.MainPage.Navigation.PopModalAsync();
                };

                /*
                if (needToSignUp)
                {
                    Debug.WriteLine("Purchase_Page -> have cats, but need to sign up, go to sign up page");
                    await App.Current.MainPage.Navigation.PushModalAsync(new SignUp_Page(true));
                }
                */

                Device.BeginInvokeOnMainThread(() =>
                {
                    App.Current.MainPage = new MainPage_(categoryResponse.result[0].viewType, 0, needToSignUp);
                    current.Navigation.RemovePage(current);
                });

            }
            else
            { // download categories...
                if (!CrossConnectivity.Current.IsConnected || !DownloadHelper.DownloadHelper.CheckInternetConnectionProgressive())
                { // check connectivity
                    AsyncMessages.CheckConnectionTimeout();
                    await App.Current.MainPage.DisplayAlert(POP_UP.OOPS, POP_UP.NO_CONNECTION, POP_UP.OK);
                    //("Error!", "No internet connection!", "ОK");
                    IsAnimating = false;
                    return;
                }
                var postData = $"cat={cat_id}";
                CategoryResponse categoryResponse = await ServerApi.PostRequestProgressive<CategoryResponse>(ServerApi.CATS_URL, postData, null);
                Debug.WriteLine("Subscribe Page -> categoryResponse = " + JsonConvert.SerializeObject(categoryResponse));
                if (categoryResponse != null && categoryResponse.result != null && categoryResponse.result.Length > 0)
                {
                    if (categoryResponse.error == null)
                    {
                        categoryResponse.ReorderForGame(UserHelper.Lang_cat);

                        await CacheHelper.Add(CacheHelper.CATEGORYS_RESPONSE + cat_id, categoryResponse);

                        if (App.DEBUG) await App.Current.MainPage.DisplayAlert("DEBUG", "we downloaded the categories -> go to main page", "ОK");
                        Page current = App.Current.MainPage;

                        while (App.Current.MainPage.Navigation.ModalStack.Count > 0)
                        {
                            await App.Current.MainPage.Navigation.PopModalAsync();
                        };
                        /*
                        if (needToSignUp)
                        {
                            Debug.WriteLine("Purchase_Page -> downloaded cats, but need to sign up, go to sign up page");
                            await App.Current.MainPage.Navigation.PushModalAsync(new SignUp_Page(true));
                        }
                        */

                        Device.BeginInvokeOnMainThread(() =>
                        {
                            App.Current.MainPage = new MainPage_(categoryResponse.result[0].viewType, 0, needToSignUp);
                            current.Navigation.RemovePage(current);
                        });                                              
                        
                    }
                    else
                    {
                        Analytics.SendResultsRegular("Purchase_Page", categoryResponse, categoryResponse?.error, ServerApi.CATS_URL, postData);
                        await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                        POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(categoryResponse?.error, ERROR_PREFIX + 1), POP_UP.OK);
                        //("Error in categories!", categoryResponse.error.message, "ОK");
                    }
                }
                else
                {
                    Analytics.SendResultsRegular("Purchase_Page", categoryResponse, categoryResponse?.error, ServerApi.CATS_URL, postData);
                    await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                        POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(categoryResponse?.error, ERROR_PREFIX + 2), POP_UP.OK);
                    //("Error!", "Can not get categories from server!", "ОK");
                }
            }
            IsAnimating = false;
        }

        private async void MenuButton_Tapped(object sender, System.EventArgs e)
        {
            Debug.WriteLine("SignUp_Page -> MenuButton_Tapped");
            if (IsAnimating) return;
            View view = sender as View;
            await AnimateView(view, 250);

            if (view.ClassId == "CloseBtn")
            {
                await App.Current.MainPage.Navigation.PopModalAsync();

            }
            else if (view.ClassId == "TermsOfUse")
            {
                Debug.WriteLine("Purchase_Page -> MenuButton_Tapped -> TermsOfUse");
                Device.OpenUri(new System.Uri("https://wp.dinolingo.com/terms/"));
            }
            else if (view.ClassId == "PrivacyPolicy")
            {
                Debug.WriteLine("Purchase_Page -> MenuButton_Tapped -> PrivacyPolicy");
                Device.OpenUri(new System.Uri("https://wp.dinolingo.com/privacy/"));
            }
            else if (view.ClassId == "PurchaseBtn")
            {
                Debug.WriteLine("Purchase_Page -> MenuButton_Tapped -> PurchaseBtn");
                // Check connection 
                
                Debug.WriteLine("Purchase_Page -> MenuButton_Tapped() -> Check connection");
                if (!CrossConnectivity.Current.IsConnected || !DownloadHelper.DownloadHelper.CheckInternetConnectionProgressive())
                { // check connectivity
                    AsyncMessages.CheckConnectionTimeout();
                    Debug.WriteLine("Purchase_Page -> MenuButton_Tapped() -> No internet connection!");
                    await App.Current.MainPage.DisplayAlert(POP_UP.OOPS, POP_UP.NO_CONNECTION, POP_UP.OK);
                    //("Error!", "No internet connection!", "ОK");                        
                }
                else if (!Purchaser.IsPurchasing)
                {
                    if (Purchaser.IsRestoring)
                    {
                        await App.Current.MainPage.DisplayAlert(Translate.GetString("purchase_try_again"),
                            Translate.GetString("purchase_sorry_restoring_subscriptions"), POP_UP.OK);
                    }
                    else
                    {
                        Debug.WriteLine("Purchase_Page -> OnItemTapped() -> !Purchaser.IsPurchasing");
                        Purchaser.Purchase(langCat, termIndex);
                    }                    
                }                
            }

            IsAnimating = false;
        }

        Task AnimateView(View view, uint time)
        {
            IsAnimating = true;
            return Task.Run(async () =>
            {
                await view.ScaleTo(0.8, time / 2);
                await view.ScaleTo(1.0, time / 2);
                return;
            });
        }
    }
}