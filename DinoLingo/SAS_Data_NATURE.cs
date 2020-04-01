using System;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace DinoLingo
{
    public class SAS_Data_NATURE
    {
        static int labelHeight = 30 * 2;
        static Color labelColor = Color.Black;
        //static int labelOffset = 5;

        public static SAS_DataItem data = new SAS_DataItem
        {
            //                      "SKY.png", "RAINBOW.png",  "SUN.png", "ROCK.png", "GRASS.png", "FLOWER.png", "RAIN.png", "SNOW.png", "MOON.png", "STAR.png",  "CLOUD.png", "SEA.png", "FOREST.png", "TREE.png"
                     
            ImgFolder = "NATURE.SAS",
            Background = new SAS_Background
            {
                Fill = Forms9Patch.Fill.Fill,
                BaseRect = new Rect { x = -1, y = -1, width = 1172, height = 680 },
                color = Color.Aqua
            },
            ActiveImages = new List<SAS_Object> {
               
                new SAS_Object {
                    KeyName = "SKY",
                    ImgFile = "SKY.png",
                        anim = SAS_Object.ANIM_TYPE.FLASH,
                        BaseCoords = new Rect (0,0,615,504),
                        Bounds = new List<Rect> {
                        new Rect  (0,0,615,350)
                        }
                    },

                new SAS_Object {
                    KeyName = "RAINBOW",
                    ImgFile = "RAINBOW.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (28,45,588,424),
                        Bounds = new List<Rect> {
                        new Rect  (102,289,120,70), new Rect  (240,137,105,100), new Rect  (290,100,115,107), new Rect  (369,55,245,115)
                        }
                    },



                new SAS_Object {
                    KeyName = "SUN",
                    ImgFile = "SUN.png",
                    anim = SAS_Object.ANIM_TYPE.ROT_ANDBACK,
                        BaseCoords = new Rect (19,-4,208,209),
                        Bounds = new List<Rect> {
                        new Rect  (19+50,-4,208-50-25,209-25)
                        }
                    },

                new SAS_Object {
                    KeyName = "ROCK",
                    ImgFile = "ROCK.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (439,515,181,165),
                        Bounds = new List<Rect> {
                        new Rect  (439,515,181,165)
                        }
                    },

                new SAS_Object {
                    KeyName = "GRASS",
                    ImgFile = "GRASS.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (0,525,615,155),
                        Bounds = new List<Rect> {
                        new Rect (0,550,450,130)
                        }
                    },
                new SAS_Object {
                    KeyName = "FLOWER",
                    ImgFile = "FLOWER.png",
                    anim = SAS_Object.ANIM_TYPE.ROT_ANDBACK,
                        BaseCoords = new Rect (91,508,64,64),
                        Bounds = new List<Rect> {
                        new Rect  (91,508,64,64)
                        }
                    },
                new SAS_Object {
                    KeyName = "RAIN",
                    ImgFile = "RAIN.png",
                    anim = SAS_Object.ANIM_TYPE.SCALE,
                        BaseCoords = new Rect (640,233,254,212),
                        Bounds = new List<Rect> {
                        new Rect  (640,233,254,212)
                        }
                    },

                new SAS_Object {
                    KeyName = "SNOW",
                    ImgFile = "SNOW.png",
                    anim = SAS_Object.ANIM_TYPE.SCALE,
                        BaseCoords = new Rect (916,233,254,212),
                        Bounds = new List<Rect> {
                        new Rect  (916,233,254,212)
                        }
                    },



                new SAS_Object {
                    KeyName = "MOON",
                    ImgFile = "MOON.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (733,20,112,132),
                        Bounds = new List<Rect> {
                        new Rect  (733,20,112,132)
                        }
                    },

                new SAS_Object {
                    KeyName = "STAR",
                    ImgFile = "STAR.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (1023,54,72,72),
                        Bounds = new List<Rect> {
                        new Rect  (1023,54,72,72)
                        }
                    },
               
                new SAS_Object {
                    KeyName = "CLOUD",
                    ImgFile = "CLOUD.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (73,182,245,126),
                        Bounds = new List<Rect> {
                        new Rect  (73+50,182,245-75,126)
                        }
                    },

                new SAS_Object {
                    KeyName = "SEA",
                    ImgFile = "SEA.png",
                    anim = SAS_Object.ANIM_TYPE.SCALE,
                        BaseCoords = new Rect (640,468,254,212),
                        Bounds = new List<Rect> {
                        new Rect  (640,468,254,212)
                        }
                    },

                new SAS_Object {
                    KeyName = "FOREST",
                    ImgFile = "FOREST.png",
                    anim = SAS_Object.ANIM_TYPE.SCALE,
                        BaseCoords = new Rect (916,468,254,212),
                        Bounds = new List<Rect> {
                        new Rect  (916,468,254,212)
                        }
                    },

                new SAS_Object {
                    KeyName = "TREE",
                    ImgFile = "TREE.png",
                    anim = SAS_Object.ANIM_TYPE.SCALE,
                        BaseCoords = new Rect (247,260,368,292),
                        Bounds = new List<Rect> {
                        new Rect  (247+20,260+20,368-20,292-40)
                        }
                    },

                },



            Labels = new List<SAS_Label>
            {
                new SAS_Label { // SKY
                    KeyName = "SKY",
                    BaseCoords = new Rect (320,20,150,labelHeight),
                        color = labelColor
                    },

                new SAS_Label { // RAINBOW
                    KeyName = "RAINBOW",
                    BaseCoords = new Rect (470,18,150,labelHeight),
                        color = labelColor
                    },


                new SAS_Label { // SUN
                    KeyName = "SUN",
                    BaseCoords = new Rect (190,80,150,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // ROCK
                    KeyName = "ROCK",
                    BaseCoords = new Rect (485,502,150,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // GRASS
                    KeyName = "GRASS",
                    BaseCoords = new Rect (232,634,150,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // FLOWER
                    KeyName = "FLOWER",
                    BaseCoords = new Rect (150,495,150,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // RAIN
                    KeyName = "RAIN",
                    BaseCoords = new Rect (640,242,150,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // SNOW
                    KeyName = "SNOW",
                    BaseCoords = new Rect (1030,242,150,labelHeight),
                        color = labelColor
                    },


                new SAS_Label { // MOON
                    KeyName = "MOON",
                    BaseCoords = new Rect (800,84,150,labelHeight),
                    color = Color.White
                    },
                new SAS_Label { // STAR
                    KeyName = "STAR",
                    BaseCoords = new Rect (1010,22,150,labelHeight),
                    color = Color.White
                    },
               
                new SAS_Label { // CLOUD
                    KeyName = "CLOUD",
                    BaseCoords = new Rect (80,217,150,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // SEA
                    KeyName = "SEA",
                    BaseCoords = new Rect (770,478,150,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // FOREST
                    KeyName = "FOREST",
                    BaseCoords = new Rect (1030,478,150,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // TREE
                    KeyName = "TREE",
                    BaseCoords = new Rect (360,237,150,labelHeight),
                        color = labelColor
                    },
            },

            StaticImages = new List<SAS_Object> {
                    
                new SAS_Object {
                        ImgFile = "white.png",
                        BaseCoords = new Rect (0,0,615,504),
                        LayoutIndex = -1
                    },
                new SAS_Object {
                        ImgFile = "nature_land.png",
                        BaseCoords = new Rect (0,281,615,399),
                        LayoutIndex = 1
                    },

                new SAS_Object {
                        ImgFile = "nature_flower_trunk.png",
                        BaseCoords = new Rect (117,536,28,120),
                        LayoutIndex = 4
                    },

                    new SAS_Object {
                        ImgFile = "nature_night.png",
                        BaseCoords = new Rect (640,0,530,212),
                        LayoutIndex = 0

                    },
                new SAS_Object {
                        ImgFile = "nature_night_forest.png",
                        BaseCoords = new Rect (645,116,530,100),
                        LayoutIndex = 100
                    },
                new SAS_Object {
                        ImgFile = "nature_cloud1.png",
                        BaseCoords = new Rect (0,5,124,105),
                        LayoutIndex = 100
                    },


                }
        };
    }
}
