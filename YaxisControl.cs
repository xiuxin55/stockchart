using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApplication1
{
    public class YaxisControl:ContentControl 
    {
        public CommonProperty CommonData;
        public  event Action<double,double> MaxMinValueChanged;
        int NumberPointCount = 2;//y轴小数点位数
        DrawingCanvas _rootCanvas;
        public YaxisControl()
        {
            this.CommonData = new CommonProperty();
            this.NumberPointCount = 2;
            this._rootCanvas = new DrawingCanvas();
            this.Content = this._rootCanvas;
        }
        /// <summary>
        /// y轴最大值
        /// </summary>
        public double YValueMax
        {
            get
            {
                return (double)GetValue(YValueMaxProperty);
            }
            set
            {
                SetValue(YValueMaxProperty, value);
            }
        }
        /// <summary>
        /// Identifies the MinorStrokeDashArray dependency property.
        /// </summary>
        public static readonly DependencyProperty YValueMaxProperty = DependencyProperty.Register("YValueMax", typeof(double), typeof(YaxisControl),
            new PropertyMetadata(0.0, new PropertyChangedCallback((sender, e) =>
            {
                YaxisControl yc = sender as YaxisControl;
                yc.CommonData.YMaxValue = (double)e.NewValue;
                if(yc.MaxMinValueChanged!=null)
                {
                    yc.MaxMinValueChanged(yc.CommonData.YMaxValue, yc.CommonData.YMinValue);
                }
                yc.ProduceYaxis();

            })));
  
        public void SourceChanged(double max,double min)
        {
            this.SetCurrentValue(YValueMaxProperty, max);
            this.SetCurrentValue(YValueMinProperty, min);

        }
        /// <summary>
        /// y轴最小值
        /// </summary>
        public double YValueMin
        {
            get
            {
                return (double)GetValue(YValueMinProperty);
            }
            set
            {
                SetValue(YValueMinProperty, value);

            }
        }
        /// <summary>
        /// Identifies the MinorStrokeDashArray dependency property.
        /// </summary>
        public static readonly DependencyProperty YValueMinProperty = DependencyProperty.Register("YValueMin", typeof(double), typeof(YaxisControl),
            new PropertyMetadata(0.0, new PropertyChangedCallback((sender, e) =>
            {
                YaxisControl yc = sender as YaxisControl;
                yc.CommonData.YMinValue = (double)e.NewValue  ;
                if (yc.MaxMinValueChanged != null)
                {
                    yc.MaxMinValueChanged(yc.CommonData.YMaxValue, yc.CommonData.YMinValue);
                }
                yc.ProduceYaxis();
            })));

        /// <summary>
        /// 窗口变化重新绘图
        /// </summary>
        /// <param name="sizeInfo"></param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            CommonData.ActualHeight = sizeInfo.NewSize.Height;
            CommonData.ActualWidth = sizeInfo.NewSize.Width;
            CommonData.ViewPortWidth = sizeInfo.NewSize.Width - CommonData.YaxisMargin;
            CommonData.ViewPortHeight = sizeInfo.NewSize.Height - CommonData.HeightOffet;
            ProduceYaxis();

        }
        /// <summary>
        /// 画y轴
        /// </summary>
        private  void ProduceYaxis()
        {
            if (_rootCanvas == null || CommonData.ViewPortWidth == 0 || CommonData.ViewPortHeight == 0)
                return;
            _rootCanvas.ClearVisual();
            //y轴刻度位置集合
            List<Point> YaxisPoints = new List<Point>();
            //y轴刻度值集合
            List<double> YValues = new List<double>();

            double ymax = CommonData.YMaxValue;
            double ymin = CommonData.YMinValue;
            if(ymax==0 && ymin == 0) { return; }
            CommonData.Scale = CommonData.ViewPortHeight / (ymax - ymin);
            //y轴刻度之间的间隔，这里始终显示5个刻度
            double span = Math.Round((ymax - ymin) / 5, NumberPointCount);
            
            double yaxis = ymin;
            //y轴路径
            StreamGeometry axispathStream = new StreamGeometry();
            StreamGeometryContext axisContext = axispathStream.Open();
            while (Math.Round(ymax - yaxis, 2) >= 0)
            {
                //循环画路径
                YValues.Add(Math.Round(yaxis, 2));
                double yvalue = CommonData.ViewPortHeight - (yaxis - ymin) * CommonData.Scale + CommonData.HeightOffet / 2;
                Point p = new Point(CommonData.ViewPortWidth, yvalue);
                YaxisPoints.Add(p);
                axisContext.BeginFigure(new Point(0, p.Y), false, false);
                axisContext.LineTo(p, true, false);
                yaxis += span;
                if(span == 0) { break; }

            }
            
            axisContext.Close();
            //在画布画路径
            _rootCanvas.DrawingYaxis(axispathStream, 1, YaxisPoints, YValues);
        }
    }
}
