using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApplication1
{
    public abstract  class BaseContentControl<T>:ContentControl
    {
        protected DrawingCanvas _rootCanvas;
        public CommonProperty CommonData;
        public FastSource<T> FastSourceInstance;
        public event Action DrawImageEvent;
        public BaseContentControl()
        {
             _rootCanvas = new DrawingCanvas();
            this.Content = _rootCanvas;
            CommonData = new CommonProperty();
            FastSourceInstance = new FastSource<T>();
            FastSourceInstance.ItemsSourceChangeEvent += Instance_ItemsSourceChangeEvent;
            FastSourceInstance.NotifyToolTipEvent += FastSourceInstance_NotifyToolTipEvent;
        }


        private void FastSourceInstance_NotifyToolTipEvent(int index, double x, double y)
        {
            DisplayToolTip(index,x, y);
        }
        protected abstract void DisplayToolTip(int index, double x, double y);
        /// <summary>
        /// 是否为主十字线
        /// </summary>
        private bool isMainCrossLine;
        public bool IsMainCrossLine
        {
            get
            {
                return isMainCrossLine;
            }

            set
            {
                isMainCrossLine = value;
            }
        }


        private void Instance_ItemsSourceChangeEvent()
        {
            if (YaxisControl != null)
            {
                YaxisControl.SourceChanged(CommonData.YMaxValue, CommonData.YMinValue);
            }
            DrawingImage();
        }

        /// <summary>
        /// 绘制蜡烛图、x轴、y轴
        /// </summary>
        protected void DrawingImage()
        {
           
            Clear();
            //if (IsDrawYaxis)
            //{
            //    (new Yaxis(CommonData, FastSourceInstance)).ProduceYaxis(_rootCanvas);
            //}
            DrawingGraphics();
            if (IsDrawXaxis)
            {

                (new Xaxis(CommonData)).ProduceXaxis(_rootCanvas, DateStart, XaxisDisplayFormatter, XaxisInternal);
            }
        }
        /// <summary>
        /// 绘制图形
        /// </summary>
        protected abstract void DrawingGraphics();
        /// <summary>
        /// 绑定的数据源
        /// </summary>
        public List<T> ItemSource
        {
            get
            {
                return (List<T>)GetValue(ItemSourceProperty);
            }
            set
            {
                SetValue(ItemSourceProperty, value);
            }
        }
        /// <summary>
        /// Identifies the MinorStrokeDashArray dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemSourceProperty = DependencyProperty.Register("ItemSource", typeof(List<T>), typeof(BaseContentControl<T>),
            new PropertyMetadata(new PropertyChangedCallback(ItemsSoureChangedCallBack)));
        protected static void ItemsSoureChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseContentControl<T> cc = d as BaseContentControl<T>;
            cc.ItemsSoureChangedSetData((List<T>)e.NewValue);
           
        }
        public abstract void ItemsSoureChangedSetData(List<T> newvalue); 
        /// <summary>
        /// 未使用
        /// </summary>
        public int XaxisInternal
        {
            get
            {
                return (int)GetValue(XaxisInternalProperty);
            }
            set
            {
                SetValue(XaxisInternalProperty, value);
            }
        }
        /// <summary>
        /// Identifies the MinorStrokeDashArray dependency property.
        /// </summary>
        public static readonly DependencyProperty XaxisInternalProperty = DependencyProperty.Register("XaxisInternal", typeof(int), typeof(BaseContentControl<T>),
            new PropertyMetadata(1, null));


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
        public static readonly DependencyProperty XaxisDisplayFormatterProperty = DependencyProperty.Register("XaxisDisplayFormatter", typeof(string), typeof(BaseContentControl<T>),
            new PropertyMetadata("MM", null));

        /// <summary>
        /// x轴第一个刻度要显示的内容
        /// </summary>
        public string DateStart
        {
            get
            {
                return (string)GetValue(DateStartProperty);
            }
            set
            {
                SetValue(DateStartProperty, value);
            }
        }
        public static readonly DependencyProperty DateStartProperty = DependencyProperty.Register("DateStart", typeof(string), typeof(BaseContentControl<T>),
            new PropertyMetadata(DateTime.Now.ToString("yyyy年"), null));

        public bool IsDrawXaxis
        {
            get { return (bool)GetValue(IsDrawXaxisProperty); }
            set { SetValue(IsDrawXaxisProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsDrawXaxis.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsDrawXaxisProperty =
            DependencyProperty.Register("IsDrawXaxis", typeof(bool), typeof(BaseContentControl<T>), new PropertyMetadata(false));
        public bool IsDrawYaxis
        {
            get { return (bool)GetValue(IsDrawYaxisProperty); }
            set { SetValue(IsDrawYaxisProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsDrawXaxis.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsDrawYaxisProperty =
            DependencyProperty.Register("IsDrawYaxis", typeof(bool), typeof(BaseContentControl<T>), new PropertyMetadata(true));



        /// <summary>
        /// 是否为主十字线
        /// </summary>

       
        public CrossLine CrossLineControl
        {
            get { return (CrossLine)GetValue(CrossLineControlProperty); }
            set { SetValue(CrossLineControlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsDrawXaxis.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CrossLineControlProperty =
            DependencyProperty.Register("CrossLineControl", typeof(CrossLine), typeof(BaseContentControl<T>), new FrameworkPropertyMetadata(null,
                new PropertyChangedCallback((sender, e) =>
                {
                    BaseContentControl<T> bcc = sender as BaseContentControl<T>;
                    CrossLine cl = (CrossLine)e.NewValue;
                    cl.IsMainCrossLine = bcc.isMainCrossLine;
                    if(!cl.FastSourceInstances.Contains(bcc.FastSourceInstance))
                    {
                        cl.FastSourceInstances.Add(bcc.FastSourceInstance);
                    }
                    cl.CommonData = bcc.CommonData;
                    cl.IsDrawCrossXaxisLabel = bcc.IsDrawXaxis;
                    bcc.CrossLineControlCallback();
                })));


        public YaxisControl YaxisControl
        {
            get { return (YaxisControl)GetValue(YaxisControlProperty); }
            set { SetValue(YaxisControlProperty, value); }
        }
        /// <summary>
        /// y轴控件属性
        /// </summary>

        // Using a DependencyProperty as the backing store for IsDrawXaxis.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YaxisControlProperty =
            DependencyProperty.Register("YaxisControl", typeof(YaxisControl), typeof(BaseContentControl<T>), new FrameworkPropertyMetadata(null,
                new PropertyChangedCallback((sender, e) =>
                {
                    BaseContentControl<T> bcc = sender as BaseContentControl<T>;
                    YaxisControl yc = (YaxisControl)e.NewValue;
                    yc.MaxMinValueChanged += bcc.MaxMinValueChanged;
                    bcc.CommonData.YMaxValue  = yc.CommonData.YMaxValue;
                    bcc.CommonData.YMinValue = yc.CommonData.YMinValue;
                    //bcc.DrawingImage();
                })));

        public void MaxMinValueChanged(double max, double min)
        {
            CommonData.YMaxValue = max;
            CommonData.YMinValue = min;
        }

        public virtual void CrossLineControlCallback()
        {
            //继承注册十字线
        }

        protected double GetScaledY(double yvalue)
        {
            return CommonData.ViewPortHeight - yvalue * CommonData.Scale + CommonData.HeightOffet / 2;
        }
        protected double GetYValue(double mouseLocationY)
        {

            return Math.Round(CommonData.YMinValue + (CommonData.ViewPortHeight + CommonData.HeightOffet / 2 - mouseLocationY) / CommonData.Scale, 2);
        }

        public void SetCommonDataSize(Size size)
        {
            CommonData.ActualHeight = size.Height;
            CommonData.ActualWidth = size.Width;
            CommonData.ViewPortWidth = size.Width - CommonData.YaxisMargin;
            CommonData.ViewPortHeight = size.Height - CommonData.HeightOffet;
        }
        /// <summary>
        /// 清除数据
        /// </summary>
        protected virtual void Clear()
        {
            if (_rootCanvas == null || FastSourceInstance.GetCachedCandleList().Count == 0 || CommonData.ViewPortWidth == 0 || CommonData.ViewPortHeight == 0)
                return;
            _rootCanvas.ClearVisual();
        }


        protected void PlaceToolTip(object data)
        {
            if (this.ToolTip is System.Windows.Controls.ToolTip)
            {
                System.Windows.Controls.ToolTip tip = (System.Windows.Controls.ToolTip)this.ToolTip;
                if (tip != null)
                {
                    tip.IsOpen = false;
                    tip.DataContext = data;
                    tip.IsOpen = true;
                }
            }
        }
        protected void HideToolTip()
        {
            if (this.ToolTip is System.Windows.Controls.ToolTip)
            {
                System.Windows.Controls.ToolTip tip = (System.Windows.Controls.ToolTip)this.ToolTip;
                if (tip != null)
                {
                    tip.IsOpen = false;
                }
            }
        }
    }
}
