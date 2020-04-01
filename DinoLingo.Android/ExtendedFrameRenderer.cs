using Android.Content;
using Android.Graphics;
using DinoLingo;
using DinoLingo.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ExtendedFrame), typeof(ExtendedFrameRenderer))]
namespace DinoLingo.Droid
{
    public class ExtendedFrameRenderer: FrameRenderer
    {
        private float _cornerRadius;
        private float _strokeWidth;
        private float _strokeWidth_05;
        private RectF _bounds;
        private RectF _bounds_to_clip;
        private Path _path;
        private Path _path_to_clip;
        private Paint _paint;

        public ExtendedFrameRenderer(Context context) : base(context)
        {
            _paint = new Paint {AntiAlias = true, };
            _paint.SetStyle(Paint.Style.Stroke);
        }

        public override void Draw(Canvas canvas)
        {
            if (Element == null) {  
                return;  
            }  

            canvas.Save();
           // if (_path == null)
                SetPath(canvas);

            canvas.ClipPath(_path_to_clip);

            base.Draw(canvas);

            DrawOutline(canvas); 
            canvas.Restore();
            
        }

        void SetPath(Canvas canvas) {
            var element = (ExtendedFrame) Element; 
            _cornerRadius = Context.ToPixels((float)element.CornerRadius);
             _strokeWidth = Context.ToPixels((float)(Element as ExtendedFrame).OutLineWidth);
            _strokeWidth_05 = Context.ToPixels((float)(Element as ExtendedFrame).OutLineWidth * 0.5f);
            

            _bounds = new RectF(_strokeWidth_05, _strokeWidth_05, canvas.Width - _strokeWidth_05, canvas.Height - _strokeWidth_05);
            _bounds_to_clip = new RectF(0, 0, canvas.Width, canvas.Height);

            _path = new Path();
            _path.Reset();
            _path.AddRoundRect(_bounds, _cornerRadius - _strokeWidth_05, _cornerRadius - _strokeWidth_05, Path.Direction.Cw);            
            _path.Close();

            _path_to_clip = new Path();
            _path_to_clip.Reset();            
            _path_to_clip.AddRoundRect(_bounds_to_clip, _cornerRadius, _cornerRadius, Path.Direction.Cw);
            _path_to_clip.Close();

            _paint.StrokeWidth = _strokeWidth;
           
        }

        void DrawOutline(Canvas canvas)
        {          
            _paint.Color = Element.BorderColor.ToAndroid(); //set outline color
            canvas.DrawPath(_path, _paint);
        }

        protected override void OnElementChanged(ElementChangedEventArgs <Frame> e) {  
            base.OnElementChanged(e);  
            if (Element == null) {  
                return;  
            }  
            var element = (ExtendedFrame) Element; 

            _cornerRadius = Context.ToPixels ((float) element.CornerRadius);
             _strokeWidth = Context.ToPixels((float)(Element as ExtendedFrame).OutLineWidth);
             _strokeWidth_05 = Context.ToPixels((float)(Element as ExtendedFrame).OutLineWidth * 0.5f);
            
            _paint.StrokeWidth = _strokeWidth;
        }  

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh) {  
            base.OnSizeChanged(w, h, oldw, oldh); 

           _strokeWidth_05 = Context.ToPixels((float)(Element as ExtendedFrame).OutLineWidth * 0.5f);
            

            if (w != oldw && h != oldh) { 

                _bounds = new RectF(_strokeWidth_05, _strokeWidth_05, w - _strokeWidth_05, h - _strokeWidth_05);
                _bounds_to_clip = new RectF(0, 0, w, h);
            }

            _path = new Path();
            _path.Reset();
            _path.AddRoundRect(_bounds, _cornerRadius - _strokeWidth_05, _cornerRadius - _strokeWidth_05, Path.Direction.Cw);
            _path.Close();

            _path_to_clip = new Path();
            _path_to_clip.Reset();
            _path_to_clip.AddRoundRect(_bounds_to_clip, _cornerRadius, _cornerRadius, Path.Direction.Cw);
            _path_to_clip.Close();
        } 
    }
}
