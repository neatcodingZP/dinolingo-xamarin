using System.Diagnostics;
using Xamarin.Forms;
[assembly: Dependency(typeof(DinoLingo.Droid.Localize))]

namespace DinoLingo.Droid
{
    public class Localize : ILocalize
    {
        public System.Globalization.CultureInfo GetCurrentCultureInfo()
        {
            var androidLocale = Java.Util.Locale.Default;
            Debug.WriteLine("Droid.Localize -> androidLocale = " + androidLocale.ToString());
            var netLanguage = androidLocale.ToString().Replace("_", "-");

            string[] substrings = netLanguage.Split("-");
            if (substrings[0].ToLower() == "in") netLanguage = "id";
            Debug.WriteLine("Droid.Localize -> netLanguage = " + netLanguage);
            return new System.Globalization.CultureInfo(netLanguage);
        }
    }
}