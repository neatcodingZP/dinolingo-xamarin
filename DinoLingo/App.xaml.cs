using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using SQLite;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using FFImageLoading;
using Plugin.Connectivity;
using Plugin.DeviceInfo;
using System.Globalization;
using System.Resources;
using HtmlAgilityPack;
using Plugin.FirebasePushNotification;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace DinoLingo
{
    public partial class App : Application
    {
        public static bool IsLandscape = false;
        

        // ===
        public static event Action OnSleepEvent;
        public static event Action OnResumeEvent;

        // ===
        static LocalDatabase database;

        public static LocalDatabase Database
        {
            get
            {
                if (database == null)
                {
                    if (DEBUG) Debug.WriteLine("App : Application --> database == null -->  database = new LocalDatabase()");
                    database = new LocalDatabase(
                      Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DinoLingoSQLite.db3"));
                }
                if (DEBUG) Debug.WriteLine("App : Application --> database --> return");
                return database;
            }
        }

        // apps audio
        static MyAudio audio;
        public static MyAudio Audio
        {
            get
            {
                if (audio == null)
                {
                    audio = new MyAudio();
                }
                return audio;
            }
        }


        // have notification
        public static bool haveNewNotification = false;

        // debug mode
        public static readonly bool DEBUG = false;

        static WeakReference _weak;

        public App()
        {
            InitializeComponent();
            var config = new FFImageLoading.Config.Configuration
            {
                TryToReadDiskCacheDurationFromHttpHeaders = false,

            };
            ImageService.Instance.Initialize(config);

            //***var navPage = new NavigationPage(new SplashPage());
            var navPage = new SplashPage_new();

            //***NavigationPage.SetHasNavigationBar(navPage.CurrentPage, false);

            Translate.Init();

            MainPage = navPage;


            //ImageService.Instance.Config.DiskCacheDuration = TimeSpan.FromDays(180);
            //ImageService.Instance.Config.TryToReadDiskCacheDurationFromHttpHeaders = false;

            //MainPage = new SplashPage();

            /*
            foreach (KeyValuePair<string, LANGUAGES.LangInfo> keyValuePair in LANGUAGES.CAT_INFO) 
            {
                Debug.WriteLine($"{keyValuePair.Value.Name.ToLower().Replace("-", "_")}_{Purchaser.TermIndexToId(2)}_{keyValuePair.Value.ProductsIds[2]}");
            }
            */

            /*
            var ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            Debug.WriteLine("App(), ci = " + ci.Name);
            */
        }

        async void ChooseStartPage()
        {
            // get AppId
            Debug.WriteLine("App.cs -> ChooseStartPage -> AppId");
            CrossDeviceInfo.Current.GenerateAppId();
            Debug.WriteLine($"Id= {CrossDeviceInfo.Current.Id}, IsDevice= {CrossDeviceInfo.Current.IsDevice}, AppVersion= {CrossDeviceInfo.Current.AppVersion}, Platform= {CrossDeviceInfo.Current.Platform}");

            // check RateState
            if (await CacheHelper.Exists(CacheHelper.RATE_STATE))
            {
                RateWidget.State = await CacheHelper.GetAsync<RateWidget.RateState>(CacheHelper.RATE_STATE);
            }
            else
            {
                RateWidget.State = new RateWidget.RateState { openedParentMenu = 0, remindLater = true, justPurchased = 0, }; 
            }

            // STEP -1- check if we have cached login ?
            Login_Response.Login login;
            if (await CacheHelper.Exists(CacheHelper.LOGIN))
            {
                Debug.WriteLine("App.cs -> ChooseStartPage -> STEP -1- -> have cached LOGIN");
                login = await CacheHelper.GetAsync<Login_Response.Login>(CacheHelper.LOGIN);
            }
            else
            {
                Debug.WriteLine("App.cs -> ChooseStartPage -> STEP -1- -> have NO cached LOGIN -> create FreeUser");
                login = UserHelper.CreateFreeUser().result;
            }

            // STEP - 1-a - check if we have null cached login
            if (login == null) login = UserHelper.CreateFreeUser().result;
            UserHelper.Login = login;

            // STEP -2- update user info (products etc. from site) 
            Login_Response login_Response;
            Purchaser.IsRestoring = true;

            

            if (!string.IsNullOrEmpty(login.user_id) && CrossConnectivity.Current.IsConnected && DownloadHelper.DownloadHelper.CheckInternetConnectionProgressive())
            { // we have registered user - try to update him 
              // check login
                Debug.WriteLine("App.cs -> ChooseStartPage -> STEP -2- -> try to update user info, ");
                Debug.WriteLine("user_id = " + UserHelper.Login.user_id);
                var postData = $"id={UserHelper.Login.user_id}";
                login_Response = await ServerApi.PostRequestProgressive<Login_Response>(ServerApi.USER_BY_ID_URL, postData, null);
                // process response
                if (login_Response == null)
                {
                    Debug.WriteLine("App.cs->ChooseStartPage->STEP - 2 -> Error! login_Response -> NULL");

                    Analytics.SendResultsRegular("App.cs", login_Response, login_Response?.error, ServerApi.USER_BY_ID_URL, postData);
                }
                else if (login_Response.error != null)
                {
                    Debug.WriteLine("App.cs -> ChooseStartPage -> STEP -2- -> Error! login_Response -> Message: " + login_Response.error.message);
                    Analytics.SendResultsRegular("App.cs", login_Response, login_Response?.error, ServerApi.USER_BY_ID_URL, postData);
                }
                else if (login_Response.result == null)
                {
                    Debug.WriteLine("App.cs -> ChooseStartPage -> STEP -2- -> Error! login_Response ->login_Response.result == null");
                    Analytics.SendResultsRegular("App.cs", login_Response, login_Response?.error, ServerApi.USER_BY_ID_URL, postData);
                }
                else
                {
                    Debug.WriteLine("App.cs -> ChooseStartPage -> STEP -2- -> login_Response -> OK !");

                    // delete duplicates in products if have
                    login_Response.result.DeleteDuplicateProducts();
                    UserHelper.Login = login_Response.result;
                    UserHelper.SetIsFree();
                }
            }
            

            // STEP -3- update purchases
            bool purchasesRestored = false;
            if (CrossConnectivity.Current.IsConnected && DownloadHelper.DownloadHelper.CheckInternetConnectionProgressive())
            {
                Debug.WriteLine("App.cs -> ChooseStartPage -> STEP -3- -> Purchaser.Restore()");
                purchasesRestored = await Purchaser.Restore();
                UserHelper.SetIsFree();
            }
            Purchaser.IsRestoring = false;
            Debug.WriteLine("App.cs -> ChooseStartPage -> STEP -3- -> purchasesRestored?=" + purchasesRestored);

            // STEP -4- finally choose start page
            if (string.IsNullOrEmpty(UserHelper.Login.user_id))
            {
                Debug.WriteLine("App.cs -> ChooseStartPage -> STEP -4- -> UserHelper.Login.user_id=null, go to LoginPage()");

                //MainPage = new LoginPage();
                MainPage = new LoginPage_new();

                //_weak = new WeakReference(MainPage);
                //Device.StartTimer(TimeSpan.FromSeconds(2), App_Timer);
            }
            else
            {
                if (await CacheHelper.Exists(CacheHelper.CURRENT_LANGUAGE) && await CacheHelper.Exists(CacheHelper.CATEGORYS_RESPONSE + (await CacheHelper.GetAsync(CacheHelper.CURRENT_LANGUAGE_CAT)).Data)) // we have the language
                {
                    Debug.WriteLine("App.cs -> ChooseStartPage -> STEP -4- -> user authorized, lang is chosen");
                    string Lang_Cat = (await CacheHelper.GetAsync(CacheHelper.CURRENT_LANGUAGE_CAT)).Data;
                    // SAVE DATA TO USER

                    UserHelper.Lang_cat = Lang_Cat;
                    UserHelper.SetIsFree();
                    UserHelper.Language = (await CacheHelper.GetAsync(CacheHelper.CURRENT_LANGUAGE)).Data;

                    GameHelper.memory_GameObjects = await CacheHelper.GetAsync<GameObjects>(CacheHelper.MEMORY_GAMEOBJECTS + UserHelper.Lang_cat);
                    GameHelper.sas_GameObjects = await CacheHelper.GetAsync<GameObjects>(CacheHelper.SAS_GAMEOBJECTS + UserHelper.Lang_cat);

                    Favorites.WaitIfListsAreBuisy("App.cs -> ChooseStartPage -> STEP -4-");
                    await Favorites.LoadFavorites_unprotected(UserHelper.Lang_cat, UserHelper.Login.user_id, UserHelper.IsFree);
                    Favorites.ListsAreBuisy = false;
                    Debug.WriteLine("App -> ListsAreBuisy = false");

                    if (!UserHelper.IsFree) Favorites.StartLoading();

                    var page = new MainPage_((await CacheHelper.GetAsync<CategoryResponse>(CacheHelper.CATEGORYS_RESPONSE + Lang_Cat)).result[0].viewType, 0);
                    /*
                    NavigationPage.SetHasNavigationBar(page, false);
                    await MainPage.Navigation.PushAsync(page);
                    App.Current.MainPage.Navigation.RemovePage(App.Current.MainPage.Navigation.NavigationStack[0]);
                    */

                    MainPage = page;
                    
                    //MainPage = new MainPage_((await CacheHelper.GetAsync<CategoryResponse>(CacheHelper.CATEGORYS_RESPONSE + Lang_Cat)).result[0].viewType);
                }
                else // we do not know the current language
                {
                    Debug.WriteLine("App.cs -> ChooseStartPage -> STEP -4- -> UserHelper.Login.user_id!=null, lang is not chosen");
                    MainPage = new LoginPage_new(true);                   
                }
            }            
        }
        
        protected override void OnStart()
        {
            // Handle when your app starts
            // ***************

            //string s = "Essayez%20une%20le%26ccedil%3Bon%20gratuite%20d%60anglais%20%28ASL%29%20%3A%20Animaux%201";
            //Debug.WriteLine("TEST OF STRING, s = " + System.Net.WebUtility.HtmlDecode(Uri.UnescapeDataString(s)));
            //var encoded = Uri.EscapeUriString("DEBUG ! How to encode   -(3 spaces),'-(one_apostroph)");
            //Debug.WriteLine($"Uri.EscapeUriString = {encoded}");



            //DownloadHelper.DownloadHelper.ParseAllMultySpaceAudios();

            
            SetUpNotifications();
            DoOnStart();
            
        }

        bool App_Timer()
        {
            //GC.Collect();            
            Debug.WriteLine(String.Format("LoginPage_new-> Timer -> IsContentAlive = {0}", _weak.IsAlive));            
            return true;
        }

        void SetUpNotifications()
        {
            System.Diagnostics.Debug.WriteLine("SetUpNotifications -> ");
            // Handle when your app starts
            CrossFirebasePushNotification.Current.Subscribe("general");
            CrossFirebasePushNotification.Current.OnTokenRefresh += (s, p) =>
            {
                System.Diagnostics.Debug.WriteLine($"TOKEN REC: {p.Token}");
            };
            System.Diagnostics.Debug.WriteLine($"TOKEN: {CrossFirebasePushNotification.Current.Token}");

            CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
            {
                System.Diagnostics.Debug.WriteLine("App -> OnNotificationReceived");
                string data_ = " ";
                foreach (var data in p.Data)
                {
                    System.Diagnostics.Debug.WriteLine($"{data.Key} : {data.Value}");

                    data_ += "\n" + $"{data.Key} : {data.Value}";
                }
                System.Diagnostics.Debug.WriteLine("data_  = " + data_);                
                
                // *** iOS
                if (Device.RuntimePlatform == Device.iOS)
                {
                    // it happens when app is running
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        
                        var answ = await App.Current.MainPage.DisplayAlert("",
                            Translate.GetString("new_version_text"), Translate.GetString("new_version_update_now"), RateWidget.REMIND_LATER);
                        if (answ) Device.OpenUri(new System.Uri(RateWidget.RATE_LINK_IOS)); 
                    });
                }
                // *** Android
                else
                {
                    try
                    {
                        if (p.Data.ContainsKey("body"))
                        {
                            haveNewNotification = true;
                            /*
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                App.Current.MainPage.DisplayAlert("App -> SetUpNotifications -> OnNotificationReceived", "p.Data[body] = " + p.Data["body"], "OK");
                                //mPage.Message = $"{p.Data["body"]}";
                            });
                            */
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            };


            CrossFirebasePushNotification.Current.OnNotificationOpened += (s, p) =>
            {
                System.Diagnostics.Debug.WriteLine("OnNotificationOpened");
                string data_ = " ";
                foreach (var data in p.Data)
                { 
                    data_ += "\n" + $"{data.Key} : {data.Value}";                   
                }
                Debug.WriteLine($"data_ = {data_}");
                

                /*
                if (!string.IsNullOrEmpty(p.Identifier))
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                       //mPage.Message = p.Identifier;
                    });
                }
                else if (p.Data.ContainsKey("color"))
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {

                        mPage.Navigation.PushAsync(new ContentPage()
                        {
                            BackgroundColor = Color.FromHex($"{p.Data["color"]}")

                        });
                    });

                }
                else if (p.Data.ContainsKey("aps.alert.title"))
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        //mPage.Message = $"{p.Data["aps.alert.title"]}";
                    });

                }
                */

                // *** iOS
                if (Device.RuntimePlatform == Device.iOS)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {  // just go to site for the update

                        Device.OpenUri(new System.Uri(RateWidget.RATE_LINK_IOS)); 
                    });
                }

                // *** Android
                else
                {
                    try
                    {
                        if (haveNewNotification || (p.Data.ContainsKey("body") && p.Data["body"] != null) || p.Data.Keys.Count > 1)
                        {
                            haveNewNotification = false;
                            Device.BeginInvokeOnMainThread(() =>
                            {  // just go to site for the update
                                
                                    //if (Device.RuntimePlatform == Device.iOS) Device.OpenUri(new System.Uri(RateWidget.RATE_LINK_IOS));
                                    Device.OpenUri(new System.Uri(RateWidget.RATE_LINK_ANDROID));
                               
                            });

                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }

                
                

                
                
            };

            CrossFirebasePushNotification.Current.OnNotificationAction += (s, p) =>
            {
                Debug.WriteLine("Action");                
                if (!string.IsNullOrEmpty(p.Identifier))
                {
                    System.Diagnostics.Debug.WriteLine($"ActionId: {p.Identifier}");
                    foreach (var data in p.Data)
                    {
                        System.Diagnostics.Debug.WriteLine($"{data.Key} : {data.Value}");
                    }
                }

            };

            CrossFirebasePushNotification.Current.OnNotificationDeleted += (s, p) =>
            {
                
                Debug.WriteLine("Dismissed");
            };


            
            
        }
        
        // ==========

        Task DoOnStart() {
            return Task.Run(async ()=>
            {

                await Task.Delay(25);

                await GameHelper.CreateGameObjects();

                Device.BeginInvokeOnMainThread(ChooseStartPage);

                //Device.BeginInvokeOnMainThread(TestSignUp);

                // create gameobjects files
                //GameHelper.GetAllAudios();
                //GameHelper.GetAllAudiosForSAS();                

                return;
            });
        }

        async void TestSignUp()
        {
            Debug.WriteLine($"App -> TestSignUp ");
            if (!CrossConnectivity.Current.IsConnected || ! DownloadHelper.DownloadHelper.CheckInternetConnectionProgressive())
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
                    var postData = $"email=neatcoding19791979@gmail.com";
                    postData += $"&pass=test123";
                    postData += $"&fname={string.Empty}"; //{fnameEntry.Text}";
                    postData += $"&lname={string.Empty}"; //{lnameEntry.Text}";

                    NewUserResponse newUserResponse = await ServerApi.PostRequestProgressive<NewUserResponse>(ServerApi.NEW_USER_URL, postData, null);
                    // process response
                    if (newUserResponse == null)
                    {
                        Analytics.SendResultsRegular("SignUp_Page", newUserResponse, newUserResponse?.error, ServerApi.NEW_USER_URL, postData);
                        Debug.WriteLine("SignUp_Page -> check new user -> newUserResponse == null");
                        if (AsyncMessages.CheckDisplayAlertTimeout())
                        {
                            await App.Current.MainPage.DisplayAlert(Translate.GetString("sign_up_cant_create_new_user"),
                            POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(newUserResponse?.error, 2), POP_UP.OK);
                        }

                    }
                    else if (newUserResponse.error != null)
                    {
                        Analytics.SendResultsRegular("SignUp_Page", newUserResponse, newUserResponse?.error, ServerApi.NEW_USER_URL, postData);
                        Debug.WriteLine("SignUp_Page -> check new user -> newUserResponse.error != null");
                        await App.Current.MainPage.DisplayAlert(Translate.GetString("sign_up_cant_create_new_user"), "Message: " + newUserResponse.error.message, POP_UP.OK);
                    }
                    else if (newUserResponse.result == null)
                    {
                        Analytics.SendResultsRegular("SignUp_Page", newUserResponse, newUserResponse?.error, ServerApi.NEW_USER_URL, postData);
                        Debug.WriteLine("SignUp_Page -> check new user -> newUserResponse.result == null");
                        await App.Current.MainPage.DisplayAlert(Translate.GetString("sign_up_cant_create_new_user"),
                            POP_UP.GetCode(newUserResponse?.error, 3), POP_UP.OK);
                    }
                    else
                    { // new_user - OK! -> try to login

                        // check login
                        Debug.WriteLine("SignUp_Page -> check login");
                        postData = $"login=neatcoding19791979@gmail.com";
                        postData += $"&pass=test123";
                        Login_Response login_Response = await ServerApi.PostRequestProgressive<Login_Response>(ServerApi.LOGIN_URL, postData, null);
                        // process response
                        if (login_Response == null)
                        {
                            Analytics.SendResultsRegular("SignUp_Page", login_Response, login_Response?.error, ServerApi.LOGIN_URL, postData);
                            Debug.WriteLine("SignUp_Page -> check login -> login_Response == null");
                            if (AsyncMessages.CheckDisplayAlertTimeout())
                            {
                                await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                                POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(login_Response?.error, 4), POP_UP.OK);
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
                                                                    POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(login_Response?.error, 5), POP_UP.OK);
                            }

                            //("Error!", "Login result is null!", "ОK");
                        }
                        else // we finally OK with user
                        {
                            // cashe login here
                            Debug.WriteLine("SignUp_Page -> check login -> we finally OK with user");
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
                            
                        }

                    }
                }
            

        }


        protected override void OnSleep()
        {
            // Handle when your app sleeps
            OnSleepEvent?.Invoke();

            /*
            var ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            Debug.WriteLine("App OnSleep(), ci = " + ci.Name);
            */
            

        }

        

        protected override async void OnResume()
        {
            Debug.WriteLine("App OnResume()");

            Translate.Init();

            // Handle when your app resumes
            OnResumeEvent?.Invoke();
           

            // if we have internet - update IAP
            if (CrossConnectivity.Current.IsConnected && !Purchaser.IsRestoring && UserHelper.Login != null && Purchaser.IsExpired && DownloadHelper.DownloadHelper.CheckInternetConnectionProgressive())
            {
                Purchaser.IsRestoring = true;
                // check login
                     
                if (!string.IsNullOrEmpty(UserHelper.Login.user_id))
                {
                    var postData = $"id={UserHelper.Login.user_id}";
                    Login_Response login_Response = await ServerApi.PostRequestProgressive<Login_Response>(ServerApi.USER_BY_ID_URL, postData, null);
                    // process response
                    if (login_Response == null)
                    {
                        Debug.WriteLine("App OnResume() -> Error! login_Response -> NULL");
                        Analytics.SendResultsRegular("App.cs -> OnResume", login_Response, login_Response?.error, ServerApi.USER_BY_ID_URL, postData);
                    }
                    else if (login_Response.error != null)
                    {
                        Debug.WriteLine("App OnResume() -> Error! login_Response -> Message: " + login_Response.error.message);
                        Analytics.SendResultsRegular("App.cs  -> OnResume", login_Response, login_Response?.error, ServerApi.USER_BY_ID_URL, postData);
                    }
                    else if (login_Response.result == null)
                    {
                        Debug.WriteLine("App OnResume() -> Error! login_Response ->login_Response.result == null");
                        Analytics.SendResultsRegular("App.cs  -> OnResume", login_Response, login_Response?.error, ServerApi.USER_BY_ID_URL, postData);
                    }
                    else
                    {
                        Debug.WriteLine("App OnResume() -> login_Response -> OK !");
                        // delete duplicates in products if have
                        login_Response.result.DeleteDuplicateProducts();
                        UserHelper.Login = login_Response.result;
                    }
                }
                

                Debug.WriteLine("App -> OnResume -> try to restore IAP");
                bool result = await Purchaser.Restore();
                UserHelper.SetIsFree();
                Debug.WriteLine("App -> OnResume -> try to restore IAP for user: " + UserHelper.Login.user_id + ", restored result: " + result);               
            }

            NewVersionWidget.CheckNewVersion(App.Current.MainPage);

        }


    }

    
}
