using System;
using System.Diagnostics;
using Newtonsoft.Json;
using Plugin.Connectivity;
using Plugin.GoogleAnalytics;
using Xamarin.Forms;

namespace DinoLingo
{
    public class ActivityTimeReport
    {
        public ActivityReport activityReport { get; set; }

        static readonly double TIME_REPORT_STEP_REGULAR = 30; // seconds
        static readonly double TIME_REPORT_STEP_FIRST = 20; // seconds
        static readonly double TIME_REPORT_STEP_NO_CONNECTION = 20;
        static readonly double TIME_REPORT_STEP_ERROR_IN_RESPONSE = 20; 

        // ===========
        DateTime prevTime, sleepTime;

        double sleepedSeconds = 0;
        Object SleepedSecondsLock = new Object();
        public double SleepedSeconds
        {
            get
            {
                lock (SleepedSecondsLock)
                {
                    return sleepedSeconds;
                }
            }
            set
            {
                lock (SleepedSecondsLock)
                {
                    sleepedSeconds = value;
                }
            }
        }

        int successReports = 0;
        bool stop = false;
        Object StopLock = new Object();
        public  bool Stop
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

        public bool IsStarted { get; set; }

        public ActivityTimeReport(ActivityReport parent)
        {
            activityReport = parent;
            IsStarted = false;
        }

        public void Start() {
            if (UserHelper.Login != null && !UserHelper.IsFree && !string.IsNullOrEmpty(UserHelper.Login.user_id))
            {
                IsStarted = true;
                prevTime = DateTime.Now;
                DoTimerStep();
            }            
        }

        bool DoTimerStep() {
            if (Stop) return false;

            // do time report 
            Debug.WriteLine("ActivityTimeReport -> DoTimerStep");
            
            if (CrossConnectivity.Current.IsConnected)
            {
                MakeTimeRequest();
            }
            else 
            {
                // retry 
                Debug.WriteLine("ActivityTimeReport -> no conecction - can't MakeTimeRequest");
                Device.StartTimer(TimeSpan.FromSeconds(TIME_REPORT_STEP_NO_CONNECTION), DoTimerStep);
            }
            return false;
        }

        async void MakeTimeRequest() {
            if (DownloadHelper.DownloadHelper.CheckInternetConnectionProgressive())
            {
                TimeSpan deltaTime = DateTime.Now - prevTime;
                double dTime = deltaTime.TotalSeconds - sleepedSeconds;
                string postData = $"user_id={UserHelper.Login.user_id}";
                postData += $"&add_time={dTime}";
                postData += $"&act_daily_id={activityReport.Act_daily_id}";
                ActivityTimeResponse activityTimeResponse = await ServerApi.PostRequestProgressive<ActivityTimeResponse>(ServerApi.ACTIVITY_TIME_URL, postData, null);

                if (activityTimeResponse != null && activityTimeResponse.error == null)
                {
                    Debug.WriteLine("regular time response = " + JsonConvert.SerializeObject(activityTimeResponse));
                    successReports++;
                    prevTime = DateTime.Now;
                    sleepedSeconds = 0;
                    Debug.WriteLine($"Activity regular time reported, add_time = {dTime}, act_daily_id = {activityReport.Act_daily_id}");
                    if (successReports < 2)
                    {
                        Device.StartTimer(TimeSpan.FromSeconds(TIME_REPORT_STEP_FIRST), DoTimerStep);
                    }
                    else
                    {
                        Device.StartTimer(TimeSpan.FromSeconds(TIME_REPORT_STEP_REGULAR), DoTimerStep);
                    }
                }
                else
                { // some error in response
                    Device.StartTimer(TimeSpan.FromSeconds(TIME_REPORT_STEP_ERROR_IN_RESPONSE), DoTimerStep);

                    Analytics.SendResultsRegular("ActivityTimeReport", activityTimeResponse, activityTimeResponse?.error, ServerApi.ACTIVITY_TIME_URL, postData);
                }
            }
            else
            {
                Device.StartTimer(TimeSpan.FromSeconds(TIME_REPORT_STEP_ERROR_IN_RESPONSE), DoTimerStep);
            }            
        }

        public void OnSleep() {
            sleepTime = DateTime.Now;
        }

        public void OnResume() {
            TimeSpan deltaSleep = DateTime.Now - sleepTime;

            SleepedSeconds += deltaSleep.TotalSeconds;
        }

        public void OnDisappearing() {
            Stop = true;
        }
    }
}
