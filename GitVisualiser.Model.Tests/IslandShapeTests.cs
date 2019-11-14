using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Windows.Media;

using GitVisualiser.Model;

namespace GitVisualiser.Model.Tests
{
    [TestClass]
    public class IslandShapeTests
    {
        IslandShapeFactoryMock _factory;
        Mock<GitVisualiser.Model.IUniqueFractalPolygon> _mockUniqueFractalPolygon;
        Mock<GitVisualiser.Model.IPolygonWrapper> _mockPolygon;
        PointCollection _mockPolygonPoints;
        PointCollection _mockUniqueFractalPolygonPoints;

        #region Helper Methods

        [TestCleanup]
        public void CleanUp()
        {
            IslandShapeFactoryMock.Instance = null;
        }

        private void CreateMockFactory()
        {
            _factory = new IslandShapeFactoryMock();
            IslandShapeFactoryMock.Instance = _factory;

            _mockUniqueFractalPolygon = new Mock<GitVisualiser.Model.IUniqueFractalPolygon>(MockBehavior.Strict);
            _factory._uniqueFractalPolygon = _mockUniqueFractalPolygon.Object;

            _mockPolygon = new Mock<GitVisualiser.Model.IPolygonWrapper>(MockBehavior.Strict);
            _mockPolygonPoints = new PointCollection();
            _mockPolygon.Setup(m => m.Points).Returns(_mockPolygonPoints);

            _mockUniqueFractalPolygon.Setup(m => m.Shape).Returns(_mockPolygon.Object);
            _mockUniqueFractalPolygonPoints = new PointCollection();
            _mockUniqueFractalPolygon.Setup(m => m.DrawablePoints).Returns(_mockUniqueFractalPolygonPoints);

            _mockUniqueFractalPolygon.Setup(m => m.SetPoints(It.IsAny<PointCollection>())).Callback(
                (PointCollection newPoints) => _mockPolygonPoints = newPoints
            );
            _mockUniqueFractalPolygon.Setup(m => m.GetWidthAndHeight(It.IsAny<PointCollection>())).Returns(
                (PointCollection pointsParameter) => TestHelper_GetWidthAndHeight(pointsParameter)
            );
        }

        // stolen from UniqueFractalPolygon.cs
        private System.Windows.Point TestHelper_GetCentrePoint(PointCollection points)
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

        // stolen from UniqueFractalPolygon.cs
        private System.Windows.Point TestHelper_GetWidthAndHeight(PointCollection points)
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

        #endregion
        
        [TestMethod]
        public void Resizing_for_the_first_time_resizes_the_shape_by_the_given_size()
        {
            // Given
            CreateMockFactory();
            _mockUniqueFractalPolygon.Setup(m => m.SetOutlineWidth(It.IsAny<double>()));

            // centred square
            _mockUniqueFractalPolygonPoints.Add(new System.Windows.Point(-1, 1));
            _mockUniqueFractalPolygonPoints.Add(new System.Windows.Point(1, 1));
            _mockUniqueFractalPolygonPoints.Add(new System.Windows.Point(1, -1));
            _mockUniqueFractalPolygonPoints.Add(new System.Windows.Point(-1, -1));

            Color unused = Color.FromRgb(1, 2, 3);
            var islandShape = new IslandShape(unused);

            // When
            var widthBeforeResizeAndMove = 2;
            var heightBeforeResizeAndMove = 2;
            var pointsBeforeResizeAndMove = new PointCollection(_mockPolygonPoints);

            var islandSize = 10.0;
            var origin = new System.Windows.Point(0, 0);
            islandShape.ResizeAndMove(islandSize, origin);

            // Then
            var expectedWidth = widthBeforeResizeAndMove * islandSize;
            var actualWidth = TestHelper_GetWidthAndHeight(_mockPolygonPoints).X;
            Assert.AreEqual(expectedWidth, actualWidth);

            var expectedHeight = heightBeforeResizeAndMove * islandSize;
            var actualHeight = TestHelper_GetWidthAndHeight(_mockPolygonPoints).Y;
            Assert.AreEqual(expectedHeight, actualHeight);

            var expectedPoints = pointsBeforeResizeAndMove;
            var actualPoints = _mockPolygonPoints;
            Assert.AreNotEqual(expectedPoints, actualPoints);
        }

        [TestMethod]
        public void Resizing_and_moving_for_the_first_time_in_a_positive_location_relocates_the_shape_by_the_given_size()
        {
            // Given
            CreateMockFactory();
            _mockUniqueFractalPolygon.Setup(m => m.SetOutlineWidth(It.IsAny<double>()));

            // centred square
            _mockUniqueFractalPolygonPoints.Add(new System.Windows.Point(-1, 1));
            _mockUniqueFractalPolygonPoints.Add(new System.Windows.Point(1, 1));
            _mockUniqueFractalPolygonPoints.Add(new System.Windows.Point(1, -1));
            _mockUniqueFractalPolygonPoints.Add(new System.Windows.Point(-1, -1));

            Color unused = Color.FromRgb(1, 2, 3);
            var islandShape = new IslandShape(unused);

            // When
            var widthBeforeResizeAndMove = 2;
            var heightBeforeResizeAndMove = 2;
            var pointsBeforeResizeAndMove = new PointCollection(_mockPolygonPoints);

            var islandSize = 5.0;
            var islandLocation = new System.Windows.Point(10, 10);  // not origin
            islandShape.ResizeAndMove(islandSize, islandLocation);

            // Then
            var expectedCentrePoint = islandLocation;
            var actualCentrePoint = TestHelper_GetCentrePoint(_mockPolygonPoints);
            Assert.AreEqual(expectedCentrePoint, actualCentrePoint);

            var expectedWidth = widthBeforeResizeAndMove * islandSize;
            var actualWidth = TestHelper_GetWidthAndHeight(_mockPolygonPoints).X;
            Assert.AreEqual(expectedWidth, actualWidth);

            var expectedHeight = heightBeforeResizeAndMove * islandSize;
            var actualHeight = TestHelper_GetWidthAndHeight(_mockPolygonPoints).Y;
            Assert.AreEqual(expectedHeight, actualHeight);

            var expectedPoints = pointsBeforeResizeAndMove;
            var actualPoints = _mockPolygonPoints;
            Assert.AreNotEqual(expectedPoints, actualPoints);
        }

        [TestMethod]
        public void Moving_for_the_first_time_in_a_negative_location_relocates_the_shape()
        {
            // Given
            CreateMockFactory();
            _mockUniqueFractalPolygon.Setup(m => m.SetOutlineWidth(It.IsAny<double>()));

            // centred square
            _mockUniqueFractalPolygonPoints.Add(new System.Windows.Point(-1, 1));
            _mockUniqueFractalPolygonPoints.Add(new System.Windows.Point(1, 1));
            _mockUniqueFractalPolygonPoints.Add(new System.Windows.Point(1, -1));
            _mockUniqueFractalPolygonPoints.Add(new System.Windows.Point(-1, -1));

            Color unused = Color.FromRgb(1, 2, 3);
            var islandShape = new IslandShape(unused);

            // When
            var pointsBeforeResizeAndMove = new PointCollection(_mockPolygonPoints);

            var islandSize = 1.0;   // no resizing
            var islandLocation = new System.Windows.Point(-10, -10);  // not origin
            islandShape.ResizeAndMove(islandSize, islandLocation);

            // Then
            var expectedCentrePoint = islandLocation;
            var actualCentrePoint = TestHelper_GetCentrePoint(_mockPolygonPoints);
            Assert.AreEqual(expectedCentrePoint, actualCentrePoint);
        }

        [TestMethod]
        public void Moving_for_the_first_time_in_a_positive_location_relocates_the_shape()
        {
            // Given
            CreateMockFactory();
            _mockUniqueFractalPolygon.Setup(m => m.SetOutlineWidth(It.IsAny<double>()));

            // centred square
            _mockUniqueFractalPolygonPoints.Add(new System.Windows.Point(-1, 1));
            _mockUniqueFractalPolygonPoints.Add(new System.Windows.Point(1, 1));
            _mockUniqueFractalPolygonPoints.Add(new System.Windows.Point(1, -1));
            _mockUniqueFractalPolygonPoints.Add(new System.Windows.Point(-1, -1));
            
            Color unused = Color.FromRgb(1, 2, 3);
            var islandShape = new IslandShape(unused);

            // When
            var pointsBeforeResizeAndMove = new PointCollection(_mockPolygonPoints);

            var islandSize = 1.0;   // no resizing
            var islandLocation = new System.Windows.Point(10, 10);  // not origin
            islandShape.ResizeAndMove(islandSize, islandLocation);

            // Then
            var expectedCentrePoint = islandLocation;
            var actualCentrePoint = TestHelper_GetCentrePoint(_mockPolygonPoints);
            Assert.AreEqual(expectedCentrePoint, actualCentrePoint);
        }

        [TestMethod]
        public void Resizing_and_moving_for_the_first_time_in_a_negative_location_relocates_the_shape_by_the_given_size()
        {
            // Given
            CreateMockFactory();
            _mockUniqueFractalPolygon.Setup(m => m.SetOutlineWidth(It.IsAny<double>()));

            // centred square
            _mockUniqueFractalPolygonPoints.Add(new System.Windows.Point(-1, 1));
            _mockUniqueFractalPolygonPoints.Add(new System.Windows.Point(1, 1));
            _mockUniqueFractalPolygonPoints.Add(new System.Windows.Point(1, -1));
            _mockUniqueFractalPolygonPoints.Add(new System.Windows.Point(-1, -1));

            Color unused = Color.FromRgb(1, 2, 3);
            var islandShape = new IslandShape(unused);

            // When
            var widthBeforeResizeAndMove = 2;
            var heightBeforeResizeAndMove = 2;
            var pointsBeforeResizeAndMove = new PointCollection(_mockPolygonPoints);

            var islandSize = 5.0;
            var islandLocation = new System.Windows.Point(-10, -10);  // not origin
            islandShape.ResizeAndMove(islandSize, islandLocation);

            // Then
            var expectedCentrePoint = islandLocation;
            var actualCentrePoint = TestHelper_GetCentrePoint(_mockPolygonPoints);
            Assert.AreEqual(expectedCentrePoint, actualCentrePoint);

            var expectedWidth = widthBeforeResizeAndMove * islandSize;
            var actualWidth = TestHelper_GetWidthAndHeight(_mockPolygonPoints).X;
            Assert.AreEqual(expectedWidth, actualWidth);

            var expectedHeight = heightBeforeResizeAndMove * islandSize;
            var actualHeight = TestHelper_GetWidthAndHeight(_mockPolygonPoints).Y;
            Assert.AreEqual(expectedHeight, actualHeight);

            var expectedPoints = pointsBeforeResizeAndMove;
            var actualPoints = _mockPolygonPoints;
            Assert.AreNotEqual(expectedPoints, actualPoints);
        }

        [TestMethod]
        public void Resizing_and_moving_sets_the_centre_point()
        {
            // Given
            CreateMockFactory();
            _mockUniqueFractalPolygon.Setup(m => m.SetOutlineWidth(It.IsAny<double>()));

            // centred square
            _mockUniqueFractalPolygonPoints.Add(new System.Windows.Point(-1, 1));
            _mockUniqueFractalPolygonPoints.Add(new System.Windows.Point(1, 1));
            _mockUniqueFractalPolygonPoints.Add(new System.Windows.Point(1, -1));
            _mockUniqueFractalPolygonPoints.Add(new System.Windows.Point(-1, -1));

            Color unused = Color.FromRgb(1, 2, 3);
            var islandShape = new IslandShape(unused);

            // When
            var islandSize = 5.0;   // arbitrary
            var islandLocation = new System.Windows.Point(-10, -10);    // arbitrary
            islandShape.ResizeAndMove(islandSize, islandLocation);

            // Then
            var expectedCentrePoint = islandLocation;
            var actualCentrePoint = islandShape.CentrePoint;
            Assert.AreEqual(expectedCentrePoint, actualCentrePoint);
        }

        [TestMethod]
        public void Resizing_and_moving_sets_the_height_and_width()
        {
            // Given
            CreateMockFactory();
            _mockUniqueFractalPolygon.Setup(m => m.SetOutlineWidth(It.IsAny<double>()));

            // centred square
            _mockUniqueFractalPolygonPoints.Add(new System.Windows.Point(-1, 1));
            _mockUniqueFractalPolygonPoints.Add(new System.Windows.Point(1, 1));
            _mockUniqueFractalPolygonPoints.Add(new System.Windows.Point(1, -1));
            _mockUniqueFractalPolygonPoints.Add(new System.Windows.Point(-1, -1));

            Color unused = Color.FromRgb(1, 2, 3);
            var islandShape = new IslandShape(unused);

            // When
            var islandSize = 1.0;   // no resize
            var islandLocation = new System.Windows.Point(-10, -10);    // arbitrary
            islandShape.ResizeAndMove(islandSize, islandLocation);

            // Then
            var expectedWidthAndHeight = new System.Windows.Point(2, 2);
            var actualWidth = islandShape.Width;
            Assert.AreEqual(expectedWidthAndHeight.X, actualWidth);

            var actualHeight = islandShape.Height;
            Assert.AreEqual(expectedWidthAndHeight.Y, actualHeight);
        }
    }
}
