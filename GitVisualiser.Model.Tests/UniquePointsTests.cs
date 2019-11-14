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
    public class UniquePointsTests
    {
        [TestMethod]
        public void Are_able_to_get_seven_sets_of_unique_points()
        {
            // When
            PointCollection one = UniquePoints.Get();
            PointCollection two = UniquePoints.Get();
            PointCollection three = UniquePoints.Get();
            PointCollection four = UniquePoints.Get();
            PointCollection five = UniquePoints.Get();
            PointCollection six = UniquePoints.Get();
            PointCollection seven = UniquePoints.Get();

            // Then
            PointCollection[] allUniquePoints = {
                one,
                two,
                three,
                four,
                five,
                six,
                seven
            };

            foreach (var currentPointCollection in allUniquePoints)
            {
                var numberOfMatches = 0;
                foreach (var pointCollectionToCompare in allUniquePoints)
                {
                    if (currentPointCollection == pointCollectionToCompare)
                    {
                        numberOfMatches++;
                    }
                }
                Assert.AreEqual(1, numberOfMatches);
            }
        }
    }
}
