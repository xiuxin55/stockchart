using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WpfApplication1
{
    public partial class CandleModel
    {
        private double _open;
        /// <summary>
        /// 开盘
        /// </summary>
        public double Open
        {
            get { return _open; }
            set { _open = value; }
        }
        private double _close;
        /// <summary>
        /// 收盘
        /// </summary>
        public double Close
        {
            get { return _close; }
            set { _close = value; }
        }
        private double _high;
        /// <summary>
        /// 最高
        /// </summary>
        public double High
        {
            get { return _high; }
            set { _high = value; }
        }
        private double _low;
        /// <summary>
        /// 最低
        /// </summary>
        public double Low
        {
            get { return _low; }
            set { _low = value; }
        }

        private bool upordown;
        /// <summary>
        /// 涨跌
        /// </summary>
        public bool UporDown
        {
            get { return upordown; }
            set { upordown = value; }
        }

        private double _Volume;
        /// <summary>
        /// 成交量
        /// </summary>
        public double Volume
        {
            get
            {
                return _Volume;
            }

            set
            {
                _Volume = value;
            }
        }

        private DateTime _Date;
        /// <summary>
        /// 标签显示对应的实际的日期
        /// </summary>
        public DateTime Date
        {
            get { return _Date; }
            set { _Date = value; }
        }

    
    }

    public class XaxisModel
    {
         private DateTime xaxisLabel;
        /// <summary>
        /// 数据对应的x轴要显示的标签
        /// </summary>
         public DateTime XaxisLabel
        {
            get { return xaxisLabel; }
            set { xaxisLabel = value; }
        }

        private double _YLocation;
        public double YLocation
        {
            get
            {
                return _YLocation;
            }

            set
            {
                _YLocation = value;
            }
        }

        private double _YValue;
        public double YValue
        {
            get
            {
                return _YValue;
            }

            set
            {
                _YValue = value;
            }
        }
        private double _startLocation;

        public double StartLocation
        {
            get { return _startLocation; }
            set { _startLocation = value; }
        }

        private double _endLocation;

        public double EndLocation
        {
            get { return _endLocation; }
            set { _endLocation = value; }
        }

       
    }

    public class Line
    {
        public Line(Point StartLocation, Point EndLocation)
        {
            this.StartLocation = StartLocation;
            this.EndLocation = EndLocation;
        }
        public Point StartLocation { get; set; }
        public Point EndLocation { get; set; }
    }

    public class LineModel
    {
        private double _LineValue;
        /// <summary>
        /// 值
        /// </summary>
        public double LineValue
        {
            get
            {
                return _LineValue;
            }

            set
            {
                _LineValue = value;
            }
        }

        private DateTime _Date;
        /// <summary>
        /// 标签显示对应的实际的日期
        /// </summary>
        public DateTime Date
        {
            get { return _Date; }
            set { _Date = value; }
        }
    }
}
