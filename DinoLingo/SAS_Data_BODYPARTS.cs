using System;
using System;
using System.Collections.Generic;
using Xamarin.Forms;


namespace DinoLingo
{
    public class SAS_Data_BODYPARTS
    {
        
        static Color colorBoy = Color.Red;
        static Color colorGirl = Color.Blue;

        static Rect baseRect = new Rect(0, 0, 1, 1);

        public static SAS_DataItem data = new SAS_DataItem
        {
            //                      "FINGER.png", "BODY.png", "LEGS.png", "HEAD.png", "FOOT.png", "HAIR.png", "FACE.png", "EYES.png", "EAR.png", "NOSE.png", "MOUTH.png", "ARM.png", "HAND.png"},

            ImgFolder = "BODYPARTS.SAS",
            Background = new SAS_Background
            {
                Fill = Forms9Patch.Fill.Tile,
                BaseRect = new Rect { x = -1, y = -1, width = 1200, height = 720 },
                ImgFile = "body_fon.png",
                color = Color.Transparent
            },
            ActiveImages = new List<SAS_Object> {

                new SAS_Object { // finger
                    KeyName = "FINGER",
                    ImgFile = "body_tag1.png",
                        anim = SAS_Object.ANIM_TYPE.SCALE,
                        BaseCoords = new Rect (999,491,190,60),
                        Bounds = new List<Rect> {
                        new Rect  (999,491,190,60)
                        },
                        InnerLabel = new SAS_Label { 
                            BaseCoords = baseRect,
                        color = colorBoy
                        }
                    },
                new SAS_Object { // body
                    KeyName = "BODY",
                    ImgFile = "body_tag4.png",
                    anim = SAS_Object.ANIM_TYPE.SCALE,
                        BaseCoords = new Rect (524,596,190,60),
                        Bounds = new List<Rect> {
                        new Rect  (524,596,190,60)
                        },
                        InnerLabel = new SAS_Label {
                            BaseCoords = baseRect,
                        color = Color.White
                        }
                    },
                new SAS_Object { // leg
                    KeyName = "LEG",
                    ImgFile = "body_tag1.png",
                    anim = SAS_Object.ANIM_TYPE.SCALE,
                        BaseCoords = new Rect (960,553,190,60),
                        Bounds = new List<Rect> {
                        new Rect  (960,553,190,60)
                        },
                        InnerLabel = new SAS_Label {
                            BaseCoords = baseRect,
                        color = colorBoy
                        }
                    },
                new SAS_Object { // head
                    KeyName = "HEAD",
                    ImgFile = "body_tag3.png",
                    anim = SAS_Object.ANIM_TYPE.SCALE,
                        BaseCoords = new Rect (21,36,190,60),
                        Bounds = new List<Rect> {
                        new Rect  (21,36,190,60)
                        },
                        InnerLabel = new SAS_Label {
                            BaseCoords = baseRect,
                        color = colorGirl
                        }
                    },
                new SAS_Object { // foot
                    KeyName = "FOOT",
                    ImgFile = "body_tag1.png",
                    anim = SAS_Object.ANIM_TYPE.SCALE,
                        BaseCoords = new Rect (959,612,190,60),
                        Bounds = new List<Rect> {
                        new Rect  (959,612,190,60)
                        },
                        InnerLabel = new SAS_Label {
                            BaseCoords = baseRect,
                        color = colorBoy
                        }
                    },
                new SAS_Object { // hair
                    KeyName = "HAIR",
                    ImgFile = "body_tag1.png",
                    anim = SAS_Object.ANIM_TYPE.SCALE,
                        BaseCoords = new Rect (431,408,190,60),
                        Bounds = new List<Rect> {
                        new Rect  (431,408,190,60)
                        },
                        InnerLabel = new SAS_Label {
                            BaseCoords = baseRect,
                        color = colorGirl
                        }
                    },
                new SAS_Object { // face
                    KeyName = "FACE",
                    ImgFile = "body_tag1.png",
                    anim = SAS_Object.ANIM_TYPE.SCALE,
                        BaseCoords = new Rect (436,14,190,60),
                        Bounds = new List<Rect> {
                        new Rect  (436,14,190,60)
                        },
                        InnerLabel = new SAS_Label {
                            BaseCoords = baseRect,
                        color = colorGirl
                        }
                    },
                new SAS_Object { // eyes
                    KeyName = "EYES",
                    ImgFile = "body_tag1.png",
                    anim = SAS_Object.ANIM_TYPE.SCALE,
                        BaseCoords = new Rect (480,82,190,60),
                        Bounds = new List<Rect> {
                        new Rect  (480,82,190,60)
                        },
                        InnerLabel = new SAS_Label {
                            BaseCoords = baseRect,
                        color = colorGirl
                        }
                    },
                new SAS_Object { // ear
                    KeyName = "EAR",
                    ImgFile = "body_tag1.png",
                    anim = SAS_Object.ANIM_TYPE.SCALE,
                        BaseCoords = new Rect (504,320,190,60),
                        Bounds = new List<Rect> {
                        new Rect  (504,320,190,60)
                        },
                        InnerLabel = new SAS_Label {
                            BaseCoords = baseRect,
                        color = colorGirl
                        }
                    },
                new SAS_Object { // nose
                    KeyName = "NOSE",
                    ImgFile = "body_tag2.png",
                    anim = SAS_Object.ANIM_TYPE.SCALE,
                        BaseCoords = new Rect (27,406,190,60),
                        Bounds = new List<Rect> {
                        new Rect  (27,406,190,60)
                        },
                        InnerLabel = new SAS_Label {
                            BaseCoords = baseRect,
                        color = colorGirl
                        }
                    },
                new SAS_Object { // mouth
                    KeyName = "MOUTH",
                    ImgFile = "body_tag2.png",
                    anim = SAS_Object.ANIM_TYPE.SCALE,
                        BaseCoords = new Rect (77,468,190,60),
                        Bounds = new List<Rect> {
                        new Rect  (77,468,190,60)
                        },
                        InnerLabel = new SAS_Label {
                            BaseCoords = baseRect,
                        color = colorGirl
                        }
                    },
                new SAS_Object { // arm
                    KeyName = "ARM",
                    ImgFile = "body_tag1.png",
                    anim = SAS_Object.ANIM_TYPE.SCALE,
                        BaseCoords = new Rect (983,318,190,60),
                        Bounds = new List<Rect> {
                        new Rect  (983,318,190,60)
                        },
                        InnerLabel = new SAS_Label {
                            BaseCoords = baseRect,
                        color = colorBoy
                        }
                    },
                new SAS_Object { // hand
                    KeyName = "HAND",
                    ImgFile = "body_tag1.png",
                    anim = SAS_Object.ANIM_TYPE.SCALE,
                        BaseCoords = new Rect (1003,380,190,60),
                        Bounds = new List<Rect> {
                        new Rect  (1003,380,190,60)
                        },
                        InnerLabel = new SAS_Label {
                            BaseCoords = baseRect,
                        color = colorBoy
                        }
                    },




                },





            StaticImages = new List<SAS_Object> {

                new SAS_Object {
                        ImgFile = "body_boy.png",
                        BaseCoords = new Rect (683,30,368,645),
                        LayoutIndex = -1
                    },
                new SAS_Object {
                        ImgFile = "body_circle.png",
                        BaseCoords = new Rect (112,45,407,404),
                        LayoutIndex = -1
                    },

                new SAS_Object {
                        ImgFile = "body_duga.png",
                        BaseCoords = new Rect (665,335,136,334),
                        LayoutIndex = -1
                    },

                    new SAS_Object {
                        ImgFile = "body_girl.png",
                        BaseCoords = new Rect (137,33,391,464),
                        LayoutIndex = 0

                    },


                }
        };
    }
}
