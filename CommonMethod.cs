using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace WpfApplication1
{
    public class CommonMethod
    {
        /// <summary>
        /// y轴缩放
        /// </summary>
        /// <param name="scale">缩放比例</param>
        /// <param name="yvalue">y值</param>
        /// <returns></returns>
        //public static double GetScaledY(double yvalue)
        //{
        //    return CommonData.ViewPortHeight - yvalue * CommonData.Scale + CommonData.HeightOffet / 2;
        //}
        /// <summary>
        /// 根据y坐标获得值，与GetScaledY(double yvalue) 相反操作
        /// </summary>
        /// <param name="mouseLocationY"></param>
        /// <returns></returns>
        //public static double GetYValue(double mouseLocationY)
        //{
           
        //    return Math.Round( CommonData.YMinValue+ (CommonData.ViewPortHeight + CommonData.HeightOffet / 2 - mouseLocationY) / CommonData.Scale,2);
        //}
        public static event Action<bool> AddOrRemoveFromDataSourceEvent;

        [DllImport("user32.dll")]
        static extern void ClipCursor(IntPtr rect);
        [DllImport("user32.dll")]
        static extern bool ClipCursor(ref System.Drawing.Rectangle lpRect);

       public static void RestrictMouse(ref System.Drawing.Rectangle lpRect)
        {
            ClipCursor(ref lpRect);
        }
        public static void RestrictMouse(IntPtr rect)
        {
            ClipCursor(rect);
        }
    }
}
