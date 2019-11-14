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
using OxyPlot.Axes;

namespace TeamCity.Views
{
    /// <summary>
    /// Interaction logic for TeamCityMetricsView.xaml
    /// </summary>
    public partial class TeamCityMetricsView : UserControl
    {
        TeamCity.Presenters.TeamCityMetricsPresenter _presenter;

        public TeamCityMetricsView(TeamCity.Presenters.TeamCityMetricsPresenter presenter)
        {
            InitializeComponent();
            _presenter = presenter;

            _presenter.MetricsUpdated += _presenter_MetricsUpdated;
            _presenter.MetricsError += _presenter_MetricsError;

            if (!_presenter.AutoscaleGraphYAxis)
            {
                var yaxis = graph.Model.Axes.FirstOrDefault(a => a.IsVertical()) as LinearAxis;
                if (yaxis != null)
                {
                    yaxis.Maximum = (double) _presenter.GraphYAxisMax;
                }
            }

            _presenter.ForceUpdate();
        }

        void _presenter_MetricsError(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {

                messageText.Visibility = System.Windows.Visibility.Visible;
                graph.Visibility = System.Windows.Visibility.Hidden;
            }));
        }

        void _presenter_MetricsUpdated(object sender, Presenters.TeamCityMetricsEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                messageText.Visibility = System.Windows.Visibility.Hidden;
                graph.Visibility = System.Windows.Visibility.Visible;

                var data = this.DataContext as ViewCommon.ZeroToAutoScaleByDateGraphModel;
                if (data != null)
                {
                    var series = data.Model.Series;
                    series.Clear();

                    foreach(var metricSeries in e.Series)
                    {
                        var seriesData = new OxyPlot.Series.LineSeries { Title = metricSeries.Name };
                        seriesData.MarkerType = OxyPlot.MarkerType.Circle;
                        seriesData.MarkerSize = 4;
                        foreach(var metricPoint in metricSeries.Points)
                        {
                            seriesData.Points.Add(OxyPlot.Axes.DateTimeAxis.CreateDataPoint(metricPoint.X, metricPoint.Y));
                        }
                        seriesData.StrokeThickness = 4;
                        series.Add(seriesData);
                    }
                }

                graph.Model.InvalidatePlot(true);
            }));
        }
    }
}
