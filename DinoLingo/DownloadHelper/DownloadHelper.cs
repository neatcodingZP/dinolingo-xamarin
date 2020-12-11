using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using DinoLingo;
using Newtonsoft.Json;
using Plugin.Connectivity;
using Xamarin.Forms;
using HtmlAgilityPack;
using System.Linq;
using Plugin.GoogleAnalytics;
using System.Net.Cache;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DownloadHelper
{
    public static class DownloadHelper
    {
        public static Action OnLoadingStarted, OnLoadingEnded, OnLoadingProgress;
        public static double progress = 0;
        public static int totalFilesDownloaded = 0;
        public enum RESULT { NONE, NOTHING_TO_DOWNLOAD, DOWNLOAD_STARTED, DOWNLOADED_OK, ERROR, ERROR_NO_CONNECTION };
        public static RESULT result = RESULT.NONE;

        public static bool BreakDownloading = false;

        public static Action OnLoadingStarted_InBckground, OnLoadingEnded_InBckground, OnLoadingProgress_InBckground;
        public static double progress_InBckground = 0;
        public static int totalFilesDownloaded_InBckground = 0;
        public static RESULT result_InBckground = RESULT.NONE;
        public static bool isDownloading_InBackground = false;

        

        public static bool CheckInternetConnection(int timeout)
        {
            string CheckUrl = "https://www.google.com/";
            Debug.WriteLine("DownloadHelper -> CheckInternetConnection ...");
            DateTime startTime = DateTime.Now;
            HttpWebRequest iNetRequest = null;
            WebResponse iNetResponse = null;
            bool result;
            try
            { 
                iNetRequest = (HttpWebRequest) WebRequest.Create(CheckUrl);
                iNetRequest.Timeout = timeout;
                iNetRequest.ReadWriteTimeout = timeout;
                iNetRequest.KeepAlive = false;
                iNetRequest.AllowWriteStreamBuffering = false;
                iNetRequest.AllowReadStreamBuffering = false;

                using (iNetResponse = (HttpWebResponse) iNetRequest.GetResponse())
                {
                    
                }

                TimeSpan deltaTime = DateTime.Now - startTime;
               
                Debug.WriteLine($"DownloadHelper -> CheckInternetConnection, googleOK = {true}, deltaTime = {deltaTime.TotalMilliseconds} ms, timeout = {timeout}");

                
                result = true;
            }
            catch (System.AggregateException ex)
            {
                Debug.WriteLine($"DownloadHelper -> CheckInternetConnection, System.AggregateException= {ex.Message}");

                result = false;
            }
            catch (WebException ex)
            {
                
                TimeSpan deltaTime = DateTime.Now - startTime;
                Debug.WriteLine($"DownloadHelper -> CheckInternetConnection, googleOK = {false}, deltaTime = {deltaTime.TotalMilliseconds} ms, timeout = {timeout}");
                
                result = false;                
            }
            finally
            {                
                iNetRequest?.Abort();
                iNetResponse?.Close();                
            }

            return result;
        }

        /*
        public static async Task<bool> CheckInternetConnection2(int timeout)
        {
            bool connectionOK = false;
            Debug.WriteLine("DownloadHelper -> CheckInternetConnection2 ...");
            DateTime startTime = DateTime.Now;
            using (var localClient = new HttpClient())
            {
                try
                {
                    //localClient.MaxResponseContentBufferSize = 256000;
                    localClient.Timeout = TimeSpan.FromMilliseconds(timeout);
                    //localClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //localClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
                    
                    //localClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var uri = new Uri("https://www.google.com/");
                    var response = await localClient.GetAsync(uri);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        connectionOK = true;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"DownloadHelper ->  CheckInternetConnection2, ex= {ex.Message}");
                }
            }

            TimeSpan deltaTime = DateTime.Now - startTime;
            Debug.WriteLine($"DownloadHelper -> CheckInternetConnection2, googleOK = {connectionOK}, deltaTime = {deltaTime.TotalMilliseconds} ms, timeout = {timeout}");
            return connectionOK;
        }
        */
        public static bool CheckInternetConnectionProgressive()
        {
            if (CheckInternetConnection(500)) return true;
            if (CheckInternetConnection(600)) return true;           
            return CheckInternetConnection(1000);
        }
        /*
        public static async Task<bool> CheckInternetConnectionProgressive2()
        {
            if (await CheckInternetConnection2(500)) return true;
            if (await CheckInternetConnection2(600)) return true;
            return await CheckInternetConnection2(1000);
        }
        */
        public static Task DownLoadWords(   List<string> wordsToDownload1, THEME_NAME theme1, 
                                         List<string> wordsToDownload2, THEME_NAME theme2)
        {
            BreakDownloading = false;
            result = RESULT.NONE;
            progress = 0;
            totalFilesDownloaded = 0;
            return Task.Run(async()=> {
                await DownloadWordsAsync(wordsToDownload1, theme1, wordsToDownload2, theme2);
                return;
            });
            
        }

        public static void DownLoadVideo(string uri, string filename)
        {
            BreakDownloading = false;
            result = RESULT.NONE;
            progress = 0;
            totalFilesDownloaded = 0;
            DownloadVideoAsync(uri, filename);
        }

        public async static Task Download2AudioAsync_InBckground(string uri1, string filename1, string uri2, string filename2)
        {
            isDownloading_InBackground = true;
            result_InBckground = RESULT.NONE;
            totalFilesDownloaded_InBckground = 0;

            Debug.WriteLine("Task DownloadAudioAsync_InBckground");
            if (OnLoadingStarted_InBckground != null) Device.BeginInvokeOnMainThread(OnLoadingStarted_InBckground);
            else return;
            result_InBckground = RESULT.DOWNLOAD_STARTED;

            if (CrossConnectivity.Current.IsConnected && CheckInternetConnectionProgressive())
            {
                //connection is ok
                int targetTotal = 0;
                if (!string.IsNullOrEmpty(uri1)) { 
                    targetTotal++;
                    await GetAudioAsync_InBckground(uri1, filename1);
                }
                if (!string.IsNullOrEmpty(uri2)) {
                    targetTotal++;
                    await GetAudioAsync_InBckground(uri2, filename2);
                }
                if (totalFilesDownloaded_InBckground == targetTotal) result_InBckground = RESULT.DOWNLOADED_OK;
                // *** // else result_InBckground = RESULT.ERROR;
                else result_InBckground = RESULT.DOWNLOADED_OK;
            }
            else
            {
                 result_InBckground = RESULT.ERROR_NO_CONNECTION;
            }

            Debug.WriteLine("Download2AudioAsync_InBckground ended, uri1 =" + uri1 + ", result = " + result_InBckground);
            isDownloading_InBackground = false;
            if (OnLoadingEnded_InBckground != null) Device.BeginInvokeOnMainThread(OnLoadingEnded_InBckground);
        }

        /*
        public async static Task DownloadVideoAsync_InBckground(string uri, string filename)
        {
            isDownloading_InBackground = true;
            result_InBckground = RESULT.NONE;
            progress_InBckground = 0;
            totalFilesDownloaded_InBckground = 0;

            Debug.WriteLine("public async static Task DownloadVideoAsync_InBckground");
            Debug.WriteLine("need to download video ?");

            if (await PCLHelper.IsFileExistAsync(filename) && false) // we don't need dowload anything if 
            { // if we have all the files
                Debug.WriteLine($"file {filename} EXISTS! , don't need to download!");
                result_InBckground = RESULT.NOTHING_TO_DOWNLOAD;
            }
            else
            { // we do not have the file - try to download whem

                Device.BeginInvokeOnMainThread(OnLoadingStarted_InBckground);
                result_InBckground = RESULT.DOWNLOAD_STARTED;

                if (CheckConnectivity() > 0)
                {
                    //connection is ok
                    await GetVideoAsync_InBckground(uri, filename);
                    if (totalFilesDownloaded_InBckground == 1) result_InBckground = RESULT.DOWNLOADED_OK;
                    else result_InBckground = RESULT.ERROR;
                }
                else
                {
                    result_InBckground = RESULT.ERROR_NO_CONNECTION;
                }
            }
            Debug.WriteLine("DownloadVideoAsync_InBckground ended, result = " + result_InBckground);
            isDownloading_InBackground = false;
            Device.BeginInvokeOnMainThread(OnLoadingEnded_InBckground);
        }
*/

        public async static Task DownloadVideoAsync(string uri, string filename)
        {
            Debug.WriteLine("public async static Task DownloadVideoAsync");
            Debug.WriteLine("need to download video ?");

            if (await PCLHelper.IsFileExistAsync(filename)) // we don't need dowload anything if 
            { // if we have all the files
                Debug.WriteLine($"file {filename} EXISTS! , don't need to download!");
                result = RESULT.NOTHING_TO_DOWNLOAD;
            }
            else
            { // we do not have the file - try to download whem
                
                if (OnLoadingStarted != null) Device.BeginInvokeOnMainThread(OnLoadingStarted);

                result = RESULT.DOWNLOAD_STARTED;

                if (CrossConnectivity.Current.IsConnected && CheckInternetConnectionProgressive()) {
                    //connection is ok
                    await GetVideoAsync(uri, filename);
                    //result = RESULT.DOWNLOADED_OK;
                    //totalFilesDownloaded = 1;
                    //Debug.WriteLine("FAST LOADING VIDEO SIMULATED...");

                    if (totalFilesDownloaded == 1) result = RESULT.DOWNLOADED_OK;
                    else result = RESULT.ERROR;
                }
                else {
                    result = RESULT.ERROR_NO_CONNECTION;
                }
            }

            Debug.WriteLine("");
            if (!BreakDownloading && OnLoadingEnded != null) Device.BeginInvokeOnMainThread(OnLoadingEnded);
        }

        public async static Task DownloadWordsAsync(List<string> wordsToDownload1, THEME_NAME theme1, List<string> wordsToDownload2, THEME_NAME theme2) {
            //get current language
            string language = UserHelper.Language;
            // check if we have gameObject file with paths


            Debug.WriteLine("public async static Task DownloadWordsAsync");
            int keysCount1, keysCount2;
            //List<string> keysToDownload1 = await App.Database.GetUnsavedWordKeys(wordsToDownload1, language, theme1);
            List<string> keysToDownload1 = await GetUnsavedWordKeys(wordsToDownload1, language);
            keysCount1 = keysToDownload1.Count;
            keysCount2 = 0;
            List<string> keysToDownload2 = null;
            if (wordsToDownload2 != null) {
                //keysToDownload2 = await App.Database.GetUnsavedWordKeys(wordsToDownload2, language, theme2);
                keysToDownload2 = await GetUnsavedWordKeys(wordsToDownload2, language);
                keysCount2 = keysToDownload2.Count;
            }
            Debug.WriteLine("need to download " + (keysCount1+keysCount2) + " files");

            if (keysCount1 == 0 & keysCount2 == 0) // we don't need dowload anything
            { // if we have all the files
                result = RESULT.NOTHING_TO_DOWNLOAD;
            }
            else
            { // we do not have the files - try to download whem
                
                if (OnLoadingStarted != null) Device.BeginInvokeOnMainThread(OnLoadingStarted);
                result = RESULT.DOWNLOAD_STARTED;
                if (CrossConnectivity.Current.IsConnected && CheckInternetConnectionProgressive())
                {
                    // download audio
                    if (Device.RuntimePlatform == Device.Android) await Task.Delay (750);    
                    if (keysCount1 > 0) await GetAudioWordsAsync(keysToDownload1, theme1, language, keysCount1 + keysCount2);
                    if (keysCount2 > 0) await GetAudioWordsAsync(keysToDownload2, theme2, language, keysCount1 + keysCount2);

                    //process result ..., we may think about textKeysCount == 0 ???
                    if (keysCount1 + keysCount2 - totalFilesDownloaded <= 2)
                    {
                        result = RESULT.DOWNLOADED_OK;
                    }
                   
                    else result = RESULT.ERROR;

                }
                else
                {
                    Debug.WriteLine("result = RESULT.ERROR_NO_CONNECTION");
                    result = RESULT.ERROR_NO_CONNECTION;
                }
            }

            if (!BreakDownloading && OnLoadingEnded != null) Device.BeginInvokeOnMainThread(OnLoadingEnded);
        }

        public static Task<GameObjects> GetGameObjects(string language)
        {
            Debug.WriteLine("PostRequest() here... GetGameObjects(string language)");
            //string url = $"https://dinolingo.com/games/memory-game/{language}/objects.js";
            //string url = $"https://dinolingo.com/games/word-wheel-game/{language}/objects.js";
            string lang = language.Replace("-", "%20");
            string url = $"https://dinolingo.com/games/quiz-game/{lang}/objects.js";


            Debug.WriteLine("url: " + url);
            var webReq = (HttpWebRequest)WebRequest.Create(url);
            int totalBytes;
            GameObjects gameObjects = null;

            return Task.Run(async () =>
            {
                try
                {
                    Debug.WriteLine("Try my HttpWebRequest..., uri = " + url);
                    webReq.CookieContainer = new CookieContainer();
                    webReq.Method = "GET";
                    using (WebResponse response = webReq.GetResponse())
                    {
                        totalBytes = Int32.Parse(response.Headers[HttpResponseHeader.ContentLength]);
                        Debug.WriteLine("totalBytes = " + totalBytes);
                        var responseString = await new StreamReader(response.GetResponseStream()).ReadToEndAsync();
                        Debug.WriteLine("responseString: " + responseString);

                        //process the response here
                        string s = GameObjects.UniformIncomingStringToObject(responseString);
                        Debug.WriteLine("UniformIncomingStringToObject: " + s);

                        gameObjects = JsonConvert.DeserializeObject<GameObjects>(s);
                        // try to save object to cache
                        await CacheHelper.Add<GameObjects>(CacheHelper.GAME_OBJECTS + language, gameObjects);
                        //Debug.WriteLine("gameObject cached ? :" + await CacheHelper.Exists(CacheHelper.GAME_OBJECTS + language));
                    } 
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("HttpWebRequest EXCEPTION :(, downloading gameObject failed :(");
                }
                Debug.WriteLine("gameObjects0 = " + JsonConvert.SerializeObject(gameObjects));
                return gameObjects;
            });
        }

        static async Task<List<string>> GetUnsavedWordKeys(List<string> wordsToDownload, string language) {
            
            List<string> keysToDownload = new List<string>();
            foreach (string key in wordsToDownload) {
                if ( ! (await PCLHelper.IsFileExistAsync(key + "_" + language + ".mp3"))) {
                    keysToDownload.Add(key);
                }
                else {
                    //
                }
            }
            return keysToDownload;
        }

        public static async Task<int> IsFirstKeyWordSaved(THEME_NAME themeName, string language)
        {
            // === ===
            // 1 - saved
            // -1 - not saved
            // 0 - have no key in game objects
            // === ===
            // get first key
            foreach (string key in Theme.Resources[(int)themeName].Item.Keys)
            {
                if (GameHelper.memory_GameObjects.HasKey(themeName, key) || GameHelper.sas_GameObjects.HasKey(themeName, key))
                {
                    // we have the key here, check if it's been saved
                    if (await PCLHelper.IsFileExistAsync(key + "_" + language + ".mp3"))
                    {
                        return 1;
                    }
                    return -1;
                }
            } 
            return 0;
        }

        static async Task<List<string>> GetUnsavedTextWordKeys(List<string> wordsToDownload, string language)
        {
            List<string> textKeysToDownload = new List<string>();
            foreach (string key in wordsToDownload)
            {
                /*
                if (!(await CacheHelper.Exists(CacheHelper.TEXT_FOR_KEY + key + language)))
                {
                    textKeysToDownload.Add(key);
                }
                else
                {
                    //
                }
                */
            }
            return textKeysToDownload;
        }

        public static async Task GetVideoAsync(string uri, string filename)
        {
            DateTime start, end;
            start = DateTime.Now;
            HttpWebRequest webReq;
            Uri url = new Uri(uri); ;

            if (BreakDownloading) return;
            byte[] bytes = default(byte[]); //null;
            byte[] buffer = new byte[4096];
            var memstream = new MemoryStream();
            int receivedBytes = 0;
            int totalBytes = 0;
            double oldProgress = progress;
            webReq = (HttpWebRequest)HttpWebRequest.Create(url);
            try
            {
                Debug.WriteLine("Try my HttpWebRequest..., uri = " + uri);
                webReq.CookieContainer = new CookieContainer();
                webReq.Method = "GET";

                using (WebResponse response = webReq.GetResponse())
                {
                    totalBytes = Int32.Parse(response.Headers[HttpResponseHeader.ContentLength]);
                    bytes = new byte[totalBytes];
                    Debug.WriteLine("totalBytes = " + totalBytes);
                    using (Stream stream = response.GetResponseStream())
                    {
                        Debug.WriteLine("try to read buffer...");
                        for (; ; )
                        {
                            //await Task.Delay(500);
                            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                            Debug.WriteLine("bytesRead = " + bytesRead);
                            if (bytesRead == 0 || BreakDownloading)
                            {
                                Debug.WriteLine("ZERO bytes - end of stream");
                                await Task.Yield();
                                break;
                            }

                            Buffer.BlockCopy(buffer, 0, bytes, receivedBytes, bytesRead);
                            receivedBytes += bytesRead;
                            //update progress
                            progress = (((double)receivedBytes) / totalBytes);

                            if (progress - oldProgress > 0.009 || progress > 0.99)
                            {
                                Debug.WriteLine("receivedBytes = " + receivedBytes);
                                oldProgress = progress;
                                Device.BeginInvokeOnMainThread(OnLoadingProgress);
                            }

                        }
                        Debug.WriteLine("receivedBytes= " + receivedBytes);

                        if (BreakDownloading) return;
                        Debug.WriteLine("Try to save video to file..." + filename);

                        if (receivedBytes == totalBytes) await PCLHelper.SaveImage(bytes, filename);

                        if (await PCLHelper.IsFileExistAsync(filename))
                        {
                            Debug.WriteLine("file successfully saved! " + filename);
                            totalFilesDownloaded++;
                            //add to database
                            //await App.Database.SaveItemAsync(new Word { Key = key, Theme = theme, Language = language, FilePath = key + "_" + language + ".mp3" });
                            progress = 1.0;
                            Device.BeginInvokeOnMainThread(OnLoadingProgress);
                            Debug.WriteLine("Need to add video to db ");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine("HttpWebRequest EXCEPTION :(, video not loaded");
            }

            end = DateTime.Now;
            TimeSpan interval = end - start;
            Debug.WriteLine($"{totalFilesDownloaded} video downloaded and saved, total time, s = " + interval.TotalSeconds);
            Debug.WriteLine("file successfully saved? = " + await PCLHelper.IsFileExistAsync(filename));
        }

        static async Task GetAudioAsync_InBckground(string uri, string filename)
        {
            DateTime start, end;
            start = DateTime.Now;
            HttpWebRequest webReq;
            Uri url = new Uri(uri);

            byte[] bytes = default(byte[]); //null;
            var memstream = new MemoryStream();
            int totalBytes = 0;
            webReq = (HttpWebRequest)HttpWebRequest.Create(url);
            webReq.Timeout = 30000;

            try
            {
                Debug.WriteLine("Try my HttpWebRequest..., uri = " + uri);
                webReq.CookieContainer = new CookieContainer();
                webReq.Method = "GET";
                using (WebResponse response = webReq.GetResponse())
                {
                    totalBytes = Int32.Parse(response.Headers[HttpResponseHeader.ContentLength]);
                    Debug.WriteLine("totalBytes = " + totalBytes);
                    using (Stream stream = response.GetResponseStream())
                    {
                        // === OLD WAY
                        await stream.CopyToAsync(memstream);
                        bytes = memstream.ToArray();
                        Debug.WriteLine("Try to save audio to file..." + filename);

                        if (bytes.Length == totalBytes) await PCLHelper.SaveImage(bytes, filename);

                        if (await PCLHelper.IsFileExistAsync(filename))
                        {
                            Debug.WriteLine("file successfully saved! " + filename);
                            totalFilesDownloaded_InBckground++;
                            //add to database
                            //await App.Database.SaveItemAsync(new Word { Key = key, Theme = theme, Language = language, FilePath = key + "_" + language + ".mp3" });
                            if (OnLoadingProgress_InBckground == null)
                            {
                                Debug.WriteLine("DownloadHelper -> OnLoadingProgress_InBckground == null - break download ?");
                            }
                            else
                            {
                                Device.BeginInvokeOnMainThread(OnLoadingProgress_InBckground);
                            }
                            
                            Debug.WriteLine("Need to add audio to db ");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                // delete file if have some
                if (await PCLHelper.IsFileExistAsync(filename))
                {
                    await PCLHelper.DeleteFile(filename);
                }
                GoogleAnalytics.Current.Tracker.SendEvent("DownloadHelper -> GetAudioAsync_InBckground", $"Exception, url= {url}", "GetResponse().ex: " + ex.Message, 3);
                Debug.WriteLine("HttpWebRequest EXCEPTION :(, audio not loaded _InBckground, uri : " + uri);
            }


            end = DateTime.Now;
            TimeSpan interval = end - start;
            Debug.WriteLine($"{totalFilesDownloaded_InBckground} audio downloaded and saved, total time _InBckground, s = " + interval.TotalSeconds);
            Debug.WriteLine("file successfully saved _InBckground? = " + await PCLHelper.IsFileExistAsync(filename));
        }

        public static async Task<Boolean> SimpleAudioLoader(string uri, string filename)
        {
            DateTime start = DateTime.Now;
            HttpWebRequest webReq;
            Uri url = new Uri(uri);

            byte[] bytes = default(byte[]); //null;
            var memstream = new MemoryStream();
            int totalBytes = 0;
            webReq = (HttpWebRequest)HttpWebRequest.Create(url);
            webReq.Timeout = 30000;

            try
            {
                Debug.WriteLine("SimpleAudioLoader HttpWebRequest..., uri = " + uri);
                webReq.CookieContainer = new CookieContainer();
                webReq.Method = "GET";
                using (WebResponse response = webReq.GetResponse())
                {
                    totalBytes = Int32.Parse(response.Headers[HttpResponseHeader.ContentLength]);
                    Debug.WriteLine("SimpleAudioLoader, totalBytes = " + totalBytes);
                    using (Stream stream = response.GetResponseStream())
                    {
                        // === OLD WAY
                        await stream.CopyToAsync(memstream);
                        bytes = memstream.ToArray();
                        Debug.WriteLine("SimpleAudioLoader, Try to save audio to file..." + filename);

                        try
                        {
                            if (bytes.Length == totalBytes) await PCLHelper.SaveImage(bytes, filename);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("SimpleAudioLoader, error saving file, ex= " + ex.Message);
                        }
                        

                        if (await PCLHelper.IsFileExistAsync(filename))
                        {
                           
                            Debug.WriteLine($"SimpleAudioLoader, audio downloaded and saved, total time, s = " + (DateTime.Now -start).TotalSeconds);
                            Debug.WriteLine("SimpleAudioLoader, file successfully saved ? = " + await PCLHelper.IsFileExistAsync(filename));

                            return true;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                // delete file if have some
                if (await PCLHelper.IsFileExistAsync(filename))
                {
                    await PCLHelper.DeleteFile(filename);
                }
                GoogleAnalytics.Current.Tracker.SendEvent("SimpleAudioLoader", $"Exception, url= {url}", "GetResponse().ex: " + ex.Message, 3);
                Debug.WriteLine("SimpleAudioLoader -> HttpWebRequest EXCEPTION :(, audio not loaded _InBckground, uri : " + uri);
            }


            Debug.WriteLine($"SimpleAudioLoader, ERROR, total time, s = " + (DateTime.Now - start).TotalSeconds);
            Debug.WriteLine("SimpleAudioLoader, file successfully saved ? = " + await PCLHelper.IsFileExistAsync(filename));
            return false;
        }

        public static async Task<Boolean> SimpleAudioLoaderNull()
        {
            return true;
        }

        public static Task<bool> GetVideoAsync_InBckground(string uri, string filename)
        {
            DateTime start, end;
            start = DateTime.Now;
            HttpWebRequest webReq;
            Uri url = new Uri(uri);

            byte[] bytes = default(byte[]); //null;
            byte[] buffer = new byte[4096*4];
            var memstream = new MemoryStream();
            int receivedBytes = 0;
            int totalBytes = 0;
            return Task.Run(async () =>
            {
                bool downloadedOK = false;
                webReq = (HttpWebRequest)HttpWebRequest.Create(url);
                webReq.Timeout = 60000 * 5;

                try
                {
                    Debug.WriteLine("DownloadHelper -> download video, uri = " + uri);
                    webReq.CookieContainer = new CookieContainer();
                    webReq.Method = "GET";

                    using (WebResponse response = webReq.GetResponse())
                    {
                        totalBytes = Int32.Parse(response.Headers[HttpResponseHeader.ContentLength]);
                        bytes = new byte[totalBytes];
                        Debug.WriteLine("DownloadHelper -> download video -> totalBytes = " + totalBytes);
                        using (Stream stream = response.GetResponseStream())
                        {
                            Debug.WriteLine("DownloadHelper -> download video -> try to read buffer...");
                            double c = 0.0;
                            for (; ; )
                            {
                                //await Task.Delay(1);
                                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                                //Debug.WriteLine("DownloadHelper -> download video -> bytesRead = " + bytesRead);
                                if (bytesRead == 0)
                                {
                                    Debug.WriteLine("DownloadHelper -> download video -> ZERO bytes - end of stream");
                                    await Task.Yield();
                                    break;
                                }

                                Buffer.BlockCopy(buffer, 0, bytes, receivedBytes, bytesRead);
                                receivedBytes += bytesRead;
                                //update progress
                                progress_InBckground = (((double)receivedBytes) / totalBytes);
                                if (progress_InBckground > c) {
                                    c += 0.02;
                                    Debug.WriteLine("DownloadHelper -> download video -> bytesRead = " + bytesRead + ", (%) = " + progress_InBckground);
                                }
                            }

                            Debug.WriteLine("DownloadHelper -> download video -> Try to save video to file..." + filename);

                            if (receivedBytes == totalBytes) await PCLHelper.SaveImage(bytes, filename);

                            if (await PCLHelper.IsFileExistAsync(filename))
                            {
                                Debug.WriteLine("DownloadHelper -> download video -> file successfully saved! " + filename);
                                downloadedOK = true;
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine("DownloadHelper -> download video -> HttpWebRequest EXCEPTION :(, video not loaded _InBckground");
                    GoogleAnalytics.Current.Tracker.SendEvent("DownloadHelper -> GetVideoAsync_InBckground", $"Exception, url= {url}", "GetResponse().ex: " + ex.Message, 3);
                }


                end = DateTime.Now;
                TimeSpan interval = end - start;
                Debug.WriteLine("DownloadHelper -> download video -> total time _InBckground, s = " + interval.TotalSeconds);
                Debug.WriteLine("file successfully saved _InBckground? = " + await PCLHelper.IsFileExistAsync(filename));

                return downloadedOK;
            });
        }

        public static Task<SoundItem[]> GetTextKeysAsyncDebug(THEME_NAME theme, string language)
        {
            DateTime start, end;
            start = DateTime.Now;

            Debug.WriteLine("GET Request() here... GetTextKeysAsync(List<string> textKeysToDownload, string  language,GameObjects gameObjects)");
            string stringTheme = Theme.Resources[(int)theme].StringName.Replace(" ", "_").ToLower();

            string url = $"https://dinolingo.com/games/see-and-say-game/{language}/{stringTheme}.html";
            Debug.WriteLine("url: " + url);
            var webReq = (HttpWebRequest)WebRequest.Create(url);
            int totalBytes;

            return Task.Run(async () =>
            {
                SoundItem[] soundItems = null;

                try
                {
                    Debug.WriteLine("Try my HttpWebRequest..., uri = " + url);
                    //HtmlWeb web = new HtmlWeb();  
                    //HtmlDocument document = web.Load(url);

                    webReq.CookieContainer = new CookieContainer();
                    webReq.Method = "GET";

                    using (WebResponse response = webReq.GetResponse())
                    {
                        totalBytes = Int32.Parse(response.Headers[HttpResponseHeader.ContentLength]);
                        Debug.WriteLine("totalBytes = " + totalBytes);
                        var responseString = await new StreamReader(response.GetResponseStream()).ReadToEndAsync();
                        Debug.WriteLine("responseString: " + responseString);

                        //PREPROCESS RESP STRING

                        //process the response here
                        var htmlDoc = new HtmlDocument();
                        Debug.WriteLine($"try to load html, theme = {theme}, lang = {language}");
                        htmlDoc.LoadHtml(responseString);
                        Debug.WriteLine($"htmlDoc.LoadHtml(responseString) - OK, theme = {theme}, lang = {language}");

                        HtmlNode[] NodesText = htmlDoc.DocumentNode.SelectNodes("//div[@class='label']").ToArray(); // its ok
                        HtmlNode[] allDivs = htmlDoc.DocumentNode.SelectNodes("//div").ToArray();
                        Debug.WriteLine($"selected node arrays - OK, , theme = {theme}, lang = {language}");


                        List<HtmlNode> selectedNodes = new List<HtmlNode>();
                        foreach (HtmlNode n in allDivs) {
                            string data_sound = n.GetAttributeValue("data-sound", "");
                            if (string.IsNullOrEmpty(data_sound)) continue;
                            selectedNodes.Add(n);
                        }
                        Debug.WriteLine($"NodesText.Length = {NodesText.Length}, selectedNodes.Count = {selectedNodes.Count}, theme = {theme}, lang = {language}");
                        soundItems = new SoundItem[Theme.Resources[(int)theme].Item.Count];

                        Console.WriteLine($"Selected nodes: theme = {theme}, lang = {language}");
                        for (int i = 0; i < NodesText.Length; i++)
                        //foreach (HtmlNode n in NodesText)
                        {
                            string innerText = NodesText[i].InnerText;
                            Debug.WriteLine("label innerText: " + innerText);
                            string data_sound = selectedNodes[i].GetAttributeValue("data-sound", "");
                            string id = selectedNodes[i].GetAttributeValue("id", "");

                            //PREPROCESS RESP STRING
                            id = id.Replace("_", string.Empty).Replace("-", string.Empty);
                            data_sound = data_sound.Replace("\n", string.Empty);

                            int index = 0;
                            foreach (KeyValuePair<string, ItemInfo> entry in Theme.Resources[(int)theme].Item)
                            {
                                

                                if (entry.Key.ToLower() == id.ToLower())
                                //if (id.ToLower().Contains(entry.Key.ToLower()))
                                {
                                    // we have our text here
                                    // inner text is our text

                                    soundItems[index] = new SoundItem { id = entry.Key.ToLower(), text = innerText, sound = data_sound };
                                    Debug.WriteLine($"id = {soundItems[index].id}, text = {innerText}, sound = {soundItems[index].sound}");
                                    break;

                                }
                                index++;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"HttpWebRequest EXCEPTION :(, downloading textKeys failed :(, theme = {theme}, lang = {language}");
                }

                end = DateTime.Now;
                TimeSpan interval = end - start;
                Debug.WriteLine("total time _InBckground, s = " + interval.TotalSeconds);
                if (soundItems != null) Debug.WriteLine("soundItems.Length = " + soundItems.Length);
                return soundItems;
            });
        }


        static Task<int> GetTextKeysAsync(List<string> textKeysToDownload, THEME_NAME theme, string  language)
        {
            DateTime start, end;
            start = DateTime.Now;

            Debug.WriteLine("GET Request() here... GetTextKeysAsync(List<string> textKeysToDownload, string  language,GameObjects gameObjects)");
            string stringTheme = Theme.Resources[(int)theme].StringName.Replace(" ", "_").ToLower();
            string url = $"https://dinolingo.com/games/see-and-say-game/{language}/{stringTheme}.html";
            Debug.WriteLine("url: " + url);
            Debug.WriteLine("need to download " + textKeysToDownload.Count + " textKeys");
            var webReq = (HttpWebRequest)WebRequest.Create(url);
            int totalBytes;


            return Task.Run(async () =>
            {
                try
                {
                    Debug.WriteLine("Try my HttpWebRequest..., uri = " + url);

                    //HtmlWeb web = new HtmlWeb();  
                    //HtmlDocument document = web.Load(url);

                    webReq.CookieContainer = new CookieContainer();
                    webReq.Method = "GET";
                    using (WebResponse response = webReq.GetResponse())
                    {
                        totalBytes = Int32.Parse(response.Headers[HttpResponseHeader.ContentLength]);
                        Debug.WriteLine("totalBytes = " + totalBytes);
                        var responseString = await new StreamReader(response.GetResponseStream()).ReadToEndAsync();
                        Debug.WriteLine("responseString: " + responseString);

                        //process the response here
                        var htmlDoc = new HtmlDocument();
                        htmlDoc.LoadHtml(responseString);

                        HtmlNode [] Nodes = htmlDoc.DocumentNode.SelectNodes("//div[@class='label']").ToArray(); 
                        Console.WriteLine("Selected nodes: ");
                        foreach (HtmlNode n in Nodes) {
                            string sound_id = n.GetAttributeValue("data-soundid", "");
                            string innerText = n.InnerText;
                            Debug.WriteLine("node: " + innerText + ", sound_id: " + sound_id);
                            int id = -1;
                            if (!int.TryParse(sound_id, out id)) continue;
                            if (id < 0) continue;
                            foreach (KeyValuePair<string, ItemInfo> entry in Theme.Resources[(int)theme].Item)
                            {
                                Debug.WriteLine("entry.Value.id = " + entry.Value.id + ", id = " + id);
                                if (entry.Value.id == id) {
                                    
                                    // we have our text here
                                    // try to save text to cache
                                    /*
                                    await CacheHelper.Add(CacheHelper.TEXT_FOR_KEY + entry.Key + language, innerText);
                                    if (await CacheHelper.Exists(CacheHelper.TEXT_FOR_KEY + entry.Key + language)) {
                                        Debug.WriteLine($"textKey {entry.Key} cached:" + (await CacheHelper.GetAsync(CacheHelper.TEXT_FOR_KEY + entry.Key + language)).Data);
                                        textKeysToDownload.Remove(entry.Key);
                                        break;
                                    }
                                    */
                                }
                            } 
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("HttpWebRequest EXCEPTION :(, downloading textKeys failed :(");
                }

                end = DateTime.Now;
                TimeSpan interval = end - start;
                Debug.WriteLine("total time _InBckground, s = " + interval.TotalSeconds);

                return textKeysToDownload.Count;
            });
        }

        static async Task GetAudioWordsAsync(List<string> keysToDownload, THEME_NAME theme, string language, int totalFiles) {
            DateTime start, end;
            start = DateTime.Now;

            HttpWebRequest webReq;
            Uri url;
            string soundFile;
            string url_prefix = string.Empty;
            string langPath;

            int totalErrors = 0;
            foreach (string key in keysToDownload)
            {
                langPath = GameHelper.memory_GameObjects.LANG;
                if (!string.IsNullOrEmpty(soundFile = GameHelper.memory_GameObjects.GetSoundUrl(theme, key))) {
                    url_prefix = GameHelper.memory_UrlPrefix;
                }
                else if (!string.IsNullOrEmpty(soundFile = GameHelper.sas_GameObjects.GetSoundUrl(theme, key)))
                {
                    url_prefix = GameHelper.sas_UrlPrefix;
                }

                if (BreakDownloading) break;

                url = new Uri(GetUrlForWord(key, theme, url_prefix, langPath,  soundFile));
                byte[] bytes =  default(byte[]); //null;
                byte[] buffer = new byte[4096];
                var memstream = new MemoryStream();
                int receivedBytes = 0;
                int totalBytes = 0;

                Debug.WriteLine("Key = " + key + ", Url =" +  url);
                webReq = (HttpWebRequest)HttpWebRequest.Create(url);
                webReq.Timeout = 30000;

                try
                {
                    Debug.WriteLine("Try my HttpWebRequest...");
                    webReq.CookieContainer = new CookieContainer();
                    webReq.Method = "GET";

                    using (WebResponse response = webReq.GetResponse())
                    {
                        totalBytes = Int32.Parse(response.Headers[HttpResponseHeader.ContentLength]);
                        bytes = new byte[totalBytes];
                        Debug.WriteLine("totalBytes = " + totalBytes);
                        using (Stream stream = response.GetResponseStream())
                        {
                            //StreamReader reader = new StreamReader(stream);
                            Debug.WriteLine("try to read buffer...");
                            for (; ; )
                            {
                               int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                                Debug.WriteLine("bytesRead = " + bytesRead);
                                if (bytesRead == 0 || BreakDownloading)
                                {
                                    Debug.WriteLine("ZERO bytes - end of stream");
                                    await Task.Yield();
                                    break;
                                }

                                Buffer.BlockCopy(buffer, 0, bytes, receivedBytes, bytesRead);
                                receivedBytes += bytesRead;
                                Debug.WriteLine("receivedBytes = " + receivedBytes);
                                //update progress
                                progress = (((double)receivedBytes) / totalBytes) * 1 / totalFiles + (float) totalFilesDownloaded / totalFiles;

                                Debug.WriteLine("receivedBytes = " + receivedBytes);
                                if (OnLoadingProgress != null) Device.BeginInvokeOnMainThread(OnLoadingProgress);
                            }

                            Debug.WriteLine("receivedBytes= " + receivedBytes);

                            Debug.WriteLine("Try to save mp3 to file..." + key+ "_" + language + ".mp3");
                            if (BreakDownloading) break;
                            if (receivedBytes == totalBytes) await PCLHelper.SaveImage(bytes, key + "_" + language + ".mp3");

                            if (await PCLHelper.IsFileExistAsync(key + "_" + language + ".mp3"))
                            {
                                Debug.WriteLine("file successfully saved! " + key + "_" + language + ".mp3");
                                totalFilesDownloaded++;
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    GoogleAnalytics.Current.Tracker.SendEvent("DownloadHelper -> GetAudioWordsAsync", $"Exception, url= {url}", "GetResponse().ex: " + ex.Message, 3);
                    Debug.WriteLine("HttpWebRequest EXCEPTION :( while downloading audio, key :" + key);
                    totalErrors++;

                    if (totalErrors > 2) break;
                }
            }

            progress = (float)totalFilesDownloaded / totalFiles;
            if (OnLoadingProgress != null)             Device.BeginInvokeOnMainThread(OnLoadingProgress);
            else
            {
                Debug.WriteLine("DownloadHelper -> OnLoadingProgress == null, can not execute");
            }

            end = DateTime.Now;
            TimeSpan interval = end - start;
            Debug.WriteLine($"{totalFilesDownloaded} audio downloaded and saved, total time, s = " + interval.TotalSeconds); 

        }


        public static string GetUrlForWord(string key, THEME_NAME themeName, string url_prefix, string langPath, string soundFile)
        {
            /*
            if (key == "HELLOW") return $"https://dinolingo.com/games/quiz-game/extras/sound/{langPath}/11.mp3";
            if (key == "HOWAREYOU") return $"https://dinolingo.com/games/quiz-game/extras/sound/{langPath}/12.mp3";
            if (key == "GOODBYE") return $"https://dinolingo.com/games/quiz-game/extras/sound/{langPath}/15.mp3";
            */
            //https://storage.googleapis.com/dino-lingo-web-games
            if (key == "HELLOW") return $"https://storage.googleapis.com/dino-lingo-web-games/quiz-game/extras/sound/{langPath}/11.mp3";
            if (key == "HOWAREYOU") return $"https://storage.googleapis.com/dino-lingo-web-games/quiz-game/extras/sound/{langPath}/12.mp3";
            if (key == "GOODBYE") return $"https://storage.googleapis.com/dino-lingo-web-games/quiz-game/extras/sound/{langPath}/15.mp3";

            string id = key;
            string audioUrl;

            Debug.WriteLine($"sound for key = {key}, themeName = {themeName}, langPath = {langPath}, soundFile = {soundFile}");
            if (string.IsNullOrEmpty(soundFile)) return string.Empty;
            audioUrl = url_prefix + langPath + "/" + Theme.Resources[(int)themeName].StringName.Replace(" ", "%20") + "/" + soundFile.Replace(" ", "%20").Replace($"'", "%27");

            Debug.WriteLine("Target url for audio :" + audioUrl);

            return audioUrl;
        }

        public async static void ParseAllMultySpaceAudios()
        {
            List<string> all_lang_cats = new List<string>(LANGUAGES.CAT_INFO.Keys);
            List<string> gameObjectsIds = new List<string> { CacheHelper.MEMORY_GAMEOBJECTS, CacheHelper.SAS_GAMEOBJECTS};
            List<string> urlPrefixes = new List<string> { GameHelper.memory_UrlPrefix, GameHelper.sas_UrlPrefix};
            string url = "";
            for (int i = 0; i< 2; i++)
            {
                foreach (string lang_cat in all_lang_cats)
                {
                    GameObjects gameobjects;
                    if (await CacheHelper.Exists(gameObjectsIds[i] + lang_cat))
                    {
                        

                        gameobjects = await CacheHelper.GetAsync<GameObjects>(gameObjectsIds[i] + lang_cat);

                        

                        // get list of themes
                        if (gameobjects != null)
                        {
                            string langPath = gameobjects.LANG;
                            if (await CacheHelper.Exists(gameObjectsIds[0] + lang_cat))
                            {
                                GameObjects memoryGameobjects = await CacheHelper.GetAsync<GameObjects>(gameObjectsIds[0] + lang_cat);
                                if (memoryGameobjects != null) langPath = memoryGameobjects.LANG;
                            }
                           

                            foreach (ThemeResource themeResource in Theme.Resources)
                            {
                                foreach (string key in themeResource.Item.Keys)
                                {
                                    if (gameobjects.HasKey(themeResource.Name, key))
                                    {

                                        // get audio file name
                                        string soundFile = gameobjects.GetSoundUrl(themeResource.Name, key);
                                        if (!string.IsNullOrEmpty(soundFile))
                                        {
                                            if (soundFile.Contains("  "))
                                            {
                                                url += urlPrefixes[i] + langPath + "/" + themeResource.StringName.Replace(" ", "%20") + "/" + soundFile.Replace(" ", "%20").Replace($"'", "%27") + "\n";

                                            }
                                        }
                                    }
                                }

                            }
                        }
                        
                    } 
                }
            }

            Debug.WriteLine($"MultySpaceAudios: {url}");
        }
        /*
        static int CheckConnectivity()
        {
            Debug.WriteLine("CrossConnectivity.Current.IsConnected = ?");
            if (CrossConnectivity.Current.IsConnected && CheckInternetConnection())
            {
                Debug.WriteLine("CrossConnectivity.Current.IsConnected = true");
                return 1;
            }
            else
            {
                Debug.WriteLine("CrossConnectivity.Current.IsConnected = false");
                return -1;
            }
        }
        */
    }


}
