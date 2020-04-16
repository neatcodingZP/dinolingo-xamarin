using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using Xamarin.Forms;

namespace DinoLingo
{
    public partial class MySlideView : ContentView
    {
        public MainPage_ViewModel rootViewModel;
        public static readonly uint FORCE_CLOSE_TIME = 150;
        public static readonly uint FORCE_OPEN_TIME = 200;

        readonly double PAN_SENSETIVITY = 0.01;
        readonly double PAN_MULTYPLIER = 1.0;
        readonly double PAN_SPEED_COEF = 8.0;
        readonly double OPEN_THRESHOLD = 0.05;

        public enum STATE { IDLE, PANNING, ANIMATING };
        STATE state;
        Object stateLock = new Object();
        public STATE State
        {
            get
            {
                if (stateLock != null) lock (stateLock)
                    {

                        return state;
                    }
                else return STATE.IDLE;
            }
            set
            {
                if (stateLock != null) lock (stateLock)
                {
                    if (value > STATE.IDLE) {
                        Cover.InputTransparent = false;
                    }
                    else Cover.InputTransparent = true;

                    state = value;
                }
            }
        }


        public bool IsOpened { get; set; } = false;
        public MySlideView pairedView;

        RelativeLayout parent;
        double RelativeWidth { get; set; }
        double RelativeXClosed { get; set; }
        double RelativeXOpened { get; set; }

        public Action OnSlideOpened;
        public Action OnSlideClosed;

       
        bool canPan;
        Object panLock = new Object();
        public bool CanPan
        {
            get
            {
                if (panLock != null) lock (panLock)
                {
                    return canPan;
                }
                return false;
            }
            set
            {
                if (panLock != null) lock (panLock)
                {
                    canPan = value;
                }
            }
        }
        bool panStarted = false;

        double absMax = -1;
        //double panThreshold = -1;
        double offset = 5000000;


        public MySlideView()
        {
            InitializeComponent();
            BindingContext = this;
            CanPan = true;
            State = STATE.IDLE;
            Cover.InputTransparent = true;

        }


        public MySlideView (RelativeLayout parent, double RelWidth, double RelXClosed, double RelXOpened, MainPage_ViewModel rootViewModel): this() {
            this.rootViewModel = rootViewModel;
            RelativeWidth = RelWidth;
            RelativeXClosed = RelXClosed;
            RelativeXOpened = RelXOpened;
            this.parent = parent;

            PanGestureRecognizer panGestureRecognizer = new PanGestureRecognizer();
            panGestureRecognizer.PanUpdated += (sender, e) =>
            {
                Debug.WriteLine("CanPan = " + CanPan);
                switch (e.StatusType)
                {
                    
                    case GestureStatus.Started:
                        if (CanPan) {
                            panStarted = true;
                            if (pairedView != null) {
                                pairedView.CanPan = false;
                            }
                        }

                        if (panStarted) {
                            ViewExtensions.CancelAnimations(this);
                            Debug.WriteLine("panStarted...");
                            State = STATE.PANNING;

                                //IsEnabled = false;
                                if (pairedView != null)
                                {
                                    ForceClosePairedView();
                                }
                            CheckPositionAndTranslate(e.TotalX);
                        }

                        break;

                    case GestureStatus.Running:
                        if (State == STATE.PANNING) {
                            CheckPositionAndTranslate(e.TotalX);
                        }
                        break;

                    case GestureStatus.Completed:
                        if (State == STATE.PANNING)
                        {
                            Debug.WriteLine("GestureStatus.Completed");
                            panStarted = false;
                            OnPanFinished();
                        }
                        break;

                    case GestureStatus.Canceled:
                        if (State == STATE.PANNING)
                        {
                            Debug.WriteLine("GestureStatus.Canceled");
                            panStarted = false;
                            OnPanFinished();
                        }
                        break;
                }

                Debug.WriteLine ("e.TotalX= " + e.TotalX.ToString());
            };

            this.GestureRecognizers.Add(panGestureRecognizer);

        }

        public void AddContent(View view) {
            MainView.Content = view;

        }



        public void ForceClosePairedView () {
            //pairedView.IsEnabled = false;
            ViewExtensions.CancelAnimations(pairedView);
            if (pairedView.IsOpened) pairedView.ForceClose(FORCE_CLOSE_TIME);
            else pairedView.ParkBack(FORCE_CLOSE_TIME);
        }

        void OnPanFinished(){
            if (absMax < 0) absMax = Math.Abs(parent.Width * (RelativeXOpened - RelativeXClosed));

            if (Math.Abs(this.TranslationX) > OPEN_THRESHOLD * absMax)
            {
                if (IsOpened) ForceClose(FORCE_CLOSE_TIME);
                else ForceOpen(FORCE_OPEN_TIME);
            }
            else {
                ParkBack(FORCE_CLOSE_TIME);
            }
        }

        void CheckPositionAndTranslate (double x) {
            x *= PAN_MULTYPLIER;
            Debug.WriteLine("CheckPositionAndTranslate --> e.TotalX =  " + x);
            if (offset > 4000000) offset = (RelativeXClosed - RelativeXOpened) * parent.Width;
            //if (Math.Abs(x) < Math.Abs(parent.Width * (RelativeXOpened - RelativeXClosed))) {
                if (IsOpened) {
                    if (RelativeXOpened > RelativeXClosed)
                    {
                        if (x < 0)  {
                        if (x < offset) x = offset;
                            Translate(x);
                        } 
                    }
                    else
                    {
                        if (x > 0) {
                        if (x > offset) x = offset;
                            Translate(x); 
                        }
                    }
                }
                else {
                    if (RelativeXOpened > RelativeXClosed) {
                        if (x > 0) {
                        if (x > -offset) x = -offset;
                            Translate(x);
                        }
                    }
                    else {
                        if (x < 0)  {
                        if (x < -offset) x = -offset;
                            Translate(x);
                        }
                    }
                }
            //}
        }

        void Translate (double x) {
            
            if (absMax < 0) { absMax = Math.Abs(parent.Width * (RelativeXOpened - RelativeXClosed)); }

            double absTranslation = Math.Abs(x - this.TranslationX) / absMax;
            if ( absTranslation > PAN_SENSETIVITY) {
                ViewExtensions.CancelAnimations(this);
                uint time = (uint) ( FORCE_OPEN_TIME * 3 / ((absTranslation * PAN_SPEED_COEF + 1) * (absTranslation * PAN_SPEED_COEF + 1)) + FORCE_OPEN_TIME * 0.5);

                Debug.WriteLine("this.TranslationX = " + this.TranslationX + ", TranslateTo x =  " + x + ", time = " + time);
                this.TranslateTo(x,0, time);

            }
        }

        public void AddToParent() {
           
            parent.Children.Add(this,
            Constraint.RelativeToParent((parent) =>
            {
                return RelativeXClosed * parent.Width;    // установка координаты X
            }),
            Constraint.RelativeToParent((parent) =>
            {
                return 0;   // установка координаты Y
            }),

            Constraint.RelativeToParent((parent) =>
            {
                return RelativeWidth * parent.Width;
            }), // установка ширины
            Constraint.RelativeToParent((parent) =>
            {
                return parent.Height;
            })
            );
        }




        public async void ForceOpen (uint time) {
            State = STATE.ANIMATING;
            //this.IsEnabled = false;


            double targetDistance = Math.Abs (parent.Width * (RelativeXOpened - RelativeXClosed));
            double distance = Math.Abs(parent.Width * (RelativeXOpened - RelativeXClosed)) - Math.Abs(this.TranslationX) + 1;
            uint targetTime = time;

            await this.TranslateTo( parent.Width * (RelativeXOpened - RelativeXClosed), 0, targetTime);

            

            Debug.WriteLine("Force Open, State = " + State + ", CanPan=" + CanPan);

            Device.BeginInvokeOnMainThread(() => {
                var c = BoundsConstraint.FromExpression((Expression<Func<Rectangle>>)(() => new Rectangle(parent.Width * RelativeXOpened, 0, parent.Width * RelativeWidth, parent.Height)), new View[0]);
                RelativeLayout.SetBoundsConstraint(this, c);

                this.TranslationX = 0;

                State = STATE.IDLE;
                IsOpened = true;
                //this.IsEnabled = true;

                SetCanPan();
                OnSlideOpened();

                parent.ForceLayout();
            });
        }
    
        public async void ForceClose(uint time)
        {
            State = STATE.ANIMATING;
                //this.IsEnabled = false;

            double targetDistance = Math.Abs(parent.Width * (RelativeXOpened - RelativeXClosed));
            double distance = Math.Abs(parent.Width * (RelativeXOpened - RelativeXClosed)) - Math.Abs(this.TranslationX) + 1;
            uint targetTime = time;

            await this.TranslateTo(parent.Width * (-RelativeXOpened + RelativeXClosed), 0, targetTime);

            
            Debug.WriteLine("Force Close, State = " + State + ", CanPan=" + CanPan);

            Device.BeginInvokeOnMainThread(() => {
                var c = BoundsConstraint.FromExpression((Expression<Func<Rectangle>>)(() => new Rectangle(parent.Width * RelativeXClosed, 0, parent.Width * RelativeWidth, parent.Height)), new View[0]);
                RelativeLayout.SetBoundsConstraint(this, c);
                this.TranslationX = 0;
                //parent.ForceLayout();

                State = STATE.IDLE;
                IsOpened = false;
                //this.IsEnabled = true;

                SetCanPan();
                OnSlideClosed();
                parent.ForceLayout();
            });
        }

        void SetCanPan () {
            if (pairedView != null)
            {
                if (pairedView.State == STATE.IDLE)
                {
                    CanPan = true;
                    pairedView.CanPan = true;
                }
                else
                {
                    
                }
            }
            else
            {
                CanPan = true;
            }
        }

        public async void ParkBack(uint time) {
            State = STATE.ANIMATING;
            await this.TranslateTo(0, 0, time);
            State = STATE.IDLE;
            //this.IsEnabled = true;
            SetCanPan();            
            Debug.WriteLine("ParkBack, State = " + State + ", CanPan=" + CanPan);

            Device.BeginInvokeOnMainThread(() => {
                parent.ForceLayout();
            });
        }


   
        public void OpenClose (uint time) {
            if (IsOpened)
            {
                ForceClose(time);
            }
            else
            {
                ForceOpen(time);
                if (pairedView != null)
                {
                    ForceClosePairedView();
                }
            }
        } 

        public void Dispose ()
        {
            Content = null;
            BindingContext = null;
            rootViewModel = null;
            stateLock = null;
            pairedView = null;
            parent = null;
            OnSlideOpened = OnSlideClosed = null;
            panLock = null;
        }
       
    }
}
