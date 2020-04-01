using Plugin.GoogleAnalytics;
using System;
using System.Collections.Generic;
using System.Text;

namespace DinoLingo
{
    public static class Analytics
    {
        public static void SendResultsRegular (string categoty_class, Object response, Login_Response.Error error_, string url, string post_data, int code = 0)
        {
            string action_info = $"url= {url}, post_data= {post_data}";

            if (response == null)
            {
                
                int code_ = code == 0 ? 100 : code;
                GoogleAnalytics.Current.Tracker.SendEvent(categoty_class, action_info, "error: NULL response", code_);               
            }
            else if (error_ != null)
            {                
                string error = $"error_code= {error_.code}, error_message= {error_.message}";
                int code_ = code == 0 ? 101 : code;

                if (code == 0) int.TryParse(error_.code, out code_);
                GoogleAnalytics.Current.Tracker.SendEvent(categoty_class, action_info, "error: " + error, code_);
            }
            else
            {
                int code_ = code == 0 ? 102 : code;
                GoogleAnalytics.Current.Tracker.SendEvent(categoty_class, action_info, "error: NULL result, NULL error", code_);
            }           

        }
    }
}
