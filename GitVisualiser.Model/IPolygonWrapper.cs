using System;
using System.Windows;
using System.Linq;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GitVisualiser.Model
{
    public interface IPolygonWrapper
    {
        PointCollection Points { get; set; }
        Brush Fill { get; set; }
        System.Windows.HorizontalAlignment HorizontalAlignment { get; set; }
        System.Windows.VerticalAlignment VerticalAlignment { get; set; }
        double StrokeThickness { get; set; }
        Brush Stroke { get; set; }

        Polygon GetWrappedPolygon();
    }
}
