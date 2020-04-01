using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace DinoLingo
{
    public class SAS_Data_NUMBERS
    {
        static int labelHeight = 30;

        public static SAS_DataItem data = new SAS_DataItem
        {
            
            ImgFolder = "NUMBERS.SAS",
            Background = new SAS_Background
            {
                Fill = Forms9Patch.Fill.Tile,
                BaseRect = new Rect { x = -1, y = -1, width = 1000, height = 720 },
                ImgFile = "FON.png",
                color = Color.Aqua
            },
            ActiveImages = new List<SAS_Object> {
                    new SAS_Object {
                    KeyName = "ONE",
                        ImgFile = "ONE.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (14,93,136,156),
                        Bounds = new List<Rect> {
                            new Rect (14,93,136,156)
                        }
                    },
                new SAS_Object {
                    KeyName = "TWO",
                        ImgFile = "TWO.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (196,88,136,156),
                        Bounds = new List<Rect> {
                        new Rect (196,88,136,156)
                        }
                    },

                new SAS_Object {
                    KeyName = "THREE",
                        ImgFile = "THREE.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (406,85,136,156),
                        Bounds = new List<Rect> {
                            new Rect (406,85,136,156)
                        }
                    },
                new SAS_Object {
                    KeyName = "FOUR",
                        ImgFile = "FOUR.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (604,84,136,156),
                        Bounds = new List<Rect> {
                            new Rect (604,84,136,156)
                        }
                    },
                new SAS_Object {
                    KeyName = "FIVE",
                        ImgFile = "FIVE.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (833,92,136,156),
                        Bounds = new List<Rect> {
                            new Rect (833,92,136,156)
                        }
                    },
                new SAS_Object {
                    KeyName = "SIX",
                        ImgFile = "SIX.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (14,437,136,156),
                        Bounds = new List<Rect> {
                            new Rect (14,437,136,156)
                        }
                    },
                new SAS_Object {
                    KeyName = "SEVEN",
                        ImgFile = "SEVEN.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (203,429,136,156),
                        Bounds = new List<Rect> {
                            new Rect (203,429,136,156)
                        }
                    },
                new SAS_Object {
                    KeyName = "EIGHT",
                        ImgFile = "EIGHT.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (402,425,136,156),
                        Bounds = new List<Rect> {
                            new Rect (402,425,136,156)
                        }
                    },

                new SAS_Object {
                    KeyName = "NINE",
                        ImgFile = "NINE.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (604,425,136,156),
                        Bounds = new List<Rect> {
                            new Rect (604,425,136,156)
                        }
                    },
                new SAS_Object {
                    KeyName = "TEN",
                        ImgFile = "TEN.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (781,436,220,140),
                        Bounds = new List<Rect> {
                        new Rect (781,436,220,140)
                        }
                    },


                },

            Labels = new List<SAS_Label>
            {
                
                new SAS_Label {
                    KeyName = "ONE",
                        BaseCoords = new Rect (14,53,136,labelHeight),
                        color = Color.Brown
                    },
                new SAS_Label { // 2
                    KeyName = "TWO",
                    BaseCoords = new Rect (196,48,136,labelHeight),
                        color = Color.Brown
                    },

                new SAS_Label { // 3
                    KeyName = "THREE",
                    BaseCoords = new Rect (406,45,136,labelHeight),
                        color = Color.Brown
                    },
                new SAS_Label { // 4
                    KeyName = "FOUR",
                    BaseCoords = new Rect (604,44,136,labelHeight),
                        color = Color.Brown
                    },
                new SAS_Label { // 5
                    KeyName = "FIVE",
                    BaseCoords = new Rect (833,52,136,labelHeight),
                        color = Color.Brown
                    },
                new SAS_Label { // 6
                    KeyName = "SIX",
                    BaseCoords = new Rect (14,397,136,labelHeight),
                        color = Color.Brown
                    },
                new SAS_Label { // 7
                    KeyName = "SEVEN",
                    BaseCoords = new Rect (203,389,136,labelHeight),
                        color = Color.Brown
                    },
                new SAS_Label { // 8
                    KeyName = "EIGHT",
                    BaseCoords = new Rect (402,385,136,labelHeight),
                        color = Color.Brown
                    },
                new SAS_Label { // 9
                    KeyName = "NINE",
                    BaseCoords = new Rect (604,385,136,labelHeight),
                        color = Color.Brown
                    },
                new SAS_Label { // 10
                    KeyName = "TEN",
                    BaseCoords = new Rect (781,396,220,labelHeight),
                        color = Color.Brown
                    },
                    
            },

            StaticImages = new List<SAS_Object> {
                    new SAS_Object {
                        ImgFile = "ONE_CANDIES.png",
                        BaseCoords = new Rect (53,254,64,42)
                    },

                new SAS_Object {
                        ImgFile = "TWO_CANDIES.png",
                        BaseCoords = new Rect (205,255,114,42)
                    },
                new SAS_Object {
                        ImgFile = "THREE_CANDIES.png",
                        BaseCoords = new Rect (389,251,164,42)
                    },
                new SAS_Object {
                        ImgFile = "FOUR_CANDIES.png",
                        BaseCoords = new Rect (629,245,120,80)
                    },
                new SAS_Object {
                        ImgFile = "FIVE_CANDIES.png",
                        BaseCoords = new Rect (809,248,180,80)
                    },
                new SAS_Object {
                        ImgFile = "SIX_CANDIES.png",
                        BaseCoords = new Rect (2,600,180,80)
                    },
                new SAS_Object {
                        ImgFile = "SEVEN_CANDIES.png",
                        BaseCoords = new Rect (183,595,160,120)
                    },
                new SAS_Object {
                        ImgFile = "EIGHT_CANDIES.png",
                        BaseCoords = new Rect (372,597,180,120)
                    },
                new SAS_Object {
                        ImgFile = "NINE_CANDIES.png",
                        BaseCoords = new Rect (573,594,180,120)
                    },
                new SAS_Object {
                        ImgFile = "TEN_CANDIES.png",
                        BaseCoords = new Rect (769,599,220,120)
                    },
                    
                }
        };
    }
}
