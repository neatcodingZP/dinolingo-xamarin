using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;
using DinoLingo.ScreenOrientations;

namespace DinoLingo
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ParentCheck_Page : ContentPage
    {

        public double KeysTextSize { get; set; }
        public double DialTextSize { get; set; }
        public enum WhatToDoNext { DEFAULT, WANT_TO_SIGN_UP, UPDATE_VERSION };
        WhatToDoNext WhatToDo = WhatToDoNext.DEFAULT;

        bool IsAnimating = false;
        string enter_the_numbers; //= "Enter the numbers:";
        List<string> key_values;// = new List<string>  { "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE" };
        string[] random_keys = new string[4];
        //string lastKey;
        int numbersTapped = 0;
        Label[] key_fields;

        static int launch = 1;
        // static WeakReference _weak;
        // static WeakReference _weak_content;


        public ParentCheck_Page()
        {
            InitializeComponent();
            BindingContext = this;
            key_fields = new Label[] { key_0, key_1, key_2, key_3 };
            //key_fields = new Label[] { new Label(), new Label(), new Label(), new Label() };



            if (launch == 1)
            {
               // _weak = new WeakReference(key_0);
               // _weak_content = new WeakReference(Content);
            }

            CloseBtn.HeightRequest = CloseBtn.WidthRequest = UI_Sizes.CloseBtnSize;

            MainGrid.WidthRequest = UI_Sizes.ScreenHeightX_UNIFORMED_TO_1_78;
            MainGrid.HeightRequest = UI_Sizes.ScreenWidthX_UNIFORMED_TO_1_78;
            double margin = UI_Sizes.ScreenHeightX_UNIFORMED_TO_1_78 * 0.05;


            ParentOnlyLabel.FontSize = UI_Sizes.ScreenHeightX_UNIFORMED_TO_1_78 * 0.06;
            ParentOnlyLabel2.FontSize = UI_Sizes.ScreenHeightX_UNIFORMED_TO_1_78 * 0.035;
            EnterTheNumbersTextLabel.FontSize = UI_Sizes.ScreenHeightX_UNIFORMED_TO_1_78 * 0.05;


            KeysTextSize = UI_Sizes.ScreenHeightX_UNIFORMED_TO_1_78 * 0.1;
            DelLabel.Text = "<";
            DialGrid.WidthRequest = DialGrid.HeightRequest = UI_Sizes.ScreenWidthX * 0.3;
            Debug.WriteLine("DialGrid.WidthRequest = " + DialGrid.WidthRequest);
            DialTextSize = DialGrid.WidthRequest * 0.3 * 0.6;
            Debug.WriteLine("text size = " + DialTextSize);

            enter_the_numbers = Translate.GetString("parent_check_enter_the_numbers");
            key_values = new List<string>  { Translate.GetString("one"), Translate.GetString("two"), Translate.GetString("three"),
                Translate.GetString("four"), Translate.GetString("five"), Translate.GetString("six"),
                Translate.GetString("seven"), Translate.GetString("eight"), Translate.GetString("nine") };

            Init();

        }

        public ParentCheck_Page(WhatToDoNext whatToDo) : this()
        {
            WhatToDo = whatToDo;
        }

        void Init()
        {
            Random random = new Random();
            for (int i = 0; i < random_keys.Length; i++)
            {
                int index = random.Next(0, key_values.Count);
                random_keys[i] = key_values[index];
                key_fields[i].Text = string.Empty;
            }

            numbersTapped = 0;
            EnterTheNumbersTextLabel.Text = enter_the_numbers + "\n" + random_keys[0] + ", " + random_keys[1] + ", " + random_keys[2] + ", " + random_keys[3];
        }


        private async void MenuButton_Tapped(object sender, System.EventArgs e)
        {
            Debug.WriteLine("MenuButton_Tapped");
            if (IsAnimating) return;
            View view = sender as View;
            if (view.ClassId == "CloseBtn")
            {
                await AnimateImage(view, 250);
                await App.Current.MainPage.Navigation.PopModalAsync();
            }
            else if (view.ClassId == "DinoLingoLink")
            {
                Debug.WriteLine("ChooseLanguage_Page -> MenuButton_Tapped");
                Device.OpenUri(new System.Uri("https://dinolingo.com/"));
            }
            else if (view.ClassId == "DelBtn")
            {
                await AnimateImage(view, 250);
                if (numbersTapped > 0)
                {
                    numbersTapped--;
                    key_fields[numbersTapped].Text = string.Empty;
                }
            }
            else
            {
                await AnimateImage(view, 250);
                int index = int.Parse(view.ClassId) - 1;
                Debug.WriteLine("Clicked: " + view.ClassId);
                Debug.WriteLine("target :" + random_keys[numbersTapped]);
                Debug.WriteLine("pressed:" + key_values[index]);
                if (random_keys[numbersTapped] == key_values[index])
                {
                    // correct number
                    key_fields[numbersTapped].Text = (index + 1).ToString();
                    numbersTapped++;
                    if (numbersTapped >= 4)
                    {
                        Debug.WriteLine("*** Correct password! ***");
                        switch (WhatToDo)
                        {
                            case WhatToDoNext.WANT_TO_SIGN_UP:
                                // check current user
                                if (UserHelper.Login != null && !string.IsNullOrEmpty(UserHelper.Login.user_id)) // user is registered -> suggest subscriptions
                                {
                                    Debug.WriteLine("ParentCheck_Page -> user is registered -> suggest subscriptions");
                                    await Task.WhenAll(
                                        App.Current.MainPage.Navigation.PopModalAsync(),
                                        App.Current.MainPage.Navigation.PushModalAsync(new Subscribe_Page()));
                                }
                                else if (string.IsNullOrEmpty(UserHelper.Login.user_id) && UserHelper.TotalProducts() == 0) // user is not registered & has no products -> suggest subscriptions
                                {
                                    Debug.WriteLine("ParentCheck_Page -> user is not registered & has no products -> suggest subscriptions");
                                    await Task.WhenAll(
                                        App.Current.MainPage.Navigation.PopModalAsync(),
                                        App.Current.MainPage.Navigation.PushModalAsync(new Subscribe_Page())
                                        //App.Current.MainPage.Navigation.PushModalAsync(new SignUp_Page())
                                        );
                                }
                                else
                                { // user is not registered & has some products (IAP) -> suggest sign up
                                    Debug.WriteLine("ParentCheck_Page -> user is not registered & has some products (IAP) -> suggest sign up");
                                    await Task.WhenAll(
                                        App.Current.MainPage.Navigation.PopModalAsync(),
                                        App.Current.MainPage.Navigation.PushModalAsync(new SignUp_Page()));
                                }
                                break;
                            case WhatToDoNext.UPDATE_VERSION:
                                if (Device.RuntimePlatform == Device.iOS) Device.OpenUri(new System.Uri(RateWidget.RATE_LINK_IOS));
                                else if (Device.RuntimePlatform == Device.Android) Device.OpenUri(new System.Uri(RateWidget.RATE_LINK_ANDROID));

                                await Task.WhenAll(
                                    App.Current.MainPage.Navigation.PopModalAsync()
                                    );

                                break;
                            case WhatToDoNext.DEFAULT:
                                await Task.WhenAll(
                                    App.Current.MainPage.Navigation.PopModalAsync(),
                                    App.Current.MainPage.Navigation.PushModalAsync(new ParentMenu_Page()));
                                break;
                        }
                    }
                }
                else
                {
                    // wrong number!
                    Debug.WriteLine("Wrong numbers!");

                    // make pop-up and generate a new code
                    await App.Current.MainPage.DisplayAlert("", Translate.GetString("parent_check_wrong_password"), POP_UP.OK);

                    Init();
                    //await App.Current.MainPage.Navigation.PopModalAsync();
                }

            }
            IsAnimating = false;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Send((ContentPage)this, "ForcePortrait");
            ScreenOrientation.Instance.ForcePortrait();

            if (launch == 1)
            {
                launch++;
                //Device.StartTimer(TimeSpan.FromSeconds(1), ParentCheckTimer);
            }

        }

        bool ParentCheckTimer()
        {
            Debug.WriteLine("ParentCheck_Page -> Timer");

            /*
            Debug.WriteLine(String.Format("fiels == null?: {0}, WeakReference(key_0).IsAlive? {1}, content IsAlive = {2}", key_fields == null, _weak.IsAlive, _weak_content.IsAlive));
            if (_weak.IsAlive)
            {
                Debug.WriteLine(String.Format("_weak.text = {0}", (_weak.Target as Label).Text));

            }
            */

            return true;
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

        public void Dispose()
        {
            Debug.WriteLine("ParentCheck_Page -> Dispose");
            
            Content = null;
            BindingContext = null;
            
            key_fields = null;
        }
    }
}
