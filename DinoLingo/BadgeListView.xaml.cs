using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace DinoLingo
{
    public partial class BadgeListView : ContentView
    {
        public BadgeListView_ViewModel viewModel;

        public BadgeListView()
        {
            InitializeComponent();
            BindingContext = viewModel = new BadgeListView_ViewModel();
        }

        private void Name_OnTapped(object sender, MyListItemEventArgs e)
        //-------------------------------------------------------------
        {
            viewModel.Name_OnTapped(sender, e);
            var item = e.MyItem;
            if (item == null) return;
            // now you can fully access the listview item here via the variable "item"
            // ...
        }

        private void Name_OnTapped2(object sender, MyListItemEventArgs e)
        //-------------------------------------------------------------
        {
            viewModel.Name_OnTapped2(sender, e);
            var item = e.MyItem;
            if (item == null) return;
            // now you can fully access the listview item here via the variable "item"
            // ...
        }

        private void Name_OnTapped3(object sender, MyListItemEventArgs e)
        //-------------------------------------------------------------
        {
            viewModel.Name_OnTapped3(sender, e);
            var item = e.MyItem;
            if (item == null) return;
            // now you can fully access the listview item here via the variable "item"
            // ...
        }

        private void Name_OnTapped4(object sender, MyListItemEventArgs e)
        //-------------------------------------------------------------
        {
            viewModel.Name_OnTapped4(sender, e);
            var item = e.MyItem;
            if (item == null) return;
            // now you can fully access the listview item here via the variable "item"
            // ...
        }

    }
}

