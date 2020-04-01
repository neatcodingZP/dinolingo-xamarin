using System;
using System.Collections.Generic;
using Xamarin.Forms;


namespace DinoLingo
{
    public class SAS_Data
    {
        public static Dictionary<THEME_NAME, SAS_DataItem[]> all = new Dictionary<THEME_NAME, SAS_DataItem[]>
        {
            [THEME_NAME.NUMBERS] =  new SAS_DataItem [] { SAS_Data_NUMBERS.data},
            [THEME_NAME.COLORS]  =  new SAS_DataItem[] { SAS_Data_COLORS.data},
            [THEME_NAME.FOOD] = new SAS_DataItem[] { SAS_Data_FOOD.data},
            [THEME_NAME.FRUITSANDVEGETABLES] = new SAS_DataItem[] { SAS_Data_FRUITSANDVEGETABLES.data},
            [THEME_NAME.HOUSE] = new SAS_DataItem[] { SAS_Data_HOUSE.data},
            [THEME_NAME.NATURE] = new SAS_DataItem[] { SAS_Data_NATURE.data},
            [THEME_NAME.BODYPARTS] = new SAS_DataItem[] { SAS_Data_BODYPARTS.data},
            [THEME_NAME.CLOTHES] = new SAS_DataItem[] { SAS_Data_CLOTHES.data},
            [THEME_NAME.VEHICLES] = new SAS_DataItem[] { SAS_Data_VEHICLES.data},
            [THEME_NAME.ANIMALS] = new SAS_DataItem[] { SAS_Data_ANIMALS_0.data, SAS_Data_ANIMALS_1.data },

            //=========================================

        };




    }



    public class SAS_DataItem
    {
        public List<int> ids { get; set; }
        public string ImgFolder { get; set; }

        public SAS_Background Background { get; set; }
        public List<SAS_Object> ActiveImages { get; set; }
        public List<SAS_Label> Labels { get; set; }
        public List<SAS_Object> StaticImages { get; set; }

        string GetImageSource(string ImgFile)
        {
            return "DinoLingo.Resources." + ImgFolder + "." + ImgFile;
        }

        public string GetImageSourceActiveImages(int index)
        {
            return GetImageSource(ActiveImages[index].ImgFile);
        }

        public string GetImageSourceStaticImages(int index)
        {
            return GetImageSource(StaticImages[index].ImgFile);
        }

        public string GetImageSourceBackground()
        {
            return GetImageSource(Background.ImgFile);
        }
    }

    public class SAS_Background
    {
        
        public Forms9Patch.Fill Fill { get; set; }
        public Rect BaseRect { get; set; }
        public string ImgFile { get; set; }
        public Color color { get; set; }
    }


    public class SAS_Object
    {
        public string KeyName { get; set; }
        public enum ANIM_TYPE { NONE, UP_DOWN, SCALE, SWING, ROT_ANDBACK, FLASH };
        public ANIM_TYPE anim { get; set; }
        public string ImgFile { get; set; }
        public Rect BaseCoords { get; set; }
        public Color color { get; set; }
        public List<Rect> Bounds { get; set; }
        public SAS_Label InnerLabel { get; set; } // coords - relative to sas_object
        public int LayoutIndex { get; set; } = -1;
        public double anchorX { get; set; } = 0.5;
        public double anchorY { get; set; } = 0.5;
    }

    public class SAS_Label {
        public string KeyName { get; set; }
        public Rect BaseCoords { get; set; }
        public Color color { get; set; }
    }

    public struct Rect {
        public Rect (double x, double y, double width, double height) {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }
        public double x { get; set; } 
        public double y { get; set; } 
        public double width { get; set; } 
        public double height { get; set; } 
    }
}
