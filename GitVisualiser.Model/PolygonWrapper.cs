using System;
using System.Linq;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GitVisualiser.Model
{
    public class PolygonWrapper : IPolygonWrapper
    {
        public PointCollection Points 
        {
            get { return _polygon.Points; }
            set { _polygon.Points = value; } 
        }

        public Brush Fill 
        {
            get { return _polygon.Fill; }
            set { _polygon.Fill = value; }
        }

        public System.Windows.HorizontalAlignment HorizontalAlignment
        {
            get { return _polygon.HorizontalAlignment; }
            set { _polygon.HorizontalAlignment = value; }
        }

        public System.Windows.VerticalAlignment VerticalAlignment
        {
            get { return _polygon.VerticalAlignment; }
            set { _polygon.VerticalAlignment = value; }
        }

        public double StrokeThickness
        {
            get { return _polygon.StrokeThickness; }
            set { _polygon.StrokeThickness = value; }
        }

        public Brush Stroke
        {
            get { return _polygon.Stroke; }
            set { _polygon.Stroke = value; }
        }
        
        private Polygon _polygon;

        public PolygonWrapper()
        {
            _polygon = new Polygon();
        }

        public Polygon GetWrappedPolygon()
        {
            return _polygon;
        }
    }
}
