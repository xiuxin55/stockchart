using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApplication1
{
    public interface  ICrossLine
    {
          CommonProperty CommonData { get; set; }
          List<IFastSource> FastSourceInstances { get; set; }
          bool IsDrawCrossXaxisLabel { get; set; }

          void PublicKeyDown(KeyEventArgs e);
          void PublicKeyDown(KeyEventArgs e, bool IsDrawXLine);

          void PublicMouseDoubleClick(MouseButtonEventArgs e);
          void PublicMouseLeave(MouseEventArgs e);
          void PublicMouseMove(MouseEventArgs e);

          void PublicDragMouseMove(MouseEventArgs e);
          void PublicMouseWheel(MouseWheelEventArgs e);
          void PublicMouseLeftButtonDown(MouseButtonEventArgs e);
          void PublicMouseLeftButtonUp(MouseButtonEventArgs e);

    }
}