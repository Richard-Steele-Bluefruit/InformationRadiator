using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GitVisualiser.Model
{
    public interface IUniqueFractalPolygon
    {
        IPolygonWrapper Shape { get; }
        PointCollection DrawablePoints { get; }

        void Highlight(bool enable);
        void SetPoints(PointCollection relocatedDrawablePoints);
        System.Windows.Point GetWidthAndHeight(PointCollection points);
        void SetOutlineWidth(double newOutlineWidth);
    }
}