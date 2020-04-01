using System;
using System.Collections.Generic;
using System.Text;

namespace DinoLingo.Games
{

    class SprintGameModel
    {
        public THEME_NAME themeName;
        public List<string> wordsToPlay;

        public int TOTAL_WORDS_IN_SPRINT = 20;
        public int MINIMUM_CORRECT = 15;

        public enum State {CLOSED, PLAYING, WAITING_FOR_NEXT, TIME_FINISHED, }
        public State state = State.WAITING_FOR_NEXT;
        int keyIndex = -1;
        public string targetKey = string.Empty;
        public string saidKey = string.Empty;
        public float totalTime = 0;
        //public float FINISH_TIME = 30;
        public float lastWordTime = 0;

        float TIME_TO_REPEAT_WORD = 5.0f;
        public int score = 0;
        public int streak = 0;
        public int correct = 0;
        public int wrong = 0;

        public Random random = new Random();

        public SprintGameModel(THEME_NAME themeName) {
            this.themeName = themeName;
        }

        public void Restart()
        {
            keyIndex = -1;
            targetKey = string.Empty;
            saidKey = string.Empty;
            totalTime = 0;
        
            lastWordTime = 0;
        
            score = 0;
            streak = 0;
            correct = 0;
            wrong = 0;

            state = State.WAITING_FOR_NEXT;
        }

        public List<string> GetWordsToPlay()
        {
            //get list from gameObjects
            List<string> wordsToDownload1 = new List<string>(Theme.Resources[(int)themeName].Item.Keys);
            foreach (string key in Theme.Resources[(int)themeName].Item.Keys)
            {
                if (GameHelper.memory_GameObjects.HasKey(themeName, key) || GameHelper.sas_GameObjects.HasKey(themeName, key))
                {
                    continue;
                }
                wordsToDownload1.Remove(key);
            }
            wordsToPlay = wordsToDownload1;
            
            // shuffle list here
            for (int i = 0; i < wordsToPlay.Count; i++)
            {
                string rs = wordsToPlay[i];
                int newIndex = random.Next(0, wordsToPlay.Count);
                wordsToPlay[i] = wordsToPlay[newIndex];
                wordsToPlay[newIndex] = rs;
            }

            return wordsToPlay;
        }

        public string GetNextKey()
        {
            keyIndex++;
            if (keyIndex > wordsToPlay.Count - 1) keyIndex = 0;
            targetKey = wordsToPlay[keyIndex];
            return targetKey;
        }

        public string GetKeyToSay()
        {
            // true or false
            if (random.Next(0,2) > 0)
            {
                saidKey = targetKey;
            }
            else
            {
                while ((saidKey = wordsToPlay[random.Next(0, wordsToPlay.Count)]) == targetKey)
                {

                }                
            }
            return saidKey;
        }

        public bool NeedToRepeatWord()
        {
            return (totalTime - lastWordTime > TIME_TO_REPEAT_WORD);
        }

        public void UpdateScore (bool correct)
        {
            if (correct)
            {
                streak++;
                this.correct++;
            }
            else
            {
                streak = 0;
                wrong++;
            }

            if (streak < 2) score += 10;
            else if (streak < 4) score += 20;
            else score += 30;
        }
    }
}
