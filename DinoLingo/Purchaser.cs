using Newtonsoft.Json;
using Plugin.Connectivity;
using Plugin.GoogleAnalytics;
using Plugin.InAppBilling;
using Plugin.InAppBilling.Abstractions;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DinoLingo
{
    public static class Purchaser
    {
        public static bool IsPurchasing = false;
        public static bool IsRestoring = false;

        public static Action OnPurchased;
        public static Action OnRestored;
        public static string Lang_cat;

        static readonly TimeSpan EXPIRED_TIME = TimeSpan.FromMinutes(60.0);

        static DateTime lastRestored = DateTime.MinValue;

        public static bool IsExpired 
        {
            get
            {
                if ((DateTime.Now - lastRestored).CompareTo(EXPIRED_TIME) > 0)
                {
                    Debug.WriteLine("Purchaser -> IsExpired = true");
                    return true;
                }
                Debug.WriteLine("Purchaser -> IsExpired = false");
                return false;
            }
        }

        public static string GetPurchaseIdForProductId (string product_id)
        {   
            foreach (KeyValuePair<string, LANGUAGES.LangInfo> pair in LANGUAGES.CAT_INFO)
            {
                
                for (int i = 0; i < pair.Value.ProductsIds.Length; i++)
                {
                    string lang = pair.Value.Name;
                    if (product_id == pair.Value.ProductsIds[i])
                    {
                        return lang.ToLower().Replace("-", "_") + "_" + TermIndexToId(i) + "_" + product_id;
                    }                   
                }
            }
            return string.Empty;
        }

        static int TermToProductIndex(string term)
        {
            if (term == "6") return 0;
            if (term == "12") return 1;
            if (term == "1") return 2;
             return -1;
        }

        public static string TermToLenght(int term)
        {
            switch (term)
            {
                case 0: return Translate.GetString("purchaser_term_semi_annual");
                case 1: return Translate.GetString("purchaser_term_annual");
                case 2: return Translate.GetString("purchaser_term_monthly");
                default: return "?";
            }
        }

        public static string Price(int term, string lang_cat)
        {
            if (Device.RuntimePlatform == Device.iOS) {
                switch (term)
                {
                    case 0:
                        if (PriceCat(lang_cat) == 1) return "58.99 $";
                        return "98.99 $";
                    case 1:
                        if (PriceCat(lang_cat) == 1) return "98.99 $";
                        return "159.99 $";
                    case 2:
                        if (PriceCat(lang_cat) == 1) return "$19.99";
                        return "$19.99";
                    default:
                        return "?";
                }
            }
            else
            {
                switch (term)
                {
                    case 0:
                        if (PriceCat(lang_cat) == 1) return "59.99 $";
                        return "99.99 $";
                    case 1:
                        if (PriceCat(lang_cat) == 1) return "99.99 $";
                        return "159.99 $";
                    case 2:
                        if (PriceCat(lang_cat) == 1) return "$19.99";
                        return "$19.99";
                    default:
                        return "?";
                }
            }
            
        }

        public static int PriceCat(string lang_cat)
        {
            switch (lang_cat)
            {
                case "644": // dari
                case "647": // hawaiian
                case "645": // kazakh
                    return 1;
                default: return 0;
            }
        }

        public static string TermIndexToId (int term)
        {
            if (term == 0) return "6";
            if (term == 1) return "12";
            if (term == 2) return "1";
            return string.Empty;
        }
        
/*
        public static string GetProductNameByCatAndId(string lang_cat, string product_id)

        {
            Debug.WriteLine("Purchaser -> GetProductNameByCatAndId - > lang_cat = " + lang_cat);
            if (LANGUAGES.CAT_INFO[lang_cat].ProductsIds == null) return string.Empty;
            if (LANGUAGES.CAT_INFO[lang_cat].ProductsIds.Length > 0 && product_id == LANGUAGES.CAT_INFO[lang_cat].ProductsIds[0])
            { // term 6 months
                return LANGUAGES.CAT_INFO[lang_cat].GetVisibleName() + " for kids 6 months";
            }

            if (LANGUAGES.CAT_INFO[lang_cat].ProductsIds.Length > 1 && product_id == LANGUAGES.CAT_INFO[lang_cat].ProductsIds[1])
            { // term 12 months
                return LANGUAGES.CAT_INFO[lang_cat].GetVisibleName() + " for kids 1 year";
            }

            return string.Empty;
        }
*/
        public static string GetProductNameByCatAndTerm(string lang_cat, int term)

        {
            Debug.WriteLine("Purchaser -> GetProductNameByCatAndId - > lang_cat = " + lang_cat);
            if (LANGUAGES.CAT_INFO[lang_cat].ProductsIds == null) return string.Empty;
            if (LANGUAGES.CAT_INFO[lang_cat].ProductsIds.Length > 0 && term == 0)
            { // term 6 months
                return string.Format(Translate.GetString("purchaser_product_name_6_months"), string.Format(Translate.GetString("header_for_kids"), LANGUAGES.CAT_INFO[lang_cat].VisibleName).FirstLetterToUpperCase());
                   
            }

            if (LANGUAGES.CAT_INFO[lang_cat].ProductsIds.Length > 1 && term == 1)
            { // term 12 months
                return string.Format(Translate.GetString("purchaser_product_name_1_year"), string.Format(Translate.GetString("header_for_kids"), LANGUAGES.CAT_INFO[lang_cat].VisibleName).FirstLetterToUpperCase());
            }

            if (LANGUAGES.CAT_INFO[lang_cat].ProductsIds.Length > 2 && term == 2)
            { // term 1 month
                return string.Format(Translate.GetString("purchaser_product_name_1_month"), string.Format(Translate.GetString("header_for_kids"), LANGUAGES.CAT_INFO[lang_cat].VisibleName).FirstLetterToUpperCase());
            }

            return string.Empty;
        }

        /*
        public static Task<bool> PurchaseAsync(string lang_cat, int term_index)
        {
            return Task.Run(async()=>
            {
                Debug.WriteLine("Purchaser -> PurchaseAsync");
                
                IsPurchasing = true;
                // set id for product
                string lang = LANGUAGES.CAT_INFO[lang_cat].Name;
                if (string.IsNullOrEmpty(lang_cat))
                {
                    Console.WriteLine("Purchaser -> Purchase -> do not have lang_cat for language:" + lang);
                    IsPurchasing = false;
                    return false;
                }
                // check if have products Id
                if (LANGUAGES.CAT_INFO[lang_cat].ProductsIds == null || LANGUAGES.CAT_INFO[lang_cat].ProductsIds.Length == 0)
                {
                    Console.WriteLine("Purchaser -> Purchase -> do not have products for language:" + lang);
                    IsPurchasing = false;
                    return false;
                }

                // check term
                if (term_index < 0 || term_index > 1)
                {
                    Console.WriteLine("Purchaser -> Purchase -> do not have such a term:" + TermIndexToId(term_index));
                    IsPurchasing = false;
                    return false;
                }

                // create product id
                string id = lang.ToLower().Replace("-", "_") + "_" + TermIndexToId(term_index) + "_" + LANGUAGES.CAT_INFO[lang_cat].ProductsIds[term_index];
                Console.WriteLine("Purchaser -> PurchaseAsync -> purchase id = " + id);
                try
                {
                    var purchase = await CrossInAppBilling.Current.PurchaseAsync(id, Plugin.InAppBilling.Abstractions.ItemType.Subscription, "mypayload");


                    if (purchase == null)
                    {
                        await App.Current.MainPage.DisplayAlert(string.Empty, "Did not purchase", POP_UP.OK);
                        IsPurchasing = false;
                        return false;
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert(string.Empty, "Subscribtion purchased!", POP_UP.OK);
                        // if subscribed

                        // add the product to IAP products
                        List<Login_Response.Product> iap_products;
                        if (UserHelper.Login.IAPproducts != null) iap_products = new List<Login_Response.Product>(UserHelper.Login.IAPproducts);
                        else iap_products = new List<Login_Response.Product>();

                        iap_products.Add(new Login_Response.Product
                        {
                            product_id = LANGUAGES.CAT_INFO[lang_cat].ProductsIds[term_index],
                            cat_id = int.Parse(lang_cat),
                        });

                        UserHelper.Login.IAPproducts = new Login_Response.Product[iap_products.Count];
                        for (int i = 0; i < iap_products.Count; i++)
                        {
                            UserHelper.Login.IAPproducts[i] = new Login_Response.Product { product_id = iap_products[i].product_id, cat_id = iap_products[i].cat_id };
                        }

                        await CacheHelper.Add(CacheHelper.LOGIN, UserHelper.Login, TimeSpan.FromSeconds(100));
                        if (App.DEBUG) await App.Current.MainPage.DisplayAlert("Great!", "Subscription " + id + " added to Your products", "Continue");

                        IsPurchasing = false;
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.DisplayAlert("", "Did not purchase", POP_UP.OK);
                    Debug.WriteLine("Purchaser -> did not purchase ->" + ex);
                    Console.WriteLine(ex);

                    IsPurchasing = false;
                    return false;                
                }                
            });
        }
        */

        public static async void Purchase(string lang_cat, int term_index)
        {
            IsPurchasing = true;

            // *** only debug ***
            /*
            Debug.WriteLine("Purchaser -> simulated purchasing -> ok");
            IsPurchasing = false;
            Lang_cat = lang_cat;
            OnPurchased?.Invoke();

            return;
            */
            // *** only debug ***

            // set id for product
            string lang = LANGUAGES.CAT_INFO[lang_cat].Name;
            if (!CrossInAppBilling.IsSupported)
            {
                Debug.WriteLine("Purchaser -> Purchase -> is not supported");
                IsPurchasing = false;
                return;
            }
                

            if (string.IsNullOrEmpty(lang_cat))
            {
                Console.WriteLine("Purchaser -> Purchase -> do not have lang_cat for language:" + lang);
                if (App.DEBUG) await App.Current.MainPage.DisplayAlert("Purchaser", "do not have lang_cat for language:" + lang, "Continue");
                IsPurchasing = false;
                return;
            }
            // check if have products Id
            if (LANGUAGES.CAT_INFO[lang_cat].ProductsIds == null || LANGUAGES.CAT_INFO[lang_cat].ProductsIds.Length == 0)
            {
                if (App.DEBUG) await App.Current.MainPage.DisplayAlert("Purchaser", "do not have products for language:" + lang, "Continue");
                Console.WriteLine("Purchaser -> Purchase -> do not have products for language:" + lang);
                IsPurchasing = false;
                return;
            }

            // check term
            if (term_index < 0 || term_index > 2)
            {
                if (App.DEBUG) await App.Current.MainPage.DisplayAlert("Purchaser", " do not have such a term:" + TermIndexToId(term_index), "Continue");
                Console.WriteLine("Purchaser -> Purchase -> do not have such a term:" + TermIndexToId(term_index));
                IsPurchasing = false;
                return;
            }

            // create product id
            string id = lang.ToLower().Replace("-", "_") + "_" + TermIndexToId(term_index) + "_" + LANGUAGES.CAT_INFO[lang_cat].ProductsIds[term_index];
            Console.WriteLine("Purchaser -> Purchase -> purchase id = " + id);
            try
            {
                var connected = await CrossInAppBilling.Current.ConnectAsync();
                InAppBillingPurchase purchase = null; 
                
                
                if (!connected)
                {
                    //Couldn't connect to billing, could be offline, alert user
                    Debug.WriteLine("Purchaser -> !connected");
                    IsPurchasing = false;
                    return;
                }
                else
                {
                    purchase = await CrossInAppBilling.Current.PurchaseAsync(id, Plugin.InAppBilling.Abstractions.ItemType.Subscription, "mypayload");                    
                }


                

                if (purchase == null)
                {
                    Debug.WriteLine("Purchaser -> Purchase -> Did not purchase");
                    await App.Current.MainPage.DisplayAlert(string.Empty, "Did not purchase", POP_UP.OK);                   
                }
                else 
                {
                    

                    Debug.WriteLine("Purchaser -> Purchase -> Subscribtion purchased");
                    //await App.Current.MainPage.DisplayAlert(string.Empty, "Subscribtion purchased!", POP_UP.OK);
                    Debug.WriteLine("Purchaser -> Purchase -> Subscribtion purchased -> details go here ->");
                    // if subscribed
                    Debug.WriteLine("Purchaser -> Purchase -> Subscribtion purchased -> details go here ->");
                   Debug.WriteLine($"Purchaser -> Purchase -> Subscribtion purchased ->purchase.ConsumptionState = {purchase.ConsumptionState}, purchase.State = {purchase.State}, purchase.TransactionDateUtc = {purchase.TransactionDateUtc}");

                    // add the product to IAP products
                    List<Login_Response.Product> iap_products;
                    if (UserHelper.Login.IAPproducts != null) iap_products = new List<Login_Response.Product>(UserHelper.Login.IAPproducts);
                    else iap_products = new List<Login_Response.Product>();

                    iap_products.Add(new Login_Response.Product {
                        product_id = LANGUAGES.CAT_INFO[lang_cat].ProductsIds[term_index],
                        cat_id = int.Parse(lang_cat),
                        trans_date = purchase.TransactionDateUtc.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                    });

                    
                    

                    Debug.WriteLine("Purchaser -> Purchase -> DateTime.Now.ToString= " + iap_products[iap_products.Count -1].trans_date);

                    UserHelper.Login.IAPproducts = new Login_Response.Product[iap_products.Count];
                    for (int i = 0; i < iap_products.Count; i++)
                    {
                        UserHelper.Login.IAPproducts[i] = new Login_Response.Product { product_id = iap_products[i].product_id, cat_id = iap_products[i].cat_id, trans_date = iap_products[i].trans_date };
                    }

                    await CacheHelper.Add(CacheHelper.LOGIN, UserHelper.Login, TimeSpan.FromSeconds(100));
                    //if (App.DEBUG) await App.Current.MainPage.DisplayAlert("Great!", "Subscription " + id + " added to Your products", "Continue");
                    Lang_cat = lang_cat;
                    OnPurchased?.Invoke();


                    
                    // send data to server ! 
                    if (!string.IsNullOrEmpty(UserHelper.Login.user_id))
                    RegisterIAP.RegisterOnePurchase(new IAPData { product_id = LANGUAGES.CAT_INFO[lang_cat].ProductsIds[term_index], period = TermIndexToId(term_index), created_at = UserHelper.Login.IAPproducts[UserHelper.Login.IAPproducts.Length-1].trans_date });
                                        
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("", "Did not purchase", POP_UP.OK);
                Debug.WriteLine("Purchaser -> did not purchase ->" + ex);
                               
            }
            finally
            {
                //Disconnect, it is okay if we never connected
                await CrossInAppBilling.Current.DisconnectAsync();
            }

            IsPurchasing = false;
        }

        public static Task<bool> Restore()
        {
            // we can Restore, only if have connection -> so we must check it before perform Restore()
            // if we failed to restore - we must delete All the IAP products
            //
            lastRestored = DateTime.Now;
            Debug.WriteLine("Purchaser ->");
            return Task.Run(async () =>
            {
               
                IsRestoring = true;
                List<Login_Response.Product> IAPproducts = new List<Login_Response.Product>();

                if (!CrossInAppBilling.IsSupported)
                {
                    Debug.WriteLine("Purchaser -> Restore -> is not supported");
                    IsRestoring = false;
                    return false;
                }

                Debug.WriteLine("Purchaser -> Restore -> try");
                try
                {
                    var connected = await CrossInAppBilling.Current.ConnectAsync();
                    IEnumerable<InAppBillingPurchase> purchases = null;
                    //SKPaymentTransaction[] transactions = null;
                    if (!connected)
                    {
                        //Couldn't connect to billing, could be offline, alert user
                        Debug.WriteLine("Purchaser -> restore -> !connected");
                        IsRestoring = false;
                        return false;
                    }
                    else
                    {
                        purchases = await CrossInAppBilling.Current.GetPurchasesAsync(Plugin.InAppBilling.Abstractions.ItemType.Subscription);
                    }

                    Debug.WriteLine("Purchaser ->  var purchases = await CrossInAppBilling.Current.GetPurchasesAsync");                   

                    Debug.WriteLine("Purchaser -> if (purchases == null)");
                    if (purchases == null)
                    {
                        Debug.WriteLine("Purchaser -> Did not restore");                        
                    }
                    else
                    {
                        Debug.WriteLine("Purchaser -> restored OK, total products:" + purchases);
                        string purchasesInfo = "Purchases: ";
                        string action_info = (UserHelper.Login != null) ? $"user_id = {UserHelper.Login.user_id}, user_login = {UserHelper.Login.user_login}, user_email = {UserHelper.Login.user_email}" : "user_id = NULL";

                        foreach (InAppBillingPurchase purchase in purchases)
                        {                            
                            Debug.WriteLine($"Purchaser -> restored -> purchase: ProductId= {purchase.ProductId}, PurchaseToken= {purchase.PurchaseToken}, Id= {purchase.Id}, State= {purchase.State}, purchase.AutoRenewing= {purchase.AutoRenewing}, purchase.ConsumptionState= {purchase.ConsumptionState}, purchase.Payload= {purchase.Payload},TransactionDateUtc= {purchase.TransactionDateUtc}");
                            purchasesInfo += $"PrId={purchase.ProductId}, State={purchase.State}, TDate={purchase.TransactionDateUtc.ToString("yyyy - MM - dd HH: mm: ss", CultureInfo.InvariantCulture)}, AR={purchase.AutoRenewing}, CS={purchase.ConsumptionState} * ";
                        }
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            Debug.WriteLine("Purchaser -> RESTORED_PURCHASES, purchasesInfo= " + purchasesInfo);
                            GoogleAnalytics.Current.Tracker.SendEvent("RESTORED_PURCHASES", purchasesInfo, action_info);
                        }); 
                        

                        // check the restored purchases
                        string purchasesInfo2 = "Purchases: ";
                        List<InAppBillingPurchase> validPurchases = GetValidPurchases(purchases);
                        Debug.WriteLine($"Purchaser -> restored -> validPurchases.Count = " + validPurchases.Count);
                        foreach (InAppBillingPurchase purchase in validPurchases)
                        {
                            Debug.WriteLine($"Purchaser -> restored -> validPurchases: ProductId= {purchase.ProductId}, PurchaseToken= {purchase.PurchaseToken}, Id= {purchase.Id}, State= {purchase.State}, purchase.AutoRenewing= {purchase.AutoRenewing}, purchase.ConsumptionState= {purchase.ConsumptionState}, purchase.Payload= {purchase.Payload},TransactionDateUtc= {purchase.TransactionDateUtc}");
                            purchasesInfo2 += $"PrId={purchase.ProductId}, State={purchase.State}, TDate={purchase.TransactionDateUtc.ToString("yyyy - MM - dd HH: mm: ss", CultureInfo.InvariantCulture)}, AR={purchase.AutoRenewing}, CS={purchase.ConsumptionState} * ";
                        }

                        Device.BeginInvokeOnMainThread(() =>
                        {
                            Debug.WriteLine("Purchaser -> VALID_PURCHASES, purchasesInfo= " + purchasesInfo2);
                            GoogleAnalytics.Current.Tracker.SendEvent("VALID_PURCHASES", purchasesInfo2, action_info);
                        });
                        

                        // check all users IAP products
                        string purchasesInfo3 = "Added IAPs: ";
                        foreach (InAppBillingPurchase purchase in validPurchases)
                        {
                            string product_id = purchase.ProductId;
                           
                            Debug.WriteLine($"Purchaser -> restored -> purchase: ProductId= {purchase.ProductId}, PurchaseToken= {purchase.PurchaseToken}, Id= {purchase.Id}, State= {purchase.State}, purchase.AutoRenewing= {purchase.AutoRenewing}, purchase.ConsumptionState= {purchase.ConsumptionState}, purchase.Payload= {purchase.Payload},TransactionDateUtc= {purchase.TransactionDateUtc}");

                            // analise product_id
                            string[] product_id_substrings = product_id.Split('_');
                            string main_prod_id = product_id_substrings[product_id_substrings.Length - 1];
                            string lang_cat = string.Empty;
                            bool gotLangCat = false;
                            foreach (KeyValuePair<string, LANGUAGES.LangInfo> keyValuePair in LANGUAGES.CAT_INFO)
                            {
                                for (int i = 0; i < keyValuePair.Value.ProductsIds.Length; i++)
                                {
                                    if (main_prod_id == keyValuePair.Value.ProductsIds[i])
                                    {
                                        lang_cat = keyValuePair.Key;
                                        gotLangCat = true;
                                        break;
                                    }
                                }
                                if (gotLangCat) break;
                            }

                            // added iap products                            

                            if (!string.IsNullOrEmpty(lang_cat) && (purchase.State == PurchaseState.Purchased || purchase.State == PurchaseState.Restored || purchase.State == PurchaseState.PaymentPending)) // MAy be add check State ???
                            {
                                IAPproducts.Add(new Login_Response.Product
                                {
                                    cat_id = int.Parse(lang_cat),
                                    product_id = main_prod_id,
                                    trans_date = purchase.TransactionDateUtc.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                                });
                                Debug.WriteLine("Purchaser -> restored -> main_prod_id = " + main_prod_id + "added to IAPproducts");
                                purchasesInfo3 += $"cat_id = {int.Parse(lang_cat)}, product_id = {main_prod_id}, trans_date = {purchase.TransactionDateUtc.ToString("yyyy - MM - dd HH: mm: ss", CultureInfo.InvariantCulture)} ***";
                            }
                            
                        }
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            GoogleAnalytics.Current.Tracker.SendEvent("VALID_PURCHASES_FINAL", purchasesInfo3, action_info);
                        });
                        
                    }

                    Debug.WriteLine("Purchaser -> restored total:" + IAPproducts.Count);

                    // update user info
                    UserHelper.Login.IAPproducts = IAPproducts.ToArray();
                    Debug.WriteLine("Purchaser -> -> update login cache");
                    await CacheHelper.Add(CacheHelper.LOGIN, UserHelper.Login, TimeSpan.FromSeconds(100));

                    // send data to server !
                    if (RegisterIAP.IsExpired && !string.IsNullOrEmpty(UserHelper.Login.user_id))
                    {
                        if (UserHelper.Login.IAPproducts.Length > 0)
                        {
                            RegisterIAP.UpdateAllIAPs();
                        }
                        else
                        {
                            //*** RegisterIAP.ClearAllIAPs();
                        }
                    }  
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Debug.WriteLine("Purchaser -> Restore -> ERROR :" + ex);
                    IAPproducts = null;
                }
                finally
                {
                    //Disconnect, it is okay if we never connected
                    await CrossInAppBilling.Current.DisconnectAsync();
                }
               
                IsRestoring = false;

                if (IAPproducts == null) return false;
                return true;
            });            
        }

        static List<InAppBillingPurchase> GetValidPurchases (IEnumerable<InAppBillingPurchase> purchases)
        {
            
            //filter purchases by id
            Dictionary<string, InAppBillingPurchase> validPurchasesDict = new Dictionary<string, InAppBillingPurchase>();

            foreach (InAppBillingPurchase purchase in purchases)
            {
                string product_id = purchase.ProductId;
                string[] product_id_substrings = product_id.Split('_');
                string main_prod_id = product_id_substrings[product_id_substrings.Length - 1];

                // check time period
                TimeSpan[] timePeriods = new TimeSpan[] { TimeSpan.FromDays(365 / 2 + 10), TimeSpan.FromDays(365 + 10), TimeSpan.FromDays(31 + 7) };
                //TimeSpan[] timePeriods = new TimeSpan[] { TimeSpan.FromMinutes(30*2*24), TimeSpan.FromMinutes(60*24) };
                
                TimeSpan timePeriod = timePeriods[1];
                //if (product_id_substrings[product_id_substrings.Length - 2] == "6") timePeriod = timePeriods[0];

                bool gotTimePeriod = false;
                foreach (KeyValuePair<string, LANGUAGES.LangInfo> keyValuePair in LANGUAGES.CAT_INFO)
                {
                    for (int i = 0; i < keyValuePair.Value.ProductsIds.Length; i++)
                    {
                        if (main_prod_id == keyValuePair.Value.ProductsIds[i])
                        {
                            timePeriod = timePeriods[i];
                            gotTimePeriod = true;
                            break;
                        }
                    }
                    if (gotTimePeriod) break;
                }

                TimeSpan fact = DateTime.Now - purchase.TransactionDateUtc;

                if ((purchase.State == PurchaseState.Purchased || purchase.State == PurchaseState.Restored || purchase.State == PurchaseState.PaymentPending)) // (fact.CompareTo (timePeriod) < 0) // ???
                {
                    // check if already have such a product ?
                    Debug.WriteLine($"Purchaser -> GetValidPurchases -> have valid product, purchase.Id = {purchase.Id}, purchase.ProductId = {purchase.ProductId}, purchase.TransactionDateUtc = {purchase.TransactionDateUtc}");
                    if (validPurchasesDict.ContainsKey(purchase.Id))
                    {
                        // we allready have purchase of the id, compare times
                        int result = DateTime.Compare(purchase.TransactionDateUtc, validPurchasesDict[purchase.Id].TransactionDateUtc);
                        if (result > 0)
                        { // the new purchase if fresher...
                            validPurchasesDict[purchase.Id] = purchase;
                        }
                    }
                    else // add new unic purchase
                    {
                        validPurchasesDict.Add(purchase.Id, purchase);
                    }
                }

            }

            return new List<InAppBillingPurchase> (validPurchasesDict.Values);
        }

    }

    
}
