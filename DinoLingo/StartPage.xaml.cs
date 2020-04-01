using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace DinoLingo
{
    public partial class StartPage : ContentPage
    {
        public StartPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            // if first launch - go to welcome page
            if (true)
            {
                Console.WriteLine("OnStart() ---> await Navigation.PushAsync (new NavigationPage (new WelcomePage()))");
                /*
                Navigation.InsertPageBefore(new WelcomePage(), this);
                await Navigation.PopAsync().ConfigureAwait(true);
                */

                //Navigation.PopAsync();
                Navigation.PushAsync(new WelcomePage_());
                Navigation.RemovePage(this);

                /*
                NavigationPage navPage = (NavigationPage)Application.Current.MainPage;
                navPage.Navigation.RemovePage(this);
                 */


            }
            // if not loggged in  - go to login page
            else if (true)
            {
                Navigation.PushAsync(new NavigationPage(new WelcomePage_()));
            }
            //if logged in  - go to main page
            else
            {
                Navigation.PushAsync(new NavigationPage(new WelcomePage_()));
            }

        }
    }
}
