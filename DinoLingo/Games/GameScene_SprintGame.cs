using CocosSharp;
using DinoLingo.CocosObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace DinoLingo.Games
{
    class GameScene_SprintGame : CCScene
    {
        SprintGameModel gameModel;

        CCLayer gameLayer;
        CocosSharpView gameView_;


        MyImageButton closeBtn, correctBtn, wrongBtn;
        CCDrawNode timeSector = new CCDrawNode();

        CCColor4B TIMER_COLOR = new CCColor4B(84, 218, 38, 255);
        CCColor3B TOTAL_CORRECT_COLOR = new CCColor3B(179, 255, 145);
        CCColor3B TOTAL_WRONG_COLOR = new CCColor3B(255, 168, 144);

        CCLabel scoreLabel, wordsCountLeftLabel, wrongLabel, correctLabel;

        CCSprite gameSprite_p, gameSpriteAfterEffect_p, gameSpriteWrong_p;
        CCSprite gameSprite, gameSpriteAfterEffect, gameSpriteWrong;
        CCSprite wordsCountLeft;
        //CCSprite timer;

        CCPoint START_SPRITE_POSITION = new CCPoint (UI_Sizes.ScreenWidth * 0.5f, UI_Sizes.ScreenHeight * 0.75f);
        CCPoint TOP_SPRITE_POSITION = new CCPoint(UI_Sizes.ScreenWidth * 0.5f, UI_Sizes.ScreenHeight * 1.0f);
        CCPoint[] ANIMATE_WRONG_POSITIONS;
        CCPoint ANIMATE_CORRECT_POSITION;

        float SPRITE_SIZE = 0.75f;

        public Action OnEndGame;
        public Action OnMadeClick;

        public bool Restart = false;

        public GameScene_SprintGame(CCGameView gameView, SprintGameModel gameModel, CocosSharpView gameView_) : base(gameView)
        {
            gameLayer = new CCLayer();
            this.gameModel = gameModel;
            this.gameView_ = gameView_;

            // create background
            Color = new CCColor3B (75,135,193);
            CCTexture2D texture = new CCTexture2D("Images/UI/pattern");            
            for (int i = 0; i <= UI_Sizes.ScreenWidth  / texture.PixelsWide; i++)
                for (int j = 0; j <= UI_Sizes.ScreenHeight / texture.PixelsHigh; j++)
                {
                    CCSprite pattern = new CCSprite(texture, new CCRect(0, 0, texture.PixelsWide, texture.PixelsHigh));
                    pattern.AnchorPoint = CCPoint.AnchorLowerLeft;
                    pattern.Scale = 1;
                    gameLayer.AddChild(pattern);
                    pattern.Position = new CCPoint(texture.PixelsWide * i, texture.PixelsHigh * j); 
                }
            float xOffset = (0.5f * UI_Sizes.ScreenWidth - 0.325f * UI_Sizes.ScreenHeight) * 0.5f;

            ANIMATE_WRONG_POSITIONS = new CCPoint[] {
            new CCPoint ((UI_Sizes.ScreenWidth * 0.5f - xOffset) * 0.75f + xOffset, UI_Sizes.ScreenHeight * 0.9f),
            new CCPoint ((UI_Sizes.ScreenWidth * 0.5f - xOffset) * 0.5f + xOffset, UI_Sizes.ScreenHeight * 0.5f),
            new CCPoint ((UI_Sizes.ScreenWidth * 0.5f - xOffset) * 0.25f + xOffset, UI_Sizes.ScreenHeight * 0.6f),
            new CCPoint (xOffset, UI_Sizes.ScreenHeight * 0.3f),
            };

            ANIMATE_CORRECT_POSITION = new CCPoint(UI_Sizes.ScreenWidth - xOffset, UI_Sizes.ScreenHeight * 0.3f);

            CCSprite wordsCountLeft = new CCSprite("Images/SPRINT_GAME/question2");
            wordsCountLeft.ScaleX = wordsCountLeft.ScaleY = UI_Sizes.ScreenHeight / wordsCountLeft.TextureRectInPixels.MaxX * 0.23f;
            wordsCountLeft.AnchorPoint = CCPoint.AnchorMiddle; // new CCPoint (0.466f, 0.464f);
            wordsCountLeft.Position = new CCPoint(xOffset, UI_Sizes.ScreenHeight * 0.74f);
            gameLayer.AddChild(wordsCountLeft);

            /*
            timeSector.DrawSolidArc (
                pos: timer.Position,
                radius: UI_Sizes.ScreenHeight * 0.23f * 0.28f,
                startAngle: CCMathHelper.Pi / 2 * 3,
                sweepAngle: CCMathHelper.Pi * 2, 
                color: TIMER_COLOR);
            gameLayer.AddChild(timeSector);
            */

            wordsCountLeftLabel = new CCLabel(gameModel.TOTAL_WORDS_IN_SPRINT.ToString(), "Arial", UI_Sizes.ScreenHeight * 0.074f, CCLabelFormat.SystemFont)
            {
                HorizontalAlignment = CCTextAlignment.Center,
                AnchorPoint = CCPoint.AnchorMiddle,
                PositionX = xOffset,
                PositionY = UI_Sizes.ScreenHeight * 0.58f,
                Color = CCColor3B.White,
            };
            gameLayer.AddChild(wordsCountLeftLabel);


            closeBtn = new MyImageButton("Images/UI/btn_close");
            closeBtn.ScaleX = closeBtn.ScaleY = closeBtn.baseScale = (float)(UI_Sizes.ScreenHeight) / closeBtn.TextureRectInPixels.MaxY * 0.1f;             
            gameLayer.AddChild(closeBtn);            
            closeBtn.AnchorPoint = CCPoint.AnchorUpperRight;
            closeBtn.Position = new CCPoint((float)(UI_Sizes.ScreenWidth), (float)(UI_Sizes.ScreenHeight));

            correctBtn = new MyImageButton("Images/UI/button_yes_green");
            correctBtn.ScaleX = correctBtn.ScaleY = correctBtn.baseScale = UI_Sizes.ScreenHeight / correctBtn.TextureRectInPixels.MaxX * 0.21f;
            gameLayer.AddChild(correctBtn);
            correctBtn.AnchorPoint = CCPoint.AnchorMiddle;
            correctBtn.Position = new CCPoint(UI_Sizes.ScreenWidth * 0.5f - 0.2f * UI_Sizes.ScreenHeight, UI_Sizes.ScreenHeight * 0.37f);

            /*
            CCLabel correctLabel = new CCLabel("Correct", "Arial", correctBtn.TextureRectInPixels.MaxY * 0.5f, CCLabelFormat.SystemFont);
            correctLabel.HorizontalAlignment = CCTextAlignment.Center;
            correctLabel.AnchorPoint = CCPoint.AnchorMiddle;
            correctLabel.PositionX = correctBtn.TextureRectInPixels.MaxX * 0.5f; 
            correctLabel.PositionY = correctBtn.TextureRectInPixels.MaxY * 0.5f;
            correctBtn.AddChild(correctLabel);
*/

            wrongBtn = new MyImageButton("Images/UI/button_no_red");
            wrongBtn.ScaleX = wrongBtn.ScaleY = wrongBtn.baseScale = UI_Sizes.ScreenHeight / wrongBtn.TextureRectInPixels.MaxX * 0.21f;
            gameLayer.AddChild(wrongBtn);
            wrongBtn.AnchorPoint = CCPoint.AnchorMiddle;
            wrongBtn.Position = new CCPoint(UI_Sizes.ScreenWidth * 0.5f + 0.2f * UI_Sizes.ScreenHeight, UI_Sizes.ScreenHeight * 0.37f);

            /*
            CCLabel wrongLabel = new CCLabel("Wrong", "Arial", wrongBtn.TextureRectInPixels.MaxY * 0.5f, CCLabelFormat.SystemFont);
            wrongLabel.HorizontalAlignment = CCTextAlignment.Center;
            wrongLabel.AnchorPoint = CCPoint.AnchorMiddle;
            wrongLabel.PositionX = wrongBtn.TextureRectInPixels.MaxX * 0.5f;
            wrongLabel.PositionY = wrongBtn.TextureRectInPixels.MaxY * 0.5f;
            wrongBtn.AddChild(wrongLabel);
            */

            CCSprite star = new CCSprite("Images/SPRINT_GAME/star_score");
            star.ScaleX = star.ScaleY = UI_Sizes.ScreenHeight / star.TextureRectInPixels.MaxX * 0.25f;
            star.AnchorPoint = CCPoint.AnchorMiddle;
            star.Position = new CCPoint(UI_Sizes.ScreenWidth - xOffset, UI_Sizes.ScreenHeight * 0.74f);
            gameLayer.AddChild(star);

            scoreLabel = new CCLabel("0", "Arial", UI_Sizes.ScreenHeight * 0.074f, CCLabelFormat.SystemFont)
            {
                HorizontalAlignment = CCTextAlignment.Center,
                AnchorPoint = CCPoint.AnchorMiddle,
                PositionX = UI_Sizes.ScreenWidth - xOffset,
                PositionY = UI_Sizes.ScreenHeight * 0.58f,
                Color = CCColor3B.White,
            };            
            gameLayer.AddChild(scoreLabel);

            CCSprite bucketCorrect = new CCSprite("Images/SPRINT_GAME/bucket_yes");
            bucketCorrect.ScaleX = bucketCorrect.ScaleY = UI_Sizes.ScreenHeight / bucketCorrect.TextureRectInPixels.MaxX * 0.27f;
            bucketCorrect.AnchorPoint = CCPoint.AnchorMiddle;
            bucketCorrect.Position = new CCPoint(UI_Sizes.ScreenWidth - xOffset, UI_Sizes.ScreenHeight * 0.15f);
            gameLayer.AddChild(bucketCorrect);

            correctLabel = new CCLabel("0", "Arial", UI_Sizes.ScreenHeight * 0.074f, CCLabelFormat.SystemFont)
            {
                HorizontalAlignment = CCTextAlignment.Center,
                AnchorPoint = CCPoint.AnchorMiddle,
                PositionX = UI_Sizes.ScreenWidth - xOffset,
                PositionY = UI_Sizes.ScreenHeight * 0.29f,
                Rotation = +30,
                Color = TOTAL_CORRECT_COLOR,
            };
            gameLayer.AddChild(correctLabel);

            CCSprite bucketWrong = new CCSprite("Images/SPRINT_GAME/bucket_no");
            bucketWrong.ScaleX = bucketWrong.ScaleY = UI_Sizes.ScreenHeight / wordsCountLeft.TextureRectInPixels.MaxX * 0.27f;
            bucketWrong.AnchorPoint = CCPoint.AnchorMiddle;
            bucketWrong.Position = new CCPoint(xOffset, UI_Sizes.ScreenHeight * 0.15f);
            gameLayer.AddChild(bucketWrong);

            wrongLabel = new CCLabel("0", "Arial", UI_Sizes.ScreenHeight * 0.074f, CCLabelFormat.SystemFont)
            {
                HorizontalAlignment = CCTextAlignment.Center,
                AnchorPoint = CCPoint.AnchorMiddle,
                PositionX = xOffset,
                PositionY = UI_Sizes.ScreenHeight * 0.29f,
                Rotation = -30,
                Color = TOTAL_WRONG_COLOR,
            };
            gameLayer.AddChild(wrongLabel);            

            gameSpriteWrong_p = new CCSprite("Images/SPRINT_GAME/card")
            {
                AnchorPoint = CCPoint.AnchorMiddle,
                Position = START_SPRITE_POSITION,               
                Visible = false,
            };
            gameSpriteWrong_p.ScaleX = gameSpriteWrong_p.ScaleY = UI_Sizes.ScreenHeight / gameSpriteWrong_p.TextureRectInPixels.MaxX * 0.4f;
            gameLayer.AddChild(gameSpriteWrong_p);

            gameSpriteWrong = new CCSprite()
            {
                AnchorPoint = CCPoint.AnchorMiddle,
                Position = new CCPoint (gameSpriteWrong_p.TextureRectInPixels.MaxX * 0.5f, gameSpriteWrong_p.TextureRectInPixels.MaxY * 0.5f),                
            };
            gameSpriteWrong_p.AddChild(gameSpriteWrong);


            gameSpriteAfterEffect_p = new CCSprite("Images/SPRINT_GAME/card")
            {
                AnchorPoint = CCPoint.AnchorMiddle,
                Position = START_SPRITE_POSITION,
                Visible = false,
            };
            gameSpriteAfterEffect_p.ScaleX = gameSpriteAfterEffect_p.ScaleY = UI_Sizes.ScreenHeight / gameSpriteAfterEffect_p.TextureRectInPixels.MaxX * 0.4f;
            gameLayer.AddChild(gameSpriteAfterEffect_p);

            gameSpriteAfterEffect = new CCSprite()
            {
                AnchorPoint = CCPoint.AnchorMiddle,
                Position = new CCPoint(gameSpriteAfterEffect_p.TextureRectInPixels.MaxX * 0.5f,
                gameSpriteAfterEffect_p.TextureRectInPixels.MaxY * 0.5f),
            };
            gameSpriteAfterEffect_p.AddChild(gameSpriteAfterEffect);


            gameSprite_p = new CCSprite("Images/SPRINT_GAME/card")
            {
                AnchorPoint = CCPoint.AnchorMiddle,
                Position = TOP_SPRITE_POSITION,
                Visible = false,
            };
            gameSprite_p.ScaleX = gameSprite_p.ScaleY = UI_Sizes.ScreenHeight / gameSprite_p.TextureRectInPixels.MaxX * 0.4f;
            gameLayer.AddChild(gameSprite_p);

            gameSprite = new CCSprite()
            {
                AnchorPoint = CCPoint.AnchorMiddle,
                Position = new CCPoint(gameSprite_p.TextureRectInPixels.MaxX *0.5f,
                gameSprite_p.TextureRectInPixels.MaxY * 0.5f),
            };
            gameSprite_p.AddChild(gameSprite);



            var touchListener = new CCEventListenerTouchAllAtOnce();
            touchListener.OnTouchesBegan = OnTouchesBegan;
            touchListener.OnTouchesEnded = OnTouchesEnded;
            touchListener.OnTouchesCancelled = OnTouchesCancelled;
            touchListener.OnTouchesMoved = OnTouchesMoved;

            AddEventListener(touchListener, gameLayer);

            AddLayer(gameLayer);
            
            //create some start delay
            CCDelayTime startDelay = new CCDelayTime(1.0f);
            var startAction = new CCCallFunc(DoAfterStartDelay);
            CCSequence startSequence = new CCSequence(startDelay, startAction);
            RunAction(startSequence);
        }           
        
        void DoOnRestart ()
        {
            gameModel.Restart();
            wordsCountLeftLabel.Text = gameModel.TOTAL_WORDS_IN_SPRINT.ToString();
            scoreLabel.Text = "0";
            correctLabel.Text = "0";
            wrongLabel.Text = "0";

            //create some start delay
            CCDelayTime startDelay = new CCDelayTime(1.0f);
            var startAction = new CCCallFunc(DoAfterStartDelay);
            CCSequence startSequence = new CCSequence(startDelay, startAction);
            RunAction(startSequence);
        }

        void UpdateTimer (float frameTimeInSeconds)
        {
            //Debug.WriteLine("GameScene_SprintGame -> UpdateTimer");
            if (Restart)
            {
                Restart = false;
                Unschedule(UpdateTimer);
                DoOnRestart();
                return;
            }

            if (gameModel.state == SprintGameModel.State.TIME_FINISHED) return;

            gameModel.totalTime += frameTimeInSeconds;

            if (false) //(gameModel.totalTime > gameModel.FINISH_TIME)
            {
                /*
                gameModel.totalTime = gameModel.FINISH_TIME;
                gameModel.state = SprintGameModel.State.TIME_FINISHED;
                //CCAudioEngine.SharedEngine.StopAllLoopingEffects();
                Unschedule (UpdateTimer);
                OnEndGame?.Invoke();
                */
            }
            else
            {
                
                if (gameModel.NeedToRepeatWord())
                {
                    Debug.WriteLine("GameScene_SprintGame -> UpdateTimer --> NeedToRepeatWord");
                    SayWord(gameModel.saidKey);
                }
                //ScheduleTimer();
            }

            // update
            /*
            timeSector.Clear();
            timeSector.DrawSolidArc(
                pos: timer.Position,
                radius: UI_Sizes.ScreenHeight * 0.23f * 0.28f,
                startAngle: - CCMathHelper.Pi / 2  + CCMathHelper.Pi * 2 * (gameModel.totalTime / gameModel.FINISH_TIME),
                sweepAngle: CCMathHelper.Pi * 2 - CCMathHelper.Pi * 2 * (gameModel.totalTime / gameModel.FINISH_TIME),
                color: TIMER_COLOR);

            //
            timerLabel.Text = ((int)gameModel.totalTime).ToString();
            */
        }
        
        void DoAfterStartDelay ()
        {
            Debug.WriteLine("GameScene_SprintGame -> DoAfterStartDelay");
            //CCAudioEngine.SharedEngine.PlayEffect("Sounds/tick_tack", true);


            ScheduleTimer();
            ShowNextCardSequence();            
        }

        void ScheduleTimer()
        {
            Debug.WriteLine("GameScene_SprintGame -> ScheduleTimer");
            CCDelayTime delay = new CCDelayTime(1.0f);
            var action = new CCCallFunc(() => {
                Schedule(UpdateTimer);
            }
            );
            CCSequence sequence = new CCSequence(delay, action);
            RunAction(sequence);
        }

        void ShowNextCardSequence()
        {
            float animTime = 1.5f;
            Debug.WriteLine("GameScene_SprintGame -> ShowNextCardSequence");

            gameSprite_p.StopAllActions();
            //gameSprite.StopAllActions();

            CreateNextCard();
            /*
            CCFiniteTimeAction animateNextCard1 = new CCScaleBy(animTime * 0.5f, 1.2f);
            CCFiniteTimeAction animateNextCard2 = new CCScaleBy(animTime * 0.5f, 1 / 1.2f);
            */



            CCFiniteTimeAction animateNextCard = new CCMoveTo(animTime, START_SPRITE_POSITION);
            CCAction easing = new CCEaseElasticOut(animateNextCard); 
            
            gameSprite_p.RunAction(easing);
        }

        void CreateNextCard()
        {
            Debug.WriteLine("GameScene_SprintGame -> CreateNextCard");

            gameSprite.Texture = new CCTexture2D(Theme.GetTileImageSourceForPairsGameCocos(gameModel.themeName, gameModel.GetNextKey()));
            gameSprite.ScaleY = gameSprite.ScaleX = gameSprite_p.TextureRectInPixels.MaxX / gameSprite.TextureRectInPixels.MaxX * SPRITE_SIZE;

            gameSprite_p.Visible = true;
            gameSprite_p.Position = TOP_SPRITE_POSITION;

            SayWord(gameModel.GetKeyToSay());
            if (gameModel.state == SprintGameModel.State.WAITING_FOR_NEXT) gameModel.state = SprintGameModel.State.PLAYING;
        }

        void SayWord(string key)
        {
            Debug.WriteLine("GameScene_SprintGame -> SayWord -> key = " + key);
            gameModel.lastWordTime = gameModel.totalTime;
            Device.BeginInvokeOnMainThread(() => {
                try
                {
                    App.Audio.SayWord(key, UserHelper.Language);
                }
                catch (Exception ex)
                {

                }
            });            
        }
        

        void OnTouchesBegan(List<CCTouch> touches, CCEvent touchEvent)
        {
            Debug.WriteLine("GameScene_SprintGame -> OnTouchesBegan");
            if (gameModel.state == SprintGameModel.State.CLOSED) return;

            if (touches.Count > 0)
            {
                var touch = touches[0];

                if (closeBtn.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                {
                    HandleBtnBegin(closeBtn, touches[0].Id);
                }
                else if(correctBtn.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                {
                    HandleBtnBegin(correctBtn, touches[0].Id);
                }
                else if (wrongBtn.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                {
                    HandleBtnBegin(wrongBtn, touches[0].Id);
                }                
            }
        }

        void OnTouchesEnded(List<CCTouch> touches, CCEvent touchEvent)
        {
            Debug.WriteLine("GameScene_SprintGame -> OnTouchesEnded");
            if (gameModel.state == SprintGameModel.State.CLOSED) return;

            if (touches.Count > 0)
            {
                var touch = touches[0];

                if (closeBtn.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                {
                    HandleBtnEnd(closeBtn, touches[0].Id);
                }
                else if (correctBtn.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                {
                    HandleBtnEnd(correctBtn, touches[0].Id);
                }
                else if (wrongBtn.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                {
                    HandleBtnEnd(wrongBtn, touches[0].Id);
                }
            }
        }

        void OnTouchesCancelled(List<CCTouch> touches, CCEvent touchEvent)
        {
            Debug.WriteLine("GameScene_SprintGame -> OnTouchesCanceled");
            if (gameModel.state == SprintGameModel.State.CLOSED) return;

            if (touches.Count > 0)
            {
                var touch = touches[0];
                HandleBtnCancel(closeBtn, touches[0].Id);
                HandleBtnCancel(correctBtn, touches[0].Id);
                HandleBtnCancel(wrongBtn, touches[0].Id);
            }
        }

        void OnTouchesMoved(List<CCTouch> touches, CCEvent touchEvent)
        {
            Debug.WriteLine("GameScene_SprintGame -> OnTouchesMoved");
            if (gameModel.state == SprintGameModel.State.CLOSED) return;

            if (touches.Count > 0)
            {
                var touch = touches[0];

                if (!closeBtn.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                {
                    HandleBtnCancel(closeBtn, touches[0].Id);
                } 
                
                if (!correctBtn.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                {
                    HandleBtnCancel(correctBtn, touches[0].Id);
                }

                if (!wrongBtn.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                {
                    HandleBtnCancel(wrongBtn, touches[0].Id);
                }
            }
        }

        void HandleBtnBegin(MyImageButton button, int touchId)
        {
            bool beginTouch = button.BeginTouch(touchId);
            Debug.WriteLine("GameScene_SprintGame -> HandleBtnBegin -> beginTouch =" + beginTouch);
        }

        void HandleBtnEnd(MyImageButton button, int touchId)
        {            
            bool endTouch = button.EndTouch(touchId);
            if (endTouch)
            {
                if (button == closeBtn)
                {
                    gameModel.state = SprintGameModel.State.CLOSED;
                    CloseGame();
                    //CLOSE GAME
                }
                else if (gameModel.state == SprintGameModel.State.PLAYING)
                {
                    if (button == correctBtn)
                    {
                        DoAfterAnswer(gameModel.targetKey == gameModel.saidKey);
                    }
                    else if (button == wrongBtn)
                    {                        
                        DoAfterAnswer(gameModel.targetKey != gameModel.saidKey);                        
                    }
                }
                
            }
            Debug.WriteLine("GameScene_SprintGame -> HandleBtnEnd -> endTouch =" + endTouch);
        }

        void DoAfterAnswer(bool correct)
        {
            Debug.WriteLine("GameScene_SprintGame -> DoAfterAnswer -> correct =" + correct);
            if (gameModel.state == SprintGameModel.State.PLAYING) gameModel.state = SprintGameModel.State.WAITING_FOR_NEXT;
            gameModel.UpdateScore(correct);
            OnMadeClick?.Invoke();

            correctLabel.Text = gameModel.correct.ToString();
            wrongLabel.Text = gameModel.wrong.ToString();

            wordsCountLeftLabel.Text = (gameModel.TOTAL_WORDS_IN_SPRINT - gameModel.correct - gameModel.wrong).ToString();

            bool isEndOfGame = gameModel.correct + gameModel.wrong >= gameModel.TOTAL_WORDS_IN_SPRINT;
            if (correct)
            {
                CCAudioEngine.SharedEngine.PlayEffect("Sounds/CORRECT");

                scoreLabel.Text = gameModel.score.ToString();

                ResetWrongAfterEffect();

                // RessetAfterEffect
                ResetAfterEffect();

                // turnOffSprite
                gameSprite_p.Visible = false;

                // setNextSprite + AnimateAftereffect
                if (!isEndOfGame) ShowNextCardSequence();

                //aftereffect
                DoAfterEffect();                
                 
            }
            else
            {
                CCAudioEngine.SharedEngine.PlayEffect("Sounds/WRONG");

                // RessetAfterEffect
                ResetWrongAfterEffect();
                ResetAfterEffect();

                // turnOffSprite
                gameSprite_p.Visible = false;

                //aftereffect
                DoWrongAfterEffect();

                // setNextSprite + AnimateAftereffect
                if (!isEndOfGame)
                {
                    CCDelayTime showNextCardAfterDelay = new CCDelayTime(0.5f);
                    CCCallFunc doAfterDelay = new CCCallFunc(ShowNextCardSequence);
                    CCSequence showWrongShowNextCardSequence = new CCSequence(showNextCardAfterDelay, doAfterDelay);
                    RunAction(showWrongShowNextCardSequence);
                }                       
            }

            if (isEndOfGame) // end of game
            {
                gameModel.state = SprintGameModel.State.TIME_FINISHED;                
                // Unschedule(UpdateTimer);
                OnEndGame?.Invoke();
            }
        }

        void DoAfterEffect()
        {
            float animTime = 1.25f;

            gameSpriteAfterEffect_p.Visible = true;

            CCFiniteTimeAction firstAction = new CCMoveTo(animTime, ANIMATE_CORRECT_POSITION);
            CCFiniteTimeAction scaleAction = new CCScaleTo(animTime, gameSpriteAfterEffect_p.ScaleX * 0.1f);
            CCFiniteTimeAction rotateAction = new CCRotateBy(animTime, 360 * 3);


            CCSpawn combo = new CCSpawn(firstAction, scaleAction, rotateAction);
            var onFinish = new CCCallFunc (DoAfterEffectOnFinish);

            CCSequence sequence = new CCSequence(combo, onFinish);

            gameSpriteAfterEffect_p.RunAction(sequence);
        }

        void DoWrongAfterEffect()
        {
            float animTime = 1.0f;

            gameSpriteWrong_p.Visible = true;

            CCFiniteTimeAction move0 = new CCMoveTo(animTime * 0.25f, ANIMATE_WRONG_POSITIONS[0]);
            CCFiniteTimeAction move1 = new CCMoveTo(animTime * 0.25f, ANIMATE_WRONG_POSITIONS[1]);
            CCFiniteTimeAction move2 = new CCMoveTo(animTime * 0.25f, ANIMATE_WRONG_POSITIONS[2]);
            CCFiniteTimeAction move3 = new CCMoveTo(animTime * 0.25f, ANIMATE_WRONG_POSITIONS[3]);

            CCFiniteTimeAction scaleAction = new CCScaleTo(animTime, gameSpriteWrong_p.ScaleX * 0.1f);
            CCFiniteTimeAction rotateAction = new CCRotateBy(animTime, 90);

            CCSequence moveSequence = new CCSequence(move0, move1, move2, move3);

            CCSpawn combo = new CCSpawn(moveSequence, scaleAction, rotateAction);

            var onFinish = new CCCallFunc(DoWrongAfterEffectOnFinish);

            CCSequence sequence = new CCSequence(combo, onFinish);

            gameSpriteWrong_p.RunAction(sequence);
        }

        void DoAfterEffectOnFinish ()
        {
            gameSpriteAfterEffect_p.Visible = false;
        }

        void DoWrongAfterEffectOnFinish()
        {
            gameSpriteWrong_p.Visible = false;
        }

        void ResetAfterEffect()
        {
            gameSpriteAfterEffect_p.StopAllActions();            
            gameSpriteAfterEffect_p.Position = START_SPRITE_POSITION;

            gameSpriteAfterEffect.Texture = gameSprite.Texture;
            gameSpriteAfterEffect.ScaleY = gameSpriteAfterEffect.ScaleX = gameSprite.ScaleX;
            gameSpriteAfterEffect_p.ScaleY = gameSpriteAfterEffect_p.ScaleX = UI_Sizes.ScreenHeight / gameSpriteAfterEffect_p.TextureRectInPixels.MaxX * 0.4f;
            gameSpriteAfterEffect_p.Rotation = 0;
            gameSpriteAfterEffect_p.Visible = false;
        }

        void ResetWrongAfterEffect()
        {
            gameSpriteWrong_p.StopAllActions();
            gameSpriteWrong_p.Position = START_SPRITE_POSITION;

            gameSpriteWrong.Texture = gameSprite.Texture;
            gameSpriteWrong_p.ScaleY = gameSpriteWrong_p.ScaleX = UI_Sizes.ScreenHeight / gameSpriteWrong_p.TextureRectInPixels.MaxX * 0.4f;

            gameSpriteWrong.ScaleY = gameSpriteWrong.ScaleX = gameSprite.ScaleX;

            gameSpriteWrong_p.Rotation = 0;
            gameSpriteWrong_p.Visible = false;
        }

        void DoOnCorrectAnswer()
        {
            
            CCAudioEngine.SharedEngine.PlayEffect("Sounds/CORRECT");
        }

        void DoOnWrongAnswer()
        {
            if (gameModel.random.Next(0, 2) > 0)
            {
                CCAudioEngine.SharedEngine.PlayEffect("Sounds/WRONG");
            }
            else
            {
                CCAudioEngine.SharedEngine.PlayEffect("Sounds/Cartoon_Boing");
            }
                
        }

        void HandleBtnCancel(MyImageButton button, int touchId)
        {
            button.Cancel(touchId);
            Debug.WriteLine("GameScene_SprintGame -> HandleBtnCancel");
        }


        void HandleCorrectLabelTouched()
        {
            Debug.WriteLine("GameScene_SprintGame -> HandleCorrectLabelTouched");
        }

        void HandleWrongLabelTouched()
        {
            Debug.WriteLine("GameScene_SprintGame -> HandleWrongLabelTouched");
        }

        void CloseGame()
        {
            Device.BeginInvokeOnMainThread(async() => {
                gameView_.IsVisible = false;
                await App.Current.MainPage.Navigation.PopModalAsync();
            });
        }

    }
}
