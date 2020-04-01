using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DinoLingo
{
    [ContentProperty("Source")]
    public class ImageMultiResourceExtension : IMarkupExtension
    {
        public string Source { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null)
                return null;

            // Do your translation lookup here, using whatever method you require
            var imageSource = Forms9Patch.ImageSource.FromMultiResource(Source);

            return imageSource;
        }
    }

}
