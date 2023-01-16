namespace Jem.WpfCommonLibrary22
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;

    public class ZoomBorder : Border
    {
        private UIElement? child = null;
        private Point origin;
        private Point start;

        #region ZoomBorder

        static ZoomBorder()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ZoomBorder), new FrameworkPropertyMetadata(typeof(ZoomBorder)));
        }

        public ZoomBorder()
        {
            Focusable = true;
            ClipToBounds = true;
            //movementTimer = SetupMovementTimer();
        }

        #endregion ZoomBorder

        #region DragButton

        public MouseButton? DragButton
        {
            get { return (MouseButton?)GetValue(DragButtonProperty); }
            set { SetValue(DragButtonProperty, value); }
        }

        public static readonly DependencyProperty DragButtonProperty = DependencyProperty.Register
            ("DragButton", typeof(MouseButton?), typeof(ZoomBorder), new FrameworkPropertyMetadata(MouseButton.Right));

        #endregion DragButton

        #region MouseWheelZoom

        public bool MouseWheelZoom
        {
            get { return (bool)GetValue(MouseWheelZoomProperty); }
            set { SetValue(MouseWheelZoomProperty, value); }
        }

        public static readonly DependencyProperty MouseWheelZoomProperty = DependencyProperty.Register
            ("MouseWheelZoom", typeof(bool), typeof(ZoomBorder), new PropertyMetadata(true));

        #endregion MouseWheelZoom

        #region Translate Functions

        private RotateTransform GetRotateTransform(UIElement element)
        {
            return (RotateTransform)((TransformGroup)element.RenderTransform)
              .Children.First(tr => tr is RotateTransform);
        }

        private TranslateTransform GetTranslateTransform(UIElement element)
        {
            return (TranslateTransform)((TransformGroup)element.RenderTransform)
              .Children.First(tr => tr is TranslateTransform);
        }

        private static ScaleTransform GetScaleTransform(UIElement element)
        {
            return (ScaleTransform)((TransformGroup)element.RenderTransform).Children.First(tr => tr is ScaleTransform);
        }

        #endregion Translate Functions

        #region Child

        public override UIElement Child
        {
            get { return base.Child; }
            set
            {
                if (value != null && value != Child)
                    Initialize(value);
                base.Child = value;
            }
        }

        private void Initialize(UIElement element)
        {
            child = element;
            if (child != null)
            {
                TransformGroup group = new TransformGroup();
                ScaleTransform st = new ScaleTransform();
                group.Children.Add(st);
                TranslateTransform tt = new TranslateTransform();
                group.Children.Add(tt);
                RotateTransform rt = new RotateTransform();
                group.Children.Add(rt);
                child.RenderTransform = group;
                child.RenderTransformOrigin = new Point(0.0, 0.0);

                child.PreviewKeyUp += Child_PreviewKeyUp; ;

                Reset();
            }
        }

        private void Child_PreviewKeyUp(object sender, KeyEventArgs e)
        {
        }

        #endregion Child

        #region Zoom...

        public void Reset()
        {
            ResetZoom();
            ResetPan();
        }

        // public Transform GetCurrentTransform() {             return GetScaleTransform(this.child);        }

        public void ResetZoomOld()
        {
            if (child == null) return;

            var st = GetScaleTransform(child);

            double width = 2510;
            double height = 3010;
            var fw = width / height;
            var fh = height / width;

            //MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));
            var childSize = child.DesiredSize;
            var fWidth = DesiredSize.Width / childSize.Width;
            var fHeight = DesiredSize.Height / childSize.Height;

            st.ScaleX = 1;
            st.ScaleY = 1;
        }

        public void ResetZoom()
        {
            if (child == null) return;

            var st = GetScaleTransform(child);
            st.ScaleX = minScale;
            st.ScaleY = minScale;

            var tt = GetTranslateTransform(child);
            tt.X = 0;
            tt.Y = 0;
        }

        public double ZoomWidth { get => child == null ? 0f : GetScaleTransform(child).ScaleX; set { if (child == null) return; GetScaleTransform(child).ScaleX = value; } }

        public double ZoomHeight { get => child == null ? 0f : GetScaleTransform(child).ScaleY; set { if (child == null) return; GetScaleTransform(child).ScaleY = value; } }

        #endregion Zoom...

        #region Pan

        public void ResetPan()
        {
            // reset pan
            if (child == null) return;
            var tt = GetTranslateTransform(child);
            tt.X = 0.0;
            tt.Y = 0.0;
        }

        public double RotateAngle { get => child == null ? 0f : GetRotateTransform(child).Angle; set { if (child == null) return; GetRotateTransform(child).Angle = value; } }

        public double PanX { get => child == null ? 0f : GetTranslateTransform(child).X; set { if (child == null) return; GetTranslateTransform(child).X = value; } }

        public double PanY { get => child == null ? 0f : GetTranslateTransform(child).Y; set { if (child == null) return; GetTranslateTransform(child).Y = value; } }

        public float movingSpeed = 15;

        //private DispatcherTimer movementTimer;

        //private DispatcherTimer SetupMovementTimer()
        //{
        //    var movementTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(20) };
        //    movementTimer.Tick += DispatcherTimer_Tick;
        //    movementTimer.Start();
        //    return movementTimer;   
        //}

        //private void DispatcherTimer_Tick(object? sender, EventArgs e)
        //{
        //    // Pan(movingX, movingY);
        //}

        // private float movingX = 0; private float movingY = 0;

        public void Pan(float offsetX, float offsetY)
        {
            if(child == null) return;
            if (offsetX == 0 && offsetY == 0) return;

            var tt = GetTranslateTransform(child);
            tt.X = tt.X + offsetX;
            tt.Y = tt.Y + offsetY;
        }

        #endregion Pan

        #region Keyboard

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.R) { Reset(); e.Handled = true; return; }

            if (e.Key == Key.Add || e.Key == Key.OemPlus) { Zoom(+mouseWheelAdjust); e.Handled = true; return; }
            if (e.Key == Key.Subtract || e.Key == Key.OemMinus) { Zoom(-mouseWheelAdjust); e.Handled = true; return; }

            if (e.Key == Key.Up) { Pan(0, +movingSpeed); e.Handled = true; return; }
            if (e.Key == Key.Down) { Pan(0, -movingSpeed); e.Handled = true; return; }
            if (e.Key == Key.Left) { Pan(+movingSpeed, 0); e.Handled = true; return; }
            if (e.Key == Key.Right) { Pan(-movingSpeed, 0); e.Handled = true; return; }

            //if (e.Key == Key.Up) { this.movingY = -movingSpeed; e.Handled = true; return; }
            //if (e.Key == Key.Down) { this.movingY = +movingSpeed; e.Handled = true; return; }
            //if (e.Key == Key.Left) { this.movingX = -movingSpeed; e.Handled = true; return; }
            //if (e.Key == Key.Right) { this.movingX = +movingSpeed; e.Handled = true; return; }

            base.OnPreviewKeyDown(e);
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            //if (e.Key == Key.Up || e.Key == Key.Down) { movingY = 0; e.Handled = true; return; }
            //if (e.Key == Key.Left || e.Key == Key.Right) { movingX = 0; e.Handled = true; return; }

            base.OnPreviewKeyUp(e);
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);
        }

        #endregion Keyboard

        #region Mouse Dragging

        private bool mouseDragging;

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (child != null) if (DragButton == e.ChangedButton)
                {
                    mouseDragging = true;

                    var tt = GetTranslateTransform(child);
                    start = e.GetPosition(this);
                    origin = new Point(tt.X, tt.Y);
                    Cursor = Cursors.ScrollAll;
                    child.CaptureMouse();
                }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            // Only if we are dragging a child
            if (child != null) if (mouseDragging) if (child.IsMouseCaptured)
                    {
                        // Move the child by the relative change in mouse position
                        var tt = GetTranslateTransform(child);
                        Vector v = start - e.GetPosition(this);
                        tt.X = origin.X - v.X;
                        tt.Y = origin.Y - v.Y;
                    }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (child != null) if (DragButton == e.ChangedButton) if (mouseDragging)
                    {
                        mouseDragging = false;
                        child.ReleaseMouseCapture();
                        Cursor = Cursors.Arrow;
                    }
        }

        #endregion Mouse Dragging

        #region Mouse Zooming

        public double mouseWheelAdjust = 0.10;

        public double minScale = 0.10;

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (!MouseWheelZoom || child == null) return;

            Point centerPoint = e.GetPosition(child);
            double zoomDelta = e.Delta > 0 ? +mouseWheelAdjust : -mouseWheelAdjust;

            Zoom(centerPoint, zoomDelta);
        }

        public void ZoomIn()
        {
            var pageViewer = Child as UIElement;
            var centerPoint = new Point(0, 0);
            double zoomDelta = +mouseWheelAdjust;
            Zoom(centerPoint, zoomDelta);
        }

        public void Zoom(double zoom)
        {
            if (child == null) return;

            var st = GetScaleTransform(child);
            var tt = GetTranslateTransform(child);

            var centerPoint = new Point
            {
                X = tt.X,// ((this.child.DesiredSize.Width / 2) ),
                Y = tt.Y // ((this.child.DesiredSize.Height / 2))
            };

            Zoom(centerPoint, zoom);
        }

        private void Zoom(Point relative, double zoom)
        {
            if (child == null) return;

            var st = GetScaleTransform(child);
            var tt = GetTranslateTransform(child);

            if (!(zoom > 0) && (st.ScaleX < minScale || st.ScaleY < minScale)) return;

            double abosuluteX;
            double abosuluteY;

            abosuluteX = relative.X * st.ScaleX + tt.X;
            abosuluteY = relative.Y * st.ScaleY + tt.Y;

            double zoomCorrected = zoom * st.ScaleX;
            st.ScaleX += zoomCorrected;
            st.ScaleY += zoomCorrected;
            //st.ScaleX += zoom;
            //st.ScaleY += zoom;

            tt.X = abosuluteX - relative.X * st.ScaleX;
            tt.Y = abosuluteY - relative.Y * st.ScaleY;
        }

        #endregion Mouse Zooming
    }
}