using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using FFImageLoading.Forms;
using Newtonsoft.Json;
using Plugin.Connectivity;
using Xamarin.Forms;
using HtmlAgilityPack;

namespace DinoLingo
{
    public class DinoPage_ViewModel: INotifyPropertyChanged
    {
        static int ERROR_PREFIX = 60;

        public ImageSource MainImage { get; set; }
        public string MainText { get; set; }
        public string TitleText { get; set; }
        public double TitleTextSize { get; set; }
        public double MainTextSize { get; set; }

        BackButtonView backButtonView;
        INavigation navigation;
        string post_id;
        PostResponse.Post post;
        bool isLoading = false;

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

        public DinoPage_ViewModel(INavigation navigation, string post_id)
        {
            this.navigation = navigation;
            this.post_id = post_id;
            TitleTextSize = UI_Sizes.MediumTextSize * 1.0;
            MainTextSize = UI_Sizes.MediumTextSize * 0.9;
        }

        public void Start() {
            Device.StartTimer(TimeSpan.FromMilliseconds(10), FillTheViewOnStartAsync);
        }

        bool FillTheViewOnStartAsync()
        {
            Debug.WriteLine("FillTheViewOnStartAsync() ... ");
            TryToShowContent();
            return false;
        }

        async void TryToShowContent()
        {
            Debug.WriteLine("TryToShowContent...");
            // check if we have post with current id
            if (await CacheHelper.Exists(CacheHelper.POST + post_id))
            {
                // if we have --> show post
                Debug.WriteLine("we have content data...");
                post = await CacheHelper.GetAsync<PostResponse.Post>(CacheHelper.POST + post_id);
                ShowPost();
            }
            else
            {
                // if we don't have --> download post
                Debug.WriteLine("we do not have post data...");
                TryToDownloadPost();
            }
        }

        Task TryToDownloadPost()
        {
            return Task.Run(async () => {
                isLoading = true;
                // check connection here
                if (!CrossConnectivity.Current.IsConnected || !DownloadHelper.DownloadHelper.CheckInternetConnectionProgressive())
                { // check connectivity
                    if (AsyncMessages.CheckConnectionTimeout())
                    {
                        AsyncMessages.DisplayAlert(POP_UP.OOPS, POP_UP.NO_CONNECTION, POP_UP.OK);
                    }                    
                    //("Error!", "No internet connection!", "ОK");
                    isLoading = false;
                    return;
                }

                // get PostResponse
                var postData = $"id={post_id}";
                postData += $"&user_id={UserHelper.Login.user_id}";
                PostResponse postResponse = await ServerApi.PostRequestProgressive<PostResponse>(ServerApi.POST_URL, postData, null);

                // process response
                if (postResponse == null)
                {
                    Analytics.SendResultsRegular("DinoPage_ViewModel", postResponse, postResponse?.error, ServerApi.POST_URL, postData);
                    if (AsyncMessages.CheckDisplayAlertTimeout())
                    {
                        AsyncMessages.DisplayAlert(POP_UP.OOPS,
                        POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(postResponse?.error, ERROR_PREFIX + 1), POP_UP.OK);
                        //"Error!", "Error in response from server!", "ОK");
                    }

                    isLoading = false;
                    return;
                }
                if (postResponse.error != null)
                {
                    Analytics.SendResultsRegular("DinoPage_ViewModel", postResponse, postResponse?.error, ServerApi.POST_URL, postData);
                    if (AsyncMessages.CheckDisplayAlertTimeout())
                    {
                        AsyncMessages.DisplayAlert(POP_UP.OOPS,
                        POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(postResponse?.error, ERROR_PREFIX + 2), POP_UP.OK);
                        //("Error!", "Message: " + postResponse.error.message, "ОK");
                    }

                    isLoading = false;
                    return;
                }
                if (postResponse.result == null)
                {
                    Analytics.SendResultsRegular("DinoPage_ViewModel", postResponse, postResponse?.error, ServerApi.POST_URL, postData);
                    if (AsyncMessages.CheckDisplayAlertTimeout())
                    {
                        AsyncMessages.DisplayAlert(POP_UP.OOPS,
                       POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(postResponse?.error, ERROR_PREFIX + 3), POP_UP.OK);
                        //("Error!", "Result from server is null!", "ОK");
                    }

                    isLoading = false;
                    return;
                }

                // add to cache
                Debug.WriteLine("postResponse downloaded ok");
                await CacheHelper.Add<PostResponse.Post>(CacheHelper.POST + post_id, postResponse.result);
                post = postResponse.result;

                Device.BeginInvokeOnMainThread(ShowPost);
                isLoading = false;
                return;
            });
        }

        void ShowPost () {
            if (post == null) return;
            GetImageUrlAndTextFromPostContent();

            TitleText = post.title;
        }

        void GetImageUrlAndTextFromPostContent() {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(post.content);

            HtmlNode imgNode = htmlDoc.DocumentNode.SelectNodes("//img")[0];
            MainImage = imgNode.GetAttributeValue("src", "").Replace ("../..", "https://dinolingo.com");

            HtmlNodeCollection mainTextNodes = htmlDoc.DocumentNode.SelectNodes("//p");
            Debug.WriteLine("post.content = " + post.content);
            if (mainTextNodes != null && mainTextNodes.Count == 2) {
                
                MainText = mainTextNodes[1].InnerText;
            }
            else {
                Debug.WriteLine("GetImageUrlAndTextFromPostContent -> 1");
                int index = post.content.ToLower().IndexOf("p>");
                Debug.WriteLine("GetImageUrlAndTextFromPostContent -> 2");
                if (index >= 0)
                {
                    Debug.WriteLine("GetImageUrlAndTextFromPostContent -> 3");
                    MainText = post.content.Substring(index + 2).Replace("\n", string.Empty).Replace("\r", string.Empty);
                }
                else
                {
                    Debug.WriteLine("GetImageUrlAndTextFromPostContent -> 4");
                    if (!string.IsNullOrEmpty(post.title)) index = post.content.ToLower().IndexOf(post.title.ToLower());
                    if (index >= 0) {
                        Debug.WriteLine("GetImageUrlAndTextFromPostContent -> 5");
                        MainText = post.content.Substring(index + post.title.Length).Replace("\n", string.Empty).Replace("\r", string.Empty);
                    }
                    else {
                        Debug.WriteLine("GetImageUrlAndTextFromPostContent -> 6");
                        MainText = post.title;
                    }
                }
            }

            /*
            int index = post.content.ToLower().IndexOf("p>");
            if (index >= 0)
            {
                MainText = post.content.Substring(index + 2).Replace("\n", string.Empty).Replace("\r", string.Empty);
            }
            else {
                MainText = post.content;
            }
            */
        }



        public void AddCloseButton(RelativeLayout totalLayout)
        {
            backButtonView = new BackButtonView();
            backButtonView.AddToTopRight(totalLayout, 0.1, 10, OnClickedClose);
        }

        async void OnClickedClose(View view)
        {
            if (IsAnimating) return;
            IsAnimating = true;
            await AnimateImage(view, 250);
            await navigation.PopModalAsync();
            IsAnimating = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

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
            MainImage = null;
            if (backButtonView != null) backButtonView.Dispose();
            backButtonView = null;
            
            navigation = null;            
            post = null;

            animLock = null;
            PropertyChanged = null;
        }
    }
}
