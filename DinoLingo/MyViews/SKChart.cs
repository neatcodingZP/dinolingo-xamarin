using SkiaSharp.Views.Forms;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace DinoLingo.MyViews
{
    public class SKChart: SKCanvasView
    {
        public float bigSize = 0.8f; // principle donut size
        public float relHoleSize = 0.5f;
        public float textSize = 0.1f;

        private float _offsetAngle = 2;
        private SKColor _frontColor = SKColors.Green;
        private SKColor _backColor = SKColors.Indigo;        

        public float value = 0.1f;                

        public void SetColors (string frontColor, string backColor, string accentColor = "#FFFFFF")
        {
            SKColor.TryParse(frontColor, out _frontColor);
            SKColor.TryParse(backColor, out _backColor);            
        }


        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            if (value > 1) value = 1;

            // draw a donut
            DrawADonut(e);

            // draw Text
            DrawText(e);                        
        }

        void DrawADonut(SKPaintSurfaceEventArgs e)
        {
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            SKPoint center = new SKPoint(info.Width / 2, info.Height / 2);
            float radius = Math.Min(info.Width, info.Height) / 2 * bigSize;
            float radiusMid = radius * relHoleSize * 0.5f + radius * 0.5f;
            float strokeWidth = (radius - radiusMid) * 2;

            SKRect arcRect = new SKRect(center.X - radiusMid, center.Y - radiusMid, center.X + radiusMid, center.Y + radiusMid);
            // draw 2 donuts

            float startAngleFront = 270;
            float sweepAngleFront = value * 360;

            float startAngleBack = startAngleFront + sweepAngleFront + _offsetAngle;
            if (startAngleBack >= 360) startAngleBack -= 360;
            float sweepAngleBack = 360 - sweepAngleFront - 2 * _offsetAngle;

            // background            
            SKPath pathB = new SKPath();

            if (sweepAngleBack < 180)
                pathB.ArcTo(arcRect, startAngleBack, sweepAngleBack, false);
            else
            {
                pathB.ArcTo(arcRect, startAngleBack, 180, false);
                pathB.ArcTo(arcRect, startAngleBack + 180, sweepAngleBack - 180, false);
            }

            SKPaint paint = new SKPaint()
            {
                Style = SKPaintStyle.Stroke,
                Color = _backColor,
                StrokeWidth = strokeWidth,
                IsAntialias = true,
            };

            canvas.DrawPath(pathB, paint);

            if (value == 0) return; // 
                                  

            // draw content
            #region content
            SKPath pathContent = new SKPath(); 
            
            if (sweepAngleFront < 180)
            pathContent.ArcTo(arcRect, startAngleFront, sweepAngleFront, false);          
            else
            {
                pathContent.ArcTo(arcRect, startAngleFront, 180, false);
                pathContent.ArcTo(arcRect, startAngleFront + 180, sweepAngleFront - 180, false);
            }

            SKPaint paintContent = new SKPaint()
            {
                Style = SKPaintStyle.Stroke,
                Color = _frontColor,
                StrokeWidth = strokeWidth + 1,
                IsAntialias = true,
            };

            canvas.DrawPath(pathContent, paintContent);
            #endregion

        }

        void DrawText(SKPaintSurfaceEventArgs e)
        {
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.GetLocalClipBounds(out var bounds);

            // get principle size
            float pSize = 0;
            if (bounds.Height > bounds.Width)
            {
                pSize = bounds.Width;
            }
            else pSize = bounds.Height;

            string str = (value * 100).ToString("0.0") + "%";
            SKPaint textPaint = new SKPaint()
            {
                Color = _frontColor,
                TextSize = textSize * pSize,
                TextAlign = SKTextAlign.Center, 
                Style = SKPaintStyle.Fill,
                StrokeWidth = 1,
            };

            canvas.DrawText(str, bounds.MidX, bounds.MidY + textSize * pSize * 0.5f, textPaint);
        }
    }
}
