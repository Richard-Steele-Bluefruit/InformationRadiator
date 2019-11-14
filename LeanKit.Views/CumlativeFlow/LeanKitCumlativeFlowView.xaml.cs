using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LeanKit.Presenters.CumlativeFlow;
using OxyPlot;

namespace LeanKit.Views.CumlativeFlow
{
    /// <summary>
    /// Interaction logic for LeanKitCumlativeFlowView.xaml
    /// </summary>
    public partial class LeanKitCumlativeFlowView : UserControl
    {
        LeanKit.Presenters.CumlativeFlow.LeanKitCumlativeFlowPresenter _presenter;
        public LeanKitCumlativeFlowView(LeanKit.Presenters.CumlativeFlow.LeanKitCumlativeFlowPresenter presenter)
        {
            InitializeComponent();
            _presenter = presenter;
            _presenter.CumaltiveFlowDataUpdate += _presenter_CumaltiveFlowDataUpdate;

            _presenter.ForceUpdate();
        }

        OxyColor InterpolateColour(System.Drawing.Color start, System.Drawing.Color end, int index, int count)
        {
            double range = 0;
            if(count > 1)
                range = (double)index / (double)(count - 1);

            return OxyColor.Interpolate(
                OxyColor.FromArgb(start.A, start.R, start.G, start.B),
                OxyColor.FromArgb(end.A, end.R, end.G, end.B),
                range);
        }

        void _presenter_CumaltiveFlowDataUpdate(object sender, Presenters.CumlativeFlow.CumaltiveFlowDataUpdateEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                var data = this.DataContext as GraphModel;
                if(data != null)
                {
                    int readyCount = e.Lanes.Count(l => l.Type == CumlativeFlowLaneType.Ready);
                    int inProcessCount = e.Lanes.Count(l => l.Type == CumlativeFlowLaneType.InProcess);
                    int completedCount = e.Lanes.Count(l => l.Type == CumlativeFlowLaneType.Completed);

                    int readyIndex = 0;
                    int inProcessIndex = 0;
                    int completedIndex = 0;

                    var series = data.Model.Series;
                    series.Clear();
                    int[] y = new int[_presenter.NumberOfDaysHistory];
                    for (int i = e.Lanes.Count - 1; i >= 0; i-- )
                    {
                        var lane = e.Lanes[i];
                        var seriesData = new OxyPlot.Series.AreaSeries { Title = lane.Title };
                        seriesData.LineStyle = OxyPlot.LineStyle.None;
                        seriesData.Points.Add(new OxyPlot.DataPoint((-_presenter.NumberOfDaysHistory) + 1, 0));
                        for (int x = 0; x < lane.PointsPerDay.Count; x++)
                        {
                            y[x] += lane.PointsPerDay[x];
                            seriesData.Points.Add(new OxyPlot.DataPoint((-_presenter.NumberOfDaysHistory) + x + 1, y[x]));
                        }
                        seriesData.Points.Add(new OxyPlot.DataPoint(0, 0));

                        OxyColor colour = Graph.Model.DefaultColors[i % Graph.Model.DefaultColors.Count];
                        switch (lane.Type)
                        {
                            case CumlativeFlowLaneType.Ready:
                                colour = InterpolateColour(e.ReadyStartColour, e.ReadyEndColour, readyIndex, readyCount);
                                readyIndex++;
                                break;
                            case CumlativeFlowLaneType.InProcess:
                                colour = InterpolateColour(e.InProcessStartColour, e.InProcessEndColour, inProcessIndex, inProcessCount);
                                inProcessIndex++;
                                break;
                            case CumlativeFlowLaneType.Completed:
                                colour = InterpolateColour(e.CompleteStartColour, e.CompleteEndColour, completedIndex, completedCount);
                                completedIndex++;
                                break;
                            default:
                                // Just use the currently set value
                                break;
                        }

                        seriesData.Fill = colour;
                        seriesData.Color = OxyColors.Transparent;

                        series.Insert(0, seriesData);
                    }
                }

                Graph.Model.InvalidatePlot(true);
            }));
        }
    }
}
