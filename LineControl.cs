using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Collections;

namespace WpfApplication1
{
    public class LineControl : BaseContentControl<LineModel>
    {
        public LineControl()
        {

        }
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
            DrawingImage();

        }
        protected override void DrawingGraphics()
        {
            if (_rootCanvas == null || FastSourceInstance.GetCachedCandleList().Count == 0 || CommonData.ViewPortWidth == 0 || CommonData.ViewPortHeight == 0)
                return;
            double strokeness = 1;
            //红色柱状图路径，当柱状图宽度很小时，画竖线
            StreamGeometry lineStream = new StreamGeometry();
            //数据集合
            List<LineModel> Datas = (List<LineModel>)FastSourceInstance.GetCachedCandleList();
            //得到集合中的最大最小值，用于画y轴
            double ymax = CommonData.YMaxValue;
            double ymin = CommonData.YMinValue;
            if (ymax == 0 && ymin == 0) { return; }
            //y轴和数据对应的缩放比例
            CommonData.Scale = CommonData.ViewPortHeight / (ymax- ymin);
            //柱状图和空白的宽度和
            double drawwidth = CommonData.ViewPortWidth / Datas.Count;
            //单根柱状图的宽度
            double onecandlewidth = 0;
            //两个柱状图之间的间距
            double onespacewidth = 0;

            if (drawwidth > 10)
            {
                //根据宽度 调整柱状图和空白间距，所占的比例
                onecandlewidth = drawwidth * 0.7;
                onespacewidth = drawwidth * 0.3;
            }
            else if (drawwidth > 6)
            {
                onecandlewidth = drawwidth * 0.4;
                onespacewidth = drawwidth * 0.6;
            }
            else
            {

                onecandlewidth = Math.Max(1, drawwidth * 0.1);
                onespacewidth = drawwidth * 0.9;

            }
            //将柱状图+间距之和赋值给静态变量
            CommonData.OneTotalWidth = drawwidth;
            //将单根柱状图宽度赋值给静态变量
            CommonData.OneCandleWidth = onecandlewidth;
            //当前正在循环的数据的索引
            int index = 0;
            //当宽度<=2时，将柱状图画成竖线
            bool isLine = onecandlewidth <= 1;
            YValues = new double[Datas.Count, 2];
            StreamGeometryContext lineStreamContext = lineStream.Open();
            foreach (var item in Datas)
            {


                double start = index * drawwidth;
                double x = start+ onecandlewidth/2;//线的x坐标
                double y = CommonData.ViewPortHeight -(item.LineValue - ymin) * CommonData.Scale + CommonData.HeightOffet / 2;// CommonMethod.GetScaledY(item.Open - ymin);// 线图对应的y坐标

                YValues[index, 0] = y-2;
                YValues[index, 1] = y+2;

                XaxisModel xm = new XaxisModel();
                xm.XaxisLabel = item.Date;
                xm.StartLocation = x;
                xm.EndLocation = x;
                xm.YLocation = y;
                xm.YValue = item.LineValue;
                if (index == 0)
                {
                    lineStreamContext.BeginFigure(new Point(x, y), false, false);
                }
                else
                {
                    lineStreamContext.LineTo(new Point(x, y), true, true);
                }
                index++;

            }
            lineStreamContext.Close();
            //将x轴数据集合赋值给静态变量
            CommonData.XaxisLabelList = XaxisLabelList; 
            _rootCanvas.DrawingGraphics(lineStream, strokeness);
        }
        /// <summary>
        /// x轴的标签集合
        /// </summary>
        protected List<XaxisModel> XaxisLabelList = new List<XaxisModel>();
        protected override void Clear()
        {
            base.Clear();
            XaxisLabelList.Clear();
        }

        public override void ItemsSoureChangedSetData(List<LineModel> newvalue)
        {
            FastSourceInstance.SetParameter(CommonData, p => p.LineValue, p => p.LineValue);
            FastSourceInstance.SetCachedCandleList(newvalue);
            DrawingImage();
        }

        public override void CrossLineControlCallback()
        {
            base.CrossLineControlCallback();
            CrossLineManager.Instance.Regester(CrossLineControl);
        }
        #region 显示提示

        private double[,] YValues;
        protected override void DisplayToolTip(int currentindex,double x, double y)
        {
            if (YValues.Length/2 > currentindex)
            {
                if (YValues[currentindex, 0] <= y && YValues[currentindex, 1] >= y)
                {
                    IList list = FastSourceInstance.GetCachedCandleList();
                    //显示tooltip
                    PlaceToolTip(list[currentindex]);
                    return;
                }
            }
            HideToolTip();
        }
        #endregion

    }
}
