using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DinoLingo
{
    public class AsyncMessages
    {
        static DateTime lastNoConnectionAlert = DateTime.MinValue;
        static int noConnectionAlertCounter = 0;

        static DateTime lastDisplayAlert = DateTime.MinValue;
        static int displayAlertCounter = 0;

        static double NO_CONNECTION_TIMEOUT = 60 * 3;
        static int NO_CONNECTION_COUNTER_MAX = 4;

        static double DISPLAYALERT_TIMEOUT =  3;
        static int DISPLAYALERT_COUNTER_MAX = 3;

        public static void DisplayAlert (string s1, string s2, string s3)
        {
            Device.BeginInvokeOnMainThread(async() => {
                await App.Current.MainPage.DisplayAlert(s1,s2,s3);
            });
        }

        public static bool CheckConnectionTimeout()
        {
            DateTime current = DateTime.Now;
            TimeSpan timePassed = current - lastNoConnectionAlert;
            noConnectionAlertCounter++;

            if (timePassed.TotalSeconds > NO_CONNECTION_TIMEOUT || noConnectionAlertCounter >= NO_CONNECTION_COUNTER_MAX)
            {
                lastNoConnectionAlert = current;
                noConnectionAlertCounter = 0;
                return true;
            }
            else
            {
                return false;
            }            
        }

        public static bool CheckDisplayAlertTimeout()
        {
            DateTime current = DateTime.Now;
            TimeSpan timePassed = current - lastDisplayAlert;
            TimeSpan timePassedFromNoConnection = current - lastNoConnectionAlert;
            displayAlertCounter++;

            if ( (timePassed.TotalSeconds > DISPLAYALERT_TIMEOUT && timePassedFromNoConnection.TotalSeconds > DISPLAYALERT_TIMEOUT) 
                    || displayAlertCounter >= DISPLAYALERT_COUNTER_MAX)
            {
                lastDisplayAlert = current;
                displayAlertCounter = 0;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
