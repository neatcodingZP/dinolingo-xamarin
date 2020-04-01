using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using FFImageLoading.Forms;
using Xamarin.Forms;

namespace DinoLingo
{
    public partial class LeftMenuView : ContentView
    {
        uint PARK_BACK_TIME = 100;
        uint OPEN_CLOSE_TIME = 250;
        uint BTN_ANIM_TIME = 200;

        public string HaveStars { get; set; }
        public double HaveStarsTextSize { get; set; }
        public ImageSource LastDinosaurImgSource { get; set; }
        public string LastDinosaurName { get; set; }
        public string Stats { get; set; }
        public double StatsTextSize { get; set; }
        public double SeeAllTextSize { get; set; }
        
        public bool IsLastDinosaurVisible { get; set; }

        public double BtnCornerRadius { get; set; }
        public double StrokeLineWidth { get; set; }
        public double BtnShadowOffset { get; set; }

        MySlideView parent;
        bool isAnimating;
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

        MainPage_ViewModel mainPage_ViewModel;
        public CachedImage[] dinos_imgs;

        //ImageSource closedBtn = Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.btn_right.png");
        //ImageSource openedBtn = Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.btn_left.png");

        public LeftMenuView(MySlideView parent, MainPage_ViewModel mainPage_ViewModel)
        {
            InitializeComponent();
            BindingContext = this;
            IsAnimating = false;
            this.parent = parent;
            this.mainPage_ViewModel = mainPage_ViewModel;
            dinos_imgs = new CachedImage[] { dino_0, dino_1,  dino_2, dino_3, dino_4, dino_5};
            HaveStarsTextSize = UI_Sizes.MediumTextSize;
            StatsTextSize = UI_Sizes.MediumTextSize;
            SeeAllTextSize = UI_Sizes.SmallTextSize;
            BtnCornerRadius = UI_Sizes.SmallTextSize * 0.4;
            StrokeLineWidth = UI_Sizes.SmallTextSize * 0.2;
            BtnShadowOffset = StrokeLineWidth * 0.6;
            NonMembersLabel.FontSize = UI_Sizes.MicroTextSize;
            transparentFrame.CornerRadius = UI_Sizes.ButtonCornerRadius;

            if (UserHelper.Login == null || string.IsNullOrEmpty(UserHelper.Login.user_id))
            {
                NonMembersLabel.IsVisible = true;
            }
            else
            {
                NonMembersLabel.IsVisible = false;
            }
        }

        public void ChangeOpenButtonImage(bool open) {
            /*
            if (open) {
                OpenButton.Source = openedBtn;

            }
            else {
                OpenButton.Source = closedBtn;
            }
            */
        }

        public void ShowReport(ShortReportResponse.ShortReport shortReport) {
            if (shortReport == null) return;

            if (shortReport.new_dino != null) {
                if (!string.IsNullOrEmpty(shortReport.new_dino.img)) LastDinosaurImgSource = shortReport.new_dino.img;
                LastDinosaurName = shortReport.new_dino.title;
                IsLastDinosaurVisible = true;
            }

            int dinos = 0;
            if (shortReport.dinos != null && shortReport.dinos.Length > 0) {
                //last dino 
                /*
                if (!string.IsNullOrEmpty(shortReport.dinos[shortReport.dinos.Length-1].img)) {
                    LastDinosaurImgSource = shortReport.dinos[shortReport.dinos.Length - 1].img;
                    //LastDinosaurName = shortReport.dinos[0].title;
                    IsLastDinosaurVisible = true;
                }
*/

                dinos = shortReport.dinos.Length;               
                for (int i = 0; i < dinos_imgs.Length; i++) {
                    if (i < shortReport.dinos.Length) {
                        if (!string.IsNullOrEmpty(shortReport.dinos[i].img)) dinos_imgs[i].Source = shortReport.dinos[shortReport.dinos.Length - 1 - i].img;
                        dinos_imgs[i].IsVisible = true;
                    }
                    else {
                        dinos_imgs[i].IsVisible = false;
                    }
                }
            }
            if (shortReport.stars > 0) {
                StarButton.IsVisible = true;
                HaveStars = shortReport.stars.ToString();
            }
            else {
                StarButton.IsVisible = false;
                HaveStars = string.Empty;
            }
            

            if (dinos > 0) {
                SeeAllButton.IsVisible = true;
                SeeAllButton.IsEnabled = true;
                StatsButton.IsVisible = true;
                Stats = dinos + "/" + shortReport.max_dinos_cnt;
            }
            else
            {
                SeeAllButton.IsVisible = false;
                SeeAllButton.IsEnabled = false;
                StatsButton.IsVisible = false;
                Stats = string.Empty; 
            }
             
            if (UserHelper.Login == null || string.IsNullOrEmpty(UserHelper.Login.user_id))
            {
                NonMembersLabel.IsVisible = true; 
            }
            else
            {
                NonMembersLabel.IsVisible = false;
            }
        }


        public void ClearAllStats() {
            Debug.WriteLine("ClearAllStats()");

            IsLastDinosaurVisible = false;
            //IsMyDinosaursVisible = false;
            for (int i = 0; i < dinos_imgs.Length; i++) dinos_imgs[i].IsVisible = false;
            StarButton.IsVisible = false;
            StatsButton.IsVisible = false;
            SeeAllButton.IsVisible = false;
            SeeAllButton.IsEnabled = false;
        }

        public void SetFavoritesButton()
        {
            Debug.WriteLine($"LeftMenuView -> SetFavoritesButton -> Favorites.Books = {Favorites.Books}, Favorites.Videos = {Favorites.Videos}");
            FavoritesButton.IsEnabled = FavoritesButton.IsVisible = (Favorites.Books + Favorites.Videos > 0);
        }

        async void ImageTapped (object sender, EventArgs args) {
            Debug.WriteLine("Image Tapped ---> parent.State= " + parent.State + ", CanPan=" + parent.CanPan);

            if (IsAnimating) return;
            IsAnimating = true;

            if (parent.State == MySlideView.STATE.PANNING) {
                parent.ParkBack(PARK_BACK_TIME);
                Debug.WriteLine("parent ---> ParkBack()");
                IsAnimating = false; 
                return;
            }

            if (parent.State > MySlideView.STATE.IDLE || !parent.CanPan)
            {
                IsAnimating = false;
                return;
            }
            if (parent.TranslationX != 0)
            {
                IsAnimating = false;
                return;
            }

            View view = sender as View;
            if (view.ClassId == "OpenButton") {
                await AnimateImage(view, BTN_ANIM_TIME);
                parent.OpenClose(OPEN_CLOSE_TIME);
            }

            else if (view.ClassId == "LastDinosaur")
            {
                //await AnimateImage(view, BTN_ANIM_TIME);
            }
            else if (view.ClassId == "FavoritesButton")
            {
                Page page = new Favorites_Page(mainPage_ViewModel);
                await AnimateImage(view, BTN_ANIM_TIME);
                if (parent.pairedView.IsOpened) parent.pairedView.ForceClose(MySlideView.FORCE_CLOSE_TIME);
                await mainPage_ViewModel.navigation.PushModalAsync(page);
            }            
            else if (view.ClassId == "SeeAllButton")
            {
                Page page = new BadgeList_Page(mainPage_ViewModel.navigation);
                await AnimateImage(view, BTN_ANIM_TIME);
                if (parent.pairedView.IsOpened) parent.pairedView.ForceClose (MySlideView.FORCE_CLOSE_TIME);
                await mainPage_ViewModel.navigation.PushModalAsync(page);
            }

            Debug.WriteLine("ImageTapped =" + view.ClassId);
            IsAnimating = false;
        }


        public Task AnimateImage(View view, uint time) {            
            return Task.Run(async () =>
            {
                try
                {
                    await view.ScaleTo(0.9, time / 2);
                    await view.ScaleTo(1.0, time / 2);
                }
                catch (Exception ex)
                {

                }                
                return;
            });
        }

        public void Dispose ()
        {
            Content = null;
            BindingContext = null;
            LastDinosaurImgSource = null;
            parent = null;
            animLock = null;
            mainPage_ViewModel = null;
            foreach (CachedImage img in dinos_imgs) img.Source = null;
            dinos_imgs = null;
        }
    }
}
