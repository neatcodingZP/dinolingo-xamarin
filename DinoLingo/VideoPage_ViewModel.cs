using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using FormsVideoLibrary;
using System.ComponentModel;
using Plugin.Connectivity;
using Newtonsoft.Json;
using Plugin.GoogleAnalytics;

namespace DinoLingo
{
    public class VideoPage_ViewModel: INotifyPropertyChanged
    {
        static int ERROR_PREFIX = 160;


        public event PropertyChangedEventHandler PropertyChanged;
        public string LoadingWaitText { get; set; }

        public VideoSource VideoSource { get; set; }


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

        INavigation navigation;
        //int index;
        View loadingView, backButtonView;
        VictoryView victoryView;
        RelativeLayout rootRelative;
        //VideoPlayer player;
        bool firstStart = true;
        string uri = String.Empty; // = "https://storage.googleapis.com/dino-lingo-videos/SHORTS/ENGLISH/DVD%201/ENG-ANIMALS1.mp4";
        //string filename = "lesson_animals_eng_1.mp4";
        VideoStatus prevStatus;
        string videoId;
        ListItemStarsInfo starsInfo;
        PostResponse.Post post;

        ActivityReport activityReport;
        double maxPosition = 0;

        public VideoPage_ViewModel(INavigation navigation, string id, View loadingView, RelativeLayout root, ListItemStarsInfo starsInfo)
        {
            this.navigation = navigation;
            this.loadingView = loadingView;
            rootRelative = root;
            videoId = id;
            this.starsInfo = starsInfo;

            App.OnSleepEvent += OnSleep;
            App.OnResumeEvent += OnResume;

            activityReport = new ActivityReport(ActivityReport.ACT_TYPE.VIDEO, id);
            activityReport.OnCompleted += ActivityReport_OnCompleted;

            Debug.WriteLine("VideoPage_ViewModel -> ");
        }

        void ActivityReport_OnCompleted()
        {
            Debug.WriteLine("VideoPage_ViewModel -> ActivityReport_OnCompleted(), maxPosition = " + maxPosition);

            //IsAnimating = true;
            //victoryView.EndOfGameVictory(starsInfo, post, activityReport);
        }

        public void AddVictoryView(RelativeLayout relLayout)
        {
            Debug.WriteLine("VideoPage_ViewModel -> AddVictoryView");
           
            victoryView = new VictoryView(navigation, OnClose, starsInfo);
            relLayout.Children.Add(victoryView,
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

        void OnClose () {
            
        }

        public void AddCloseButton(RelativeLayout totalLayout)
        {
            Debug.WriteLine("VideoPage_ViewModel -> AddCloseButton()");

            backButtonView = new BackButtonView();
            double padding = 10;
            double btn_size = 0.1;

            totalLayout.Children.Add(backButtonView,
                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Width - parent.Height * btn_size - padding;    // установка координаты X
                }),
                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Height * 0.1 + padding;   // установка координаты Y
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
                BackButtonView view = (BackButtonView)sender;
                if (view != null) OnClickedClose(view);
                // now you have a reference to the image
                Debug.WriteLine("tapped back");
            };
            backButtonView.GestureRecognizers.Add(tapGestureRecognizer);
        }

        async void OnClickedClose(View view) {
            if (IsAnimating) return;
            IsAnimating = true;
            await AnimateImage(view, 250);
            await navigation.PopModalAsync();

            IsAnimating = false;
        }


        bool StartLoading()
        {
            Debug.WriteLine("VideoPage_ViewModel -> StartLoading ()");

            AddVictoryView(rootRelative);
            
            // set victory view
            Debug.WriteLine("VideoPage_ViewModel -> StartLoading () -> set victory size");
            victoryView.SetSizesAndStartY(App.Current.MainPage.Height - 20, 0.621, 0.05, -App.Current.MainPage.Height);

            AnimateLoadingView_InBckground(1000);
            return false;
        }

        void OnLoadingAppeared()
        {
            LoadingWaitText = "Please wait...";
            //LoadingPercentText = "Checking video";
            //ProgressValue = 0;
        }

        void OnLoadingStarted()
        {
            LoadingWaitText = "Please wait... cashing video...";
            //LoadingPercentText = "0 %";
        }

        void OnLoadingProgress()
        {
            Debug.WriteLine("OnLoadingProgress() --> progress = " + DownloadHelper.DownloadHelper.progress);
            //ProgressValue = DownloadHelper.DownloadHelper.progress;
            //LoadingPercentText = ((int)(ProgressValue * 100)).ToString() + " %";
        }   



        async void AnimateLoadingView_InBckground(uint time)
        {
            IsAnimating = true;
            try
            {
                await loadingView.TranslateTo(0, -App.Current.MainPage.Height, time);
                loadingView.IsVisible = false;
                loadingView.InputTransparent = true;
                IsAnimating = false;

                activityReport.Start();
            }
            catch
            {
                return;
            }

            // check if it is favorite post ...
            Favorites.WaitIfListsAreBuisy("BookPage_ViewModel");
            bool mayBeFavorite = Favorites.IsFavorite(videoId);
            Favorites.ListsAreBuisy = false;
            Debug.WriteLine("VideoPage_ViewModel -> AnimateLoadingView_InBckground -> ListsAreBuisy = false");
            try {
                int stars = 0;
                if (starsInfo.favorites_Page != null)
                {
                    FavoriteListCoords favoriteListCoords = starsInfo.favoriteListCoords;

                    stars = Favorites.allListsData[(int)favoriteListCoords.CentralView][favoriteListCoords.SubView][favoriteListCoords.index].Stars;
                }
                else
                {
                    stars = starsInfo.data[starsInfo.isSingleItem ? starsInfo.eventArgs.MyItem.Index : (starsInfo.eventArgs.MyItem as DoubleItem).Index2].Stars;
                }
                Debug.WriteLine("VideoPage_ViewModel -> AnimateLoadingView_InBckground -> stars = " + stars);

                var isreacheable = DownloadHelper.DownloadHelper.CheckInternetConnectionProgressive();

                if (mayBeFavorite && await PCLHelper.IsFileExistAsync(videoId + ".mp4"))
                { // ! it is favorite !
                    Debug.WriteLine("it's in favorites, try play it");
                    if (stars < 5) post = await victoryView.DownloadPost(starsInfo);
                    PlayFromPCL();
                }
                else if ((CrossConnectivity.Current.IsConnected && isreacheable && (post = await victoryView.DownloadPost(starsInfo)) != null)) // GET URI HERE
                {

                    Debug.WriteLine("Try to get uri...");
                    if (!string.IsNullOrEmpty(post.content))
                    {
                        uri = post.content;
                        Debug.WriteLine("VideoPage_ViewModel -> uri = " + uri);
                        int index1 = uri.IndexOf("\"]");
                        if (index1 != -1)
                        {
                            uri = uri.Remove(index1);
                        }

                        uri = uri.Replace("[dlol_player url=\"", string.Empty).Replace(" ", "%20").Replace($"'", "%27");

                        Debug.WriteLine("Try to play uri = " + uri);

                        if (Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                        {
                            if (!string.IsNullOrEmpty(uri))
                            {
                                //***
                                //uri = "https://www.dropbox.com/s/9nd99xzq7z4jkaw/ALB-INTHEHOUSE2.mp4";
                                //uri = "https://storage.googleapis.com/dino-lingo-videos/SHORTS/ENGLISH/DVD%201/ENG-ANIMALS1.mp4";
                                
                                VideoSource = new UriVideoSource
                                {
                                    Uri = uri
                                };
                                /*
                                VideoSource = new ResourceVideoSource
                                {
                                    Path = "ALB_INTHEHOUSE2.mp4",
                                };
                                */
                            }
                                
                        }
                        else
                        {
                            Debug.WriteLine("VideoPage_ViewModel -> Uri.IsWellFormedUriString(uri, UriKind.Absolute) == false, uri =" + uri);
                            if (AsyncMessages.CheckDisplayAlertTimeout())
                            {
                                await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                                POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(null, ERROR_PREFIX + 1), POP_UP.OK);
                            }
                            

                            GoogleAnalytics.Current.Tracker.SendEvent("VideoPage_ViewModel -> AnimateLoadingView_InBckground", "try to play video", "error: post.content= " + post.content + ", uri= " + uri, 1);

                            await navigation.PopModalAsync();
                        }

                    }
                    else
                    {
                        Debug.WriteLine("VideoPage_ViewModel -> post = await victoryView.DownloadPost(starsInfo)).content == null");
                        if (AsyncMessages.CheckDisplayAlertTimeout())
                        {
                            await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                            POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(null, ERROR_PREFIX + 2), POP_UP.OK);
                        }
                            
                        GoogleAnalytics.Current.Tracker.SendEvent("VideoPage_ViewModel -> AnimateLoadingView_InBckground", "try to play video", "error: post.content= " + post.content + ", uri= " + uri, 2);
                        await navigation.PopModalAsync();
                    }
                }
                else if (!CrossConnectivity.Current.IsConnected || !isreacheable)
                {
                    Debug.WriteLine("VideoPage_ViewModel -> NO CONNECTION");
                    if (AsyncMessages.CheckConnectionTimeout())
                    {
                        await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                                        POP_UP.NO_CONNECTION, POP_UP.OK);
                    }
                    
                    //("Error!", "No connection. Please, check Your internet.", "Cancel");
                    await navigation.PopModalAsync();
                }
                else
                {
                    Debug.WriteLine("VideoPage_ViewModel -> post = await victoryView.DownloadPost(starsInfo)) == null");
                    if (AsyncMessages.CheckConnectionTimeout())
                    {
                        await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                                        POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(null, ERROR_PREFIX + 3), POP_UP.OK);
                        //("Error!", "Some error in server response.", "Cancel");
                    }

                    await navigation.PopModalAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("VideoPage_ViewModel -> some major Exception");
                if (loadingView == null) return;
                /*
                await App.Current.MainPage.DisplayAlert(POP_UP.OOPS,
                    POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(null, ERROR_PREFIX + 4), POP_UP.OK);
                    GoogleAnalytics.Current.Tracker.SendEvent("VideoPage_ViewModel -> AnimateLoadingView_InBckground", "try to play video", "EX: " +ex.Message + ", post.content= " + post?.content + ", uri= " + uri, 2);
                    */
                //await navigation.PopModalAsync();
            }
            
        }





        void PlayFromPCL()
        {
            Debug.WriteLine("PlayFromPCL -> Try to play...");
            var filePath = PCLStorage.FileSystem.Current.LocalStorage.Path + "/" + videoId + ".mp4";
            var pathToFileURL = new System.Uri(filePath).AbsolutePath;
            //await CrossMediaManager.Current.Play("file://" + pathToFileURL);
            VideoSource = new FileVideoSource
            {
                File = "file://" + pathToFileURL
            };           
        }

        public async void OnAppearing()
        {
            if (firstStart)
            {
                firstStart = false;
                OnLoadingAppeared();
                Device.StartTimer(TimeSpan.FromMilliseconds(500), StartLoading);
            }
        }

        public void OnDisappearing()
        {
            Debug.WriteLine("VideoPage_ViewModel -> Disappearing()");
            App.OnSleepEvent -= OnSleep;
            App.OnResumeEvent -= OnResume;

            if (App.Audio.playerVictory != null && App.Audio.playerVictory.IsPlaying) App.Audio.playerVictory.Stop();

            activityReport.OnDisappearing();
        }


        void OnSleep()
        {
            Debug.WriteLine("VideoPage_ViewModel -> OnSleep()");
            activityReport.OnSleep();
        }

        void OnResume()
        {
            Debug.WriteLine("VideoPage_ViewModel -> OnResume()");
            activityReport.OnResume();
        }

        public void Handle_UpdateStatus(VideoPlayer videoPlayer, object sender, System.EventArgs e)
        {
            double newPosition = videoPlayer.Position.TotalSeconds / videoPlayer.Duration.TotalSeconds;
            // check new position
            double deltaPos = newPosition - maxPosition;
            if (deltaPos > 0)
            {
                if (newPosition < 0.9)
                { // step 0.1
                    if (deltaPos > 0.1)
                    {
                        maxPosition = newPosition;
                        activityReport.ReportProgress(maxPosition * 100);
                        Debug.WriteLine("VideoPage_ViewModel -> Handle_UpdateStatus - newPosition = " + newPosition);
                    }
                }
                else if (newPosition < 0.95)
                {
                    if (deltaPos > 0.05)
                    {
                        maxPosition = newPosition;
                        activityReport.ReportProgress(maxPosition * 100);
                        Debug.WriteLine("VideoPage_ViewModel -> Handle_UpdateStatus - newPosition = " + newPosition);
                    }
                }
                else 
                {
                    if (deltaPos > 0.02)
                    {
                        maxPosition = newPosition;
                        activityReport.ReportProgress(maxPosition * 100);
                        Debug.WriteLine("VideoPage_ViewModel -> Handle_UpdateStatus - newPosition = " + newPosition);
                    }
                }

            }
            else return; // it's not a new position

            if (videoPlayer.Status == prevStatus) return;
            Debug.WriteLine("videoPlayer.Status =" + videoPlayer.Status);
            prevStatus = videoPlayer.Status;


            // anyway, when the video ends -> 
            if (videoPlayer.Status == VideoStatus.Paused) {
                Debug.WriteLine("videoPlayer.Duration.TotalSeconds  = " + (videoPlayer.Duration.TotalSeconds));
                Debug.WriteLine("videoPlayer.Position.TotalSeconds = " + (videoPlayer.Position.TotalSeconds));

                if ((int) videoPlayer.Duration.TotalSeconds == (int) videoPlayer.Position.TotalSeconds) {
                    Debug.WriteLine("VIDEO OVER !!!");
                    IsAnimating = true;
                    victoryView.EndOfGameVictory(starsInfo, post, activityReport);
                }
            }
        }

        public Task AnimateImage(View view, uint time)
        {
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

        public void Dispose()
        {
            PropertyChanged = null;
            VideoSource = null;
            animLock = null;
            navigation = null;

            loadingView = null;
            backButtonView = null;

            victoryView.Dispose();
            victoryView = null;

            rootRelative = null;
            
            starsInfo = null;
            post = null;
            activityReport = null;
        }
    }
}
