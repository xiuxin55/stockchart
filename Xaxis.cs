using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WpfApplication1
{
    /// <summary>
    /// x轴
    /// </summary>
    public class Xaxis
    {
        CommonProperty CommonData;
        public Xaxis(CommonProperty CommonData)
        {
            this.CommonData = CommonData;
        }

        //记录x轴上一个刻度
        private  string previewLabel = "";
        public  void ProduceXaxis(DrawingCanvas _rootCanvas, string DateStart, string XaxisDisplayFormatter, int XaxisInternal)
        {
            List<Point> ActualXaxisLabelLocations = new List<Point>();
            List<string> ActualDrawXaxisLabelList = new List<string>();
            if (CommonData.XaxisLabelList == null || CommonData.XaxisLabelList.Count == 0) { return; }
            List<XaxisModel> LabelList = CommonData.XaxisLabelList;
            StreamGeometry axispathStream = new StreamGeometry();
         
            StreamGeometryContext axisContext = axispathStream.Open();

            for (int i = 0; i < LabelList.Count; i += XaxisInternal)
            {
                Point p = new Point(LabelList[i].StartLocation, CommonData.ActualHeight - 20);

                if (i == 0)
                {
                    //第一个刻度根据用户给定的数据显示
                    ActualXaxisLabelLocations.Add(p);
                    previewLabel = CommonData.XaxisLabelList[i].XaxisLabel.ToString(XaxisDisplayFormatter);
                    if(previewLabel.Length==2 && previewLabel[0]=='0')
                    {
                        //如果是刻度是月，会出现08、09等两位数，将其去掉0
                        previewLabel = previewLabel[1].ToString();
                    }
                    ActualDrawXaxisLabelList.Add(DateStart);

                    axisContext.BeginFigure(p, false, false);
                    axisContext.LineTo(new Point(p.X, p.Y + 20), true, false);

                }
                else
                {
                    //按照指定格式转换刻度值
                    string currentLabel = CommonData.XaxisLabelList[i].XaxisLabel.ToString(XaxisDisplayFormatter);
                    if (currentLabel.Length == 2 && currentLabel[0] == '0')
                    {
                        //如果是刻度是月，会出现08、09等两位数，将其去掉0
                        currentLabel = currentLabel[1].ToString();
                    }
                    if (currentLabel != previewLabel)
                    {
                        //如果当前刻度和前一个刻度相等则不画该刻度
                        previewLabel = currentLabel;
                        if (p.X > 60)
                        {
                            //当刻度的x坐标小于60，不画刻度竖线，防止盖住第一个刻度
                            if (CommonData.ViewPortWidth - p.X > 60 && p.X - ActualXaxisLabelLocations[ActualXaxisLabelLocations.Count - 1].X > 60 )
                            {
                                int labecount = ActualDrawXaxisLabelList.Count;
                                if(labecount < 3||(labecount >= 3 
                                    && ActualDrawXaxisLabelList[labecount - 1] != currentLabel 
                                    && ActualDrawXaxisLabelList[labecount - 2] != currentLabel))
                                {
                                    //当刻度框的宽度小于40时，不绘制当前刻度
                                    ActualXaxisLabelLocations.Add(p);
                                    ActualDrawXaxisLabelList.Add(currentLabel);
                                    axisContext.BeginFigure(p, false, false);
                                    axisContext.LineTo(new Point(p.X, p.Y + 20), true, false);
                                }
                                
                            }
                        }
                    }
                }
            }
            if (ActualXaxisLabelLocations.Count > 0)
            {
                //最后绘制两条横线和最后一个竖线
                axisContext.BeginFigure(new Point(0, CommonData.ActualHeight - 20), false, false);
                axisContext.LineTo(new Point(CommonData.ViewPortWidth, CommonData.ActualHeight - 20), true, false);

                axisContext.BeginFigure(new Point(0, CommonData.ActualHeight), false, false);
                axisContext.LineTo(new Point(CommonData.ViewPortWidth, CommonData.ActualHeight), true, false);

                axisContext.BeginFigure(new Point(CommonData.ViewPortWidth, CommonData.ActualHeight - 20), false, false);
                axisContext.LineTo(new Point(CommonData.ViewPortWidth, CommonData.ActualHeight), true, false);

            }
            axisContext.Close();
           
            _rootCanvas.DrawingXaxis(axispathStream, 1, ActualXaxisLabelLocations, ActualDrawXaxisLabelList);
        }
    }
}
