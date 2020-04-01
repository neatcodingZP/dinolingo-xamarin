using System;
using System.Collections.Generic;
using System.Text;

namespace DinoLingo.Games
{
    class MemoryGameModel
    {
        public enum STATE {CLOSED, OPENING, OPENING_TO_DONE, OPENING_TO_CLOSE, CLOSE_AT_ONCE, CLOSING, OPENED, DONE, DONE_AT_ONCE};

        List<STATE> START_STATES = new List<STATE> {
            STATE.CLOSED, STATE.CLOSED, STATE.CLOSED, STATE.CLOSED, STATE.CLOSED, STATE.CLOSED, STATE.CLOSED, STATE.CLOSED,
            STATE.CLOSED, STATE.CLOSED, STATE.CLOSED, STATE.CLOSED, STATE.CLOSED, STATE.CLOSED, STATE.CLOSED, STATE.CLOSED,};

        public STATE[] states = new STATE[16];
        public float[] animProgress = new float[16];
        public int[] currentIndexes = new int[2];
        public string[] objectIds;
        public int pairsDone;

        public Action DoOnMatchesThePair;
        public Action DoOnMissedThePair;
        public Action<int> DoOnOpenCard;

        public MemoryGameModel ()
        {
            Reset();
        }

        public void Reset ()
        {
            for (int i = 0; i < 16; i++)
            {
                states[i] = START_STATES[i];
                animProgress[i] = 0;
            }
            currentIndexes[0] = currentIndexes[1] = -1;
            pairsDone = 0;
        }

        public void SetIds(List<string> ids)
        {
            objectIds = new string[ids.Count];
            for (int i = 0; i < objectIds.Length; i++)
            {
                objectIds[i] = ids[i];
            }
        }

        public void ObjectTouched(int index)
        {
            if (states[index] == STATE.OPENING_TO_DONE || states[index] == STATE.DONE || states[index] == STATE.DONE_AT_ONCE) return;

            if (currentIndexes[0] == -1) // nothing was touched
            {
                currentIndexes[0] = index;
                states[index] = STATE.OPENING;
                animProgress[index] = 0;

                DoOnOpenCard?.Invoke(index);
            }
            else if (currentIndexes[1] == -1) // 1 object touched
            {
                if (currentIndexes[0] == index) return;
                currentIndexes[1] = index;


                DoOnOpenCard?.Invoke(index);
                // check conditions
                if (CheckIfHavePair()) // we matched here!
                {
                    
                    pairsDone++;
                    states[currentIndexes[1]] = STATE.OPENING_TO_DONE;
                    animProgress[currentIndexes[1]] = 0;

                    // check first object
                    switch (states[currentIndexes[0]])
                    {
                        case STATE.OPENING:
                            states[currentIndexes[0]] = STATE.OPENING_TO_DONE;
                            break;
                        case STATE.OPENED:
                            states[currentIndexes[0]] = STATE.DONE;
                            break;
                    }
                    // ===  DO ON MATCHED === 
                    DoOnMatchesThePair?.Invoke();
                }
                else // we missed !
                {
                    DoOnOpenCard?.Invoke(index);
                    // ===  DO ON MISSED === 
                    DoOnMissedThePair?.Invoke();
                    for (int i = 0; i < 2; i++)
                    {
                        // check each object
                        switch (states[currentIndexes[i]])
                        {
                            case STATE.OPENING:
                                states[currentIndexes[i]] = STATE.OPENING_TO_CLOSE;
                                break;                           
                            case STATE.CLOSED:
                                states[currentIndexes[i]] = STATE.OPENING_TO_CLOSE;
                                animProgress[currentIndexes[i]] = 0;
                                break;
                        }
                    }
                }
            } 
            else // we already have 2 objects
            {
                DoOnOpenCard?.Invoke(index);

                // forbid touch any of them
                if (index == currentIndexes[0] || index == currentIndexes[1]) return;

                if (CheckIfHavePair()) // force DONE
                {
                    states[currentIndexes[0]] = states[currentIndexes[1]] = STATE.DONE_AT_ONCE;
                    animProgress[currentIndexes[0]] = animProgress[currentIndexes[1]] = 0;
                }
                else // force close
                {
                    states[currentIndexes[0]] = states[currentIndexes[1]] = STATE.CLOSE_AT_ONCE;
                    animProgress[currentIndexes[0]] = animProgress[currentIndexes[1]] = 0;
                    
                }
                currentIndexes[0] = index;
                currentIndexes[1] = -1;
                states[index] = STATE.OPENING;
                animProgress[index] = 0;
            }
        }

        public bool CheckIfHavePair()
        {
            return (currentIndexes[0] > -1 && currentIndexes[1] > -1 && objectIds[currentIndexes[0]] == objectIds[currentIndexes[1]]);
        }

        public bool CheckWin()
        {
            return pairsDone >= 8;
        }

        public bool CheckBothFinishedAnim()
        {
            return animProgress[currentIndexes[0]] == 1 && animProgress[currentIndexes[1]] == 1;
        }

        
    }
}
