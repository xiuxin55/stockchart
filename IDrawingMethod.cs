using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace WpfApplication1
{
    public interface IDrawingMethod
    {
        void DrawingGraphics(Geometry positiveGroup, Geometry negativeGroup, bool IsDrawingLine, double CustomDrawLineWidth);
    }
}
