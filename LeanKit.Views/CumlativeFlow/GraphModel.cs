using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OxyPlot;

namespace LeanKit.Views.CumlativeFlow
{
    public class GraphModel
    {
        public GraphModel()
        {
            Model = new PlotModel();
            Model.LegendPlacement = LegendPlacement.Outside;
            Model.LegendPosition = LegendPosition.TopCenter;
            Model.LegendOrientation = LegendOrientation.Horizontal;

            Model.TextColor = OxyPlot.OxyColors.White;
            Model.Background = OxyPlot.OxyColors.Black;
        }

        public PlotModel Model { get; private set; }
    }
}
