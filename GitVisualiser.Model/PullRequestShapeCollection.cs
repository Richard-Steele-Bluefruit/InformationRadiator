using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace GitVisualiser.Model
{
    public class PullRequestShapeCollection
    {
        private List<Tuple<Polygon, PointCollection>> _shapes;
        private readonly Color _identifyingColour;

        private const double ShapeSize = 7;

        public PullRequestShapeCollection(Color identifyingColour)
        {
            _identifyingColour = identifyingColour;
            _shapes = new List<Tuple<Polygon, PointCollection>>();

            CreateShape();
        }

        private void CreateShape()
        {
            Polygon newShape = new Polygon();
            newShape.Fill = new SolidColorBrush(_identifyingColour);
            newShape.StrokeThickness = 1;
            newShape.Stroke = System.Windows.Media.Brushes.Black;
            newShape.HorizontalAlignment = HorizontalAlignment.Left;
            newShape.VerticalAlignment = VerticalAlignment.Top;

            var oneUnitLength = 5.0;
            var one = oneUnitLength;
            var half = oneUnitLength / 2;
            // triangle
            System.Windows.Point point1 = new System.Windows.Point(0, -half);
            System.Windows.Point point2 = new System.Windows.Point(half, 0);
            System.Windows.Point point3 = new System.Windows.Point(-half, 0);

            PointCollection points = new PointCollection();
            points.Add(point1);
            points.Add(point2);
            points.Add(point3);

            // resize
            PointCollection drawablePointsAtOrigin = new PointCollection(); // origin, as in (0,0)
            var centreDotSize = ShapeSize / 2;
            foreach (var point in points)
            {
                var newPoint = new System.Windows.Point(point.X * centreDotSize, point.Y * centreDotSize);
                drawablePointsAtOrigin.Add(newPoint);
            }

            Tuple<Polygon, PointCollection> newPullRequestShape = new Tuple<Polygon, PointCollection>(newShape, drawablePointsAtOrigin);
            _shapes.Add(newPullRequestShape);
        }

        public void Move(System.Windows.Point newLocation)
        {
            PointCollection drawablePointsAtOrigin = _shapes[_shapes.Count - 1].Item2;

            PointCollection relocatedDrawablePoints = new PointCollection();
            foreach (var point in drawablePointsAtOrigin)
            {
                var x = point.X + newLocation.X;
                var y = point.Y + newLocation.Y;
                var newPoint = new System.Windows.Point(x, y);
                relocatedDrawablePoints.Add(newPoint);
            }

            _shapes[_shapes.Count - 1].Item1.Points = relocatedDrawablePoints;
        }

        public List<System.Windows.Shapes.Shape> Drawables()
        {
            List<System.Windows.Shapes.Shape> drawables = new List<System.Windows.Shapes.Shape>();
            foreach (var shape in _shapes)
            {
                drawables.Add(shape.Item1);
            }

            return drawables;
        }
    }
}