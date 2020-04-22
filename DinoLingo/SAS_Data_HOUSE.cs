using System;
using System.Collections.Generic;
using Xamarin.Forms;


namespace DinoLingo
{
    public class SAS_Data_HOUSE
    {
        static int labelHeight = 30 * 2;
        static Color labelColor = Color.Black;
        static int labelOffset = 2;

        public static SAS_DataItem data = new SAS_DataItem
        {
            //                      "BED.png",    "DOOR.png",      "TABLE.png", "BALL.png", "CHAIR.png",  "TELEVISION.png", "COMPUTER.png", "TELEPHONE.png", "CLOCK.png", "LAMP.png", "KITE.png", "BALLOON.png", "BOX.png", "TOYS.png", "BOOK.png", "PICTURE.png",  "WINDOW.png"  BAG
                       
            ImgFolder = "HOUSE.SAS",
            Background = new SAS_Background
            {
                Fill = Forms9Patch.Fill.Fill,
                BaseRect = new Rect { x = -1, y = -1, width = 1225, height = 792 },
                ImgFile = "house_fon.png",
                color = Color.Aqua
            },
            ActiveImages = new List<SAS_Object> {
                new SAS_Object {
                    KeyName = "BED",
                    ImgFile = "BED.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (555,249,483,358),
                        Bounds = new List<Rect> {
                        new Rect  (605,335,425,165)
                        }
                    },

                new SAS_Object {
                    KeyName = "DOOR",
                    ImgFile = "DOOR.png",
                    anim = SAS_Object.ANIM_TYPE.SCALE,
                        BaseCoords = new Rect (7,87,216,524),
                        Bounds = new List<Rect> {
                        new Rect  (7,87,216,524)
                        }
                    },

                new SAS_Object {
                    KeyName = "TABLE",
                    ImgFile = "TABLE.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (105,536,280,172),
                        Bounds = new List<Rect> {
                            new Rect (135,536,250,55)
                        }
                    },

                new SAS_Object {
                    KeyName = "BALL",
                    ImgFile = "BALL.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (494,563,76,76),
                        Bounds = new List<Rect> {
                        new Rect  (494,563,76,76)
                        }
                    },

                new SAS_Object {
                    KeyName = "CHAIR",
                    ImgFile = "CHAIR.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (76,521,370,224),
                        Bounds = new List<Rect> {
                        new Rect  (76,521,70,224), new Rect  (140,607,100,120), new Rect  (326,600,120,120), new Rect  (424,521,20,224)
                        }
                    },

                new SAS_Object {
                    KeyName = "TELEVISION",
                    ImgFile = "TELEVISION.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (356,242,180,168),
                        Bounds = new List<Rect> {
                        new Rect  (356,242,180,168)
                        }
                    },
                new SAS_Object {
                    KeyName = "COMPUTER",
                    ImgFile = "COMPUTER.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (751,296,128,128),
                        Bounds = new List<Rect> {
                        new Rect  (751,296,128,128)
                        }
                    },
                new SAS_Object {
                    KeyName = "TELEPHONE",
                    ImgFile = "TELEPHONE.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (235,493,98,68),
                        Bounds = new List<Rect> {
                        new Rect  (235,493,98,68)
                        }
                    },

                new SAS_Object {
                    KeyName = "CLOCK",
                    ImgFile = "CLOCK.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (382,22,128,128),
                        Bounds = new List<Rect> {
                        new Rect  (382,22,128,128)
                        }
                    },
                new SAS_Object {
                    KeyName = "LAMP",
                    ImgFile = "LAMP.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (258,166,84,276),
                        Bounds = new List<Rect> {
                        new Rect  (258,166,84,276)
                        }
                    },
                new SAS_Object {
                    KeyName = "KITE",
                    ImgFile = "KITE.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (933,503,112,192),
                        Bounds = new List<Rect> {
                        new Rect  (933,503,112,192)
                        }
                    },

                new SAS_Object {
                    KeyName = "BALLOON",
                    ImgFile = "BALLOON.png",
                    anim = SAS_Object.ANIM_TYPE.SCALE,
                        BaseCoords = new Rect (1048,155,131,166),
                        Bounds = new List<Rect> {
                        new Rect  (1048,155,131,166)
                        }
                    },

                new SAS_Object {
                    KeyName = "BOX",
                    ImgFile = "BOX.png",
                    anim = SAS_Object.ANIM_TYPE.SCALE,
                        BaseCoords = new Rect (753,505,124,156),
                        Bounds = new List<Rect> {
                        new Rect  (753,505,124,16), new Rect  (753,598,124,63)
                        }
                    },

                new SAS_Object {
                    KeyName = "TOYS",
                    ImgFile = "TOYS.png",
                    anim = SAS_Object.ANIM_TYPE.SCALE,
                        BaseCoords = new Rect (753,526,140,72),
                        Bounds = new List<Rect> {
                        new Rect  (753,526,140,72)
                        }
                    },

                new SAS_Object {
                    KeyName = "BOOK",
                    ImgFile = "BOOK.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (583,578,144,100),
                        Bounds = new List<Rect> {
                        new Rect  (583,578,144,100)
                        }
                    },
                new SAS_Object {
                    KeyName = "PICTURE",
                    ImgFile = "PICTURE.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (882,32,152,180),
                        Bounds = new List<Rect> {
                        new Rect  (882,32,152,180)
                        }
                    },

                new SAS_Object {
                    KeyName = "WINDOW",
                    ImgFile = "WINDOW.png",
                    anim = SAS_Object.ANIM_TYPE.SCALE,
                        BaseCoords = new Rect (636,7,188,260),
                        Bounds = new List<Rect> {
                        new Rect  (636,7,188,260)
                        }
                    },

                new SAS_Object {
                    KeyName = "BAG",
                    ImgFile = "BAG.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (529,404,124,144),
                        Bounds = new List<Rect> {
                        new Rect  (529,404,124,144)
                        }
                    },

                },



            Labels = new List<SAS_Label>
            {
                new SAS_Label { // BED
                    KeyName = "BED",
                    BaseCoords = new Rect (605,335,150,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // DOOR
                    KeyName = "DOOR",
                    BaseCoords = new Rect (7 ,87- labelHeight,216,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // TABLE
                    KeyName = "TABLE",
                    BaseCoords = new Rect (135,536+55+labelOffset,250,labelHeight),
                        color = labelColor
                    },

                new SAS_Label { // BALL
                    KeyName = "BALL",
                    BaseCoords = new Rect (494,563+76+labelOffset,76,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // CHAIR
                    KeyName = "CHAIR",
                    BaseCoords = new Rect (76,521+224+labelOffset,370*0.5,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // TELEVISION
                    KeyName = "TELEVISION",
                    BaseCoords = new Rect (356,242+168+labelOffset,180,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // COMPUTER
                    KeyName = "COMPUTER",
                    BaseCoords = new Rect (751,296+128+labelOffset,128 * 2, labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // TELEPHONE
                    KeyName = "TELEPHONE",
                    BaseCoords = new Rect (235-125,493 -labelHeight ,250,labelHeight),
                        color = labelColor
                    },

                //" ".png", ".png", ".png", ".png", ".png", ".png", ".png", ".png",  ".png"  
                new SAS_Label { // CLOCK
                    KeyName = "CLOCK",
                    BaseCoords = new Rect (382,22+128+labelOffset,180,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // LAMP
                    KeyName = "LAMP",
                    BaseCoords = new Rect (258 + 10,166 + 276 + labelOffset,84*2,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // KITE
                    KeyName = "KITE",
                    BaseCoords = new Rect (933,503+192+labelOffset,160,labelHeight),
                        color = labelColor
                    },

                new SAS_Label { // BALLOON
                    KeyName = "BALLOON",
                    BaseCoords = new Rect (1048,155 - labelHeight,131,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // BOX
                    KeyName = "BOX",
                    BaseCoords = new Rect (753,505+156+labelOffset,124,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // TOYS
                    KeyName = "TOYS",
                    BaseCoords = new Rect (753+140,526 + 72*0.0,100,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // BOOK
                    KeyName = "BOOK",
                    BaseCoords = new Rect (583,578+100+labelOffset,144,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // PICTURE
                    KeyName = "PICTURE",
                    BaseCoords = new Rect (882,32+180+labelOffset,152,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // WINDOW
                    KeyName = "WINDOW",
                    BaseCoords = new Rect (636,7+260+labelOffset,188,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // BAG
                    KeyName = "BAG",
                    BaseCoords = new Rect (529 - 150,404+144*0.5,150,labelHeight),
                        color = labelColor
                    },

            },

            StaticImages = new List<SAS_Object> {
                    new SAS_Object {
                        ImgFile = "house_boy.png",
                        BaseCoords = new Rect (975,297,200,392),
                        LayoutIndex = 0
                    },
                    new SAS_Object {
                        ImgFile = "BOY_HAND.png",
                        BaseCoords = new Rect (970,528,28,36),
                        LayoutIndex = 100
                        
                    },

                }
        };
    }
}
