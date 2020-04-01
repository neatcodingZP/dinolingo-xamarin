using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DinoLingo
{
    public partial class RightMenuView : ContentView
    {
        uint PARK_BACK_TIME = 100;
        uint OPEN_CLOSE_TIME = 250;
        uint BTN_ANIM_TIME = 200;

        public string LanguageText { get; set; }
        public double CornerRadius { get; set; }
        public double LanguageTextSize { get; set; }
        public double BtnTextSize { get; set; }
        public double BtnSmallTextSize { get; set; }

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
        
        Image[] cat_images;
        MyLabel[] labels;
        Grid[] btn_grids;

        //ImageSource openedBtn = Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.btn_right.png");
        //ImageSource closedBtn = Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.btn_left.png");

        public RightMenuView(MySlideView parent, MainPage_ViewModel mainPage_ViewModel)
        {
            InitializeComponent();
            BindingContext = this;
            IsAnimating = false;            
            this.parent = parent;
            this.mainPage_ViewModel = mainPage_ViewModel;
            cat_images = new Image[] { l_0_img, l_1_img, l_2_img, l_3_img };
            btn_grids = new Grid[] { g_0, g_1, g_2, g_3 };
            labels = new MyLabel[] { l_0, l_1, l_2, l_3 };

            CornerRadius = UI_Sizes.SmallTextSize;
            LanguageTextSize = UI_Sizes.SmallTextSize * 0.85;
            BtnTextSize = UI_Sizes.SmallTextSize * 0.75;
            BtnSmallTextSize = UI_Sizes.SmallTextSize * 0.75;
            transparentFrame.CornerRadius = UI_Sizes.ButtonCornerRadius;
        }

        public void ChangeOpenButtonImage(bool open)
        {
            /*
            if (open)
            {
                OpenButton.Source = openedBtn;
            }
            else
            {
                OpenButton.Source = closedBtn;
            }
            */
        }

        public async void SetCategoriesButtons() {
            //get allcats list
            //check if we have categories for current language
            CategoryResponse categoryResponse = await CacheHelper.GetAsync<CategoryResponse>(CacheHelper.CATEGORYS_RESPONSE + UserHelper.Lang_cat);
            for (int i = 0; i < cat_images.Length; i ++) {
                if (i < categoryResponse.result.Length) {
                    switch (categoryResponse.result[i].viewType) {
                        case MainPage_ViewModel.CENTRAL_VIEWS.LESSONS_AND_GAMES: 
                            cat_images[i].Source = Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.btn_lessons.png");
                            labels[i].Text = Translate.GetString("right_menu_lessons");
                            break;
                        case MainPage_ViewModel.CENTRAL_VIEWS.BOOKS:
                            cat_images[i].Source = Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.btn_books.png");
                            labels[i].Text = Translate.GetString("right_menu_books");
                            break;
                        case MainPage_ViewModel.CENTRAL_VIEWS.STORIES:
                            cat_images[i].Source = Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.btn_stories.png");
                            labels[i].Text = Translate.GetString("right_menu_stories");
                            break;
                        case MainPage_ViewModel.CENTRAL_VIEWS.SONGS:
                            cat_images[i].Source = Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.btn_songs.png");
                            labels[i].Text = Translate.GetString("right_menu_songs");
                            break;
                    }
                }
                else {
                    btn_grids[i].IsVisible = false;
                    btn_grids[i].IsEnabled = false;
                }
            }

            // set visability of dinos in the bottom
            dinoBig.IsVisible = dinoSmall.IsVisible = false;
            if (categoryResponse.result.Length < 3)
            {
                dinoBig.IsVisible = true;
            }
            else if (categoryResponse.result.Length < 4)
            {
                dinoSmall.IsVisible = true;
            }
        }

        public void SetAdditionalButtons() {
            
            string userName = string.IsNullOrEmpty(UserHelper.Login.user_id)? Translate.GetString("default_user_name") : UserHelper.Login.display_name;

            LanguageText = LANGUAGES.CAT_INFO[UserHelper.Lang_cat].VisibleName.FirstLetterToUpperCase();
        }


        async void ImageTapped(object sender, EventArgs args)
        {
            Debug.WriteLine("parent.State= " + parent.State + ", CanPan=" + parent.CanPan);

            if (IsAnimating) return;
            IsAnimating = true;


            if (parent.State == MySlideView.STATE.PANNING)
            {
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
            if (view.ClassId == "OpenButton")
            {
                await AnimateImage(view, BTN_ANIM_TIME);
                parent.OpenClose(OPEN_CLOSE_TIME);
            }
            else if (view.ClassId == "ParentLockBtn")
            {
                await AnimateImage(view, BTN_ANIM_TIME);
                if (parent.IsOpened) parent.ForceClose(MySlideView.FORCE_CLOSE_TIME);
                if (parent.pairedView.IsOpened) parent.pairedView.ForceClose(MySlideView.FORCE_CLOSE_TIME);
                 await App.Current.MainPage.Navigation.PushModalAsync(new ParentCheck_Page());
                Debug.WriteLine("ParentLockBtn");
            }

            else if (view.ClassId == "0")
            {
                await AnimateImage(view, BTN_ANIM_TIME);
                if (parent.IsOpened) parent.ForceClose(MySlideView.FORCE_CLOSE_TIME);
                if (parent.pairedView.IsOpened) parent.pairedView.ForceClose(MySlideView.FORCE_CLOSE_TIME);
                parent.rootViewModel.SwitchCentralView(0);

            }

            else if (view.ClassId == "1")
            {
                await AnimateImage(view, BTN_ANIM_TIME);
                if (parent.IsOpened) parent.ForceClose(MySlideView.FORCE_CLOSE_TIME);
                if (parent.pairedView.IsOpened) parent.pairedView.ForceClose(MySlideView.FORCE_CLOSE_TIME);
                parent.rootViewModel.SwitchCentralView(1);
            }

            else if (view.ClassId == "2")
            {
                await AnimateImage(view, BTN_ANIM_TIME);
                if (parent.IsOpened) parent.ForceClose(MySlideView.FORCE_CLOSE_TIME);
                if (parent.pairedView.IsOpened) parent.pairedView.ForceClose(MySlideView.FORCE_CLOSE_TIME);

                parent.rootViewModel.SwitchCentralView(2);
            }

            else if (view.ClassId == "3")
            {
                await AnimateImage(view, BTN_ANIM_TIME);
                if (parent.IsOpened) parent.ForceClose(MySlideView.FORCE_CLOSE_TIME);
                if (parent.pairedView.IsOpened) parent.pairedView.ForceClose(MySlideView.FORCE_CLOSE_TIME);
                parent.rootViewModel.SwitchCentralView(3);
            }
            Debug.WriteLine("ImageTapped =" + view.ClassId);
            IsAnimating = false;
        }


        public Task AnimateImage(View view, uint time)
        {            
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

        public void Dispose()
        {
            Content = null;
            BindingContext = null;
            parent = null;
            animLock = null;
            mainPage_ViewModel = null;

            cat_images = null;
            labels = null;
            btn_grids = null;
        }

    }
}
