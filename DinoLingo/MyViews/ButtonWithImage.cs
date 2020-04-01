using Xamarin.Forms;

namespace DinoLingo.MyViews
{
    class ButtonWithImage : AbsoluteLayout
    {
        Label myLabel_;
        public string Text
        {
            get
            {
                return myLabel_?.Text;
            }
            set {
                myLabel_.Text = value;
            }
        }
            
        public ButtonWithImage(
            string classId, 
            Rectangle mainAbsLayout, 
            string imgName, 
            string text, double fontSize, Color textColor, 
            TapGestureRecognizer tapGestureRecognizer,
            AbsoluteLayoutFlags flags = AbsoluteLayoutFlags.All)
        {
            Image buttonImage = new Image
            {
                Source = Forms9Patch.ImageSource.FromResource(imgName),
                Aspect = Aspect.Fill,
            };
            SetLayoutBounds(buttonImage, new Rectangle(0.5, 0.5, 1, 1));
            SetLayoutFlags(buttonImage, AbsoluteLayoutFlags.All);

            AbsoluteLayout buttonAbsLayout = new AbsoluteLayout {ClassId = classId };
            buttonAbsLayout.GestureRecognizers.Add(tapGestureRecognizer);
            SetLayoutBounds(buttonAbsLayout, mainAbsLayout);
            SetLayoutFlags(buttonAbsLayout, flags);

            
            MyLabel myLabel = new MyLabel {
                Text = text,
                FontSize = fontSize,
                TextColor = textColor,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
            };
            myLabel_ = myLabel;
            SetLayoutBounds(myLabel, new Rectangle(0.5, 0.5, 1, 1));
            SetLayoutFlags(myLabel, AbsoluteLayoutFlags.All);

            buttonAbsLayout.Children.Add(buttonImage);
            buttonAbsLayout.Children.Add(myLabel);

            Children.Add(buttonAbsLayout);
        }

    }
}
