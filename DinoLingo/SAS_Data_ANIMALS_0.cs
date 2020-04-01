using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace DinoLingo
{
    public class SAS_Data_ANIMALS_0
    {
        static int labelHeight = 30 * 2;
        static Color labelColor = Color.Black;
        //static int labelOffset = 5;

        public static SAS_DataItem data = new SAS_DataItem
        {
            //     MOUSE PIG RHINO SHEEP TURTLE RABBIT TIGER OWL SNAKE MONKEY LION WHALE PARROT ZEBRA ROOSTER TURKEY SHARK SPIDER PINGUIN GORILLA OSTRICH HORSE
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
                    KeyName = "MOUSE",
                    ImgFile = "MOUSE.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                    BaseCoords = new Rect (15,52,160,160),
                    Bounds = new List<Rect> {
                        new Rect  (15,52,160,160)
                       }
                },

                new SAS_Object {
                    KeyName = "PIG",
                    ImgFile = "PIG.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (157,21,160,160),
                        Bounds = new List<Rect> {
                        new Rect  (157,21,160,160)
                        }
                    },

                new SAS_Object {
                    KeyName = "RHINO",
                    ImgFile = "RHINO.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (482,-19,200,200),
                        Bounds = new List<Rect> {
                        new Rect (482,-19,200,200)
                        }
                    },

                new SAS_Object {
                    KeyName = "SHEEP",
                    ImgFile = "SHEEP.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (750,4,160,160),
                        Bounds = new List<Rect> {
                        new Rect  (750,4,160,160)
                        }
                    },
                new SAS_Object {
                    KeyName = "TURTLE",
                    ImgFile = "TURTLE.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (925,54,160,160),
                        Bounds = new List<Rect> {
                        new Rect  (925,54,160,160)
                        }
                    },

                new SAS_Object {
                    KeyName = "RABBIT",
                    ImgFile = "RABBIT.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (1057,60,160,160),
                        Bounds = new List<Rect> {
                        new Rect  (1057,60,160,160)
                        }
                    },

                new SAS_Object {
                    KeyName = "TIGER",
                    ImgFile = "TIGER.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (339,110,200,200),
                        Bounds = new List<Rect> {
                        new Rect  (339,110,200,200)
                        }
                    },
                new SAS_Object {
                    KeyName = "OWL",
                    ImgFile = "OWL.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (578,153,160,160),
                        Bounds = new List<Rect> {
                        new Rect  (578,153,160,160)
                        }
                    },
                new SAS_Object {
                    KeyName = "SNAKE",
                    ImgFile = "SNAKE.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (715,195,160,160),
                        Bounds = new List<Rect> {
                        new Rect  (715,195,160,160)
                        }
                    },

                new SAS_Object {
                    KeyName = "MONKEY",
                    ImgFile = "MONKEY.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (882,219,160,160),
                        Bounds = new List<Rect> {
                        new Rect (882,219,160,160)
                        }
                },
                new SAS_Object {
                    KeyName = "LION",
                    ImgFile = "LION.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (1031,219,200,200),
                        Bounds = new List<Rect> {
                        new Rect  (1031,219,200,200)
                        }
                },
                new SAS_Object {
                    KeyName = "WHALE",
                    ImgFile = "WHALE.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (1,325,200,200),
                        Bounds = new List<Rect> {
                        new Rect  (1,325,200,200)
                        }
                },
                new SAS_Object {
                    KeyName = "PARROT",
                    ImgFile = "PARROT.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (181,400,160,160),
                        Bounds = new List<Rect> {
                        new Rect  (181,400,160,160)
                        }
                },
                new SAS_Object {
                    KeyName = "ZEBRA",
                    ImgFile = "ZEBRA.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (315,405,200,200),
                        Bounds = new List<Rect> {
                        new Rect  (315,405,200,200)
                        }
                },
                new SAS_Object {
                    KeyName = "ROOSTER",
                    ImgFile = "ROOSTER.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (520,336,160,160),
                        Bounds = new List<Rect> {
                        new Rect  (520,336,160,160)
                        }
                },
                new SAS_Object {
                    KeyName = "TURKEY",
                    ImgFile = "TURKEY.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (664,354,160,160),
                        Bounds = new List<Rect> {
                        new Rect  (664,354,160,160)
                        }
                },
                new SAS_Object {
                    KeyName = "SHARK",
                    ImgFile = "SHARK.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (3,535,160,160),
                        Bounds = new List<Rect> {
                        new Rect  (3,535,160,160)
                        }
                },
                new SAS_Object {
                    KeyName = "SPIDER",
                    ImgFile = "SPIDER.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (367,631,160,160),
                        Bounds = new List<Rect> {
                        new Rect  (367,631,160,160)
                        }
                },
                new SAS_Object {
                    KeyName = "PENGUIN",
                    ImgFile = "PENGUIN.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (559,585,160,160),
                        Bounds = new List<Rect> {
                        new Rect  (559,585,160,160)
                        }
                },
                new SAS_Object {
                    KeyName = "GORILLA",
                    ImgFile = "GORILLA.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (723,533,200,200),
                        Bounds = new List<Rect> {
                        new Rect  (723,533,200,200)
                        }
                },
                new SAS_Object {
                    KeyName = "OSTRICH",
                    ImgFile = "OSTRICH.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (872,523,200,200),
                        Bounds = new List<Rect> {
                        new Rect (872,523,200,200)
                        }
                },
                new SAS_Object {
                    KeyName = "HORSE",
                    ImgFile = "HORSE.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (1037,512,200,200),
                        Bounds = new List<Rect> {
                        new Rect  (1037,512,200,200)
                        }
                },

            },



            Labels = new List<SAS_Label>
            {
                new SAS_Label { 
                    KeyName = "MOUSE",
                    BaseCoords = new Rect (15,52+160,160,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "PIG",
                    BaseCoords = new Rect (157,21+160,160,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "RHINO",
                    BaseCoords = new Rect (418,18,160,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "SHEEP",
                    BaseCoords = new Rect (750,4+160,160,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "TURTLE",
                    BaseCoords = new Rect (925,54,160,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "RABBIT",
                    BaseCoords = new Rect (1057,60,160,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "TIGER",
                    BaseCoords = new Rect (339,110+200,200,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "OWL",
                    BaseCoords = new Rect (578,153+160,160,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "SNAKE",
                    BaseCoords = new Rect (715,195+160*0.85,160,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "MONKEY",
                    BaseCoords = new Rect (882,219+160,160,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "LION",
                    BaseCoords = new Rect (1031,219+200*0.9,200,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "WHALE",
                    BaseCoords = new Rect (1,325-10,200,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "PARROT",
                    BaseCoords = new Rect (181,400+160,160,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "ZEBRA",
                    BaseCoords = new Rect (315,405,200,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "ROOSTER",
                    BaseCoords = new Rect (520,336+160,160,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "TURKEY",
                    BaseCoords = new Rect (664,354+160,160,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "SHARK",
                    BaseCoords = new Rect (3,535+160*0.85,160,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "SPIDER",
                    BaseCoords = new Rect (367,631+160*0.75,160,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "PENGUIN",
                    BaseCoords = new Rect (559,585+160,160,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "GORILLA",
                    BaseCoords = new Rect (723,533+200,200,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "OSTRICH",
                    BaseCoords = new Rect (872,523+200,200,labelHeight),
                    color = labelColor
                },
                new SAS_Label {
                    KeyName = "HORSE",
                    BaseCoords = new Rect (1037,512,200,labelHeight),
                    color = labelColor
                },

            }

        };
    }
}
