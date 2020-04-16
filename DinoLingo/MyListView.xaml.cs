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

        //private void ItemTapped(object sender, ItemTappedEventArgs e)
        private void ItemTapped(object sender, EventArgs e)
        {
            viewModel.ItemTapped(sender, e as ItemTappedEventArgs);
        }

        private void ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            viewModel.ItemSelected(sender, e);
        }

        //private void SubCat_Tapped(object sender, ItemTappedEventArgs e)
        private void SubCat_Tapped(object sender, EventArgs e)
        {
            viewModel.SubCat_Tapped(sender, e as ItemTappedEventArgs);
        }

        //private void Name_OnTapped(object sender, MyListItemEventArgs e)
        private void Name_OnTapped(object sender, EventArgs e)
        //-------------------------------------------------------------
        {
            Debug.WriteLine("MyListView -> Name_OnTapped");

            var item = (e as TappedEventArgs).Parameter as ListViewItem;
            if (item == null) return;

            viewModel.Name_OnTapped(sender, item); 


            //var item = (e as MyListItemEventArgs).MyItem;            
            
        }

        //private void Favorites_OnTapped(object sender, MyListItemEventArgs e)
        private void Favorites_OnTapped(object sender, EventArgs e)
        //-------------------------------------------------------------
        {
            var item = (e as TappedEventArgs).Parameter as ListViewItem;
            if (item == null) return;

            viewModel.Favorites_OnTapped(sender, item);
            
        }

        //private void Favorites_OnTapped2(object sender, MyListItemEventArgs e)
        private void Favorites_OnTapped2(object sender, EventArgs e)
        //-------------------------------------------------------------
        {
            var item = (e as TappedEventArgs).Parameter as ListViewItem;
            if (item == null) return;

            viewModel.Favorites_OnTapped2(sender, item);
        }

        //private void Name_OnTapped2(object sender, MyListItemEventArgs e)
        private void Name_OnTapped2(object sender, EventArgs e)
        //-------------------------------------------------------------
        {
            var item = (e as TappedEventArgs).Parameter as ListViewItem;
            if (item == null) return;

            viewModel.Name_OnTapped2(sender, item);
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
