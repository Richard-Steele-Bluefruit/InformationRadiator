using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections.ObjectModel;
using PresenterCommon.Configuration;

namespace TeamCity.Presenters
{
    public class TeamCityMetricsSeriesPoint
    {
        public TeamCityMetricsSeriesPoint(DateTime x, float y)
        {
            X = x;
            Y = y;
        }

        public DateTime X { get; private set; }
        public float Y { get; private set; }
    }

    public class TeamCityMetricsSeries
    {
        public TeamCityMetricsSeries(string name, List<TeamCityMetricsSeriesPoint> points)
        {
            Name = name;
            Points = points.AsReadOnly();
        }

        public string Name { get; internal set; }
        public ReadOnlyCollection<TeamCityMetricsSeriesPoint> Points { get; private set; }
    }

    public class TeamCityMetricsEventArgs : EventArgs
    {
        public TeamCityMetricsEventArgs(List<TeamCityMetricsSeries> series)
        {
            Series = series.AsReadOnly();
        }

        public ReadOnlyCollection<TeamCityMetricsSeries> Series { get; private set; }
    }

    public class TeamCityMetricsPresenter
    {
        private List<string> _uRLs;
        private string _userName;
        private string _password;

        private TeamCity.Model.ITeamCityMetricsDownload _metricsDownload;
        private PresenterCommon.ITimer _timer;
        
        public TeamCityMetricsPresenter(InformationRadiatorItemConfiguration configuration)
        {
            _uRLs = new List<string>();
            ParseConfiguration(configuration);

            _metricsDownload = TeamCityFactory.Instance.CreateMetricsDownload(_userName, _password);
            _timer = TeamCityFactory.Instance.CreateTimer(600000);

            _timer.Tick += _timer_Tick;
        }

        public bool AutoscaleGraphYAxis { get; private set; }
        public decimal GraphYAxisMax { get; private set; }

        public event EventHandler<TeamCityMetricsEventArgs> MetricsUpdated;
        protected void OnMetricsUpdated(List<TeamCityMetricsSeries> series)
        {
            var handler = MetricsUpdated;
            if (handler != null)
            {
                handler(this, new TeamCityMetricsEventArgs(series));
            }
        }

        public event EventHandler MetricsError;
        protected void OnMetricsError()
        {
            var handler = MetricsError;
            if(handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void SetDefaultConfiguration()
        {
            _userName = "guest";
            _password = "password";
            AutoscaleGraphYAxis = true;
        }

        private void ParseConfiguration(InformationRadiatorItemConfiguration configuration)
        {
            SetDefaultConfiguration();

            foreach (var item in configuration)
            {
                switch (item.ID.ToLower())
                {
                    case "url":
                        _uRLs.Add(item.Value);
                        break;
                    case "username":
                        _userName = item.Value;
                        break;
                    case "password":
                        _password = item.Value;
                        break;
                    case "maxy":
                        decimal value;
                        if (decimal.TryParse(item.Value, out value))
                        {
                            AutoscaleGraphYAxis = false;
                            GraphYAxisMax = value;
                        }
                        break;
                }
            }

        }

        private bool ParseLine(string line, ref string name, ref DateTime x, ref float y)
        {
            var columns = line.Split(',');

            if (!DateTime.TryParse(columns[2], out x))
                return false;

            if (!float.TryParse(columns[4], out y))
                return false;

            name = columns[3];

            return true;
        }

        private List<TeamCityMetricsSeries> ParseCSV(string csv)
        {
            var series = new Dictionary<string, List<TeamCityMetricsSeriesPoint>>();
            string[] spliters = {"\r\n"};
            var lines = csv.Split(spliters, StringSplitOptions.RemoveEmptyEntries);

            string name = null;
            DateTime x = DateTime.Now;
            float y = 0.0f;

            for (int i = 1; i < lines.Length; ++i)
            {
                if (!ParseLine(lines[i], ref name, ref x, ref y))
                    continue;

                if (!series.Keys.Contains(name))
                {
                    series.Add(name, new List<TeamCityMetricsSeriesPoint>());
                }

                series[name].Add(new TeamCityMetricsSeriesPoint(x, y));
            }

            var result = new List<TeamCityMetricsSeries>();
            foreach(var key in series.Keys)
            {
                result.Add(new TeamCityMetricsSeries(key, series[key]));
            }
            return result;
        }

        private void DownloadData(object notUsed)
        {
            try
            {
                lock (_metricsDownload)
                {
                    var metrics = new List<TeamCityMetricsSeries>();
                    foreach(var url in _uRLs)
                    {
                        var csv = _metricsDownload.DownloadMetrics(url);
                        metrics.AddRange(ParseCSV(csv));
                    }
                    OnMetricsUpdated(metrics);
                }
            }
            catch
            {
                OnMetricsError();
            }
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(DownloadData);
        }

        public void ForceUpdate()
        {
            _timer_Tick(this, EventArgs.Empty);
        }

    }
}
