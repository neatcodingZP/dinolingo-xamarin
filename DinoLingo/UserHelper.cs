using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DinoLingo
{
    public static class UserHelper
    {
        static Login_Response.Login login = null;
        static Object LoginLock = new Object();
        public static Login_Response.Login Login
        {
            get
            {
                lock (LoginLock)
                {
                    return login;
                }
            }
            set
            {
                lock (LoginLock)
                {
                    login = value;
                }
            }
        }

        static string lang_cat = string.Empty;
        static Object LangCatLock = new Object();
        public static string Lang_cat
        {
            get
            {
                lock (LangCatLock)
                {
                    return lang_cat;
                }
            }
            set
            {
                lock (LangCatLock)
                {
                    lang_cat = value;
                }

            }
        }

        static string language = string.Empty;
        static Object LanguageLock = new Object();
        public static string Language
        {
            get
            {
                lock (LanguageLock)
                {
                    return language;
                }
            }
            set
            {
                lock (LanguageLock)
                {
                    language = value;
                }

            }
        }

        static bool isFree = true;
        static Object IsFreeLock = new Object();
        public static bool IsFree
        {
            get
            {
                lock (IsFreeLock)
                {
                    return isFree;
                }
            }
            set
            {
                lock (IsFreeLock)
                {
                    isFree = value;
                }
            }
        }



        public static Login_Response CreateFreeUser(Login_Response.Login userInfo = null)
        {
            IsFree = true;
            Login_Response login_Response;
            if (userInfo == null)
                login_Response = new Login_Response
                {
                    result = new Login_Response.Login
                    {
                        user_id = "",
                        display_name = Translate.GetString("default_user_name"),

                    },
                    error = null
                };
            else
            {
                login_Response = new Login_Response
                {
                    result = new Login_Response.Login
                    {
                        user_id = userInfo.user_id,
                        display_name = userInfo.display_name,
                        user_login = userInfo.user_login,
                        user_email = userInfo.user_email
                    },
                    error = null
                };
            }

            login_Response.result.products = null;
            login_Response.result.IAPproducts = null;

            Login = login_Response.result;
            return login_Response;
        }

        public static void SetIsFree() {
            if (Login == null) {
                IsFree = true;
                return;
            }
            if (string.IsNullOrEmpty(Lang_cat))
            {
                IsFree = true;
                return;
            }
            int totalProducts = 0;
            if (Login.products != null) totalProducts += Login.products.Length;
            if (Login.IAPproducts != null) totalProducts += Login.IAPproducts.Length;
            if (totalProducts == 0)
            {
                IsFree = true;
                return;
            }

            if (Login.products != null)
                foreach (Login_Response.Product p in Login.products) {
                    if (p.cat_id.ToString() == Lang_cat) {
                    IsFree = false;
                    return;
                    }
                }

            if (Login.IAPproducts != null)
                foreach (Login_Response.Product p in Login.IAPproducts)
                {
                if (p.cat_id.ToString() == Lang_cat) {
                    IsFree = false;
                    return;
                    }
                }

            IsFree = true;
        }

        public static Login_Response.Product GetSingleProduct(Login_Response login_Response)
        {
            Debug.WriteLine("UserHelper -> GetSingleProduct ->");          
            
            if (login_Response.result.products != null && login_Response.result.products.Length > 0) return login_Response.result.products[0];
            if (login_Response.result.IAPproducts != null && login_Response.result.IAPproducts.Length > 0) return login_Response.result.IAPproducts[0];

            return new Login_Response.Product();
        }

        public static int TotalProducts()
        {
            Debug.WriteLine("UserHelper -> TotalProducts ->");
            int totalProducts = 0;
            if (Login.products != null) totalProducts += Login.products.Length;
            if (Login.IAPproducts != null) totalProducts += Login.IAPproducts.Length;

            return totalProducts;
        }

        public static bool HaveProductId (string product_id)
        {
            if (Login.products != null)
                foreach (Login_Response.Product p in Login.products)
                {
                    if (p.product_id == product_id)
                    {
                        return true;
                    }
                }

            if (Login.IAPproducts != null)
                foreach (Login_Response.Product p in Login.IAPproducts)
                {
                    if (p.product_id == product_id)
                    {
                        return true;
                    }
                }
            return false;
        }
    }
}
