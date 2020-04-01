using DinoLingo.ScreenOrientations;
using Newtonsoft.Json;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DinoLingo
{

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Subscribe_Page : ContentPage
	{
        public ObservableCollection<ChooseLanguage_ViewModel.LangItem> ListItemSource { get; set; }
        public bool IsListEnabled { get; set; }
        public double DinoSize { get; set; }
        public Thickness ListPadding { get; set; }
        Thickness listItemsMargin;

        public double HeaderFontSize { get; set; }
        public double BigCornerRadius { get; set; }

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

        string[] plans_ = new string[] { "6", "12", "1" };
        int term_index = 2;
        //string[] prices; // = new string[] {"$98.99 for 6 months", "$159.99 for 1 year" };
        //string[] month_prices;// = new string[] { "$16.50 a month", "$13.30 a month" };
        
        
        
        

        public Subscribe_Page ()
		{
			InitializeComponent ();
            BindingContext = this;

            FlowDirection = Translate.FlowDirection_;
            mainGrid.Padding = UI_Sizes.MainMargin;
            shadowFrame.TranslationX = UI_Sizes.BigFrameShadowTranslationX;
            shadowFrame.TranslationY = UI_Sizes.BigFrameShadowTranslationY;
            CloseBtn.WidthRequest = CloseBtn.HeightRequest = UI_Sizes.CloseBtnSize;

            DinoSize = UI_Sizes.ScreenHeightX * 0.5;
            subscriptonPlan.FontSize = UI_Sizes.MediumTextSize * 1.0;
            HeaderFontSize = UI_Sizes.SmallTextSize * 0.9;
            BigCornerRadius = UI_Sizes.BigFrameCornerRadius;
            ListPadding = new Thickness(BigCornerRadius * 0.27, BigCornerRadius * 0.35, BigCornerRadius * 0.27, BigCornerRadius * 0.35);
            listItemsMargin = new Thickness(8, 4, 8, 4);

            // prices

            // string sixMonthsPrise, yearPrice, monthlyPriceFor6Months, monthlyPriceForYear;
            string price;
            if (Device.RuntimePlatform == Device.iOS)
            {
                price = "19.99";
                /*
                sixMonthsPrise = "98.99";
                yearPrice = "159.99";
                monthlyPriceFor6Months = "16.50";
                monthlyPriceForYear = "13.30";
                */
            }
            else
            {
                price = "19.99";
                /*
                sixMonthsPrise = "99.99";
                yearPrice = "159.99";
                monthlyPriceFor6Months = "16.70";
                monthlyPriceForYear = "13.30";
                */
            }

            /*
            prices = new string[] {
                string.Format(Translate.GetString("subscribe_price_for_6_months"), sixMonthsPrise),
                string.Format(Translate.GetString("subscribe_price_for_1_year"), yearPrice),  };

            
            month_prices = new string[] {
                string.Format(Translate.GetString("subscribe_price_a_month"), monthlyPriceFor6Months),
                 string.Format(Translate.GetString("subscribe_price_a_month"), monthlyPriceForYear),
            };
            */


            SetPriceButtons();
            
            CreateProducts();
        }

        private async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (IsAnimating) return;
            IsAnimating = true;
            if (e.Item != null)
            {
                Debug.WriteLine("Subscribe_Page -> Tapped :" + (e.Item as ChooseLanguage_ViewModel.LangItem).LanguageName + ", product_id:" + (e.Item as ChooseLanguage_ViewModel.LangItem).product_id);
                ChooseLanguage_ViewModel.LangItem item = e.Item as ChooseLanguage_ViewModel.LangItem;

                string product_id = LANGUAGES.CAT_INFO[item.cat_id].ProductsIds[term_index];
                //string productText = Purchaser.GetProductNameByCatAndId(item.cat_id, product_id);
                if (UserHelper.HaveProductId(product_id))
                {
                    Debug.WriteLine("Subscribe_Page -> OnItemTapped(), product_id = " + product_id + "you have such a product");
                    await App.Current.MainPage.DisplayAlert("", Translate.GetString("subscribe_you_have_access_to_subscription"), POP_UP.OK);
                }
                else
                {
                    await App.Current.MainPage.Navigation.PushModalAsync(new Purchase_Page(item.cat_id, term_index));
                }
            }
            IsAnimating = false;
        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            
            Debug.WriteLine("Subscribe_Page -> OnItemSelected ...");
            if (e.SelectedItem != null)
            {
                ((ListView)sender).SelectedItem = null;
            }
        }

        void SetPriceButtons()
        {
            // set header
            //subscriptonPlan.Text = (term_index == 0) ? Translate.GetString("subscribe_six_months_subscriptions") : Translate.GetString("1 year subscriptions");
            subscriptonPlan.Text =  Translate.GetString("subscribe_one_month_subscriptions");
        }

        void CreateProducts()
        {
            Debug.WriteLine("Subscribe_Page -> SetProducts()");

            int maxLenght = 15;
            double lenghtCoeff = 0.7;

            ObservableCollection<ChooseLanguage_ViewModel.LangItem> items = new ObservableCollection<ChooseLanguage_ViewModel.LangItem>();
            List<string> all_lang_cats = new List<string>(LANGUAGES.CAT_INFO.Keys);

            if (UserHelper.Login.products != null && UserHelper.Login.products.Length > 0)
            {
                foreach (Login_Response.Product p in UserHelper.Login.products)
                {
                    string lang_cat = p.cat_id.ToString();
                    if (all_lang_cats.Contains(lang_cat))
                    {
                        all_lang_cats.Remove(lang_cat);
                        string name = LANGUAGES.CAT_INFO[lang_cat].GetVisibleName();
                        items.Add(new ChooseLanguage_ViewModel.LangItem
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

            if (UserHelper.Login.IAPproducts != null && UserHelper.Login.IAPproducts.Length > 0)
            {
                foreach (Login_Response.Product p in UserHelper.Login.IAPproducts)
                {
                    string lang_cat = p.cat_id.ToString();
                    if (all_lang_cats.Contains(lang_cat))
                    {
                        all_lang_cats.Remove(lang_cat);
                        string name = LANGUAGES.CAT_INFO[lang_cat].GetVisibleName();
                        items.Add(new ChooseLanguage_ViewModel.LangItem
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


            foreach (string lang_cat in all_lang_cats)
            {
                string name = LANGUAGES.CAT_INFO[lang_cat].GetVisibleName();
                items.Add(new ChooseLanguage_ViewModel.LangItem
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

        void UpdateProducts()
        {
            Debug.WriteLine("Subscribe_Page -> UpdateProducts()");
            
            for (int i = 0; i < ListItemSource.Count; i++)
            {
                ListItemSource[i].CellColor = 
                    UserHelper.HaveProductId(LANGUAGES.CAT_INFO[ListItemSource[i].cat_id].ProductsIds[1]) ||
                    UserHelper.HaveProductId(LANGUAGES.CAT_INFO[ListItemSource[i].cat_id].ProductsIds[2]) ||
                    UserHelper.HaveProductId(LANGUAGES.CAT_INFO[ListItemSource[i].cat_id].ProductsIds[0]) 
                    ? BACKGROUND_PURCHASED 
                    : BACKGROUND_NOT_PURCHASED;                              
            }
        }

        private async void MenuButton_Tapped(object sender, System.EventArgs e)
        {
            Debug.WriteLine("Subscribe_Page -> MenuButton_Tapped");
            if (IsAnimating) return;
            View view = sender as View;
            await AnimateImage(view, 250);

            if (view.ClassId == "CloseBtn")
            {               
                await App.Current.MainPage.Navigation.PopModalAsync();
            }
            else if (view.ClassId == "DinoLingoLink")
            {
                Debug.WriteLine("Subscribe_Page -> MenuButton_Tapped");
                Device.OpenUri(new System.Uri("https://dinolingo.com/"));
            }
            else if (view.ClassId == "PriceBtn")
            {
                // switch price
                term_index++;
                if (term_index >= plans_.Length) term_index = 0;
                SetPriceButtons();

                UpdateProducts();
            }
            
            
            IsAnimating = false;
        }

       

        Task AnimateImage(View view, uint time)
        {
            IsAnimating = true;
            return Task.Run(async () =>
            {
                await view.ScaleTo(0.8, time / 2);
                await view.ScaleTo(1.0, time / 2);
                return;
            });
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Send((ContentPage)this, "ForcePortrait");
            ScreenOrientation.Instance.ForcePortrait();
        }

    }    

    
}

            
