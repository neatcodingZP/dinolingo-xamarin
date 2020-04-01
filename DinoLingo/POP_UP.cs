using System;
using System.Collections.Generic;
using System.Text;

namespace DinoLingo
{
    public static class POP_UP
    {
        public static string OOPS;
        public static string OK;
        public static string CANCEL;

        public static string INFO;
        public static string ATTENTION;
        public static string NO_CONNECTION;
        public static string SOME_ERROR_IN_RESPONSE;

        public static string YES;
        public static string NO;


        static int ERROR_PREFIX = 0;
        public static string GetCode(Login_Response.Error error, int code)
        {
            string error_code = error?.code;
            if (string.IsNullOrEmpty(error_code))
            {
                error_code = "0";                
            }

            return "\n(" + error_code + "." + (ERROR_PREFIX + code).ToString("D3") + ")"; 
        }

        public static void SetStrings()
        {
            OOPS = Translate.GetString("popup_oops");
            OK = Translate.GetString("popup_ok");
            CANCEL = Translate.GetString("popup_cancel");
            INFO = Translate.GetString("popup_info");
            ATTENTION = Translate.GetString("popup_attention");
            NO_CONNECTION = Translate.GetString("popup_no_connection");
            SOME_ERROR_IN_RESPONSE = Translate.GetString("popup_something_went_wrong");

            YES = Translate.GetString("popup_yes");
            NO = Translate.GetString("popup_no");
        }
    }
}
