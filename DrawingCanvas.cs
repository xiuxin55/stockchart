using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Globalization;

namespace WpfApplication1
{
    public class DrawingCanvas : Canvas, IDrawingMethod
    {
        List<Visual> _visuals = new List<Visual>();
        private Brush redBrush;
        public DrawingCanvas()
        {
            dasharray.Add(0);
            dasharray.Add(6);
        }
        /// <summary>
        /// 红色画刷，用于获取界面绑定的画刷
        /// </summary>
        public Brush RedBrush
        {
            get { return redBrush ?? Brushes.Red; }
            set { redBrush = value; }
        }
        private Brush greenBrush;
        /// <summary>
        /// 绿色画刷，用于获取界面绑定的画刷
        /// </summary>
        public Brush GreenBrush
        {
            get { return greenBrush ?? Brushes.Green; }
            set { greenBrush = value; }
        }
        protected override Visual GetVisualChild(int index)
        {
            return _visuals.Count>index? _visuals[index]:null;
        }
        protected override int VisualChildrenCount
        {
            get
            {
                return _visuals.Count;
            }
        }
        protected override void OnVisualChildrenChanged(System.Windows.DependencyObject visualAdded, System.Windows.DependencyObject visualRemoved)
        {
            if (visualAdded != null && visualAdded is Visual)
            {
                _visuals.Add((Visual)visualAdded);
            }
            if (visualRemoved != null && visualRemoved is Visual)
            {
                _visuals.Remove((Visual)visualRemoved);
            }
        }
        /// <summary>
        /// 绘制蜡烛
        /// </summary>
        /// <param name="positiveGroup"></param>
        /// <param name="negativeGroup"></param>
        /// <param name="IsDrawingLine"></param>
        /// <param name="CustomDrawLineWidth"></param>

        public void DrawingGraphics(Geometry positiveGroup, Geometry negativeGroup, bool IsDrawingLine, double CustomDrawLineWidth)
        {
            positiveGroup.Freeze();
            negativeGroup.Freeze();
    
            DrawingVisual drawingVisual = new DrawingVisual(); 
            RedBrush.Freeze();
            GreenBrush.Freeze();
            Pen RedPen = new Pen(RedBrush, CustomDrawLineWidth);
            Pen GreenPen = new Pen(GreenBrush, CustomDrawLineWidth);
            RedPen.Freeze();
            GreenPen.Freeze();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            if (IsDrawingLine)
            {
                drawingContext.DrawGeometry(null, RedPen, positiveGroup);
                drawingContext.DrawGeometry(null, GreenPen, negativeGroup);
            }
            else
            {
                Brush white = Brushes.White;
                white.Freeze();
                drawingContext.DrawGeometry(white, null , positiveGroup);
                drawingContext.DrawGeometry(GreenBrush, GreenPen, negativeGroup);
            }
            drawingContext.Close();
            BitmapCache bc = new BitmapCache();
            bc.SnapsToDevicePixels = true;
            bc.Freeze();
            drawingVisual.CacheMode = bc;
            
            this.AddVisualChild(drawingVisual);
        }

        /// <summary>
        /// 画折线
        /// </summary>
        /// <param name="LineGroup"></param>
        /// <param name="CustomDrawLineWidth"></param>
        public void DrawingGraphics(Geometry LineGroup, double CustomDrawLineWidth)
        {
            LineGroup.Freeze();
            DrawingVisual drawingVisual = new DrawingVisual();
            RedBrush.Freeze();
            GreenBrush.Freeze();
            Pen RedPen = new Pen(RedBrush, CustomDrawLineWidth);
            Pen GreenPen = new Pen(GreenBrush, CustomDrawLineWidth);
            RedPen.Freeze();
            GreenPen.Freeze();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            drawingContext.DrawGeometry(null, RedPen, LineGroup);
            drawingContext.Close();
            BitmapCache bc = new BitmapCache();
            bc.SnapsToDevicePixels = true;
            bc.Freeze();
            drawingVisual.CacheMode = bc;
            this.AddVisualChild(drawingVisual);
        }

        public void DrawingGraphics(Geometry positiveGroup, Geometry negativeGroup, bool IsDrawingLine, double CustomDrawLineWidth, DoubleCollection guidlinesX, DoubleCollection guidlinesY)
        {
            positiveGroup.Freeze();
            negativeGroup.Freeze();


            DrawingVisual drawingVisual = new DrawingVisual();
            RedBrush.Freeze();
            GreenBrush.Freeze();
            Pen RedPen = new Pen(RedBrush, CustomDrawLineWidth);
            Pen GreenPen = new Pen(GreenBrush, CustomDrawLineWidth);
            RedPen.Freeze();
            GreenPen.Freeze();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            drawingContext.PushGuidelineSet(new GuidelineSet(guidlinesX.ToArray(), guidlinesY.ToArray()));
            if (IsDrawingLine)
            {
                drawingContext.DrawGeometry(null, RedPen, positiveGroup);
                drawingContext.DrawGeometry(null, GreenPen, negativeGroup);
            }
            else
            {
                Brush white = Brushes.White;
                white.Freeze();
               
                drawingContext.DrawGeometry(white, RedPen, positiveGroup);
                drawingContext.DrawGeometry(GreenBrush, GreenPen, negativeGroup);
            }
            drawingContext.Pop();
            drawingContext.Close();
            
            BitmapCache bc = new BitmapCache();
            bc.SnapsToDevicePixels = true;
            bc.Freeze();
            drawingVisual.CacheMode = bc;
            this.AddVisualChild(drawingVisual);
        }
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
              
        }


        public void DrawingLines(List<Line> positiveGroup, List<Line> negativeGroup, double CustomDrawLineWidth)
        {
            DrawingVisual drawingVisual = new DrawingVisual();

            Pen RedPen = new Pen(RedBrush, CustomDrawLineWidth);
            Pen GreenPen = new Pen(GreenBrush, CustomDrawLineWidth);
            RedPen.Freeze();
            GreenPen.Freeze();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            foreach (var item in positiveGroup)
            {
                drawingContext.DrawLine(RedPen, item.StartLocation, item.EndLocation);
            }
            foreach (var item in negativeGroup)
            {
                drawingContext.DrawLine(GreenPen, item.StartLocation, item.EndLocation);
            }
            drawingContext.Close();
            
            BitmapCache bc = new BitmapCache();
            bc.SnapsToDevicePixels = true;
            bc.Freeze();
            drawingVisual.CacheMode = bc;

            
            this.AddVisualChild(drawingVisual);
        }


        List<double> dasharray = new List<double>();
        /// <summary>
        /// 以虚线绘制y轴
        /// </summary>
        /// <param name="YaxisPath"></param>
        /// <param name="CustomDrawLineWidth"></param>
        /// <param name="YLabelPoint"></param>
        /// <param name="YValues"></param>
        public void DrawingYaxis(Geometry YaxisPath, double CustomDrawLineWidth, List<Point> YLabelPoint, List<double> YValues)
        {
            YaxisPath.Freeze();
            DrawingVisual drawingVisual = new DrawingVisual();
            Pen BlackPen = new Pen(Brushes.Black, CustomDrawLineWidth);
            BlackPen.DashStyle = new DashStyle(dasharray,0);
            BlackPen.Freeze();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            drawingContext.DrawGeometry(null, BlackPen, YaxisPath);
            for (int i = 0; i < YLabelPoint.Count; i++)
            {
                drawingContext.DrawText(new FormattedText(YValues[i].ToString(), CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight, new Typeface("Tahoma"), 14, Brushes.Black),new Point(YLabelPoint[i].X+4,YLabelPoint[i].Y-10));
            }
            drawingContext.Close();
            BitmapCache bc = new BitmapCache();
            bc.SnapsToDevicePixels = true;
            bc.Freeze();
            drawingVisual.CacheMode = bc;
            //drawingVisual.Drawing.Freeze();
            this.AddVisualChild(drawingVisual);
        }

        /// <summary>
        /// 绘制x轴
        /// </summary>
        /// <param name="YaxisPath"></param>
        /// <param name="CustomDrawLineWidth"></param>
        /// <param name="YLabelPoint"></param>
        /// <param name="LabelList"></param>
        public void DrawingXaxis(Geometry YaxisPath, double CustomDrawLineWidth, List<Point> YLabelPoint, List<string> LabelList)
        {
            YaxisPath.Freeze();
            DrawingVisual drawingVisual = new DrawingVisual();
            Brush blackbrush = Brushes.Black;
            blackbrush.Freeze();
            Pen BlackPen = new Pen(blackbrush, CustomDrawLineWidth);
            BlackPen.Freeze();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            drawingContext.DrawGeometry(null, BlackPen, YaxisPath);
            for (int i = 0; i < YLabelPoint.Count; i++)
            {
                drawingContext.DrawText(new FormattedText(LabelList[i].ToString(), CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight, new Typeface("Tahoma"), 14, Brushes.Black), new Point(YLabelPoint[i].X+2, YLabelPoint[i].Y));
                
            }
            drawingContext.Close();
            BitmapCache bc = new BitmapCache();
            bc.SnapsToDevicePixels = true;
            bc.Freeze();
            drawingVisual.CacheMode =bc;
            //drawingVisual.Drawing.Freeze();
            this.AddVisualChild(drawingVisual);
        }
        
        
        /// <summary>
        /// 清除图形
        /// </summary>
        public void ClearVisual()
        {
            #region 移除已存在的DrawingVisual
            List<Visual> visualsClone = _visuals != null ? _visuals.ToList() : null;
            foreach (var item in visualsClone)
            {

                this.RemoveVisualChild(item);

            }
            if (visualsClone != null)
            {
                visualsClone.Clear();
                visualsClone = null;
            }
            #endregion
        }
    }
}
