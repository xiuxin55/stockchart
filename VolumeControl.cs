using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WpfApplication1
{
    public class VolumeControl : BaseContentControl<CandleModel>
    {
        public VolumeControl()
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
            StreamGeometry positiveStream = new StreamGeometry();
            //绿色柱状图路径，当柱状图宽度很小时，画竖线
            StreamGeometry negativeStream = new StreamGeometry();

            //数据集合
            List<CandleModel> Datas = (List<CandleModel>)FastSourceInstance.GetCachedCandleList();
            //得到集合中的最大最小值，用于画y轴
            double ymax =  CommonData.YMaxValue;
            double ymin = CommonData.YMinValue;
            if (ymax == 0 && ymin == 0) { return; }
            //y轴和数据对应的缩放比例
            CommonData.Scale = CommonData.ViewPortHeight / (ymax - ymin);
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
            DoubleCollection guidlinesX = new DoubleCollection();
            DoubleCollection guidlinesY = new DoubleCollection();
            YValues = new double[Datas.Count, 2];
            StreamGeometryContext positiveContext = positiveStream.Open();
            StreamGeometryContext negativeContext = negativeStream.Open();
            foreach (var item in Datas)
            {


                double start = index * drawwidth;
                double left = start;//柱状图左边的x坐标
                double right = start + onecandlewidth;//柱状图右边界的x坐标
                double center = start + onecandlewidth / 2;//柱状图中心的x坐标
                double top = CommonData.ViewPortHeight - (item.Volume-ymin)* CommonData.Scale + CommonData.HeightOffet / 2;// CommonMethod.GetScaledY(item.Open - ymin);// 柱状图对应的y坐标
                double bottom = CommonData.ViewPortHeight + CommonData.HeightOffet/2;


                XaxisModel xm = new XaxisModel();
                xm.StartLocation = left;
                xm.XaxisLabel = item.Date;
                xm.EndLocation = right;// onecandlewidth > drawwidth ? right : right + onespacewidth;
                xm.YLocation = top;
                xm.YValue = item.Volume;
                XaxisLabelList.Add(xm);
                YValues[index, 0] = top;
                YValues[index, 1] = bottom;
                if (!isLine)
                {
                    //画柱状图
                    guidlinesX.Add(left - strokeness / 2);
                    guidlinesX.Add(left + strokeness / 2);

                    guidlinesX.Add(right - strokeness / 2);
                    guidlinesX.Add(right + strokeness / 2);

                    guidlinesY.Add(top - strokeness / 2);
                    guidlinesY.Add(top + strokeness / 2);

                    guidlinesY.Add(bottom - strokeness / 2);
                    guidlinesY.Add(bottom + strokeness / 2);
                    
                    if (item.UporDown)
                    {

                        positiveContext.BeginFigure(new Point(left, top), true, true);
                        positiveContext.LineTo(new Point(left, bottom), true, false);
                        positiveContext.LineTo(new Point(right, bottom), true, false);
                        positiveContext.LineTo(new Point(right, top), true, false);
                    }
                    else
                    {
                        negativeContext.BeginFigure(new Point(left, top), true, true);
                        negativeContext.LineTo(new Point(left, bottom), true, false);
                        negativeContext.LineTo(new Point(right, bottom), true, false);
                        negativeContext.LineTo(new Point(right, top), true, false);
                        negativeContext.LineTo(new Point(left, top), true, false);

                    }

                }
                else
                {
                    //画竖线
                    if (item.UporDown)
                    {
                        //红色柱状图线
                        positiveContext.BeginFigure(new Point(center, top), false, false);
                        positiveContext.LineTo(new Point(center, bottom), true, false);
                     
                    }
                    else
                    {
                        //绿色柱状图线
                        negativeContext.BeginFigure(new Point(center, top), false, false);
                        negativeContext.LineTo(new Point(center, bottom), true, false);
                    }
                    guidlinesX.Add(center - strokeness / 2);
                    guidlinesX.Add(center + strokeness / 2);
                }
                index++;
                
            }
            negativeContext.Close();
            positiveContext.Close();
            //将x轴数据集合赋值给静态变量
            CommonData.XaxisLabelList = XaxisLabelList;
            _rootCanvas.DrawingGraphics(positiveStream, negativeStream, isLine, strokeness, guidlinesX, guidlinesY);
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

        public override void ItemsSoureChangedSetData(List<CandleModel> newvalue)
        {
            FastSourceInstance.SetParameter(CommonData, p => p.Volume,null);
            FastSourceInstance.SetCachedCandleList(newvalue);
            DrawingImage();
        }

        public override void CrossLineControlCallback()
        {
            base.CrossLineControlCallback();
            //CrossLineManager.Instance.Regester(Window.GetWindow(this).PersistId.ToString(), CrossLineControl);
            CrossLineManager.Instance.Regester(CrossLineControl);
        }

        #region 显示提示

        private double[,] YValues;
        protected override void DisplayToolTip(int currentindex, double x, double y)
        {
            if (YValues.Length / 2 > currentindex)
            {
                if (YValues[currentindex, 0] <= y && YValues[currentindex, 1] >= y)
                {
                    IList list = FastSourceInstance.GetCachedCandleList();
                    //显示tooltip
                    PlaceToolTip( list[currentindex]);
                    return;
                }
            }
            HideToolTip();
        }
        #endregion
    }
}
