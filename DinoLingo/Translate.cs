
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Text;
using Xamarin.Forms;

namespace DinoLingo
{
    public static class Translate
    {

        static CultureInfo ci { get; set; }
        const string ResourceId = "DinoLingo.Text.Resource";
        static ResourceManager resmgr;

        // font options
        public static string fontFamily;
        public static FontAttributes fontAttributes;

        //
        public static string LangId { get; set; }
        public static FlowDirection FlowDirection_ {get; set; }

        public static void Init()
        {
            Debug.WriteLine("Translate -> Init");

           CultureInfo ci_prev = ci;
            

            ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            resmgr = new ResourceManager(ResourceId,
                        typeof(App).Assembly);
            

            Debug.WriteLine("Translate -> Init -> ci.Name" + ci.Name);
            Debug.WriteLine($"Translate -> Init -> Name= {ci.Name}, Parent.Name= {ci.Parent.Name}, EnglishName= {ci.EnglishName}, IsNeutralCulture= {ci.IsNeutralCulture}");
            /*
             *             

            Debug.WriteLine("Translate -> Init -> NeutralCultures");
            IEnumerable<CultureInfo> cultures = CultureInfo.GetCultures(CultureTypes.NeutralCultures);
            foreach (CultureInfo culture in cultures)
            {
                Debug.WriteLine($"Name= {culture.Name}, EnglishName= {culture.EnglishName}, IsNeutralCulture= {culture.IsNeutralCulture}");
            }

            Debug.WriteLine("Translate -> Init -> SpecificCultures");
            IEnumerable<CultureInfo> cultures_s = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            foreach (CultureInfo culture in cultures_s)
            {
                Debug.WriteLine($"Name= {culture.Name}, Parent.Name= {culture.Parent.Name}, EnglishName= {culture.EnglishName}, IsNeutralCulture= {culture.IsNeutralCulture}");
            }
            */

            if (ci_prev == null)
            {
                Debug.WriteLine("Translate -> START LOCALIZATION -> ");
                string name = GetString("lang_");
                SetFont(name);
                SetDirection(name);
                SetLangId(name);
                RateWidget.SetStrings();
                POP_UP.SetStrings();
                LANGUAGES.SetVisibleNames();
            }
            else if (ci.Name != ci_prev.Name)
            {
                Debug.WriteLine("Translate -> NEW LOCALIZATION -> Application.Current.Quit()");
                // close app
                Application.Current.Quit();
            }
        }

        static void SetFont(string name)
        {
            Debug.WriteLine("Translate -> SetFont");
            if (ci == null)
            {
                fontAttributes = FontAttributes.None;
                fontFamily = Font.Default.FontFamily;
                return;
            }

            //string name = string.Empty;
            /*
            if (ci.IsNeutralCulture) name = ci.Name;
            else if (ci.Parent.IsNeutralCulture) name = ci.Parent.Name;
            else if (ci.Parent.Parent.IsNeutralCulture) name = ci.Parent.Parent.Name;
            */
                       

            switch (name)
            {
                // === LATINS ===
                case "id": // Indonesian +++
                case "pl": // Polish +++
                case "sv": // Swedish +++
                case "it": // Italian +++
                case "pt": // Portuguese +++
                case "nl": // Dutch +++
                case "de": // German +++
                case "tr": // Turkish +++
                case "es": // Spanish +++
                case "fr": // French +++
                case "en": // english +++
                    // set lang_id
                    
                    if (Device.RuntimePlatform == Device.iOS) fontFamily = "Arial Rounded MT Bold";
                    else if (Device.RuntimePlatform == Device.Android) fontFamily = "Arial_Rounded_MT_Bold.ttf#Arial Rounded MT Bold";
                    fontAttributes = FontAttributes.None;
                    break;

                // latins no res
                case "sq": // albanian
                case "cs": // czech
                case "da": // danish                
                case "fi": // finnish 
                case "ga": // Irish                
                case "no": // Norwegian
                               
                case "ro": // Romanian                
                case "cy": // Welsh  

                // have no res
                case "el": // Greek +++
                case "ar": // arabic +++
                case "hy": // armenian
                case "bg": // bulgarian
                case "zh": // chineese +++
                case "hr": // croatian
                case "gu": // Gujarati
                case "haw": // Hawaiian
                case "he": // Hebrew
                case "hi": // Hindi
                case "hu": // Hungarian                
                case "ja": // Japanese +++
                case "kk": // Kazakh
                case "ko": // Korean +++
                case "ms": // Malay
                case "fa": // Persian  
                case "pa": // Punjabi 
                case "ru": // Russian +++
                case "sr": // Serbian
                case "sk": // Slovak
                case "sl": // Slovenian
                case "sw": // Swahili
                case "th": // Thai
                
                case "uk": // Ukrainian +++
                case "ur": // Urdu 
                case "vi": // Vietnamese 

                // === NOT LATINS 
                default:
                    fontAttributes = FontAttributes.None;
                    fontFamily = Font.Default.FontFamily;
                    break;
            }
        }

        static void SetDirection(string name)
        {
            switch (name)
            {
                case "ar": // arabic
                case "he": // Hebrew
                    FlowDirection_ = FlowDirection.RightToLeft;
                    break;

                case "pa": // Punjabi  ???
                case "ur": // Urdu ???
                default:
                    FlowDirection_ = FlowDirection.LeftToRight;
                    break;
            }
        }

        static void SetLangId(string name)
        {    
            switch (name)
            {                
                case "en": LangId = "1"; break; 
                    
                case "sq": LangId = "39"; break; // albanian
                case "cs": LangId = "32"; break;  // czech
                case "da": LangId = "33"; break; // danish                
                case "fi": LangId = "16"; break; // finnish
                case "fr": LangId = "25"; break; // French
                case "nl": LangId = "4"; break; // Dutch
                case "de": LangId = "21"; break; // German
                case "ga": LangId = "28"; break; // Irish
                case "it": LangId = "12"; break; // Italian
                case "no": LangId = "35"; break; // Norwegian
                case "pl": LangId = "2"; break; // Polish
                case "pt": LangId = "7"; break; //LangId = "17"; break; // Portuguese
                //case "pt-BR": LangId = "7"; break; // Portuguese
                case "ro": LangId = "36"; break; // Romanian
                case "es": LangId = "8"; break; // Spanish
                case "sv": LangId = "13"; break; // Swedish
                case "cy": LangId = "43"; break; // Welsh 

                case "el": LangId = "5"; break; // Greek
                case "ar": LangId = "9"; break; // arabic
                case "hy": LangId = "40"; break; // armenian
                case "bg": LangId = "27"; break; // bulgarian
                case "zh": LangId = "14"; break; // chineese
                case "hr": LangId = "31"; break; // croatian
                case "gu": LangId = "44"; break; // Gujarati
                case "haw": LangId = "55"; break; // Hawaiian
                case "he": LangId = "22"; break; // Hebrew
                case "hi": LangId = "30"; break; // Hindi
                case "hu": LangId = "26"; break; // Hungarian
                case "id": LangId = "29"; break; // Indonesian
                case "ja": LangId = "6"; break; // Japanese
                case "kk": LangId = "57"; break; // Kazakh
                case "ko": LangId = "10"; break; // Korean
                case "ms": LangId = "34"; break; // Malay
                case "fa": LangId = "23"; break; // Persian  
                case "pa": LangId = "62"; break; // Punjabi 
                case "ru": LangId = "15"; break; // Russian
                case "sr": LangId = "37"; break; // Serbian
                case "sk": LangId = "41"; break; // Slovak
                case "sl": LangId = "63"; break; // Slovenian
                case "sw": LangId = "38"; break; // Swahili
                case "th": LangId = "20"; break; // Thai
                case "tr": LangId = "3"; break; // Turkish   
                case "uk": LangId = "42"; break; // Ukrainian 
                case "ur": LangId = "19"; break; // Urdu 
                case "vi": LangId = "18"; break; // Vietnamese                

                default:    LangId = "1";  break;
            }
        }

        public static string GetString (string key)
        {
            var translation = resmgr.GetString(key, ci);

            if (translation == null)
            {
                translation = key;
            }
            
            return translation.Replace("\\n", Environment.NewLine);
        }



    }
}
