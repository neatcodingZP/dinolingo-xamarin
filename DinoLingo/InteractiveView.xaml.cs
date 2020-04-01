using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DinoLingo
{
    public partial class InteractiveView : ContentView
    {
        public ImageSource ImageSource { get; set; }
        public int id { get; set; }
        public string Key { get; set; }
        public Label InnerLabel;

        public InteractiveView(SAS_Label innerLabel, string labelText)
        {
            InitializeComponent();
            BindingContext = this;

            //add inner label if have
            if (innerLabel != null)
            {
                Label label = new Label
                {
                    BackgroundColor = Color.Transparent,
                    TextColor = innerLabel.color,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    FontSize = App.Current.MainPage.Height * 0.025, //Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                    FontAttributes = FontAttributes.Bold,
                    Text = labelText
                };
                InnerLabel = label;

                double x = innerLabel.BaseCoords.x / (1-innerLabel.BaseCoords.width + 0.001);
                double y = innerLabel.BaseCoords.y / (1 - innerLabel.BaseCoords.height + 0.001);

                double width = innerLabel.BaseCoords.width;
                double height = innerLabel.BaseCoords.height;

                AbsoluteLayout.SetLayoutBounds(label, new Rectangle(x, y, width, height));
                AbsoluteLayout.SetLayoutFlags(label, AbsoluteLayoutFlags.All);
                absLayout.Children.Add(label);

                /*
                relLayout.Children.Add(label,
                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Width * innerLabel.BaseCoords.x;    // установка координаты X
                    }),
                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Height * innerLabel.BaseCoords.y;   // установка координаты Y
                    }),

                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Width * innerLabel.BaseCoords.width;
                }), // установка ширины
                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Height * innerLabel.BaseCoords.height;
                })
                );
*/
            }

        }

        public void Dispose()
        {
            ImageSource = null;
            InnerLabel = null;
            Content = null;
            BindingContext = null;
        }

        public Task Animate(SAS_Object.ANIM_TYPE animType) {
            
            switch (animType) {

                case SAS_Object.ANIM_TYPE.UP_DOWN:
                    return Task.Run( async () =>
                    {
                        Debug.WriteLine (SAS_Object.ANIM_TYPE.UP_DOWN);
                        await this.TranslateTo(0, -App.Current.MainPage.Height * 0.05, 150);
                        await this.TranslateTo(0, 0, 150);
                        return;
                    });

                case SAS_Object.ANIM_TYPE.SCALE:
                    return Task.Run(async () =>
                    {
                        
                        Debug.WriteLine(SAS_Object.ANIM_TYPE.SCALE);
                        await this.ScaleTo(1.2, 150);
                        await this.ScaleTo(1.0, 150);
                        return;
                    });
                

                case SAS_Object.ANIM_TYPE.SWING:
                    return Task.Run(async () =>
                    {

                        await this.RotateTo(10, 150);
                        await this.RotateTo(0, 150);
                        await this.RotateTo(-10, 150);
                        await this.RotateTo(5, 100);
                        await this.RotateTo(0, 100);

                        return;

                     });

                case SAS_Object.ANIM_TYPE.ROT_ANDBACK:
                    return Task.Run(async () =>
                    {
                        
                        await Task.WhenAll (
                            this.RotateTo(90, 250),
                            this.ScaleTo(1.2, 250)
                        );
                        await Task.WhenAll(
                            this.RotateTo(0, 250),
                            this.ScaleTo(1.0, 250)
                        );

                        return;
                    });

                case SAS_Object.ANIM_TYPE.FLASH:
                    return Task.Run(async () =>
                    {
                        await this.FadeTo(0, 150);
                        await this.FadeTo(1, 150);
                        await this.FadeTo(0, 150);
                        await this.FadeTo(1, 150);
                        return;
                    });

                
                default: return Task.Run(() =>
                {

                    return;
                });
            }

        }
    }
}
