using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace DinoLingo
{
    public class SAS_Data_ANIMALS_1
    {
        static int labelHeight = 30 * 2;
        static Color labelColor = Color.Black;
        //static int labelOffset = 5;

        public static SAS_DataItem data = new SAS_DataItem
        {
            
            ImgFolder = "ANIMALS",
            Background = new SAS_Background
            {
                Fill = Forms9Patch.Fill.Fill,
                BaseRect = new Rect { x = -1, y = -1, width = 1224, height = 790 },
                ImgFile = "animals_fon.jpg",
                color = Color.Aqua
            },

            ActiveImages = new List<SAS_Object> {
                new SAS_Object {
                    KeyName = "ANT",
                    ImgFile = "ANT.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                    BaseCoords = new Rect (29,43,150,150),
                    Bounds = new List<Rect> {
                        new Rect  (29,43,150,150)
                       }
                },

                new SAS_Object {
                    KeyName = "BEAR",
                    ImgFile = "BEAR.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (140,13,200,200),
                        Bounds = new List<Rect> {
                        new Rect  (140,13,200,200)
                        }
                    },

                new SAS_Object {
                    KeyName = "COW",
                    ImgFile = "COW.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (333,109,200,200),
                        Bounds = new List<Rect> {
                        new Rect (333,109,200,200)
                        }
                    },

                new SAS_Object {
                    KeyName = "BEE",
                    ImgFile = "BEE.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (504,5,160,160),
                        Bounds = new List<Rect> {
                        new Rect  (504,5,160,160)
                        }
                    },
                new SAS_Object {
                    KeyName = "HIPPOPOTAMUS",
                    ImgFile = "HIPPO.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (733,4,180,180),
                        Bounds = new List<Rect> {
                        new Rect  (733,4,180,180)
                        }
                    },

                new SAS_Object {
                    KeyName = "BUTTERFLY",
                    ImgFile = "BUTTERFLY.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (972,62,160,160),
                        Bounds = new List<Rect> {
                        new Rect  (972,62,160,160)
                        }
                    },

                new SAS_Object {
                    KeyName = "CROCODILE",
                    ImgFile = "CROCODILE.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (621,156,200,200),
                        Bounds = new List<Rect> {
                        new Rect  (621,156,200,200)
                        }
                    },
                new SAS_Object {
                    KeyName = "DEER",
                    ImgFile = "DEER.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (858,196,200,200),
                        Bounds = new List<Rect> {
                        new Rect  (858,196,200,200)
                        }
                    },
                new SAS_Object {
                    KeyName = "FROG",
                    ImgFile = "FROG.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (1038,260,150,150),
                        Bounds = new List<Rect> {
                        new Rect  (1038,260,150,150)
                        }
                    },

                new SAS_Object {
                    KeyName = "EAGLE",
                    ImgFile = "EAGLE.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (5,187,200,200),
                        Bounds = new List<Rect> {
                        new Rect (5,187,200,200)
                        }
                },
                new SAS_Object {
                    KeyName = "CAMEL",
                    ImgFile = "CAMEL.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (22,331,200,200),
                        Bounds = new List<Rect> {
                        new Rect  (22,331,200,200)
                        }
                },
                new SAS_Object {
                    KeyName = "CHICKEN",
                    ImgFile = "CHICKEN.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (191,407,150,150),
                        Bounds = new List<Rect> {
                        new Rect  (191,407,150,150)
                        }
                },
                new SAS_Object {
                    KeyName = "DUCK",
                    ImgFile = "DUCK.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (353,430,150,150),
                        Bounds = new List<Rect> {
                        new Rect  (353,430,150,150)
                        }
                },
                new SAS_Object {
                    KeyName = "DOLPHIN",
                    ImgFile = "DOLPHIN.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (516,336,200,200),
                        Bounds = new List<Rect> {
                        new Rect  (516,336,200,200)
                        }
                },
                new SAS_Object {
                    KeyName = "DOG",
                    ImgFile = "DOG.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (666,352,160,160),
                        Bounds = new List<Rect> {
                        new Rect (666,352,160,160)
                        }
                },
                new SAS_Object {
                    KeyName = "BIRD",
                    ImgFile = "BIRD.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (832,378,160,160),
                        Bounds = new List<Rect> {
                        new Rect  (832,378,160,160)
                        }
                },
                new SAS_Object {
                    KeyName = "CAT",
                    ImgFile = "CAT.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (-24,528,160,160),
                        Bounds = new List<Rect> {
                        new Rect  (-24,528,160,160)
                        }
                },
                new SAS_Object {
                    KeyName = "CHICK",
                    ImgFile = "CHICK.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (168,572,150,150),
                        Bounds = new List<Rect> {
                        new Rect  (168,572,150,150)
                        }
                },
                new SAS_Object {
                    KeyName = "FISH",
                    ImgFile = "FISH.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (357,636,160,160),
                        Bounds = new List<Rect> {
                        new Rect  (357,636,160,160)
                        }
                },
                new SAS_Object {
                    KeyName = "ELEPHANT",
                    ImgFile = "ELEPHANT.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (524,535,230,230),
                        Bounds = new List<Rect> {
                        new Rect  (524,535,230,230)
                        }
                },
                new SAS_Object {
                    KeyName = "DONKEY",
                    ImgFile = "DONKEY.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (758,547,200,200),
                        Bounds = new List<Rect> {
                        new Rect (758,547,200,200)
                        }
                },
                new SAS_Object {
                    KeyName = "FLAMINGO",
                    ImgFile = "FLAMINGO.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (879,550,200,200),
                        Bounds = new List<Rect> {
                        new Rect  (879,550,200,200)
                        }
                },
                new SAS_Object {
                    KeyName = "GIRAFFE",
                    ImgFile = "GIRAFFE.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (1019,520,230,230),
                        Bounds = new List<Rect> {
                        new Rect  (1019,520,230,230)
                        }
                },

            },



            Labels = new List<SAS_Label>
            {
                new SAS_Label {
                    KeyName = "ANT",
                    BaseCoords = new Rect (29,43-labelHeight,150,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "BEAR",
                    BaseCoords = new Rect (140,13+200,200,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "COW",
                    BaseCoords = new Rect (333,109+200,200,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "BEE",
                    BaseCoords = new Rect (504,5+160,160,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "HIPPOPOTAMUS",
                    BaseCoords = new Rect (733,4+180*0.9,180,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "BUTTERFLY",
                    BaseCoords = new Rect (972,62-labelHeight/2,160,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "CROCODILE",
                    BaseCoords = new Rect (621,156+200*0.8,200,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "DEER",
                    BaseCoords = new Rect (858-50,196+200*0.75,200,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "FROG",
                    BaseCoords = new Rect (1038,260+150,150,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "EAGLE",
                    BaseCoords = new Rect (5,187+15,150,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "CAMEL",
                    BaseCoords = new Rect (22,331+15,150,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "CHICKEN",
                    BaseCoords = new Rect (191,407-labelHeight/2,150,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "DUCK",
                    BaseCoords = new Rect (353,430-labelHeight/2,150,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "DOLPHIN",
                    BaseCoords = new Rect (516,336+200*0.8,200,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "DOG",
                    BaseCoords = new Rect (666,352+160,160,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "BIRD",
                    BaseCoords = new Rect (832,378+160*0.8,160,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "CAT",
                    BaseCoords = new Rect (-24,528+160,160,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "CHICK",
                    BaseCoords = new Rect (168,572+150,150,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "FISH",
                    BaseCoords = new Rect (357,636+160*0.8,160,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "ELEPHANT",
                    BaseCoords = new Rect (524,535+230,230,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "DONKEY",
                    BaseCoords = new Rect (758,547+200,200,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "FLAMINGO",
                    BaseCoords = new Rect (879,550+200+15,200,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "GIRAFFE",
                    BaseCoords = new Rect (1019,520+230,230,labelHeight),
                    color = labelColor
                },

            }

        };
    }
}
