using Xamarin.Forms;
using System.Diagnostics;
using System;
using System.Threading.Tasks;
using DinoLingo.ScreenOrientations;

namespace DinoLingo
{
    public partial class MainPage_ : ContentPage
    {
        
        public MainPage_ViewModel viewModel;
        int id = -1;

        public MainPage_(MainPage_ViewModel.CENTRAL_VIEWS viewType, int id = -1, bool needToSignUp = false)
        {
            InitializeComponent();
            BindingContext = viewModel = new MainPage_ViewModel(Navigation, 
                new FavoriteListCoords()
                {
                    CentralView = viewType,
                    SubView = 0
                }, id, needToSignUp
            );
            viewModel.AddCenterView(centralView);
            viewModel.AddSlideViews(rootRelaytiveLayout);
            this.id = id;
        }

        protected override void OnAppearing()
        {
            Debug.WriteLine("MainPage_ -> OnAppearing()");
            base.OnAppearing();
            MessagingCenter.Send((ContentPage)this, "ForceLandscape");
            ScreenOrientation.Instance.ForceLandscape();
            viewModel?.OnAppearing();
            Debug.WriteLine(string.Empty);
            if (RateWidget.State.justPurchased > 0) RateWidget.CheckRateWidget(App.Current.MainPage.Navigation, this);  
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (viewModel != null) viewModel.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            Debug.WriteLine("MainPage_ -> OnBackButtonPressed");
            if (!viewModel.OnBackButtonPressed()) return true;

            Debug.WriteLine("MainPage_ -> OnBackButtonPressed -> base.OnBackButtonPressed()");
            return base.OnBackButtonPressed();
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();
            id = id;
            if (Parent == null)
            {
                viewModel.Dispose();
                Dispose();
                GC.Collect();
                MemoryLeak.TrackMemory();
            }
        }

        void Dispose()
        {
            Debug.WriteLine("MainPage_ -> Dispose");

            Content = null;
            BindingContext = null;
            if (viewModel != null) viewModel.OnDisappearing();
            viewModel = null;
        }

        public Task PushPage(Page page)
        {            
            return Task.Run(async () =>
            {
                await Navigation.PushAsync(page);
                return;
            });
        }
    }
}
