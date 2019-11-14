using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace GitVisualiser.Model
{
    public class IslandShape : IIslandShape
    {
        public IUniqueFractalPolygon MainIslandShape { get; private set; }
        public PullRequestShapeCollection PullRequestShapes { get; private set; }
        public System.Windows.Point CentrePoint { get; private set; }
        public double Width { get; private set; }
        public double Height { get; private set; }

        private const double CliffOutlineWidthModifier = 8;

        public IslandShape(Color identifyingColour)
        {
            MainIslandShape = IslandShapeFactory.Instance.CreateUniqueFractalPolygon();
            PullRequestShapes = new PullRequestShapeCollection(identifyingColour);
        }

        public void Highlight(bool enable)
        {
            MainIslandShape.Highlight(enable);
        }

        public void ResizeAndMove(double islandSize, System.Windows.Point islandLocation)
        {
            var resizedMainIslandShapePoints = GetResizedShapePoints(islandSize, MainIslandShape.DrawablePoints);
            SetLocation(islandLocation, resizedMainIslandShapePoints);
            var cliffOutlineWidth = islandSize / CliffOutlineWidthModifier;
            MainIslandShape.SetOutlineWidth(cliffOutlineWidth);
        }

        private PointCollection GetResizedShapePoints(double resizeAmount, PointCollection points)
        {
            PointCollection resizedPoints = new PointCollection();
            foreach (var point in points)
            {
                var newPoint = new System.Windows.Point(point.X * resizeAmount, point.Y * resizeAmount);
                resizedPoints.Add(newPoint);
            }

            return resizedPoints;
        }

        private void SetLocation(System.Windows.Point newLocation, PointCollection drawablePoints)
        {
            var relocatedDrawablePoints = GetRelocatedShapePoints(drawablePoints, newLocation);
            MainIslandShape.SetPoints(relocatedDrawablePoints);
            PullRequestShapes.Move(newLocation);

            var shapeCentre = new System.Windows.Point(newLocation.X, newLocation.Y);
            CentrePoint = shapeCentre;

            // we use the following for collision detection
            var widthAndHeight = MainIslandShape.GetWidthAndHeight(drawablePoints); // only accepts centred points
            var width = widthAndHeight.X;
            Width = width;
            var height = widthAndHeight.Y;
            Height = height;
        }

        private PointCollection GetRelocatedShapePoints(PointCollection pointsToRelocate, System.Windows.Point shapeLocation)
        {
            PointCollection reloactedPoints = new PointCollection();
            foreach (var point in pointsToRelocate)
            {
                var x = point.X + shapeLocation.X;
                var y = point.Y + shapeLocation.Y;
                var newPoint = new System.Windows.Point(x, y);
                reloactedPoints.Add(newPoint);
            }

            return reloactedPoints;
        }

        public List<System.Windows.Shapes.Shape> Drawables()
        {
            List<System.Windows.Shapes.Shape> drawables = new List<System.Windows.Shapes.Shape>();
            drawables.Add(MainIslandShape.Shape.GetWrappedPolygon());
            foreach (var pullRequestShape in PullRequestShapes.Drawables())
            {
                drawables.Add(pullRequestShape);
            }

            return drawables;
        }
    }
}