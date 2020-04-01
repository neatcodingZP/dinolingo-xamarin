using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

namespace DinoLingo
{
    public partial class MyListView : ContentView
    {
        Image[] subCatsImages;

        void Handle_ItemDisappearing(object sender, Xamarin.Forms.ItemVisibilityEventArgs e)
        {
           // Debug.WriteLine("item disappearing...");
            viewModel.Handle_ItemDisappearing(sender, e);
        }

        void Handle_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            viewModel.Handle_ItemAppearing(sender, e);
        }

        public TestViewModel viewModel;

        public MyListView(FavoriteListCoords centralViewType, MainPage_ViewModel rootViewModel)
        {
            InitializeComponent();
            BindingContext = viewModel = new TestViewModel(centralViewType, rootViewModel, listView, rootStack, subMenuBar);
        }

        private void ItemTapped(object sender, ItemTappedEventArgs e)
        {
            viewModel.ItemTapped(sender, e);
        }

        private void ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            viewModel.ItemSelected(sender, e);
        }

        private void SubCat_Tapped(object sender, ItemTappedEventArgs e)
        {
            viewModel.SubCat_Tapped(sender, e);
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

        private void Favorites_OnTapped(object sender, MyListItemEventArgs e)
        //-------------------------------------------------------------
        {
            viewModel.Favorites_OnTapped(sender, e);
            var item = e.MyItem;
            if (item == null) return;
        }

        private void Favorites_OnTapped2(object sender, MyListItemEventArgs e)
        //-------------------------------------------------------------
        {
            viewModel.Favorites_OnTapped2(sender, e);
            var item = e.MyItem;
            if (item == null) return;
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

        public void Dispose()
        {
            Debug.WriteLine("MyListView -> Dispose");
            Content = null;
            BindingContext = null;
            subCatsImages = null;
            viewModel.Dispose();
            viewModel = null;
        }
    }
}
