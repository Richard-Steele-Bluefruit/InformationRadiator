using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Windows.Media;
using System.Windows.Shapes;

using GitVisualiser.Model;

namespace GitVisualiser.Model.Tests
{
    [TestClass]
    public class UniqueFractalPolygonTests
    {
        #region Helper methods

        // stolen from UniqueFractalPolygon.cs
        public System.Windows.Point TestHelper_GetCentrePoint(PointCollection points)
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

        #endregion

        [TestMethod]
        public void One_shape_has_a_different_set_of_points_to_another()
        {
            // When
            UniqueFractalPolygon polygonOne = new UniqueFractalPolygon();
            UniqueFractalPolygon polygonTwo = new UniqueFractalPolygon();

            // Then
            Assert.AreNotEqual(polygonOne.DrawablePoints, polygonTwo.DrawablePoints);
        }

        [TestMethod]
        public void Able_to_get_the_centre_of_a_complex_polygon()
        {
            // Given
            UniqueFractalPolygon polygon = new UniqueFractalPolygon();

            // When
            PointCollection points = polygon.DrawablePoints;
            // modify the fractal shape so that we know what the centre point will be
            points.Add(new System.Windows.Point(-50, -150));
            points.Add(new System.Windows.Point(100, 450));

            System.Windows.Point centrePoint = TestHelper_GetCentrePoint(points);

            // Then
            var expectedX = 25.0;
            Assert.AreEqual(expectedX, centrePoint.X);

            var expectedY = 150.0;
            Assert.AreEqual(expectedY, centrePoint.Y);
        }

        [TestMethod]
        public void On_creation_the_drawable_points_are_centred_on_zero_zero()
        {
            // Given
            UniqueFractalPolygon polygon = new UniqueFractalPolygon();

            // When
            System.Windows.Point centrePoint = TestHelper_GetCentrePoint(polygon.DrawablePoints);

            // Then
            var expectedX = 0.0;
            Assert.AreEqual(expectedX, centrePoint.X);

            var expectedY = 0.0;
            Assert.AreEqual(expectedY, centrePoint.Y);
        }

        [TestMethod]
        public void Able_to_get_the_width_and_height_of_a_square()
        {
            // Given
            UniqueFractalPolygon polygon = new UniqueFractalPolygon();

            // When
            PointCollection points = new PointCollection();
            points.Add(new System.Windows.Point(0, 0));
            points.Add(new System.Windows.Point(0, 1));
            points.Add(new System.Windows.Point(1, 1));
            points.Add(new System.Windows.Point(1, 0));
            System.Windows.Point centrePoint = polygon.GetWidthAndHeight(points);

            // Then
            var expectedX = 1.0;
            Assert.AreEqual(expectedX, centrePoint.X);

            var expectedY = 1.0;
            Assert.AreEqual(expectedY, centrePoint.Y);
        }

        [TestMethod]
        public void Able_to_get_the_width_and_height_of_a_complex_polygon()
        {
            // Given
            UniqueFractalPolygon polygon = new UniqueFractalPolygon();

            PointCollection points = polygon.DrawablePoints;
            // modify the fractal shape so that we know what the centre point will be
            points.Add(new System.Windows.Point(-20, 50));
            points.Add(new System.Windows.Point(220, -70));

            // When
            System.Windows.Point widthAndHeight = polygon.GetWidthAndHeight(points);

            // Then
            var expectedWidth = 240.0;
            var actualWidth = widthAndHeight.X;
            Assert.AreEqual(expectedWidth, actualWidth);

            var expectedHeight = 120.0;
            var actualHeight = widthAndHeight.Y;
            Assert.AreEqual(expectedHeight, actualHeight);
        }

        [TestMethod]
        public void Able_to_get_the_width_and_height_of_that_is_centred_on_a_negative_location_and_not_origin()
        {
            // Given
            UniqueFractalPolygon polygon = new UniqueFractalPolygon();

            // centred on (-10, -10)
            PointCollection pointsNegativeOffsetFromCentre = new PointCollection();
            pointsNegativeOffsetFromCentre.Add(new System.Windows.Point(-11, -9));
            pointsNegativeOffsetFromCentre.Add(new System.Windows.Point(-9, -9));
            pointsNegativeOffsetFromCentre.Add(new System.Windows.Point(-9, -11));
            pointsNegativeOffsetFromCentre.Add(new System.Windows.Point(-11, -11));

            // When

            System.Windows.Point widthAndHeight = polygon.GetWidthAndHeight(pointsNegativeOffsetFromCentre);

            // Then
            var expectedWidth = 2.0;
            var actualWidth = widthAndHeight.X;
            Assert.AreEqual(expectedWidth, actualWidth);

            var expectedHeight = 2.0;
            var actualHeight = widthAndHeight.Y;
            Assert.AreEqual(expectedHeight, actualHeight);
        }

        [TestMethod]
        public void Able_to_get_the_width_and_height_of_that_is_centred_on_a_positive_location_and_not_origin()
        {
            // Given
            UniqueFractalPolygon polygon = new UniqueFractalPolygon();

            // centred on (+10, +10)
            PointCollection pointsPositiveOffsetFromCentre = new PointCollection();
            pointsPositiveOffsetFromCentre.Add(new System.Windows.Point(11, 9));
            pointsPositiveOffsetFromCentre.Add(new System.Windows.Point(9, 9));
            pointsPositiveOffsetFromCentre.Add(new System.Windows.Point(9, 11));
            pointsPositiveOffsetFromCentre.Add(new System.Windows.Point(11, 11));

            // When

            System.Windows.Point widthAndHeight = polygon.GetWidthAndHeight(pointsPositiveOffsetFromCentre);

            // Then
            var expectedWidth = 2.0;
            var actualWidth = widthAndHeight.X;
            Assert.AreEqual(expectedWidth, actualWidth);

            var expectedHeight = 2.0;
            var actualHeight = widthAndHeight.Y;
            Assert.AreEqual(expectedHeight, actualHeight);
        }

        [TestMethod]
        public void Highlighting_changes_the_ouline_colour_and_the_stroke_thickness()
        {
            // Given
            UniqueFractalPolygon polygon = new UniqueFractalPolygon();

            var originalOutlineThickness = polygon.Shape.StrokeThickness;
            var originalOutlineColour = polygon.Shape.Stroke.ToString();

            // When
            polygon.Highlight(true);

            // Then
            var highlightedStrokeThickness = polygon.Shape.StrokeThickness;
            Assert.AreNotEqual(originalOutlineThickness, highlightedStrokeThickness);

            var highlightedOutlineColour = polygon.Shape.Stroke.ToString();
            Assert.AreNotEqual(originalOutlineColour, highlightedOutlineColour);
        }

        [TestMethod]
        public void Unhighlighting_after_highlighting_changes_the_ouline_colour_and_the_stroke_thickness_back()
        {
            // Given
            UniqueFractalPolygon polygon = new UniqueFractalPolygon();

            var originalOutlineThickness = polygon.Shape.StrokeThickness;
            var originalOutlineColour = polygon.Shape.Stroke.ToString();

            // When
            polygon.Highlight(true);
            polygon.Highlight(false);

            // Then
            var revertedStrokeThickness = polygon.Shape.StrokeThickness;
            Assert.AreEqual(originalOutlineThickness, revertedStrokeThickness);

            var revertedOutlineColour = polygon.Shape.Stroke.ToString();
            Assert.AreEqual(originalOutlineColour, revertedOutlineColour);
        }
    }
}
