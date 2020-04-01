using System;
namespace DinoLingo
{
    public class ListViewItemData
    {

        public enum VISUAL_TYPE { HEADER, SINGLE, DOUBLE, SPACE, HEADER_WITH_IMAGE };
        public VISUAL_TYPE Type { get; set; }
        public string id { get; set; }
        public string Name { get; set; }
        public string GameType { get; set; }
        public string TransName { get; set; }
        public string ImgResource { get; set; }
        public int Stars { get; set; }
        //public string Action { get; set; }

        //additional data
        //public THEME_NAME Theme { get; set; }
        //public GAME_TYPE GameType { get; set; }
        public int Number { get; set; }
    }
}
