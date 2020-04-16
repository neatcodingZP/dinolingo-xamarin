using Foundation;
using System.Diagnostics;
using System.Threading;
using Xamarin.Forms;

[assembly: Dependency(typeof(DinoLingo.iOS.Localize))]
namespace DinoLingo.iOS
{
    public class Localize : ILocalize
    {
        public System.Globalization.CultureInfo GetCurrentCultureInfo()
        {
            
            var netLanguage = "en";
            var prefLanguage = "en-US";


            /*
            var lang = NSBundle.MainBundle.PreferredLocalizations[0];
            Debug.WriteLine("DinoLingo.iOS.Localize -> NSBundle.MainBundle.PreferredLocalizations");

            foreach (var s in NSBundle.MainBundle.PreferredLocalizations)
            {
                Debug.WriteLine(s);
            }
            */           

            
            if (NSLocale.PreferredLanguages.Length > 0)
            {
                var pref = NSLocale.PreferredLanguages[0];
                Debug.WriteLine("DinoLingo.iOS.Localize ->  NSLocale.PreferredLanguages: ");
                foreach (var s in NSLocale.PreferredLanguages)
                {
                    Debug.WriteLine(s);
                }

                netLanguage = pref.Replace("_", "-"); // заменяет pt_BR на pt-BR
            }

            var localIdentifier = NSLocale.CurrentLocale.LocaleIdentifier;
            Debug.WriteLine("DinoLingo.iOS.Localize ->  LocaleIdentifier = " + localIdentifier);

            var identifier = NSLocale.AutoUpdatingCurrentLocale.LocaleIdentifier;
            Debug.WriteLine("DinoLingo.iOS.Localize ->  identifier = " + identifier);           


            System.Globalization.CultureInfo ci = null;
            Debug.WriteLine("DinoLingo.iOS.Localize ->  netLanguage = " + netLanguage);
            try
            {
                
                ci = new System.Globalization.CultureInfo(netLanguage);
                Debug.WriteLine("DinoLingo.iOS.Localize ->  ci.Name = " + ci.Name);
               
            }
            catch
            {
                // here we have some non-common culture
                string[] languages = netLanguage.Split('-');
                try
                {
                    Debug.WriteLine("DinoLingo.iOS.Localize ->  try -> try -> ci = new System.Globalization.CultureInfo");
                    ci = new System.Globalization.CultureInfo(languages[0]);
                    Debug.WriteLine("DinoLingo.iOS.Localize ->  try -> try -> ci.Name = " + ci.Name);
                }
                catch
                {
                    Debug.WriteLine("DinoLingo.iOS.Localize ->  catch -> catch");
                    ci = new System.Globalization.CultureInfo(prefLanguage);
                    Debug.WriteLine("DinoLingo.iOS.Localize -> catch -> catch ->  ci.Name = " + ci.Name);
                }                
            }

            Debug.WriteLine("DinoLingo.iOS.Localize -> RETURN ->  ci.Name = " + ci.Name);

            Debug.WriteLine("DinoLingo.iOS.Localize ->  NSLocale.PreferredLanguages.Length: " + NSLocale.PreferredLanguages.Length);



            //return new System.Globalization.CultureInfo("ru");
            //SetLanguage(ci);

            return ci;
        }

        public void SetLanguage(System.Globalization.CultureInfo ci)
        {
            
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }
    }
}