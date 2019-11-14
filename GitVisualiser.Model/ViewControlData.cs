using System;

namespace GitVisualiser.Model
{
    public class ViewControlData
    {
        public readonly Tuple<double, double> MinAndMaxPixelSizeOfIslands;
        public readonly double Height;
        public readonly double Width;
        public readonly System.Windows.Point CentrePoint;
        public readonly double PixelsPerArchipelagoUnitDistance;

        public const double ControlEdgePaddingPixels = 10;    // empirical
        public const double CollisionPaddingPixels = 2;    // empirical

        // these are all empirical
        private const double MinPixelSizeDivisionModifier = 35;
        private const double MaxPixelSizeDivisionModifier = 7;
        private const double AbsoluteMinIslandPixelSize = 3;
        private const double PixelsPerArchipelagoUnitDistanceDefault = 2;

        public ViewControlData(double configFileMinPixelSizeIslandAdditiveModifier, 
            double configFileMaxPixelSizeIslandAdditiveModifier, 
            double configFilePixelsPerArchipelagoUnitDistanceAdditiveModifier, 
            double width, 
            double height)
        {
            // setting the width and height shouldn't affect the centre (hence, "width" and not "Width", etc)
            CentrePoint = new System.Windows.Point(width / 2, height / 2);

            Width = width - ControlEdgePaddingPixels;  // so that when we draw things, we don't go smack right up to the edge
            Height = height - ControlEdgePaddingPixels;

            MinAndMaxPixelSizeOfIslands = GetMinAndMaxPixelSizeOfIslands(configFileMinPixelSizeIslandAdditiveModifier, configFileMaxPixelSizeIslandAdditiveModifier);
            PixelsPerArchipelagoUnitDistance = GetPixelsPerArchipelagoUnitDistance(configFilePixelsPerArchipelagoUnitDistanceAdditiveModifier);
        }

        private Tuple<double, double> GetMinAndMaxPixelSizeOfIslands(double configFileMinPixelSizeIslandAdditiveModifier, double configFileMaxPixelSizeIslandAdditiveModifier)
        {
            var nearestControlEdgeDistance = 0.0;
            var centreValue = 0.0;
            if (Width < Height)
            {
                nearestControlEdgeDistance = Width;
                centreValue = CentrePoint.X;
            }
            else
            {
                nearestControlEdgeDistance = Height;
                centreValue = CentrePoint.Y;
            }
            var shortestDistanceFromCentreToControlEdge = nearestControlEdgeDistance - centreValue;
            var min = shortestDistanceFromCentreToControlEdge / MinPixelSizeDivisionModifier;
            var max = shortestDistanceFromCentreToControlEdge / MaxPixelSizeDivisionModifier;

            min += configFileMinPixelSizeIslandAdditiveModifier;
            max += configFileMaxPixelSizeIslandAdditiveModifier;

            if (min < 0)
            {
                min = AbsoluteMinIslandPixelSize;
            }
            if (max < 0)
            {
                max = AbsoluteMinIslandPixelSize;
            }

            Tuple<double, double> minAndMaxPixelSizeOfIslands = new Tuple<double, double>(min, max);
            return minAndMaxPixelSizeOfIslands;
        }

        private double GetPixelsPerArchipelagoUnitDistance(double configFilePixelsPerArchipelagoUnitDistanceAdditiveModifier)
        {
            double pixelsPerArchipelagoUnitDistance = PixelsPerArchipelagoUnitDistanceDefault + configFilePixelsPerArchipelagoUnitDistanceAdditiveModifier;

            if (pixelsPerArchipelagoUnitDistance <= 0)
            {
                pixelsPerArchipelagoUnitDistance = PixelsPerArchipelagoUnitDistanceDefault;
            }
            return pixelsPerArchipelagoUnitDistance;
        }
    }
}