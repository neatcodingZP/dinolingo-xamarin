using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace DinoLingo
{
    public class SAS_Data_FOOD
    {
        static SAS_Label InnerLabel = new SAS_Label
        {
            BaseCoords = new Rect(0, 0.7, 1, 0.3),
            color = Color.Brown
        };

        static double dx = 246;
        static double dy = 250;
        static double x0 = 34;
        static double y0 = 37;

        public static SAS_DataItem data = new SAS_DataItem{
            
            ImgFolder = "FOOD.SAS",
            Background = new SAS_Background
            {
                Fill = Forms9Patch.Fill.Fill,
                BaseRect = new Rect { x = -1, y = -1, width = 1224, height = 792 },
                ImgFile = "food_fon.png",
                color = Color.White
            },

            ActiveImages = new List<SAS_Object> {
                    new SAS_Object {
                    KeyName = "CAKE",
                        ImgFile = "CAKE.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (x0 + 1 *dx, y0 + 1 *dy, 224,224),
                        Bounds = new List<Rect> {
                            new Rect (x0 + 1 *dx, y0 + 1 *dy, 224,224)
                        },
                        InnerLabel = InnerLabel
                    },

                new SAS_Object {
                    KeyName = "MILK",
                        ImgFile = "MILK.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (x0 + 3 *dx, y0 + 1 *dy, 224,224),
                        Bounds = new List<Rect> {
                            new Rect (x0 + 3 *dx, y0 + 1 *dy, 224,224)
                        },
                        InnerLabel = InnerLabel
                    },

                new SAS_Object {
                    KeyName = "CHOCOLATE",
                        ImgFile = "CHOCOLATE.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (x0 + 2 *dx, y0 + 2 *dy, 224,224),
                        Bounds = new List<Rect> {
                            new Rect (x0 + 2 *dx, y0 + 2 *dy, 224,224)
                        },
                        InnerLabel = InnerLabel
                    },

                new SAS_Object {
                    KeyName = "BREAD",
                        ImgFile = "BREAD.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (x0 + 2 *dx, y0 + 0 *dy, 224,224),
                        Bounds = new List<Rect> {
                            new Rect (x0 + 2 *dx, y0 + 0 *dy, 224,224)
                        },
                        InnerLabel = InnerLabel
                    },

                new SAS_Object {
                    KeyName = "WATER",
                        ImgFile = "WATER.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (x0 + 0 *dx, y0 + 2 *dy, 224,224),
                        Bounds = new List<Rect> {
                            new Rect (x0 + 0 *dx, y0 + 2 *dy, 224,224)
                        },
                        InnerLabel = InnerLabel
                    },

                new SAS_Object {
                    KeyName = "EGG",
                        ImgFile = "EGG.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (x0 + 0 *dx, y0 + 1 *dy, 224,224),
                        Bounds = new List<Rect> {
                            new Rect (x0 + 0 *dx, y0 + 1 *dy, 224,224)
                        },
                        InnerLabel = InnerLabel
                    },

                new SAS_Object {
                    KeyName = "BUTTER",
                        ImgFile = "BUTTER.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (x0 + 3 *dx, y0 + 0 *dy, 224,224),
                        Bounds = new List<Rect> {
                            new Rect (x0 + 3 *dx, y0 + 0 *dy, 224,224)
                        },
                        InnerLabel = InnerLabel
                    },

                new SAS_Object {
                    KeyName = "CHEESE",
                        ImgFile = "CHEESE.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (x0 + 0 *dx, y0 + 0 *dy, 224,224),
                        Bounds = new List<Rect> {
                            new Rect (x0 + 0 *dx, y0 + 0 *dy, 224,224)
                        },
                        InnerLabel = InnerLabel
                    },

                new SAS_Object {
                    KeyName = "HONEY",
                        ImgFile = "HONEY.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (x0 + 1 *dx, y0 + 0 *dy, 224,224),
                        Bounds = new List<Rect> {
                            new Rect (x0 + 1 *dx, y0 + 0 *dy, 224,224)
                        },
                        InnerLabel = InnerLabel
                    },

                new SAS_Object {
                    KeyName = "JUICE",
                        ImgFile = "JUICE.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (x0 + 3 *dx, y0 + 2 *dy, 224,224),
                        Bounds = new List<Rect> {
                            new Rect (x0 + 3 *dx, y0 + 2 *dy, 224,224)
                        },
                        InnerLabel = InnerLabel
                    },

                new SAS_Object {
                    KeyName = "SOUP",
                        ImgFile = "SOUP.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (x0 + 1 *dx, y0 + 2 *dy, 224,224),
                        Bounds = new List<Rect> {
                            new Rect (x0 + 1 *dx, y0 + 2 *dy, 224,224)
                        },
                        InnerLabel = InnerLabel
                    },

                new SAS_Object {
                    KeyName = "ICECREAM",
                        ImgFile = "ICECREAM.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (x0 + 2 *dx, y0 + 1 *dy, 224,224),
                        Bounds = new List<Rect> {
                            new Rect (x0 + 2 *dx, y0 + 1 *dy, 224,224)
                        },
                        InnerLabel = InnerLabel
                    },
            },

            StaticImages = new List<SAS_Object> {
                    new SAS_Object {
                        ImgFile = "food_girl.png",
                        BaseCoords = new Rect (958,107,285,540)
                    },
            }

        };
    }
}
