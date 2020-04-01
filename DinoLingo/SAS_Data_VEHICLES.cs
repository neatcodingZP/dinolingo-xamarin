using System;
using System.Collections.Generic;
using Xamarin.Forms;


namespace DinoLingo
{
    public class SAS_Data_VEHICLES
    {
        static int labelHeight = 30 * 2;
        static Color labelColor = Color.Black;
       // static int labelOffset = 5;

        public static SAS_DataItem data = new SAS_DataItem
        {
            //                      "TRAIN.png", "ROCKET.png", "BICYCLE.png", "MOTORCYCLE.png", "CAR.png", "BUS.png", "TRUCK.png",  "HELICOPTER.png", "AIRPLANE.png", "BOAT.png"
            ImgFolder = "VEHICLES.SAS",
            Background = new SAS_Background
            {
                Fill = Forms9Patch.Fill.Fill,
                BaseRect = new Rect { x = -1, y = -1, width = 1225, height = 795 },
                ImgFile = "vehicles_fon2.png",
                color = Color.Aqua
            },

            ActiveImages = new List<SAS_Object> {
                new SAS_Object {
                    KeyName = "TRAIN",
                    ImgFile = "vehicles_train2.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (210,68,1005,411),
                        Bounds = new List<Rect> {
                            new Rect  (233,161,351,194), new Rect  (578,229,342,188)
                        }
                    },

                new SAS_Object {
                    KeyName = "ROCKET",
                    ImgFile = "vehicles_rocket.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (37,29,136,237),
                        Bounds = new List<Rect> {
                        new Rect  (37,29,136,237)
                        }
                    },

                new SAS_Object {
                    KeyName = "BICYCLE",
                    ImgFile = "vehicles_bicycle2.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (605,546,246,147),
                        Bounds = new List<Rect> {
                        new Rect (605,546,246,147)
                        }
                    },

                new SAS_Object {
                    KeyName = "MOTORCYCLE",
                    ImgFile = "vehicles_motorcycle2.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (868,541,283,187),
                        Bounds = new List<Rect> {
                        new Rect  (868,541,283,187)
                        }
                    },

                new SAS_Object {
                    KeyName = "CAR",
                    ImgFile = "vehicles_car2.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (236,514,231,196),
                        Bounds = new List<Rect> {
                        new Rect  (236,514,231,196)
                        }
                    },

                new SAS_Object {
                    KeyName = "BUS",
                    ImgFile = "vehicles_bus2.png",
                        anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (338,358,262,155),
                        Bounds = new List<Rect> {
                        new Rect  (338,358,262,155)
                        }
                    },
                new SAS_Object {
                    KeyName = "TRUCK",
                    ImgFile = "vehicles_truck2.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (609,371,226,139),
                        Bounds = new List<Rect> {
                        new Rect  (609,371,226,139)
                        }
                    },
                new SAS_Object {
                    KeyName = "HELICOPTER",
                    ImgFile = "vehicles_helicopter.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (402,2,240,217),
                        Bounds = new List<Rect> {
                        new Rect  (402,2,240,217)
                        }
                    },

                new SAS_Object {
                    KeyName = "AIRPLANE",
                    ImgFile = "vehicles_airplane.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (709,4,378,264),
                        Bounds = new List<Rect> {
                        new Rect (709,4,378,264)
                        }
                },
                new SAS_Object {
                    KeyName = "BOAT",
                    ImgFile = "vehicles_boat2.png",
                    anim = SAS_Object.ANIM_TYPE.UP_DOWN,
                        BaseCoords = new Rect (42,304,232,227),
                        Bounds = new List<Rect> {
                        new Rect  (42,304,232,227)
                        }
                    },


                },



            Labels = new List<SAS_Label>
            {
                new SAS_Label { // TRAIN
                    KeyName = "TRAIN",
                    BaseCoords = new Rect (321,330,150,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // ROCKET
                    KeyName = "ROCKET",
                    BaseCoords = new Rect (37,237,136,labelHeight),
                    color = Color.Black
                    },
                new SAS_Label { // BICYCLE
                    KeyName = "BICYCLE",
                    BaseCoords = new Rect (605,546+147,246,labelHeight),
                        color = labelColor
                    },

                new SAS_Label { // MOTORCYCLE
                    KeyName = "MOTORCYCLE",
                    BaseCoords = new Rect (868,541+187,283,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // CAR
                    KeyName = "CAR",
                    BaseCoords = new Rect (236,546+157,150,labelHeight),

                     color = labelColor
                    },
                new SAS_Label { // BUS
                    KeyName = "BUS",
                    BaseCoords = new Rect (338,358+155,262,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // TRUCK
                    KeyName = "TRUCK",
                    BaseCoords = new Rect (609,371+139,226,labelHeight),
                        color = labelColor
                    },


                new SAS_Label { // HELICOPTER
                    KeyName = "HELICOPTER",
                    BaseCoords = new Rect (581,149,175,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // AIRPLANE
                    KeyName = "AIRPLANE",
                    BaseCoords = new Rect (709,4 + 264*0.8,378,labelHeight),
                        color = labelColor
                    },
                new SAS_Label { // BOAT
                    KeyName = "BOAT",
                    BaseCoords = new Rect (42,304+227,232,labelHeight),
                        color = labelColor
                    }
            },

            StaticImages = new List<SAS_Object> {
                new SAS_Object {
                        ImgFile = "vehicles_house.png",
                        BaseCoords = new Rect (774,199,451,459),
                        LayoutIndex = 0
                    },
                new SAS_Object {
                    ImgFile = "vehicles_kust.png",
                        BaseCoords = new Rect (450,607,178,163),
                        LayoutIndex = 100
                    },
                }
        };
    }
}
