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
using OxyPlot;
using OxyPlot.Axes;
using ViewCommon;

namespace CoreQualityMetrics.Views
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class CoreQualityMetricView : UserControl
    {
        private CoreQualityMetrics.Presenters.CoreQualityMetricsPresenter _presenter;

        public CoreQualityMetricView(CoreQualityMetrics.Presenters.CoreQualityMetricsPresenter presenter)
        {
            InitializeComponent();

            _presenter = presenter;
            _presenter.QualityGraphUpdate += _presenter_QualityGraphUpdate;
            _presenter.ErrorDownloadingData += _presenter_ErrorDownloadingData;
            _presenter.ManualUpdate();
        }

        void _presenter_ErrorDownloadingData(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {

                messageText.Visibility = System.Windows.Visibility.Visible;
                graph.Visibility = System.Windows.Visibility.Hidden;
            }));
        }

        void _presenter_QualityGraphUpdate(object sender, Presenters.QualityGraphEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {

                messageText.Visibility = System.Windows.Visibility.Hidden;
                graph.Visibility = System.Windows.Visibility.Visible;

                var series = graph.Model.Series;
                series.Clear();

                var xaxis = graph.Model.Axes.First((axis) => axis is DateTimeAxis) as DateTimeAxis;

                if (xaxis != null)
                {
                    xaxis.StringFormat = "MMM-yyyy";
                    xaxis.IntervalType = DateTimeIntervalType.Months;
                    var now = DateTime.Now;
                    xaxis.Maximum = DateTimeAxis.ToDouble(now);
                    xaxis.Minimum = DateTimeAxis.ToDouble(now.AddYears(-2));
                    xaxis.MajorGridlineStyle = LineStyle.Solid;
                    xaxis.MajorGridlineColor = OxyColors.DarkSlateGray;
                }

                foreach (var data in e.Graphs)
                {
                    var seriesData = new OxyPlot.Series.LineSeries { Title = data.ProjectName };
                    foreach(var point in data.Points)
                    {
                        seriesData.Points.Add(OxyPlot.Axes.DateTimeAxis.CreateDataPoint(point.x, point.y));
                    }
                    seriesData.StrokeThickness = 4;
                    seriesData.MarkerType = GraphFormatting.MarkerWithColours(series.Count);
                    seriesData.MarkerSize = 6;
                    seriesData.Color = GraphFormatting.Colour(series.Count);
                    seriesData.MarkerFill = GraphFormatting.Colour(series.Count);
                    series.Add(seriesData);
                }

                graph.Model.InvalidatePlot(true);
            }));
        }
    }
}
