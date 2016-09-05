using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Globalization;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Documents;
using System.Runtime.InteropServices;

namespace WpfApplication1
{
    public class CrossLine : UserControl,ICrossLine
    {
        
        public CrossLine()
        {
            Color penColor = Color.FromRgb(0, 0, 0);
            BlackPen.Brush = new SolidColorBrush(penColor);
            BlackPen.Freeze();
            dasharray.Add(0);
            dasharray.Add(6);
            this.Background = Brushes.Transparent;
            CommonData = new CommonProperty();
            IsDrawCrossXaxisLabel = true;
            FastSourceInstances = new List<IFastSource>();
        }


        /// <summary>
        /// x轴刻度显示的时间格式化
        /// </summary>
        public string XaxisDisplayFormatter
        {
            get
            {
                return (string)GetValue(XaxisDisplayFormatterProperty);
            }
            set
            {
                SetValue(XaxisDisplayFormatterProperty, value);
            }
        }

        /// <summary>
        /// Identifies the MinorStrokeDashArray dependency property.
        /// </summary>
        public static readonly DependencyProperty XaxisDisplayFormatterProperty = DependencyProperty.Register("XaxisDisplayFormatter", typeof(string), typeof(CrossLine),
            new PropertyMetadata("yyyy-MM-dd", null));

        /// <summary>
        /// 是否为主十字线
        /// </summary>
        public bool IsMainCrossLine { get; set; }
        
        CrossLineModel model = new CrossLineModel();
        Pen BlackPen = new Pen();
        double thisWidth = 0;
        double thisHeight = 0;
        int currentDataIndex = 0;
        bool IsKeyBoard = false;

        #region 绘图
        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {

            base.OnRender(drawingContext);
            if (IsDrawVirtualRect)
            {
                //正在进行拖拽放大,以虚线框显示正在放大的区域
                DrawVirtualRect(drawingContext);
            }
            else
            {
                DrawCrossLine(drawingContext);
                IsKeyBoard = false;
            }
        }
        /// <summary>
        /// 画线和放大框
        /// </summary>
        /// <param name="drawingContext"></param>
        private void DrawCrossLine(DrawingContext drawingContext)
        {
            if (model.MouseLocation != null && thisHeight != 0 && thisWidth != 0 && model.MouseLocation != new Point(0, 0))
            {
                Size rectsize = new Size(0, 20);
                double width = CommonData.OneTotalWidth == 0 ? 1 : CommonData.OneTotalWidth;
                int dataindex = (int)(model.MouseLocation.X / width);
                currentDataIndex = IsKeyBoard ? currentDataIndex : dataindex;
                XaxisModel item = null;
                double yvalue = 0;
                if (CommonData.XaxisLabelList == null || CommonData.XaxisLabelList.Count <= currentDataIndex)
                {
                    return;
                }
                item = CommonData.XaxisLabelList[currentDataIndex];
                if (model.MouseLocation.Y > 0)
                {
                    model.MouseLocation = IsKeyBoard ? new Point(0, item.YLocation) : model.MouseLocation;
                    yvalue = IsKeyBoard ? item.YValue : GetYValue(model.MouseLocation.Y);
                    DrawXCrossLine(drawingContext, yvalue);
                }
                model.MouseLocation = IsKeyBoard ? new Point(item.StartLocation + CommonData.OneCandleWidth / 2, model.MouseLocation.Y) : model.MouseLocation;
                DrawYCrossLine(drawingContext, item, rectsize);
            }
        }
        /// <summary>
        /// 画十字线的横线
        /// </summary>
        /// <param name="drawingContext"></param>
        /// <param name="yvalue"></param>
        private void DrawXCrossLine(DrawingContext drawingContext, double yvalue)
        {
            if (model.MouseLocation.X <= CommonData.ViewPortWidth)
            {
                drawingContext.PushGuidelineSet(new GuidelineSet(null,new double[] { model.MouseLocation.Y - 0.5, model.MouseLocation.Y + 0.5 }));
                drawingContext.DrawLine(BlackPen, new Point(0, model.MouseLocation.Y), new Point(thisWidth- CommonData.CrossLineYaxisLabelMargin, model.MouseLocation.Y));
                drawingContext.Pop();
            }
            string str = yvalue.ToString();
            Size rectsize = new Size(CommonData.CrossLineYaxisLabelMargin, 20);//Math.Max(str.Length * 8, CommonData.CrossLineYaxisLabelMargin)
            if (yvalue >= CommonData.YMinValue && yvalue <= CommonData.YMaxValue)
            {
                //只有鼠标y坐标对应的值，在数据最大和最小值之间 才绘制十字线抹掉显示的值
                double left = CommonData.ViewPortWidth + 4;
                double top = model.MouseLocation.Y - 10;
                //绘制十字线y轴末端的值（蓝色背景的值）
                DrawingText(drawingContext, str, new Point(left, top), rectsize);
            }
        }
        /// <summary>
        /// 画十字线的纵线
        /// </summary>
        /// <param name="drawingContext"></param>
        /// <param name="item"></param>
        /// <param name="rectsize"></param>
        private void DrawYCrossLine(DrawingContext drawingContext, XaxisModel item, Size rectsize)
        {

            if (item.StartLocation <= model.MouseLocation.X && item.EndLocation > model.MouseLocation.X && IsDrawCrossXaxisLabel)
            {
                //绘制十字线x轴末端的值（蓝色背景的值）
                string labelcontent = item.XaxisLabel.ToString(XaxisDisplayFormatter);
                rectsize.Width = labelcontent.Length * 8;// Math.Max(labelcontent.Length * 8, CommonData.CrossLineYaxisLabelMargin);
                if ((rectsize.Width + model.MouseLocation.X) > CommonData.ViewPortWidth)
                {
                    DrawingText(drawingContext, labelcontent, new Point(model.MouseLocation.X - rectsize.Width, this.thisHeight - CommonData.AxisLabelMargin), rectsize);
                }
                else
                {
                    DrawingText(drawingContext, labelcontent, new Point(model.MouseLocation.X, this.thisHeight - CommonData.AxisLabelMargin), rectsize);
                }

            }

            if (model.MouseLocation.X <= CommonData.ViewPortWidth+2)
            {
                //当鼠标超出范围时不画线
                drawingContext.PushGuidelineSet(new GuidelineSet(new double[] { model.MouseLocation.X - 0.5, model.MouseLocation.X + 0.5 }, null));
                drawingContext.DrawLine(BlackPen, new Point(model.MouseLocation.X, 0), new Point(model.MouseLocation.X, thisHeight));
                drawingContext.Pop();

            }
        }
        /// <summary>
        /// 十字线末端显示数据
        /// </summary>
        /// <param name="drawingContext"></param>
        /// <param name="labelcontent"></param>
        /// <param name="location"></param>
        private void DrawingText(DrawingContext drawingContext, string labelcontent, Point location, Size rectsize)
        {
            drawingContext.DrawRectangle(Brushes.Blue, BlackPen, new Rect(location, rectsize));
            drawingContext.DrawText(new FormattedText(labelcontent, CultureInfo.CurrentCulture,
             FlowDirection.LeftToRight, new Typeface("Tahoma"), 14, Brushes.White), location);
        } 
        #endregion
        /// <summary>
        /// 获得控件的宽度和高度
        /// </summary>
        /// <param name="arrangeBounds"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            thisHeight = arrangeBounds.Height;
            thisWidth = arrangeBounds.Width;
            return base.ArrangeOverride(arrangeBounds);
        }
        /// <summary>
        ///true显示十字线 false不显示
        /// </summary>
        private bool currentState = false;
        /// <summary>
        /// 鼠标双击显示和隐藏十字线
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDoubleClick(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            PublicMouseDoubleClick(e);
            CrossLineManager.Instance.NotifyPublicMouseDoubleClick(e,this);
        }
        /// <summary>
        /// 鼠标离开不显示十字线
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            PublicMouseLeave(e);
            CrossLineManager.Instance.NotifyPublicMouseLeave(e, this);
        }
        /// <summary>
        /// 鼠标移动绘制十字线
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseMove(e);
            PublicMouseMove(e);
            if (this.IsDrawVirtualRect)
            {
                CrossLineManager.Instance.NotifyPublicMouseMove2(e, this);
            }
            else
            {
                CrossLineManager.Instance.NotifyPublicMouseMove(e, this);
            }
        }

       
        /// <summary>
        /// 外部使用告诉x坐标直接绘制纵线
        /// </summary>
        #region 键盘键左右移动
        protected override void OnKeyDown(KeyEventArgs e)
        {

            PublicKeyDown(e);
            base.OnKeyDown(e);
            CrossLineManager.Instance.NotifyPublicKeyDown(e, this);

        }
        #endregion
        #region 放大窗口
        /// <summary>
        /// 记录鼠标左键按下时，鼠标的位置
        /// </summary>
        Point MouseLeftButtonPressL = new Point(0, 0);
        /// <summary>
        /// 记录鼠标移动时，鼠标的实时位置
        /// </summary>
        Point MouseMoveCurrentL = new Point(0, 0);
        /// <summary>
        /// pen的虚线
        /// </summary>
        List<double> dasharray = new List<double>();
        /// <summary>
        /// true表示正在进行拖拽放大，false表示正常移动鼠标
        /// </summary>
        private bool IsDrawVirtualRect = false;
        private bool IsMouseButtonPress = false;
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            PrivateMouseLeftButtonDown(e);
            CrossLineManager.Instance.NotifyPublicMouseLeftButtonDown(e, this);

        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            PrivateMouseLeftButtonUp(e);
            CrossLineManager.Instance.NotifyPublicMouseLeftButtonUp(e, this);
        }

        /// <summary>
        /// 拖放结束后的操作
        /// </summary>
        private void ScaleCompleted()
        {
            double max = 0, min = 0;
            double width = CommonData.OneTotalWidth;
            double startL = Math.Min(MouseLeftButtonPressL.X, MouseMoveCurrentL.X);
            double endL= Math.Max(MouseLeftButtonPressL.X, MouseMoveCurrentL.X);
            int startindex = (int)(startL / width);
            int endindex = (int)(endL / width);
            MouseLeftButtonPressL = new Point(0, 0);
            MouseMoveCurrentL = new Point(0, 0);
            if (endindex - startindex >= 5)
            {
                foreach (var item in FastSourceInstances)
                {
                    item.DragZoomIn(startindex, endindex);
                    if (max < item.GetYaxisMax())
                        max = item.GetYaxisMax();
                    double tempmin = item.GetYaxisMin();
                    if (min > tempmin || min ==0)
                        min = tempmin;
                }
                foreach (var item in FastSourceInstances)
                {
                    item.SetYaxisMax(max);
                    item.SetYaxisMin(min);
                    item.ItemsSourceChange();
                }

            }
            this.IsDrawVirtualRect = false;
            this.InvalidateVisual();
            
        }
        private void DrawVirtualRect(DrawingContext drawingContext)
        {
            Pen BlackPen = new Pen(Brushes.Black, 1);
            BlackPen.DashStyle = new DashStyle(dasharray, 0);
            BlackPen.Freeze();
            Brush brush = new SolidColorBrush(Color.FromArgb(55, Colors.Gray.R, Colors.Gray.G, Colors.Gray.B));
            GeometryGroup gg = new GeometryGroup();
            gg.Children.Add(new RectangleGeometry(new Rect(new Size(CommonData.ViewPortWidth, CommonData.ViewPortHeight+CommonData.HeightOffet/2))));
            gg.Children.Add(new RectangleGeometry(new Rect(MouseLeftButtonPressL, MouseMoveCurrentL)));
            drawingContext.DrawGeometry(brush, BlackPen, gg);
        }
        #endregion
        /// <summary>
        /// 滚轮进行放大缩小
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            PublicMouseWheel(e);
            CrossLineManager.Instance.NotifyPublicMouseWheel(e, this);
        }
        #region 私有方法
        protected double GetScaledY(double yvalue)
        {
            return CommonData.ViewPortHeight - yvalue * CommonData.Scale + CommonData.HeightOffet / 2;
        }
        protected double GetYValue(double mouseLocationY)
        {

            return Math.Round(CommonData.YMinValue + (CommonData.ViewPortHeight + CommonData.HeightOffet / 2 - mouseLocationY) / CommonData.Scale, 2);
        }

      
        /// <summary>
        /// 限定鼠标只能在指定的矩形框中移动
        /// </summary>
        /// <param name="e"></param>
        private void PrivateMouseLeftButtonDown(MouseButtonEventArgs e)
        {
         
            IsMouseButtonPress = true;
            MouseLeftButtonPressL = e.GetPosition(this);
            if (currentState)
            {
                //鼠标单击按下后隐藏十字线
                OnMouseLeave(e);
            }

        }
        /// <summary>
        /// 限定鼠标只能在指定的矩形框中移动
        /// </summary>
        /// <param name="e"></param>
        private void PrivateMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            CommonMethod.RestrictMouse(IntPtr.Zero);
            IsMouseButtonPress = false;
            OnMouseMove(e);
            if (this.IsDrawVirtualRect)
            {
                ScaleCompleted();
            }
        }
        #endregion



        #region 外部通知使用
        public  void PublicMouseDoubleClick(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton != System.Windows.Input.MouseButtonState.Pressed) { return; }

            if (currentState)
            {
                currentState = !currentState;
                OnMouseLeave(e);
            }
            else
            {
                currentState = !currentState;
                OnMouseMove(e);
            }
        }

        public  void PublicMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            if (IsMouseButtonPress)
            {

                if (MouseLeftButtonPressL.X > 0 && MouseLeftButtonPressL.Y > 0 && MouseLeftButtonPressL.Y<=CommonData.ViewPortHeight + CommonData.HeightOffet / 2
                    && MouseLeftButtonPressL.X<= CommonData.ViewPortWidth)
                {
                    #region 限制鼠标活动范围
                    Point targetLoc = this.PointToScreen(new Point(0, 0));
                    //起点y值+1，
                    System.Drawing.Rectangle r = new System.Drawing.Rectangle((int)targetLoc.X, (int)targetLoc.Y + 1, (int)(targetLoc.X + CommonData.ViewPortWidth), (int)(targetLoc.Y + CommonData.ViewPortHeight + CommonData.HeightOffet / 2));
                    CommonMethod.RestrictMouse(ref r); 
                    #endregion
                    MouseMoveCurrentL = e.GetPosition(this);
            
                    if (Math.Abs(MouseMoveCurrentL.X - MouseLeftButtonPressL.X) > 0)
                    {
                        this.IsDrawVirtualRect = true;
           
                        this.InvalidateVisual();
                    }
                }

            }
            else
            {
               
                if (currentState)
                {
                    //显示十字线并随鼠标移动
                    model.MouseLocation = e.GetPosition(this);
                    if (IsMainCrossLine)
                    {
                        this.Focusable = true;
                        Keyboard.Focus(this);
                    }
                    this.InvalidateVisual();
                }
                //else
                //{
                    Point p= e.GetPosition(this);
                    //通知显示提示信息
                    NotifyToolTip(p.X,p.Y);
                //}
                
            }
        }
        /// <summary>
        /// 显示提示tooltip
        /// </summary>
        /// <param name="x">x坐标</param>
        /// <param name="y">y坐标</param>
        private void NotifyToolTip(double x,double y)
        {
            double width = CommonData.OneTotalWidth == 0 ? 1 : CommonData.OneTotalWidth;
            int dataindex = (int)(x / width);
            XaxisModel xm = null;
            if (CommonData.XaxisLabelList == null || CommonData.XaxisLabelList.Count <= dataindex)
            {
                return;
            }
            xm = CommonData.XaxisLabelList[dataindex];
            if (xm.StartLocation <= x && xm.EndLocation > x  && y>0)
            {
                foreach (var item in FastSourceInstances)
                {
                    item.NotifyToolTip(dataindex, x, y);
                }
            }
            else
            {
                foreach (var item in FastSourceInstances)
                {
                    item.NotifyToolTip(int.MaxValue, x, y);
                }
            }
        }
        public  void PublicDragMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            if (IsMouseButtonPress)
            {

                if (MouseLeftButtonPressL.X > 0)
                {

                    MouseMoveCurrentL = e.GetPosition(this);
                    if (Math.Abs(MouseMoveCurrentL.X - MouseLeftButtonPressL.X) > 0)
                    {
                        this.IsDrawVirtualRect = true;
                    }
                }

            }
            else
            {
                if (currentState)
                {
                    //显示十字线并随鼠标移动
                    model.MouseLocation = e.GetPosition(this);
                    this.Focusable = true;
                    Keyboard.Focus(this);
                    this.InvalidateVisual();
                }
            }
        }

        public  void PublicMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            if (this.IsDrawVirtualRect)
            {
                ScaleCompleted();
                return;
            }
            model.MouseLocation = new Point(0, 0);
            this.InvalidateVisual();
        }
        public  void PublicKeyDown(KeyEventArgs e)
        {
            
            e.Handled = true;
            double max = 0, min = 0;
            switch (e.Key)
            {
                case Key.Left:
                    if (currentDataIndex == 0)
                    {
                        foreach (var item in FastSourceInstances)
                        {
                            item.KeyDownLeft();
                            if (max < item.GetYaxisMax())
                                max = item.GetYaxisMax();
                            double tempmin = item.GetYaxisMin();
                            if (min > tempmin || min == 0)
                                min = tempmin;
                        }
                        foreach (var item in FastSourceInstances)
                        {
                            item.SetYaxisMax(max);
                            item.SetYaxisMin(min);
                            item.ItemsSourceChange();
                        }
                        IsKeyBoard = true;
                        this.InvalidateVisual();
                        return;
                    }
                    currentDataIndex--;
                    IsKeyBoard = true;

                    this.InvalidateVisual();

                    break;
                case Key.Right:

                    if (CommonData.XaxisLabelList != null && currentDataIndex == (CommonData.XaxisLabelList.Count - 1))
                    {

                        if (currentDataIndex == (CommonData.XaxisLabelList.Count - 1))
                        {
                            IsKeyBoard = true;
                          
                            foreach (var item in FastSourceInstances)
                            {
                                item.KeyDownRight();
                                if (max < item.GetYaxisMax())
                                    max = item.GetYaxisMax();
                                double tempmin = item.GetYaxisMin();
                                if (min > tempmin || min == 0)
                                    min = tempmin;
                            }
                            foreach (var item in FastSourceInstances)
                            {
                                item.SetYaxisMax(max);
                                item.SetYaxisMin(min);
                                item.ItemsSourceChange();
                            }
                            this.InvalidateVisual();
                            break;
                        }
                    }
                    currentDataIndex++;
                    IsKeyBoard = true;
                    this.InvalidateVisual();
                    break;
                default:
                    break;
            }
           
        }
        public  void PublicKeyDown(KeyEventArgs e,bool IsDrawXLine)
        {
           
            if (!IsDrawXLine)
            {
                model.MouseLocation = new Point(model.MouseLocation.X, 0);
            }
            e.Handled = true;
            double max = 0, min = 0;
            switch (e.Key)
            {
                case Key.Left:
                    if (currentDataIndex == 0)
                    {
                        foreach (var item in FastSourceInstances)
                        {
                            item.KeyDownLeft();
                            if (max < item.GetYaxisMax())
                                max = item.GetYaxisMax();
                            double tempmin = item.GetYaxisMin();
                            if (min > tempmin || min == 0)
                                min = tempmin;
                        }
                        foreach (var item in FastSourceInstances)
                        {
                            item.SetYaxisMax(max);
                            item.SetYaxisMin(min);
                            item.ItemsSourceChange();
                        }
                        IsKeyBoard = true;
                        this.InvalidateVisual();
                        break;
                    }
                    currentDataIndex--;
                    IsKeyBoard = true;

                    this.InvalidateVisual();

                    break;
                case Key.Right:

                    if (CommonData.XaxisLabelList != null && currentDataIndex == (CommonData.XaxisLabelList.Count - 1))
                    {
                        if (currentDataIndex == (CommonData.XaxisLabelList.Count - 1))
                        {
                            IsKeyBoard = true;
                           
                            foreach (var item in FastSourceInstances)
                            {
                                item.KeyDownRight();
                                if (max < item.GetYaxisMax())
                                    max = item.GetYaxisMax();
                                double tempmin = item.GetYaxisMin();
                                if (min > tempmin || min == 0)
                                    min = tempmin;
                            }
                            foreach (var item in FastSourceInstances)
                            {
                                item.SetYaxisMax(max);
                                item.SetYaxisMin(min);
                                item.ItemsSourceChange();
                            }
                            this.InvalidateVisual();
                            break;
                        }
                    }
                    currentDataIndex++;
                    IsKeyBoard = true;
                    this.InvalidateVisual();
                    break;
                default:
                    break;
            }
           
        }
        public  void PublicMouseWheel(MouseWheelEventArgs e)
        {
            double max=0, min=0;
            if (e.Delta < 0)
            {
              
                foreach (var item in FastSourceInstances)
                {
                    item.MouseWheelZoomOut();
                    if (max < item.GetYaxisMax())
                        max = item.GetYaxisMax();
                    if (min > item.GetYaxisMin() || min == 0)
                        min = item.GetYaxisMin();
                }
                foreach (var item in FastSourceInstances)
                {
                    item.SetYaxisMax(max);
                    item.SetYaxisMin(min);
                    item.ItemsSourceChange();
                }
            }
            if (e.Delta > 0)
            {
               
                foreach (var item in FastSourceInstances)
                {
                    item.MouseWheelZoomIn();
                    if (max < item.GetYaxisMax())
                        max = item.GetYaxisMax();
                    if (min > item.GetYaxisMin() || min == 0)
                        min = item.GetYaxisMin();
                }
                foreach (var item in FastSourceInstances)
                {
                    item.SetYaxisMax(max);
                    item.SetYaxisMin(min);
                    item.ItemsSourceChange();
                }
            }
            this.InvalidateVisual();
        }

        public  void PublicMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            
            IsMouseButtonPress = true;
            MouseLeftButtonPressL = e.GetPosition(this);
            if (currentState)
            {
                //鼠标单击按下后隐藏十字线
                OnMouseLeave(e);
            }

        }


        public  void PublicMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            IsMouseButtonPress = false;
            OnMouseMove(e);
            if (this.IsDrawVirtualRect)
            {
                ScaleCompleted();
            }
        }
        #endregion
        #region 接口属性的实现
        private CommonProperty _CommonData;
        public  CommonProperty CommonData
        {
            get
            {
                return _CommonData;
            }

            set
            {
                _CommonData = value;
            }
        }
        private List<IFastSource> _FastSourceInstances;
        public  List<IFastSource> FastSourceInstances
        {
            get
            {
                return _FastSourceInstances;
            }

            set
            {
                _FastSourceInstances = value;
            }
        }
        private bool _IsDrawCrossXaxisLabel;
        public  bool IsDrawCrossXaxisLabel
        {
            get
            {
                return _IsDrawCrossXaxisLabel;
            }

            set
            {
                _IsDrawCrossXaxisLabel = value;
            }
        }
       
        #endregion
    }
    public class CrossLineModel
    {
        public Point MouseLocation { get; set; }
    }
}
