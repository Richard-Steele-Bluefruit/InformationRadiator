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
    public class IslandTests
    {
        IslandsFactoryMock _factory;
        Mock<IIslandCollisionDetector> _mockCollisionDetector;

        #region Helper Methods

        [TestCleanup]
        public void CleanUp()
        {
            IslandsFactoryMock.Instance = null;
        }

        private void CreateMockFactory()
        {
            _factory = new IslandsFactoryMock();
            IslandsFactory.Instance = _factory;

            _mockCollisionDetector = new Mock<IIslandCollisionDetector>(MockBehavior.Strict);
            _factory._collisionDetector = _mockCollisionDetector.Object;
        }

        #endregion Helper Methods

        [TestMethod]
        public void On_creation_adds_and_moves_release_archipelago_to_the_centre_and_takes_into_account_the_padding_pixels()
        {
            // Given
            var unused = 0.0;
            var width = 100;
            var height = 200;
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var releaseBranchName = "ReleaseBranch";
            var releaseArchipelago = new Branch(releaseBranchName, Color.FromRgb(255, 255, 255));

            // When
            Islands islands = new Islands(viewControlData, releaseArchipelago);

            // Then
            var actualLocation = releaseArchipelago.Shape.CentrePoint;
            var expectedLocation = viewControlData.CentrePoint;

            Assert.AreEqual(expectedLocation.X, actualLocation.X);
            Assert.AreEqual(expectedLocation.Y, actualLocation.Y);
        }

        [TestMethod]
        public void Adding_an_island_makes_it_exist_in_the_collection()
        {
            // Given
            var unused = 0.0;
            var width = 100;
            var height = 200;
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var releaseBranchName = "ReleaseBranch";
            var unusedColour = Color.FromRgb(255, 255, 255);
            var releaseArchipelago = new Branch(releaseBranchName, unusedColour);

            Islands islands = new Islands(viewControlData, releaseArchipelago);

            // When
            var newBranchName = "bob";
            var newBranch = new Branch(newBranchName, unusedColour);
            islands.Add(newBranch);

            // Then
            var exists = islands.Contains(newBranchName);
            Assert.IsTrue(exists);
        }

        [TestMethod]
        public void Adding_multiple_islands_make_them_exist_in_the_collection()
        {
            // Given
            var unused = 0.0;
            var width = 100;
            var height = 200;
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var releaseBranchName = "ReleaseBranch";
            var unusedColour = Color.FromRgb(255, 255, 255);
            var releaseArchipelago = new Branch(releaseBranchName, unusedColour);

            Islands islands = new Islands(viewControlData, releaseArchipelago);

            // When
            var newBranchNameOne = "bob";
            var newBranch = new Branch(newBranchNameOne, unusedColour);
            islands.Add(newBranch);

            var newBranchNameTwo = "ben";
            newBranch = new Branch(newBranchNameTwo, unusedColour);
            islands.Add(newBranch);

            var newBranchNameThree = "bill";
            newBranch = new Branch(newBranchNameThree, unusedColour);
            islands.Add(newBranch);

            // Then
            var exists = islands.Contains(newBranchNameOne);
            Assert.IsTrue(exists);

            exists = islands.Contains(newBranchNameTwo);
            Assert.IsTrue(exists);

            exists = islands.Contains(newBranchNameThree);
            Assert.IsTrue(exists);
        }

        [TestMethod]
        public void Moving_with_one_island_added_moves_the_island()
        {
            // Given
            var unused = 0.0;
            var width = 100;
            var height = 200;
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var releaseBranchName = "ReleaseBranch";
            var unusedColour = Color.FromRgb(255, 255, 255);
            var releaseArchipelago = new Branch(releaseBranchName, unusedColour);

            Islands islands = new Islands(viewControlData, releaseArchipelago);

            var newBranchName = "bob";
            var newBranch = new Branch(newBranchName, unusedColour);
            islands.Add(newBranch);

            // the island has never been moved
            Assert.AreEqual(0.0, newBranch.Shape.CentrePoint.X);
            Assert.AreEqual(0.0, newBranch.Shape.CentrePoint.Y);

            // When
            islands.MoveAll();

            // Then
            Assert.AreNotEqual(0.0, newBranch.Shape.CentrePoint.X);
            Assert.AreNotEqual(0.0, newBranch.Shape.CentrePoint.Y);
        }

        [TestMethod]
        public void Moving_with_multiple_islands_added_moves_the_islands()
        {
            // Given
            var unused = 0.0;
            var width = 100;
            var height = 200;
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var releaseBranchName = "ReleaseBranch";
            var unusedColour = Color.FromRgb(255, 255, 255);
            var releaseArchipelago = new Branch(releaseBranchName, unusedColour);

            Islands islands = new Islands(viewControlData, releaseArchipelago);

            var newBranchNameOne = "bob";
            var newBranchOne = new Branch(newBranchNameOne, unusedColour);
            islands.Add(newBranchOne);
            var newBranchNameTwo = "ben";
            var newBranchTwo = new Branch(newBranchNameTwo, unusedColour);
            islands.Add(newBranchTwo);
            var newBranchNameThree = "bill";
            var newBranchThree = new Branch(newBranchNameThree, unusedColour);
            islands.Add(newBranchThree);

            Assert.AreEqual(0.0, newBranchOne.Shape.CentrePoint.X);
            Assert.AreEqual(0.0, newBranchOne.Shape.CentrePoint.Y);

            Assert.AreEqual(0.0, newBranchTwo.Shape.CentrePoint.X);
            Assert.AreEqual(0.0, newBranchTwo.Shape.CentrePoint.Y);

            Assert.AreEqual(0.0, newBranchThree.Shape.CentrePoint.X);
            Assert.AreEqual(0.0, newBranchThree.Shape.CentrePoint.Y);

            // When
            islands.MoveAll();

            // Then
            Assert.AreNotEqual(0.0, newBranchOne.Shape.CentrePoint.X);
            Assert.AreNotEqual(0.0, newBranchOne.Shape.CentrePoint.Y);

            Assert.AreNotEqual(0.0, newBranchTwo.Shape.CentrePoint.X);
            Assert.AreNotEqual(0.0, newBranchTwo.Shape.CentrePoint.Y);

            Assert.AreNotEqual(0.0, newBranchThree.Shape.CentrePoint.X);
            Assert.AreNotEqual(0.0, newBranchThree.Shape.CentrePoint.Y);
        }

        [TestMethod]
        public void Moving_multiple_islands_with_distances_set_does_not_set_their_positions_to_be_the_same()
        {
            // Given
            var unused = 0.0;
            var width = 1000;
            var height = 2000;
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var releaseBranchName = "ReleaseBranch";
            var unusedColour = Color.FromRgb(255, 255, 255);
            var releaseArchipelago = new Branch(releaseBranchName, unusedColour);

            Islands islands = new Islands(viewControlData, releaseArchipelago);

            var newBranchOne = new Branch("bob", unusedColour);
            var newBranchTwo = new Branch("ben", unusedColour);
            var newBranchThree = new Branch("bill", unusedColour);

            newBranchOne.Distance = 10;
            newBranchTwo.Distance = 20;
            newBranchThree.Distance = 30;

            islands.Add(newBranchOne);
            islands.Add(newBranchTwo);
            islands.Add(newBranchThree);

            // When
            islands.MoveAll();

            // Then
            List<Branch> islandArray = new List<Branch>(new Branch[]
                {
                    newBranchOne,
                    newBranchTwo,
                    newBranchThree
                }
            );

            foreach (var island in islandArray)
            {
                var otherIslands = islandArray.Where(b => b != island);

                foreach (var comparingIsland in otherIslands)
                {
                    Assert.AreNotEqual(comparingIsland.Shape.CentrePoint.X, island.Shape.CentrePoint.X);
                    Assert.AreNotEqual(comparingIsland.Shape.CentrePoint.Y, island.Shape.CentrePoint.Y);
                }
            }
        }

        [TestMethod]
        public void Moving_four_islands_with_distances_set_moves_each_island_to_a_corner()
        {
            // Given
            var unused = 0.0;
            var width = 1000;
            var height = 2000;
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var releaseBranchName = "ReleaseBranch";
            var unusedColour = Color.FromRgb(255, 255, 255);
            var releaseArchipelago = new Branch(releaseBranchName, unusedColour);

            Islands islands = new Islands(viewControlData, releaseArchipelago);

            var rightBranch = new Branch("right", unusedColour);
            var bottomBranch = new Branch("bottom", unusedColour);
            var leftBranch = new Branch("left", unusedColour);
            var topBranch = new Branch("top", unusedColour);
            
            rightBranch.Distance = 50;
            bottomBranch.Distance = 50;
            leftBranch.Distance = 50;
            topBranch.Distance = 50;

            // Locations are determinted by the order in which they're added, as we use the index of the 
            //      underlying list.
            /*
                -------------------
                |        4        |
                |                 |
                |                 |
                | 3      C      1 |
                |                 |
                |                 |
                |        2        |
                -------------------
            */
            islands.Add(rightBranch);   // 1
            islands.Add(bottomBranch);  // 2
            islands.Add(leftBranch);    // 3
            islands.Add(topBranch);     // 4

            // When
            islands.MoveAll();

            // Then
            var centre = viewControlData.CentrePoint;
            var rightBranchCentre = rightBranch.Shape.CentrePoint;
            rightBranchCentre.X = Math.Round(rightBranchCentre.X, 0, MidpointRounding.AwayFromZero);   // There may be some very small inaccuracy
            rightBranchCentre.Y = Math.Round(rightBranchCentre.Y, 0, MidpointRounding.AwayFromZero);   //   when we do the rotation.
            var isOnPositiveXRelativeToCentre = (rightBranchCentre.X > centre.X);
            var isOnEqualYToCentre = (rightBranchCentre.Y == centre.Y);
            var rightBranchIsOnTheRight = (isOnPositiveXRelativeToCentre && isOnEqualYToCentre);
            Assert.IsTrue(rightBranchIsOnTheRight,  "rightBranchIsOnTheRight");

            var bottomBranchCentre = bottomBranch.Shape.CentrePoint;
            bottomBranchCentre.X = Math.Round(bottomBranchCentre.X, 0, MidpointRounding.AwayFromZero);
            bottomBranchCentre.Y = Math.Round(bottomBranchCentre.Y, 0, MidpointRounding.AwayFromZero);
            var isOnEqualXToCentre = (bottomBranchCentre.X == centre.X);
            var isOnPositiveYRelativeToCentre = (bottomBranchCentre.Y > centre.Y);
            var bottomBranchIsOnTheBottom = (isOnEqualXToCentre && isOnPositiveYRelativeToCentre);
            Assert.IsTrue(bottomBranchIsOnTheBottom, "bottomBranchIsOnTheBottom");

            var leftBranchCentre = leftBranch.Shape.CentrePoint;
            leftBranchCentre.X = Math.Round(leftBranchCentre.X, 0, MidpointRounding.AwayFromZero);
            leftBranchCentre.Y = Math.Round(leftBranchCentre.Y, 0, MidpointRounding.AwayFromZero);
            isOnPositiveXRelativeToCentre = (leftBranchCentre.X > centre.X);
            isOnEqualYToCentre = (leftBranchCentre.Y == centre.Y);
            var leftBranchIsOnTheLeft = (!isOnPositiveXRelativeToCentre && isOnEqualYToCentre);
            Assert.IsTrue(leftBranchIsOnTheLeft, "leftBranchIsOnTheLeft");

            var topBranchCentre = topBranch.Shape.CentrePoint;
            topBranchCentre.X = Math.Round(topBranchCentre.X, 0, MidpointRounding.AwayFromZero);
            topBranchCentre.Y = Math.Round(topBranchCentre.Y, 0, MidpointRounding.AwayFromZero);
            isOnEqualXToCentre = (topBranchCentre.X == centre.X);
            var isOnNegativeYRelativeToCentre = (topBranchCentre.Y < centre.Y);
            var topBranchIsOnTheTop = (isOnNegativeYRelativeToCentre && isOnEqualXToCentre);
            Assert.IsTrue(topBranchIsOnTheTop, "topBranchIsOnTheTop");
        }

        [TestMethod]
        public void Branch_moves_to_rotated_location_when_there_is_no_collision()
        {
            // Given
            CreateMockFactory();
            
            var unused = 0.0;
            var width = 1000;
            var height = 2000;
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var releaseBranchName = "ReleaseBranch";
            var unusedColour = Color.FromRgb(255, 255, 255);
            var releaseArchipelago = new Branch(releaseBranchName, unusedColour);

            Islands islands = new Islands(viewControlData, releaseArchipelago);

            var branch = new Branch("mr branch", unusedColour);

            branch.Distance = 50;
            islands.Add(branch);

            _mockCollisionDetector.Setup(m => m.GetNearestFreeLocation(It.IsAny<IslandShape>(),
                It.IsAny<IslandShape>())).Returns(
                    (IslandShape possibleCollidingIslandShapeParameterPassedIn,
                    IslandShape releaseArchipelagoIslandShapeParameterPassedIn) =>
                        possibleCollidingIslandShapeParameterPassedIn.CentrePoint);   // there is no collision, always returns given location

            // When
            islands.MoveAll();

            // Then
            var expectedRotatedLocation = new System.Windows.Point(
                (branch.Distance * viewControlData.PixelsPerArchipelagoUnitDistance) + viewControlData.CentrePoint.X, 
                viewControlData.CentrePoint.Y);
            var branchCentre = branch.Shape.CentrePoint;
            Assert.AreEqual(expectedRotatedLocation.X, branchCentre.X);
            Assert.AreEqual(expectedRotatedLocation.Y, branchCentre.Y);
        }

        [TestMethod]
        public void Branch_moves_to_collision_free_location_instead_of_rotated_location_when_there_is_a_collision()
        {
            // Given
            CreateMockFactory();
            var collisionFreeLocation = new System.Windows.Point(999, 999);
            _mockCollisionDetector.Setup(m => m.GetNearestFreeLocation(It.IsAny<IslandShape>(), 
                It.IsAny<IslandShape>())).Returns(collisionFreeLocation);   // forcing a collision

            var unused = 0.0;
            var width = 1000;
            var height = 2000;
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var releaseBranchName = "ReleaseBranch";
            var unusedColour = Color.FromRgb(255, 255, 255);
            var releaseArchipelago = new Branch(releaseBranchName, unusedColour);

            Islands islands = new Islands(viewControlData, releaseArchipelago);

            var branch = new Branch("mr branch", unusedColour);
            
            branch.Distance = 50;
            islands.Add(branch);

            // When
            islands.MoveAll();

            // Then
            var branchCentre = branch.Shape.CentrePoint;
            Assert.AreEqual(collisionFreeLocation.X, branchCentre.X);
            Assert.AreEqual(collisionFreeLocation.Y, branchCentre.Y);
        }

        [TestMethod]
        public void Moving_multiple_branches_to_the_collision_free_location_instead_of_a_rotated_location_when_there_are_collisions()
        {
            // Given
            CreateMockFactory();
            var collisionFreeLocation = new System.Windows.Point(999, 999);
            _mockCollisionDetector.Setup(m => m.GetNearestFreeLocation(It.IsAny<IslandShape>(),
                It.IsAny<IslandShape>())).Returns(collisionFreeLocation);

            var unused = 0.0;
            var width = 1000;
            var height = 2000;
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var releaseBranchName = "ReleaseBranch";
            var unusedColour = Color.FromRgb(255, 255, 255);
            var releaseArchipelago = new Branch(releaseBranchName, unusedColour);

            Islands islands = new Islands(viewControlData, releaseArchipelago);

            var branch1 = new Branch("mr 1", unusedColour);
            var branch2 = new Branch("mr 2", unusedColour);
            var branch3 = new Branch("mr 3", unusedColour);

            branch1.Distance = 50;
            branch2.Distance = 50;
            branch3.Distance = 50;

            islands.Add(branch1);
            islands.Add(branch2);
            islands.Add(branch3);

            // When
            islands.MoveAll();

            // Then
            var branchCentre = branch1.Shape.CentrePoint;
            Assert.AreEqual(collisionFreeLocation.X, branchCentre.X);
            Assert.AreEqual(collisionFreeLocation.Y, branchCentre.Y);

            branchCentre = branch2.Shape.CentrePoint;
            Assert.AreEqual(collisionFreeLocation.X, branchCentre.X);
            Assert.AreEqual(collisionFreeLocation.Y, branchCentre.Y);

            branchCentre = branch3.Shape.CentrePoint;
            Assert.AreEqual(collisionFreeLocation.X, branchCentre.X);
            Assert.AreEqual(collisionFreeLocation.Y, branchCentre.Y);
        }

        [TestMethod]
        public void Highlighting_a_branch_that_does_not_exist_does_not_highlight_any_branches()
        {
            // Given
            var unused = 0.0;
            var width = 1000;
            var height = 2000;
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var releaseBranchName = "ReleaseBranch";
            var unusedColour = Color.FromRgb(255, 255, 255);
            var releaseArchipelago = new Branch(releaseBranchName, unusedColour);

            Islands islands = new Islands(viewControlData, releaseArchipelago);

            var branch = new Branch("existingBranch", unusedColour);

            // this is a bit of a kludge, really, we should be mocking
            double[] islandStrokeThicknessesBeforeHighlighting = 
            {
                branch.Shape.MainIslandShape.Shape.StrokeThickness,
                releaseArchipelago.Shape.MainIslandShape.Shape.StrokeThickness
            };

            islands.Add(branch);

            // When
            bool exceptionRaised = false;
            try
            {
                islands.Highlight("name of a branch that does not exist");
            }
            catch (IndexOutOfRangeException)
            {
                exceptionRaised = true;
            }

            // Then
            Assert.IsTrue(exceptionRaised);

            Assert.AreEqual(islandStrokeThicknessesBeforeHighlighting[0], branch.Shape.MainIslandShape.Shape.StrokeThickness);
            Assert.AreEqual(islandStrokeThicknessesBeforeHighlighting[1], releaseArchipelago.Shape.MainIslandShape.Shape.StrokeThickness);
        }

        [TestMethod]
        public void Highlighting_a_branch_that_does_exist_highlights_only_that_branch()
        {
            // Given
            var unused = 0.0;
            var width = 1000;
            var height = 2000;
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var releaseBranchName = "ReleaseBranch";
            var unusedColour = Color.FromRgb(255, 255, 255);
            var releaseArchipelago = new Branch(releaseBranchName, unusedColour);

            Islands islands = new Islands(viewControlData, releaseArchipelago);

            var branch = new Branch("existingBranch", unusedColour);
            var branch2 = new Branch("anotherBranch", unusedColour);

            // this is a bit of a kludge, really, we should be mocking
            double[] islandStrokeThicknessesBeforeHighlighting = 
            {
                branch.Shape.MainIslandShape.Shape.StrokeThickness,
                releaseArchipelago.Shape.MainIslandShape.Shape.StrokeThickness,
                branch2.Shape.MainIslandShape.Shape.StrokeThickness
            };

            islands.Add(branch);
            islands.Add(branch2);

            // When
            islands.Highlight("existingBranch");

            // Then
            Assert.AreNotEqual(islandStrokeThicknessesBeforeHighlighting[0], branch.Shape.MainIslandShape.Shape.StrokeThickness);
            Assert.AreEqual(islandStrokeThicknessesBeforeHighlighting[1], releaseArchipelago.Shape.MainIslandShape.Shape.StrokeThickness);
            Assert.AreEqual(islandStrokeThicknessesBeforeHighlighting[2], branch2.Shape.MainIslandShape.Shape.StrokeThickness);
        }

        [TestMethod]
        public void Highlighting_a_new_branch_unhighlights_the_previously_highlighted_branch()
        {
            // Given
            var unused = 0.0;
            var width = 1000;
            var height = 2000;
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var releaseBranchName = "ReleaseBranch";
            var unusedColour = Color.FromRgb(255, 255, 255);
            var releaseArchipelago = new Branch(releaseBranchName, unusedColour);

            Islands islands = new Islands(viewControlData, releaseArchipelago);

            var branch = new Branch("existingBranch", unusedColour);
            var branch2 = new Branch("anotherBranch", unusedColour);

            islands.Add(branch);
            islands.Add(branch2);

            islands.Highlight("existingBranch");

            // this is a bit of a kludge, really, we should be mocking
            double[] islandStrokeThicknessesAfterFirstHighlighting = 
            {
                branch.Shape.MainIslandShape.Shape.StrokeThickness,
                releaseArchipelago.Shape.MainIslandShape.Shape.StrokeThickness,
                branch2.Shape.MainIslandShape.Shape.StrokeThickness
            };

            islands.Add(branch);
            islands.Add(branch2);

            // When
            islands.Highlight("anotherBranch");

            // Then
            Assert.AreNotEqual(islandStrokeThicknessesAfterFirstHighlighting[0], branch.Shape.MainIslandShape.Shape.StrokeThickness);
            Assert.AreEqual(islandStrokeThicknessesAfterFirstHighlighting[1], releaseArchipelago.Shape.MainIslandShape.Shape.StrokeThickness);
            Assert.AreNotEqual(islandStrokeThicknessesAfterFirstHighlighting[2], branch2.Shape.MainIslandShape.Shape.StrokeThickness);
        }

        [TestMethod]
        public void An_island_does_not_exist_if_it_has_been_deleted()
        {
            // Given
            var unused = 0.0;
            var width = 1000;
            var height = 2000;
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var releaseBranchName = "ReleaseBranch";
            var unusedColour = Color.FromRgb(255, 255, 255);
            var releaseArchipelago = new Branch(releaseBranchName, unusedColour);

            Islands islands = new Islands(viewControlData, releaseArchipelago);

            var branch = new Branch("existingBranch", unusedColour);
            var branch2 = new Branch("branch-to-delete", unusedColour);
            var branch3 = new Branch("anotherBranch", unusedColour);

            islands.Add(branch);
            islands.Add(branch2);
            islands.Add(branch3);

            // When 
            islands.Delete("branch-to-delete");

            // Then
            var islandExists = islands.Contains("branch-to-delete");

            Assert.IsFalse(islandExists);
        }

        [TestMethod]
        public void Deleting_an_island_that_does_not_exist()
        {
            // Given
            var unused = 0.0;
            var width = 1000;
            var height = 2000;
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var releaseBranchName = "ReleaseBranch";
            var unusedColour = Color.FromRgb(255, 255, 255);
            var releaseArchipelago = new Branch(releaseBranchName, unusedColour);

            Islands islands = new Islands(viewControlData, releaseArchipelago);

            var branch = new Branch("existingBranch", unusedColour);

            islands.Add(branch);

            // When 
            bool exceptionRaised = false;
            try
            {
                islands.Delete("branch-that-does-not-exist");
            }
            catch (IndexOutOfRangeException)
            {
                exceptionRaised = true;
            }

            // Then
            Assert.IsTrue(exceptionRaised);

            var islandExists = islands.Contains("branch-that-does-not-exist");
            Assert.IsFalse(islandExists);
        }

        [TestMethod]
        public void Setting_an_islands_distance()
        {
            // Given
            var unused = 0.0;
            var width = 1000;
            var height = 2000;
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var releaseBranchName = "ReleaseBranch";
            var unusedColour = Color.FromRgb(255, 255, 255);
            var releaseArchipelago = new Branch(releaseBranchName, unusedColour);

            Islands islands = new Islands(viewControlData, releaseArchipelago);

            var branch = new Branch("existingBranch", unusedColour);
            islands.Add(branch);

            var distanceToSet = 123.45;
            islands.SetBranchDistance("existingBranch", distanceToSet);

            // When 
            var actualDistance = islands.GetDistance("existingBranch");

            // Then
            var expectedDistance = distanceToSet;
            Assert.AreEqual(expectedDistance, actualDistance);
        }

        [TestMethod]
        public void Setting_an_islands_distance_for_an_island_that_does_not_exist()
        {
            // Given
            var unused = 0.0;
            var width = 1000;
            var height = 2000;
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var releaseBranchName = "ReleaseBranch";
            var unusedColour = Color.FromRgb(255, 255, 255);
            var releaseArchipelago = new Branch(releaseBranchName, unusedColour);

            Islands islands = new Islands(viewControlData, releaseArchipelago);

            // When
            bool exceptionRaised = false;
            try
            {
                var distanceToSet = 123.45;
                islands.SetBranchDistance("branch-that-does-not-exist", distanceToSet);
            }
            catch (IndexOutOfRangeException)
            {
                exceptionRaised = true;
            }

            // Then
            Assert.IsTrue(exceptionRaised);
        }

        [TestMethod]
        public void Getting_an_islands_distance_for_an_island_that_does_not_exist()
        {
            // Given
            var unused = 0.0;
            var width = 1000;
            var height = 2000;
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var releaseBranchName = "ReleaseBranch";
            var unusedColour = Color.FromRgb(255, 255, 255);
            var releaseArchipelago = new Branch(releaseBranchName, unusedColour);

            Islands islands = new Islands(viewControlData, releaseArchipelago);

            // When
            bool exceptionRaised = false;
            try
            {
                islands.GetDistance("branch-that-does-not-exist");
            }
            catch (IndexOutOfRangeException)
            {
                exceptionRaised = true;
            }

            // Then
            Assert.IsTrue(exceptionRaised);
        }

        [TestMethod]
        public void Getting_an_islands_location()
        {
            // Given
            var unused = 0.0;
            var width = 1000;
            var height = 2000;
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var releaseBranchName = "ReleaseBranch";
            var unusedColour = Color.FromRgb(255, 255, 255);
            var releaseArchipelago = new Branch(releaseBranchName, unusedColour);

            Islands islands = new Islands(viewControlData, releaseArchipelago);
            var branch = new Branch("existingBranch", unusedColour);
            islands.Add(branch);

            // When
            var actualLocation = islands.GetLocation("existingBranch");

            // Then
            var expectedLocation = branch.Shape.CentrePoint;
            Assert.AreEqual(expectedLocation, actualLocation);
        }

        [TestMethod]
        public void Getting_an_islands_location_for_an_island_that_does_not_exist()
        {
            // Given
            var unused = 0.0;
            var width = 1000;
            var height = 2000;
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var releaseBranchName = "ReleaseBranch";
            var unusedColour = Color.FromRgb(255, 255, 255);
            var releaseArchipelago = new Branch(releaseBranchName, unusedColour);
        
            Islands islands = new Islands(viewControlData, releaseArchipelago);
        
            // When
            bool exceptionRaised = false;
            try
            {
                islands.GetLocation("branch-that-does-not-exist");
            }
            catch (IndexOutOfRangeException)
            {
                exceptionRaised = true;
            }
        
            // Then
            Assert.IsTrue(exceptionRaised);
        }

        [TestMethod]
        public void Getting_all_island_names()
        {
            // Given
            var unused = 0.0;
            var width = 1000;
            var height = 2000;
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var releaseBranchName = "ReleaseBranch";
            var unusedColour = Color.FromRgb(255, 255, 255);
            var releaseArchipelago = new Branch(releaseBranchName, unusedColour);

            Islands islands = new Islands(viewControlData, releaseArchipelago);
            var branch1 = new Branch("one", unusedColour);
            var branch2 = new Branch("two", unusedColour);
            var branch3 = new Branch("three", unusedColour);

            islands.Add(branch1);
            islands.Add(branch2);
            islands.Add(branch3);

            // When
            var nameList = islands.GetAllNames();

            // Then
            var expectedCount = 3;
            var actualCount = nameList.Count;
            Assert.AreEqual(expectedCount, actualCount);

            var branchOneIsInTheList = nameList.Contains("one");
            Assert.IsTrue(branchOneIsInTheList);

            var branchTwoIsInTheList = nameList.Contains("two");
            Assert.IsTrue(branchTwoIsInTheList);

            var branchThreeIsInTheList = nameList.Contains("three");
            Assert.IsTrue(branchThreeIsInTheList);
        }

        [TestMethod]
        public void Getting_all_island_names_when_there_are_no_islands()
        {
            // Given
            var unused = 0.0;
            var width = 1000;
            var height = 2000;
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var releaseBranchName = "ReleaseBranch";
            var unusedColour = Color.FromRgb(255, 255, 255);
            var releaseArchipelago = new Branch(releaseBranchName, unusedColour);

            Islands islands = new Islands(viewControlData, releaseArchipelago);

            // When
            var nameList = islands.GetAllNames();

            // Then
            var expectedCount = 0;
            var actualCount = nameList.Count;
            Assert.AreEqual(expectedCount, actualCount);
        }

        [TestMethod]
        public void Removing_an_island_and_getting_all_island_names_does_not_include_the_removed_island_in_the_list()
        {
            // Given
            var unused = 0.0;
            var width = 1000;
            var height = 2000;
            var viewControlData = new ViewControlData(unused, unused, unused, width, height);
            var releaseBranchName = "ReleaseBranch";
            var unusedColour = Color.FromRgb(255, 255, 255);
            var releaseArchipelago = new Branch(releaseBranchName, unusedColour);

            Islands islands = new Islands(viewControlData, releaseArchipelago);
            var branch1 = new Branch("one", unusedColour);
            var branch2 = new Branch("island-to-delete", unusedColour);
            var branch3 = new Branch("three", unusedColour);

            islands.Add(branch1);
            islands.Add(branch2);
            islands.Add(branch3);

            islands.Delete("island-to-delete");

            // When
            var nameList = islands.GetAllNames();

            // Then
            var expectedCount = 2;
            var actualCount = nameList.Count;
            Assert.AreEqual(expectedCount, actualCount);

            var islandToDeleteIsInTheList = nameList.Contains("island-to-delete");
            Assert.IsFalse(islandToDeleteIsInTheList);
        }
    }
}
