using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication1
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:WpfApplication1"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:WpfApplication1;assembly=WpfApplication1"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误: 
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:ChartContainer/>
    ///
    /// </summary>
    public class ChartContainer : Panel
    {
        public ChartContainer()
        {
            this.Loaded += ChartContainer_Loaded;
        }
       

        private void ChartContainer_Loaded(object sender, RoutedEventArgs e)
        {
            if (YaxisControl != null)
            {
                this.Children.Add(YaxisControl);
            }
            foreach (var item in CandleControls)
            {
                this.Children.Add(item);
            }
            foreach (var item in VolumeControls)
            {
                this.Children.Add(item);
            }
            foreach (var item in LineControls)
            {
                this.Children.Add(item);
            }
            if (CrossLineControl != null)
            {
                this.Children.Add(CrossLineControl);
            }
        }
        public int HashCode
        {
            get
            {
                return this.GetHashCode();
            }
        }
        private YaxisControl _YaxisControl;
        public YaxisControl YaxisControl
        {
            get
            {
                return _YaxisControl;
            }
            set
            {
                _YaxisControl = value;
            }
        }
        private CrossLine _CrossLineControl ;
        public CrossLine CrossLineControl
        {
            get { return _CrossLineControl; }
            set { _CrossLineControl=value; }
        }


        

        private ObservableCollection<LineControl> _LineControls = new ObservableCollection<LineControl>();
        public ObservableCollection<LineControl> LineControls
        {
            get
            {
                return _LineControls;
            }
            set
            {
                _LineControls = value;
            }
        }
        private ObservableCollection<CandleControl> _CandleControls = new ObservableCollection<CandleControl>();
        public ObservableCollection<CandleControl> CandleControls
        {
            get
            {
                return _CandleControls;
            }
            set
            {
                _CandleControls = value;
            }
        }

        private ObservableCollection<VolumeControl> _VolumeControls = new ObservableCollection<VolumeControl>();
        public ObservableCollection<VolumeControl> VolumeControls
        {
            get
            {
                return _VolumeControls;
            }
            set
            {
                _VolumeControls = value;
            }
        }
        protected override Size MeasureOverride(Size availableSize)
        {
            if (YaxisControl != null)
            {
                YaxisControl.Measure(availableSize);
            }
            foreach (var item in CandleControls)
            {
                item.Measure(availableSize);
            }
            foreach (var item in VolumeControls)
            {
                item.Measure(availableSize);
            }
            foreach (var item in LineControls)
            {
                item.Measure(availableSize);
            }
            if (CrossLineControl != null)
            {
                CrossLineControl.Measure(availableSize);
            }
            
            return base.MeasureOverride(availableSize); 
        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (YaxisControl != null)
            {
                YaxisControl.Arrange(new Rect(finalSize));
            }
            foreach (var item in CandleControls)
            {
                item.Arrange(new Rect(finalSize));
            }
            foreach (var item in VolumeControls)
            {
                item.Arrange(new Rect(finalSize));
            }
            foreach (var item in LineControls)
            {
                item.Arrange(new Rect(finalSize));
            }
            if (CrossLineControl != null)
            {
                CrossLineControl.Arrange(new Rect(finalSize));
            }
            return base.ArrangeOverride(finalSize);
        }

 
    }


}
