using System;
using System.Collections.Generic;
using System.Linq;

namespace GitVisualiser.Model
{
    public class IslandCollisionDetector : IIslandCollisionDetector
    {
        private readonly ViewControlData _ViewControlData;
        private readonly double _MinControlX;
        private readonly double _MaxControlX;
        private readonly double _MinControlY;
        private readonly double _MaxControlY;

        public IslandCollisionDetector(ViewControlData viewControlData)
        {
            _ViewControlData = viewControlData;

            _MinControlX = (0 + ViewControlData.ControlEdgePaddingPixels);
            _MaxControlX = (_ViewControlData.Width - ViewControlData.ControlEdgePaddingPixels);
            _MinControlY = (0 + ViewControlData.ControlEdgePaddingPixels);
            _MaxControlY = (_ViewControlData.Height - ViewControlData.ControlEdgePaddingPixels);
        }

        public System.Windows.Point GetNearestFreeLocation(IIslandShape possibleCollidingIslandShape, 
            IIslandShape releaseArchipelagoIslandShape)
        {
            var nonCollidingLocation = GetNonCollidingLocation(possibleCollidingIslandShape, 
                releaseArchipelagoIslandShape);

            var insideViewLoc = MoveIfOutsideOfView(possibleCollidingIslandShape, nonCollidingLocation);
            return insideViewLoc;
        }

        private System.Windows.Point GetNonCollidingLocation(IIslandShape possibleCollidingIslandShape,
            IIslandShape releaseArchipelagoIslandShape)
        {
            var originalIslandLocation = possibleCollidingIslandShape.CentrePoint;

            // deal with special case
            var shapesAreOnTopOfEachOther = (originalIslandLocation == releaseArchipelagoIslandShape.CentrePoint);
            if (shapesAreOnTopOfEachOther)
            {
                var x = releaseArchipelagoIslandShape.CentrePoint.X;
                x += (releaseArchipelagoIslandShape.Width / 2) + (possibleCollidingIslandShape.Width / 2);
                x += ViewControlData.CollisionPaddingPixels;

                var y = releaseArchipelagoIslandShape.CentrePoint.Y;

                var locationToTheRight = new System.Windows.Point(x, y);

                return locationToTheRight;
            }

            // normally, they aren't on top of each other
            var nonCollidingLocation = possibleCollidingIslandShape.CentrePoint;

            double newIslandXLocation = GetX(possibleCollidingIslandShape,
                originalIslandLocation.X,
                releaseArchipelagoIslandShape);
            if (newIslandXLocation != originalIslandLocation.X)
            {
                nonCollidingLocation.X = newIslandXLocation;
            }

            double newIslandYLocation = GetY(possibleCollidingIslandShape,
                originalIslandLocation.Y,
                releaseArchipelagoIslandShape);
            if (newIslandYLocation != originalIslandLocation.Y)
            {
                nonCollidingLocation.Y = newIslandYLocation;
            }

            return nonCollidingLocation;
        }

        private double GetX(IIslandShape possibleCollidingIslandShape, 
            double islandXLocation, 
            IIslandShape releaseArchipelagoIslandShape)
        {
            var islandCollidingXAmount = GetCollidingXAmount(possibleCollidingIslandShape, islandXLocation, releaseArchipelagoIslandShape);

            double closestNonCollidingXLocation = GetClosestNonCollidingXLocation(islandCollidingXAmount, 
                possibleCollidingIslandShape, 
                islandXLocation,
                releaseArchipelagoIslandShape);
            return closestNonCollidingXLocation;
        }

        private double GetCollidingXAmount(IIslandShape collidingIslandShape, 
            double islandXLocation, 
            IIslandShape releaseArchipelagoIslandShape)
        {
            var distX = 0.0;
            bool islandIsOnPositiveX = (islandXLocation >= releaseArchipelagoIslandShape.CentrePoint.X);
            if (islandIsOnPositiveX)
            {
                distX = islandXLocation - releaseArchipelagoIslandShape.CentrePoint.X;
            }
            else
            {
                distX = releaseArchipelagoIslandShape.CentrePoint.X - islandXLocation;
            }

            var collidingXIsSameAsReleaseX = (distX == 0.0);
            if (!collidingXIsSameAsReleaseX)
            {
                distX -= releaseArchipelagoIslandShape.Width / 2;
                distX -= collidingIslandShape.Width / 2;
            }

            // lets stop working in negatives
            distX = -distX;

            return distX;
        }

        private double GetClosestNonCollidingXLocation(double collidingAmount, 
            IIslandShape collidingIslandShape, 
            double islandXLocation,
            IIslandShape releaseArchipelagoIslandShape)
        {
            var x = islandXLocation;
            bool isCollidingOnX = (collidingAmount > 0);
            if (isCollidingOnX)
            {
                bool islandIsOnPositiveXRelativeToReleaseArchipelago = 
                    (collidingIslandShape.CentrePoint.X >= releaseArchipelagoIslandShape.CentrePoint.X);
                if (islandIsOnPositiveXRelativeToReleaseArchipelago)
                {
                    x = x + collidingAmount + ViewControlData.CollisionPaddingPixels;
                }
                else
                {
                    x = x - collidingAmount - ViewControlData.CollisionPaddingPixels;
                }
            }

            return x;
        }

        private double GetY(IIslandShape possibleCollidingIslandShape,
            double islandYLocation,
            IIslandShape releaseArchipelagoIslandShape)
        {
            var islandCollidingAmount = GetCollidingYAmount(possibleCollidingIslandShape, islandYLocation, releaseArchipelagoIslandShape);

            double closestNonCollidingYLocation = GetClosestNonCollidingYLocation(islandCollidingAmount,
                possibleCollidingIslandShape,
                islandYLocation,
                releaseArchipelagoIslandShape);
            return closestNonCollidingYLocation;
        }

        private double GetCollidingYAmount(IIslandShape collidingIslandShape,
            double islandYLocation,
            IIslandShape releaseArchipelagoIslandShape)
        {
            var distY = 0.0;
            bool islandIsOnPositiveY = (islandYLocation >= releaseArchipelagoIslandShape.CentrePoint.Y);
            if (islandIsOnPositiveY)
            {
                distY = islandYLocation - releaseArchipelagoIslandShape.CentrePoint.Y;
            }
            else
            {
                distY = releaseArchipelagoIslandShape.CentrePoint.Y - islandYLocation;
            }

            var collidingYIsSameAsReleaseY = (distY == 0.0);
            if (!collidingYIsSameAsReleaseY)
            {
                distY -= releaseArchipelagoIslandShape.Height / 2;
                distY -= collidingIslandShape.Height / 2;
            }

            // lets stop working in negatives
            distY = -distY;

            return distY;
        }

        private double GetClosestNonCollidingYLocation(double collidingAmount,
            IIslandShape collidingIslandShape,
            double islandYLocation,
            IIslandShape releaseArchipelagoIslandShape)
        {
            var y = islandYLocation;
            bool isCollidingOnY = (collidingAmount > 0);
            if (isCollidingOnY)
            {
                bool islandIsOnPositiveYRelativeToReleaseArchipelago =
                    (collidingIslandShape.CentrePoint.Y >= releaseArchipelagoIslandShape.CentrePoint.Y);
                if (islandIsOnPositiveYRelativeToReleaseArchipelago)
                {
                    y = y + collidingAmount + ViewControlData.CollisionPaddingPixels;
                }
                else
                {
                    y = y - collidingAmount - ViewControlData.CollisionPaddingPixels;
                }
            }

            return y;
        }

        private System.Windows.Point MoveIfOutsideOfView(IIslandShape islandShape, System.Windows.Point islandLocation)
        {

            var newX = GetInsideControlX(islandShape, islandLocation);
            var newY = GetInsideControlY(islandShape, islandLocation);

            var newTopMostPointOfTheIsland = (newY - (islandShape.Height / 2));
            var newBottomMostPointOfTheIsland = (newY + (islandShape.Height / 2));
            var newLeftMostPointOfTheIsland = (newX - (islandShape.Width / 2));
            var newRightMostPointOfTheIsland = (newX + (islandShape.Width / 2));

            var newXIsWithinControl = ((newLeftMostPointOfTheIsland >= _MinControlX) && 
                (newRightMostPointOfTheIsland <= _MaxControlX));
            var newYIsWithinControl = ((newTopMostPointOfTheIsland >= _MinControlY) && 
                (newBottomMostPointOfTheIsland <= _MaxControlY));
            if (newXIsWithinControl && newYIsWithinControl)
            {
                var newIslandLocation = new System.Windows.Point(newX, newY);
                return newIslandLocation;
            }

            // failed to get a location inside the control
            var islandLocationThatHasNotChanged = islandLocation;
            return islandLocationThatHasNotChanged;
        }

        private double GetInsideControlX(IIslandShape islandShape, System.Windows.Point islandLocation)
        {
            var newX = 0.0;
            var leftMostPointOfTheIsland = (islandLocation.X - (islandShape.Width / 2));
            var rightMostPointOfTheIsland = (islandLocation.X + (islandShape.Width / 2));
            if (leftMostPointOfTheIsland < _MinControlX)
            {
                newX = _MinControlX + (islandShape.Width / 2);
            }
            else if (rightMostPointOfTheIsland > _MaxControlX)
            {
                newX = _MaxControlX - (islandShape.Width / 2);
            }
            else
            {
                newX = islandLocation.X;
            }

            return newX;
        }

        private double GetInsideControlY(IIslandShape islandShape, System.Windows.Point islandLocation)
        {
            var newY = 0.0;
            var topMostPointOfTheIsland = (islandLocation.Y - (islandShape.Height / 2));
            var bottomMostPointOfTheIsland = (islandLocation.Y + (islandShape.Height / 2));
            if (topMostPointOfTheIsland < _MinControlY)
            {
                newY = _MinControlY + (islandShape.Height / 2);
            }
            else if (bottomMostPointOfTheIsland > _MaxControlY)
            {
                newY = _MaxControlY - (islandShape.Height / 2);
            }
            else
            {
                newY = islandLocation.Y;
            }

            return newY;
        }
    }
}
