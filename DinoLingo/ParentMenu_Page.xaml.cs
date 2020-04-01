using DinoLingo.ScreenOrientations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace DinoLingo
{
    public partial class ParentMenu_Page : ContentPage
    {
        public static bool IsBlocked;

        public double TextSize { get; set; }

        public float BtnCornerRadius { get; set; }
        public float FrameCornerRadius { get; set; }
        public double ShadowTranslation { get; set; }
        public double BtnOutlineWidth { get; set; }
        public double BtnHeight { get; set; }
        public double BtnWidth { get; set; }

        public Thickness ButtonShadowPadding { get; set; }

        bool IsAnimating = false;
        bool rateWidgetShown = false;

        public ParentMenu_Page()
        {
            IsBlocked = false;

            InitializeComponent();
            BindingContext = this;
           
            CloseBtn.WidthRequest = CloseBtn.HeightRequest = UI_Sizes.CloseBtnSize;
            mainGrid.Margin = UI_Sizes.MainMarginPortrait;
            FrameCornerRadius = UI_Sizes.BigFrameCornerRadius;
            BtnHeight = UI_Sizes.ScreenHeightX_UNIFORMED_TO_1_78 * 0.12;
            BtnWidth = BtnHeight * 5;
            TextSize = BtnHeight * 0.3;
            BtnCornerRadius = UI_Sizes.ButtonCornerRadius;
            BtnOutlineWidth = UI_Sizes.ButtonOutlineWidth;
            Debug.WriteLine("BtnOutlineWidth = " + BtnOutlineWidth);
            Debug.WriteLine("BtnCornerRadius = " + BtnCornerRadius);

            ButtonShadowPadding = UI_Sizes.ButtonShadowPadding;
            shadowFrame.TranslationX = UI_Sizes.BigFrameShadowTranslationX;
            shadowFrame.TranslationY = UI_Sizes.BigFrameShadowTranslationY;
            shadowFrame.WidthRequest = whiteFrame.WidthRequest = BtnWidth + 5;
            shadowFrame.HeightRequest = whiteFrame.HeightRequest = BtnHeight * 7;
            dinoImage.WidthRequest = BtnWidth * 0.4;

            label_course.FontSize = UI_Sizes.SmallTextSize;
            label_course.Text = string.Format(Translate.GetString("header_for_kids"), LANGUAGES.CAT_INFO[UserHelper.Lang_cat].VisibleName).FirstLetterToUpperCase();
            label_userName.FontSize = UI_Sizes.SmallTextSize;
                       
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            SetUserState();

            MessagingCenter.Send((ContentPage)this, "ForcePortrait");
            ScreenOrientation.Instance.ForcePortrait();

            // check RateWidget here
            if (!rateWidgetShown)
            {
                rateWidgetShown = true;
                RateWidget.CheckRateWidget(App.Current.MainPage.Navigation, this);
            }                
        }

        public void SetUserState()
        {
            if (UserHelper.Login == null || string.IsNullOrEmpty(UserHelper.Login.user_id))
            {
                loginLabel.Text = Translate.GetString("login_login");
                //CouponsBtn.IsVisible = CouponsBtn.IsEnabled = false;
            }
            else
            {
                loginLabel.Text = Translate.GetString("parent_menu_log_out");
                //CouponsBtn.IsVisible = CouponsBtn.IsEnabled = true;                
                label_userName.Text = UserHelper.Login.user_login;
            }
        }

      

        public ICommand PrivacyPolicyCommand => new Command<string>((url) =>
        {
            App.Current.MainPage.Navigation.PopModalAsync();
            Device.OpenUri(new System.Uri(url));
            
        });

        private async void MenuButton_Tapped(object sender, System.EventArgs e)
        {
            if (IsBlocked) return;

            Debug.WriteLine("MenuButton_Tapped");
            if (IsAnimating) return;
            View view = sender as View;
            await AnimateImage(view, 250);
            if (view.ClassId == "CloseBtn")
            {

                await App.Current.MainPage.Navigation.PopModalAsync();
            }
            else if (view.ClassId == "DinoLingoLink")
            {
                Debug.WriteLine("ParentMenu_Page -> MenuButton_Tapped");
                Device.OpenUri(new System.Uri("https://dinolingo.com/"));
            }
            else if (view.ClassId == "LoginBtn")
            {
                
                if (UserHelper.Login != null && !string.IsNullOrEmpty(UserHelper.Login.user_id))
                {
                    bool answer = true;
                    answer = await App.Current.MainPage.DisplayAlert(Translate.GetString("parent_menu_log_out_ex"),
                        Translate.GetString("parent_menu_do_you_want_to_log_out"), POP_UP.YES, POP_UP.NO);

                    if (answer)
                    {
                        Login_Response.Product[] oldIAPs = UserHelper.Login.IAPproducts; 
                        UserHelper.Login = null;
                        UserHelper.CreateFreeUser();
                        UserHelper.Login.IAPproducts = oldIAPs;

                        await CacheHelper.Add<Login_Response.Login>(CacheHelper.LOGIN, UserHelper.Login);

                        Page current = App.Current.MainPage;
                        App.Current.MainPage = new LoginPage_new();
                        current.Navigation.RemovePage(current);
                    }
                }
                else
                {
                    Login_Response.Product[] oldIAPs = UserHelper.Login.IAPproducts;
                    UserHelper.Login = null;
                    UserHelper.CreateFreeUser();
                    UserHelper.Login.IAPproducts = oldIAPs;

                    await CacheHelper.Add<Login_Response.Login>(CacheHelper.LOGIN, UserHelper.Login);

                    Page current = App.Current.MainPage;
                    App.Current.MainPage = new LoginPage_new();
                    current.Navigation.RemovePage(current);
                }                  

            }
            else if (view.ClassId == "MyReportBtn")
            {
                // check user
                if (UserHelper.Login == null || string.IsNullOrEmpty(UserHelper.Login.user_id))
                {
                    await App.Current.MainPage.DisplayAlert(Translate.GetString("parent_menu_sorry"),
                        Translate.GetString("parent_menu_please_login_to_access_report"), POP_UP.OK);
                }
                else
                {
                    DateTime start = DateTime.Now;
                    Page page = new ReportPage(App.Current.MainPage.Navigation);
                    TimeSpan delta = DateTime.Now - start;
                    double deltaTime = delta.TotalMilliseconds;
                    if (deltaTime < 200)
                    {
                        await Task.Delay(200 - (int)deltaTime);
                    }
                    await Task.WhenAll(
                       App.Current.MainPage.Navigation.PopModalAsync(),
                       App.Current.MainPage.Navigation.PushModalAsync(page));
                }

            }
            else if (view.ClassId == "ChangeLanguageBtn")
            {

               /*
                Page current = App.Current.MainPage;
               await App.Current.MainPage.Navigation.PopModalAsync();
               App.Current.MainPage = new ChooseLanguage_Page(UserHelper.Login);
               current.Navigation.RemovePage(current);
               */
                await App.Current.MainPage.Navigation.PushModalAsync(new ChooseLanguage_Page(UserHelper.Login));
                

            }

            else if (view.ClassId == "SignUpBtn")
            {
                await App.Current.MainPage.Navigation.PopModalAsync();

                // check current user

                if (UserHelper.Login != null && !string.IsNullOrEmpty(UserHelper.Login.user_id)) // we have registered user
                {
                    Debug.WriteLine("ParentMenu_Page - > signUp Tapped -> have user_id, go to subscribe page");
                    await App.Current.MainPage.Navigation.PushModalAsync(new Subscribe_Page());
                }
                else if (string.IsNullOrEmpty(UserHelper.Login.user_id) && UserHelper.TotalProducts() > 0) // we have no user_id, but we have some products
                {
                    Debug.WriteLine($"ParentMenu_Page - > signUp Tapped -> have no user_id, UserHelper.TotalProducts()= {UserHelper.TotalProducts()}, go to sign up page");
                    await App.Current.MainPage.Navigation.PushModalAsync(new SignUp_Page());
                }
                else
                { // we have no user_id, have no products
                    Debug.WriteLine($"ParentMenu_Page - > signUp Tapped -> have no user_id, UserHelper.TotalProducts()= {UserHelper.TotalProducts()}, go to subscribe page");
                    await App.Current.MainPage.Navigation.PushModalAsync(new Subscribe_Page());
                }
                

                //Device.OpenUri(new System.Uri("https://dinolingo.com/subscribe/"));
            }
            else if (view.ClassId == "HelpBtn")
            {
                await App.Current.MainPage.Navigation.PopModalAsync();
                Device.OpenUri(new System.Uri("https://dinolingo.com/help/"));
            }
            
            else if (view.ClassId == "PrivacyPolicyBtn")
            {
                App.Current.MainPage.Navigation.PopModalAsync();
                Device.OpenUri(new System.Uri("https://dinolingo.com/privacy/"));
            }
            else if (view.ClassId == "FeedbackBtn")
            {
                Debug.WriteLine("RateWidget -> rate dinolingo");
                RateWidget.State.remindLater = false;
                await CacheHelper.Add<RateWidget.RateState>(CacheHelper.RATE_STATE, RateWidget.State);
                await App.Current.MainPage.Navigation.PopModalAsync();
                
                
                if (Device.RuntimePlatform == Device.iOS) Device.OpenUri(new System.Uri(RateWidget.RATE_LINK_IOS));
                else if (Device.RuntimePlatform == Device.Android) Device.OpenUri(new System.Uri(RateWidget.RATE_LINK_ANDROID));
               
            }
            else if (view.ClassId == "FavoritesBtn")
            {
               

            }

            IsAnimating = false;
        }


        Task AnimateImage(View view, uint time)
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
            Debug.WriteLine("ParentMenu_Page -> Dispose");

            Content = null;
            BindingContext = null;            
        }
    }
}
