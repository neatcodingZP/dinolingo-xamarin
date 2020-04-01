using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DinoLingo
{
    public static class RegisterIAP
    {
        static readonly TimeSpan EXPIRED_TIME = TimeSpan.FromDays(7);

        static DateTime lastRestored = DateTime.MinValue;
        public static bool IsExpired
        {
            get
            {
                if ((DateTime.Now - lastRestored).CompareTo(EXPIRED_TIME) > 0)
                {
                    Debug.WriteLine("RegisterIAP -> IsExpired = true");
                    return true;
                }
                Debug.WriteLine("RegisterIAP -> IsExpired = false");
                return false;
            }
        }

        public static async void RegisterOnePurchase(IAPData iapData)
        { 
            string postData = $"user_id={UserHelper.Login.user_id}";
            postData += $"&data={JsonConvert.SerializeObject(new IAPData[] {iapData})}";
            postData += $"&action={"insert"}";

            Debug.WriteLine("RegisterIAP -> RegisterOnePurchase ...");
            RegisterIAPsResponse registerIAPsResponse = await ServerApi.PostRequestProgressive<RegisterIAPsResponse>(ServerApi.REGISTER_IAPS, postData, null);
            if (registerIAPsResponse != null)
            {
                Debug.WriteLine("RegisterIAP -> RegisterOnePurchase -> registerIAPsResponse = " + JsonConvert.SerializeObject(registerIAPsResponse));
                
                if (registerIAPsResponse.error == null)
                {                    
                    if (registerIAPsResponse.result == "")
                    {
                        Analytics.SendResultsRegular("RegisterIAP", registerIAPsResponse, registerIAPsResponse?.error, ServerApi.REGISTER_IAPS, postData);
                    }
                    else
                    {

                    }
                    Debug.WriteLine("RegisterIAP -> RegisterOnePurchase -> registerIAPsResponse.result= " + registerIAPsResponse.result);
                    
                }
                else
                {
                    Analytics.SendResultsRegular("RegisterIAP", registerIAPsResponse, registerIAPsResponse?.error, ServerApi.REGISTER_IAPS, postData);
                    Debug.WriteLine("RegisterIAP -> RegisterOnePurchase -> registerIAPsResponse.error.message= " + registerIAPsResponse.error.message);
                }
            }
            else
            { // some error in response
                Analytics.SendResultsRegular("RegisterIAP", registerIAPsResponse, registerIAPsResponse?.error, ServerApi.REGISTER_IAPS, postData);
                Debug.WriteLine("RegisterIAP -> RegisterOnePurchase -> NULL RESPONSE");
                lastRestored = DateTime.MinValue;
            }
        }

        public static async void UpdateAllIAPs()
        {
            // create all IAPs
            Debug.WriteLine("RegisterIAP -> UpdateAllIAPs ...");

            IAPData[] iapData = new IAPData[UserHelper.Login.IAPproducts.Length];
            for (int i = 0; i < iapData.Length; i++)
            {
                string productId = Purchaser.GetPurchaseIdForProductId(UserHelper.Login.IAPproducts[i].product_id);
                if (string.IsNullOrEmpty(productId))
                {
                    Debug.WriteLine("RegisterIAP -> UpdateAllIAPs -> productId == null, break !");
                    return;
                }
                string[] term = productId.Split('_');

                iapData[i] = new IAPData {
                    product_id = term[term.Length - 1],
                    period = term[term.Length-2],
                    created_at = UserHelper.Login.IAPproducts[i].trans_date,
                };
            }

            string postData = $"user_id={UserHelper.Login.user_id}";
            postData += $"&data={JsonConvert.SerializeObject(iapData)}";
            postData += $"&action={"update"}";

            Debug.WriteLine("RegisterIAP -> UpdateAllIAPs ...");
            RegisterIAPsResponse registerIAPsResponse = await ServerApi.PostRequestProgressive<RegisterIAPsResponse>(ServerApi.REGISTER_IAPS, postData, null);
            if (registerIAPsResponse != null)
            {
                Debug.WriteLine("RegisterIAP -> UpdateAllIAPs -> registerIAPsResponse = " + JsonConvert.SerializeObject(registerIAPsResponse));

                if (registerIAPsResponse.error == null)
                {
                    if (registerIAPsResponse.result == "")
                    {
                        Analytics.SendResultsRegular("RegisterIAP", registerIAPsResponse, registerIAPsResponse?.error, ServerApi.REGISTER_IAPS, postData);
                    }
                    else
                    {

                    }
                    Debug.WriteLine("RegisterIAP -> UpdateAllIAPs -> registerIAPsResponse.result= " + registerIAPsResponse.result);
                    lastRestored = DateTime.Now;
                }
                else
                {
                    Analytics.SendResultsRegular("RegisterIAP", registerIAPsResponse, registerIAPsResponse?.error, ServerApi.REGISTER_IAPS, postData);
                    Debug.WriteLine("RegisterIAP -> UpdateAllIAPs -> registerIAPsResponse.error.message= " + registerIAPsResponse.error.message);
                }
            }
            else
            { // some error in response
                Analytics.SendResultsRegular("RegisterIAP", registerIAPsResponse, registerIAPsResponse?.error, ServerApi.REGISTER_IAPS, postData);
                Debug.WriteLine("RegisterIAP -> UpdateAllIAPs -> NULL RESPONSE");
                lastRestored = DateTime.MinValue;
            }
        }

        public static async void ClearAllIAPs()
        {
            string postData = $"user_id={UserHelper.Login.user_id}";
            postData += $"&data={JsonConvert.SerializeObject(new IAPData[0])}";
            postData += $"&action={"clear"}";

            Debug.WriteLine("RegisterIAP -> ClearAllIAPs ...");
            RegisterIAPsResponse registerIAPsResponse = await ServerApi.PostRequestProgressive<RegisterIAPsResponse>(ServerApi.REGISTER_IAPS, postData, null);
            if (registerIAPsResponse != null)
            {
                Debug.WriteLine("RegisterIAP -> ClearAllIAPs -> registerIAPsResponse = " + JsonConvert.SerializeObject(registerIAPsResponse));
                
                if (registerIAPsResponse.error == null)
                {                    
                    if (registerIAPsResponse.result == "")
                    {
                        Analytics.SendResultsRegular("RegisterIAP", registerIAPsResponse, registerIAPsResponse?.error, ServerApi.REGISTER_IAPS, postData);
                    }
                    else
                    {

                    }
                    Debug.WriteLine("RegisterIAP -> ClearAllIAPs -> registerIAPsResponse.result= " + registerIAPsResponse.result);
                    lastRestored = DateTime.Now;
                }
                else
                {
                    Analytics.SendResultsRegular("RegisterIAP", registerIAPsResponse, registerIAPsResponse?.error, ServerApi.REGISTER_IAPS, postData);
                    Debug.WriteLine("RegisterIAP -> ClearAllIAPs -> registerIAPsResponse.error.message= " + registerIAPsResponse.error.message);
                }
            }
            else
            { // some error in response
                Analytics.SendResultsRegular("RegisterIAP", registerIAPsResponse, registerIAPsResponse?.error, ServerApi.REGISTER_IAPS, postData);
                Debug.WriteLine("RegisterIAP -> ClearAllIAPs -> NULL RESPONSE");
                lastRestored = DateTime.MinValue;
            }
        }
    }
}
