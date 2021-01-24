using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.GoogleAnalytics;

namespace DinoLingo
{
    public static class ServerApi
    {
       // static readonly string ROOT_URL_DEV = "https://dev.dinolingo.com/wp-json/dinolingo_api/v1/";
        //static readonly string ROOT_URL = "https://dinolingo.com/wp-json/dinolingo_api/v1/";
        static readonly string ROOT_URL = "https://wp.dinolingo.com/wp-json/dinolingo_api/v1/";

        public static readonly string LOGIN_URL = ROOT_URL + "login";  // login, pass --> Login_Response

        public static readonly string POST_URL = ROOT_URL + "post"; // id, user_id, type, lang_id --> PostResponse
        //public static readonly string POST_URL = "https://dev.dinolingo.com/wp-json/dinolingo_api/v1/post"; // id, user_id, type, lang_id --> PostResponse

        public static readonly string CATS_URL = ROOT_URL + "cats"; // cat : cat_id for product from login --> CategoryResponse

        public static readonly string URL_POSTLIST = ROOT_URL + "post_list"; // cat, user_id, lang_id :from cats --> PostListResponse
        //public static readonly string URL_POSTLIST = "https://dev.dinolingo.com/wp-json/dinolingo_api/v1/" + "post_list";

        public static readonly string BOOK_LIST_URL = ROOT_URL + "book_list"; // lang_id, sort(default: 1), offset(default: 0), limit(default: 20), cat [level-1|level-2|level-3|level-4|multilevel|fun|dinosaur-books] --> PostListResponse

        public static readonly string BOOK_URL = ROOT_URL + "book"; // lang_id, book_id, page(default: 0) --> BookPageResponse

        public static readonly string USER_BY_ID_URL = ROOT_URL + "user_by_id"; // id --> Login_Response

        public static readonly string SHORT_REPORT_URL = ROOT_URL + "short_report"; // cat, user_id --> ShortReportResponse

        public static readonly string BADGE_LIST_URL = ROOT_URL + "badge_list"; // cat, user_id --> BadgeListResponse

        //public static readonly string REPORT_URL = ROOT_URL + "report"; // cat, user_id, daily_limit(default: 100) --> ReportResponse
        public static readonly string REPORT_URL = ROOT_URL + "report_v2";

        public static readonly string ACTIVITY_MAIN_URL = ROOT_URL + "activity_main"; // user_id, post_id, act_daily_id, progress, lesson_type (video, game)  --> ActivityMainResponse

        public static readonly string ACTIVITY_BOOK_URL = ROOT_URL + "activity_book"; // user_id, book_id, act_daily_id, progress --> ActivityBookResponse

        public static readonly string ACTIVITY_TIME_URL = ROOT_URL + "activity_time"; // user_id, add_time, act_daily_id --> ActivityTimeResponse

        public static readonly string NEW_USER_URL = ROOT_URL + "new_user"; // email, pass, fname, lname --> NewUserResponse

        public static readonly string COUPON_URL = ROOT_URL + "coupon"; // user_id, product_list --> CouponResponse

        public static readonly string REGISTER_IAPS = ROOT_URL + "merpsubscription"; // user_id, data = IAPData[],action = [insert|update|clear] --> RegisterIAPsResponse

        public static readonly string RESET_PASSWORD = ROOT_URL + "forgot"; // email --> ResetPasswordResponse

        public static Task<T> PostRequest<T>(string url, string postData, int timeout, T defaultValue = default(T))
        {
            Debug.WriteLine("PostRequest() here..., url =" + url + ", postData = " + postData);
            T objFromServer = defaultValue;

            var request = (HttpWebRequest)WebRequest.Create(url);
            // set timeouts here
            request.Timeout = timeout;
            request.ReadWriteTimeout = timeout;
            request.KeepAlive = false;
            request.AllowWriteStreamBuffering = false;
            request.AllowReadStreamBuffering = false;
            //request.AllowAutoRedirect = true;

            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            
            request.ContentLength = data.Length;            

            return Task.Run(async () =>
            {
                // Debug.WriteLine("ServerApi -> try to get stream ->");
                HttpWebResponse response = null;
                try {                    
                    using (var stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }                    
                    
                    Debug.WriteLine("ServerApi -> try to get response ->");
                    response = (HttpWebResponse)request.GetResponse();                    

                    try
                    {
                        Debug.WriteLine("Try my HttpWebRequest... get text data ");
                        int c = (int)response.StatusCode;

                        var responseString = await new StreamReader(response.GetResponseStream()).ReadToEndAsync();

                        Debug.WriteLine("got object from server: " + responseString);

                        // process response
                        try
                        {
                            objFromServer = JsonConvert.DeserializeObject<T>(responseString);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("error: can not DeserializeObject, ex:" + ex.Message);
                            GoogleAnalytics.Current.Tracker.SendEvent("ServerApi", $"Exception, url= {url}, postData= {postData}", "JsonConvert.ex: " + ex.Message, 1);
                                                        
                            request?.Abort();
                            response?.Close();

                            return defaultValue;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("HttpWebRequest EXCEPTION :(, download text data failed, " + "ReadToEndAsync().ex: " + ex.Message);
                        GoogleAnalytics.Current.Tracker.SendEvent("ServerApi", $"Exception, url= {url}, postData= {postData}", "ReadToEndAsync().ex: " + ex.Message, 2);

                        request?.Abort();
                        response?.Close();
                        return defaultValue;
                    }
                    
                }
                catch (Exception ex) {
                    Debug.WriteLine($"HttpWebRequest EXCEPTION, problem with request.GetRequestStream()  :(, download text data failed, ex.Message= {ex.Message}");
                    if (response != null)
                    {
                        Debug.WriteLine($"HttpWebRequest EXCEPTION, problem with request.GetRequestStream()  :(, download text data failed, response.statusCode= {response.StatusCode}");
                    }
                    GoogleAnalytics.Current.Tracker.SendEvent("ServerApi", $"Exception, url= {url}, postData= {postData}", "GetRequestStream().ex: " + ex.Message, 3);                    

                    request?.Abort();
                    response?.Close();
                    return defaultValue;
                }

                request?.Abort();
                response?.Close();
                return objFromServer;
            });
        }

        public static async Task<T> PostRequestProgressive<T>(string url, string postData, T defaultValue = default(T), int firstTimeout = 2000)
        {            
            T result = await PostRequest<T>(url, postData, firstTimeout, defaultValue);
            if (result != null) return result;

            result = await PostRequest<T>(url, postData, 5000, defaultValue);
            if (result != null) return result;

            result = await PostRequest<T>(url, postData, 20000, defaultValue);
            return result; 
        }
    }

    public class IAPData
    {
        public string product_id { get; set; } // site id of product
        public string period { get; set; } // months
        public string created_at { get; set; } // DateTime.ToString("yyyy-MM-dd HH:mm:ss")
    }

    // ==============================
    public class ResetPasswordResponse
    {
        public string result { get; set; }
        public Login_Response.Error error { get; set; }
    }

    // ==============================
    public class RegisterIAPsResponse
    {
        public string result { get; set; }
        public Login_Response.Error error { get; set; }                
    }

    // ==============================
    public class CouponResponse
    {
        public Result result { get; set; }
        public Login_Response.Error error { get; set; }

        public class Result
        {
            public Coupon[] coupons{ get; set; }

            public class Coupon
            {
                public string course { get; set; }
                public string code { get; set; }
            }
        }
    }

    // ==============================
    public class NewUserResponse
    {
        public Result result { get; set; }
        public Login_Response.Error error { get; set; }

        public class Result
        {
            public int user_id { get; set; }
        }
    }

    // ==============================
    public class ActivityMainResponse {
        public Result result { get; set; }
        public Login_Response.Error error { get; set; }

        public class Result {
            public bool lesson_completed { get; set; }
            public string act_daily_id { get; set; }
        }
    }

    public class ActivityBookResponse
    {
        public Result result { get; set; }
        public Login_Response.Error error { get; set; }

        public class Result
        {
            public string act_daily_id { get; set; }
        }
    }

    public class ActivityTimeResponse
    {
        public Result result { get; set; }
        public Login_Response.Error error { get; set; }

        public class Result
        {
            public string status { get; set; }
        }
    }


    //================================
    public class ReportResponse {
        public Report result { get; set; }
        public Login_Response.Error error { get; set; }

        public class Report {
            public MainInfo main_info { get; set; }
            public ChartInfo chart_info { get; set; }
            public DailyReport[] daily_report { get; set; }
            public LessonsReport[] lessons_report { get; set; }
            public LessonsReport[] books_report { get; set; }

            public class MainInfo {
                public string email { get; set; }
                public string course { get; set; }
                public int dinos { get; set; }
                public int stars { get; set; }
                public int quizes { get; set; }
                public int books { get; set; }
            }

            public class ChartInfo {
                public int lessons_compl { get; set; }
                public int lessons_total { get; set; }
                public int books_compl { get; set; }
                public int books_total { get; set; }
                public int overall_compl { get; set; }
                public int overall_total { get; set; }
            }

            public class DailyReport {
                public string date { get; set; }
                public Report[] report { get; set; }
                public class Report {
                    public string time { get; set; }
                    public string id { get; set; }
                    public string name { get; set; }
                    public string progress { get; set; }
                    public string spent { get; set; }
                }
            }

            public class LessonsReport {
                public string id { get; set; }
                public string last_activity { get; set; }
                public string name { get; set; }
                public string progress { get; set; }
            }
        }
    }


    //=================================
    public class BadgeListResponse
    {
        public Badge[] result { get; set; }
        public Login_Response.Error error { get; set; }

        public class Badge
        {
            public string id { get; set; }
            public string thumbnail { get; set; }
            public string title { get; set; }
        }
    }

    //=================================

    public class ShortReportResponse {
        public ShortReport result { get; set; }
        public Login_Response.Error error { get; set; } 

        public class ShortReport {
            public int max_dinos_cnt { get; set; }
            public NewDino new_dino { get; set; }
            public int stars { get; set; }
            public int quizes { get; set; }
            public Dino[] dinos { get; set; }

            public class NewDino {
                public string title { get; set; }
                public string img { get; set; }
            }
            public class Dino
            {
                public string id { get; set; }
                public string img { get; set; }
            }
        }
    }

    //=================================
    public class BookPageResponse {
        public BookPage result { get; set; }
        public Login_Response.Error error { get; set; } 

        public class BookPage {
            public int prev_page { get; set; }
            public string page_count { get; set; }
            public int next_page { get; set; }
            public Data data { get; set; }
            public string title { get; set; }
            public Data engTrans { get; set; }

            public class Data {
                public string book_id { get; set; }
                public string title { get; set; }
                public string content { get; set; }
                public string page_num { get; set; }
                public string audio { get; set; }
                public string image { get; set; }               
            }
        }
    }

    //==============================
    public class PostResponse {
        public Post result { get; set; }
        public Login_Response.Error error { get; set; }

        public class Post {
            public string id { get; set; }
            public string title { get; set; }
            public string trans_title { get; set; }
            public string content { get; set; }
            public string type { get; set; }
            public string after_finish { get; set; }
        }
    }
    //============================================
    public class CategoryResponse
    {
        public Category[] result { get; set; }
        public Login_Response.Error error { get; set; }

        public class Category
        {
            public string term_id { get; set; }
            public string name { get; set; }
            public string slug { get; set; }
            public int count { get; set; }
            public MainPage_ViewModel.CENTRAL_VIEWS viewType { get; set; }
        }

        public void ReorderForGame(string lang_id)
        {            
            string firstCat = LANGUAGES.CAT_INFO[lang_id].GetSlug() + "-for-kids";
            string[] keys = { firstCat, "books-for-kids", "stories-for-kids", "songs-for-kids" };
            //{LESSONS_AND_GAMES, BOOKS, STORIES, SONGS, NONE}
            List<Category> categories = new List<Category>();
            for (MainPage_ViewModel.CENTRAL_VIEWS view = MainPage_ViewModel.CENTRAL_VIEWS.LESSONS_AND_GAMES; view <= MainPage_ViewModel.CENTRAL_VIEWS.SONGS; view++)
            {
                foreach (Category category in result) {
                    if (category == null) continue;
                    if (category.slug.ToLower().Contains(keys[(int) view]) && category.count > 0) {
                        categories.Add(new Category { term_id = category.term_id, name = category.name, count = category.count, slug = category.slug, viewType = view });
                        break;
                    }
                }
            }
            if (categories.Count > 0) result = categories.ToArray();
        }

    }
    //======================================
    public class Login_Response
    {
        public Login result { get; set; }
        public Error error { get; set; }


        public class Login
        {
            public string user_id { get; set; }
            public string user_login { get; set; }
            public string user_email { get; set; }
            public string display_name { get; set; }
            public Product[] products { get; set; }
            public Product[] IAPproducts { get; set; }


            public bool HasProducts() {
                if (products == null || products.Length == 0) return false;
                return true;
            }

            public void DeleteDuplicateProducts () {
                Debug.WriteLine("ServerAPI -> DeleteDuplicateProducts ->");
                if (products == null) return;
                List<Product> products_new = new List<Product>();
                foreach (Product source_p in products) {
                    Debug.WriteLine($"ServerAPI -> DeleteDuplicateProducts -> watch product_id: {source_p.product_id}");
                    bool isProductUniqe = true;
                    foreach (Product new_p in products_new) {
                        if (new_p.cat_id == source_p.cat_id && 
                            new_p.product_id == source_p.product_id && 
                            new_p.course == source_p.course) {
                            // products are clones
                            isProductUniqe = false;
                            break;
                        }                       
                    }

                    if (isProductUniqe) {

                        if (source_p.cat_id == 0)
                        {
                            // unknown product
                        }
                        else if (string.IsNullOrEmpty(source_p.product_id) || source_p.product_id == "0")
                        {
                            products_new.Add(source_p);
                            Debug.WriteLine("ServerAPI -> added product with null or 0 id, cat_id :" + source_p.cat_id);
                        }
                        else
                        {
                            products_new.Add(source_p);
                            Debug.WriteLine("ServerAPI -> added product :" + source_p.product_id);
                        }                        
                    }
                }

                products = products_new.ToArray();
            }


        }

        public class Product
        {
            public string product_id { get; set; }
            public int cat_id { get; set; }
            public string course { get; set; }
            public string trans_date { get; set; }
        }

        public class Error
        {
            public string code { get; set; }
            public string message { get; set; }
        }
    }
}
