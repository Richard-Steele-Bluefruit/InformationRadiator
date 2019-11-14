using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using GitVisualiser.Model;

namespace GitVisualiser.Model.Tests
{
    [TestClass]
    public class ViewControlDataTests
    {
        [TestMethod]
        public void Public_properties_are_set_on_creation()
        {
            // When
            double configFileMinPixelSizeIslandAdditiveModifier = 0.0;
            double configFileMaxPixelSizeIslandAdditiveModifier = 0.0;
            double configFilePixelsPerArchipelagoUnitDistanceAdditiveModifier = 0.0;
            double width = 640;
            double height = 480;
            ViewControlData _viewControlData = new ViewControlData(configFileMinPixelSizeIslandAdditiveModifier,
                configFileMaxPixelSizeIslandAdditiveModifier,
                configFilePixelsPerArchipelagoUnitDistanceAdditiveModifier,
                width,
                height);

            // Then
            Assert.AreNotEqual(0.0, _viewControlData.MinAndMaxPixelSizeOfIslands.Item1);
            Assert.AreNotEqual(0.0, _viewControlData.MinAndMaxPixelSizeOfIslands.Item2);
            Assert.AreNotEqual(0.0, _viewControlData.Width);
            Assert.AreNotEqual(0.0, _viewControlData.Height);
            Assert.AreNotEqual(0.0, _viewControlData.CentrePoint.X);
            Assert.AreNotEqual(0.0, _viewControlData.CentrePoint.Y);
            Assert.AreNotEqual(0.0, _viewControlData.PixelsPerArchipelagoUnitDistance);

        }

        [TestMethod]
        public void The_width_height_and_centre_point_take_into_account_padding_around_the_edge()
        {
            // When
            double configFileMinPixelSizeIslandAdditiveModifier = 0.0;
            double configFileMaxPixelSizeIslandAdditiveModifier = 0.0; 
            double configFilePixelsPerArchipelagoUnitDistanceAdditiveModifier = 0.0;
            double width = 640;
            double height = 480;
            ViewControlData _viewControlData = new ViewControlData(configFileMinPixelSizeIslandAdditiveModifier,
                configFileMaxPixelSizeIslandAdditiveModifier,
                configFilePixelsPerArchipelagoUnitDistanceAdditiveModifier,
                width,
                height);

            // Then
            var actualWidth = _viewControlData.Width;
            var givenWidth = width;
            Assert.IsTrue(actualWidth < givenWidth);

            var actualHeight = _viewControlData.Height;
            var givenHeight = height;
            Assert.IsTrue(actualHeight < givenHeight);

            // the centre point should not be affected by the padding pixels
            var expectedCentrePointX = width / 2;
            var actualCentrePointX = _viewControlData.CentrePoint.X;
            Assert.AreEqual(expectedCentrePointX, actualCentrePointX);

            var expectedCentrePointY = height / 2;
            var actualCentrePointY = _viewControlData.CentrePoint.Y;
            Assert.AreEqual(expectedCentrePointY, actualCentrePointY);
        }

        [TestMethod]
        public void Sets_the_min_and_max_island_sizes_to_be_relative_to_the_width_and_height()
        {
            // When
            double configFileMinPixelSizeIslandAdditiveModifier = 0.0;
            double configFileMaxPixelSizeIslandAdditiveModifier = 0.0;
            double configFilePixelsPerArchipelagoUnitDistanceAdditiveModifier = 0.0;
            double width = 100;
            double height = 100;
            ViewControlData _viewControlDataForSmallControl = new ViewControlData(configFileMinPixelSizeIslandAdditiveModifier,
                configFileMaxPixelSizeIslandAdditiveModifier,
                configFilePixelsPerArchipelagoUnitDistanceAdditiveModifier,
                width,
                height);

            width = 1000;
            height = 1000;
            ViewControlData _viewControlDataForLargeControl = new ViewControlData(configFileMinPixelSizeIslandAdditiveModifier,
                configFileMaxPixelSizeIslandAdditiveModifier,
                configFilePixelsPerArchipelagoUnitDistanceAdditiveModifier,
                width,
                height);

            // Then
            var smallControlMinSize = _viewControlDataForSmallControl.MinAndMaxPixelSizeOfIslands.Item1;
            var largeControlMinSize = _viewControlDataForLargeControl.MinAndMaxPixelSizeOfIslands.Item1;
            var minSizeForSmallControlIsSmaller = (smallControlMinSize < largeControlMinSize);
            Assert.IsTrue(minSizeForSmallControlIsSmaller, "minSizeForSmallControlIsSmaller");

            var smallControlMaxSize = _viewControlDataForSmallControl.MinAndMaxPixelSizeOfIslands.Item2;
            var largeControlMaxSize = _viewControlDataForLargeControl.MinAndMaxPixelSizeOfIslands.Item2;
            var maxSizeForSmallControlIsSmaller = (smallControlMaxSize < largeControlMaxSize);
            Assert.IsTrue(maxSizeForSmallControlIsSmaller, "maxSizeForSmallControlIsSmaller");
        }
    }
}
