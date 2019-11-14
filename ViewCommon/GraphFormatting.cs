using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OxyPlot;

namespace ViewCommon
{
    public static class GraphFormatting
    {
        private static readonly OxyColor[] _lineColour =
        {
            OxyColors.Green,
            OxyColors.Blue,
            OxyColors.Red,
            OxyColors.Yellow,
            OxyColors.Purple,
            OxyColors.Orange,
            OxyColors.Brown,
            OxyColors.LightSlateGray,
        };

        private static readonly MarkerType[] _markers =
        {
            MarkerType.Circle,
            MarkerType.Square,
            MarkerType.Diamond,
            MarkerType.Triangle,
        };

        public static OxyColor Colour(int index)
        {
            return _lineColour[index % _lineColour.Length];
        }

        public static MarkerType MarkerWithColours(int index)
        {
            return _markers[(index / _lineColour.Length) % _markers.Length];
        }
    }
}
