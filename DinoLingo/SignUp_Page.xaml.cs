using DinoLingo.ScreenOrientations;
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
    public partial class SignUp_Page : ContentPage
    {
        static int ERROR_PREFIX = 130;

        public double FrameCornerRadius { get; set; }
        public double SmallTextSize { get; set; }
        public double MicroTextSize { get; set; }
        public double BtnCornerRadius { get; set; }
        public double BtnOutlineWidth { get; set; }

        bool IsAnimating = false;
        bool justPurchased = false;

        private double _loginBtnWidthFactor = 420.0 / 125.0;
        private string _blueBtnRes = "DinoLingo.Resources.UI.btnblue_new.png";

        public SignUp_Page(bool justPurchased = false)
        {
            Debug.WriteLine("SignUp_Page() ->");
            this.justPurchased = justPurchased;
            InitializeComponent();
            FlowDirection = Translate.FlowDirection_;
            BindingContext = this;
            absMainLayout.Margin = UI_Sizes.MainMarginPortrait;            
            FrameCornerRadius = UI_Sizes.BigFrameCornerRadius;
            HeaderBig.FontSize = UI_Sizes.SmallTextSize * 1.2;
            HeaderSmall.FontSize = UI_Sizes.SmallTextSize * 0.8;
            SmallTextSize = UI_Sizes.SmallTextSize * 1.2;
            MicroTextSize = UI_Sizes.MicroTextSize;

            //mainFrame.WidthRequest = shadowFrame.WidthRequest = UI_Sizes.ScreenWidthX - UI_Sizes.LeftPadding - UI_Sizes.RightPadding - UI_Sizes.BigFrameShadowTranslationX;
            //mainFrame.HeightRequest = shadowFrame.HeightRequest = UI_Sizes.ScreenHeightX - 2 * UI_Sizes.TopBottomPadding - UI_Sizes.BigFrameShadowTranslationY;

            //Debug.WriteLine($"mainFrame.WidthRequest = {mainFrame.WidthRequest}, UI_Sizes.ScreenWidthX = {UI_Sizes.ScreenWidthX}");
            //shadowFrame.TranslationX = UI_Sizes.BigFrameShadowTranslationX;
            //shadowFrame.TranslationY = UI_Sizes.BigFrameShadowTranslationY;

            CloseBtn.WidthRequest = CloseBtn.HeightRequest = UI_Sizes.CloseBtnSize;
            BtnCornerRadius = UI_Sizes.ButtonCornerRadius;
            BtnOutlineWidth = UI_Sizes.ButtonOutlineWidth;
            //BtnShadow.TranslationY = UI_Sizes.ButtonShadowTranslationY;
            fieldsGrid.WidthRequest = UI_Sizes.ScreenHeightX_UNIFORMED_TO_1_78 * 0.75;
            fieldsGrid.HeightRequest = fieldsGrid.WidthRequest * 0.22 * 3;


            passEntry.WidthRequest = passConfirmEntry.WidthRequest =  emailEntry.WidthRequest = UI_Sizes.ScreenHeightX_UNIFORMED_TO_1_78 * 0.7;

            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += MenuButton_Tapped;

            // sign-up button
            MyViews.ButtonWithImage signUpBtn = new MyViews.ButtonWithImage(
                "SignUpBtn",
                new Rectangle(0.5, 0, UI_Sizes.ButtonHeight * _loginBtnWidthFactor, UI_Sizes.ButtonHeight),
                _blueBtnRes,
                Translate.GetString("login_sign_up"),
                UI_Sizes.SmallTextSize,
                Color.White,
                tapGestureRecognizer,
                AbsoluteLayoutFlags.PositionProportional
                );
            mainStack.Children.Add(signUpBtn);

        }

        private async void MenuButton_Tapped(object sender, System.EventArgs e)
        {
            Debug.WriteLine("SignUp_Page -> MenuButton_Tapped");
            if (IsAnimating) return;
            View view = sender as View;
            await AnimateImage(view, 250);

            if (view.ClassId == "CloseBtn")
            {
                // check if signed up
                var answ = await App.Current.MainPage.DisplayAlert(Translate.GetString("sign_up_are_u_sure"),
                    Translate.GetString("sign_up_without_registration_popup"), 
                    Translate.GetString("parent_menu_sign_up"), Translate.GetString("sign_up_do_not_signup"));

                if (!answ) {
                    await App.Current.MainPage.Navigation.PopModalAsync();
                }  

            }
            else if (view.ClassId == "SignUpBtn")
            {
                Debug.WriteLine("SignUp_Page -> SignUpBtn -> ");
               
                string s = CheckAllFields();
                if (string.IsNullOrEmpty(s)) {
                    if (!CrossConnectivity.Current.IsConnected || !DownloadHelper.DownloadHelper.CheckInternetConnectionProgressive())
                    { // check connectivity
                        Debug.WriteLine("SignUp_Page -> check connectivity - no connection");                        
                        await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                        POP_UP.NO_CONNECTION
                        //+ POP_UP.GetCode(null, ERROR_PREFIX + 1)
                        , POP_UP.OK);
                        //("Error!", "No internet connection!", "ОK");
                    }
                    else
                    {
                        Debug.WriteLine("SignUp_Page -> check new user");
                        // check new user
                        var postData = $"email={emailEntry.Text}";
                        postData += $"&pass={passEntry.Text}";
                        postData += $"&fname={string.Empty}"; //{fnameEntry.Text}";
                        postData += $"&lname={string.Empty}"; //{lnameEntry.Text}";

                        NewUserResponse newUserResponse = await ServerApi.PostRequestProgressive<NewUserResponse>(ServerApi.NEW_USER_URL, postData, null, 7000);
                        // process response
                        if (newUserResponse == null)
                        {
                            Analytics.SendResultsRegular("SignUp_Page", newUserResponse, newUserResponse?.error, ServerApi.NEW_USER_URL, postData);
                            Debug.WriteLine("SignUp_Page -> check new user -> newUserResponse == null");
                            if (AsyncMessages.CheckDisplayAlertTimeout())
                            {
                                await App.Current.MainPage.DisplayAlert(Translate.GetString("sign_up_cant_create_new_user"),
                                POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(newUserResponse?.error, ERROR_PREFIX + 2), POP_UP.OK);
                            }
                            
                        }
                        else if (newUserResponse.error != null)
                        {
                            Analytics.SendResultsRegular("SignUp_Page", newUserResponse, newUserResponse?.error, ServerApi.NEW_USER_URL, postData);
                            Debug.WriteLine("SignUp_Page -> check new user -> newUserResponse.error != null");

                            // have some response from server, try to login here
                            // check login
                            Debug.WriteLine("SignUp_Page -> check login");
                            postData = $"login={emailEntry.Text}";
                            postData += $"&pass={passEntry.Text}";
                            Login_Response login_Response = await ServerApi.PostRequestProgressive<Login_Response>(ServerApi.LOGIN_URL, postData, null);
                            // process response
                            if (login_Response == null || login_Response.error?.message != null || login_Response.result == null)
                            {
                                // have some error here
                                await App.Current.MainPage.DisplayAlert(Translate.GetString("sign_up_cant_create_new_user"), "Message: " + newUserResponse.error.message, POP_UP.OK);
                            }                            
                            else // we finally OK with user
                            {
                                // cashe login here
                                Debug.WriteLine("SignUp_Page -> check login AFTER FAIL TO SIGN UP -> we finally OK with user");
                                login_Response.result.DeleteDuplicateProducts();
                                UserHelper.Login = login_Response.result;

                                // if we have internet - update IAP
                                if (CrossConnectivity.Current.IsConnected && DownloadHelper.DownloadHelper.CheckInternetConnectionProgressive())
                                { // check connectivity
                                    Debug.WriteLine("Sign_Up -> new user -> try to restore IAP");
                                    bool result = await Purchaser.Restore();
                                    UserHelper.SetIsFree();
                                    Debug.WriteLine("Sign_Up -> new user -> try to restore IAP for user: " + UserHelper.Login.user_id + ", restored result: " + result);
                                }

                                await CacheHelper.Add(CacheHelper.LOGIN, UserHelper.Login, TimeSpan.FromSeconds(100));

                                //if (App.DEBUG) await App.Current.MainPage.DisplayAlert("Great!", fnameEntry.Text + ", " + "Welcome to DinoLingo!", "Continue");
                                
                                // GO AFTER SIGN UP
                                Debug.WriteLine("Sign_Up -> AFTER SIGN UP -> choose page and go");
                                // Get categories and launch next page
                                ProcessLogin(login_Response);                                
                            }                                                       
                        }
                        else if (newUserResponse.result == null)
                        {
                            Analytics.SendResultsRegular("SignUp_Page", newUserResponse, newUserResponse?.error, ServerApi.NEW_USER_URL, postData);
                            Debug.WriteLine("SignUp_Page -> check new user -> newUserResponse.result == null");
                            await App.Current.MainPage.DisplayAlert(Translate.GetString("sign_up_cant_create_new_user"), 
                                POP_UP.GetCode(newUserResponse?.error, ERROR_PREFIX + 3), POP_UP.OK);
                        }
                        else
                        { // new_user - OK! -> try to login

                            // check login
                            Debug.WriteLine("SignUp_Page -> check login");
                            postData = $"login={emailEntry.Text}";
                            postData += $"&pass={passEntry.Text}";
                            Login_Response login_Response = await ServerApi.PostRequestProgressive<Login_Response>(ServerApi.LOGIN_URL, postData, null);
                            // process response
                            if (login_Response == null)
                            {
                                Analytics.SendResultsRegular("SignUp_Page", login_Response, login_Response?.error, ServerApi.LOGIN_URL, postData);
                                Debug.WriteLine("SignUp_Page -> check login -> login_Response == null");
                                if (AsyncMessages.CheckDisplayAlertTimeout())
                                {
                                    await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                                    POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(login_Response?.error, ERROR_PREFIX + 4), POP_UP.OK);
                                    //("Error!", "Error in the response from server!", "ОK");                                
                                }

                            }
                            else if (login_Response.error?.message != null)
                            {
                                Analytics.SendResultsRegular("SignUp_Page", login_Response, login_Response?.error, ServerApi.LOGIN_URL, postData);
                                Debug.WriteLine("SignUp_Page -> check login -> login_Response.error != null");

                                await App.Current.MainPage.DisplayAlert(POP_UP.OOPS, "Message: " + login_Response.error.message, POP_UP.OK);
                                
                            }
                            else if (login_Response.result == null)
                            {
                                Analytics.SendResultsRegular("SignUp_Page", login_Response, login_Response?.error, ServerApi.LOGIN_URL, postData);
                                Debug.WriteLine("SignUp_Page -> check login -> login_Response.result == null");
                                if (AsyncMessages.CheckDisplayAlertTimeout())
                                {
                                    await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                                                                        POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(login_Response?.error, ERROR_PREFIX + 5), POP_UP.OK);
                                }
                                    
                                //("Error!", "Login result is null!", "ОK");
                            }
                            else // we finally OK with user
                            {
                                // cashe login here
                                Debug.WriteLine("SignUp_Page -> check login -> we finally OK with user");

                                login_Response.result.DeleteDuplicateProducts();
                                UserHelper.Login = login_Response.result;
                                

                                // if we have internet - update IAP
                                if (CrossConnectivity.Current.IsConnected && DownloadHelper.DownloadHelper.CheckInternetConnectionProgressive())
                                { // check connectivity
                                    Debug.WriteLine("Sign_Up -> new user -> try to restore IAP");
                                    bool result = await Purchaser.Restore();
                                    UserHelper.SetIsFree();
                                    Debug.WriteLine("Sign_Up -> new user -> try to restore IAP for user: " + UserHelper.Login.user_id + ", restored result: " + result);
                                }



                                await CacheHelper.Add(CacheHelper.LOGIN, UserHelper.Login, TimeSpan.FromSeconds(100));

                                //if (App.DEBUG) await App.Current.MainPage.DisplayAlert("Great!", fnameEntry.Text + ", " + "Welcome to DinoLingo!", "Continue");


                                // GO AFTER SIGN UP
                                Debug.WriteLine("Sign_Up -> AFTER SIGN UP -> choose page and go");

                                // Get categories and launch next page
                                ProcessLogin(login_Response);                                
                            }        
                            
                        }
                    }
                }
                else
                {
                    // some error in fields
                    await App.Current.MainPage.DisplayAlert(Translate.GetString("sign_up_error"), s, POP_UP.OK);
                }

            }

            IsAnimating = false;
        }

        async void ProcessLogin(Login_Response login)
        {
            Debug.WriteLine("Sign_Up -> process login");
            try
            {
                // process amount of products                 
                int totalProducts = UserHelper.TotalProducts();
                Debug.WriteLine("Login -> ProcessLogin -> totalProducts = " + totalProducts);  

                if (totalProducts == 0)
                { // 
                    Debug.WriteLine("Login -> ProcessLogin -> ");
                    await App.Current.MainPage.DisplayAlert(Translate.GetString("login_info"),
                        Translate.GetString("login_have_no_products"), POP_UP.OK);
                    // continue free                    
                    await App.Current.MainPage.Navigation.PushModalAsync(new ChooseLanguage_Page(UserHelper.Login));
                }
                else if (totalProducts == 1)
                { // only 1 product
                  //add current language 
                    Debug.WriteLine("Login -> ProcessLogin -> totalProducts == 1 ");
                    UserHelper.Lang_cat = UserHelper.GetSingleProduct(login).cat_id.ToString();
                    UserHelper.Language = LANGUAGES.CAT_INFO[UserHelper.GetSingleProduct(login).cat_id.ToString()].Name;
                    UserHelper.SetIsFree();

                    await CacheHelper.Add(CacheHelper.CURRENT_LANGUAGE, UserHelper.Language);
                    await CacheHelper.Add(CacheHelper.CURRENT_LANGUAGE_CAT, UserHelper.Lang_cat);

                    GameHelper.memory_GameObjects = await CacheHelper.GetAsync<GameObjects>(CacheHelper.MEMORY_GAMEOBJECTS + UserHelper.Lang_cat);
                    GameHelper.sas_GameObjects = await CacheHelper.GetAsync<GameObjects>(CacheHelper.SAS_GAMEOBJECTS + UserHelper.Lang_cat);

                    await Favorites.Refresh(string.Empty, string.Empty, true, UserHelper.Lang_cat, UserHelper.Login.user_id, UserHelper.IsFree);
                    if (!UserHelper.IsFree) Favorites.StartLoading();

                    GetCategoriesAndLaunchNextPage(UserHelper.Lang_cat);
                }
                else
                { // > 1 products               

                    /*
                   Page current = App.Current.MainPage;
                   App.Current.MainPage = new ChooseLanguage_Page(UserHelper.Login);
                   navigation.RemovePage(current);
                   */
                    await App.Current.MainPage.Navigation.PushModalAsync(new ChooseLanguage_Page(UserHelper.Login));
                }


            }
            catch (Exception ex)
            {
                Debug.WriteLine("Sign_Up -> ProcessLogin -> ex:" + ex.Message);
            }             
        }

        async void GetCategoriesAndLaunchNextPage(string cat_id)
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
                    App.Current.MainPage = new MainPage_(categoryResponse.result[0].viewType, 0, false);
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
                            App.Current.MainPage = new MainPage_(categoryResponse.result[0].viewType, 0, false);
                            current.Navigation.RemovePage(current);
                        });                        

                    }
                    else
                    {
                        Analytics.SendResultsRegular("Sign_Up_Page", categoryResponse, categoryResponse?.error, ServerApi.CATS_URL, postData);
                        await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                        POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(categoryResponse?.error, ERROR_PREFIX + 1), POP_UP.OK);
                        //("Error in categories!", categoryResponse.error.message, "ОK");
                    }
                }
                else
                {
                    Analytics.SendResultsRegular("Sign_Up_Page", categoryResponse, categoryResponse?.error, ServerApi.CATS_URL, postData);
                    await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                        POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(categoryResponse?.error, ERROR_PREFIX + 2), POP_UP.OK);
                    //("Error!", "Can not get categories from server!", "ОK");
                }
            }            
        }

        string  CheckAllFields()
        {
            string checkResult = string.Empty;
            // check e-mail

            
            if (string.IsNullOrEmpty(emailEntry.Text) || !emailEntry.Text.Contains('@') || emailEntry.Text.Contains(' ') ||  emailEntry.Text.Length < 3)
            {
                Debug.WriteLine("SignUp_Page -> incorrect e-mail!");
                checkResult = Translate.GetString("login_please_enter_correct_email");
                return checkResult;
            }            
            emailEntry.Text = emailEntry.Text.Replace(" ", string.Empty);

            // check fname
            /*
            if (string.IsNullOrEmpty(fnameEntry.Text) ||fnameEntry.Text.Length < 1 || fnameEntry.Text.Contains(' '))
            {
                Debug.WriteLine("SignUp_Page -> incorrect fnamel!");
                checkResult = "Please, enter correct first name.";
                return checkResult;
            }
            fnameEntry.Text = fnameEntry.Text.Replace(" ", string.Empty);

    */

            // last fname

            // check password
            if (string.IsNullOrEmpty(passEntry.Text) || string.IsNullOrEmpty(passConfirmEntry.Text) || passEntry.Text != passConfirmEntry.Text)
            {
                Debug.WriteLine("SignUp_Page -> incorrect pass!");
                checkResult = Translate.GetString("sign_up_different_passwords");
                return checkResult;
            }

            return checkResult;
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
            // Debug.WriteLine($"SignUp_Page -> mainFrame.WidthRequest = {mainFrame.WidthRequest}, mainFrame.Width = {mainFrame.Width}");
        }
    }
}