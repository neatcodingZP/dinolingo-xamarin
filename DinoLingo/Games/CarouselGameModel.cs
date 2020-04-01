using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace DinoLingo.Games
{   

    public class CarouselGameModel
    {
        public string targetItemKey;

        public List<string> randomKeys;
        
        public List<string> playingItems;
        public bool[] isOpenedOnWheel;
        public bool[] isOpenedOnLine;
        int START_SHOTS = 16;
        public int targets, aimed, misses;

        public Action DoOnMatchesThePair;
        public Action DoOnMissedThePair;

        Random random = new Random();

        public bool canClick = true;

        public CarouselGameModel()
        {
            isOpenedOnWheel = new bool[] { false, false, false, false, false };
            isOpenedOnLine = new bool[] { false, false, false, false, false };
            targets = START_SHOTS / 2;
            aimed = misses = 0;

        }

        public void Restart(List<string> randomKeys)
        {
            this.randomKeys = randomKeys;
            playingItems = new List<string>(randomKeys);
            isOpenedOnWheel = new bool[] { false, false, false, false, false };
            isOpenedOnLine = new bool[] { false, false, false, false, false };
            targets = START_SHOTS / 2;
            aimed = misses = 0;
            SetNextTarget();
            canClick = true;
        }

        public void SetNextTarget()
        {
            if (playingItems.Count > 0)            targetItemKey = playingItems[random.Next(0, playingItems.Count)];
        }

        public void TouchOnLine (int index)
        {
            TouchObject(index +5);
        }

        public void TouchOnCircle(int index)
        {
            TouchObject(index);
        }

        void TouchObject(int index)
        {
            Debug.WriteLine("CarouselGameModel -> TouchObject -> index =" + index);

            
            //check if we missed ?
            if (randomKeys[index] == targetItemKey)
            {
                canClick = false;
                Debug.WriteLine("CarouselGameModel -> TouchObject -> HIT");
                aimed++;
                targets--;
                if (index < 5)
                {
                    isOpenedOnWheel[index] = true;
                }
                else
                {
                    isOpenedOnLine[index - 5] = true;
                }

                playingItems.Remove(targetItemKey);
                SetNextTarget();
                Device.BeginInvokeOnMainThread(()=>
                {
                    DoOnMatchesThePair?.Invoke();
                });
                
            }
            else
            {
                Debug.WriteLine("CarouselGameModel -> TouchObject -> MISSED");
                misses++;
                Device.BeginInvokeOnMainThread(() =>
                {
                    DoOnMissedThePair?.Invoke();
                });
                
                
            }
        }
    }
}
