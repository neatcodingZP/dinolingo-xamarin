using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace DinoLingo
{
    public partial class WelcomePage_ : ContentPage
    {
        public static WelcomePage_ instance;

        public WelcomePage_()
        {
            InitializeComponent();
            instance = this;
        }

        private void OnHaveAccountClicked(object sender, EventArgs e)
        {
            
            App.Current.MainPage.Navigation.PushAsync(new LoginPage());
            //Navigation.RemovePage(this);
        } 

        private void OnGetAccountClicked(object sender, EventArgs e)
        {
            //   implement registration on the web-site here

            Console.WriteLine("Go to dinolingo for the account --->");
        } 
    }
}
