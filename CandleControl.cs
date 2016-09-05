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
using System.Windows.Input;

namespace WpfApplication1
{
    public class CandleControl: BaseContentControl<CandleModel>, IDisposable
    {
       
        public CandleControl()
        {
        }
        /// <summary>
        /// 是否画boll线
        /// </summary>
        public bool IsBoll
        {
            get
            {
                return (bool)GetValue(IsBollProperty);
            }
            set
            {
                SetValue(IsBollProperty, value);
            }
        }

        /// <summary>
        /// Identifies the MinorStrokeDashArray dependency property.
        /// </summary>
        public static readonly DependencyProperty IsBollProperty = DependencyProperty.Register("IsBoll", typeof(bool), typeof(CandleControl),
            new PropertyMetadata(false, null));
        private ObservableCollection<LineControl> _LineControls = new ObservableCollection<LineControl>();
        public ObservableCollection<LineControl> LineControls
        {
            get
            {
                return _LineControls;
            }
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

        /// <summary>
        /// 绘制蜡烛图
        /// </summary>
        protected override void DrawingGraphics()
        {
           
            if (_rootCanvas == null || FastSourceInstance.GetCachedCandleList().Count == 0 || CommonData.ViewPortWidth == 0 || CommonData.ViewPortHeight == 0)
                return;
            
            double strokeness = 1;
            //红色蜡烛路径，当蜡烛宽度很小时，画竖线
            StreamGeometry positiveStream = new StreamGeometry();
            //绿色蜡烛路径，当蜡烛宽度很小时，画竖线
            StreamGeometry negativeStream = new StreamGeometry();
            //数据集合
            List<CandleModel> Datas =(List<CandleModel>) FastSourceInstance.GetCachedCandleList();
            //得到集合中的最大最小值，用于画y轴
            double ymax = CommonData.YMaxValue ;
            double ymin = CommonData.YMinValue ;
            if (ymax == 0 && ymin == 0) { return; }
            //y轴和数据对应的缩放比例
            CommonData.Scale = CommonData.ViewPortHeight / (ymax - ymin);
            //蜡烛和空白的宽度和
            double drawwidth = CommonData.ViewPortWidth / Datas.Count;
            //单根蜡烛的宽度
            double onecandlewidth = 0;
            //两个蜡烛之间的间距
            double onespacewidth = 0;

            if (drawwidth >10)
            {
                //根据宽度 调整蜡烛和空白间距，所占的比例
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
                
                onecandlewidth =Math.Max(1, drawwidth * 0.1);
                onespacewidth = drawwidth * 0.9;

            }

            //将蜡烛+间距之和赋值给静态变量
            CommonData.OneTotalWidth = drawwidth;
            //将单根蜡烛宽度赋值给静态变量
            CommonData.OneCandleWidth = onecandlewidth;
            //当前正在循环的数据的索引
            int index=0;
            //当宽度<=2时，将蜡烛画成竖线
            bool isLine = onecandlewidth <= 1;
            StreamGeometryContext positiveContext = positiveStream.Open();
            StreamGeometryContext negativeContext = negativeStream.Open();
            YValues = new double[Datas.Count,2];
            DoubleCollection guidlinesX = new DoubleCollection();
            DoubleCollection guidlinesY = new DoubleCollection();
            foreach (var item in Datas)
            {
                double start = index * drawwidth;
                double left = start;//蜡烛左边的x坐标
                double right = start + onecandlewidth;//蜡烛右边界的x坐标
                double center = start + onecandlewidth / 2;//蜡烛中心的x坐标
                double open = CommonData.ViewPortHeight - (item.Open - ymin) * CommonData.Scale + CommonData.HeightOffet / 2;// CommonMethod.GetScaledY(item.Open - ymin);// 蜡烛开盘对应的y坐标
                double close = CommonData.ViewPortHeight - (item.Close - ymin) * CommonData.Scale + CommonData.HeightOffet / 2;// CommonMethod.GetScaledY(item.Close - ymin);//蜡烛
                double high = CommonData.ViewPortHeight - (item.High - ymin) * CommonData.Scale + CommonData.HeightOffet / 2; //CommonMethod.GetScaledY(item.High - ymin);
                double low = CommonData.ViewPortHeight - (item.Low - ymin) * CommonData.Scale + CommonData.HeightOffet / 2;// CommonMethod.GetScaledY(item.Low - ymin);
     
                XaxisModel xm = new XaxisModel();
                xm.StartLocation = left;
                xm.XaxisLabel = item.Date;
                xm.EndLocation = right;// onecandlewidth > drawwidth ? right : right + onespacewidth;
                xm.YLocation = close;
                xm.YValue = item.Close;
                XaxisLabelList.Add(xm);
                YValues[index,0] = high;
                YValues[index,1] = low;
                if (!isLine)
                {
                    //画蜡烛
                    if (open < close)
                    {
                        double tempopen = open;
                        open = close;
                        close = tempopen;
                    }
                    guidlinesX.Add(left - strokeness / 2);
                    guidlinesX.Add(left + strokeness / 2);
                    guidlinesX.Add(right - strokeness / 2);
                    guidlinesX.Add(right + strokeness / 2);
                    guidlinesY.Add(open - strokeness / 2);
                    guidlinesY.Add(open + strokeness / 2);
                    guidlinesY.Add(close - strokeness / 2);
                    guidlinesY.Add(close + strokeness / 2);
                  
                    if (item.UporDown)
                    {
                        if (IsBoll)
                        {
                            positiveContext.BeginFigure(new Point(center, high), false, false);
                            positiveContext.LineTo(new Point(center, low), true, false);
                            positiveContext.BeginFigure(new Point(left, open), false, false);
                            positiveContext.LineTo(new Point(center, open), true, false);
                            positiveContext.BeginFigure(new Point(right, close), false, false);
                            positiveContext.LineTo(new Point(center, close), true, false);

                        }
                        else
                        {
                            positiveContext.BeginFigure(new Point(center, high), true, false);
                            positiveContext.LineTo(new Point(center, close), true, false);
                            positiveContext.BeginFigure(new Point(center, low), true, false);
                            positiveContext.LineTo(new Point(center, open), true, false);
                            positiveContext.BeginFigure(new Point(left, close), true, true);
                            positiveContext.LineTo(new Point(left, open), true, false);
                            positiveContext.LineTo(new Point(right, open), true, false);
                            positiveContext.LineTo(new Point(right, close), true, false);
                        }
                    }
                    else
                    {
                        if (IsBoll)
                        {
                            negativeContext.BeginFigure(new Point(center, high), false, false);
                            negativeContext.LineTo(new Point(center, low), true, false);
                            negativeContext.BeginFigure(new Point(left, close), false, false);
                            negativeContext.LineTo(new Point(center, close), true, false);
                            negativeContext.BeginFigure(new Point(right, open), false, false);
                            negativeContext.LineTo(new Point(center, open), true, false);
                        }
                        else
                        {
                            negativeContext.BeginFigure(new Point(center, high), true, false);
                            negativeContext.LineTo(new Point(center, close), true, false);
                            negativeContext.BeginFigure(new Point(center, low), true, false);
                            negativeContext.LineTo(new Point(center, open), true, false);
                            negativeContext.BeginFigure(new Point(left, close), true, true);
                            negativeContext.LineTo(new Point(left, open), true, false);
                            negativeContext.LineTo(new Point(right, open), true, false);
                            negativeContext.LineTo(new Point(right, close), true, false);
                        }
                    }
                }
                else
                {
                    //画竖线
                    if (item.UporDown)
                    {
                        //红色蜡烛线
                        positiveContext.BeginFigure(new Point(center, high), false, false);
                        positiveContext.LineTo(new Point(center, low), true, false);
                    }
                    else
                    {
                        //绿色蜡烛线
                        negativeContext.BeginFigure(new Point(center, high), false, false);
                        negativeContext.LineTo(new Point(center, low), true, false);
                    }
                }
                index++;
                guidlinesX.Add(center - strokeness / 2);
                guidlinesX.Add(center + strokeness / 2);
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



        public void Dispose()
        {
            
        }

        public override void ItemsSoureChangedSetData(List<CandleModel> newvalue)
        {
            FastSourceInstance.SetParameter(CommonData, p => p.High, p => p.Low);
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
        protected override void DisplayToolTip(int currentindex, double x, double y)
        {
            if (YValues.Length / 2 > currentindex)
            {
                if (YValues[currentindex, 0] <= y && YValues[currentindex, 1] >= y)
                {
                    IList list=FastSourceInstance.GetCachedCandleList();
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

