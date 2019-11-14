using System;
using System.Collections.Generic;
using System.Linq;

namespace GitVisualiser.Model
{
    public class Islands : IIslands
    {
        private Branch _releaseArchipelago;

        private List<Branch> _islands;
        private System.Windows.Point _centrePoint;
        private readonly ViewControlData _ViewControlData;
        private IIslandCollisionDetector _islandCollisionDetector;
        private Branch _lastHighlightedIsland;

        public Islands(ViewControlData viewControlData, Branch releaseArchipelago)
        {
            _ViewControlData = viewControlData;

            _islands = new List<Branch>();
            _islandCollisionDetector = IslandsFactory.Instance.CreateIslandCollisionDetector(viewControlData);

            AddAndMoveReleaseArchipelago(releaseArchipelago, _ViewControlData.CentrePoint);
        }

        private void AddAndMoveReleaseArchipelago(Branch releaseArchipelago, System.Windows.Point location)
        {
            _centrePoint = location;

            _releaseArchipelago = releaseArchipelago;

            uint distanceFromRelease = 0;
            _releaseArchipelago.Distance = distanceFromRelease;

            var minPixelSize = _ViewControlData.MinAndMaxPixelSizeOfIslands.Item1;
            var DefaultSize = minPixelSize;
            _releaseArchipelago.Shape.ResizeAndMove(DefaultSize, location);
        }

        public void Add(Branch newIsland)
        {
            _islands.Add(newIsland);
        }

        public void Delete(string branchName)
        {
            var islandToDelete = Find(branchName);
            if (islandToDelete == null)
            {
                throw new IndexOutOfRangeException();
            }

            _islands.Remove(islandToDelete);
        }

        public bool Contains(string branchName)
        {
            var index = _islands.FindIndex(f => f.Name == branchName);
            var islandIsInIslandList = (index != -1);
            return islandIsInIslandList;
        }

        public void SetBranchDistance(string branchName, double distance)
        {
            var island = Find(branchName);
            if (island == null)
            {
                throw new IndexOutOfRangeException();
            }

            island.Distance = distance;
        }

        public void MoveAll()
        {
            foreach (var island in _islands)
            {
                var islandIndex = GetIndex(island);
                var islandShape = island.Shape;

                System.Windows.Point islandLocation = GetIslandLocationFromDistance(islandIndex, island.Distance);

                var islandSize = GetIslandSizeFromDistance(island.Distance);
                islandShape.ResizeAndMove(islandSize, islandLocation);

                var nearestFreeLocation = _islandCollisionDetector.GetNearestFreeLocation(islandShape,
                    _releaseArchipelago.Shape);
                if (nearestFreeLocation != islandLocation)
                {
                    islandShape.ResizeAndMove(islandSize, nearestFreeLocation);
                }
            }
        }

        public System.Windows.Point GetLocation(string branchName)
        {
            var island = Find(branchName);
            if (island == null)
            {
                throw new IndexOutOfRangeException();
            }

            var islandLocation = island.Shape.CentrePoint;
            return islandLocation;
        }

        public void Highlight(string branchName)
        {
            var island = Find(branchName);
            if (island == null)
            {
                throw new IndexOutOfRangeException();
            }

            if (_lastHighlightedIsland != null)
            {
                _lastHighlightedIsland.Shape.Highlight(false);
            }

            island.Shape.Highlight(true);

            _lastHighlightedIsland = island;
        }

        public double GetDistance(string branchName)
        {
            var island = Find(branchName);
            if (island == null)
            {
                throw new IndexOutOfRangeException();
            }

            var distance = island.Distance;
            return distance;
        }

        private Branch Find(string branchName)
        {
            var index = _islands.FindIndex(f => f.Name == branchName);
            bool couldNotFindIsland = (index == -1);
            if (couldNotFindIsland)
            {
                return null;
            }

            return _islands[index];
        }

        private int GetIndex(Branch island)
        {
            var index = _islands.FindIndex(f => f.Name == island.Name);
            bool couldNotFindIsland = (index == -1);
            if (couldNotFindIsland)
            {
                return -1;
            }

            return index;
        }

        private System.Windows.Point GetIslandLocationFromDistance(int islandIndex, double distance)
        {
            var centreX = _centrePoint.X;
            var centreY = _centrePoint.Y;

            var degreesForDistanceBetweenIslands = (360 / _islands.Count()) * (double)islandIndex;
            var radiansForDistanceBetweenIslands = DegreesToRadians(degreesForDistanceBetweenIslands);
            var cos = Math.Cos(radiansForDistanceBetweenIslands);
            var sin = Math.Sin(radiansForDistanceBetweenIslands);

            var circleStartingX = distance * _ViewControlData.PixelsPerArchipelagoUnitDistance;
            var circleStartingY = 0;
            var XRelativeToOrigin = (circleStartingX * cos) - (circleStartingY * sin);
            var YRelativeToOrigin = (circleStartingX * sin) + (circleStartingY * cos);
            var XRelativeToCentre = XRelativeToOrigin + centreX;
            var YRelativeToCentre = YRelativeToOrigin + centreY;

            var islandLocX = XRelativeToCentre;
            var islandLocY = YRelativeToCentre;
            System.Windows.Point islandLocation = new System.Windows.Point(XRelativeToCentre, YRelativeToCentre);
            return islandLocation;
        }

        private double DegreesToRadians(double degrees)
        {
            return Math.PI * degrees / 180.0;
        }

        private double GetIslandSizeFromDistance(double distance)
        {
            var islandSize = distance / _ViewControlData.PixelsPerArchipelagoUnitDistance;
            var minPixelSizeToBeVisible = _ViewControlData.MinAndMaxPixelSizeOfIslands.Item1;
            var maxPixelSizeToStopTheApocolypse = _ViewControlData.MinAndMaxPixelSizeOfIslands.Item2;

            if (islandSize < minPixelSizeToBeVisible)
            {
                islandSize = minPixelSizeToBeVisible;
            }
            if (islandSize >= maxPixelSizeToStopTheApocolypse)
            {
                islandSize = maxPixelSizeToStopTheApocolypse;
            }

            return islandSize;
        }

        public List<string> GetAllNames()
        {
            var names = new List<string>();
            foreach (var island in _islands)
            {
                names.Add(island.Name);
            }

            return names;
        }
    }
}
