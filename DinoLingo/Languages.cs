using System;
using System.Collections.Generic;

namespace DinoLingo
{
    public static class LANGUAGES
    {
        public static string FirstLetterToUpperCase(this string s)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentException("There is no first letter");

            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        

        public static Dictionary<string, LangInfo> CAT_INFO = new Dictionary<string, LangInfo>()
        {
            ["4"] = new LangInfo { Name = "albanian", Id = "39", ProductsIds = new string[] {"47921", "411", "6252" } },
            ["10"] = new LangInfo { Name = "arabic", Id = "9", ProductsIds = new string[] { "517321", "449", "6259" } },
            ["14"] = new LangInfo { Name = "armenian", Id = "40", ProductsIds = new string[] { "517327", "464", "6260" }, HasBookInSubCats = LangInfo.ALL_SUBCATS_HAVE_BOOKS },
            
            ["60"] = new LangInfo { Name = "bulgarian", Id = "27", ProductsIds = new string[] { "517354", "479", "6263" } },
            ["597"] = new LangInfo { Name = "cantonese", Id = "45", ProductsIds = new string[] { "517355", "11743", "11741" } },
            ["13"] = new LangInfo { Name = "chinese", Id = "14", ProductsIds = new string[] { "517356", "556", "6265" } },
            ["638"] = new LangInfo { Name = "creole", Id = "51", ProductsIds = new string[] { "517358", "15104", "537647" } }, 
            ["15"] = new LangInfo { Name = "croatian", Id = "31", ProductsIds = new string[] { "517357", "561", "6268" } },
            ["16"] = new LangInfo { Name = "czech", Id = "32", ProductsIds = new string[] { "517359", "575", "6270" } },
            // ===
            ["17"] = new LangInfo { Name = "danish", Id = "33", ProductsIds = new string[] { "517360", "589", "6272" } },
            ["644"] = new LangInfo { Name = "dari", Id = "52", ProductsIds = new string[] { "517361", "15119", "537648" } }, 
            ["19"] = new LangInfo { Name = "dutch", Id = "4", ProductsIds = new string[] { "517364", "603", "6274" } },
            ["20"] = new LangInfo { Name = "english", Id = "1", ProductsIds = new string[] { "517365", "618", "6276" }, HasBookInSubCats = LangInfo.ALL_SUBCATS_HAVE_BOOKS },
            
            ["22"] = new LangInfo { Name = "finnish", Id = "16", ProductsIds = new string[] { "517363", "1125", "6280" } }, 
            ["23"] = new LangInfo { Name = "french", Id = "25", ProductsIds = new string[] { "517366", "1128", "6282" } },
            ["24"] = new LangInfo { Name = "german", Id = "21", ProductsIds = new string[] { "517367", "1130", "6284" } },
            ["25"] = new LangInfo { Name = "greek", Id = "5", ProductsIds = new string[] { "517368", "1132", "6286" } },
            ["604"] = new LangInfo { Name = "gujarati", Id = "44", ProductsIds = new string[] { "517369", "12150", "12148" } },
            // ===
            ["647"] = new LangInfo { Name = "hawaiian", Id = "55",  ProductsIds = new string[] { "517370", "16118", "537649" } }, 
            ["26"] = new LangInfo { Name = "hebrew", Id = "22", ProductsIds = new string[] { "517371", "1134", "6288" }, HasBookInSubCats = LangInfo.ALL_SUBCATS_HAVE_BOOKS },
            ["27"] = new LangInfo { Name = "hindi", Id = "30", ProductsIds = new string[] { "517372", "1136", "6290" } },
            ["28"] = new LangInfo { Name = "hungarian", Id = "26", ProductsIds = new string[] { "517373", "1138", "6292" } },
            ["29"] = new LangInfo { Name = "indonesian", Id = "29", ProductsIds = new string[] { "517332", "655", "6294" } },
            ["30"] = new LangInfo { Name = "irish", Id = "28", ProductsIds = new string[] { "517333", "654", "6297" } },
            ["31"] = new LangInfo { Name = "italian", Id = "12", ProductsIds = new string[] { "517334", "653", "6299" } },
            ["32"] = new LangInfo { Name = "japanese", Id = "6", ProductsIds = new string[] { "517335", "652", "6301" } },
            ["645"] = new LangInfo { Name = "kazakh", Id = "57", ProductsIds = new string[] { "517336", "15120", "537650" } },
            ["33"] = new LangInfo { Name = "korean", Id = "10", ProductsIds = new string[] { "517337", "650", "6303" } },
            ["34"] = new LangInfo { Name = "latin", Id = "11", ProductsIds = new string[] { "517338", "648", "6305" } },
            // ===
            ["35"] = new LangInfo { Name = "malay", Id = "34", ProductsIds = new string[] { "517339", "647", "6307" } },
            ["36"] = new LangInfo { Name = "norwegian", Id = "35", ProductsIds = new string[] { "517340", "646", "6309" } },
            ["37"] = new LangInfo { Name = "persian", Id = "23", ProductsIds = new string[] { "517341", "644", "6311" } },            
            ["38"] = new LangInfo { Name = "polish", Id = "2", ProductsIds = new string[] { "517342", "643", "6313" } },
            ["39"] = new LangInfo { Name = "portuguese", Id = "7", ProductsIds = new string[] { "517343", "642", "6315" }, VisibleName = "portuguese (BR)" },
            ["21"] = new LangInfo { Name = "european-portuguese", SlugName = "eu-portuguese", Id = "17", ProductsIds = new string[] { "517362", "665", "6278" }, VisibleName = "portuguese (EU)" },
            ["646"] = new LangInfo { Name = "punjabi", Id = "62", ProductsIds = new string[] { "517344", "15121", "537646" } },  
            ["40"] = new LangInfo { Name = "romanian", Id = "36", ProductsIds = new string[] { "517345", "641", "6318" } },
            ["41"] = new LangInfo { Name = "russian", Id = "15", ProductsIds = new string[] { "517346", "640", "6320" } },
            ["42"] = new LangInfo { Name = "serbian", Id = "37", ProductsIds = new string[] { "517347", "639", "6322" } },
            ["43"] = new LangInfo { Name = "slovak", Id = "41", ProductsIds = new string[] { "517348", "638", "6324" } },
            ["641"] = new LangInfo { Name = "slovenian", Id = "63", ProductsIds = new string[] { "518777", "15108", "537645" } },
            // ===
            ["44"] = new LangInfo { Name = "spanish", Id = "8", ProductsIds = new string[] { "517349", "637", "6326" }, HasBookInSubCats = LangInfo.ALL_SUBCATS_HAVE_BOOKS },
            ["45"] = new LangInfo { Name = "swahili", Id = "38", ProductsIds = new string[] { "517350", "636", "6328" } },
            ["46"] = new LangInfo { Name = "swedish", Id = "13", ProductsIds = new string[] { "517351", "634", "6330" } },
            ["47"] = new LangInfo { Name = "tagalog", Id = "24", ProductsIds = new string[] { "517352", "633", "6332" } },
            ["48"] = new LangInfo { Name = "thai", Id = "20", ProductsIds = new string[] { "517353", "632", "6334" } },
            ["49"] = new LangInfo { Name = "turkish", Id = "3", ProductsIds = new string[] { "517330", "631", "6336" } },
            ["50"] = new LangInfo { Name = "ukrainian", Id = "42", ProductsIds = new string[] { "517331", "630", "6338" } },
            ["51"] = new LangInfo { Name = "urdu", Id = "19", ProductsIds = new string[] { "517329", "629", "6340" } },
            ["52"] = new LangInfo { Name = "vietnamese", Id = "18", ProductsIds = new string[] { "517374", "628", "6342" } },
            ["53"] = new LangInfo { Name = "welsh", Id = "43", ProductsIds = new string[] { "517375", "627", "6344" } },
                        // total 51
        };

        public static void SetVisibleNames()
        {
            foreach (KeyValuePair<string, LangInfo> pair in CAT_INFO)
            {
                pair.Value.VisibleName = Translate.GetString("lang_" + pair.Value.Name.Replace('-', '_'));
            }
        }

        public static string GetCatForLang (string language)
        {
            string lang = language.ToLower();
            foreach (KeyValuePair<string, LangInfo> pair in CAT_INFO)
            {
                if (lang == pair.Value.Name) return pair.Key;
            }
            return string.Empty;
        }

        public class LangInfo
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string VisibleName { get; set; }
            public string SlugName { get; set; }
            public string[] ProductsIds { get; set; }
            public int[] HasBookInSubCats { get; set; }
            public static readonly int[] ALL_SUBCATS_HAVE_BOOKS =   { 1, 1, 1, 1, 1, 1, 1};
            public static readonly int[] SUBCATS_ALL_BUT_DINOS =    { 1, 1, 1, 1, 1, 1, 0};


            public string GetVisibleName()
            {
                if (string.IsNullOrEmpty(VisibleName)) return Name.FirstLetterToUpperCase();
                return VisibleName.FirstLetterToUpperCase();
            }

            public string GetSlug()
            {
                if (string.IsNullOrEmpty(SlugName)) return Name;
                return SlugName;
            }

            public LangInfo()
            {
                HasBookInSubCats = SUBCATS_ALL_BUT_DINOS;
            }
        }
    }


}
