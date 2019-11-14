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

namespace TeamCity.Views
{
    /// <summary>
    /// Interaction logic for TeamCityMetricsSummaryView.xaml
    /// </summary>
    public partial class TeamCityLatestMetricsView : UserControl
    {
        TeamCity.Presenters.TeamCityMetricsPresenter _presenter;

        public TeamCityLatestMetricsView(TeamCity.Presenters.TeamCityMetricsPresenter presenter)
        {
            InitializeComponent();
            _presenter = presenter;

            _presenter.MetricsError += _presenter_MetricsError;
            _presenter.MetricsUpdated += _presenter_MetricsUpdated;

            _presenter.ForceUpdate();
        }

        void _presenter_MetricsError(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                messageText.Visibility = System.Windows.Visibility.Visible;
                gridMetrics.Visibility = System.Windows.Visibility.Hidden;
            }));
        }

        void _presenter_MetricsUpdated(object sender, Presenters.TeamCityMetricsEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                messageText.Visibility = System.Windows.Visibility.Hidden;
                gridMetrics.Visibility = System.Windows.Visibility.Visible;

                gridMetrics.Children.Clear();
                gridMetrics.RowDefinitions.Clear();

                foreach (var metricSeries in e.Series)
                {
                    var row = new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) };
                    gridMetrics.RowDefinitions.Add(row);
                    var rowIndex = gridMetrics.RowDefinitions.IndexOf(row);

                    var textBrush = new SolidColorBrush(Colors.White);

                    // Create the metric name TextBox wrapped in a Viewbox for auto scaling
                    var viewBoxMetricName = new Viewbox();
                    var textBoxMetricName = new TextBlock();
                    viewBoxMetricName.Child = textBoxMetricName;
                    textBoxMetricName.Foreground = textBrush;
                    gridMetrics.Children.Add(viewBoxMetricName);
                    Grid.SetColumn(viewBoxMetricName, 0);
                    Grid.SetRow(viewBoxMetricName, rowIndex);

                    textBoxMetricName.Text = metricSeries.Name;

                    // Create the metric value TextBox wrapped in a Viewbox for auto scaling
                    var viewBoxMetricValue = new Viewbox();
                    var textBoxMetricValue = new TextBlock();
                    viewBoxMetricValue.Child = textBoxMetricValue;
                    textBoxMetricValue.Foreground = textBrush;
                    gridMetrics.Children.Add(viewBoxMetricValue);
                    Grid.SetColumn(viewBoxMetricValue, 1);
                    Grid.SetRow(viewBoxMetricValue, rowIndex);

                    var lastPoint = metricSeries.Points.Last();
                    textBoxMetricValue.Text = lastPoint.Y.ToString("0.00");
                }
            }));
        }
    }
}
