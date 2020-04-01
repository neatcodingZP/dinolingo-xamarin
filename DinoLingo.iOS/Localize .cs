using Foundation;
using System.Diagnostics;
using Xamarin.Forms;

[assembly: Dependency(typeof(DinoLingo.iOS.Localize))]
namespace DinoLingo.iOS
{
    public class Localize : ILocalize
    {
        public System.Globalization.CultureInfo GetCurrentCultureInfo()
        {
            Debug.WriteLine("DinoLingo.iOS.Localize -> ");
            var netLanguage = "en";
            var prefLanguage = "en-US";
            if (NSLocale.PreferredLanguages.Length > 0)
            {
                var pref = NSLocale.PreferredLanguages[0];
                netLanguage = pref.Replace("_", "-"); // заменяет pt_BR на pt-BR
            }
            System.Globalization.CultureInfo ci = null;
            Debug.WriteLine("DinoLingo.iOS.Localize ->  netLanguage = " + netLanguage);
            try
            {
                Debug.WriteLine("DinoLingo.iOS.Localize ->  try -> ci = new System.Globalization.CultureInfo");
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
            return ci;
        }
    }
}