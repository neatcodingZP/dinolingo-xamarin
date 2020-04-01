using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace DinoLingo
{
    public class SAS_Data_FRUITSANDVEGETABLES
    {

        static int labelHeight = 30*2;
        static Color labelColor = Color.Brown;
        static int labelOffset = 5;

        public static SAS_DataItem data = new SAS_DataItem 
        {
            
            ImgFolder = "FRUITSANDVEGETABLES.SAS",
            Background = new SAS_Background
            {
                Fill = Forms9Patch.Fill.Tile,
                BaseRect = new Rect { x = -1, y = -1, width = 1224, height = 787 },
                ImgFile = "fruit_fon.png",
                color = Color.Aqua
            },
            ActiveImages = new List<SAS_Object> {
                    new SAS_Object {
                    KeyName = "PEACH",
                        ImgFile = "PEACH.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (492,559,124,140),
                        Bounds = new List<Rect> {
                            new Rect (492,559,124,140)
                        }
                    }, 

                new SAS_Object {
                    KeyName = "APPLE",
                        ImgFile = "APPLE.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (695,513,124,156),
                        Bounds = new List<Rect> {
                        new Rect  (695,513,124,156)
                        }
                    },

                new SAS_Object {
                    KeyName = "LEMON",
                        ImgFile = "LEMON.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (675,93,112,152),
                        Bounds = new List<Rect> {
                        new Rect  (675,93,112,152)
                        }
                    },
                new SAS_Object {
                    KeyName = "CARROT",
                        ImgFile = "CARROT.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (1012,8,139,280),
                        Bounds = new List<Rect> {
                        new Rect  (1012,8,139,280)
                        }
                    },
                new SAS_Object {
                    KeyName = "TOMATO",
                        ImgFile = "TOMATO.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (800,148,124,124),
                        Bounds = new List<Rect> {
                        new Rect  (800,148,124,124)
                        }
                    },
                new SAS_Object {
                    KeyName = "POTATO",
                        ImgFile = "POTATO.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (440,135,104,156),
                        Bounds = new List<Rect> {
                        new Rect  (440,135,104,156)
                        }
                    },
                new SAS_Object {
                    KeyName = "BROCCOLI",
                        ImgFile = "BROCCOLI.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (512,11,160,152),
                        Bounds = new List<Rect> {
                        new Rect  (512,11,160,152)
                        }
                    },

                new SAS_Object {
                    KeyName = "BANANA",
                        ImgFile = "BANANA.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (852,322,108,212),
                        Bounds = new List<Rect> {
                        new Rect  (852,322,108,212)
                        }
                    },
                new SAS_Object {
                    KeyName = "ORANGE",
                        ImgFile = "ORANGE.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (327,465,124,140),
                        Bounds = new List<Rect> {
                        new Rect  (327,465,124,140)
                        }
                    },
                new SAS_Object {
                    KeyName = "STRAWBERRY",
                        ImgFile = "STRAWBERRY.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (568,211,108,140),
                        Bounds = new List<Rect> {
                        new Rect  (568,211,108,140)
                        }
                    },

                new SAS_Object {
                    KeyName = "GRAPES",
                        ImgFile = "GRAPES.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (425,337,148,184),
                        Bounds = new List<Rect> {
                        new Rect  (425,337,148,184)
                        }
                    },
                new SAS_Object {
                    KeyName = "WATERMELON",
                        ImgFile = "WATERMELON.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (271,239,148,184),
                        Bounds = new List<Rect> {
                        new Rect  (271,239,148,184)
                        }
                    },
                new SAS_Object {
                    KeyName = "MELON",
                        ImgFile = "MELON.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (700,285,148,184),
                        Bounds = new List<Rect> {
                        new Rect  (700,285,148,184)
                        }
                    },
                new SAS_Object {
                    KeyName = "PEAR",
                        ImgFile = "PEAR.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (578,405,124,164),
                        Bounds = new List<Rect> {
                        new Rect  (578,405,124,164)
                        }
                    },
                new SAS_Object {
                    KeyName = "CHERRY",
                        ImgFile = "CHERRY.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (338,34,92,172),
                        Bounds = new List<Rect> {
                        new Rect  (338,34,92,172)
                        }
                    },

                },

            Labels = new List<SAS_Label>
            {
                new SAS_Label { // PEACH
                    KeyName = "PEACH",
                    BaseCoords = new Rect (492,559 + 140 + labelOffset,136,labelHeight),
                        color = labelColor
                    },

                new SAS_Label { // APPLE
                    KeyName = "APPLE",
                    BaseCoords = new Rect (695,513 + 156 + labelOffset,124,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // LEMON
                    KeyName = "LEMON",
                    BaseCoords = new Rect (675,93 + 152 + labelOffset,112,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // CARROT
                    KeyName = "CARROT",
                    BaseCoords = new Rect (1012,8 + 280  + labelOffset,139,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // TOMATO
                    KeyName = "TOMATO",
                    BaseCoords = new Rect (800,148 + 124 + labelOffset,124,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // POTATO
                    KeyName = "POTATO",
                    BaseCoords = new Rect (440,135 + 156 + labelOffset,104,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // BROCCOLI
                    KeyName = "BROCCOLI",
                    BaseCoords = new Rect (512,11 + 152 + labelOffset,160,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // BANANA
                    KeyName = "BANANA",
                    BaseCoords = new Rect (852,322 + 212 + labelOffset,108,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // ORANGE
                    KeyName = "ORANGE",
                    BaseCoords = new Rect (327,465 + 140 + labelOffset,124,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // STRAWBERRY
                    KeyName = "STRAWBERRY",
                    BaseCoords = new Rect (568,211 + 140 + labelOffset,108,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // GRAPES
                    KeyName = "GRAPES",
                    BaseCoords = new Rect (425,337 + 184 + labelOffset,148,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // WATERMELON
                    KeyName = "WATERMELON",
                    BaseCoords = new Rect (271,239 + 184 + labelOffset,148,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // MELON
                    KeyName = "MELON",
                    BaseCoords = new Rect (700,285 + 184 + labelOffset,148,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // PEAR
                    KeyName = "PEAR",
                    BaseCoords = new Rect (578,405 + 164 + labelOffset,124,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // CHERRY
                    KeyName = "CHERRY",
                    BaseCoords = new Rect (338,34 + 172 + labelOffset,92,labelHeight),
                        color = labelColor
                    },

            },

            StaticImages = new List<SAS_Object> {
                    new SAS_Object {
                        ImgFile = "fruit_dish.png",
                        BaseCoords = new Rect (204,0,816,787)
                    },
                    new SAS_Object {
                        ImgFile = "fruit_shadow.png",
                        BaseCoords = new Rect (1003,265,76,38)
                    },
                    new SAS_Object {
                        ImgFile = "fruit_rabbit.png",
                        BaseCoords = new Rect (972,311,333,485)
                    },

                }
        };
    }
}
