using System.Collections;
using System.Collections.Generic;

namespace WpfApplication1
{
    public interface IFastSource
    {
        void DragZoomIn(int start, int end);
        void MouseWheelZoomIn();
        void MouseWheelZoomOut();

        bool KeyDownRight();
        bool KeyDownLeft();

        double GetYaxisMin();
        double GetYaxisMax();

        void SetYaxisMin(double min);
        void SetYaxisMax(double max);
        IList GetCachedCandleList();
        void ItemsSourceChange();
        void NotifyToolTip(int index, double x, double y);
    }
}