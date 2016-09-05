using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfApplication1
{
    public class CommonProperty
    {
        /// <summary>
        /// y轴绘制的虚线向左移动的距离
        /// </summary>
        public  double YaxisMargin = 50;

        public double CrossLineYaxisLabelMargin = 48;
        /// <summary>
        /// 
        /// </summary>
        public  double HeightOffet = 50;
        /// <summary>
        /// 用与显示图表的窗口的大小
        /// </summary>
        public  double ViewPortWidth = 0;
        public  double ViewPortHeight = 0;
        /// <summary>
        /// 显示图表窗的实际大小
        /// </summary>
        public  double ActualWidth = 0;
        public  double ActualHeight = 0;
        /// <summary>
        /// y轴缩放比例
        /// </summary>
        public  double Scale = 0;
        /// <summary>
        /// 数据集合的最大值
        /// </summary>
        public  double YMaxValue = 0;
        /// <summary>
        /// 数据集合的最小值
        /// </summary>
        public  double YMinValue = 0;
        /// <summary>
        /// 单根蜡烛的宽度
        /// </summary>
        public  double OneCandleWidth = 0;
        /// <summary>
        /// 单根蜡烛宽度+空白宽度
        /// </summary>
        public  double OneTotalWidth = 0;
        /// <summary>
        /// 所有蜡烛的对应的x轴刻度的和
        /// </summary>
        public  List<XaxisModel> XaxisLabelList;

        /// <summary>
        /// x轴的高度
        /// </summary>
        public  double AxisLabelMargin = 20;

        /// <summary>
        /// 数据加减量
        /// </summary>
        public  int DataAddOrReduceCount = 20;


    }
}
