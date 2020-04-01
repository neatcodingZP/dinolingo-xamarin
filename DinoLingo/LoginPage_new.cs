using DinoLingo.ScreenOrientations;
using FFImageLoading.Forms;
using Newtonsoft.Json;
using Plugin.Connectivity;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DinoLingo
{
    public class LoginPage_new: ContentPage
    {
        static int ERROR_PREFIX = 70;

        bool goToCooseLanguage_Page = false;

        bool isAnimating;
        bool isLoginProcessing = false;
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

        // available UI elements
        Entry loginEntry, passwordEntry;
        View blockerView;

        private double _loginBtnWidthFactor = 420.0 / 125.0;
        private string _blueBtnRes = "DinoLingo.Resources.UI.btnblue_new.png";
        
        public LoginPage_new(bool goToCooseLanguage_Page = false)
        {
            FlowDirection = Translate.FlowDirection_;
            this.goToCooseLanguage_Page = goToCooseLanguage_Page;


            var absLayout = new AbsoluteLayout() { BackgroundColor = Color.White }; //MyColors.BackgroundBlueColor };

            /*
            Forms9Patch.Image patternImage = new Forms9Patch.Image
            {
                Source = Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.pattern.png"),
                Fill = Forms9Patch.Fill.Tile,
            };

            
            AbsoluteLayout.SetLayoutBounds(patternImage, new Rectangle(0.5, 0.5, 1, 1));
            AbsoluteLayout.SetLayoutFlags(patternImage, AbsoluteLayoutFlags.All);
            */

            ScrollView scrollView = new ScrollView();
            AbsoluteLayout.SetLayoutBounds(scrollView, new Rectangle(0.5, 0.5, 1, 1));
            AbsoluteLayout.SetLayoutFlags(scrollView, AbsoluteLayoutFlags.All);



            /*
            RelativeLayout relLayout = new RelativeLayout { HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
            HeightRequest = UI_Sizes.ScreenWidthX_UNIFORMED_TO_1_78,
            WidthRequest = UI_Sizes.ScreenHeightX_UNIFORMED_TO_1_78,
            };

            
            Frame shadowFrame = new Frame { CornerRadius = UI_Sizes.BigFrameCornerRadius, BorderColor = Color.Transparent,
                HasShadow = false, BackgroundColor = MyColors.ShadowLoginColor,
                TranslationX = UI_Sizes.BigFrameShadowTranslationX, TranslationY = UI_Sizes.BigFrameShadowTranslationX };
            relLayout.Children.Add(shadowFrame,
                Constraint.RelativeToParent((parent) =>
                    {
                        return (.16 * parent.Width);
                    }),
                Constraint.RelativeToParent((parent) =>
                    {
                        return (.08 * parent.Height);
                    }),
                Constraint.RelativeToParent((parent) =>
                {
                    return (.7 * parent.Width);
                }),
                Constraint.RelativeToParent((parent) =>
                {
                    return (.88 * parent.Height);
                })
            );

            Frame mainFrame = new Frame
            {
                CornerRadius = UI_Sizes.BigFrameCornerRadius,
                BorderColor = Color.Transparent,
                HasShadow = false,
                BackgroundColor = Color.White,
                Padding = 10
            };
            */

            // add grid to main frame
            var grid = new Grid()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                HeightRequest = UI_Sizes.ScreenWidthX_UNIFORMED_TO_1_78,
                WidthRequest = UI_Sizes.ScreenHeightX_UNIFORMED_TO_1_78,
                RowSpacing = 0,
                ColumnSpacing = 0,
            };

            /*
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            */

            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(10, GridUnitType.Star) }); // top offset
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(36, GridUnitType.Star) }); // top image
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(14, GridUnitType.Star) }); // welcome text
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(10, GridUnitType.Star) }); // lang learn program text
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(9, GridUnitType.Star) }); // member login text
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // space
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(14, GridUnitType.Star) }); // e-mail entry
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // space
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(14, GridUnitType.Star) }); // password entry
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(3, GridUnitType.Star) }); // space
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(13, GridUnitType.Star) }); // login line
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(14, GridUnitType.Star) }); // space
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(10, GridUnitType.Star) }); // new to dino text
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20, GridUnitType.Star) }); // sign up button
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20, GridUnitType.Star) }); // free lesson button
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(14, GridUnitType.Star) }); // bottom offset


            // add top image
            // left dino
            ContentView topDino = new ContentView
            {
                IsEnabled = false,
                Content = new Image
                {
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    Source = Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.login_dino_3.png"),
                    Aspect = Aspect.AspectFit,
                }
            };
            grid.Children.Add(topDino, 0, 1);


            /*
            var stackLayout = new StackLayout();
            stackLayout.Children.Add(new MyViews.MyLabel { Text = Translate.GetString("login_welcome_to_dinolingo"),
                FontSize = UI_Sizes.MediumTextSize,
                TextColor = MyColors.BlueTextLoginColor, HorizontalOptions = LayoutOptions.Center });

            stackLayout.Children.Add(new MyViews.MyLabel
            {
                Text = Translate.GetString("login_lang_learn_program"), FontSize = UI_Sizes.MicroTextSize,
                TextColor = MyColors.ReportOrangeColor, HorizontalOptions = LayoutOptions.Center
            });
            */
            grid.Children.Add(
                new MyViews.MyLabel
                {
                    Text = Translate.GetString("login_welcome_to_dinolingo"),
                    FontSize = UI_Sizes.SmallTextSize * 1.2,
                    TextColor = MyColors.BlueTextLoginColor,
                    HorizontalOptions = LayoutOptions.Center
                }, 0, 2);

            grid.Children.Add(
                new MyViews.MyLabel
                {
                    Text = Translate.GetString("login_lang_learn_program"),
                    FontSize = UI_Sizes.MicroTextSize,
                    TextColor = MyColors.ReportOrangeColor,
                    HorizontalOptions = LayoutOptions.Center
                }, 0, 3);

            grid.Children.Add(
                new MyViews.MyLabel
                {
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    Text = Translate.GetString("login_members"),
                    FontSize = UI_Sizes.SmallTextSize,
                    TextColor = MyColors.BlueTextLoginColor,
                }, 0, 4);


            // login-password grid
            var loginGrid = new Grid();
            loginGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(16, GridUnitType.Star) });
            loginGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(76, GridUnitType.Star) });
            loginGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(16, GridUnitType.Star) });

            var passGrid = new Grid();
            passGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(16, GridUnitType.Star) });
            passGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(76, GridUnitType.Star) });
            passGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(16, GridUnitType.Star) });

            Image entryBackImage = new Image
            {
                Source = Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.login_entry_gray.png"),
                Aspect = Aspect.Fill,
                Margin = new Thickness(0, 10, 0, 0),
            };
            Image entryBackImage2 = new Image
            {
                Source = Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.login_entry_gray.png"),
                Aspect = Aspect.Fill,
                Margin = new Thickness(0, 10, 0, 0),
            };
            loginGrid.Children.Add(entryBackImage, 1, 0);
            passGrid.Children.Add(entryBackImage2, 1, 0);

            MyViews.BorderlessEntry loginEntry_ = new MyViews.BorderlessEntry
            {
                TextColor = Color.Black,
                PlaceholderColor = Color.Gray,
                Margin = new Thickness(10, 10, 5, 0),
                Placeholder = Translate.GetString("login_email"),
                FontSize = UI_Sizes.MicroTextSize,
            };
            loginGrid.Children.Add(loginEntry_, 1, 0);
            loginEntry = (Entry)loginEntry_;

            // password entry
            MyViews.BorderlessEntry passwordEntry_ = new MyViews.BorderlessEntry
            {
                TextColor = Color.Black,
                PlaceholderColor = Color.Gray,
                Margin = new Thickness(10, 10, 5, 0),
                Placeholder = Translate.GetString("login_password"),
                FontSize = UI_Sizes.MicroTextSize,
                IsPassword = true,
            };

            passwordEntry_.Completed += OnLoginClicked;
            passGrid.Children.Add(passwordEntry_, 1, 0);
            passwordEntry = (Entry)passwordEntry_;

            grid.Children.Add(loginGrid, 0, 6);
            grid.Children.Add(passGrid, 0, 8);

            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (sender, e) =>
            {
                View view = (View)sender;
                OnButtonTapped(view);
            };

            var loginLineGrid = new Grid();
            loginLineGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(16, GridUnitType.Star) });
            loginLineGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(76, GridUnitType.Star) });
            loginLineGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(16, GridUnitType.Star) });

            

            // login button            
            MyViews.ButtonWithImage loginBtn = new MyViews.ButtonWithImage(
                "LoginBtn",
                new Rectangle(1.0, 0.5, UI_Sizes.ButtonHeight * _loginBtnWidthFactor, UI_Sizes.ButtonHeight),
                //"DinoLingo.Resources.UI.btnblue_x2.png",
                _blueBtnRes,
                Translate.GetString("login_login"),
                UI_Sizes.SmallTextSize,
                //MyColors.ButtonBlueTextColor,
                Color.White,
                tapGestureRecognizer,
                AbsoluteLayoutFlags.PositionProportional
                );            

            loginLineGrid.Children.Add(loginBtn, 1, 0);
            grid.Children.Add(loginLineGrid, 0, 10);

            // forgot password
            MyViews.MyLabel forgotPass = new MyViews.MyLabel()
            {
                ClassId = "ForgotPassword",
                Text = Translate.GetString("login_forgot_password"),
                FontSize = UI_Sizes.MicroTextSize * 0.75,
                TextColor = Color.DarkGray,
                TextDecorations = TextDecorations.Underline,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.End,
                Margin = new Thickness(0, 10, 0, 0),
            };
            forgotPass.GestureRecognizers.Add(tapGestureRecognizer);

            loginLineGrid.Children.Add(forgotPass, 1, 0);


            MyViews.MyLabel newToDinoLabel = new MyViews.MyLabel()
            {
                Text = Translate.GetString("login_new_to_dinolingo"),
                FontSize = UI_Sizes.SmallTextSize,
                TextColor = MyColors.ReportOrangeColor,
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
            }; 
            grid.Children.Add(newToDinoLabel, 0, 12);

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
            grid.Children.Add(signUpBtn, 0, 13);           

            // try free button
            MyViews.ButtonWithImage tryFreeBtn = new MyViews.ButtonWithImage(
                "TryFreeBtn",
                new Rectangle(0.5, 0, UI_Sizes.ButtonHeight * _loginBtnWidthFactor, UI_Sizes.ButtonHeight),
                _blueBtnRes,
                Translate.GetString("login_try_a_free_lesson"),
                UI_Sizes.SmallTextSize * 0.75,
                Color.White,
                tapGestureRecognizer,
                AbsoluteLayoutFlags.PositionProportional
                );            
            grid.Children.Add(tryFreeBtn, 0, 14);

           









            //grid.Children.Add(membersLabel, 0, 1);
            //Grid.SetColumnSpan(membersLabel, 2);





            // login entry






            // login-password grid




            // new to dinolingo
            /*
            StackLayout newToDinoStack = new StackLayout
            {
                Padding = new Thickness(20,0,20,0),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center,
            };

            BoxView line = new BoxView
            {
                BackgroundColor = Color.FromHex("#8cc5e3"),
                HeightRequest = 3,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            */


            // Grid.SetColumnSpan(newToDinoStack, 2);


            // mainFrame.Content = grid;

            // left dino
            /*
            ContentView leftDino = new ContentView {
                IsEnabled = false,
                Content = new Image {
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    VerticalOptions = LayoutOptions.StartAndExpand,
                    Source = Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.login_dino_1.png"),
                    Aspect = Aspect.AspectFit,                }
            };
            
            relLayout.Children.Add(leftDino,
                 Constraint.RelativeToParent((parent) =>
                 {
                     return (.03 * parent.Width);
                 }),
                 Constraint.RelativeToParent((parent) =>
                 {
                     return (.63 * parent.Height);
                 }),
                 Constraint.RelativeToParent((parent) =>
                 {
                     return (.4 * parent.Height);
                 }),
                 Constraint.RelativeToParent((parent) =>
                 {
                     return (.4 * parent.Height);
                 })
             );
             */

            // right dino
            /*
            ContentView rightDino = new ContentView
            {
                IsEnabled = false,
                Content = new Image
                {
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    VerticalOptions = LayoutOptions.StartAndExpand,
                    Source = Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.login_dino_2.png"),
                    Aspect = Aspect.AspectFit,
                }
            };
            relLayout.Children.Add(rightDino,
                Constraint.RelativeToParent((parent) =>
                {
                    return (.83 * parent.Width);
                }),
                Constraint.RelativeToParent((parent) =>
                {
                    return (.64 * parent.Height);
                }),
                Constraint.RelativeToParent((parent) =>
                {
                    return (.4 * parent.Height);
                }),
                Constraint.RelativeToParent((parent) =>
                {
                    return (.4 * parent.Height);
                })
            );
            */

            scrollView.Content = grid;

            // blocker
            CachedImage imageBlocker = new CachedImage {
                LoadingPlaceholder = "loading.gif",
                Source = "loading.gif",
                Aspect = Aspect.AspectFit,
            };
            AbsoluteLayout.SetLayoutBounds(imageBlocker, new Rectangle(0.5, 0.5, -1, -1));
            AbsoluteLayout.SetLayoutFlags(imageBlocker, AbsoluteLayoutFlags.PositionProportional);
            

            AbsoluteLayout blocker = new AbsoluteLayout {
                IsEnabled = false,
                IsVisible = false,
                BackgroundColor = MyColors.ShadowLoginColor,
            };
            AbsoluteLayout.SetLayoutBounds(blocker, new Rectangle(0.5, 0.5, 1, 1));
            AbsoluteLayout.SetLayoutFlags(blocker, AbsoluteLayoutFlags.All);
            blocker.Children.Add(imageBlocker);
            blockerView = (View) blocker;


            //absLayout.Children.Add(patternImage);
            absLayout.Children.Add(scrollView);
            absLayout.Children.Add(blocker);

            Content = absLayout;           
        }

        public void OnLoginClicked(object sender, EventArgs e)
        {
            if (!isLoginProcessing) ProcessLogin();
        }

        async void OnButtonTapped(View view)
        {
            if (IsAnimating) return;
            IsAnimating = true;
            await AnimateView(view, 250);

            if (view.ClassId == "LoginBtn")
            {                 
                // check login first!
                if (string.IsNullOrEmpty(loginEntry.Text) || string.IsNullOrWhiteSpace(loginEntry.Text))
                {
                    await App.Current.MainPage.DisplayAlert("", Translate.GetString("login_please_enter_your_email"), POP_UP.OK);
                    IsAnimating = false;
                }
                else if (string.IsNullOrEmpty(passwordEntry.Text) || string.IsNullOrWhiteSpace(passwordEntry.Text))
                {
                    await App.Current.MainPage.DisplayAlert("", Translate.GetString("login_please_enter_valid_pass"), POP_UP.OK);
                    IsAnimating = false;
                }
                else
                {
                    ProcessLogin();
                }
            }
            else if (view.ClassId == "TryFreeBtn")
            {
                // create and save "free user"
                Debug.WriteLine("LoginViewModel -> TryFreeBtn");
                UserHelper.CreateFreeUser();


                ProcessLogin(UserHelper.Login);
            }
            else if (view.ClassId == "SignUpBtn")
            {

                /*
                // create and save "free user"
                Login_Response login_Response = UserHelper.CreateFreeUser();

                //add response
                await CacheHelper.Add(CacheHelper.LOGIN, login_Response.result);
                */


                await App.Current.MainPage.Navigation.PushModalAsync(new ParentCheck_Page(ParentCheck_Page.WhatToDoNext.WANT_TO_SIGN_UP));
                IsAnimating = false;
                //Device.OpenUri(new System.Uri("https://dinolingo.com/"));
            }
            else if (view.ClassId == "ForgotPassword")
            {
                Debug.WriteLine("LoginViewModel -> ForgotPassword");

                // check login
                if (string.IsNullOrEmpty(loginEntry.Text) || string.IsNullOrWhiteSpace(loginEntry.Text))
                {
                    await App.Current.MainPage.DisplayAlert("", Translate.GetString("login_please_enter_your_email"), POP_UP.OK);
                }
                else if (!loginEntry.Text.Contains("@"))
                {
                    await App.Current.MainPage.DisplayAlert("", Translate.GetString("login_please_enter_correct_email"), POP_UP.OK);
                }
                else
                {
                    // if login is ok -> make pop-up

                    var answer = await App.Current.MainPage.DisplayAlert(Translate.GetString("login_reset_pass"),
                        $"{Translate.GetString("login_link_will_be_sent_to_email")}\n***{loginEntry.Text}***\n{Translate.GetString("login_are_you_sure_to_reset_pass")}",
                        Translate.GetString("login_reset_pass"), POP_UP.CANCEL);

                    if (answer)
                    {
                        Debug.WriteLine("LoginViewModel -> ForgotPassword -> reset password");
                        // check the internet
                        if (!CrossConnectivity.Current.IsConnected || !DownloadHelper.DownloadHelper.CheckInternetConnectionProgressive())
                        { // check connectivity

                            await App.Current.MainPage.DisplayAlert(POP_UP.OOPS, POP_UP.NO_CONNECTION, POP_UP.OK);
                            //("Error!", "No internet connection!", "ОK");
                            IsAnimating = false;
                            isLoginProcessing = false;
                            return;
                        }
                        else
                        {
                            Debug.WriteLine("LoginViewModel -> ForgotPassword -> reset password -> make request to the site, to reset the password");
                            var postData = $"email={loginEntry.Text}";
                            blockerView.IsEnabled = blockerView.IsVisible = true;
                            ResetPasswordResponse resetPasswordResponse = await ServerApi.PostRequestProgressive<ResetPasswordResponse>(ServerApi.RESET_PASSWORD, postData, null);
                            blockerView.IsEnabled = blockerView.IsVisible = false;
                            // process response
                            if (resetPasswordResponse == null)
                            {
                                Analytics.SendResultsRegular("LoginViewModel", resetPasswordResponse, resetPasswordResponse?.error, ServerApi.RESET_PASSWORD, postData);
                                await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                                    POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(resetPasswordResponse?.error, ERROR_PREFIX + 1), POP_UP.OK);
                                //("Error!", "Error in response from server!", "ОK");                                
                            }
                            else if (resetPasswordResponse.error != null)
                            {
                                Analytics.SendResultsRegular("LoginViewModel", resetPasswordResponse, resetPasswordResponse?.error, ServerApi.RESET_PASSWORD, postData);
                                if (resetPasswordResponse.error.code == "600")
                                {
                                    await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                                     Translate.GetString("login_user_with_email_doesnt_exist") + "\n" + POP_UP.GetCode(resetPasswordResponse?.error, ERROR_PREFIX + 2), POP_UP.OK);
                                }
                                else await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                                    POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(resetPasswordResponse?.error, ERROR_PREFIX + 2), POP_UP.OK);
                                //("Error!", "Message: " + resetPasswordResponse.error.message, "ОK");                                
                            }
                            else if (resetPasswordResponse.result == null)
                            {
                                Analytics.SendResultsRegular("LoginViewModel", resetPasswordResponse, resetPasswordResponse?.error, ServerApi.RESET_PASSWORD, postData);
                                await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                                    POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(resetPasswordResponse?.error, ERROR_PREFIX + 3), POP_UP.OK);
                                //("Error!", "Error in response from server (null result)!", "ОK");                                
                            }
                            else
                            {
                                await App.Current.MainPage.DisplayAlert(Translate.GetString("login_success"),
                                    $"{Translate.GetString("login_instructions_to_reset_pass_sent_to_email")}\n{loginEntry.Text}", POP_UP.OK);
                            }

                        }
                    }


                }

                IsAnimating = false;
            }
            //IsAnimating = false;
        }

        async void ProcessLogin(Login_Response.Login login = null)
        {
            isLoginProcessing = true;
            Debug.WriteLine("LoginViewModel -> process login");

            try
            {
                // preserve old IAPs 
                Login_Response.Product[] oldIAPs;
                if (UserHelper.Login != null) oldIAPs = UserHelper.Login.IAPproducts;
                else oldIAPs = new Login_Response.Product[0];

                Login_Response login_Response;
                if (login == null)
                {
                    //check login here
                    if (!CrossConnectivity.Current.IsConnected || !DownloadHelper.DownloadHelper.CheckInternetConnectionProgressive())
                    { // check connectivity

                        await App.Current.MainPage.DisplayAlert(POP_UP.OOPS, POP_UP.NO_CONNECTION, POP_UP.OK);
                        //("Error!", "No internet connection!", "ОK");
                        isLoginProcessing = false;
                        IsAnimating = false;
                        return;
                    }


                    blockerView.IsVisible = blockerView.IsEnabled = true;
                    // check login
                    var postData = $"login={loginEntry.Text}";
                    postData += $"&pass={passwordEntry.Text}";
                    login_Response = await ServerApi.PostRequestProgressive<Login_Response>(ServerApi.LOGIN_URL, postData, null);
                    // process response
                    if (login_Response == null)
                    {

                        Analytics.SendResultsRegular("LoginViewModel", login_Response, login_Response?.error, ServerApi.LOGIN_URL, postData);
                        blockerView.IsVisible = blockerView.IsEnabled = false;
                        await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                            POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(login_Response?.error, ERROR_PREFIX + 4), POP_UP.OK);
                        //("Error!", "Error in response from server!", "ОK");
                        IsAnimating = false;
                        isLoginProcessing = false;
                        return;
                    }
                    else if (login_Response.error != null)
                    {

                        Analytics.SendResultsRegular("LoginViewModel", login_Response, login_Response?.error, ServerApi.LOGIN_URL, postData);
                        blockerView.IsVisible = blockerView.IsEnabled = false;
                        if (login_Response.error.code == "1")
                        {
                            await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                            Translate.GetString("login_user_and_pass_doesnt_exist") + "\n" + POP_UP.GetCode(login_Response?.error, ERROR_PREFIX + 5), POP_UP.OK);
                        }
                        else await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                            POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(login_Response?.error, ERROR_PREFIX + 5), POP_UP.OK);
                        //("Error!", "Message: " + login_Response.error.message, "ОK");
                        isLoginProcessing = false;
                        IsAnimating = false;
                        return;
                    }
                    else if (login_Response.result == null)
                    {
                        Analytics.SendResultsRegular("LoginViewModel", login_Response, login_Response?.error, ServerApi.LOGIN_URL, postData);
                        blockerView.IsVisible = blockerView.IsEnabled = false;
                        await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                            POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(login_Response?.error, ERROR_PREFIX + 6), POP_UP.OK);
                        //("Error!", "Login result is null!", "ОK");
                        isLoginProcessing = false;
                        IsAnimating = false;
                        return;
                    }

                }
                else
                {
                    login_Response = new Login_Response { result = login };
                }


                // delete duplicates in products if have
                login_Response.result.DeleteDuplicateProducts();
                UserHelper.Login = login_Response.result;
                UserHelper.Login.IAPproducts = oldIAPs;

                // if we have internet - update IAP
                if (CrossConnectivity.Current.IsConnected && !Purchaser.IsRestoring && DownloadHelper.DownloadHelper.CheckInternetConnectionProgressive())
                { // check connectivity
                    blockerView.IsVisible = blockerView.IsEnabled = true;
                    Debug.WriteLine("Login -> ProcessLogin -> try to restore IAP");
                    bool result = await Purchaser.Restore();
                    UserHelper.SetIsFree();
                    Debug.WriteLine("Login -> ProcessLogin -> try to restore IAP for user: " + UserHelper.Login.user_id + ", UserHelper.TotalProducts()=" + UserHelper.TotalProducts());
                }

                // cashe login here           
                await CacheHelper.Add(CacheHelper.LOGIN, UserHelper.Login, TimeSpan.FromSeconds(100));

                // process amount of products 
                Debug.WriteLine("Login -> ProcessLogin -> try to get total products...");
                int totalProducts = UserHelper.TotalProducts();
                Debug.WriteLine("Login -> ProcessLogin -> totalProducts = " + totalProducts);

                blockerView.IsVisible = blockerView.IsEnabled = false;


                if (login != null) // try free modeDisplay
                {
                    Debug.WriteLine("Login -> ProcessLogin -> try free mode ");
                    await App.Current.MainPage.Navigation.PushModalAsync(new ChooseLanguage_Page(UserHelper.Login));
                }
                else if (totalProducts == 0)
                { // 
                    Debug.WriteLine("Login -> ProcessLogin -> ");
                    await App.Current.MainPage.DisplayAlert(Translate.GetString("login_info"),
                        Translate.GetString("login_have_no_products"), POP_UP.OK);
                    // continue free

                    /*
                    Page current = App.Current.MainPage;
                    App.Current.MainPage = new ChooseLanguage_Page(UserHelper.Login);
                    navigation.RemovePage(current);
                    */
                    await App.Current.MainPage.Navigation.PushModalAsync(new ChooseLanguage_Page(UserHelper.Login));
                }
                else if (totalProducts == 1)
                { // only 1 product
                  //add current language 
                    Debug.WriteLine("Login -> ProcessLogin -> totalProducts == 1 ");
                    UserHelper.Lang_cat = UserHelper.GetSingleProduct(login_Response).cat_id.ToString();
                    UserHelper.Language = LANGUAGES.CAT_INFO[UserHelper.GetSingleProduct(login_Response).cat_id.ToString()].Name;
                    UserHelper.SetIsFree();

                    await CacheHelper.Add(CacheHelper.CURRENT_LANGUAGE, UserHelper.Language);
                    await CacheHelper.Add(CacheHelper.CURRENT_LANGUAGE_CAT, UserHelper.Lang_cat);

                    GameHelper.memory_GameObjects = await CacheHelper.GetAsync<GameObjects>(CacheHelper.MEMORY_GAMEOBJECTS + UserHelper.Lang_cat);
                    GameHelper.sas_GameObjects = await CacheHelper.GetAsync<GameObjects>(CacheHelper.SAS_GAMEOBJECTS + UserHelper.Lang_cat);

                    await Favorites.Refresh(string.Empty, string.Empty, true, UserHelper.Lang_cat, UserHelper.Login.user_id, UserHelper.IsFree);
                    if (!UserHelper.IsFree) Favorites.StartLoading();

                    GetCategoriesAndLaunchMainPage(UserHelper.Lang_cat);
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
                Debug.WriteLine("LoadingViewModel -> ProcessLogin -> ex:" + ex.Message);
            }

            IsAnimating = false;
            isLoginProcessing = false;

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
                    Debug.WriteLine("categories:" + (await CacheHelper.GetAsync(CacheHelper.CATEGORYS_RESPONSE + cat_id)).Data);

                    Page current = App.Current.MainPage;
                    App.Current.MainPage = new MainPage_(categoryResponse.result[0].viewType);
                    //navigation.RemovePage(current);
                }
                else
                { // download categories...
                    if (!CrossConnectivity.Current.IsConnected || !DownloadHelper.DownloadHelper.CheckInternetConnectionProgressive())
                    { // check connectivity
                        await App.Current.MainPage.DisplayAlert(POP_UP.OOPS, POP_UP.NO_CONNECTION, POP_UP.OK);
                        //("Error!", "No internet connection!", "ОK");
                        return;
                    }
                    var postData = $"cat={cat_id}";
                    CategoryResponse categoryResponse = await ServerApi.PostRequestProgressive<CategoryResponse>(ServerApi.CATS_URL, postData, null);
                    Debug.WriteLine("Login ViewModel -> categoryResponse = " + JsonConvert.SerializeObject(categoryResponse));
                    if (categoryResponse != null && categoryResponse.result != null && categoryResponse.result.Length > 0)
                    {
                        if (categoryResponse.error == null)
                        {
                            categoryResponse.ReorderForGame(UserHelper.Lang_cat);
                            Debug.WriteLine("categoryResponse after reorder = " + JsonConvert.SerializeObject(categoryResponse));
                            await CacheHelper.Add(CacheHelper.CATEGORYS_RESPONSE + cat_id, categoryResponse);

                            Page current = App.Current.MainPage;
                            App.Current.MainPage = new MainPage_(categoryResponse.result[0].viewType);
                            //navigation.RemovePage(current);
                        }
                        else
                        {
                            Analytics.SendResultsRegular("LoginViewModel", categoryResponse, categoryResponse?.error, ServerApi.CATS_URL, postData);
                            await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                            POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(categoryResponse?.error, ERROR_PREFIX + 7), POP_UP.OK);
                            //("Error in categories!", categoryResponse.error.message, "ОK");
                        }
                    }
                    else
                    {
                        Analytics.SendResultsRegular("LoginViewModel", categoryResponse, categoryResponse?.error, ServerApi.CATS_URL, postData);
                        await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                            POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(categoryResponse?.error, ERROR_PREFIX + 8), POP_UP.OK);
                        //("Error!", "Can not get categories from server!", "ОK");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LoadingViewModel -> GetCategoriesAndLaunchMainPage -> ex:" + ex.Message);
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
                    Debug.WriteLine("LoadingPage_new -> AnimateView -> ex:" + ex.Message);
                }
                return;
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            MessagingCenter.Send((ContentPage) this, "ForcePortrait");
            ScreenOrientation.Instance.ForcePortrait();

            //MessagingCenter.Send(this, "ForceLandscape");

            if (goToCooseLanguage_Page)
            {
                goToCooseLanguage_Page = false;
                blockerView.IsEnabled = blockerView.IsVisible = true;
                Device.BeginInvokeOnMainThread(async () => {
                    await App.Current.MainPage.Navigation.PushModalAsync(new ChooseLanguage_Page(UserHelper.Login));
                    blockerView.IsEnabled = blockerView.IsVisible = false;
                });                
            }

            
            
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
            Debug.WriteLine("LoginPage_new -> Dispose");
            Content = null;
            BindingContext = null;
            animLock = null;
            loginEntry = null;
            passwordEntry = null;
            blockerView = null;
        }
    }    
}
