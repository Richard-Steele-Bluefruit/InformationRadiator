using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GitVisualiser.Model
{
    public class UniqueFractalPolygon : IUniqueFractalPolygon
    {
        public IPolygonWrapper Shape { get; private set; }
        public PointCollection DrawablePoints { get; private set; }

        private Color BrownOutlineColour = Color.FromRgb(139, 69, 19);
        private double _previousStrokeThickness;

        public UniqueFractalPolygon()
        {
            Shape = new PolygonWrapper();
            Highlight(false);
            var green = Color.FromRgb(34, 139, 34);
            Shape.Fill = new SolidColorBrush(green);
            Shape.HorizontalAlignment = HorizontalAlignment.Left;
            Shape.VerticalAlignment = VerticalAlignment.Top;

            DrawablePoints = GetDrawablePoints();
        }

        public void Highlight(bool enable)
        {
            if (enable)
            {
                _previousStrokeThickness = Shape.StrokeThickness;   // possible threading issue.....

                var yellow = Color.FromRgb(255, 255, 0);
                Shape.Stroke = new SolidColorBrush(yellow);
                var MinHighlightingLineThinkness = 3;
                if (Shape.StrokeThickness < MinHighlightingLineThinkness)
                {
                    Shape.StrokeThickness = MinHighlightingLineThinkness;
                }
            }
            else
            {
                Shape.Stroke = new SolidColorBrush(BrownOutlineColour);
                Shape.StrokeThickness = _previousStrokeThickness;
            }
        }

        public void SetPoints(PointCollection relocatedDrawablePoints)
        {
            Shape.Points = relocatedDrawablePoints;
        }

        public System.Windows.Point GetWidthAndHeight(PointCollection points)
        {
            var SomeNumberThatIsHigherThanTheMaximumXOrY = 99999.0; // hopefully nobody has a screen this big!
            var SomeNumberThatIsLowerThanTheMinimumXOrY = -SomeNumberThatIsHigherThanTheMaximumXOrY;
            var lowestX = SomeNumberThatIsHigherThanTheMaximumXOrY;
            var highestX = SomeNumberThatIsLowerThanTheMinimumXOrY;
            var lowestY = SomeNumberThatIsHigherThanTheMaximumXOrY;
            var highestY = SomeNumberThatIsLowerThanTheMinimumXOrY;
            foreach (var point in points)
            {
                var x = point.X;
                var y = point.Y;

                if (x < lowestX)
                {
                    lowestX = x;
                }
                if (x > highestX)
                {
                    highestX = x;
                }

                if (y < lowestY)
                {
                    lowestY = y;
                }
                if (y > highestY)
                {
                    highestY = y;
                }
            }

            var largestXDistance = highestX - lowestX;
            var largestYDistance = highestY - lowestY;

            double width = largestXDistance;
            double height = largestYDistance;
            System.Windows.Point widthAndHeight = new System.Windows.Point(width, height);
            return widthAndHeight;
        }

        public void SetOutlineWidth(double newOutlineWidth)
        {
            Shape.StrokeThickness = newOutlineWidth;
        }

        private PointCollection GetDrawablePoints()
        {
            PointCollection points = UniquePoints.Get();
            PointCollection centredPoints = GetCentredPoints(points);

            uint kochSizeModifier = 3;
            uint level = 4;
            var kochPoints = GetDeformedKochN(centredPoints, kochSizeModifier, level);

            kochPoints = GetCentredPoints(kochPoints);
            
            return kochPoints;
        }

        private PointCollection GetCentredPoints(PointCollection points)
        {
            var shapeCentre = GetCentrePoint(points);

            PointCollection centredPoints = new PointCollection();
            foreach (var point in points)
            {
                var x = point.X;
                var y = point.Y;

                x = x - shapeCentre.X;
                y = y - shapeCentre.Y;

                System.Windows.Point newPoint = new System.Windows.Point(x, y);
                centredPoints.Add(newPoint);
            }

            return centredPoints;
        }

        private System.Windows.Point GetCentrePoint(PointCollection points)
        {
            var SomeNumberThatIsHigherThanTheMaximumXOrY = 99999.0; // hopefully nobody has a screen this big!
            var SomeNumberThatIsLowerThanTheMinimumXOrY = -SomeNumberThatIsHigherThanTheMaximumXOrY;
            var lowestX = SomeNumberThatIsHigherThanTheMaximumXOrY;
            var highestX = SomeNumberThatIsLowerThanTheMinimumXOrY;
            var lowestY = SomeNumberThatIsHigherThanTheMaximumXOrY;
            var highestY = SomeNumberThatIsLowerThanTheMinimumXOrY;
            foreach (var point in points)
            {
                var x = point.X;
                var y = point.Y;

                if (x < lowestX)
                {
                    lowestX = x;
                }
                if (y < lowestY)
                {
                    lowestY = y;
                }
                if (x > highestX)
                {
                    highestX = x;
                }
                if (y > highestY)
                {
                    highestY = y;
                }
            }

            var largestXDistance = highestX - lowestX;
            var largestYDistance = highestY - lowestY;
            var xModifyDistance = largestXDistance / 2;
            var yModifyDistance = largestYDistance / 2;

            var centreX = lowestX + xModifyDistance;
            var centreY = lowestY + yModifyDistance;

            System.Windows.Point centrePoint = new System.Windows.Point(centreX, centreY);
            return centrePoint;
        }

        private PointCollection GetDeformedKochN(PointCollection inputPoints, uint sizeModifier, uint level)
        {
            PointCollection accumulatedPoints = inputPoints;
            for (int i = 0; i < level; i++)
            {
                var deformedKoch1Points = GetDeformedKoch1Shape(accumulatedPoints, sizeModifier);
                accumulatedPoints = deformedKoch1Points;
            }

            return accumulatedPoints;
        }

        private PointCollection GetDeformedKoch1Shape(PointCollection inputPoints, uint sizeModifier)
        {
            PointCollection kochPoints = new PointCollection();

            for (int i = 0; i < inputPoints.Count(); i++)
            {
                System.Windows.Point start = inputPoints[i];
                System.Windows.Point end;
                if (i + 1 == inputPoints.Count())
                {
                    end = inputPoints[0];
                }
                else
                {
                    end = inputPoints[i + 1];
                }

                var differenceX = end.X - start.X;
                var differenceY = end.Y - start.Y;
                var modifierXSpaceToKoch = differenceX / sizeModifier;
                var modifierYSpaceToKoch = differenceY / sizeModifier;

                var point1 = new System.Windows.Point(start.X + modifierXSpaceToKoch, start.Y + modifierYSpaceToKoch);
                var point2 = new System.Windows.Point(end.X - modifierXSpaceToKoch, end.Y - modifierYSpaceToKoch);
                var point1And2DifferenceX = point2.X - point1.X;
                var point1And2DifferenceY = point2.Y - point1.Y;
                var middlePoint = new System.Windows.Point(point1.X + point1And2DifferenceX, point1.Y + point1And2DifferenceY);
                var degreesOfKoch = 60;
                middlePoint = RotateAroundPointAntiClockwise(point1, middlePoint, degreesOfKoch);

                kochPoints.Add(start);
                kochPoints.Add(middlePoint);
                kochPoints.Add(point2);
            }

            return kochPoints;
        }

        private System.Windows.Point RotateAroundPointAntiClockwise(System.Windows.Point origin, System.Windows.Point pointToRotate, double degrees)
        {
            var radians = DegreesToRadians(degrees);
            var cos = Math.Cos(radians);
            var sin = Math.Sin(radians);

            var circleStartingX = pointToRotate.X - origin.X;
            var circleStartingY = pointToRotate.Y - origin.Y;
            var XRelativeToOrigin = (circleStartingX * cos) - (circleStartingY * sin);
            var YRelativeToOrigin = (circleStartingX * sin) + (circleStartingY * cos);
            var XRelativeToCentre = XRelativeToOrigin + origin.X;
            var YRelativeToCentre = YRelativeToOrigin + origin.Y;

            System.Windows.Point rotatedPoint = new System.Windows.Point(XRelativeToCentre, YRelativeToCentre);
            return rotatedPoint;
        }

        private double DegreesToRadians(double degrees)
        {
            return Math.PI * degrees / 180.0;
        }
    }
}