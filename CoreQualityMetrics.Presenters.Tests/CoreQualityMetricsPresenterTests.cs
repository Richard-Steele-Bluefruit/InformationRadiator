using System;
using System.Threading;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PresenterCommon.Configuration;
using Moq;

namespace CoreQualityMetrics.Presenters.Tests
{
    [TestClass]
    public class CoreQualityMetricsPresenterTests
    {
        private Mock<CoreQualityMetricsFactory> mockFactory;
        private Mock<CoreQualityMetrics.Model.IWebsiteConnection> mockWebsite;
        private Mock<PresenterCommon.IDayUpdateMonitor> mockDayUpdateMonitor;

        [TestInitialize]
        public void Setup()
        {
            mockFactory = new Mock<CoreQualityMetricsFactory>(MockBehavior.Strict);
            CoreQualityMetrics.Presenters.CoreQualityMetricsFactory.Instance = mockFactory.Object;

            mockWebsite = new Mock<Model.IWebsiteConnection>(MockBehavior.Strict);

            mockDayUpdateMonitor = new Mock<PresenterCommon.IDayUpdateMonitor>(MockBehavior.Strict);
        }

        [TestCleanup]
        public void CleanUp()
        {
            CoreQualityMetrics.Presenters.CoreQualityMetricsFactory.Instance = null;

            mockFactory.VerifyAll();
            mockWebsite.VerifyAll();
            mockDayUpdateMonitor.VerifyAll();
        }

        [TestMethod]
        public void Specifying_the_website_configuration()
        {
            // Given
            var configuration = new InformationRadiatorItemConfiguration();
            configuration.Add(new InformationRadiatorItemConfigurationField { ID = "WebsiteURL", Value = "http://192.168.1.1" });

            mockFactory.Setup(m => m.CreateWebsiteConnection("http://192.168.1.1")).Returns(mockWebsite.Object);

            // When
            var target = new CoreQualityMetricsPresenter(configuration, mockDayUpdateMonitor.Object);

            // Then - mocks verified in the Cleanup()
        }

        [TestMethod]
        public void The_default_configuration_is_valid()
        {
            // Given
            var configuration = new InformationRadiatorItemConfiguration();

            mockFactory.Setup(m => m.CreateWebsiteConnection("http://localhost/")).Returns(mockWebsite.Object);

            // When
            var target = new CoreQualityMetricsPresenter(configuration, mockDayUpdateMonitor.Object);

            // Then - mocks verified in the Cleanup()
        }

#region Helper Functions

        private CoreQualityMetricsPresenter CreateConfiguredTarget()
        {
            var configuration = new InformationRadiatorItemConfiguration();
            configuration.Add(new InformationRadiatorItemConfigurationField { ID = "WebsiteURL", Value = "http://192.168.1.1" });

            mockFactory.Setup(m => m.CreateWebsiteConnection("http://192.168.1.1")).Returns(mockWebsite.Object);

            return new CoreQualityMetricsPresenter(configuration, mockDayUpdateMonitor.Object);
        }

        private void RaiseDayChanged()
        {
            mockDayUpdateMonitor.Raise(m => m.DayChanged += null, EventArgs.Empty);
        }

#endregion Helper Functions

        [TestMethod]
        public void When_the_day_changes_the_view_is_updated_with_the_latest_metrics()
        {
            // Given
            var target = CreateConfiguredTarget();

            mockWebsite.Setup(m => m.DownloadProjectsAndMetricsJson())
                .Returns("{\"projects\":{\"project_55ca71b5b2d02\":{\"name\":\"Cytronex\"},\"project_55ca71bac656f\":{\"name\":\"ELGA - Flex\"}},\"metrics\":{\"metric_55ca718c5ac18\":{\"name\":\"Regression Tested\",\"enabled\":true},\"metric_55ca7197b9771\":{\"name\":\"Release report\",\"enabled\":false}}}");

            mockWebsite.Setup(m => m.DownloadReleaseReportsListJson())
                .Returns("{\"project_55ca71b5b2d02\":[\"0.0.5\",\"0.0.7\"],\"project_55ca71bac656f\":[\"1.2.0\",\"1.3.0\"]}");

            mockWebsite.Setup(m => m.DownloadReleaseReportJson("project_55ca71b5b2d02", "0.0.5"))
                .Returns("{\"project_id\":\"project_55ca71b5b2d02\",\"version\":\"0.0.5\",\"date\":\"20150903\",\"metrics\":{\"metric_55ca718c5ac18\":true,\"metric_55ca7197b9771\":false}}");
            mockWebsite.Setup(m => m.DownloadReleaseReportJson("project_55ca71b5b2d02", "0.0.7"))
                .Returns("{\"project_id\":\"project_55ca71b5b2d02\",\"version\":\"0.0.5\",\"date\":\"20151005\",\"metrics\":{\"metric_55ca718c5ac18\":true,\"metric_55ca7197b9771\":false}}");

            mockWebsite.Setup(m => m.DownloadReleaseReportJson("project_55ca71bac656f", "1.2.0"))
                .Returns("{\"project_id\":\"project_55ca71bac656f\",\"version\":\"1.2.0\",\"date\":\"20151202\",\"metrics\":{\"metric_55ca718c5ac18\":true,\"metric_55ca7197b9771\":false}}");
            mockWebsite.Setup(m => m.DownloadReleaseReportJson("project_55ca71bac656f", "1.3.0"))
                .Returns("{\"project_id\":\"project_55ca71bac656f\",\"version\":\"1.3.0\",\"date\":\"20141203\",\"metrics\":{\"metric_55ca718c5ac18\":true,\"metric_55ca7197b9771\":false}}");


            var callbackHappened = new AutoResetEvent(false);
            QualityGraphEventArgs arguments = null;
            target.QualityGraphUpdate += (sender, e) =>
            {
                arguments = e;
                callbackHappened.Set();
            };

            // When
            RaiseDayChanged();

            // Then
            Assert.IsTrue(callbackHappened.WaitOne(5000), "Event was not raised");

            var graph = arguments.Graphs.FirstOrDefault(g => g.ProjectName == "Cytronex");
            Assert.IsNotNull(graph, "Unable to find \"Cytronex\" graphs in metrics data");

            Assert.AreEqual(new DateTime(2015, 09, 03), graph.Points[0].x);
            Assert.AreEqual(50, graph.Points[0].y);
            Assert.AreEqual(new DateTime(2015, 10, 05), graph.Points[1].x);
            Assert.AreEqual(50, graph.Points[1].y);

            graph = arguments.Graphs.FirstOrDefault(g => g.ProjectName == "ELGA - Flex");
            Assert.IsNotNull(graph, "Unable to find \"ELGA - Flex\" graphs in metrics data");

            Assert.AreEqual(new DateTime(2014, 12, 03), graph.Points[0].x);
            Assert.AreEqual(50, graph.Points[0].y);
            Assert.AreEqual(new DateTime(2015, 12, 02), graph.Points[1].x);
            Assert.AreEqual(50, graph.Points[1].y);

            // Mocks verified in the Cleanup
        }

        [TestMethod]
        public void An_exception_thrown_during_the_download_raises_the_ErrorDownloadData_event()
        {
            // Given
            var target = CreateConfiguredTarget();

            mockWebsite.Setup(m => m.DownloadProjectsAndMetricsJson()).Throws(new System.Net.WebException());


            var callbackHappened = new AutoResetEvent(false);
            target.ErrorDownloadingData += (sender, e) =>
            {
                callbackHappened.Set();
            };

            // When
            RaiseDayChanged();

            // Then
            Assert.IsTrue(callbackHappened.WaitOne(5000), "Event was not raised");

            // Mocks verified in the Cleanup
        }
    }
}
