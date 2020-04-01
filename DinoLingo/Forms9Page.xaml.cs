using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DinoLingo
{
    public partial class Forms9Page : ContentPage
    {
        public Forms9Page()
        {
            InitializeComponent();
            PhoneLabel.ActionTagTapped += ActionTagTapped;
            EmailLabel.ActionTagTapped += ActionTagTapped;
        }

        private void ActionTagTapped(object sender, Forms9Patch.ActionTagEventArgs e)
        {
            Forms9Patch.Toast.Create("Link Activated", "The link (id: " + e.Id + ", href:" + e.Href + ") was activated.");
        }
    }
}
