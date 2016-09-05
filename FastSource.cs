using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections;

namespace WpfApplication1
{
     public class FastSource<T> : IFastSource
    {
        
        public FastSource()
        {
            //CommonMethod.AddOrRemoveFromDataSourceEvent += CommonMethod_AddOrRemoveFromDataSourceEvent; ; 
           
        }
        CommonProperty CommonData;
        Func<T, double> MaxSelector;
        Func<T, double> MinSelector;
        /// <summary>
        /// 初始参数设置
        /// </summary>
        /// <param name="CommonData">公共数据</param>
        /// <param name="maxSelector">获取最大值</param>
        /// <param name="minSelector">获取最小值</param>
        public void SetParameter(CommonProperty CommonData, Func<T, double> maxSelector, Func<T, double> minSelector)
        {
            this.CommonData = CommonData;
            this.MaxSelector = maxSelector;
            this.MinSelector = minSelector;
        }
        private List<T> CachedCandleList = new List<T>();
        private List<T> DisplayCandleList = new List<T>();

        private List<T> RightHideCandleList = new List<T>();
        private List<T> LeftHideCandleList = new List<T>();


        public void SetCachedCandleList(List<T> list)
         {
             CachedCandleList.Clear();
             CachedCandleList = list.ToList();
            RightHideCandleList.Clear();
            LeftHideCandleList.Clear();
            SetZoomCount();
            //SetYaxisMax();
            //SetYaxisMin();
        }


         public IList GetCachedCandleList()
         {
             return CachedCandleList;
         }
         public int GetDataCount()
         {
             return CachedCandleList.Count;
         }
      
        /// <summary>
        /// 设置最大值
        /// </summary>
        public double GetYaxisMax()
         {
            double max=CachedCandleList.Count>0 && MaxSelector!=null? CachedCandleList.Max<T>(MaxSelector) :0;
            return max;
         }
        /// <summary>
        /// 设置最小值
        /// </summary>
         public double GetYaxisMin()
         {
            double min= CachedCandleList.Count>0 && MinSelector != null ? CachedCandleList.Min<T>(MinSelector) :0;
            return min;
         }

        public void SetYaxisMin(double min)
        {
            CommonData.YMinValue = min;
        }

        public void  SetYaxisMax(double max)
        {
            CommonData.YMaxValue = max;
        }
        /// <summary>
        /// 根据数据量大小调整缩放量
        /// </summary>
        private void SetZoomCount()
        {
            CommonData.DataAddOrReduceCount= (int)(1 + GetDataCount() / 5);
            
        }
        /// <summary>
        /// 数据发生改变触发
        /// </summary>
        public event Action ItemsSourceChangeEvent;

        public void ItemsSourceChange()
        {
            if (ItemsSourceChangeEvent != null)
            {
                ItemsSourceChangeEvent();
            }
        }

        /// <summary>
        /// 触发提示
        /// </summary>
        public event Action<int,double,double> NotifyToolTipEvent;
        public void NotifyToolTip(int index, double x, double y)
        {
            if (NotifyToolTipEvent != null)
            {
                NotifyToolTipEvent(index,x,y);
            }
        }

        /// <summary>
        /// 拖拽放大
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void DragZoomIn(int start,int end)
        {
           
            if(ItemsSourceChangeEvent!=null)
            {

                if(CachedCandleList.Count<=end)
                {
                    end = CachedCandleList.Count-1;
                }
                if (end -start  < 5)
                {
                    return;
                }
                int endremovecout = CachedCandleList.Count - end-1;
                LeftHideCandleList.AddRange(CachedCandleList.GetRange(0, start));
                CachedCandleList.RemoveRange(0, start);
                end = end-start + 1;
                RightHideCandleList.InsertRange(0,CachedCandleList.GetRange(end, endremovecout));
                CachedCandleList.RemoveRange(end, endremovecout);
                
            }
        }

        /// <summary>
        /// 滚轮放大
        /// </summary>
        public void MouseWheelZoomIn()
        {
            SetZoomCount();
            if (CachedCandleList.Count>=6)
            {
                if(CachedCandleList.Count>= CommonData.DataAddOrReduceCount+6)
                {
                    LeftHideCandleList.AddRange(CachedCandleList.GetRange(0, CommonData.DataAddOrReduceCount));
                    CachedCandleList.RemoveRange(0, CommonData.DataAddOrReduceCount);
                }
                else
                {
                    LeftHideCandleList.AddRange(CachedCandleList.GetRange(0, CachedCandleList.Count-6));
                    CachedCandleList.RemoveRange(0, CachedCandleList.Count - 6);
                }
            }
        }
        /// <summary>
        /// 滚轮缩小
        /// </summary>
        public void MouseWheelZoomOut()
        {
            SetZoomCount();
            if (ItemsSourceChangeEvent == null) { return; }
            if (RightHideCandleList.Count > 0)
            {
                if (RightHideCandleList.Count >= CommonData.DataAddOrReduceCount)
                {
                    CachedCandleList.AddRange(RightHideCandleList.GetRange(0, CommonData.DataAddOrReduceCount));
                    RightHideCandleList.RemoveRange(0, CommonData.DataAddOrReduceCount);

                }
                else
                {
                    CachedCandleList.AddRange(RightHideCandleList);
                    RightHideCandleList.Clear();
                }
            }
            else if(LeftHideCandleList.Count>0)
            {
                if (LeftHideCandleList.Count >= CommonData.DataAddOrReduceCount)
                {
                    CachedCandleList.InsertRange(0, LeftHideCandleList.GetRange(LeftHideCandleList.Count- CommonData.DataAddOrReduceCount, CommonData.DataAddOrReduceCount));
                    LeftHideCandleList.RemoveRange(LeftHideCandleList.Count - CommonData.DataAddOrReduceCount , CommonData.DataAddOrReduceCount);

                }
                else
                {
                    CachedCandleList.InsertRange(0,LeftHideCandleList);
                    LeftHideCandleList.Clear();
                }
            }
        }

        public bool KeyDownRight()
        {
            if (ItemsSourceChangeEvent == null) { return false; }

            if (RightHideCandleList.Count >= 1)
            {
                CachedCandleList.Add(RightHideCandleList[0]);
                RightHideCandleList.RemoveAt(0);
                LeftHideCandleList.Add(CachedCandleList[0]);
                CachedCandleList.RemoveAt(0);
                //SetYaxisMax();
                //SetYaxisMin();
                //ItemsSourceChangeEvent();
                return true;
            }
            return false;
        }

        public bool KeyDownLeft()
        {
            if (ItemsSourceChangeEvent == null) { return false; }
            if (LeftHideCandleList.Count >= 1)
            {
                CachedCandleList.Insert(0, LeftHideCandleList.Last<T>());
                LeftHideCandleList.RemoveAt(LeftHideCandleList.Count - 1);
                RightHideCandleList.Insert(0, CachedCandleList.Last<T>());
                CachedCandleList.RemoveAt(CachedCandleList.Count - 1);
                //SetYaxisMax();
                //SetYaxisMin();
                //ItemsSourceChangeEvent();
                return true;
            }
            return false;
        }

    }
}
