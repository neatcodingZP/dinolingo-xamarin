using System;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using Plugin.SimpleAudioPlayer;
using Plugin.Connectivity;
using Newtonsoft.Json;
using FFImageLoading;

namespace DinoLingo
{
    public class SASViewModel: INotifyPropertyChanged
    {
        static int ERROR_PREFIX = 120;

        public event PropertyChangedEventHandler PropertyChanged;
        public INavigation navigation;
        public THEME_NAME theme;

        public ImageSource BackgroundImageSource { get; set; }
        public Forms9Patch.Fill BackgroundFillMode { get; set; }
        public Color BackgroundColor { get; set; }

        public string LoadingWaitText { get; set; }
        public bool IsGifVisible { get; set; }
        public ImageSource GifSource { get; set; }
        public double CloseBtnSize { get; set; }
        //public string LoadingPercentText { get; set; }
        //public double ProgressValue { get; set; }

        BackButtonView backButtonView;
        RelativeLayout gameLayout;
        AbsoluteLayout absRoot;
        RelativeLayout totalLayout;
        View loadingView;

        bool isAnimating = false;

        List<InteractiveView> interactiveImages;
        List<Label> labels;
        List<int> unTappedIndexes = new List<int>();
        List<BoundView> boundViews;
        VictoryView victoryView;
        TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
        int gameIndex;

        bool firstStart = true;
        string language;
        ListItemStarsInfo starsInfo;
        PostResponse.Post post;
        ActivityReport activityReport;
        double progressRatio = 1.0;
        int totalObjects;

        public SASViewModel(INavigation navigation, THEME_NAME theme, int gameIndex, AbsoluteLayout absRoot, RelativeLayout totalLayout, View loadingView,  ListItemStarsInfo starsInfo) 
        {
            this.absRoot = absRoot;
            this.totalLayout = totalLayout;
            this.loadingView = loadingView;
            this.navigation = navigation;
            this.theme = theme;
            this.gameIndex = gameIndex;
            this.starsInfo = starsInfo;
            CloseBtnSize = UI_Sizes.CloseBtnSize;

            tapGestureRecognizer.Tapped += (sender, e) =>
            {

                BoundView v = (BoundView)sender;
                Debug.WriteLine("tapped bond view, id=" + v.id);
                AnimateObject(v.id);
            };

            App.OnSleepEvent += OnSleep;
            App.OnResumeEvent += OnResume;

            int index = starsInfo.isSingleItem ? starsInfo.eventArgs.MyItem.Index : (starsInfo.eventArgs.MyItem as DoubleItem).Index2;
            activityReport = new ActivityReport(ActivityReport.ACT_TYPE.GAME, starsInfo.data[index].id);

            progressRatio = (theme == THEME_NAME.ANIMALS) ? 0.5 : 1.0;
        }

        public void OnClose()
        {
            // dispose objects
            if (App.Audio.playerVictory != null && App.Audio.playerVictory.IsPlaying) App.Audio.playerVictory.Stop();

            App.OnSleepEvent -= OnSleep;
            App.OnResumeEvent -= OnResume;
            activityReport.OnDisappearing();
        }

        void OnSleep()
        {
            if (App.Audio.playerVictory != null && App.Audio.playerVictory.IsPlaying) App.Audio.playerVictory.Stop();
            activityReport.OnSleep();
        }

        void OnResume()
        {
            activityReport.OnResume();
        }

        public void OnLoadingAppeared()
        {
            //LoadingPercentText = "Checking data for game";
            //ProgressValue = 0;
            language = UserHelper.Language;
        }

        void OnLoadingStarted()
        {
            //LoadingWaitText = "Please wait... dowloading audio ...";
            //IsGifVisible = true;
            //LoadingPercentText = "0 %";
            IsGifVisible = true;
            GifSource = LoadingView_Logic.GetRandomDancingDinoImg();
        }

        void OnLoadingProgress()
        {
            Debug.WriteLine("OnLoadingProgress() --> progress = " + DownloadHelper.DownloadHelper.progress);
            //ProgressValue = DownloadHelper.DownloadHelper.progress;
            //LoadingPercentText = ((int)(ProgressValue * 100)).ToString() + " %";
        }

        async void OnLoadingEnded()
        {
            Debug.WriteLine("OnLoadingEnded() --> progress = " + DownloadHelper.DownloadHelper.progress);
            switch (DownloadHelper.DownloadHelper.result)
            {
                case DownloadHelper.DownloadHelper.RESULT.NONE:
                case DownloadHelper.DownloadHelper.RESULT.ERROR:
                case DownloadHelper.DownloadHelper.RESULT.DOWNLOAD_STARTED:
                    if (AsyncMessages.CheckDisplayAlertTimeout())
                    {
                        await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                       POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(null, ERROR_PREFIX + 1), POP_UP.OK);
                        //("Error!", "Error, while loading audio from server..., download.result = " + DownloadHelper.DownloadHelper.result, "Cancel");
                    }

                    await navigation.PopModalAsync();
                    break;
                case DownloadHelper.DownloadHelper.RESULT.ERROR_NO_CONNECTION:
                    if (AsyncMessages.CheckConnectionTimeout())
                    {
                        await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                       POP_UP.NO_CONNECTION
                       //+ POP_UP.GetCode(null, ERROR_PREFIX + 2)
                       , POP_UP.OK);
                        //("Error!", "No internet! Check your internet connection.", "Cancel");
                    }

                    await navigation.PopModalAsync();
                    break;
                case DownloadHelper.DownloadHelper.RESULT.NOTHING_TO_DOWNLOAD:
                    Debug.WriteLine("nothing to download");
                    //Download post if we need it
                    if ((post = await victoryView.DownloadPost(starsInfo)) != null)
                    {
                        SetAllAfterLoading();
                        AnimateLoadingView(1000);
                    }
                    else {
                        SetAllAfterLoading();
                        AnimateLoadingView(1000);
                       
                        //await navigation.PopModalAsync();
                    }
                    break;
                case DownloadHelper.DownloadHelper.RESULT.DOWNLOADED_OK:
                    Debug.WriteLine("downloaded - ok, total files = " + DownloadHelper.DownloadHelper.totalFilesDownloaded);
                    //Download post if we need it
                    if ((post = await victoryView.DownloadPost(starsInfo)) != null)
                    { // post downloaded ok
                        SetAllAfterLoading();
                        AnimateLoadingView(1000);
                    }
                    else
                    {
                        SetAllAfterLoading();
                        AnimateLoadingView(1000);
                        //await App.Current.MainPage.DisplayAlert("Error!", "Some error in post data", "Cancel");
                        //await navigation.PopModalAsync();
                    }
                    break;
            }
        }

        async void SetAllAfterLoading() {
            Debug.WriteLine("SetAllAfterLoading() --> post == null ? :" + (post == null));

            for (int i = 0; i < labels.Count; i++) {
                string key = SAS_Data.all[theme][gameIndex].Labels[i].KeyName;
                labels[i].Text = GameHelper.sas_GameObjects.GetTextForSound(theme, key);
                /*
                if (await CacheHelper.Exists(CacheHelper.TEXT_FOR_KEY + key + language))
                    labels[i].Text = (await CacheHelper.GetAsync(CacheHelper.TEXT_FOR_KEY + key + language)).Data;
                else labels[i].Text = string.Empty;
                */
            } 

            foreach (InteractiveView v in interactiveImages) {
                
                    if (//GameHelper.memory_GameObjects.HasKey(theme, key) || 
                        GameHelper.sas_GameObjects.HasKey(theme, v.Key))
                        {
                        if (v.InnerLabel != null) v.InnerLabel.Text = GameHelper.sas_GameObjects.GetTextForSound(theme, v.Key);
                        /*
                        if (await CacheHelper.Exists(CacheHelper.TEXT_FOR_KEY + v.Key + language))
                        v.InnerLabel.Text = (await CacheHelper.GetAsync(CacheHelper.TEXT_FOR_KEY + v.Key + language)).Data;
                        else v.InnerLabel.Text = string.Empty;
                        */
                        Debug.WriteLine("interactive object - show: " + v.Key);
                        }
                    else {
                    Debug.WriteLine("do not show object - show: " + v.Key);    
                    unTappedIndexes.Remove(v.id);
                        v.IsVisible = false;
                        v.IsEnabled = false;
                    }
            }
        }

        async void AnimateLoadingView(uint time)
        {
            try {
                await loadingView.TranslateTo(0, -App.Current.MainPage.Height, time);
                loadingView.IsVisible = false;
                loadingView.InputTransparent = true;
                activityReport.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("SASViewModel -> AnimateLoadingView -> ex:" + ex.Message);
            }
        }

        public async void OnAppearing()
        {
            if (firstStart)
            {
                OnLoadingAppeared();
                Device.StartTimer(TimeSpan.FromMilliseconds(500), StartLoading);
                firstStart = false;
            }
        }

        bool StartLoading()
        {
            Debug.WriteLine("StartLoading ()");

            InitOnStart();
            AddInteactiveObjects();
            AddLabels();
            AddVictoryView();
            AddGameLayout();
            AddCloseButton();

            List<string> wordsToDownload1 = new List<string>(Theme.Resources[(int)theme].Item.Keys);
            foreach (string key in Theme.Resources[(int)theme].Item.Keys)
            {
                if (//GameHelper.memory_GameObjects.HasKey(theme, key) || 
                    GameHelper.sas_GameObjects.HasKey(theme, key))
                {
                    continue;
                }
                wordsToDownload1.Remove(key);
                Debug.WriteLine("removed from downloading : " + key);
            }

            DownloadHelper.DownloadHelper.OnLoadingStarted = OnLoadingStarted;
            DownloadHelper.DownloadHelper.OnLoadingProgress = OnLoadingProgress;
            DownloadHelper.DownloadHelper.OnLoadingEnded = OnLoadingEnded;

            DownloadHelper.DownloadHelper.DownLoadWords(wordsToDownload1, theme, null, theme);
            return false;
        }

        public void AddVictoryView()
        {
            victoryView = new VictoryView(navigation, OnClose, starsInfo);
            AbsoluteLayout.SetLayoutBounds(victoryView, new Rectangle(0, 0, 1, 1));
            AbsoluteLayout.SetLayoutFlags(victoryView, AbsoluteLayoutFlags.All);
            absRoot.Children.Add(victoryView);
        }

        public void InitOnStart()
        {
            Debug.WriteLine("InitOnStart()");

            gameLayout = new RelativeLayout { BackgroundColor = Color.Transparent};
            if (SAS_Data.all[theme][gameIndex].Background.Fill == Forms9Patch.Fill.Tile)
            {
                BackgroundFillMode = SAS_Data.all[theme][gameIndex].Background.Fill;
                BackgroundImageSource = Forms9Patch.ImageSource.FromResource(SAS_Data.all[theme][gameIndex].GetImageSourceBackground());
            }
            else
            {
                BackgroundColor = SAS_Data.all[theme][gameIndex].Background.color;
                Image gameBackground = new Image
                    {
                    Source = Forms9Patch.ImageSource.FromResource(SAS_Data.all[theme][gameIndex].GetImageSourceBackground()),
                    Aspect = Aspect.Fill,
                    IsEnabled = false};

                gameLayout.Children.Add(gameBackground,
                Constraint.RelativeToParent((parent) =>
                {
                    return 0;    // установка координаты X
                }),
                Constraint.RelativeToParent((parent) =>
                {
                    return 0;   // установка координаты Y
                }),

                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Width;
                }), // установка ширины
                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Height;
                })
                );
            }

            interactiveImages = new List<InteractiveView>();
            boundViews = new List<BoundView>();
        }

        public void AddCloseButton() {
            Debug.WriteLine("AddCloseButton()");

            backButtonView = new BackButtonView();
            double btn_size = 0.1;

            totalLayout.Children.Add(backButtonView,
                Constraint.RelativeToParent((parent) =>
                {
                return parent.Width - parent.Height * btn_size;    // установка координаты X
                }),
                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Height * 0;   // установка координаты Y
                }),

                Constraint.RelativeToParent((parent) =>
                {
                return parent.Height * btn_size;
                }), // установка ширины
                Constraint.RelativeToParent((parent) =>
                {
                return parent.Height * btn_size;
                })
                );
            
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (sender, e) =>
            {
                BackButtonView view = (BackButtonView) sender;
                if (view != null) OnClickedClose(view);
                // now you have a reference to the image
                Debug.WriteLine("tapped back");
            };

            backButtonView.GestureRecognizers.Add(tapGestureRecognizer);
        }

        public void AddInteactiveObjects() {

            int staticImageIndex = 0;
            for (int i = 0; i < SAS_Data.all[theme][gameIndex].ActiveImages.Count; i++)
            {

                //add all static images, if we have to
                staticImageIndex = AddAllStaticImagesToLowerLayout(staticImageIndex, i);

                InteractiveView view = new InteractiveView(SAS_Data.all[theme][gameIndex].ActiveImages[i].InnerLabel, SAS_Data.all[theme][gameIndex].ActiveImages[i].KeyName) { 
                    IsEnabled = false, 
                    InputTransparent = true,
                    Key = SAS_Data.all[theme][gameIndex].ActiveImages[i].KeyName
                };
                
                view.AnchorX = SAS_Data.all[theme][gameIndex].ActiveImages[i].anchorX;
                view.AnchorY = SAS_Data.all[theme][gameIndex].ActiveImages[i].anchorY;  
                view.ImageSource = Forms9Patch.ImageSource.FromResource(SAS_Data.all[theme][gameIndex].GetImageSourceActiveImages(i));
                view.id = i;
                interactiveImages.Add(view);
                unTappedIndexes.Add(i);

                double x = SAS_Data.all[theme][gameIndex].ActiveImages[i].BaseCoords.x / SAS_Data.all[theme][gameIndex].Background.BaseRect.width;
                double y = SAS_Data.all[theme][gameIndex].ActiveImages[i].BaseCoords.y / SAS_Data.all[theme][gameIndex].Background.BaseRect.height;
                //double x = SAS_Data.all[theme][gameIndex].ActiveImages[i].BaseCoords.x / (SAS_Data.all[theme][gameIndex].Background.BaseRect.width - SAS_Data.all[theme][gameIndex].ActiveImages[i].BaseCoords.width);
                //double y = SAS_Data.all[theme][gameIndex].ActiveImages[i].BaseCoords.y / (SAS_Data.all[theme][gameIndex].Background.BaseRect.height - SAS_Data.all[theme][gameIndex].ActiveImages[i].BaseCoords.height);


                double width = SAS_Data.all[theme][gameIndex].ActiveImages[i].BaseCoords.width / SAS_Data.all[theme][gameIndex].Background.BaseRect.width;
                double height = SAS_Data.all[theme][gameIndex].ActiveImages[i].BaseCoords.height / SAS_Data.all[theme][gameIndex].Background.BaseRect.height;

                gameLayout.Children.Add(view,
                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Width * x;    // установка координаты X
                }),
                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Height * y;   // установка координаты Y
                }),

                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Width * width;
                }), // установка ширины
                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Height * height;
                })
                );

                // add bound views
                for (int j = 0; j < SAS_Data.all[theme][gameIndex].ActiveImages[i].Bounds.Count; j++) {
                    BoundView boundView = new BoundView {BackgroundColor = Color.Transparent};
                    boundView.id = i;
                    boundViews.Add(boundView);

                    double x_ = SAS_Data.all[theme][gameIndex].ActiveImages[i].Bounds[j].x / (SAS_Data.all[theme][gameIndex].Background.BaseRect.width - 0);
                    double y_ = SAS_Data.all[theme][gameIndex].ActiveImages[i].Bounds[j].y / (SAS_Data.all[theme][gameIndex].Background.BaseRect.height - 0);

                    double width_ = SAS_Data.all[theme][gameIndex].ActiveImages[i].Bounds[j].width / SAS_Data.all[theme][gameIndex].Background.BaseRect.width;
                    double height_ = SAS_Data.all[theme][gameIndex].ActiveImages[i].Bounds[j].height / SAS_Data.all[theme][gameIndex].Background.BaseRect.height;

                    //AbsoluteLayout.SetLayoutBounds(boundView, new Rectangle(x, y, width, height));
                    //AbsoluteLayout.SetLayoutFlags(boundView, AbsoluteLayoutFlags.All);
                    //gameLayout.Children.Add(boundView);

                    gameLayout.Children.Add(boundView,
                    Constraint.RelativeToParent((parent) =>
                    {
                    return parent.Width * x_;    // установка координаты X
                     }),
                    Constraint.RelativeToParent((parent) =>
                    {
                    return parent.Height * y_;   // установка координаты Y
                    }),

                    Constraint.RelativeToParent((parent) =>
                    {
                    return parent.Width * width_;
                    }), // установка ширины
                    Constraint.RelativeToParent((parent) =>
                    {
                    return parent.Height * height_;
                    })
                    );

                    //var tapGestureRecognizer = new TapGestureRecognizer();
                    boundView.GestureRecognizers.Add(tapGestureRecognizer);
                }
            }


            // finally add all left static images
            totalObjects = unTappedIndexes.Count;

            AddAllStaticImagesToLowerLayout(staticImageIndex, 1000);
        }

        public void AddLabels () {
            labels = new List<Label>();
            if (SAS_Data.all[theme][gameIndex].Labels == null) return;

            for (int i = 0; i < SAS_Data.all[theme][gameIndex].Labels.Count; i++)
            {

                Label label = new Label
                {
                    InputTransparent = true,
                    IsEnabled = false,
                    BackgroundColor = Color.Transparent,
                    TextColor = SAS_Data.all[theme][gameIndex].Labels[i].color,
                    HorizontalTextAlignment = TextAlignment.Center,
                    FontSize = App.Current.MainPage.Height * 0.025,//Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                    FontAttributes = FontAttributes.Bold,
                    Text = SAS_Data.all[theme][gameIndex].Labels[i].KeyName + "," + Theme.Resources[(int) theme].Item[SAS_Data.all[theme][gameIndex].Labels[i].KeyName].id};

                double x = SAS_Data.all[theme][gameIndex].Labels[i].BaseCoords.x / (SAS_Data.all[theme][gameIndex].Background.BaseRect.width - 0);
                double y = SAS_Data.all[theme][gameIndex].Labels[i].BaseCoords.y / (SAS_Data.all[theme][gameIndex].Background.BaseRect.height - 0);

                double width = SAS_Data.all[theme][gameIndex].Labels[i].BaseCoords.width / SAS_Data.all[theme][gameIndex].Background.BaseRect.width;
                double height = SAS_Data.all[theme][gameIndex].Labels[i].BaseCoords.height / SAS_Data.all[theme][gameIndex].Background.BaseRect.height;

                labels.Add(label);
                gameLayout.Children.Add(label,
                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Width * x;    // установка координаты X
                }),
                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Height * y;   // установка координаты Y
                }),

                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Width * width;
                }), // установка ширины
                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Height * height;
                })
                );
                
            }
        }

        int AddAllStaticImagesToLowerLayout (int staticImageIndex, int i) {
            if (SAS_Data.all[theme][gameIndex].StaticImages == null) return 0;
            Debug.WriteLine("staticImageIndex="+staticImageIndex + "/SAS_Data.all[theme][gameIndex].StaticImages.Count=" + SAS_Data.all[theme][gameIndex].StaticImages.Count);

            int curImgIndex = staticImageIndex;
            while (curImgIndex < SAS_Data.all[theme][gameIndex].StaticImages.Count)  {
                
                Debug.WriteLine("curImgIndex=" + curImgIndex);
                if (SAS_Data.all[theme][gameIndex].StaticImages[curImgIndex].LayoutIndex >= i) break;
                //here we add a static image
                InteractiveView staticView = new InteractiveView(null, string.Empty) {
                    IsEnabled = false, InputTransparent = true};
                staticView.ImageSource = Forms9Patch.ImageSource.FromResource(SAS_Data.all[theme][gameIndex].GetImageSourceStaticImages(curImgIndex));
                staticView.id = -1;

                double x = SAS_Data.all[theme][gameIndex].StaticImages[curImgIndex].BaseCoords.x / (SAS_Data.all[theme][gameIndex].Background.BaseRect.width - 0);
                double y = SAS_Data.all[theme][gameIndex].StaticImages[curImgIndex].BaseCoords.y / (SAS_Data.all[theme][gameIndex].Background.BaseRect.height - 0);

                double width = SAS_Data.all[theme][gameIndex].StaticImages[curImgIndex].BaseCoords.width / SAS_Data.all[theme][gameIndex].Background.BaseRect.width;
                double height = SAS_Data.all[theme][gameIndex].StaticImages[curImgIndex].BaseCoords.height / SAS_Data.all[theme][gameIndex].Background.BaseRect.height;

                //AbsoluteLayout.SetLayoutBounds(staticView, new Rectangle(x, y, width, height));
                //AbsoluteLayout.SetLayoutFlags(staticView, AbsoluteLayoutFlags.All);
                //gameLayout.Children.Add(staticView);


                gameLayout.Children.Add(staticView,
                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Width * x;    // установка координаты X
                }),
                Constraint.RelativeToParent((parent) =>
                {

                    return parent.Height * y;   // установка координаты Y
                }),

                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Width * width;
                }), // установка ширины
                Constraint.RelativeToParent((parent) =>
                {

                    return parent.Height * height;
                })
                );
                
                curImgIndex++;
            }

            return curImgIndex;
        }

        public void AddStaticImages()
        {

            if (SAS_Data.all[theme][gameIndex].StaticImages == null) return;

            for (int i = 0; i < SAS_Data.all[theme][gameIndex].StaticImages.Count; i++)
            {
                InteractiveView staticView = new InteractiveView(null, string.Empty) {IsEnabled = false};
                staticView.ImageSource = Forms9Patch.ImageSource.FromResource(SAS_Data.all[theme][gameIndex].GetImageSourceStaticImages(i));
                staticView.id = -1;

                double x = SAS_Data.all[theme][gameIndex].StaticImages[i].BaseCoords.x / (SAS_Data.all[theme][gameIndex].Background.BaseRect.width - 0); 
                double y = SAS_Data.all[theme][gameIndex].StaticImages[i].BaseCoords.y / (SAS_Data.all[theme][gameIndex].Background.BaseRect.height - 0);

                double width = SAS_Data.all[theme][gameIndex].StaticImages[i].BaseCoords.width / SAS_Data.all[theme][gameIndex].Background.BaseRect.width;
                double height = SAS_Data.all[theme][gameIndex].StaticImages[i].BaseCoords.height / SAS_Data.all[theme][gameIndex].Background.BaseRect.height;

                gameLayout.Children.Add(staticView,
                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Width * x;    // установка координаты X
                }),
                Constraint.RelativeToParent((parent) =>
                {
                    
                    return parent.Height * y;   // установка координаты Y
                }),

                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Width * width;
                }), // установка ширины
                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Height * height;
                })
                );
                
            }
        }

        public async void AnimateObject (int i) {
            if (isAnimating) return;
            isAnimating = true;

            App.Audio.SayWord(interactiveImages[i].Key, language);
            //check end of game 
            if (unTappedIndexes.Contains(i))
            {
                activityReport.ReportProgress((totalObjects - unTappedIndexes.Count + 1) * progressRatio / totalObjects * 100 + 50 * gameIndex);
            }

            await interactiveImages[i].Animate(SAS_Data.all[theme][gameIndex].ActiveImages[i].anim);
           


            if (unTappedIndexes.Count > 0) unTappedIndexes.Remove(i);

            

            if (unTappedIndexes.Count == 0) EndOfGameVictory();
            else
            {
                isAnimating = false;
            }
        }

        async void EndOfGameVictory() {
            isAnimating = true;

            // check if it is animal game
            if (gameIndex == 0 && theme == THEME_NAME.ANIMALS) {
                // start second game
                Debug.WriteLine("starting second part of animals sas ...");
                //navigation.PopModalAsync();
                await navigation.PushModalAsync(new SASGamePage(navigation, theme, 1, starsInfo));
                //navigation.RemovePage(current);
            }
            else {
                victoryView.EndOfGameVictory(starsInfo, post, activityReport);
            }
        }


        public void AddGameLayout () {
            Debug.WriteLine("Padding.Top = " + totalLayout.Padding.Top + "padding.Bottom = " + totalLayout.Padding.Bottom);

            double backgroundWidth = SAS_Data.all[theme][gameIndex].Background.BaseRect.width;
            double backgroundHeight = SAS_Data.all[theme][gameIndex].Background.BaseRect.height;

            //check  aspects 
            double aspectedWidth = 0;
            double aspectedHeight = 0;
            if (backgroundWidth / backgroundHeight < (UI_Sizes.ScreenWidthX - 20) / (UI_Sizes.ScreenHeightX -20))
            {
                // big width, height is priority
                aspectedHeight = UI_Sizes.ScreenHeightX - 20;
                aspectedWidth = aspectedHeight * backgroundWidth / backgroundHeight;
            }
            else
            {
                // big height, width is priority
                aspectedWidth = UI_Sizes.ScreenWidthX - 20;
                aspectedHeight = aspectedWidth * backgroundHeight / backgroundWidth;
            }

            totalLayout.Children.Add(gameLayout,
                Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Width - aspectedWidth) * 0.5;    // установка координаты X
                }),
                Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Height - aspectedHeight) * 0.5;   // установка координаты Y
                }),

                Constraint.RelativeToParent((parent) =>
                {
                    return aspectedWidth;
                }), // установка ширины
                Constraint.RelativeToParent((parent) =>
                {
                    
                    // set victory view
                    victoryView.SetSizesAndStartY(parent.Height, 0.621, 0.05, -App.Current.MainPage.Height);

                    return aspectedHeight;
                })
            );
            
        }

        async void OnClickedClose(BackButtonView view)
        {
            if (isAnimating) return;
            isAnimating = true;
            await view.ScaleTo(1.2, 125);
            await view.ScaleTo(1.0, 125);
            OnClose();
            await navigation.PopModalAsync();
            isAnimating = false;
        }

        public async void MenuButton_Tapped(object sender, System.EventArgs e)
        {
            if (isAnimating) return;
            isAnimating = true;

            View view = sender as View;
            Debug.WriteLine("view.ClassId = " + view.ClassId);
            if (view.ClassId == "CloseBtn")
            {
                await AnimateImage(view, 250);
                OnClose();                
                await navigation.PopModalAsync();
            }
            isAnimating = false;
        }

        public Task AnimateImage(View view, uint time)
        {            
            return Task.Run(async () =>
            {
                await view.ScaleTo(0.8, time / 2);
                await view.ScaleTo(1.0, time / 2);                
                return;
            });
        }

        public void Dispose()
        {
            App.OnSleepEvent -= OnSleep;
            App.OnResumeEvent -= OnResume;

            DownloadHelper.DownloadHelper.OnLoadingStarted -= OnLoadingStarted;
            DownloadHelper.DownloadHelper.OnLoadingProgress -= OnLoadingProgress;
            DownloadHelper.DownloadHelper.OnLoadingEnded -= OnLoadingEnded;

            PropertyChanged = null;
            BackgroundImageSource = null;
            navigation = null;
            GifSource = null;

            backButtonView = null;
            gameLayout = null;
            absRoot = null;
            totalLayout = null;
            loadingView = null;           

            foreach (InteractiveView view in interactiveImages)
            {
                view.Dispose();
            }
            interactiveImages = null;

            labels = null; // ??? is it enough ??            
            boundViews = null;

            victoryView.Dispose();
            victoryView = null;

            tapGestureRecognizer = null;
            
            starsInfo = null;
            post = null;
            activityReport = null;
            
        }

    }

    
}
