using System;
using Xamarin.Forms;

namespace DinoLingo
{
	public class ProductTemplateSelector : DataTemplateSelector
{
        public DataTemplate HeaderWithImageItemTemplate { get; set; }
        public DataTemplate HeaderItemTemplate { get; set; }
        public DataTemplate SpaceItemTemplate { get; set; }
        public DataTemplate SingleItemTemplate { get; set; }
        public DataTemplate DoubleItemTemplate { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {

            if (item is HeaderWithImageItem) return HeaderWithImageItemTemplate;
            if (item is HeaderItem) return HeaderItemTemplate;
            if (item is SpaceItem) return   SpaceItemTemplate;
            if (item is SingleItem) return SingleItemTemplate;
            if (item is DoubleItem) return DoubleItemTemplate;

            return HeaderItemTemplate;
    }
}
}
