using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace GitVisualiser.Model.Tests
{
    [TestClass]
    public class IslandCollisionDetectorTests
    {
        [TestMethod]
        public void An_island_exactly_on_top_of_the_release_archipelago_is_moved_to_the_right()
        {
            // Given
            var unused = 0.0;
            var width = 1000; // set up so that we don't go outside of the control
            var height = 1000; // set up so that we don't go outside of the control
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var detector = new IslandCollisionDetector(viewControlData);

            Mock<IIslandShape> mockReleaseArchipelagoIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);
            mockReleaseArchipelagoIslandShape.Setup(m => m.CentrePoint).Returns(viewControlData.CentrePoint);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Height).Returns(20.0);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Width).Returns(20.0);

            // When
            Mock<IIslandShape> mockCollidingIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);
            mockCollidingIslandShape.Setup(m => m.CentrePoint).Returns(viewControlData.CentrePoint);
            mockCollidingIslandShape.Setup(m => m.Height).Returns(20.0);
            mockCollidingIslandShape.Setup(m => m.Width).Returns(20.0);

            var nearestFreeLocation = detector.GetNearestFreeLocation(mockCollidingIslandShape.Object, 
                mockReleaseArchipelagoIslandShape.Object);

            // Then
            double expectedX = mockCollidingIslandShape.Object.CentrePoint.X + 
                (mockCollidingIslandShape.Object.Width / 2) +
                (mockReleaseArchipelagoIslandShape.Object.Width / 2) +
                ViewControlData.CollisionPaddingPixels;
            double expectedY = mockCollidingIslandShape.Object.CentrePoint.Y;

            Assert.AreEqual(expectedX, nearestFreeLocation.X, "X");
            Assert.AreEqual(expectedY, nearestFreeLocation.Y, "Y");
        }

        [TestMethod]
        public void An_island_colliding_with_the_left_of_the_release_archipelago_is_moved_to_the_left()
        {
            // Given
            var unused = 0.0;
            var width = 1000; // set up so that we don't go outside of the control
            var height = 1000; // set up so that we don't go outside of the control
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var detector = new IslandCollisionDetector(viewControlData);

            Mock<IIslandShape> mockReleaseArchipelagoIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);

            mockReleaseArchipelagoIslandShape.Setup(m => m.CentrePoint).Returns(viewControlData.CentrePoint);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Height).Returns(20.0);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Width).Returns(20.0);

            // When
            double amountToTheLeftOfTheRelaseArchipelago = 5.0;
            Mock<IIslandShape> mockCollidingIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);
            mockCollidingIslandShape.Setup(m => m.CentrePoint).Returns(
                new System.Windows.Point(viewControlData.CentrePoint.X - amountToTheLeftOfTheRelaseArchipelago, // move to the left
                    viewControlData.CentrePoint.Y));
            mockCollidingIslandShape.Setup(m => m.Height).Returns(20.0);
            mockCollidingIslandShape.Setup(m => m.Width).Returns(20.0);

            var nearestFreeLocation = detector.GetNearestFreeLocation(mockCollidingIslandShape.Object,
                mockReleaseArchipelagoIslandShape.Object);

            // Then
            double expectedX = mockCollidingIslandShape.Object.CentrePoint.X - 
                amountToTheLeftOfTheRelaseArchipelago -
                (mockCollidingIslandShape.Object.Width / 2) - 
                ViewControlData.CollisionPaddingPixels;
            double expectedY = mockCollidingIslandShape.Object.CentrePoint.Y;

            Assert.AreEqual(expectedX, nearestFreeLocation.X, "X");
            Assert.AreEqual(expectedY, nearestFreeLocation.Y, "Y");
        }

        [TestMethod]
        public void An_island_colliding_with_the_right_of_the_release_archipelago_is_moved_to_the_right()
        {
            // Given
            var unused = 0.0;
            var width = 1000; // set up so that we don't go outside of the control
            var height = 1000; // set up so that we don't go outside of the control
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var detector = new IslandCollisionDetector(viewControlData);

            Mock<IIslandShape> mockReleaseArchipelagoIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);

            mockReleaseArchipelagoIslandShape.Setup(m => m.CentrePoint).Returns(viewControlData.CentrePoint);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Height).Returns(20.0);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Width).Returns(20.0);

            // When
            double amountToTheRightOfTheRelaseArchipelago = 5.0;
            Mock<IIslandShape> mockCollidingIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);
            mockCollidingIslandShape.Setup(m => m.CentrePoint).Returns(
                new System.Windows.Point(viewControlData.CentrePoint.X + amountToTheRightOfTheRelaseArchipelago, // move to the right
                    viewControlData.CentrePoint.Y));
            mockCollidingIslandShape.Setup(m => m.Height).Returns(20.0);
            mockCollidingIslandShape.Setup(m => m.Width).Returns(20.0);

            var nearestFreeLocation = detector.GetNearestFreeLocation(mockCollidingIslandShape.Object,
                mockReleaseArchipelagoIslandShape.Object);

            // Then
            double expectedX = mockCollidingIslandShape.Object.CentrePoint.X +
                amountToTheRightOfTheRelaseArchipelago +
                (mockCollidingIslandShape.Object.Width / 2) +
                ViewControlData.CollisionPaddingPixels;
            double expectedY = mockCollidingIslandShape.Object.CentrePoint.Y;

            Assert.AreEqual(expectedX, nearestFreeLocation.X, "X");
            Assert.AreEqual(expectedY, nearestFreeLocation.Y, "Y");
        }

        [TestMethod]
        public void An_island_colliding_with_the_bottom_of_the_release_archipelago_is_moved_towards_the_bottom()
        {
            // Given
            var unused = 0.0;
            var width = 1000; // set up so that we don't go outside of the control
            var height = 1000; // set up so that we don't go outside of the control
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var detector = new IslandCollisionDetector(viewControlData);

            Mock<IIslandShape> mockReleaseArchipelagoIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);

            mockReleaseArchipelagoIslandShape.Setup(m => m.CentrePoint).Returns(viewControlData.CentrePoint);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Height).Returns(20.0);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Width).Returns(20.0);

            // When
            double amountBelowTheRelaseArchipelago = 5.0;
            Mock<IIslandShape> mockCollidingIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);
            mockCollidingIslandShape.Setup(m => m.CentrePoint).Returns(
                new System.Windows.Point(viewControlData.CentrePoint.X,
                    viewControlData.CentrePoint.Y + amountBelowTheRelaseArchipelago)); // move below
            mockCollidingIslandShape.Setup(m => m.Height).Returns(20.0);
            mockCollidingIslandShape.Setup(m => m.Width).Returns(20.0);

            var nearestFreeLocation = detector.GetNearestFreeLocation(mockCollidingIslandShape.Object,
                mockReleaseArchipelagoIslandShape.Object);

            // Then
            double expectedX = mockCollidingIslandShape.Object.CentrePoint.X;
            double expectedY = mockCollidingIslandShape.Object.CentrePoint.Y +
                amountBelowTheRelaseArchipelago +
                (mockCollidingIslandShape.Object.Height / 2) +
                ViewControlData.CollisionPaddingPixels;

            Assert.AreEqual(expectedX, nearestFreeLocation.X, "X");
            Assert.AreEqual(expectedY, nearestFreeLocation.Y, "Y");
        }

        [TestMethod]
        public void An_island_colliding_with_the_top_of_the_release_archipelago_is_moved_towards_the_top()
        {
            // Given
            var unused = 0.0;
            var width = 1000; // set up so that we don't go outside of the control
            var height = 1000; // set up so that we don't go outside of the control
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var detector = new IslandCollisionDetector(viewControlData);

            Mock<IIslandShape> mockReleaseArchipelagoIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);

            mockReleaseArchipelagoIslandShape.Setup(m => m.CentrePoint).Returns(viewControlData.CentrePoint);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Height).Returns(20.0);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Width).Returns(20.0);

            // When
            double amountAboveTheRelaseArchipelago = 5.0;
            Mock<IIslandShape> mockCollidingIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);
            mockCollidingIslandShape.Setup(m => m.CentrePoint).Returns(
                new System.Windows.Point(viewControlData.CentrePoint.X,
                    viewControlData.CentrePoint.Y - amountAboveTheRelaseArchipelago)); // move below
            mockCollidingIslandShape.Setup(m => m.Height).Returns(20.0);
            mockCollidingIslandShape.Setup(m => m.Width).Returns(20.0);

            var nearestFreeLocation = detector.GetNearestFreeLocation(mockCollidingIslandShape.Object,
                mockReleaseArchipelagoIslandShape.Object);

            // Then
            double expectedX = mockCollidingIslandShape.Object.CentrePoint.X;
            double expectedY = mockCollidingIslandShape.Object.CentrePoint.Y -
                amountAboveTheRelaseArchipelago -
                (mockCollidingIslandShape.Object.Height / 2) -
                ViewControlData.CollisionPaddingPixels;

            Assert.AreEqual(expectedX, nearestFreeLocation.X, "X");
            Assert.AreEqual(expectedY, nearestFreeLocation.Y, "Y");
        }

        [TestMethod]
        public void An_island_colliding_with_the_top_and_left_of_the_release_archipelago_is_moved_towards_the_top_and_left()
        {
            // Given
            var unused = 0.0;
            var width = 1000; // set up so that we don't go outside of the control
            var height = 1000; // set up so that we don't go outside of the control
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var detector = new IslandCollisionDetector(viewControlData);

            Mock<IIslandShape> mockReleaseArchipelagoIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);

            mockReleaseArchipelagoIslandShape.Setup(m => m.CentrePoint).Returns(viewControlData.CentrePoint);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Height).Returns(20.0);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Width).Returns(20.0);

            // When
            double amountToMove = 5.0;
            Mock<IIslandShape> mockCollidingIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);
            mockCollidingIslandShape.Setup(m => m.CentrePoint).Returns(
                new System.Windows.Point(viewControlData.CentrePoint.X - amountToMove,
                    viewControlData.CentrePoint.Y - amountToMove));
            mockCollidingIslandShape.Setup(m => m.Height).Returns(20.0);
            mockCollidingIslandShape.Setup(m => m.Width).Returns(20.0);

            var nearestFreeLocation = detector.GetNearestFreeLocation(mockCollidingIslandShape.Object,
                mockReleaseArchipelagoIslandShape.Object);

            // Then
            double expectedX = mockCollidingIslandShape.Object.CentrePoint.X -
                amountToMove -
                (mockCollidingIslandShape.Object.Width / 2) -
                ViewControlData.CollisionPaddingPixels;
            double expectedY = mockCollidingIslandShape.Object.CentrePoint.Y -
                amountToMove -
                (mockCollidingIslandShape.Object.Height / 2) -
                ViewControlData.CollisionPaddingPixels;

            Assert.AreEqual(expectedX, nearestFreeLocation.X, "X");
            Assert.AreEqual(expectedY, nearestFreeLocation.Y, "Y");
        }

        [TestMethod]
        public void An_island_colliding_with_the_top_and_right_of_the_release_archipelago_is_moved_towards_the_top_and_right()
        {
            // Given
            var unused = 0.0;
            var width = 1000; // set up so that we don't go outside of the control
            var height = 1000; // set up so that we don't go outside of the control
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var detector = new IslandCollisionDetector(viewControlData);

            Mock<IIslandShape> mockReleaseArchipelagoIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);

            mockReleaseArchipelagoIslandShape.Setup(m => m.CentrePoint).Returns(viewControlData.CentrePoint);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Height).Returns(20.0);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Width).Returns(20.0);

            // When
            double amountToMove = 5.0;
            Mock<IIslandShape> mockCollidingIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);
            mockCollidingIslandShape.Setup(m => m.CentrePoint).Returns(
                new System.Windows.Point(viewControlData.CentrePoint.X + amountToMove,
                    viewControlData.CentrePoint.Y - amountToMove));
            mockCollidingIslandShape.Setup(m => m.Height).Returns(20.0);
            mockCollidingIslandShape.Setup(m => m.Width).Returns(20.0);

            var nearestFreeLocation = detector.GetNearestFreeLocation(mockCollidingIslandShape.Object,
                mockReleaseArchipelagoIslandShape.Object);

            // Then
            double expectedX = mockCollidingIslandShape.Object.CentrePoint.X +
                amountToMove +
                (mockCollidingIslandShape.Object.Width / 2) +
                ViewControlData.CollisionPaddingPixels;
            double expectedY = mockCollidingIslandShape.Object.CentrePoint.Y -
                amountToMove -
                (mockCollidingIslandShape.Object.Height / 2) -
                ViewControlData.CollisionPaddingPixels;

            Assert.AreEqual(expectedX, nearestFreeLocation.X, "X");
            Assert.AreEqual(expectedY, nearestFreeLocation.Y, "Y");
        }

        [TestMethod]
        public void An_island_colliding_with_the_bottom_and_left_of_the_release_archipelago_is_moved_towards_the_bottom_and_left()
        {
            // Given
            var unused = 0.0;
            var width = 1000; // set up so that we don't go outside of the control
            var height = 1000; // set up so that we don't go outside of the control
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var detector = new IslandCollisionDetector(viewControlData);

            Mock<IIslandShape> mockReleaseArchipelagoIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);

            mockReleaseArchipelagoIslandShape.Setup(m => m.CentrePoint).Returns(viewControlData.CentrePoint);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Height).Returns(20.0);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Width).Returns(20.0);

            // When
            double amountToMove = 5.0;
            Mock<IIslandShape> mockCollidingIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);
            mockCollidingIslandShape.Setup(m => m.CentrePoint).Returns(
                new System.Windows.Point(viewControlData.CentrePoint.X - amountToMove,
                    viewControlData.CentrePoint.Y + amountToMove));
            mockCollidingIslandShape.Setup(m => m.Height).Returns(20.0);
            mockCollidingIslandShape.Setup(m => m.Width).Returns(20.0);

            var nearestFreeLocation = detector.GetNearestFreeLocation(mockCollidingIslandShape.Object,
                mockReleaseArchipelagoIslandShape.Object);

            // Then
            double expectedX = mockCollidingIslandShape.Object.CentrePoint.X -
                amountToMove -
                (mockCollidingIslandShape.Object.Width / 2) -
                ViewControlData.CollisionPaddingPixels;
            double expectedY = mockCollidingIslandShape.Object.CentrePoint.Y +
                amountToMove +
                (mockCollidingIslandShape.Object.Height / 2) +
                ViewControlData.CollisionPaddingPixels;

            Assert.AreEqual(expectedX, nearestFreeLocation.X, "X");
            Assert.AreEqual(expectedY, nearestFreeLocation.Y, "Y");
        }

        [TestMethod]
        public void An_island_colliding_with_the_bottom_and_right_of_the_release_archipelago_is_moved_towards_the_bottom_and_right()
        {
            // Given
            var unused = 0.0;
            var width = 1000; // set up so that we don't go outside of the control
            var height = 1000; // set up so that we don't go outside of the control
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var detector = new IslandCollisionDetector(viewControlData);

            Mock<IIslandShape> mockReleaseArchipelagoIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);

            mockReleaseArchipelagoIslandShape.Setup(m => m.CentrePoint).Returns(viewControlData.CentrePoint);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Height).Returns(20.0);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Width).Returns(20.0);

            // When
            double amountToMove = 5.0;
            Mock<IIslandShape> mockCollidingIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);
            mockCollidingIslandShape.Setup(m => m.CentrePoint).Returns(
                new System.Windows.Point(viewControlData.CentrePoint.X + amountToMove,
                    viewControlData.CentrePoint.Y + amountToMove));
            mockCollidingIslandShape.Setup(m => m.Height).Returns(20.0);
            mockCollidingIslandShape.Setup(m => m.Width).Returns(20.0);

            var nearestFreeLocation = detector.GetNearestFreeLocation(mockCollidingIslandShape.Object,
                mockReleaseArchipelagoIslandShape.Object);

            // Then
            double expectedX = mockCollidingIslandShape.Object.CentrePoint.X +
                amountToMove +
                (mockCollidingIslandShape.Object.Width / 2) +
                ViewControlData.CollisionPaddingPixels;
            double expectedY = mockCollidingIslandShape.Object.CentrePoint.Y +
                amountToMove +
                (mockCollidingIslandShape.Object.Height / 2) +
                ViewControlData.CollisionPaddingPixels;

            Assert.AreEqual(expectedX, nearestFreeLocation.X, "X");
            Assert.AreEqual(expectedY, nearestFreeLocation.Y, "Y");
        }

        [TestMethod]
        public void An_island_outside_of_the_control_and_to_the_right_is_moved_back_inside_the_control()
        {
            // Given
            var unused = 0.0;
            var width = 300;
            var height = 300;
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var detector = new IslandCollisionDetector(viewControlData);

            Mock<IIslandShape> mockReleaseArchipelagoIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);

            // shouldn't collide with the release archipelago
            mockReleaseArchipelagoIslandShape.Setup(m => m.CentrePoint).Returns(viewControlData.CentrePoint);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Height).Returns(20.0);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Width).Returns(20.0);

            // When
            var arbitraryNumber = 37;
            var locationOutsideAndToTheRightOfTheControl = new System.Windows.Point(viewControlData.Width + arbitraryNumber, 
                viewControlData.CentrePoint.Y);
            Mock<IIslandShape> mockCollidingIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);
            mockCollidingIslandShape.Setup(m => m.CentrePoint).Returns(locationOutsideAndToTheRightOfTheControl);
            mockCollidingIslandShape.Setup(m => m.Height).Returns(20.0);
            mockCollidingIslandShape.Setup(m => m.Width).Returns(20.0);

            var nearestFreeLocation = detector.GetNearestFreeLocation(mockCollidingIslandShape.Object,
                mockReleaseArchipelagoIslandShape.Object);

            // Then
            double expectedX = viewControlData.Width - // furthest right
                (mockCollidingIslandShape.Object.Width / 2) -
                ViewControlData.ControlEdgePaddingPixels;
            double expectedY = locationOutsideAndToTheRightOfTheControl.Y;

            Assert.AreEqual(expectedX, nearestFreeLocation.X, "X");
            Assert.AreEqual(expectedY, nearestFreeLocation.Y, "Y");
        }

        [TestMethod]
        public void An_island_outside_of_the_control_and_to_the_left_is_moved_back_inside_the_control()
        {
            // Given
            var unused = 0.0;
            var width = 300;
            var height = 300;
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var detector = new IslandCollisionDetector(viewControlData);

            Mock<IIslandShape> mockReleaseArchipelagoIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);

            // shouldn't collide with the release archipelago
            mockReleaseArchipelagoIslandShape.Setup(m => m.CentrePoint).Returns(viewControlData.CentrePoint);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Height).Returns(20.0);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Width).Returns(20.0);

            // When
            var arbitraryNumber = 37;
            var locationOutsideOfTheControl = new System.Windows.Point(0 - arbitraryNumber,
                viewControlData.CentrePoint.Y);
            Mock<IIslandShape> mockCollidingIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);
            mockCollidingIslandShape.Setup(m => m.CentrePoint).Returns(locationOutsideOfTheControl);
            mockCollidingIslandShape.Setup(m => m.Height).Returns(20.0);
            mockCollidingIslandShape.Setup(m => m.Width).Returns(20.0);

            var nearestFreeLocation = detector.GetNearestFreeLocation(mockCollidingIslandShape.Object,
                mockReleaseArchipelagoIslandShape.Object);

            // Then
            double expectedX = 0 +  // 0 == left
                (mockCollidingIslandShape.Object.Width / 2) +
                ViewControlData.ControlEdgePaddingPixels;
            double expectedY = locationOutsideOfTheControl.Y;

            Assert.AreEqual(expectedX, nearestFreeLocation.X, "X");
            Assert.AreEqual(expectedY, nearestFreeLocation.Y, "Y");
        }

        [TestMethod]
        public void An_island_outside_of_the_control_and_towards_the_bottom_is_moved_back_inside_the_control()
        {
            // Given
            var unused = 0.0;
            var width = 300;
            var height = 300;
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var detector = new IslandCollisionDetector(viewControlData);

            Mock<IIslandShape> mockReleaseArchipelagoIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);

            // shouldn't collide with the release archipelago
            mockReleaseArchipelagoIslandShape.Setup(m => m.CentrePoint).Returns(viewControlData.CentrePoint);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Height).Returns(20.0);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Width).Returns(20.0);

            // When
            var arbitraryNumber = 37;
            var locationOutsideOfTheControl = new System.Windows.Point(viewControlData.CentrePoint.X,
                viewControlData.Height + arbitraryNumber);
            Mock<IIslandShape> mockCollidingIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);
            mockCollidingIslandShape.Setup(m => m.CentrePoint).Returns(locationOutsideOfTheControl);
            mockCollidingIslandShape.Setup(m => m.Height).Returns(20.0);
            mockCollidingIslandShape.Setup(m => m.Width).Returns(20.0);

            var nearestFreeLocation = detector.GetNearestFreeLocation(mockCollidingIslandShape.Object,
                mockReleaseArchipelagoIslandShape.Object);

            // Then
            double expectedX = locationOutsideOfTheControl.X;
            double expectedY = viewControlData.Height -
                (mockCollidingIslandShape.Object.Height / 2) -
                ViewControlData.ControlEdgePaddingPixels;

            Assert.AreEqual(expectedX, nearestFreeLocation.X, "X");
            Assert.AreEqual(expectedY, nearestFreeLocation.Y, "Y");
        }

        // why does this test take so much longer than the others to complete?
        [TestMethod]
        public void An_island_outside_of_the_control_and_towards_the_top_is_moved_back_inside_the_control()
        {
            // Given
            var unused = 0.0;
            var width = 300;
            var height = 300;
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var detector = new IslandCollisionDetector(viewControlData);

            Mock<IIslandShape> mockReleaseArchipelagoIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);

            // shouldn't collide with the release archipelago
            mockReleaseArchipelagoIslandShape.Setup(m => m.CentrePoint).Returns(viewControlData.CentrePoint);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Height).Returns(20.0);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Width).Returns(20.0);

            // When
            var arbitraryNumber = 37;
            var locationOutsideOfTheControl = new System.Windows.Point(viewControlData.CentrePoint.X,
                0 - arbitraryNumber);
            Mock<IIslandShape> mockCollidingIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);
            mockCollidingIslandShape.Setup(m => m.CentrePoint).Returns(locationOutsideOfTheControl);
            mockCollidingIslandShape.Setup(m => m.Height).Returns(20.0);
            mockCollidingIslandShape.Setup(m => m.Width).Returns(20.0);

            var nearestFreeLocation = detector.GetNearestFreeLocation(mockCollidingIslandShape.Object,
                mockReleaseArchipelagoIslandShape.Object);

            // Then
            double expectedX = locationOutsideOfTheControl.X;
            double expectedY = 0 +
                (mockCollidingIslandShape.Object.Height / 2) +
                ViewControlData.ControlEdgePaddingPixels;

            Assert.AreEqual(expectedX, nearestFreeLocation.X, "X");
            Assert.AreEqual(expectedY, nearestFreeLocation.Y, "Y");
        }

        [TestMethod]
        public void An_island_outside_of_the_control_and_towards_the_top_and_left_is_moved_back_inside_the_control()
        {
            // Given
            var unused = 0.0;
            var width = 300;
            var height = 300;
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var detector = new IslandCollisionDetector(viewControlData);

            Mock<IIslandShape> mockReleaseArchipelagoIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);

            // shouldn't collide with the release archipelago
            mockReleaseArchipelagoIslandShape.Setup(m => m.CentrePoint).Returns(viewControlData.CentrePoint);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Height).Returns(20.0);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Width).Returns(20.0);

            // When
            var arbitraryNumber = 37;
            var locationOutsideOfTheControl = new System.Windows.Point(0 - arbitraryNumber,
                0 - arbitraryNumber);
            Mock<IIslandShape> mockCollidingIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);
            mockCollidingIslandShape.Setup(m => m.CentrePoint).Returns(locationOutsideOfTheControl);
            mockCollidingIslandShape.Setup(m => m.Height).Returns(20.0);
            mockCollidingIslandShape.Setup(m => m.Width).Returns(20.0);

            var nearestFreeLocation = detector.GetNearestFreeLocation(mockCollidingIslandShape.Object,
                mockReleaseArchipelagoIslandShape.Object);

            // Then
            double expectedX = 0 +
                (mockCollidingIslandShape.Object.Width / 2) +
                ViewControlData.ControlEdgePaddingPixels;
            double expectedY = 0 +
                (mockCollidingIslandShape.Object.Height / 2) +
                ViewControlData.ControlEdgePaddingPixels;

            Assert.AreEqual(expectedX, nearestFreeLocation.X, "X");
            Assert.AreEqual(expectedY, nearestFreeLocation.Y, "Y");
        }

        [TestMethod]
        public void An_island_outside_of_the_control_and_towards_the_top_and_right_is_moved_back_inside_the_control()
        {
            // Given
            var unused = 0.0;
            var width = 300;
            var height = 300;
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var detector = new IslandCollisionDetector(viewControlData);

            Mock<IIslandShape> mockReleaseArchipelagoIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);

            // shouldn't collide with the release archipelago
            mockReleaseArchipelagoIslandShape.Setup(m => m.CentrePoint).Returns(viewControlData.CentrePoint);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Height).Returns(20.0);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Width).Returns(20.0);

            // When
            var arbitraryNumber = 37;
            var locationOutsideOfTheControl = new System.Windows.Point(viewControlData.Width + arbitraryNumber,
                0 - arbitraryNumber);
            Mock<IIslandShape> mockCollidingIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);
            mockCollidingIslandShape.Setup(m => m.CentrePoint).Returns(locationOutsideOfTheControl);
            mockCollidingIslandShape.Setup(m => m.Height).Returns(20.0);
            mockCollidingIslandShape.Setup(m => m.Width).Returns(20.0);

            var nearestFreeLocation = detector.GetNearestFreeLocation(mockCollidingIslandShape.Object,
                mockReleaseArchipelagoIslandShape.Object);

            // Then
            double expectedX = viewControlData.Width -
                (mockCollidingIslandShape.Object.Width / 2) -
                ViewControlData.ControlEdgePaddingPixels;
            double expectedY = 0 +
                (mockCollidingIslandShape.Object.Height / 2) +
                ViewControlData.ControlEdgePaddingPixels;

            Assert.AreEqual(expectedX, nearestFreeLocation.X, "X");
            Assert.AreEqual(expectedY, nearestFreeLocation.Y, "Y");
        }

        [TestMethod]
        public void An_island_outside_of_the_control_and_towards_the_bottom_and_left_is_moved_back_inside_the_control()
        {
            // Given
            var unused = 0.0;
            var width = 300;
            var height = 300;
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var detector = new IslandCollisionDetector(viewControlData);

            Mock<IIslandShape> mockReleaseArchipelagoIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);

            // shouldn't collide with the release archipelago
            mockReleaseArchipelagoIslandShape.Setup(m => m.CentrePoint).Returns(viewControlData.CentrePoint);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Height).Returns(20.0);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Width).Returns(20.0);

            // When
            var arbitraryNumber = 37;
            var locationOutsideOfTheControl = new System.Windows.Point(0 - arbitraryNumber,
                viewControlData.Height + arbitraryNumber);
            Mock<IIslandShape> mockCollidingIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);
            mockCollidingIslandShape.Setup(m => m.CentrePoint).Returns(locationOutsideOfTheControl);
            mockCollidingIslandShape.Setup(m => m.Height).Returns(20.0);
            mockCollidingIslandShape.Setup(m => m.Width).Returns(20.0);

            var nearestFreeLocation = detector.GetNearestFreeLocation(mockCollidingIslandShape.Object,
                mockReleaseArchipelagoIslandShape.Object);

            // Then
            double expectedX = 0 +
                (mockCollidingIslandShape.Object.Width / 2) +
                ViewControlData.ControlEdgePaddingPixels;
            double expectedY = viewControlData.Height -
                (mockCollidingIslandShape.Object.Height / 2) -
                ViewControlData.ControlEdgePaddingPixels;

            Assert.AreEqual(expectedX, nearestFreeLocation.X, "X");
            Assert.AreEqual(expectedY, nearestFreeLocation.Y, "Y");
        }

        [TestMethod]
        public void An_island_outside_of_the_control_and_towards_the_bottom_and_right_is_moved_back_inside_the_control()
        {
            // Given
            var unused = 0.0;
            var width = 300;
            var height = 300;
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var detector = new IslandCollisionDetector(viewControlData);

            Mock<IIslandShape> mockReleaseArchipelagoIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);

            // shouldn't collide with the release archipelago
            mockReleaseArchipelagoIslandShape.Setup(m => m.CentrePoint).Returns(viewControlData.CentrePoint);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Height).Returns(20.0);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Width).Returns(20.0);

            // When
            var arbitraryNumber = 37;
            var locationOutsideOfTheControl = new System.Windows.Point(viewControlData.Width + arbitraryNumber,
                viewControlData.Height + arbitraryNumber);
            Mock<IIslandShape> mockCollidingIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);
            mockCollidingIslandShape.Setup(m => m.CentrePoint).Returns(locationOutsideOfTheControl);
            mockCollidingIslandShape.Setup(m => m.Height).Returns(20.0);
            mockCollidingIslandShape.Setup(m => m.Width).Returns(20.0);

            var nearestFreeLocation = detector.GetNearestFreeLocation(mockCollidingIslandShape.Object,
                mockReleaseArchipelagoIslandShape.Object);

            // Then
            double expectedX = viewControlData.Width -
                (mockCollidingIslandShape.Object.Width / 2) -
                ViewControlData.ControlEdgePaddingPixels;
            double expectedY = viewControlData.Height -
                (mockCollidingIslandShape.Object.Height / 2) -
                ViewControlData.ControlEdgePaddingPixels;

            Assert.AreEqual(expectedX, nearestFreeLocation.X, "X");
            Assert.AreEqual(expectedY, nearestFreeLocation.Y, "Y");
        }

        [TestMethod]
        public void An_island_colliding_with_the_left_of_the_release_archipelago_and_outside_of_the_control_is_moved_inside_the_control()
        {
            // Given
            var unused = 0.0;
            var width = 300;
            var height = 300;
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var detector = new IslandCollisionDetector(viewControlData);
        
            Mock<IIslandShape> mockReleaseArchipelagoIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);
        
            mockReleaseArchipelagoIslandShape.Setup(m => m.CentrePoint).Returns(viewControlData.CentrePoint);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Height).Returns(50.0); // 6x smaller
            mockReleaseArchipelagoIslandShape.Setup(m => m.Width).Returns(50.0); // 6x smaller
        
            // When
            var arbitraryNumber = 5;
            Mock<IIslandShape> mockCollidingIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);
            var mockCollidingIslandShapeHeight = (150.0 + 20.0); // 1/2 height + then some
            var mockCollidingIslandShapeWidth = (150.0 + 20.0);
            mockCollidingIslandShape.Setup(m => m.CentrePoint).Returns(
                new System.Windows.Point(viewControlData.CentrePoint.X - (mockCollidingIslandShapeWidth / 2) - arbitraryNumber, 
                    viewControlData.CentrePoint.Y));
            mockCollidingIslandShape.Setup(m => m.Height).Returns(mockCollidingIslandShapeHeight);
            mockCollidingIslandShape.Setup(m => m.Width).Returns(mockCollidingIslandShapeWidth);
        
            var nearestFreeLocation = detector.GetNearestFreeLocation(mockCollidingIslandShape.Object,
                mockReleaseArchipelagoIslandShape.Object);
        
            // Then
            double expectedX = 0 + (mockCollidingIslandShapeWidth / 2) + ViewControlData.ControlEdgePaddingPixels;
            double expectedY = mockCollidingIslandShape.Object.CentrePoint.Y;
        
            Assert.AreEqual(expectedX, nearestFreeLocation.X, "X");
            Assert.AreEqual(expectedY, nearestFreeLocation.Y, "Y");
        }

        [TestMethod]
        public void An_island_colliding_with_the_right_of_the_release_archipelago_and_outside_of_the_control_is_moved_inside_the_control()
        {
            // Given
            var unused = 0.0;
            var width = 300;
            var height = 300;
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var detector = new IslandCollisionDetector(viewControlData);

            Mock<IIslandShape> mockReleaseArchipelagoIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);

            mockReleaseArchipelagoIslandShape.Setup(m => m.CentrePoint).Returns(viewControlData.CentrePoint);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Height).Returns(50.0); // 6x smaller
            mockReleaseArchipelagoIslandShape.Setup(m => m.Width).Returns(50.0); // 6x smaller

            // When
            var arbitraryNumber = 5;
            Mock<IIslandShape> mockCollidingIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);
            var mockCollidingIslandShapeHeight = (150.0 + 20.0); // 1/2 height + then some
            var mockCollidingIslandShapeWidth = (150.0 + 20.0);
            mockCollidingIslandShape.Setup(m => m.CentrePoint).Returns(
                new System.Windows.Point(viewControlData.CentrePoint.X + (mockCollidingIslandShapeWidth / 2) - arbitraryNumber,
                    viewControlData.CentrePoint.Y));
            mockCollidingIslandShape.Setup(m => m.Height).Returns(mockCollidingIslandShapeHeight);
            mockCollidingIslandShape.Setup(m => m.Width).Returns(mockCollidingIslandShapeWidth);

            var nearestFreeLocation = detector.GetNearestFreeLocation(mockCollidingIslandShape.Object,
                mockReleaseArchipelagoIslandShape.Object);

            // Then
            double expectedX = viewControlData.Width - (mockCollidingIslandShapeWidth / 2) - ViewControlData.ControlEdgePaddingPixels;
            double expectedY = mockCollidingIslandShape.Object.CentrePoint.Y;

            Assert.AreEqual(expectedX, nearestFreeLocation.X, "X");
            Assert.AreEqual(expectedY, nearestFreeLocation.Y, "Y");
        }

        [TestMethod]
        public void An_island_colliding_with_the_bottom_of_the_release_archipelago_and_outside_of_the_control_is_moved_inside_the_control()
        {
            // Given
            var unused = 0.0;
            var width = 300;
            var height = 300;
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var detector = new IslandCollisionDetector(viewControlData);

            Mock<IIslandShape> mockReleaseArchipelagoIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);

            mockReleaseArchipelagoIslandShape.Setup(m => m.CentrePoint).Returns(viewControlData.CentrePoint);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Height).Returns(50.0); // 6x smaller
            mockReleaseArchipelagoIslandShape.Setup(m => m.Width).Returns(50.0); // 6x smaller

            // When
            var arbitraryNumber = 5;
            Mock<IIslandShape> mockCollidingIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);
            var mockCollidingIslandShapeHeight = (150.0 + 20.0); // 1/2 height + then some
            var mockCollidingIslandShapeWidth = (150.0 + 20.0);
            mockCollidingIslandShape.Setup(m => m.CentrePoint).Returns(
                new System.Windows.Point(viewControlData.CentrePoint.X,
                    viewControlData.CentrePoint.Y + (mockCollidingIslandShapeWidth / 2) - arbitraryNumber));
            mockCollidingIslandShape.Setup(m => m.Height).Returns(mockCollidingIslandShapeHeight);
            mockCollidingIslandShape.Setup(m => m.Width).Returns(mockCollidingIslandShapeWidth);

            var nearestFreeLocation = detector.GetNearestFreeLocation(mockCollidingIslandShape.Object,
                mockReleaseArchipelagoIslandShape.Object);

            // Then
            double expectedX = mockCollidingIslandShape.Object.CentrePoint.X;
            double expectedY = viewControlData.Height - (mockCollidingIslandShapeHeight / 2) - ViewControlData.ControlEdgePaddingPixels;

            Assert.AreEqual(expectedX, nearestFreeLocation.X, "X");
            Assert.AreEqual(expectedY, nearestFreeLocation.Y, "Y");
        }

        [TestMethod]
        public void An_island_colliding_with_the_top_of_the_release_archipelago_and_outside_of_the_control_is_moved_inside_the_control()
        {
            // Given
            var unused = 0.0;
            var width = 300;
            var height = 300;
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var detector = new IslandCollisionDetector(viewControlData);

            Mock<IIslandShape> mockReleaseArchipelagoIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);

            mockReleaseArchipelagoIslandShape.Setup(m => m.CentrePoint).Returns(viewControlData.CentrePoint);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Height).Returns(50.0); // 6x smaller
            mockReleaseArchipelagoIslandShape.Setup(m => m.Width).Returns(50.0); // 6x smaller

            // When
            var arbitraryNumber = 5;
            Mock<IIslandShape> mockCollidingIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);
            var mockCollidingIslandShapeHeight = (150.0 + 20.0); // 1/2 height + then some
            var mockCollidingIslandShapeWidth = (150.0 + 20.0);
            mockCollidingIslandShape.Setup(m => m.CentrePoint).Returns(
                new System.Windows.Point(viewControlData.CentrePoint.X,
                    viewControlData.CentrePoint.Y - (mockCollidingIslandShapeWidth / 2) - arbitraryNumber));
            mockCollidingIslandShape.Setup(m => m.Height).Returns(mockCollidingIslandShapeHeight);
            mockCollidingIslandShape.Setup(m => m.Width).Returns(mockCollidingIslandShapeWidth);

            var nearestFreeLocation = detector.GetNearestFreeLocation(mockCollidingIslandShape.Object,
                mockReleaseArchipelagoIslandShape.Object);

            // Then
            double expectedX = mockCollidingIslandShape.Object.CentrePoint.X;
            double expectedY = 0 + (mockCollidingIslandShapeHeight / 2) + ViewControlData.ControlEdgePaddingPixels;

            Assert.AreEqual(expectedX, nearestFreeLocation.X, "X");
            Assert.AreEqual(expectedY, nearestFreeLocation.Y, "Y");
        }

        [TestMethod]
        public void An_island_colliding_with_the_top_and_left_of_the_release_archipelago_and_outside_of_the_control_is_moved_inside_the_control()
        {
            // Given
            var unused = 0.0;
            var width = 300;
            var height = 300;
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var detector = new IslandCollisionDetector(viewControlData);

            Mock<IIslandShape> mockReleaseArchipelagoIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);

            mockReleaseArchipelagoIslandShape.Setup(m => m.CentrePoint).Returns(viewControlData.CentrePoint);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Height).Returns(50.0); // 6x smaller
            mockReleaseArchipelagoIslandShape.Setup(m => m.Width).Returns(50.0); // 6x smaller

            // When
            var arbitraryNumber = 5;
            Mock<IIslandShape> mockCollidingIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);
            var mockCollidingIslandShapeHeight = (150.0 + 20.0); // 1/2 height + then some
            var mockCollidingIslandShapeWidth = (150.0 + 20.0);
            mockCollidingIslandShape.Setup(m => m.CentrePoint).Returns(
                new System.Windows.Point(viewControlData.CentrePoint.X - (mockCollidingIslandShapeWidth / 2) - arbitraryNumber,
                    viewControlData.CentrePoint.Y - (mockCollidingIslandShapeWidth / 2) - arbitraryNumber));
            mockCollidingIslandShape.Setup(m => m.Height).Returns(mockCollidingIslandShapeHeight);
            mockCollidingIslandShape.Setup(m => m.Width).Returns(mockCollidingIslandShapeWidth);

            var nearestFreeLocation = detector.GetNearestFreeLocation(mockCollidingIslandShape.Object,
                mockReleaseArchipelagoIslandShape.Object);

            // Then
            double expectedX = 0 + (mockCollidingIslandShapeWidth / 2) + ViewControlData.ControlEdgePaddingPixels;
            double expectedY = 0 + (mockCollidingIslandShapeHeight / 2) + ViewControlData.ControlEdgePaddingPixels;

            Assert.AreEqual(expectedX, nearestFreeLocation.X, "X");
            Assert.AreEqual(expectedY, nearestFreeLocation.Y, "Y");
        }

        [TestMethod]
        public void An_island_colliding_with_the_top_and_right_of_the_release_archipelago_and_outside_of_the_control_is_moved_inside_the_control()
        {
            // Given
            var unused = 0.0;
            var width = 300;
            var height = 300;
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var detector = new IslandCollisionDetector(viewControlData);

            Mock<IIslandShape> mockReleaseArchipelagoIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);

            mockReleaseArchipelagoIslandShape.Setup(m => m.CentrePoint).Returns(viewControlData.CentrePoint);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Height).Returns(50.0); // 6x smaller
            mockReleaseArchipelagoIslandShape.Setup(m => m.Width).Returns(50.0); // 6x smaller

            // When
            var arbitraryNumber = 5;
            Mock<IIslandShape> mockCollidingIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);
            var mockCollidingIslandShapeHeight = (150.0 + 20.0); // 1/2 height + then some
            var mockCollidingIslandShapeWidth = (150.0 + 20.0);
            mockCollidingIslandShape.Setup(m => m.CentrePoint).Returns(
                new System.Windows.Point(viewControlData.CentrePoint.X + (mockCollidingIslandShapeWidth / 2) - arbitraryNumber,
                    viewControlData.CentrePoint.Y - (mockCollidingIslandShapeWidth / 2) - arbitraryNumber));
            mockCollidingIslandShape.Setup(m => m.Height).Returns(mockCollidingIslandShapeHeight);
            mockCollidingIslandShape.Setup(m => m.Width).Returns(mockCollidingIslandShapeWidth);

            var nearestFreeLocation = detector.GetNearestFreeLocation(mockCollidingIslandShape.Object,
                mockReleaseArchipelagoIslandShape.Object);

            // Then
            double expectedX = viewControlData.Width - (mockCollidingIslandShapeWidth / 2) - ViewControlData.ControlEdgePaddingPixels;
            double expectedY = 0 + (mockCollidingIslandShapeHeight / 2) + ViewControlData.ControlEdgePaddingPixels;

            Assert.AreEqual(expectedX, nearestFreeLocation.X, "X");
            Assert.AreEqual(expectedY, nearestFreeLocation.Y, "Y");
        }

        [TestMethod]
        public void An_island_colliding_with_the_bottom_and_left_of_the_release_archipelago_and_outside_of_the_control_is_moved_inside_the_control()
        {
            // Given
            var unused = 0.0;
            var width = 300;
            var height = 300;
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var detector = new IslandCollisionDetector(viewControlData);

            Mock<IIslandShape> mockReleaseArchipelagoIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);

            mockReleaseArchipelagoIslandShape.Setup(m => m.CentrePoint).Returns(viewControlData.CentrePoint);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Height).Returns(50.0); // 6x smaller
            mockReleaseArchipelagoIslandShape.Setup(m => m.Width).Returns(50.0); // 6x smaller

            // When
            var arbitraryNumber = 5;
            Mock<IIslandShape> mockCollidingIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);
            var mockCollidingIslandShapeHeight = (150.0 + 20.0); // 1/2 height + then some
            var mockCollidingIslandShapeWidth = (150.0 + 20.0);
            mockCollidingIslandShape.Setup(m => m.CentrePoint).Returns(
                new System.Windows.Point(viewControlData.CentrePoint.X - (mockCollidingIslandShapeWidth / 2) - arbitraryNumber,
                    viewControlData.CentrePoint.Y + (mockCollidingIslandShapeWidth / 2) - arbitraryNumber));
            mockCollidingIslandShape.Setup(m => m.Height).Returns(mockCollidingIslandShapeHeight);
            mockCollidingIslandShape.Setup(m => m.Width).Returns(mockCollidingIslandShapeWidth);

            var nearestFreeLocation = detector.GetNearestFreeLocation(mockCollidingIslandShape.Object,
                mockReleaseArchipelagoIslandShape.Object);

            // Then
            double expectedX = 0 + (mockCollidingIslandShapeWidth / 2) + ViewControlData.ControlEdgePaddingPixels;
            double expectedY = viewControlData.Height - (mockCollidingIslandShapeHeight / 2) - ViewControlData.ControlEdgePaddingPixels;

            Assert.AreEqual(expectedX, nearestFreeLocation.X, "X");
            Assert.AreEqual(expectedY, nearestFreeLocation.Y, "Y");
        }

        [TestMethod]
        public void An_island_colliding_with_the_bottom_and_right_of_the_release_archipelago_and_outside_of_the_control_is_moved_inside_the_control()
        {
            // Given
            var unused = 0.0;
            var width = 300;
            var height = 300;
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var detector = new IslandCollisionDetector(viewControlData);

            Mock<IIslandShape> mockReleaseArchipelagoIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);

            mockReleaseArchipelagoIslandShape.Setup(m => m.CentrePoint).Returns(viewControlData.CentrePoint);
            mockReleaseArchipelagoIslandShape.Setup(m => m.Height).Returns(50.0); // 6x smaller
            mockReleaseArchipelagoIslandShape.Setup(m => m.Width).Returns(50.0); // 6x smaller

            // When
            var arbitraryNumber = 5;
            Mock<IIslandShape> mockCollidingIslandShape = new Mock<IIslandShape>(MockBehavior.Strict);
            var mockCollidingIslandShapeHeight = (150.0 + 20.0); // 1/2 height + then some
            var mockCollidingIslandShapeWidth = (150.0 + 20.0);
            mockCollidingIslandShape.Setup(m => m.CentrePoint).Returns(
                new System.Windows.Point(viewControlData.CentrePoint.X + (mockCollidingIslandShapeWidth / 2) - arbitraryNumber,
                    viewControlData.CentrePoint.Y + (mockCollidingIslandShapeWidth / 2) - arbitraryNumber));
            mockCollidingIslandShape.Setup(m => m.Height).Returns(mockCollidingIslandShapeHeight);
            mockCollidingIslandShape.Setup(m => m.Width).Returns(mockCollidingIslandShapeWidth);

            var nearestFreeLocation = detector.GetNearestFreeLocation(mockCollidingIslandShape.Object,
                mockReleaseArchipelagoIslandShape.Object);

            // Then
            double expectedX = viewControlData.Width - (mockCollidingIslandShapeWidth / 2) - ViewControlData.ControlEdgePaddingPixels;
            double expectedY = viewControlData.Height - (mockCollidingIslandShapeHeight / 2) - ViewControlData.ControlEdgePaddingPixels;

            Assert.AreEqual(expectedX, nearestFreeLocation.X, "X");
            Assert.AreEqual(expectedY, nearestFreeLocation.Y, "Y");
        }
    }
}
