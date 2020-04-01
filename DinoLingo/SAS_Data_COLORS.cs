using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace DinoLingo
{
    public class SAS_Data_COLORS
    {


        public static SAS_DataItem data = new SAS_DataItem
        {
            
            ImgFolder = "COLORS.SAS",
            Background = new SAS_Background
            {
                Fill = Forms9Patch.Fill.Fill,
                BaseRect = new Rect { x = -1, y = -1, width = 1222, height = 790 },
                ImgFile = "color_fon.jpg",
                color = Color.Aqua
            },
            ActiveImages = new List<SAS_Object> {
                    new SAS_Object {
                    KeyName = "BROWN",
                        ImgFile = "BROWN.png",
                        anim = SAS_Object.ANIM_TYPE.SCALE,
                        BaseCoords = new Rect (49,186,150,186),
                        Bounds = new List<Rect> {
                            new Rect (49,186,150,186)
                        },
                        InnerLabel = new SAS_Label {
                        BaseCoords = new Rect (0,0.3,1,0.2),
                        color = Color.White
                        },
                        anchorY = 0.9
                        
                    },

                new SAS_Object {
                    KeyName = "RED",
                        ImgFile = "RED.png",
                        anim = SAS_Object.ANIM_TYPE.SCALE,
                        BaseCoords = new Rect (288,315,150,186),
                        Bounds = new List<Rect> {
                            new Rect (288,315,150,186)
                        },
                        InnerLabel = new SAS_Label {
                        BaseCoords = new Rect (0,0.3,1,0.2),
                        color = Color.White
                        },
                    anchorY = 0.9
                    },

                new SAS_Object {
                    KeyName = "GREEN",
                        ImgFile = "GREEN.png",
                        anim = SAS_Object.ANIM_TYPE.SCALE,
                        BaseCoords = new Rect (488,232,150,186),
                        Bounds = new List<Rect> {
                            new Rect (488,232,150,186)
                        },
                        InnerLabel = new SAS_Label {
                        BaseCoords = new Rect (0,0.3,1,0.2),
                        color = Color.White
                        },
                    anchorY = 0.9
                    },

                new SAS_Object {
                    KeyName = "YELLOW",
                        ImgFile = "YELLOW.png",
                        anim = SAS_Object.ANIM_TYPE.SCALE,
                        BaseCoords = new Rect (640,98,150,186),
                        Bounds = new List<Rect> {
                            new Rect (640,98,150,186)
                        },
                        InnerLabel = new SAS_Label {
                        BaseCoords = new Rect (0,0.3,1,0.2),
                        color = Color.White
                        },
                    anchorY = 0.9
                    },

                new SAS_Object {
                    KeyName = "BLUE",
                        ImgFile = "BLUE.png",
                        anim = SAS_Object.ANIM_TYPE.SCALE,
                        BaseCoords = new Rect (406,45,150,186),
                        Bounds = new List<Rect> {
                            new Rect (406,45,150,186)
                        },
                        InnerLabel = new SAS_Label {
                        BaseCoords = new Rect (0,0.3,1,0.2),
                        color = Color.White
                        },
                    anchorY = 0.9
                    },

                new SAS_Object {
                    KeyName = "PURPLE",
                        ImgFile = "PURPLE.png",
                        anim = SAS_Object.ANIM_TYPE.SCALE,
                        BaseCoords = new Rect (798,193,150,186),
                        Bounds = new List<Rect> {
                            new Rect (798,193,150,186)
                        },
                        InnerLabel = new SAS_Label {
                        BaseCoords = new Rect (0,0.3,1,0.2),
                        color = Color.White
                        },
                    anchorY = 0.9
                    },

                new SAS_Object {
                    KeyName = "ORANGE",
                        ImgFile = "ORANGE.png",
                        anim = SAS_Object.ANIM_TYPE.SCALE,
                        BaseCoords = new Rect (204,86,150,186),
                        Bounds = new List<Rect> {
                            new Rect (204,86,150,186)
                        },
                        InnerLabel = new SAS_Label {
                        BaseCoords = new Rect (0,0.3,1,0.2),
                        color = Color.Brown
                        },
                    anchorY = 0.9
                    },

                new SAS_Object {
                    KeyName = "PINK",
                        ImgFile = "PINK.png",
                        anim = SAS_Object.ANIM_TYPE.SCALE,
                        BaseCoords = new Rect (1029,361,150,186),
                        Bounds = new List<Rect> {
                            new Rect (1029,361,150,186)
                        },
                        InnerLabel = new SAS_Label {
                        BaseCoords = new Rect (0,0.3,1,0.2),
                        color = Color.White
                        },
                    anchorY = 0.9
                    },

                new SAS_Object {
                    KeyName = "BLACK",
                        ImgFile = "BLACK.png",
                        anim = SAS_Object.ANIM_TYPE.SCALE,
                        BaseCoords = new Rect (904,55,150,186),
                        Bounds = new List<Rect> {
                            new Rect (904,55,150,186)
                        },
                        InnerLabel = new SAS_Label {
                        BaseCoords = new Rect (0,0.3,1,0.2),
                        color = Color.White
                        },
                    anchorY = 0.9
                    },

                new SAS_Object {
                    KeyName = "WHITE",
                        ImgFile = "WHITE.png",
                        anim = SAS_Object.ANIM_TYPE.SCALE,
                        BaseCoords = new Rect (620,329,150,186),
                        Bounds = new List<Rect> {
                            new Rect (620,329,150,186)
                        },
                        InnerLabel = new SAS_Label {
                        BaseCoords = new Rect (0,0.3,1,0.2),
                        color = Color.Brown
                        },
                    anchorY = 0.9
                    },
            
            },

        };
    }
}
