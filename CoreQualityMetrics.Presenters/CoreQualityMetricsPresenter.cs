using System;
using System.Collections.Generic;
using System.Linq;
using PresenterCommon.Configuration;

namespace CoreQualityMetrics.Presenters
{
    public class CoreQualityMetricsPresenter
    {
        private string _websiteURL;
        private CoreQualityMetrics.Model.IWebsiteConnection _webConnection;

        public event EventHandler<QualityGraphEventArgs> QualityGraphUpdate;

        protected void OnQualityGraphUpdate(QualityGraphEventArgs args)
        {
            EventHandler<QualityGraphEventArgs> ev = QualityGraphUpdate;
            if(ev != null)
            {
                ev(this, args);
            }
        }

        public event EventHandler ErrorDownloadingData;

        protected void OnErrorDownloadingData()
        {
            EventHandler ev = ErrorDownloadingData;
            if (ev != null)
            {
                ev(this, EventArgs.Empty);
            }
        }

        public CoreQualityMetricsPresenter(InformationRadiatorItemConfiguration configuration, PresenterCommon.IDayUpdateMonitor updateMonitor)
        {
            ParseConfiguration(configuration);
            _webConnection = CoreQualityMetricsFactory.Instance.CreateWebsiteConnection(_websiteURL);

            updateMonitor.DayChanged += updateMonitor_DayChanged;
        }

        private void SetDefaultConfiguration()
        {
            _websiteURL = "http://localhost/";
            //_websiteURL = "http://";
        }

        private void ParseConfiguration(InformationRadiatorItemConfiguration configuration)
        {
            SetDefaultConfiguration();

            foreach (var item in configuration)
            {
                switch (item.ID.ToLower())
                {
                    case "websiteurl":
                        _websiteURL = item.Value;
                        break;
                }
            }
        }

        private QualityGraph CreateProjectQualityGraph(
            CoreQualityMetrics.Model.ProjectsAndMetrics projectsAndMetrics,
            CoreQualityMetrics.Model.ListReleaseReports releaseReports, string project)
        {
            var points = new List<QualityGraphPoint>();
            foreach (var version in releaseReports.ReleaseReports[project])
            {
                var report = new CoreQualityMetrics.Model.ReleaseReport(
                    _webConnection.DownloadReleaseReportJson(project, version),
                    projectsAndMetrics.Metrics);

                int passingMetrics = report.BooleanMetricResults.Count(r => r.Value);
                int totalMetrics = projectsAndMetrics.Metrics.Count;

                points.Add(new QualityGraphPoint(report.Date, (passingMetrics * 100) / totalMetrics));
            }

            points.Sort((a, b) => a.x.CompareTo(b.x));
            return new QualityGraph(projectsAndMetrics.Projects[project].name, points);
        }

        private void Update(object notUsed)
        {
            try
            {
                var projectsAndMetrics = new CoreQualityMetrics.Model.ProjectsAndMetrics(
                    _webConnection.DownloadProjectsAndMetricsJson());

                var releaseReports = new CoreQualityMetrics.Model.ListReleaseReports(
                    _webConnection.DownloadReleaseReportsListJson());

                var graphs = new List<QualityGraph>();

                foreach (var project in releaseReports.ReleaseReports.Keys)
                {
                    graphs.Add(CreateProjectQualityGraph(projectsAndMetrics, releaseReports, project));
                }

                OnQualityGraphUpdate(new QualityGraphEventArgs(graphs));
            }
            catch
            {
                OnErrorDownloadingData();
            }
        }

        private void updateMonitor_DayChanged(object sender, EventArgs e)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(Update);
        }

        public void ManualUpdate()
        {
            updateMonitor_DayChanged(this, EventArgs.Empty);
        }
    }
}
