using System;
using System.Collections.Generic;
using System.Text;
using CocosSharp;

namespace DinoLingo.CocosObjects
{
    class MyImageButton: CCSprite
    {
        public enum State {UP, DOWN};
        public State state = State.UP;
        public int touchId = -1;
        public float baseScale;

        float TIME_DOWN = 0.15f;
        float TIME_UP = 0.1f;
        float scaleFactor = 0.85f;

        public MyImageButton(string fileName) : base (fileName)
        {           
            
        }

        public bool BeginTouch (int Id)
        {
            if (state == State.UP)
            {
                StopAllActions();
                //ScaleX = ScaleY = baseScale;
                touchId = Id;
                state = State.DOWN;

                CCFiniteTimeAction actionDown = new CCScaleTo(TIME_DOWN * (ScaleX - baseScale * scaleFactor) / (baseScale - baseScale * scaleFactor), baseScale * scaleFactor);

                RunAction(actionDown);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool EndTouch (int Id)
        {
            if (state == State.DOWN && Id == touchId)
            {
                StopAllActions();
                touchId = -1;
                state = State.UP;
                CCFiniteTimeAction finishActionDown = new CCScaleTo(TIME_DOWN * (ScaleX - baseScale * 0.8f) / (baseScale - baseScale * 0.8f) , baseScale * 0.8f);
                CCFiniteTimeAction actionUp = new CCScaleTo(TIME_UP, baseScale);
                CCSequence mySequence = new CCSequence(finishActionDown, actionUp);

                RunAction(mySequence);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Cancel(int Id)
        {
            if (state == State.DOWN && Id == touchId)
            {
                StopAllActions();
                touchId = -1;
                state = State.UP;

                ScaleX = ScaleY = baseScale;
                /*
                CCFiniteTimeAction finishActionDown = new CCScaleTo(TIME_DOWN * (ScaleX - baseScale * 0.8f) / (baseScale - baseScale * 0.8f), baseScale * 0.8f);
                CCFiniteTimeAction actionUp = new CCScaleTo(TIME_UP, baseScale);
                CCSequence mySequence = new CCSequence(finishActionDown, actionUp);

                RunAction(mySequence);   
                */
            }
            else
            {
               
            }
        }
    }
}
