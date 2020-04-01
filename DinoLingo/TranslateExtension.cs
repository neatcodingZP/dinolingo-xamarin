using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Resources;
using System.Globalization;
using System.Reflection;
using System.Diagnostics;

namespace DinoLingo
{
    [ContentProperty("Text")]
    public class TranslateExtension : IMarkupExtension
    {
        readonly CultureInfo ci;
        const string ResourceId = "DinoLingo.Text.Resource";

        public TranslateExtension()
        {
            ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();

        }

        public string Text { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            Debug.WriteLine($"TranslateExtension -> Text = {Text}, ci.Name = {ci?.Name}");

            if (Text == null)
                return "";

            ResourceManager resmgr = new ResourceManager(ResourceId,
                        typeof(TranslateExtension).GetTypeInfo().Assembly);

            var translation = resmgr.GetString(Text, ci);

            if (translation == null)
            {
                translation = Text;
            }
            Debug.WriteLine($"TranslateExtension -> translation= {translation}");
            return translation.Replace("\\n", Environment.NewLine); ;
        }
    }
}
