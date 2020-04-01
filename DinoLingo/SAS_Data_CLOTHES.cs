using System;
using System.Collections.Generic;
using Xamarin.Forms;


namespace DinoLingo
{
    public class SAS_Data_CLOTHES
    {
        static int labelHeight = 30 * 2;
        static Color labelColor = Color.White;
        static int labelOffset = 5;

        public static SAS_DataItem data = new SAS_DataItem
        {
            //                      "PANTS.png", "SOCKS.png", "TSHIRT.png", "SCARF.png", "GLASSES.png",  "HAT.png", "JACKET.png", "DRESS.png", "SKIRT.png", "SWEATER.png", "SHOES.png", "BOOTS.png", "GLOVES.png"

            ImgFolder = "CLOTHES.SAS",
            Background = new SAS_Background
            {
                Fill = Forms9Patch.Fill.Tile,
                BaseRect = new Rect { x = -1, y = -1, width = 1212, height = 796 },
                ImgFile = "clothes_fon.png",
                color = Color.Aqua
            },

            ActiveImages = new List<SAS_Object> {
                new SAS_Object {
                    KeyName = "PANTS",
                    ImgFile = "PANTS.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (536,431,224,168),
                        Bounds = new List<Rect> {
                        new Rect  (536,431,224,168)
                        }
                        
                    },

                new SAS_Object {
                    KeyName = "SOCKS",
                    ImgFile = "SOCKS.png",
                    anim = SAS_Object.ANIM_TYPE.SWING,
                        BaseCoords = new Rect (895,375,120,200),
                        Bounds = new List<Rect> {
                        new Rect  (895,375,120,200)
                        },
                    anchorY = 0.05
                    },

                new SAS_Object {
                    KeyName = "TSHIRT",
                    ImgFile = "TSHIRT.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (533,341,228,172),
                        Bounds = new List<Rect> {
                        new Rect (533,341,228,172)
                        }
                    },

                new SAS_Object {
                    KeyName = "SCARF",
                    ImgFile = "SCARF.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (602,306,152,140),
                        Bounds = new List<Rect> {
                        new Rect  (602,306,152,55), new Rect  (692,354,28,80)
                        }
                    },

                new SAS_Object {
                    KeyName = "GLASSES",
                    ImgFile = "GLASSES.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (1017,575,124,52),
                        Bounds = new List<Rect> {
                        new Rect  (1017,575,124,52)
                        }
                    },

                new SAS_Object {
                    KeyName = "HAT",
                    ImgFile = "HAT.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (532,82,196,84),
                        Bounds = new List<Rect> {
                        new Rect  (532,82,196,84)
                        }
                    },
                new SAS_Object {
                    KeyName = "JACKET",
                    ImgFile = "JACKET.png",
                    anim = SAS_Object.ANIM_TYPE.SWING,
                        BaseCoords = new Rect (1,2,228,236),
                        Bounds = new List<Rect> {
                        new Rect  (1,2,228,236)
                        },
                    anchorY = 0.05
                    },
                new SAS_Object {
                    KeyName = "DRESS",
                    ImgFile = "DRESS.png",
                    anim = SAS_Object.ANIM_TYPE.SWING,
                        BaseCoords = new Rect (849,6,200,348),
                        Bounds = new List<Rect> {
                        new Rect  (849,6,200,348)
                        },
                    anchorY = 0.05
                    },

                new SAS_Object {
                    KeyName = "SKIRT",
                    ImgFile = "SKIRT.png",
                        anim = SAS_Object.ANIM_TYPE.SWING,
                        BaseCoords = new Rect (135,303,200,200),
                        Bounds = new List<Rect> {
                        new Rect  (135,303,200,200)
                        },
                    anchorY = 0.05
                    },
                new SAS_Object {
                    KeyName = "SWEATER",
                    ImgFile = "SWEATER.png",
                        anim = SAS_Object.ANIM_TYPE.SWING,
                        BaseCoords = new Rect (244,5,228,228),
                        Bounds = new List<Rect> {
                        new Rect  (244,5,228,228)
                        },
                    anchorY = 0.05
                    },
                new SAS_Object {
                    KeyName = "SHOES",
                    ImgFile = "SHOES.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (303,652,132,104),
                        Bounds = new List<Rect> {
                        new Rect  (303,652,132,104)
                        }
                    },

                new SAS_Object {
                    KeyName = "BOOTS",
                    ImgFile = "BOOTS.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (78,591,132,160),
                        Bounds = new List<Rect> {
                        new Rect  (78,591,132,160)
                        }
                    },

                new SAS_Object {
                    KeyName = "GLOVES",
                    ImgFile = "GLOVES.png",
                    anim = SAS_Object.ANIM_TYPE.SWING,
                        BaseCoords = new Rect (1065,228,220,280),
                        Bounds = new List<Rect> {
                        new Rect  (1065,228,220,280)
                        },
                    anchorY = 0.05
                    },

                },



            Labels = new List<SAS_Label>
            {
                new SAS_Label { // PANTS
                    KeyName = "PANTS",
                    BaseCoords = new Rect (536 + 224 + labelOffset  ,431 + 168/2, 150, labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // SOCKS
                    KeyName = "SOCKS",
                    BaseCoords = new Rect (895,375 + 200 + labelOffset,120, labelHeight),
                    color = Color.Black
                    },
                new SAS_Label { // TSHIRT
                    KeyName = "TSHIRT",
                    BaseCoords = new Rect (533+228,341+172/2,120,labelHeight),
                        color = labelColor
                    },

                new SAS_Label { // SCARF
                    KeyName = "SCARF",
                    BaseCoords = new Rect (602+152 - labelOffset,306 - labelHeight - labelOffset, 120,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // GLASSES
                    KeyName = "GLASSES",
                    BaseCoords = new Rect (1017 + 124 + labelOffset ,575 + 52/2, 120, labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // HAT
                    KeyName = "HAT",
                    BaseCoords = new Rect (532,82 - labelOffset - labelHeight,196,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // JACKET
                    KeyName = "JACKET",
                    BaseCoords = new Rect (1,2 + 236 + labelOffset, 228, labelHeight),
                        color = labelColor
                    },

                 
                new SAS_Label { // DRESS
                    KeyName = "DRESS",
                    BaseCoords = new Rect (849,6 + 348 + labelOffset, 200, labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // SKIRT
                    KeyName = "SKIRT",
                    BaseCoords = new Rect (135,303 + 200 + labelOffset, 200, labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // SWEATER
                    KeyName = "SWEATER",
                    BaseCoords = new Rect (244,5 + 228 + labelOffset, 228, labelHeight),
                        color = labelColor 
                    },

                new SAS_Label { // SHOES
                    KeyName = "SHOES",
                    BaseCoords = new Rect (303,652 + 104 + labelOffset, 132, labelHeight),
                    color = Color.Black
                    },
                new SAS_Label { // BOOTS
                    KeyName = "BOOTS",
                    BaseCoords = new Rect (78,591 + 160 + labelOffset, 132, labelHeight),
                    color = Color.Black
                    },
                new SAS_Label { // GLOVES
                    KeyName = "GLOVES",
                    BaseCoords = new Rect (1065,228 + 280 + labelOffset,220, labelHeight),
                        color = labelColor
                    },

            },

            StaticImages = new List<SAS_Object> {
                new SAS_Object {
                        ImgFile = "clothes_floor.png",
                        BaseCoords = new Rect (-200,583,1212+400,1000),
                        LayoutIndex = -1
                    },    
                new SAS_Object {
                        ImgFile = "clothes_dog.png",
                        BaseCoords = new Rect (1016,533,192,260),
                        LayoutIndex = -1
                    },
                    new SAS_Object {
                        ImgFile = "clothes_gesha3.png",
                        BaseCoords = new Rect (359,115,462,572),
                        LayoutIndex = -1
                    },
                new SAS_Object {
                        ImgFile = "clothes_gesha_head.png",
                        BaseCoords = new Rect (492,113,245,246),
                        LayoutIndex = 3
                    },


                }
        };
    }
}
