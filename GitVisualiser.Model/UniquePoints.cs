using System;
using System.Windows.Media;
using System.Collections.Generic;

namespace GitVisualiser.Model
{
    public class UniquePoints
    {
        private static Random _randomPointsPicker = new Random();

        private const double oneUnitLength = 5.0;
        private const double one = oneUnitLength;
        private const double half = oneUnitLength / 2;
        private const double quarter = oneUnitLength / 4;

        private static System.Windows.Point[] _shape1Points = {
            new System.Windows.Point(0,0),
            new System.Windows.Point(half, half),
            new System.Windows.Point(one, half),
            new System.Windows.Point(half, one),
            new System.Windows.Point(one, one),
        };
        private static System.Windows.Point[] _shape2Points = {
            new System.Windows.Point(one, 0),
            new System.Windows.Point(one, -half),
            new System.Windows.Point(one, quarter),
            new System.Windows.Point(0, half),
            new System.Windows.Point(half, half),
        };
        private static System.Windows.Point[] _shape3Points = {
            new System.Windows.Point(one, 0),
            new System.Windows.Point(one, -half),
            new System.Windows.Point(one, quarter),
            new System.Windows.Point(0, 0),
            new System.Windows.Point(half, half),
        };
        private static System.Windows.Point[] _shape4Points = {
            new System.Windows.Point(one, 0),
            new System.Windows.Point(one, quarter),
            new System.Windows.Point(one, -half),
            new System.Windows.Point(half, half),
            new System.Windows.Point(0, 0),
        };
        private static System.Windows.Point[] _shape5Points = {
            new System.Windows.Point(-half, -one),
            new System.Windows.Point(0, 0),
            new System.Windows.Point(half, quarter),
            new System.Windows.Point(0, 0),
            new System.Windows.Point(-quarter, -half),
            new System.Windows.Point(0, -half),
        };
        private static System.Windows.Point[] _shape6Points = {
            new System.Windows.Point(quarter, quarter),
            new System.Windows.Point(0, quarter),
            new System.Windows.Point(-half, quarter),
            new System.Windows.Point(half, 0),
            new System.Windows.Point(0, -half),
        };
        private static System.Windows.Point[] _shape7Points = {
            new System.Windows.Point(half, quarter),
            new System.Windows.Point(half, 0),
            new System.Windows.Point(half, -quarter),
            new System.Windows.Point(-quarter, half),
            new System.Windows.Point(-half, -quarter),
        };
        private static System.Windows.Point[][] _uniqueShapes = {
            _shape1Points,
            _shape2Points,
            _shape3Points,
            _shape4Points,
            _shape5Points,
            _shape6Points,
            _shape7Points,
        };

        public static PointCollection Get()
        {
            var runOutOfUniqueShapes = (_uniqueShapes.Length == 0);
            if (runOutOfUniqueShapes)
            {
                PointCollection points = new PointCollection();
                foreach (var point in _shape1Points)
                {
                    points.Add(point);
                }
                return points;
            }

            var chosenIndex = _randomPointsPicker.Next(_uniqueShapes.Length);
            System.Windows.Point[] chosenPoints = _uniqueShapes[chosenIndex];

            PointCollection pointCollection = new PointCollection();
            foreach (var point in chosenPoints)
            {
                pointCollection.Add(point);
            }

            // remove unique shape from list so that we don't get it again
            List<System.Windows.Point[]> newUniqueList = new List<System.Windows.Point[]>();
            foreach (var i in _uniqueShapes)
            {
                if (i != chosenPoints)
                {
                    newUniqueList.Add(i);
                }
            }
            System.Windows.Point[][] newUniqueListAsArray = newUniqueList.ToArray();
            _uniqueShapes = newUniqueListAsArray;

            return pointCollection;
        }
    }
}
