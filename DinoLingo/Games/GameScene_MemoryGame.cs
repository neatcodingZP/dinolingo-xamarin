using CocosSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DinoLingo.Games
{
    class GameScene_MemoryGame : CCScene
    {
        CCLayer gameLayer;

        CCSprite[] gameSprites;
        CCSprite[] backGrounds;
        CCSprite[] foreGrounds;

        public MemoryGameModel memoryGameModel;

        float baseScaleXbackground = 1.0f;
        float baseScaleXobject = 1.0f;

        float ANIMATION_RATE =  1 / 0.7f;
        THEME_NAME theme;
        List<string> randomCards;
        bool restart = false;

        public GameScene_MemoryGame(CCGameView gameView) : base(gameView)
        {
                       
            gameLayer = new CCLayer();            

            CCSprite backGround = new CCSprite("Images/MEMORY_GAME/logo_bg_v2.png");
            gameLayer.AddChild(backGround);
            backGround.ScaleY = (float)(100) / backGround.TextureRectInPixels.MaxY;
            backGround.ScaleX = (float)(100) / backGround.TextureRectInPixels.MaxX;
            backGround.AnchorPoint =  CCPoint.AnchorMiddle;            
            backGround.Position = new CCPoint(50, 50);

            // Register for touch events
            var touchListener = new CCEventListenerTouchAllAtOnce();
            
            touchListener.OnTouchesBegan = OnTouchesBegan;
            AddEventListener(touchListener, gameLayer);

            AddLayer(gameLayer);
            memoryGameModel = new MemoryGameModel();
            CreateAllSprites();

            Schedule(RunGameLogic);
        }

        public void Restart(THEME_NAME theme, List<string> randomCards)
        {
            if (restart) return;
            restart = true;
            this.theme = theme;
            this.randomCards = randomCards;
        }

        void RunGameLogic(float frameTimeInSeconds)
        {
            if (restart)
            {
                DoOnRestart();
                return;
            }

            for (int i = 0; i < 16; i++)
            {
                switch (memoryGameModel.states[i])
                {
                    case MemoryGameModel.STATE.CLOSED:
                    case MemoryGameModel.STATE.DONE:
                    case MemoryGameModel.STATE.OPENED:
                        break;
                    case MemoryGameModel.STATE.OPENING:
                        // just do opening
                        AnimateOpening(i, frameTimeInSeconds);
                        break;
                    case MemoryGameModel.STATE.OPENING_TO_DONE:
                        // just do opening
                        AnimateOpeningToDone(i, frameTimeInSeconds);
                        break;
                    case MemoryGameModel.STATE.OPENING_TO_CLOSE:
                        // just do opening
                        AnimateOpeningThenClose(i, frameTimeInSeconds);
                        break;
                    case MemoryGameModel.STATE.CLOSING:
                        // just do opening
                        AnimateClosing(i, frameTimeInSeconds);
                        break;
                    case MemoryGameModel.STATE.CLOSE_AT_ONCE:
                        CloseAtOnce(i);
                        break;
                    case MemoryGameModel.STATE.DONE_AT_ONCE:
                        DoneAtOnce(i);
                        break;
                }
            }
        }

        void DoneAtOnce(int i)
        {
            foreGrounds[i].Visible = false;
            backGrounds[i].Visible = false;
            gameSprites[i].Visible = false;
           
            memoryGameModel.states[i] = MemoryGameModel.STATE.DONE;
        }

        void CloseAtOnce(int i)
        {
            foreGrounds[i].Visible = true;
            backGrounds[i].Visible = true;
            gameSprites[i].Visible = true;
            gameSprites[i].ScaleX = baseScaleXobject;
            backGrounds[i].ScaleX = foreGrounds[i].ScaleX = baseScaleXbackground;
            memoryGameModel.animProgress[i] = 0;
            memoryGameModel.states[i] = MemoryGameModel.STATE.CLOSED;
        }

        void AnimateOpeningToDone(int i, float frameTimeInSeconds)
        {            
            memoryGameModel.animProgress[i] += frameTimeInSeconds * ANIMATION_RATE;
            if (foreGrounds[i].Visible) // first part of animation
            {
                if (memoryGameModel.animProgress[i] > 0.5f)
                {
                    foreGrounds[i].Visible = false;
                }
                float scale = 1 - (memoryGameModel.animProgress[i]) * 2;
                if (scale < 0) scale = 0;
                gameSprites[i].ScaleX = scale * baseScaleXobject;
                backGrounds[i].ScaleX = foreGrounds[i].ScaleX = scale * baseScaleXbackground;
            }
            else // second part
            {
                if (memoryGameModel.animProgress[i] > 1)
                {
                    gameSprites[i].ScaleX = baseScaleXobject;
                    backGrounds[i].ScaleX = foreGrounds[i].ScaleX = baseScaleXbackground;
                    memoryGameModel.animProgress[i] = 1;                    

                    // check if second card finished opening
                    if (memoryGameModel.CheckBothFinishedAnim())
                    {
                        DoneAtOnce(memoryGameModel.currentIndexes[0]);
                        DoneAtOnce(memoryGameModel.currentIndexes[1]);
                        
                            memoryGameModel.animProgress[memoryGameModel.currentIndexes[1]] = -1;
                    }
                }
                else
                {
                    float scale = memoryGameModel.animProgress[i] * 2 - 1;
                    gameSprites[i].ScaleX = scale * baseScaleXobject;
                    backGrounds[i].ScaleX = foreGrounds[i].ScaleX = scale * baseScaleXbackground;
                }
            }
        }

        void AnimateOpeningThenClose(int i, float frameTimeInSeconds)
        {       
            memoryGameModel.animProgress[i] += frameTimeInSeconds * ANIMATION_RATE;
            if (foreGrounds[i].Visible) // first part of animation
            {
                if (memoryGameModel.animProgress[i] > 0.5f)
                {
                    foreGrounds[i].Visible = false;
                }
                float scale = 1 - (memoryGameModel.animProgress[i]) * 2;
                if (scale < 0) scale = 0;
                gameSprites[i].ScaleX = scale * baseScaleXobject;
                backGrounds[i].ScaleX = foreGrounds[i].ScaleX = scale * baseScaleXbackground;
            }
            else // second part
            {
                if (memoryGameModel.animProgress[i] > 1)
                {
                    gameSprites[i].ScaleX = baseScaleXobject;
                    backGrounds[i].ScaleX = foreGrounds[i].ScaleX = baseScaleXbackground;
                    memoryGameModel.animProgress[i] = 1;                    

                    // check if second card finished opening
                    if (memoryGameModel.CheckBothFinishedAnim())
                    {
                        memoryGameModel.states[memoryGameModel.currentIndexes[0]] =
                            memoryGameModel.states[memoryGameModel.currentIndexes[1]] = MemoryGameModel.STATE.CLOSING;
                        memoryGameModel.animProgress[memoryGameModel.currentIndexes[0]] =
                            memoryGameModel.animProgress[memoryGameModel.currentIndexes[1]] = 0;
                    }
                }
                else
                {
                    float scale = memoryGameModel.animProgress[i] * 2 - 1;
                    gameSprites[i].ScaleX = scale * baseScaleXobject;
                    backGrounds[i].ScaleX = foreGrounds[i].ScaleX = scale * baseScaleXbackground;
                }
            }
        }

        void AnimateOpening(int i, float frameTimeInSeconds)
        {    
            memoryGameModel.animProgress[i] += frameTimeInSeconds * ANIMATION_RATE;
            if (foreGrounds[i].Visible) // first part of animation
            {
                if (memoryGameModel.animProgress[i] > 0.5f)
                {
                    foreGrounds[i].Visible = false;
                }
                float scale = 1 - (memoryGameModel.animProgress[i]) * 2;
                if (scale < 0) scale = 0;
                gameSprites[i].ScaleX = scale * baseScaleXobject;
                backGrounds[i].ScaleX = foreGrounds[i].ScaleX = scale * baseScaleXbackground;                
            }
            else // second part
            {
                if (memoryGameModel.animProgress[i] > 1)
                {
                    gameSprites[i].ScaleX = baseScaleXobject;
                    backGrounds[i].ScaleX = foreGrounds[i].ScaleX = baseScaleXbackground;
                    memoryGameModel.animProgress[i] = 1;
                    memoryGameModel.states[i] = MemoryGameModel.STATE.OPENED;
                }
                else
                {
                    float scale = memoryGameModel.animProgress[i] * 2 - 1 ;
                    gameSprites[i].ScaleX = scale * baseScaleXobject;
                    backGrounds[i].ScaleX = foreGrounds[i].ScaleX = scale * baseScaleXbackground;
                }
            }            
        }

        void AnimateClosing(int i, float frameTimeInSeconds)
        {   
            memoryGameModel.animProgress[i] += frameTimeInSeconds * ANIMATION_RATE;
            if (!foreGrounds[i].Visible) // first part of animation
            {
                if (memoryGameModel.animProgress[i] > 0.5f)
                {
                    foreGrounds[i].Visible = true;
                }
                float scale = 1 - (memoryGameModel.animProgress[i]) * 2;
                if (scale < 0) scale = 0;
                gameSprites[i].ScaleX = scale * baseScaleXobject;
                backGrounds[i].ScaleX = foreGrounds[i].ScaleX = scale * baseScaleXbackground;
            }
            else // second part
            {
                if (memoryGameModel.animProgress[i] > 1)
                {
                    gameSprites[i].ScaleX = baseScaleXobject;
                    backGrounds[i].ScaleX = foreGrounds[i].ScaleX = baseScaleXbackground;
                    memoryGameModel.animProgress[i] = 0;
                    memoryGameModel.states[i] = MemoryGameModel.STATE.CLOSED;

                    // here we must reset cards
                    memoryGameModel.currentIndexes[0] = memoryGameModel.currentIndexes[1] = -1;
                }
                else
                {
                    float scale = memoryGameModel.animProgress[i] * 2 - 1;
                    gameSprites[i].ScaleX = scale * baseScaleXobject;
                    backGrounds[i].ScaleX = foreGrounds[i].ScaleX = scale * baseScaleXbackground;
                }
            }
        }

        void OnTouchesBegan(List<CCTouch> touches, CCEvent touchEvent)
        {
            
            if (touches.Count > 0)
            {         
                    Debug.WriteLine($"touch began !, touch.Location.X = {touches[0].Location.X}, touch.Location.Y = {touches[0].Location.Y}");
                    int row = (int)touches[0].Location.Y / 25;
                    int col = (int)touches[0].Location.X / 25;
                    int index = (3 - row) * 4 + col;
                    Debug.WriteLine($"memoryGameModel.objectId =  {memoryGameModel.objectIds[index]}");
                    memoryGameModel.ObjectTouched(index);                
            }
        }

        void DoOnRestart()
        {
            memoryGameModel.Reset();
            memoryGameModel.SetIds(randomCards);

            for (int i = 0; i < gameSprites.Length; i++)
            {
                CloseAtOnce(i);                
                gameSprites[i].Texture = new CCTexture2D(Theme.GetTileImageSourceForPairsGameCocos(theme, randomCards[i]));
                gameSprites[i].ScaleY = gameSprites[i].ScaleX = (float)(100 * 0.98) / gameSprites[i].TextureRectInPixels.MaxX * 0.25f;
            }
            restart = false;

            baseScaleXobject = gameSprites[0].ScaleX;            
        }

        public void CreateAllSprites()
        {
            gameSprites = new CCSprite[16];
            backGrounds = new CCSprite[16];
            foreGrounds = new CCSprite[16];

            
            for (int i = 0; i < gameSprites.Length; i++)
            {
                backGrounds[i] = new CCSprite("Images/MEMORY_GAME/square_white.png");
                gameLayer.AddChild(backGrounds[i]);

                gameSprites[i] = new CCSprite();
                gameLayer.AddChild(gameSprites[i]);

                foreGrounds[i] = new CCSprite("Images/MEMORY_GAME/square_blue.png");
                gameLayer.AddChild(foreGrounds[i]);
                backGrounds[i].ScaleY = backGrounds[i].ScaleX = foreGrounds[i].ScaleY = foreGrounds[i].ScaleX = (float)(100 * 0.98) / foreGrounds[i].TextureRectInPixels.MaxX * 0.25f * 0.95f;

                backGrounds[i].AnchorPoint = foreGrounds[i].AnchorPoint = gameSprites[i].AnchorPoint = CCPoint.AnchorMiddle;

                // set position
                int row = i / 4;
                int col = i % 4;
                backGrounds[i].Position = foreGrounds[i].Position = gameSprites[i].Position = new CCPoint(12.5f + 25 * col, 87.5f - 25 * row);
            }
            baseScaleXbackground = backGrounds[0].ScaleX;
        }
    }
}
