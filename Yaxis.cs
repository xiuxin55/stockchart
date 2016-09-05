using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WpfApplication1
{
    /// <summary>
    /// y轴
    /// </summary>
    public class Yaxis
    {
        CommonProperty CommonData;
        IFastSource FastSourceInstance;
        int NumberPointCount = 2;//y轴小数点位数
        public Yaxis(CommonProperty CommonData, IFastSource FastSourceInstance, int NumberPointCount = 2)
        {
            this.CommonData = CommonData;
            this.FastSourceInstance= FastSourceInstance;
            this.NumberPointCount = NumberPointCount;
        }
        /// <summary>
        /// 画y轴
        /// </summary>
        public  void ProduceYaxis(DrawingCanvas _rootCanvas)
        {
            if (_rootCanvas == null || FastSourceInstance.GetCachedCandleList().Count == 0 || CommonData.ViewPortWidth == 0 || CommonData.ViewPortHeight == 0)
                return;
            //y轴刻度位置集合
            List<Point> YaxisPoints = new List<Point>();
            //y轴刻度值集合
            List<double> YValues = new List<double>();

            double ymax = CommonData.YMaxValue;
            double ymin = CommonData.YMinValue;
            CommonData.Scale = CommonData.ViewPortHeight / (ymax - ymin);
            //y轴刻度之间的间隔，这里始终显示5个刻度
            double span =Math.Round((ymax - ymin) / 5, NumberPointCount);
    
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
 
            }
            axisContext.Close();
            //在画布画路径
            _rootCanvas.DrawingYaxis(axispathStream, 1, YaxisPoints, YValues);
        }

    }
}
