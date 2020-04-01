using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Connectivity;
using Plugin.GoogleAnalytics;
using Xamarin.Forms;

namespace DinoLingo
{
    public class ActivityReport
    {
        // *******************
        static readonly double REPORT_STEP_NO_CONNECTION = 30;
        static readonly double REPORT_STEP_ERROR_IN_RESPONSE = 20;

        // *******************

        string act_daily_id = "";
        Object act_daily_id_Lock = new Object();
        public string Act_daily_id
        {
            get
            {
                lock (act_daily_id_Lock)
                {
                    return act_daily_id;
                }
            }
            set
            {
                lock (act_daily_id_Lock)
                {
                    act_daily_id = value;
                }
            }
        }

        bool stop = false;
        Object StopLock = new Object();
        public bool Stop
        {
            get
            {
                lock (StopLock)
                {
                    return stop;
                }
            }
            set
            {
                lock (StopLock)
                {
                    stop = value;
                }
            }
        }

        bool isEnabled = false;
        Object IsEnabledLock = new Object();
        public bool IsEnabled
        {
            get
            {
                lock (IsEnabledLock)
                {
                    return isEnabled;
                }
            }
            set
            {
                lock (IsEnabledLock)
                {
                    isEnabled = value;
                }
            }
        }

        bool isCompleted = false;
        Object IsCompletedLock = new Object();
        public bool IsCompleted
        {
            get
            {
                lock (IsCompletedLock)
                {
                    return isCompleted;
                }
            }
            set
            {
                lock (IsCompletedLock)
                {
                    isCompleted = value;
                }
            }
        }

        public Action OnCompleted;

        ActivityTimeReport timeReport;
        public enum ACT_TYPE { VIDEO, GAME, BOOK };
        static readonly string[] TYPES = new string[] {"video", "game", "book"};
        ACT_TYPE act_type;
        string post_id;

        public ActivityReport(ACT_TYPE act_type, string post_id)
        {
            this.act_type = act_type;
            this.post_id = post_id;
            timeReport = new ActivityTimeReport(this);
        }

        public bool Start() {
            if (Stop) return false;

            if (UserHelper.Login != null && !UserHelper.IsFree && !string.IsNullOrEmpty(UserHelper.Login.user_id))
            {
                // do time report 
                Debug.WriteLine("ActivityReport -> Start");                

                if (CrossConnectivity.Current.IsConnected)
                {
                    GetActivityId();
                }
                else
                {
                    // retry 
                    Debug.WriteLine("ActivityReport -> Start -> no conecction - can't MakeRequest");
                    Device.StartTimer(TimeSpan.FromSeconds(REPORT_STEP_NO_CONNECTION), Start);
                }
            }
            
            return false;
        }



        async void GetActivityId() {
            Debug.WriteLine("ActivityReport -> GetActivityId");
            // if timereport started - stop it & create new
            if (timeReport.IsStarted) {
                timeReport.Stop = true;
                timeReport = new ActivityTimeReport(this);
            }

            string post_id_;
            if (act_type == ACT_TYPE.BOOK)
            {
                int BOOK_SIGN_ID_COEF = 1000000;
                string book_sign_id = (int.Parse(LANGUAGES.CAT_INFO[UserHelper.Lang_cat].Id) * BOOK_SIGN_ID_COEF + int.Parse(post_id)).ToString();
                post_id_ = book_sign_id;
            }
            else
            {
                post_id_ = post_id;
            }

            if (DownloadHelper.DownloadHelper.CheckInternetConnectionProgressive())
            {
                string postData = $"user_id={UserHelper.Login.user_id}";
                postData += $"&post_id={post_id_}";
                postData += $"&act_daily_id={""}";
                postData += $"&progress={0}";
                postData += $"&lesson_type={TYPES[(int)act_type]}";

                ActivityMainResponse activityMainResponse = await ServerApi.PostRequestProgressive<ActivityMainResponse>(ServerApi.ACTIVITY_MAIN_URL, postData, null);
                Debug.WriteLine("first response = " + JsonConvert.SerializeObject(activityMainResponse));

                if (activityMainResponse != null && activityMainResponse.result != null && activityMainResponse.error == null && !string.IsNullOrEmpty(activityMainResponse.result.act_daily_id))
                {
                    // we have new act_daily_id here, proceed...
                    Act_daily_id = activityMainResponse.result.act_daily_id;
                    // we can start timer report here
                    timeReport.Start();

                    // and enable regular report
                    IsEnabled = true;
                }
                else
                {
                    // we some error, retry get act_id
                    Debug.WriteLine("ActivityReport -> GetActivityId -> some error in response");

                    Analytics.SendResultsRegular("ActivityReport", activityMainResponse, activityMainResponse?.error, ServerApi.ACTIVITY_MAIN_URL, postData);

                    Device.StartTimer(TimeSpan.FromSeconds(REPORT_STEP_ERROR_IN_RESPONSE), Start);
                }
            }
            else
            {
                Device.StartTimer(TimeSpan.FromSeconds(REPORT_STEP_ERROR_IN_RESPONSE), Start);
            }
           
        }

        public async void ReportProgress(double progress) {
            if (Stop || !IsEnabled) return;

            Debug.WriteLine("ActivityReport -> ReportProgress, progress = " + progress);
            if (CrossConnectivity.Current.IsConnected && DownloadHelper.DownloadHelper.CheckInternetConnectionProgressive())
            {
                string postData = $"user_id={UserHelper.Login.user_id}";
                postData += $"&act_daily_id={Act_daily_id}";
                postData += $"&progress={progress}";

                if (act_type < ACT_TYPE.BOOK ) { // video or game
                    postData += $"&post_id={post_id}";
                    postData += $"&lesson_type={TYPES[(int)act_type]}";
                    Debug.WriteLine("postData = " + postData);
                    ActivityMainResponse activityMainResponse = await ServerApi.PostRequestProgressive<ActivityMainResponse>(ServerApi.ACTIVITY_MAIN_URL, postData, null);
                    Debug.WriteLine("ReportProgress (VIDEO OR GAME) response = " + JsonConvert.SerializeObject(activityMainResponse));

                    if (activityMainResponse != null && activityMainResponse.result != null && activityMainResponse.error == null
                        && !string.IsNullOrEmpty(activityMainResponse.result.act_daily_id))
                    {
                        // we have the good act_daily_id, proceed...
                        Act_daily_id = activityMainResponse.result.act_daily_id;
                        if (activityMainResponse.result.lesson_completed)
                        {
                            Debug.WriteLine("ActivityReport -> ReportProgress -> post is comleted, progress = " + progress);
                            IsCompleted = true;
                            OnCompleted?.Invoke();
                        }
                    }
                    else
                    {
                        // we some error -> try to get new act_id
                        Debug.WriteLine("ActivityReport -> ReportProgress -> some error in response, try to get new act_id");
                        IsEnabled = false;
                        Device.StartTimer(TimeSpan.FromSeconds(REPORT_STEP_ERROR_IN_RESPONSE), Start);

                        Analytics.SendResultsRegular("ActivityReport", activityMainResponse, activityMainResponse?.error, ServerApi.ACTIVITY_MAIN_URL, postData);
                    }
                }
                else { // it's a book
                    int BOOK_SIGN_ID_COEF = 1000000;
                    string book_sign_id = (int.Parse(LANGUAGES.CAT_INFO[UserHelper.Lang_cat].Id) * BOOK_SIGN_ID_COEF + int.Parse(post_id)).ToString();
                    postData += $"&book_id={book_sign_id}";
                    Debug.WriteLine("postData = " + postData);
                    ActivityBookResponse activityBookResponse = await ServerApi.PostRequestProgressive<ActivityBookResponse>(ServerApi.ACTIVITY_BOOK_URL, postData, null);
                    Debug.WriteLine("ReportProgress (BOOK) response = " + JsonConvert.SerializeObject(activityBookResponse));

                    if (activityBookResponse != null && activityBookResponse.result != null && activityBookResponse.error == null
                        && !string.IsNullOrEmpty(activityBookResponse.result.act_daily_id))
                    {
                        // we have the good act_daily_id, proceed...
                        Act_daily_id = activityBookResponse.result.act_daily_id;
                    }
                    else
                    {
                        // we some error -> try to get new act_id
                        Debug.WriteLine("ActivityReport -> ReportProgress -> some error in response, try to get new act_id");
                        IsEnabled = false;
                        Device.StartTimer(TimeSpan.FromSeconds(REPORT_STEP_ERROR_IN_RESPONSE), Start);

                        Analytics.SendResultsRegular("ActivityReport", activityBookResponse, activityBookResponse?.error, ServerApi.ACTIVITY_MAIN_URL, postData);
                    }
                }


            }
            else
            {
                // retry 
                Debug.WriteLine("ActivityReport -> ReportProgress -> no conecction - can't ReportProgress");
            }
        } 

        public void OnSleep()
        {
            timeReport.OnSleep();
        }

        public void OnResume()
        {
            timeReport.OnResume();
        }

        public void OnDisappearing()
        {
            timeReport.OnDisappearing();
            Stop = true;
        } 
    }
}
