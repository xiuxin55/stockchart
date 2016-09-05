using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Interop;
using System.Runtime.InteropServices;

using System.Windows.Markup;

namespace WpfApplication1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        VModel vm = new VModel();
        List<CandleModel> temp = new List<CandleModel>();
        List<LineModel> tempLine = new List<LineModel>();
        List<CandleModel> Removetemp = new List<CandleModel>();
        DateTime dt =DateTime.Parse( DateTime.Now.ToShortDateString());
        string timestart;
        string timeend;
        public MainWindow()
        {
            InitializeComponent();
            //CommonMethod.AddOrRemoveFromDataSourceEvent += CommonMethod_AddOrRemoveFromDataSourceEvent;
            vm.MyList = new List<CandleModel>();
            vm.MyLineList = new List<LineModel>();
            vm.Interval = 1;
            vm.Display = "Name";
            vm.XaxisStart=dt.ToShortDateString();




            temp.Add(new CandleModel() { High = 10, Low = 8, Open = 9, Close = 8, UporDown = false ,Date=dt,Volume=10});
            tempLine.Add(new LineModel() { LineValue = 10, Date = dt });
            dt = dt.AddDays(1);
            temp.Add(new CandleModel() { High = 11.55, Low = 8, Open = 8, Close = 10, UporDown = true, Date = dt, Volume = 12 });
            tempLine.Add(new LineModel() { LineValue = 20, Date = dt });
            dt = dt.AddDays(1);
            temp.Add(new CandleModel() { High = 11.44, Low = 8, Open = 11, Close = 9, UporDown = false, Date = dt, Volume = 12 });
            tempLine.Add(new LineModel() { LineValue = 25, Date = dt });
            dt = dt.AddDays(1);
            temp.Add(new CandleModel() { High = 11, Low = 7, Open = 11, Close = 7, UporDown = false, Date = dt, Volume = 10 });
            tempLine.Add(new LineModel() { LineValue = 18, Date = dt });
            dt = dt.AddDays(1);
            temp.Add(new CandleModel() { High = 6, Low = 5, Open = 5, Close = 6, UporDown = true, Date = dt, Volume = 8 });
            tempLine.Add(new LineModel() { LineValue = 19, Date = dt });
            dt = dt.AddDays(1);
            temp.Add(new CandleModel() { High = 11, Low = 7, Open = 9, Close = 10, UporDown = true, Date = dt, Volume = 7 });
            tempLine.Add(new LineModel() { LineValue = 16, Date = dt });
            dt = dt.AddDays(1);
            temp.Add(new CandleModel() { High = 11, Low = 7, Open = 11, Close = 10, UporDown = false, Date = dt, Volume = 9 });
            tempLine.Add(new LineModel() { LineValue = 14, Date = dt });

          

            vm.XaxisEnd = dt.ToShortDateString();
            dt = dt.AddDays(1);
            vm.MyList = temp.ToList();
            vm.MyLineList=tempLine.ToList();
            SetMaxMin();




            this.DataContext = vm;

        }
        private void SetMaxMin()
        {
            double max, min;
            max = temp.Max(c => c.Volume);
            double tempmax = temp.Max(c => c.High);
            if (max < tempmax)
                max = tempmax;
            tempmax = tempLine.Max(c => c.LineValue);
            if (max < tempmax)
                max = tempmax;
            min = temp.Min(c => c.Volume);
            double tempmin = temp.Min(c => c.Low);
            if (min > tempmin)
                min = tempmin;
            tempmin = tempLine.Min(c => c.LineValue);
            if (min > tempmin)
                min = tempmin;
            vm.Ymax = max;
            vm.Ymin = min;

        }
        private void CommonMethod_AddOrRemoveFromDataSourceEvent(bool obj)
        {

            if(obj)
            {
                if (temp.Count == 0) { return; }
                Removetemp.Add(temp[0]);
                temp.RemoveAt(0);
                CandleModel cm = new CandleModel();
                Random r = new Random();
                cm.High = r.Next(10, 10);
                cm.Low = r.Next(10, 10);
                cm.Open = r.Next((int)cm.Low, (int)cm.High);
                cm.Close = r.Next((int)cm.Low, (int)cm.High);
                cm.UporDown = cm.Open < cm.Close;
                cm.Date = dt;
                dt = dt.AddDays(1);
                temp.Add(cm);
                vm.MyList = temp.ToList();
                SetMaxMin();
            }
            else
            {

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 100; i++)
            {
                CandleModel cm = new CandleModel();
                Random r = new Random();
                cm.High = r.Next(10, 12);
                cm.Low = r.Next(7, 10);
                cm.Open = r.Next((int)cm.Low, (int)cm.High);
                cm.Close = r.Next((int)cm.Low, (int)cm.High);
                cm.UporDown = cm.Open < cm.Close;
                cm.Date = dt;

                cm.Volume = (new Random()).Next(10, 40);

                LineModel lm = new LineModel();
                lm.Date = dt;
                lm.LineValue = (new Random()).Next(10, 40);
                tempLine.Add(lm);

               

                dt = dt.AddDays(1);
                temp.Add(cm);
             
            }
            SetMaxMin();
            vm.XaxisEnd = dt.ToShortDateString();
 
             vm.MyList = temp.ToList();
            vm.MyLineList = tempLine.ToList();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (vm.MyList.Count > 0)
            {
                //vm.MyList.RemoveAt(vm.MyList.Count - 1);
                if (vm.MyList.Count > 200)
                {
                    temp.RemoveRange(vm.MyList.Count - 200,200);
                }
                else
                {
                    temp.RemoveAt(vm.MyList.Count - 1);
                }
                vm.MyList = temp.ToList();
            }
        }

        private void OnMainLoad(object sender, RoutedEventArgs e)
        {
            int nStyle = Win32API.GetWindowLong(new WindowInteropHelper(this).Handle, Win32API.GWL_STYLE);

            nStyle &= ~Win32API.WS_CAPTION;

            Win32API.SetWindowLong(new WindowInteropHelper(this).Handle, Win32API.GWL_STYLE, nStyle);
        }
    }
    public class Win32API

    {
        public const int GWL_STYLE = -16;

        public const int GWL_EXSTYLE = -20;

        public const int WS_CAPTION = 0x00C00000;

        [DllImport("user32.dll")]

        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int New);



        [DllImport("user32.dll")]

        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    }
    public class VModel : INotifyPropertyChanged
    {
        
        private string display;

        public string Display
        {
            get { return display; }
            set
            {
                display = value;
                Notify("Display");
            }
        }

        private int interval;

        public int Interval
        {
            get { return interval; }
            set
            {
                interval = value;
                Notify("Interval");
            }
        }

        private string _xaxisStart;

        public string XaxisStart
        {
            get { return _xaxisStart; }
            set { _xaxisStart = value;
            Notify("XaxisStart");
            }
        }

        private string _xaxisEnd;

        public string XaxisEnd
        {
            get { return _xaxisEnd; }
            set { _xaxisEnd = value;
            Notify("XaxisEnd");
            }
        }

        private List<CandleModel> myList;
       
        public event PropertyChangedEventHandler PropertyChanged;
        public void Notify(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }


        public List<CandleModel> MyList
        {
            get { return myList; }
            set
            {
                myList = value;
                Notify("MyList");
            }
        }

        private List<LineModel> _MyLineList;
        public List<LineModel> MyLineList
        {
            get { return _MyLineList; }
            set
            {
                _MyLineList = value;
                Notify("MyLineList");
            }
        }

        private double ymax;
        public double Ymax
        {
            get { return ymax; }
            set
            {
                ymax = value;
                Notify("Ymax");
            }
        }

        private double ymin;
        public double Ymin
        {
            get { return ymin; }
            set
            {
                ymin = value;
                Notify("Ymin");
            }
        }
    }
    public class Model
    {
        public string Name { get; set; }
        public string Number { get; set; }
    }
    
}
