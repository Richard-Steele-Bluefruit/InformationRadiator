using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OxyPlot;

namespace ViewCommon
{
    public class PercentageByDateGraphModel
    {
        public PercentageByDateGraphModel()
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
            // Make the maximum 100.5 as if you set the maximum to 100
            // then the Markers for points at 100 will not be drawn :(
            yaxis.Maximum = 100.5;
            yaxis.MajorGridlineStyle = LineStyle.Solid;
            yaxis.MajorGridlineColor = OxyColors.DarkSlateGray;

            Model.Axes.Add(xaxis);
            Model.Axes.Add(yaxis);
        }

        public PlotModel Model { get; private set; }
    }
}
