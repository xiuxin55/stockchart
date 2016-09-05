using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WpfApplication1
{
    public　class CrossLineManager
    {
        private static CrossLineManager _instance;
        public static CrossLineManager Instance
        {
            get
            {
                if(_instance ==null)
                {
                    _instance = new CrossLineManager();
                }
                return _instance;
            }
        }
        


        private List<ICrossLine> CrossLineList = new List<ICrossLine>();

        public void Regester(ICrossLine crossline)
        {
           if(!CrossLineList.Contains(crossline))
            {
                CrossLineList.Add(crossline);
            }
        }
        public void UnRegester(ICrossLine crossline)
        {
            if (CrossLineList.Contains(crossline))
            {
                CrossLineList.Remove(crossline);
            }
        }
        public void NotifyPublicKeyDown(KeyEventArgs e, ICrossLine crossline)
        {
            foreach (var item in CrossLineList)
            {
                if (item != crossline)
                {
                    item.PublicKeyDown(e,false);
                }
            }
        }
        //public void NotifyPublicKeyDown(KeyEventArgs e, ICrossLine crossline)
        //{
        //    foreach (var item in CrossLineList)
        //    {
        //        if (item != crossline)
        //        {
        //            item.PublicKeyDown(e);
        //        }
        //    }
        //}


        public void NotifyPublicMouseDoubleClick(MouseButtonEventArgs e, ICrossLine crossline)
        {
            foreach (var item in CrossLineList)
            {
                if (item != crossline)
                {
                    item.PublicMouseDoubleClick(e);
                }
            }
        }

        public void NotifyPublicMouseLeave(MouseEventArgs e, ICrossLine crossline)
        {
            foreach (var item in CrossLineList)
            {
                if (item != crossline)
                {
                    item.PublicMouseLeave(e);
                }
            }
        }

        public void NotifyPublicMouseMove(MouseEventArgs e, ICrossLine crossline)
        {
            foreach (var item in CrossLineList)
            {
                if (item != crossline)
                {
                    
                    item.PublicMouseMove(e);
                }
            }
        }


        public void NotifyPublicMouseMove2(MouseEventArgs e, ICrossLine crossline)
        {
            foreach (var item in CrossLineList)
            {
                if (item != crossline)
                {

                    item.PublicDragMouseMove(e);
                }
            }
        }

        public void NotifyPublicMouseWheel(MouseWheelEventArgs e, ICrossLine crossline)
        {
            foreach (var item in CrossLineList)
            {
                if (item != crossline)
                {
                    item.PublicMouseWheel(e);
                }
            }
        }
        public void NotifyPublicMouseLeftButtonDown(MouseButtonEventArgs e, ICrossLine crossline)
        {
            foreach (var item in CrossLineList)
            {
                if (item != crossline)
                {
                    item.PublicMouseLeftButtonDown(e);
                }
            }
        }
        public void NotifyPublicMouseLeftButtonUp(MouseButtonEventArgs e, ICrossLine crossline)
        {
            foreach (var item in CrossLineList)
            {
                if (item != crossline)
                {
                    item.PublicMouseLeftButtonUp(e);
                }
            }
        }
    }
}
