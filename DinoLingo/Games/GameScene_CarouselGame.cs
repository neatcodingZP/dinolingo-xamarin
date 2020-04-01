using CocosSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DinoLingo.Games
{
    public class GameScene_CarouselGame : CCScene
    {
        CCLayer gameLayer;        
        
        // === some const ===
        static float SIZE_X = 100;
        static float SIZE_Y = 73;
        static float TILE_FACTOR = 0.15f;
        static float TILE_CONTENT_FACTOR = 0.13f;
        static float LINE_X_DISTANCE_FACTOR = 0.25f;
        static float LINE_ANIM_TIME = 7.0f;
        static float LINE_ANIM_DISTANCE = 0.3f;

        static float WHEEL_ROT_ANGLE_PER_SEC = 20.0f;
        static float MAX_ANGLE = 180 + 22.5f;
        static float MIN_ANGLE = -22.5f;


        CCPoint wheelCenter; 
        float wheelSizeFactor = 0.8f;
        float radiusSizeFactor = 0.75f * 0.5f;
        float  moveLayerStartY;

        CCSprite[] spritesOnWheel = new CCSprite[5];
        CCSprite[] spritesOnWheelContent = new CCSprite[5];
        CCSprite[] spritesOnLine = new CCSprite[5];
        CCSprite[] spritesOnLineContent = new CCSprite[5];

        float baseScaleTile = 1.0f;
        float baseScaleContent = 1.0f;

        float ANIMATION_RATE = 1 / 0.7f;
        float[] wheelAnimationStates = new float[5];
        float[] lineAnimationStates = new float[5];
        float[] rotations = new float[5];
        float lineAnimation;
        float pi_180 = (float) Math.PI / 180;
        bool[] wheelAnimating = new bool[5];
        bool[] lineAnimating = new bool[5];

        public CarouselGameModel gameModel;
        THEME_NAME theme;
        
        bool restart = false;
        public bool pause = true;
        int needToCreateSprites = -1;
        bool canTouch = true;

        public GameScene_CarouselGame(CCGameView gameView) : base(gameView)
        {
            SIZE_X = GameView.DesignResolution.Width;
            SIZE_Y = GameView.DesignResolution.Height;
            wheelCenter = new CCPoint(SIZE_X * 0.5f, SIZE_Y);
            moveLayerStartY = SIZE_Y * 0.25f;

            gameModel = new CarouselGameModel();
            gameLayer = new CCLayer();
            
            CCLayer backgroundLayer = new CCLayer();

            CCSprite backGround = new CCSprite("Images/CAROUSEL_GAME/bg.jpg");
            backgroundLayer.AddChild(backGround);

            backGround.ScaleX = SIZE_X / backGround.TextureRectInPixels.MaxX * 1.05f;
            backGround.ScaleY = SIZE_Y / backGround.TextureRectInPixels.MaxY * 1.05f;
            backGround.AnchorPoint = CCPoint.Zero;
            backGround.Position = CCPoint.Zero;

            CCSprite wheel = new CCSprite("Images/CAROUSEL_GAME/round2.png");
            backgroundLayer.AddChild(wheel);
            wheel.ScaleY = wheel.ScaleX = SIZE_X / wheel.TextureRectInPixels.MaxX * wheelSizeFactor;
            wheel.AnchorPoint = new CCPoint (0.5f, 0.9f);
            wheel.Position = wheelCenter;

            CCSprite line = new CCSprite("Images/CAROUSEL_GAME/line2.png");
            backgroundLayer.AddChild(line);
            line.ScaleY = wheel.ScaleY;
            line.ScaleX = SIZE_X / line.TextureRectInPixels.MaxX;
            line.AnchorPoint = CCPoint.AnchorMiddleLeft;
            line.Position = new CCPoint (0, moveLayerStartY);

            // Register for touch events
            var touchListener = new CCEventListenerTouchAllAtOnce();
            touchListener.OnTouchesBegan = OnTouchesBegan;
            //touchListener.OnTouchesEnded = OnTouchesEnded;
            //touchListener.OnTouchesCancelled = OnTouchesEnded;
            AddEventListener(touchListener, gameLayer);            


            AddLayer(backgroundLayer);
            AddLayer(gameLayer);

            
            
            Schedule(RunGameLogic);
        }

        void RunGameLogic(float frameTimeInSeconds)
        {
            if (needToCreateSprites < 0) return;
            if (needToCreateSprites == 0)
            {
                CreateAllSprites();
                needToCreateSprites = 1;
            }

            if (restart)
            {
                DoOnRestart();
                return;
            }

            if (pause) return;
            //do wheel
            float deltaAngle = WHEEL_ROT_ANGLE_PER_SEC * frameTimeInSeconds;
            for (int i = 0; i < spritesOnLine.Length; i++)
            {
                if (spritesOnWheel[i].Visible)
                {
                    rotations[i] += deltaAngle;
                    // check rotations here
                    if (rotations[i] > MAX_ANGLE)
                    {
                        rotations[i] += MIN_ANGLE - MAX_ANGLE;
                    }

                    spritesOnWheelContent[i].Rotation = -90 + rotations[i];
                    float xx = SIZE_X * radiusSizeFactor * (float)Math.Cos(rotations[i] * pi_180);
                    float y = SIZE_X * radiusSizeFactor * (float)Math.Sin(rotations[i] * pi_180);
                    spritesOnWheelContent[i].Position = spritesOnWheel[i].Position = new CCPoint(wheelCenter.X + xx, wheelCenter.Y - y);

                    //do animation here
                    if (wheelAnimating[i])
                    {
                        wheelAnimationStates[i] += frameTimeInSeconds * ANIMATION_RATE;
                        if (wheelAnimationStates[i] > 1)
                        {
                            // end of animation
                            wheelAnimating[i] = false;
                            if (gameModel.isOpenedOnWheel[i])
                            {
                                spritesOnWheelContent[i].Visible = spritesOnWheel[i].Visible = false;
                            }
                        }
                        // set current scale
                        float newScale = GetScaleForSpriteAnim(wheelAnimationStates[i]) * 0.15f;
                        spritesOnWheel[i].ScaleY = spritesOnWheel[i].ScaleX = baseScaleTile * (1 + newScale);
                        spritesOnWheelContent[i].ScaleY = spritesOnWheelContent[i].ScaleX = baseScaleContent * (1 + newScale);
                    }
                }
            }



            //do line
            lineAnimation += frameTimeInSeconds / LINE_ANIM_TIME;
            if (lineAnimation > 1) lineAnimation = lineAnimation - 1;
            float x = GetXFromAnimX(lineAnimation);
            float startOffsetX = ((1 - LINE_X_DISTANCE_FACTOR * 4) * 0.5f + x * LINE_ANIM_DISTANCE) * SIZE_X;
            float halfWidth = LINE_X_DISTANCE_FACTOR * SIZE_X * 0.5f;
            for (int i = 0; i < spritesOnLine.Length; i++)
            {
                if (spritesOnLine[i].Visible)
                {
                    spritesOnLineContent[i].Position = spritesOnLine[i].Position = new CCPoint(startOffsetX + i * SIZE_X * LINE_X_DISTANCE_FACTOR, moveLayerStartY);

                    // check positions here
                    if (spritesOnLine[i].Position.X > SIZE_X + halfWidth)
                    {
                        float dx = spritesOnLine[i].Position.X  - SIZE_X - halfWidth;
                        spritesOnLineContent[i].Position = spritesOnLine[i].Position = new CCPoint(-halfWidth + dx, moveLayerStartY);
                    }
                    else if (spritesOnLine[i].Position.X < - halfWidth)
                    {
                        float dx = spritesOnLine[i].Position.X + halfWidth;
                        spritesOnLineContent[i].Position = spritesOnLine[i].Position = new CCPoint(SIZE_X + halfWidth + dx, moveLayerStartY);
                    }


                    //do animation here
                    if (lineAnimating[i])
                    {
                        lineAnimationStates[i] += frameTimeInSeconds * ANIMATION_RATE;
                        if (lineAnimationStates[i] > 1)
                        {
                            // end of animation
                            lineAnimating[i] = false;
                            if (gameModel.isOpenedOnLine[i])
                            {
                                spritesOnLineContent[i].Visible = spritesOnLine[i].Visible = false;
                            }
                        }
                        // set current scale
                        float newScale = GetScaleForSpriteAnim(lineAnimationStates[i]) * 0.15f;
                        spritesOnLine[i].ScaleY = spritesOnLine[i].ScaleX = baseScaleTile * (1 + newScale);
                        spritesOnLineContent[i].ScaleY = spritesOnLineContent[i].ScaleX = baseScaleContent * (1 + newScale);
                    }
                } 
            }
        }

        float GetScaleForSpriteAnim(float animation)
        {
            if (animation < 0.5f) return animation * 2;
            if (animation < 1.0f) return 1 - (animation -0.5f) * 2;
            return 0;
        }

        float GetXFromAnimX(float anim)
        {
            if (anim < 0.25f) return anim * 4;
            if (anim < 0.5f) return (0.5f - anim) * 4;
            if (anim < 0.75f) return -(anim - 0.5f) * 4;
            return -(1 - anim) * 4;
        }

        public void Restart(List<string> randomKeys)
        {
            if (restart) return;
            gameModel.Restart(randomKeys);
            restart = true;            
        }

        void DoOnRestart()
        {                 

            // setup wheel            
            for (int i = 0; i < spritesOnWheel.Length; i++)
            {
                wheelAnimating[i] = false;
                spritesOnWheel[i].Visible = true;
                spritesOnWheelContent[i].Visible = true;
                wheelAnimationStates[i] = -1; // no animation
                rotations[i] = i * 45;

                spritesOnWheelContent[i].Rotation = -90 + rotations[i];

                float x = SIZE_X * radiusSizeFactor * (float) Math.Cos (rotations[i] * pi_180);
                float y = SIZE_X * radiusSizeFactor * (float) Math.Sin (rotations[i] * pi_180);

                spritesOnWheelContent[i].Position = spritesOnWheel[i].Position = new CCPoint(wheelCenter.X + x, wheelCenter.Y - y);
                spritesOnWheel[i].ScaleY = spritesOnWheel[i].ScaleX = baseScaleTile;
                spritesOnWheelContent[i].ScaleY = spritesOnWheelContent[i].ScaleX = baseScaleContent;
            }

            // setup line 
            lineAnimation = 0;
            float startOffsetX = (1 - LINE_X_DISTANCE_FACTOR * 4) * 0.5f * SIZE_X;

            for (int i = 0; i < spritesOnLine.Length; i++)
            {
                lineAnimating[i] = false;
                spritesOnLine[i].Visible = true;
                spritesOnLineContent[i].Visible = true;
                lineAnimationStates[i] = -1; // no animation    
                spritesOnLineContent[i].Position = spritesOnLine[i].Position = new CCPoint(startOffsetX + i * SIZE_X * LINE_X_DISTANCE_FACTOR, moveLayerStartY);

                spritesOnLine[i].ScaleY = spritesOnLine[i].ScaleX = baseScaleTile;
                spritesOnLineContent[i].ScaleY = spritesOnLineContent[i].ScaleX = baseScaleContent;
            }

            restart = false;
            
        }

        public void CreateAllSprites(THEME_NAME theme, List<string> randomCards)
        {
            needToCreateSprites = 0;
            this.theme = theme;
            gameModel.randomKeys = randomCards;
        }

        public void CreateAllSprites()
        {
            List<string> randomCards = gameModel.randomKeys;
            // create sprites on the wheel
            for (int i = 0; i < spritesOnWheel.Length; i++)
            {
                spritesOnWheel[i] = new CCSprite("Images/CAROUSEL_GAME/circle_white.png");
                spritesOnWheelContent[i] = new CCSprite(Theme.GetTileImageSourceForPairsGameCocos(theme, randomCards[i]));
                

                gameLayer.AddChild(spritesOnWheel[i]);
                gameLayer.AddChild(spritesOnWheelContent[i]);

                spritesOnWheel[i].ScaleY = spritesOnWheel[i].ScaleX = SIZE_X / spritesOnWheel[i].TextureRectInPixels.MaxX * TILE_FACTOR;                
                spritesOnWheelContent[i].ScaleY = spritesOnWheelContent[i].ScaleX = SIZE_X / spritesOnWheelContent[i].TextureRectInPixels.MaxX * TILE_CONTENT_FACTOR;

                spritesOnWheel[i].AnchorPoint = spritesOnWheelContent[i].AnchorPoint = CCPoint.AnchorMiddle;
            }

            // create sprites on the line
            for (int i = 0; i < spritesOnLine.Length; i++)
            {
                spritesOnLine[i] = new CCSprite("Images/CAROUSEL_GAME/circle_white.png");
                spritesOnLineContent[i] = new CCSprite(Theme.GetTileImageSourceForPairsGameCocos(theme, randomCards[i + 5]));
                
                gameLayer.AddChild(spritesOnLine[i]);
                gameLayer.AddChild(spritesOnLineContent[i]);

                spritesOnLine[i].ScaleY = spritesOnLine[i].ScaleX = SIZE_X / spritesOnLine[i].TextureRectInPixels.MaxX * TILE_FACTOR;
                spritesOnLineContent[i].ScaleY = spritesOnLineContent[i].ScaleX = SIZE_X / spritesOnLineContent[i].TextureRectInPixels.MaxX * TILE_CONTENT_FACTOR;
                spritesOnLine[i].AnchorPoint = spritesOnLineContent[i].AnchorPoint = CCPoint.AnchorMiddle;
               
            }

            baseScaleTile = spritesOnWheel[0].ScaleX;
            baseScaleContent = spritesOnWheelContent[0].ScaleX;
        }
        
        /*
        void OnTouchesEnded(List<CCTouch> touches, CCEvent touchEvent)
        {
            if (touches.Count <=1)
            {
                canTouch = true;
            }
        }
        */
        void OnTouchesBegan(List<CCTouch> touches, CCEvent touchEvent)
        {
            /*
            if (!canTouch) return;
            canTouch = false;
            */

            // check if we can click now
            if (!gameModel.canClick) return;
            
            // line zone
            float yLineMax = moveLayerStartY + SIZE_X * TILE_FACTOR * 0.5f;
            float R2 = SIZE_X * TILE_FACTOR * SIZE_X * TILE_FACTOR * 0.25f;

            if (touches.Count > 0)
            {
                // Perform touch handling here
                int index_line = -1;
                int index_circle = -1;
                foreach (CCTouch touch in touches)
                {
                    Debug.WriteLine($"touch began !, touch.Location.X = {touch.Location.X}, touch.Location.Y = {touch.Location.Y}");                    
                    // check the locations
                    if (touch.Location.Y < yLineMax) // check line
                    {
                        for (int i = 0; i < spritesOnLine.Length; i++)
                        {
                            float x = touch.Location.X - spritesOnLine[i].PositionX;
                            float y = touch.Location.Y - spritesOnLine[i].PositionY;
                            if (x*x + y*y < R2)
                            {
                                index_line = i;
                                break;
                            }
                        }
                        //if (index_line > -1) break;
                    }
                    else // check circle
                    {
                        for (int i = 0; i < spritesOnWheel.Length; i++)
                        {
                            float x = touch.Location.X - spritesOnWheel[i].PositionX;
                            float y = touch.Location.Y - spritesOnWheel[i].PositionY;
                            if (x * x + y * y < R2)
                            {
                                index_circle = i;
                                break;
                            }
                        }
                        //if (index_circle > -1) break;
                    }
                }

                
                if (index_line > -1)
                {
                    if (spritesOnLine[index_line].Visible && !gameModel.isOpenedOnLine[index_line])
                    {
                        lineAnimating[index_line] = true;
                        lineAnimationStates[index_line] = 0;
                        gameModel.TouchOnLine(index_line);                        
                    }                    
                }
                else if(index_circle > -1)
                {
                    if (spritesOnWheel[index_circle].Visible && !gameModel.isOpenedOnWheel[index_circle])
                    {
                        wheelAnimating[index_circle] = true;
                        wheelAnimationStates[index_circle] = 0;
                        gameModel.TouchOnCircle(index_circle);
                    }                    
                }


                // else do nothing
            }
        }
    }
}
