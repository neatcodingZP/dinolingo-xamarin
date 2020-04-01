using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DinoLingo
{
    public static class GameHelper
    {

        public static GameObjects memory_GameObjects;
        public static GameObjects sas_GameObjects;

        //public static string memory_UrlPrefix = "https://dinolingo.com/games/memory-game/sound/";
        //public static string sas_UrlPrefix = "https://dinolingo.com/games/see-and-say-game/sound/";
        public static string memory_UrlPrefix = "https://storage.googleapis.com/dino-lingo-web-games/memory-game/sound/";
        public static string sas_UrlPrefix = "https://storage.googleapis.com/dino-lingo-web-games/see-and-say-game/sound/";

        public static Task CreateGameObjects() {
            return Task.Run(async () => {
                // check if already have gameobjects
                bool AreGameObjectsAvailable = true;
                List<string> all_lang_cats = new List<string> (LANGUAGES.CAT_INFO.Keys);
                foreach (string lang_cat in all_lang_cats) {
                    if (await CacheHelper.Exists(CacheHelper.MEMORY_GAMEOBJECTS + lang_cat) && await CacheHelper.Exists(CacheHelper.SAS_GAMEOBJECTS + lang_cat)) continue;
                    else {
                        if (lang_cat != "53" && lang_cat != "888") {
                            AreGameObjectsAvailable = false;
                            break;
                        }
                    }
                }
                // return if we have all gameobjects
                Debug.WriteLine("do we have the gameobjects ?:" + AreGameObjectsAvailable);
                if (AreGameObjectsAvailable) return;

                // try to read gameobjects
                GameObjects[] memGO = await ReadGameObjectsFromFile("DinoLingo.Resources.memorygame_gameobjects.txt"); 
                GameObjects[] sasGO = await ReadGameObjectsFromFile("DinoLingo.Resources.sas_gameobjects.txt");
                List<string> all_lang_cats_mem = new List<string>(LANGUAGES.CAT_INFO.Keys);
                List<string> all_lang_cats_sas = new List<string>(LANGUAGES.CAT_INFO.Keys);

                // and save all gameobjects
                for (int i = 0; i < memGO.Length; i++)
                {
                    if (memGO[i] != null)
                    {
                        all_lang_cats_mem.Remove(memGO[i].Lang_Cat);
                        await CacheHelper.Add(CacheHelper.MEMORY_GAMEOBJECTS + memGO[i].Lang_Cat, memGO[i]);
                    }
                }

                // save all null objects
                foreach (string key in all_lang_cats_mem)
                {
                    await CacheHelper.Add(CacheHelper.MEMORY_GAMEOBJECTS + key, null);
                }
                
                // and save all sas gameobjects
                for (int i = 0; i < sasGO.Length; i++)
                {

                    if (sasGO[i] != null)
                    {
                        all_lang_cats_sas.Remove(sasGO[i].Lang_Cat);
                        await CacheHelper.Add(CacheHelper.SAS_GAMEOBJECTS + sasGO[i].Lang_Cat, sasGO[i]);
                    }
                }

                // save all null objects
                foreach (string key in all_lang_cats_sas)
                {
                    await CacheHelper.Add(CacheHelper.SAS_GAMEOBJECTS + key, null);
                }

                // check if we finally have gameobjects
                AreGameObjectsAvailable = true;
                foreach (string lang_cat in LANGUAGES.CAT_INFO.Keys)
                {
                    if (await CacheHelper.Exists(CacheHelper.MEMORY_GAMEOBJECTS + lang_cat) && await CacheHelper.Exists(CacheHelper.SAS_GAMEOBJECTS + lang_cat)) continue;
                    else
                    {
                        if (lang_cat != "53" && lang_cat != "888")
                        {
                            AreGameObjectsAvailable = false;
                            break;
                        }
                    }
                }
                Debug.WriteLine("we created and cached gameobjects ?:" + AreGameObjectsAvailable);
            }
            );
        }
        static Task<GameObjects[]> ReadGameObjectsFromFile (string resourceName) {
            return Task.Run(async() => {
                var assembly = Assembly.GetExecutingAssembly();
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = await reader.ReadToEndAsync();
                    return JsonConvert.DeserializeObject<GameObjects[]>(result);
                }
            });
        }

        public static async void ReadGameObjectsFromFiles_() {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceNameMemorygame = "DinoLingo.Resources.memorygame_gameobjects.txt";
            var resourceNameSas = "DinoLingo.Resources.sas_gameobjects.txt";
            GameObjects[] memGO, sasGO;

            using (Stream stream = assembly.GetManifestResourceStream(resourceNameMemorygame))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = await reader.ReadToEndAsync();
                //Debug.WriteLine("memGO = " + result);
                memGO = JsonConvert.DeserializeObject<GameObjects[]>(result);
            }

            using (Stream stream = assembly.GetManifestResourceStream(resourceNameSas))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = await reader.ReadToEndAsync();
                //Debug.WriteLine("sasGO = " + result);
                sasGO = JsonConvert.DeserializeObject<GameObjects[]>(result);
            }
            Debug.WriteLine($"total elements in memGO: {memGO.Length}, in sasGO: {sasGO.Length}");
            //connect two arrays into one (quizGO)
            for (int i = 0; i < memGO.Length; i++) {
                GameObjects memO = memGO[i];
                GameObjects sasO = sasGO[i];

                if (memO == null && sasO != null) {
                    memO = sasO;
                    continue;
                }

                if (sasO == null) continue;

                if (memO == null && sasO == null) continue;

                for (THEME_NAME theme = THEME_NAME.ACTIONS; theme < THEME_NAME.FAMILY; theme++) {
                    SoundItem[] soundItemsMem = memO.GetSoundItemsForTheme(theme);
                    SoundItem[] soundItemsSas = sasO.GetSoundItemsForTheme(theme);
                    if (soundItemsSas == null) continue;
                    //Debug.WriteLine($"soundItemsQuiz != null : " + (soundItemsMem != null) + ", sounitems: " + soundItemsMem.Length);
                    //Debug.WriteLine($"soundItemsSas != null : " + (soundItemsSas != null)+ ", sounitems: " + soundItemsSas.Length);
                    for (int j = 0; j < soundItemsMem.Length; j++) {
                        if (soundItemsMem[j]==null && soundItemsSas[j] != null) {
                            soundItemsMem[j] = soundItemsSas[j];
                            Debug.WriteLine($"sound obj added to Mem: {soundItemsSas[j].sound}, language: {sasO.LANG}");
                            continue;
                        }
                        if (soundItemsSas[j] == null) continue;
                        if (soundItemsMem[j] == null && soundItemsSas[j] == null) continue;

                        // do text

                        if (!string.IsNullOrEmpty(soundItemsSas[j].text)) soundItemsMem[j].text = soundItemsSas[j].text;
                            
                        // do sound
                        if (string.IsNullOrEmpty(soundItemsMem[j].sound) && !string.IsNullOrEmpty(soundItemsSas[j].sound))
                        {
                            soundItemsMem[j].sound = soundItemsSas[j].sound;
                            Debug.WriteLine($"sound added to Mem: {soundItemsSas[j].sound}, language: {sasO.LANG}");
                            continue;
                        }

                        if (string.IsNullOrEmpty(soundItemsSas[j].sound)) continue;

                        if (string.IsNullOrEmpty(soundItemsMem[j].sound) && string.IsNullOrEmpty(soundItemsSas[j].sound)) continue;

                        //here we have two values, compare it
                        if (soundItemsMem[j].sound != soundItemsSas[j].sound) { // sound do not match !
                            
                            //Debug.WriteLine($"sounds do not match! Mem: {soundItemsMem[j].sound}, Sas: {soundItemsSas[j].sound}, language: {sasO.LANG}");
                            string s = soundItemsMem[j].sound.Replace("_RNM", string.Empty);
                            if (s != soundItemsSas[j].sound) {
                                Debug.WriteLine($"sounds does not match (deleted _RNM)! Mem: {soundItemsMem[j].sound}, Sas: {soundItemsSas[j].sound}, language: {sasO.LANG}");
                            }
                        }
                        else {
                            //Debug.WriteLine($"sounds +++ Mem: {soundItemsMem[j].sound}, Sas: {soundItemsSas[j].sound}, language: {sasO.LANG}");
                        }

                    }
                } 
            }



            Debug.WriteLine("ReadGameObjectsFromFiles() - END.");
            string res = JsonConvert.SerializeObject(memGO);
            Debug.WriteLine(res);
        }

        public static async void GetAllAudios() {
            int totalAudioItems = 0;
            List<LangData> langDatas = new List<LangData>();
            List<GameObjects> allGameObjects = new List<GameObjects>();
            
            List<string> langCats = new List<string>(LANGUAGES.CAT_INFO.Keys);
            for (int i = 0; i < langCats.Count; i++) {
                string key = langCats[i];
                Debug.WriteLine("i = " + i);
                Debug.WriteLine("try to download gameobject for : " + LANGUAGES.CAT_INFO[key].Name);
                GameObjects gameObject = await DownloadHelper.DownloadHelper.GetGameObjects(LANGUAGES.CAT_INFO[key].Name);
                Debug.WriteLine("gameObject1 =" + JsonConvert.SerializeObject(gameObject));
                if (gameObject != null)  {
                    Debug.WriteLine("gameObject.LANG = " + gameObject.LANG);
                    // change gameObjects Id
                    gameObject.ChangeIdToKeys();
                    gameObject.Lang_Cat = key;
                    Debug.WriteLine("gameObject2 =" + JsonConvert.SerializeObject(gameObject));
                    // for gameObject create a LangData
                    int langCat = int.Parse(key);
                    string languageName = LANGUAGES.CAT_INFO[key].Name;
                    string[] values = new string[Theme.GetIndexForKeyId(THEME_NAME.FAMILY, 81) + 1];
                    Debug.WriteLine("total keys = " + values.Length);
                    int index = 0;
                    foreach (ThemeResource r in Theme.Resources) {
                        THEME_NAME themeName = r.Name;
                        SoundItem[] soundItem = gameObject.GetSoundItemsForTheme(themeName);

                        foreach (KeyValuePair<string, ItemInfo> pair in r.Item) {
                            Debug.WriteLine("index = " + index + ",  id = " + pair.Value.id);

                            string audioUrl = ""; //DownloadHelper.DownloadHelper.GetUrlForWord(pair.Key, themeName, languageName, gameObject, soundItem);

                            if (!string.IsNullOrEmpty(audioUrl)) {
                                audioUrl = "+";
                                totalAudioItems++;
                            }
                            else audioUrl = "@" + pair.Key + " " + pair.Value.id;

                            values[index] = audioUrl;
                            index++;
                        }

                        //total valid audios 
                        foreach (string item_key in r.Item.Keys) {
                            string audio = gameObject.GetSoundUrl(themeName, item_key);
                            if (!string.IsNullOrEmpty(audio)) totalAudioItems++;
                        }
                    }
                    Debug.WriteLine("gameObject3 =" + JsonConvert.SerializeObject(gameObject));
                    langDatas.Add(new LangData {LangCat = langCat, LanguageName = languageName, Values = values });

                    string s = JsonConvert.SerializeObject(langDatas[langDatas.Count-1]);
                    Debug.WriteLine("got new LangData = " + s);
                    Debug.WriteLine("total gameObjects downloaded: " + langDatas.Count);
                }
                else {
                    langDatas.Add(new LangData { LangCat = int.Parse(langCats[i]), LanguageName = "MISSED " + LANGUAGES.CAT_INFO[langCats[i]].Name, Values = null });
                }
                Debug.WriteLine("gameObject4 =" + JsonConvert.SerializeObject(gameObject));
                allGameObjects.Add(gameObject);                
            }
            Debug.WriteLine("total gameObjects downloaded: " + langDatas.Count + ", totalAudioItems + = " + totalAudioItems);
            Debug.WriteLine("Missed languages: ");
            for (int k = 0; k < langCats.Count; k++) {
                if (allGameObjects[k] == null) {
                    Debug.WriteLine(LANGUAGES.CAT_INFO[langCats[k]].Name);
                }
            }
            string ss = JsonConvert.SerializeObject(langDatas.ToArray());
            string allGO = JsonConvert.SerializeObject(allGameObjects.ToArray());
            Console.WriteLine(allGO);
        }

        static Task<SoundItem[]> ActionsAsync (string key) {
            return Task.Run(async () =>
            {
                return await DownloadHelper.DownloadHelper.GetTextKeysAsyncDebug(THEME_NAME.ACTIONS, LANGUAGES.CAT_INFO[key].Name);
            });
        }

        static Task<SoundItem[]> NumbersAsync(string key)
        {
            return Task.Run(async () =>
            {
                return await DownloadHelper.DownloadHelper.GetTextKeysAsyncDebug(THEME_NAME.NUMBERS, LANGUAGES.CAT_INFO[key].Name);
            });
        }

        static Task<SoundItem[]> AnimalsAsync(string key)
        {
            return Task.Run(async () =>
            {
                return await DownloadHelper.DownloadHelper.GetTextKeysAsyncDebug(THEME_NAME.ANIMALS, LANGUAGES.CAT_INFO[key].Name);
            });
        }

        static Task<SoundItem[]> body_partsAsync(string key)
        {
            return Task.Run(async () =>
            {
                return await DownloadHelper.DownloadHelper.GetTextKeysAsyncDebug(THEME_NAME.BODYPARTS, LANGUAGES.CAT_INFO[key].Name);
            });
        }

        static Task<SoundItem[]> clothesAsync(string key)
        {
            return Task.Run(async () =>
            {
                return await DownloadHelper.DownloadHelper.GetTextKeysAsyncDebug(THEME_NAME.CLOTHES, LANGUAGES.CAT_INFO[key].Name);
            });
        }

        static Task<SoundItem[]> colorsAsync(string key)
        {
            return Task.Run(async () =>
            {
                return await DownloadHelper.DownloadHelper.GetTextKeysAsyncDebug(THEME_NAME.COLORS, LANGUAGES.CAT_INFO[key].Name);
            });
        }

        static Task<SoundItem[]> vehiclesAsync(string key)
        {
            return Task.Run(async () =>
            {
                return await DownloadHelper.DownloadHelper.GetTextKeysAsyncDebug(THEME_NAME.VEHICLES, LANGUAGES.CAT_INFO[key].Name);
            });
        }

        static Task<SoundItem[]> foodAsync(string key)
        {
            return Task.Run(async () =>
            {
                return await DownloadHelper.DownloadHelper.GetTextKeysAsyncDebug(THEME_NAME.FOOD, LANGUAGES.CAT_INFO[key].Name);
            });
        }
        static Task<SoundItem[]> fruit_and_vegetablesAsync(string key)
        {
            return Task.Run(async () =>
            {
                return await DownloadHelper.DownloadHelper.GetTextKeysAsyncDebug(THEME_NAME.FRUITSANDVEGETABLES, LANGUAGES.CAT_INFO[key].Name);
            });
        }
        static Task<SoundItem[]> in_the_houseAsync(string key)
        {
            return Task.Run(async () =>
            {
                return await DownloadHelper.DownloadHelper.GetTextKeysAsyncDebug(THEME_NAME.HOUSE, LANGUAGES.CAT_INFO[key].Name);
            });
        }
        static Task<SoundItem[]> natureAsync(string key)
        {
            return Task.Run(async () =>
            {
                return await DownloadHelper.DownloadHelper.GetTextKeysAsyncDebug(THEME_NAME.NATURE, LANGUAGES.CAT_INFO[key].Name);
            });
        }

        static Task<SoundItem[]> familyAsync(string key)
        {
            return Task.Run(async () =>
            {
                return await DownloadHelper.DownloadHelper.GetTextKeysAsyncDebug(THEME_NAME.FAMILY, LANGUAGES.CAT_INFO[key].Name);
            });
        }

        public static async void GetAllAudiosForSAS()
        {
            int totalValidAudioItems = 0;
            List<LangData> langDatas = new List<LangData>();
            List<GameObjects> allGameObjects = new List<GameObjects>();

            List<string> langCats = new List<string>(LANGUAGES.CAT_INFO.Keys);
            
            for (int i = 0; i < langCats.Count; i++)
            {
                string key = langCats[i];
                Debug.WriteLine("i = " + i);
                Debug.WriteLine("try to download gameobject for : " + LANGUAGES.CAT_INFO[key].Name);

                var ActionsAsyncTask = ActionsAsync(key);
                var NumbersAsyncTask = NumbersAsync(key);
                var AnimalsAsyncTask = AnimalsAsync(key);
                var body_partsAsyncTask = body_partsAsync(key);
                var clothesAsyncTask = clothesAsync(key);
                var colorsAsyncTask = colorsAsync(key);
                var vehiclesAsyncTask = vehiclesAsync(key);
                var foodAsyncTask = foodAsync(key);
                var fruit_and_vegetablesAsyncTask = fruit_and_vegetablesAsync(key);
                var in_the_houseAsyncTask = in_the_houseAsync(key);
                var natureAsyncTask = natureAsync(key);
                var familyAsyncTask = familyAsync(key);


                await Task.WhenAll( ActionsAsyncTask,   NumbersAsyncTask,   AnimalsAsyncTask,    body_partsAsyncTask,
                                    clothesAsyncTask,   colorsAsyncTask,    vehiclesAsyncTask,  foodAsyncTask, 
                                    fruit_and_vegetablesAsyncTask,   in_the_houseAsyncTask, natureAsyncTask, familyAsyncTask);

                SoundItem[] actions = await ActionsAsyncTask;
                SoundItem[] numbers = await NumbersAsyncTask;
                SoundItem[] animals = await AnimalsAsyncTask;
                SoundItem[] body_parts = await body_partsAsyncTask;
                SoundItem[] clothes = await clothesAsyncTask;
                SoundItem[] colors = await colorsAsyncTask;
                SoundItem[] vehicles = await vehiclesAsyncTask;
                SoundItem[] food = await foodAsyncTask;
                SoundItem[] fruit_and_vegetables = await fruit_and_vegetablesAsyncTask;
                SoundItem[] in_the_house = await in_the_houseAsyncTask;
                SoundItem[] nature = await natureAsyncTask;
                SoundItem[] family = await familyAsyncTask;

                for (int i_ = 0; i_ < 2; i_++)
                {
                    if (actions == null) ActionsAsyncTask = ActionsAsync(key);
                    if (numbers == null) NumbersAsyncTask = NumbersAsync(key);
                    if (animals == null) AnimalsAsyncTask = AnimalsAsync(key);
                    if (body_parts == null) body_partsAsyncTask = body_partsAsync(key);
                    if (clothes == null) clothesAsyncTask = clothesAsync(key);
                    if (colors == null) colorsAsyncTask = colorsAsync(key);
                    if (vehicles == null) vehiclesAsyncTask = vehiclesAsync(key);
                    if (food == null) foodAsyncTask = foodAsync(key);
                    if (fruit_and_vegetables == null) fruit_and_vegetablesAsyncTask = fruit_and_vegetablesAsync(key);
                    if (in_the_house == null) in_the_houseAsyncTask = in_the_houseAsync(key);
                    if (nature == null) natureAsyncTask = natureAsync(key);
                    if (family == null) familyAsyncTask = familyAsync(key);

                    await Task.WhenAll(ActionsAsyncTask, NumbersAsyncTask, AnimalsAsyncTask, body_partsAsyncTask,
                                    clothesAsyncTask, colorsAsyncTask, vehiclesAsyncTask, foodAsyncTask,
                                    fruit_and_vegetablesAsyncTask, in_the_houseAsyncTask, natureAsyncTask, familyAsyncTask);

                    actions = await ActionsAsyncTask;
                    numbers = await NumbersAsyncTask;
                    animals = await AnimalsAsyncTask;
                    body_parts = await body_partsAsyncTask;
                    clothes = await clothesAsyncTask;
                    colors = await colorsAsyncTask;
                    vehicles = await vehiclesAsyncTask;
                    food = await foodAsyncTask;
                    fruit_and_vegetables = await fruit_and_vegetablesAsyncTask;
                    in_the_house = await in_the_houseAsyncTask;
                    nature = await natureAsyncTask;
                    family = await familyAsyncTask;
                }

                /*
                SoundItem[] actions = await DownloadHelper.DownloadHelper.GetTextKeysAsyncDebug(THEME_NAME.ACTIONS, LANGUAGES.CAT_INFO[key].Name);
                SoundItem[] numbers = await DownloadHelper.DownloadHelper.GetTextKeysAsyncDebug(THEME_NAME.NUMBERS, LANGUAGES.CAT_INFO[key].Name);
                SoundItem[] animals = await DownloadHelper.DownloadHelper.GetTextKeysAsyncDebug(THEME_NAME.ANIMALS, LANGUAGES.CAT_INFO[key].Name);
                SoundItem[] body_parts = await DownloadHelper.DownloadHelper.GetTextKeysAsyncDebug(THEME_NAME.BODYPARTS, LANGUAGES.CAT_INFO[key].Name);
                SoundItem[] clothes = await DownloadHelper.DownloadHelper.GetTextKeysAsyncDebug(THEME_NAME.CLOTHES, LANGUAGES.CAT_INFO[key].Name);
                SoundItem[] colors = await DownloadHelper.DownloadHelper.GetTextKeysAsyncDebug(THEME_NAME.COLORS, LANGUAGES.CAT_INFO[key].Name);
                SoundItem[] vehicles = await DownloadHelper.DownloadHelper.GetTextKeysAsyncDebug(THEME_NAME.VEHICLES, LANGUAGES.CAT_INFO[key].Name);
                SoundItem[] food = await DownloadHelper.DownloadHelper.GetTextKeysAsyncDebug(THEME_NAME.FOOD, LANGUAGES.CAT_INFO[key].Name);
                SoundItem[] fruit_and_vegetables = await DownloadHelper.DownloadHelper.GetTextKeysAsyncDebug(THEME_NAME.FRUITSANDVEGETABLES, LANGUAGES.CAT_INFO[key].Name);
                SoundItem[] in_the_house = await DownloadHelper.DownloadHelper.GetTextKeysAsyncDebug(THEME_NAME.HOUSE, LANGUAGES.CAT_INFO[key].Name);
                SoundItem[] nature = await DownloadHelper.DownloadHelper.GetTextKeysAsyncDebug(THEME_NAME.NATURE, LANGUAGES.CAT_INFO[key].Name);
                SoundItem[] family = await DownloadHelper.DownloadHelper.GetTextKeysAsyncDebug(THEME_NAME.FAMILY, LANGUAGES.CAT_INFO[key].Name);
*/
                GameObjects gameObject;
                if (actions != null || numbers != null || animals != null || body_parts!=null || clothes!=null || colors!=null || vehicles!=null || food!=null || 
                    fruit_and_vegetables!=null || in_the_house!=null || nature!=null || family!=null)
                    gameObject = new GameObjects
                    {
                        LANG = LANGUAGES.CAT_INFO[key].Name,
                        ACTIONS = actions,
                        NUMBERS = numbers,
                        ANIMALS = animals,
                        BODY_PARTS = body_parts,
                        CLOTHES = clothes,
                        COLORS = colors,
                        VEHICLES = vehicles,
                        FOOD = food,
                        FRUIT_AND_VEGETABLES = fruit_and_vegetables,
                        IN_THE_HOUSE = in_the_house,
                        NATURE = nature,
                        FAMILY = family
                    };
                else gameObject = null;

                Debug.WriteLine("we have gameobject : = " + JsonConvert.SerializeObject(gameObject)); 

                if (gameObject != null)
                {
                    Debug.WriteLine("gameObject.LANG = " + gameObject.LANG);
                    gameObject.Lang_Cat = langCats[i];
                    // change gameObjects Id
                    gameObject.OrderByKeys();

                    // for gameObject create a LangData
                    int langCat = int.Parse(key);
                    string languageName = LANGUAGES.CAT_INFO[key].Name;
                    string[] values = new string[Theme.GetIndexForKeyId(THEME_NAME.FAMILY, 81) + 1];
                    Debug.WriteLine("total keys = " + values.Length);
                    int index = 0;
                    foreach (ThemeResource r in Theme.Resources)
                    {
                        THEME_NAME themeName = r.Name;
                        SoundItem[] soundItem = gameObject.GetSoundItemsForTheme(themeName);
                        if (soundItem == null) continue;
                        foreach (KeyValuePair<string, ItemInfo> pair in r.Item)
                        {
                            Debug.WriteLine("index = " + index + ",  id = " + pair.Value.id);
                            string audioUrl =  ""; //DownloadHelper.DownloadHelper.GetUrlForWord(pair.Key, themeName, languageName, gameObject, soundItem);

                            /*
                            if (!string.IsNullOrEmpty(audioUrl)) {
                                audioUrl = "+";
                                totalValidAudioItems++;
                            }
                            else audioUrl = "@" + pair.Key; //+ " " + pair.Value.id;
                            */


                            values[index] = audioUrl;
                            index++;
                        }

                        foreach (SoundItem soundIt in soundItem) {
                            if (soundIt != null && !string.IsNullOrEmpty(soundIt.sound))  {
                                totalValidAudioItems++;
                            }
                        }
                    }



                    langDatas.Add(new LangData { LangCat = langCat, LanguageName = languageName, Values = values });

                    string s = JsonConvert.SerializeObject(langDatas[langDatas.Count - 1]);
                    Debug.WriteLine("got new LangData = " + s);
                    Debug.WriteLine("total gameObjects downloaded: " + langDatas.Count);
                }
                else
                {
                    langDatas.Add(new LangData { LangCat = int.Parse(langCats[i]), LanguageName = "MISSED " + LANGUAGES.CAT_INFO[langCats[i]].Name, Values = null });
                }

                allGameObjects.Add(gameObject);
            }
            Debug.WriteLine("total gameObjects downloaded: " + langDatas.Count + ", totalValidAudioItems = " + totalValidAudioItems);
            Debug.WriteLine("Missed languages: ");
            for (int k = 0; k < langCats.Count; k++)
            {
                if (allGameObjects[k] == null)
                {
                    Debug.WriteLine(LANGUAGES.CAT_INFO[langCats[k]].Name);
                }
            }
            string ss = JsonConvert.SerializeObject(langDatas.ToArray());
            string allGO = JsonConvert.SerializeObject(allGameObjects.ToArray());
            Console.WriteLine(allGO);
        }
    }

    public class LangData {
        public int LangCat { get; set; }
        public string LanguageName { get; set; }
        public string[] Values { get; set; }
    }
}
