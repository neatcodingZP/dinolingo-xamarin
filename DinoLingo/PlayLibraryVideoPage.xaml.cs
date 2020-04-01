using System;
using Xamarin.Forms;
using FormsVideoLibrary;
using System.Diagnostics;
using System.Net;
using System.IO;
using System.Threading.Tasks;

namespace DinoLingo
{
    public partial class PlayLibraryVideoPage : ContentPage
    {
        public PlayLibraryVideoPage()
        {
            InitializeComponent();


            DownloadAndPlayAsync();
        }

        async Task Download () {
            //PCL STORAGE
            Debug.WriteLine("is folder exists? (VIDEO) = " + await PCLHelper.IsFolderExistAsync("VIDEO"));

            // download video

            HttpWebRequest webReq;
            Uri url;
            url = new Uri("http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4");
            byte[] bytes = default(byte[]); //null;
            var memstream = new MemoryStream();

            webReq = (HttpWebRequest)HttpWebRequest.Create(url);
            try
            {
                Debug.WriteLine("Try my HttpWebRequest...");
                webReq.CookieContainer = new CookieContainer();
                webReq.Method = "GET";

                using (WebResponse response = webReq.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream);
                        await stream.CopyToAsync(memstream);
                        bytes = memstream.ToArray();
                        Debug.WriteLine("Bytes downloaded. L= " + bytes.Length);

                        Debug.WriteLine("Try to save video to file..." + "big_buck_bunny.mp4");
                        await PCLHelper.SaveImage(bytes, "big_buck_bunny.mp4");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("HttpWebRequest EXCEPTION :(");
            }
            Debug.WriteLine("Download ended.");
            Debug.WriteLine("does file exist? (big_buck_bunny.mp4) = " + await PCLHelper.IsFileExistAsync("big_buck_bunny.mp4"));
        }
        async void DownloadAndPlayAsync() {
           await Download();
            Play();
        }

        void Play () {
            Debug.WriteLine("Try to play...");
            var filePath = PCLStorage.FileSystem.Current.LocalStorage.Path + "/" + "big_buck_bunny.mp4";
            var pathToFileURL = new System.Uri(filePath).AbsolutePath;

            Debug.WriteLine("File = " + "file://" + pathToFileURL);
            //await CrossMediaManager.Current.Play("file://" + pathToFileURL);
            videoPlayer.Source = new FileVideoSource
            {
                File = "file://" + pathToFileURL
            };
            videoPlayer.Play();
        }

        async void OnShowVideoLibraryClicked(object sender, EventArgs args)
        {
            Button btn = (Button)sender;
            btn.IsEnabled = false;

            string filename = await DependencyService.Get<IVideoPicker>().GetVideoFileAsync();

            if (!String.IsNullOrWhiteSpace(filename))
            {
                videoPlayer.Source = new FileVideoSource
                {
                    File = filename
                };
            }

            btn.IsEnabled = true;
        }
    }
}