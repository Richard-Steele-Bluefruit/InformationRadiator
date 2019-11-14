using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OxyPlot;

namespace ViewCommon
{
    public class ZeroToAutoScaleByDateGraphModel
    {
        public ZeroToAutoScaleByDateGraphModel()
        {
            Model = new PlotModel();
            Model.LegendPlacement = LegendPlacement.Outside;
            Model.LegendPosition = LegendPosition.TopCenter;
            Model.LegendOrientation = LegendOrientation.Horizontal;

            Model.TextColor = OxyPlot.OxyColors.White;
            Model.Background = OxyPlot.OxyColors.Black;

            var xaxis = new OxyPlot.Axes.DateTimeAxis();
            xaxis.Position = OxyPlot.Axes.AxisPosition.Bottom;
            xaxis.StringFormat = "dd-MM-yyyy";

            var yaxis = new OxyPlot.Axes.LinearAxis();
            yaxis.Position = OxyPlot.Axes.AxisPosition.Left;
            yaxis.Minimum = 0.0;

            Model.Axes.Add(xaxis);
            Model.Axes.Add(yaxis);
        }

        public PlotModel Model { get; private set; }
    }
}
