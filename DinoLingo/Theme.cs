using System;
using System.Collections.Generic;
using DinoLingo;

namespace DinoLingo
{
    public enum THEME_NAME { ACTIONS, NUMBERS, ANIMALS, BODYPARTS, CLOTHES, COLORS, VEHICLES, FOOD, FRUITSANDVEGETABLES, HOUSE, NATURE, FAMILY  };


    public enum GAME_TYPE { FIND_THE_PAIR = 1, CAROUSEL, SEE_AND_SAY, QUIZ, SEE_AND_SAY_2, SPRINT, NOT_A_GAME, NEW_GAME };

    public static class Game {
        private static string[] Keys = new string[] {"", "game 1", "game 2", "game 3", "quiz", "see and say 2", "game 4", "game 5", "game 6", "game 7", "game 8", "game 9"};

        public static GAME_TYPE GetTypeFromTitle(string title, string game_type) {

            if (NewGameType(game_type) != GAME_TYPE.NOT_A_GAME) return NewGameType(game_type);

            if (string.IsNullOrEmpty(title)) return GAME_TYPE.NOT_A_GAME;

            string s = title.ToLower();

            for (int i = 1; i < Keys.Length; i++) {
                if (s.Contains(Keys[i])) {
                    if (i <= 6) return (GAME_TYPE)i;
                    else return GAME_TYPE.NEW_GAME;
                }
            }
            return GAME_TYPE.NOT_A_GAME; 
        }

        static GAME_TYPE NewGameType(string game_type)
        {
            if (string.IsNullOrEmpty(game_type)) return GAME_TYPE.NOT_A_GAME;

            if (game_type == "4") return GAME_TYPE.SPRINT;

            return GAME_TYPE.NOT_A_GAME;
        }
    }


    public class Theme
    {


        public static ThemeResource[] Resources = new ThemeResource[] {
            new ThemeResource {
                Name = THEME_NAME.ACTIONS,
                StringName = "ACTIONS",
                ImgFolder = "ACTIONS",
                PostKeyWord = "actions",
                Item = new Dictionary<string, ItemInfo> {
                    ["SMILING"] = new ItemInfo {id = 101, PairsGameImgFiles = "SMILING.png"},
                    ["EATING"] = new ItemInfo  {id = 11, PairsGameImgFiles = "EATING.png"},
                    ["SWIMMING"] = new ItemInfo {id = 111, PairsGameImgFiles = "SWIMMING.png"},
                    ["FLYING"] = new ItemInfo {id = 121, PairsGameImgFiles = "FLYING.png"},
                    ["JUMPING"] = new ItemInfo {id = 131, PairsGameImgFiles = "JUMPING.png"},
                    ["DANCING"] = new ItemInfo {id = 141, PairsGameImgFiles = "DANCING.png"},
                    ["DRINKING"] = new ItemInfo {id = 21, PairsGameImgFiles = "DRINKING.png"},
                    ["LOOKING"] = new ItemInfo {id = 31, PairsGameImgFiles = "LOOKING.png"},
                    ["PLAYING"] = new ItemInfo {id = 41, PairsGameImgFiles = "PLAYING.png"},
                    ["READING"] = new ItemInfo {id = 51, PairsGameImgFiles = "READING.png"},
                    ["WALKING"] = new ItemInfo {id = 61, PairsGameImgFiles = "WALKING.png"},
                    ["RUNNING"] = new ItemInfo {id = 71, PairsGameImgFiles = "RUNNING.png"},
                    ["SLEEPING"] = new ItemInfo {id = 81, PairsGameImgFiles = "SLEEPING.png"},
                    ["SMELLING"] = new ItemInfo {id = 91, PairsGameImgFiles = "SMELLING.png"},
                },
                Games = new List<GAME_TYPE> {GAME_TYPE.FIND_THE_PAIR, GAME_TYPE.CAROUSEL, GAME_TYPE.QUIZ}
            },  

            new ThemeResource {
                Name = THEME_NAME.NUMBERS,
                StringName = "NUMBERS",
                ImgFolder = "NUMBERS",
                PostKeyWord = "numbers",
                Item = new Dictionary<string, ItemInfo> {
                    ["TEN"] = new ItemInfo {id = 101, PairsGameImgFiles = "TEN.png"},
                    ["ONE"] = new ItemInfo {id = 11, PairsGameImgFiles = "ONE.png"},
                    ["TWO"] = new ItemInfo {id = 21, PairsGameImgFiles = "TWO.png"},
                    ["THREE"] = new ItemInfo {id = 31, PairsGameImgFiles = "THREE.png"},
                    ["FOUR"] = new ItemInfo {id = 41, PairsGameImgFiles = "FOUR.png"},
                    ["FIVE"] = new ItemInfo {id = 51, PairsGameImgFiles = "FIVE.png"},
                    ["SIX"] = new ItemInfo {id = 61, PairsGameImgFiles = "SIX.png"},
                    ["SEVEN"] = new ItemInfo {id = 71, PairsGameImgFiles = "SEVEN.png"},
                    ["EIGHT"] = new ItemInfo {id = 81, PairsGameImgFiles = "EIGHT.png"},
                    ["NINE"] = new ItemInfo {id = 91, PairsGameImgFiles = "NINE.png"},
                },

            },

            new ThemeResource {
                Name = THEME_NAME.ANIMALS,
                StringName = "ANIMALS",
                ImgFolder = "ANIMALS",

                PostKeyWord = "animals",
                Item = new Dictionary<string, ItemInfo> {
                    ["CAT"] = new ItemInfo {id = 100011, PairsGameImgFiles = "CAT.png"},
                    ["DOG"] = new ItemInfo {id = 100021, PairsGameImgFiles = "DOG.png"},
                    ["BIRD"] = new ItemInfo {id = 100031, PairsGameImgFiles = "BIRD.png"},
                    ["FISH"] = new ItemInfo {id = 100041, PairsGameImgFiles = "FISH.png"},
                    ["DEER"] = new ItemInfo {id = 2000101, PairsGameImgFiles = "DEER.png"},
                    ["MONKEY"] = new ItemInfo {id = 200011, PairsGameImgFiles = "MONKEY.png"},
                    ["BEAR"] = new ItemInfo {id = 2000111, PairsGameImgFiles = "BEAR.png"},
                    ["MOUSE"] = new ItemInfo {id = 2000121, PairsGameImgFiles = "MOUSE.png"},
                    ["RABBIT"] = new ItemInfo {id = 2000131, PairsGameImgFiles = "RABBIT.png"},
                    ["GORILLA"] = new ItemInfo {id = 200021, PairsGameImgFiles = "GORILLA.png"},
                    ["GIRAFFE"] = new ItemInfo {id = 200031, PairsGameImgFiles = "GIRAFFE.png"},
                    ["LION"] = new ItemInfo {id = 200041, PairsGameImgFiles = "LION.png"},
                    ["TIGER"] = new ItemInfo {id = 200051, PairsGameImgFiles = "TIGER.png"},
                    ["RHINO"] = new ItemInfo {id = 200061, PairsGameImgFiles = "RHINO.png"},
                    ["ZEBRA"] = new ItemInfo {id = 200071, PairsGameImgFiles = "ZEBRA.png"},
                    ["HIPPOPOTAMUS"] = new ItemInfo {id = 200081, PairsGameImgFiles = "HIPPO.png"},
                    ["ELEPHANT"] = new ItemInfo {id = 200091, PairsGameImgFiles = "ELEPHANT.png"},
                    ["ANT"] = new ItemInfo {id = 300011, PairsGameImgFiles = "ANT.png"},
                    ["BEE"] = new ItemInfo {id = 300021, PairsGameImgFiles = "BEE.png"},
                    ["BUTTERFLY"] = new ItemInfo {id = 300031, PairsGameImgFiles = "BUTTERFLY.png"},
                    ["SPIDER"] = new ItemInfo {id = 300041, PairsGameImgFiles = "SPIDER.png"},
                    ["CROCODILE"] = new ItemInfo {id = 300051, PairsGameImgFiles = "CROCODILE.png"},
                    ["FROG"] = new ItemInfo {id = 300061, PairsGameImgFiles = "FROG.png"},
                    ["SNAKE"] = new ItemInfo {id = 300071, PairsGameImgFiles = "SNAKE.png"},
                    ["TURTLE"] = new ItemInfo {id = 300081, PairsGameImgFiles = "TURTLE.png"},
                    ["CHICK"] = new ItemInfo {id = 4000101, PairsGameImgFiles = "CHICK.png"},
                    ["COW"] = new ItemInfo {id = 400011, PairsGameImgFiles = "COW.png"},
                    ["CAMEL"] = new ItemInfo {id = 4000111, PairsGameImgFiles = "CAMEL.png"},
                    ["SHEEP"] = new ItemInfo {id = 400021, PairsGameImgFiles = "SHEEP.png"},
                    ["HORSE"] = new ItemInfo {id = 400031, PairsGameImgFiles = "HORSE.png"},
                    ["DONKEY"] = new ItemInfo {id = 400041, PairsGameImgFiles = "DONKEY.png"},
                    ["TURKEY"] = new ItemInfo {id = 400051, PairsGameImgFiles = "TURKEY.png"},
                    ["CHICKEN"] = new ItemInfo {id = 400061, PairsGameImgFiles = "CHICKEN.png"},
                    ["DUCK"] = new ItemInfo {id = 400071, PairsGameImgFiles = "DUCK.png"},
                    ["ROOSTER"] = new ItemInfo {id = 400081, PairsGameImgFiles = "ROOSTER.png"},
                    ["PIG"] = new ItemInfo {id = 400091, PairsGameImgFiles = "PIG.png"},
                    ["DOLPHIN"] = new ItemInfo {id = 500011, PairsGameImgFiles = "DOLPHIN.png"},
                    ["SHARK"] = new ItemInfo {id = 500021, PairsGameImgFiles = "SHARK.png"},
                    ["WHALE"] = new ItemInfo {id = 500031, PairsGameImgFiles = "WHALE.png"},
                    ["PENGUIN"] = new ItemInfo {id = 500041, PairsGameImgFiles = "PENGUIN.png"},
                    ["PARROT"] = new ItemInfo {id = 600011, PairsGameImgFiles = "PARROT.png"},
                    ["EAGLE"] = new ItemInfo {id = 600021, PairsGameImgFiles = "EAGLE.png"},
                    ["OSTRICH"] = new ItemInfo {id = 600031, PairsGameImgFiles = "OSTRICH.png"},
                    ["OWL"] = new ItemInfo {id = 600041, PairsGameImgFiles = "OWL.png"},
                    ["FLAMINGO"] = new ItemInfo {id = 600051, PairsGameImgFiles = "FLAMINGO.png"},
                },
                Games = new List<GAME_TYPE> {GAME_TYPE.FIND_THE_PAIR, GAME_TYPE.CAROUSEL, GAME_TYPE.SEE_AND_SAY, GAME_TYPE.SEE_AND_SAY_2, GAME_TYPE.QUIZ}
            },

            new ThemeResource {
                Name = THEME_NAME.BODYPARTS,
                StringName = "BODY PARTS",
                ImgFolder = "BODYPARTS",
                PostKeyWord = "body",
                Item = new Dictionary<string, ItemInfo> {
                    ["FINGER"] = new ItemInfo {id = 101, PairsGameImgFiles = "FINGER.png"},
                    ["BODY"] = new ItemInfo {id = 11, PairsGameImgFiles = "BODY.png"},
                    ["LEG"] = new ItemInfo {id = 112, PairsGameImgFiles = "LEGS.png"},
                    ["HEAD"] = new ItemInfo {id = 12, PairsGameImgFiles = "HEAD.png"},
                    ["FOOT"] = new ItemInfo {id = 122, PairsGameImgFiles = "FOOT.png"},
                    ["HAIR"] = new ItemInfo {id = 21, PairsGameImgFiles = "HAIR.png"},
                    ["FACE"] = new ItemInfo {id = 31, PairsGameImgFiles = "FACE.png"},
                    ["EYES"] = new ItemInfo {id = 41, PairsGameImgFiles = "EYES.png"},
                    ["EAR"] = new ItemInfo {id = 51, PairsGameImgFiles = "EAR.png"},
                    ["NOSE"] = new ItemInfo {id = 61, PairsGameImgFiles = "NOSE.png"},
                    ["MOUTH"] = new ItemInfo {id = 71, PairsGameImgFiles = "MOUTH.png"},
                    ["ARM"] = new ItemInfo {id = 81, PairsGameImgFiles = "ARM.png"},
                    ["HAND"] = new ItemInfo {id = 91, PairsGameImgFiles = "HAND.png"},                    
                }
            },

            new ThemeResource {
                Name = THEME_NAME.CLOTHES,
                StringName = "CLOTHES",
                ImgFolder = "CLOTHES",
                PostKeyWord = "clothing",
                Item = new Dictionary<string, ItemInfo> {
                    ["SOCKS"] = new ItemInfo {id = 101, PairsGameImgFiles = "SOCKS.png"},
                    ["TSHIRT"] = new ItemInfo {id = 11, PairsGameImgFiles = "TSHIRT.png"},
                    ["SCARF"] = new ItemInfo {id = 111, PairsGameImgFiles = "SCARF.png"},
                    ["GLASSES"] = new ItemInfo {id = 121, PairsGameImgFiles = "GLASSES.png"},
                    ["GLOVES"] = new ItemInfo {id = 133, PairsGameImgFiles = "GLOVES.png"},
                    ["PANTS"] = new ItemInfo {id = 21, PairsGameImgFiles = "PANTS.png"},
                    ["HAT"] = new ItemInfo {id = 31, PairsGameImgFiles = "HAT.png"},
                    ["JACKET"] = new ItemInfo {id = 41, PairsGameImgFiles = "JACKET.png"},
                    ["DRESS"] = new ItemInfo {id = 51, PairsGameImgFiles = "DRESS.png"},
                    ["SKIRT"] = new ItemInfo {id = 61, PairsGameImgFiles = "SKIRT.png"},
                    ["SWEATER"] = new ItemInfo {id = 71, PairsGameImgFiles = "SWEATER.png"},
                    ["SHOES"] = new ItemInfo {id = 81, PairsGameImgFiles = "SHOES.png"},
                    ["BOOTS"] = new ItemInfo {id = 91, PairsGameImgFiles = "BOOTS.png"},
                }
            },  

            new ThemeResource {
                Name = THEME_NAME.COLORS,
                StringName = "COLORS",
                ImgFolder = "COLORS",
                PostKeyWord = "colors",
                Item = new Dictionary<string, ItemInfo> {
                    ["BROWN"] = new ItemInfo {id = 101, PairsGameImgFiles = "BROWN.png"},
                    ["RED"] = new ItemInfo {id = 11, PairsGameImgFiles = "RED.png"},
                    ["GREEN"] = new ItemInfo {id = 21, PairsGameImgFiles = "GREEN.png"},
                    ["YELLOW"] = new ItemInfo {id = 31, PairsGameImgFiles = "YELLOW.png"},
                    ["BLUE"] = new ItemInfo {id = 41, PairsGameImgFiles = "BLUE.png"},
                    ["PURPLE"] = new ItemInfo {id = 51, PairsGameImgFiles = "PURPLE.png"},
                    ["ORANGE"] = new ItemInfo {id = 61, PairsGameImgFiles = "ORANGE.png"},
                    ["PINK"] = new ItemInfo {id = 71, PairsGameImgFiles = "PINK.png"},
                    ["BLACK"] = new ItemInfo {id = 81, PairsGameImgFiles = "BLACK.png"},
                    ["WHITE"] = new ItemInfo {id = 91, PairsGameImgFiles = "WHITE.png"},
                }
            },    

            new ThemeResource {
                Name = THEME_NAME.VEHICLES,
                StringName = "VEHICLES",
                ImgFolder = "VEHICLES",
                PostKeyWord = "vehicles",
                Item = new Dictionary<string, ItemInfo> {
                    ["ROCKET"] = new ItemInfo {id = 101, PairsGameImgFiles = "ROCKET.png"},
                    ["BICYCLE"] = new ItemInfo {id = 11, PairsGameImgFiles = "BICYCLE.png"},
                    ["MOTORCYCLE"] = new ItemInfo {id = 21, PairsGameImgFiles = "MOTORCYCLE.png"},
                    ["CAR"] = new ItemInfo {id = 31, PairsGameImgFiles = "CAR.png"},
                    ["BUS"] = new ItemInfo {id = 41, PairsGameImgFiles = "BUS.png"},
                    ["TRUCK"] = new ItemInfo {id = 51, PairsGameImgFiles = "TRUCK.png"},
                    ["TRAIN"] = new ItemInfo {id = 61, PairsGameImgFiles = "TRAIN.png"},
                    ["HELICOPTER"] = new ItemInfo {id = 71, PairsGameImgFiles = "HELICOPTER.png"},
                    ["AIRPLANE"] = new ItemInfo {id = 81, PairsGameImgFiles = "AIRPLANE.png"},
                    ["BOAT"] = new ItemInfo {id = 91, PairsGameImgFiles = "BOAT.png"},
                }
            },  

            new ThemeResource {
                Name = THEME_NAME.FOOD,
                StringName = "FOOD",
                ImgFolder = "FOOD",
                PostKeyWord = "food",
                Item = new Dictionary<string, ItemInfo> {
                    ["CAKE"] = new ItemInfo {id = 101, PairsGameImgFiles = "CAKE.png"},
                    ["MILK"] = new ItemInfo {id = 11, PairsGameImgFiles = "MILK.png"},
                    ["CHOCOLATE"] = new ItemInfo {id = 111, PairsGameImgFiles = "CHOCOLATE.png"},
                    ["ICECREAM"] = new ItemInfo {id = 121, PairsGameImgFiles = "ICECREAM.png"},
                    ["BREAD"] = new ItemInfo {id = 21, PairsGameImgFiles = "BREAD.png"},
                    ["WATER"] = new ItemInfo {id = 31, PairsGameImgFiles = "WATER.png"},
                    ["EGG"] = new ItemInfo {id = 41, PairsGameImgFiles = "EGG.png"},
                    ["BUTTER"] = new ItemInfo {id = 51, PairsGameImgFiles = "BUTTER.png"},
                    ["CHEESE"] = new ItemInfo {id = 61, PairsGameImgFiles = "CHEESE.png"},
                    ["HONEY"] = new ItemInfo {id = 71, PairsGameImgFiles = "HONEY.png"},
                    ["JUICE"] = new ItemInfo {id = 81, PairsGameImgFiles = "JUICE.png"},
                    ["SOUP"] = new ItemInfo {id = 91, PairsGameImgFiles = "SOUP.png"},
                }
            },  

            new ThemeResource {
                Name = THEME_NAME.FRUITSANDVEGETABLES,
                StringName = "FRUIT AND VEGETABLES",
                ImgFolder = "FRUITSANDVEGETABLES",
                PostKeyWord = "fruit",
                Item = new Dictionary<string, ItemInfo> {
                    ["PEACH"] = new ItemInfo {id = 101, PairsGameImgFiles = "PEACH.png"},
                    ["APPLE"] = new ItemInfo {id = 11, PairsGameImgFiles = "APPLE.png"},
                    ["LEMON"] = new ItemInfo {id = 111, PairsGameImgFiles = "LEMON.png"},
                    ["CARROT"] = new ItemInfo {id = 121, PairsGameImgFiles = "CARROT.png"},
                    ["TOMATO"] = new ItemInfo {id = 131, PairsGameImgFiles = "TOMATO.png"},
                    ["POTATO"] = new ItemInfo {id = 141, PairsGameImgFiles = "POTATO.png"},
                    ["BROCCOLI"] = new ItemInfo {id = 151, PairsGameImgFiles = "BROCCOLI.png"},
                    ["BANANA"] = new ItemInfo {id = 21, PairsGameImgFiles = "BANANA.png"},
                    ["ORANGE"] = new ItemInfo {id = 31, PairsGameImgFiles = "ORANGE.png"},
                    ["STRAWBERRY"] = new ItemInfo {id = 41, PairsGameImgFiles = "STRAWBERRY.png"},
                    ["GRAPES"] = new ItemInfo {id = 51, PairsGameImgFiles = "GRAPES.png"},
                    ["WATERMELON"] = new ItemInfo {id = 61, PairsGameImgFiles = "WATERMELON.png"},
                    ["MELON"] = new ItemInfo {id = 71, PairsGameImgFiles = "MELON.png"},
                    ["CHERRY"] = new ItemInfo {id = 82, PairsGameImgFiles = "CHERRY.png"},
                    ["PEAR"] = new ItemInfo {id = 91, PairsGameImgFiles = "PEAR.png"},
                }
            },  

            new ThemeResource {
                Name = THEME_NAME.HOUSE,
                StringName = "IN THE HOUSE",
                ImgFolder = "HOUSE",
                PostKeyWord = "house",
                Item = new Dictionary<string, ItemInfo> {
                    ["TABLE"] = new ItemInfo {id = 101, PairsGameImgFiles = "TABLE.png"},
                    ["BALL"] = new ItemInfo {id = 11, PairsGameImgFiles = "BALL.png"},
                    ["CHAIR"] = new ItemInfo {id = 111, PairsGameImgFiles = "CHAIR.png"},
                    ["BED"] = new ItemInfo {id = 121, PairsGameImgFiles = "BED.png"},
                    ["TELEVISION"] = new ItemInfo {id = 131, PairsGameImgFiles = "TELEVISION.png"},
                    ["COMPUTER"] = new ItemInfo {id = 141, PairsGameImgFiles = "COMPUTER.png"},
                    ["TELEPHONE"] = new ItemInfo {id = 151, PairsGameImgFiles = "TELEPHONE.png"},
                    ["CLOCK"] = new ItemInfo {id = 161, PairsGameImgFiles = "CLOCK.png"},
                    ["LAMP"] = new ItemInfo {id = 171, PairsGameImgFiles = "LAMP.png"},
                    ["KITE"] = new ItemInfo {id = 181, PairsGameImgFiles = "KITE.png"},
                    ["BALLOON"] = new ItemInfo {id = 21, PairsGameImgFiles = "BALLOON.png"},
                    ["TOYS"] = new ItemInfo {id = 31, PairsGameImgFiles = "TOYS.png"},
                    ["BOX"] = new ItemInfo {id = 41, PairsGameImgFiles = "BOX.png"},
                    ["BOOK"] = new ItemInfo {id = 51, PairsGameImgFiles = "BOOK.png"},
                    ["PICTURE"] = new ItemInfo {id = 61, PairsGameImgFiles = "PICTURE.png"},
                    ["DOOR"] = new ItemInfo {id = 81, PairsGameImgFiles = "DOOR.png"},
                    ["WINDOW"] = new ItemInfo {id = 91, PairsGameImgFiles = "WINDOW.png"},
                    ["BAG"] = new ItemInfo {id = 191, PairsGameImgFiles = "BAG.png"},
                }
            },  


            new ThemeResource {
                Name = THEME_NAME.NATURE,
                StringName = "NATURE",
                ImgFolder = "NATURE",
                PostKeyWord = "nature",
                Item = new Dictionary<string, ItemInfo> {
                    ["FLOWER"] = new ItemInfo {id = 101, PairsGameImgFiles = "FLOWER.png"},
                    ["SUN"] = new ItemInfo {id = 11, PairsGameImgFiles = "SUN.png"},
                    ["GRASS"] = new ItemInfo {id = 111, PairsGameImgFiles = "GRASS.png"},
                    ["RAIN"] = new ItemInfo {id = 121, PairsGameImgFiles = "RAIN.png"},
                    ["SNOW"] = new ItemInfo {id = 131, PairsGameImgFiles = "SNOW.png"},
                    ["RAINBOW"] = new ItemInfo {id = 141, PairsGameImgFiles = "RAINBOW.png"},
                    ["ROCK"] = new ItemInfo {id = 152, PairsGameImgFiles = "ROCK.png"},
                    ["MOON"] = new ItemInfo {id = 31, PairsGameImgFiles = "MOON.png"},
                    ["STAR"] = new ItemInfo {id = 41, PairsGameImgFiles = "STAR.png"},
                    ["SKY"] = new ItemInfo {id = 51, PairsGameImgFiles = "SKY.png"},
                    ["CLOUD"] = new ItemInfo {id = 61, PairsGameImgFiles = "CLOUD.png"},
                    ["SEA"] = new ItemInfo {id = 71, PairsGameImgFiles = "SEA.png"},
                    ["FOREST"] = new ItemInfo {id = 81, PairsGameImgFiles = "FOREST.png"},
                    ["TREE"] = new ItemInfo {id = 91, PairsGameImgFiles = "TREE.png"},
                } 
            }, 

            new ThemeResource {
                Name = THEME_NAME.FAMILY,
                StringName = "FAMILY",
                ImgFolder = "FAMILY",
                PostKeyWord = "family",
                Item = new Dictionary<string, ItemInfo> {
                    ["MOTHER"] = new ItemInfo {id = 11, PairsGameImgFiles = "MOTHER.png"},
                    ["FATHER"] = new ItemInfo {id = 21, PairsGameImgFiles = "FATHER.png"},
                    ["BABY"] = new ItemInfo {id = 41, PairsGameImgFiles = "BABY.png"},
                    ["BOY"] = new ItemInfo {id = 51, PairsGameImgFiles = "BOY.png"},
                    ["GIRL"] = new ItemInfo {id = 61, PairsGameImgFiles = "GIRL.png"},
                    ["WOMAN"] = new ItemInfo {id = 71, PairsGameImgFiles = "WOMAN.png"},
                    ["MAN"] = new ItemInfo {id = 81, PairsGameImgFiles = "MAN.png"},
                },
                Games = new List<GAME_TYPE> {GAME_TYPE.FIND_THE_PAIR, GAME_TYPE.CAROUSEL, GAME_TYPE.QUIZ}
            },  

        };

        public static string GetTileImageSourceForPairsGame(THEME_NAME curThemeName, string itemKey)
        {
            return "DinoLingo.Resources." + Resources[(int)curThemeName].ImgFolder + "." + Resources[(int)curThemeName].Item[itemKey].PairsGameImgFiles;
        }

        public static string GetTileImageSourceForPairsGameCocos(THEME_NAME curThemeName, string itemKey)
        {
            return "Images/" + Resources[(int)curThemeName].ImgFolder + "/" + Resources[(int)curThemeName].Item[itemKey].PairsGameImgFiles;
        }

        public static THEME_NAME GetThemeOfGame(string title) {
            string t = title.ToLower();
            foreach (ThemeResource res in Resources) {
                if (t.Contains(res.PostKeyWord)) return res.Name;
            }
            return THEME_NAME.ACTIONS;
        }

        public static int GetIndexForKeyId(THEME_NAME theme, int id) {
            int index = 0;
            for (THEME_NAME name = THEME_NAME.ACTIONS; name <= THEME_NAME.FAMILY; name++) {
                if (theme != name) {
                    index += Resources[(int)name].Item.Count;
                    continue;
                }
                foreach (KeyValuePair<string, ItemInfo> pair in Resources[(int) name].Item) {
                    if (pair.Value.id == id) {
                        return index;
                    }
                    index++;
                }
            }

            return -1;
        }
    }



    public class ThemeResource
    {
        public THEME_NAME Name { get; set; }
        public string StringName { get; set; }
        public string ImgFolder { get; set; }
        public string PostKeyWord { get; set; }
        //public string[] PairsGameImgFiles { get; set; }
        //public List<int> ImgIds { get; set; }
        public Dictionary<string, ItemInfo> Item { get; set; }
        public List<GAME_TYPE> Games { get; set; } = new List<GAME_TYPE> { GAME_TYPE.FIND_THE_PAIR, GAME_TYPE.CAROUSEL, GAME_TYPE.SEE_AND_SAY, GAME_TYPE.QUIZ };

    }

    public class ItemInfo {
        public string PairsGameImgFiles { get; set; }
        public int id { get; set; }
    }



    public class GameObjects {
        public string Lang_Cat { get; set; }
        public string LANG { get; set; }
        public SoundItem[] ACTIONS { get; set; }
        public SoundItem[] NUMBERS { get; set; }
        public SoundItem[] ANIMALS { get; set; }
        public SoundItem[] BODY_PARTS { get; set; }
        public SoundItem[] CLOTHES { get; set; }

        public SoundItem[] COLORS { get; set; }
        public SoundItem[] VEHICLES { get; set; }
        public SoundItem[] FOOD { get; set; }
        public SoundItem[] FRUIT_AND_VEGETABLES { get; set; }
        public SoundItem[] IN_THE_HOUSE { get; set; }
        public SoundItem[] NATURE { get; set; }
        public SoundItem[] FAMILY { get; set; }

        public static string UniformIncomingStringToObject(string incomingString) {
            // delete 'DL = ' and ';'
            string s;
            s = incomingString.Replace("DL = ", "").Replace(";", "");
            // replace KEYS with SPACE
            s = s.Replace("BODY PARTS", "BODY_PARTS")
                 .Replace("FRUIT AND VEGETABLES", "FRUIT_AND_VEGETABLES")
                 .Replace("IN THE HOUSE", "IN_THE_HOUSE");
            s = s.Replace("\'", "\"");

            return s;
        }

        public string GetSoundUrl(THEME_NAME theme, string key) {
            string s = string.Empty;
            SoundItem[] soundItems = GetSoundItemsForTheme(theme);
            if (soundItems == null) return s;
            foreach (SoundItem soundItem in soundItems)
            {
                if (soundItem == null) continue;
                if (soundItem.id.ToLower() == key.ToLower()) return soundItem.sound;
            }
            return s;
        }

        public string GetTextForSound(THEME_NAME theme, string key)
        {
            string s = string.Empty;
            SoundItem[] soundItems = GetSoundItemsForTheme(theme);
            if (soundItems == null) return s;
            foreach (SoundItem soundItem in soundItems)
            {
                if (soundItem == null) continue;
                if (soundItem.id.ToLower() == key.ToLower()) return soundItem.text;
            }
            return s;
        }


        public bool HasKey(THEME_NAME theme, string key) {
            SoundItem[] soundItems = GetSoundItemsForTheme(theme);
            if (soundItems == null) return false;
            foreach (SoundItem soundItem in soundItems) {
                if (soundItem == null) continue;
                if (soundItem.id.ToLower() == key.ToLower() && !string.IsNullOrEmpty(soundItem.sound)) return true;
            }
            return false;
        }

        public SoundItem[] GetSoundItemsForTheme(THEME_NAME themeName) {
            switch (themeName) {
                case THEME_NAME.ACTIONS: return ACTIONS;
                case THEME_NAME.ANIMALS: return ANIMALS;
                case THEME_NAME.BODYPARTS: return BODY_PARTS;
                case THEME_NAME.CLOTHES: return CLOTHES;
                case THEME_NAME.COLORS: return COLORS;
                case THEME_NAME.FAMILY: return FAMILY;
                case THEME_NAME.FOOD: return FOOD;
                case THEME_NAME.FRUITSANDVEGETABLES: return FRUIT_AND_VEGETABLES;
                case THEME_NAME.HOUSE: return IN_THE_HOUSE;
                case THEME_NAME.NATURE: return NATURE;
                case THEME_NAME.NUMBERS: return NUMBERS;
                case THEME_NAME.VEHICLES: return VEHICLES;
                default: return NUMBERS;  
            }
        }

        public void ChangeIdToKeys () {
            for (THEME_NAME theme = THEME_NAME.ACTIONS; theme <= THEME_NAME.FAMILY; theme++) {
                List<string> keys = new List<string>(Theme.Resources[(int)theme].Item.Keys);
                SoundItem[] soundItemsOld = GetSoundItemsForTheme(theme);
                SoundItem[] soundItemsNew = new SoundItem[keys.Count];

                foreach (SoundItem itemOld in soundItemsOld) {
                    for (int i = 0; i < soundItemsNew.Length; i++) {
                        
                        if (Theme.Resources[(int)theme].Item[keys[i]].id.ToString() == itemOld.id) {
                            itemOld.id = keys[i];
                            soundItemsNew[i] = itemOld;
                            break;
                        }
                    }                    
                }
                switch (theme)
                {
                    case THEME_NAME.ACTIONS: ACTIONS = soundItemsNew; break;
                    case THEME_NAME.ANIMALS: ANIMALS = soundItemsNew; break;
                    case THEME_NAME.BODYPARTS: BODY_PARTS = soundItemsNew; break;
                    case THEME_NAME.CLOTHES: CLOTHES = soundItemsNew; break;
                    case THEME_NAME.COLORS: COLORS = soundItemsNew; break;
                    case THEME_NAME.FAMILY: FAMILY = soundItemsNew; break;
                    case THEME_NAME.FOOD: FOOD = soundItemsNew; break;
                    case THEME_NAME.FRUITSANDVEGETABLES: FRUIT_AND_VEGETABLES = soundItemsNew; break;
                    case THEME_NAME.HOUSE: IN_THE_HOUSE = soundItemsNew; break;
                    case THEME_NAME.NATURE: NATURE = soundItemsNew; break;
                    case THEME_NAME.NUMBERS: NUMBERS = soundItemsNew; break;
                    case THEME_NAME.VEHICLES: VEHICLES = soundItemsNew; break;
                }
            }
        }

        public void OrderByKeys()
        {
            for (THEME_NAME theme = THEME_NAME.ACTIONS; theme <= THEME_NAME.FAMILY; theme++)
            {
                List<string> keys = new List<string>(Theme.Resources[(int)theme].Item.Keys);
                SoundItem[] soundItemsOld = GetSoundItemsForTheme(theme);
                SoundItem[] soundItemsNew = new SoundItem[keys.Count];
                if (soundItemsOld == null) continue; 
                foreach (SoundItem itemOld in soundItemsOld)
                {
                    for (int i = 0; i < soundItemsNew.Length; i++)
                    {
                        
                        if (itemOld != null && keys[i].ToLower() == itemOld.id)
                        {
                            itemOld.id = keys[i];
                            soundItemsNew[i] = itemOld;
                            break;
                        }
                    }
                }
                switch (theme)
                {
                    case THEME_NAME.ACTIONS: ACTIONS = soundItemsNew; break;
                    case THEME_NAME.ANIMALS: ANIMALS = soundItemsNew; break;
                    case THEME_NAME.BODYPARTS: BODY_PARTS = soundItemsNew; break;
                    case THEME_NAME.CLOTHES: CLOTHES = soundItemsNew; break;
                    case THEME_NAME.COLORS: COLORS = soundItemsNew; break;
                    case THEME_NAME.FAMILY: FAMILY = soundItemsNew; break;
                    case THEME_NAME.FOOD: FOOD = soundItemsNew; break;
                    case THEME_NAME.FRUITSANDVEGETABLES: FRUIT_AND_VEGETABLES = soundItemsNew; break;
                    case THEME_NAME.HOUSE: IN_THE_HOUSE = soundItemsNew; break;
                    case THEME_NAME.NATURE: NATURE = soundItemsNew; break;
                    case THEME_NAME.NUMBERS: NUMBERS = soundItemsNew; break;
                    case THEME_NAME.VEHICLES: VEHICLES = soundItemsNew; break;
                }
            }
        }

    }

    public class SoundItem {
        public string id { get; set; }
        public string text { get; set; }
        public string sound { get; set; }

    }
}
