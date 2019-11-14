using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

using Moq;
using PresenterCommon.Configuration;

namespace TeamCity.Presenters.Tests
{

    [TestClass]
    public class TeamCityMetricsPresenterTests
    {
        TeamCityFactoryMock _factory;
        Mock<TeamCity.Model.ITeamCityMetricsDownload> _mockDownload;
        Mock<PresenterCommon.ITimer> _mockTimer;


        #region Helper Methods

        [TestCleanup]
        public void CleanUp()
        {
            TeamCityFactory.Instance = null;
        }

        private InformationRadiatorItemConfiguration CreateConfiguration(string uRL = "Host", string userName = "User", string password = "password", string uRL2 = null)
        {
            var configuration = new InformationRadiatorItemConfiguration();
            configuration.Add(new InformationRadiatorItemConfigurationField() { ID = "URL", Value = uRL });
            if(uRL2 != null)
            {
                configuration.Add(new InformationRadiatorItemConfigurationField() { ID = "URL", Value = uRL2 });
            }
            configuration.Add(new InformationRadiatorItemConfigurationField() { ID = "UserName", Value = userName });
            configuration.Add(new InformationRadiatorItemConfigurationField() { ID = "Password", Value = password });
            return configuration;
        }

        private void CreateMockFactory()
        {
            _factory = new TeamCityFactoryMock();
            TeamCityFactory.Instance = _factory;

            _mockDownload = new Mock<Model.ITeamCityMetricsDownload>();
            _factory._metricsDownload = _mockDownload.Object;

            _mockTimer = new Mock<PresenterCommon.ITimer>(MockBehavior.Strict);
            _factory._timer = _mockTimer.Object;
        }

        #endregion Helper Methods

        #region Example metric results

        private string SingleMetricResult
        {
            get
            {
                return @"Build number,Build status,Start time,Series,Value" + EOL +
                       @"4,SUCCESS,2015-11-12 09:59,OpenCppCoverage,82.352941" + EOL +
                       @"5,FAILURE,2015-11-12 10:02,OpenCppCoverage,82.352941" + EOL +
                       @"6,FAILURE,2015-11-12 10:08,OpenCppCoverage,82.352941" + EOL +
                       @"7,FAILURE,2015-11-12 10:11,OpenCppCoverage,51.851852" + EOL +
                       @"8,FAILURE,2015-11-12 10:14,OpenCppCoverage,82.352941" + EOL +
                       @"9,FAILURE,2015-11-12 10:24,OpenCppCoverage,82.352941" + EOL +
                       @"10,FAILURE,2015-11-12 10:44,OpenCppCoverage,82.352941" + EOL;
            }
        }

        private string MultiMetricResult
        {
            get
            {
                return @"Build number,Build status,Start time,Series,Value" + EOL +
                       @"3,SUCCESS,2015-11-12 09:21,Average_Complexity,1.000000" + EOL +
                       @"3,SUCCESS,2015-11-12 09:21,Maximum_Complexity,1.000000" + EOL +
                       @"3,SUCCESS,2015-11-12 09:21,Average_Block_Depth,0.430000" + EOL +
                       @"3,SUCCESS,2015-11-12 09:21,Maximum_Block_Depth,1.000000" + EOL +
                       @"6,FAILURE,2015-11-12 10:08,Average_Complexity,1.000000" + EOL +
                       @"6,FAILURE,2015-11-12 10:08,Maximum_Complexity,1.000000" + EOL +
                       @"6,FAILURE,2015-11-12 10:08,Average_Block_Depth,0.430000" + EOL +
                       @"6,FAILURE,2015-11-12 10:08,Maximum_Block_Depth,1.000000" + EOL +
                       @"7,FAILURE,2015-11-12 10:11,Average_Complexity,1.000000" + EOL +
                       @"7,FAILURE,2015-11-12 10:11,Maximum_Complexity,1.000000" + EOL +
                       @"7,FAILURE,2015-11-12 10:11,Average_Block_Depth,0.670000" + EOL +
                       @"7,FAILURE,2015-11-12 10:11,Maximum_Block_Depth,1.000000" + EOL +
                       @"8,FAILURE,2015-11-12 10:14,Average_Complexity,1.000000" + EOL +
                       @"8,FAILURE,2015-11-12 10:14,Maximum_Complexity,1.000000" + EOL +
                       @"8,FAILURE,2015-11-12 10:14,Average_Block_Depth,0.430000" + EOL +
                       @"8,FAILURE,2015-11-12 10:14,Maximum_Block_Depth,1.000000" + EOL +
                       @"9,FAILURE,2015-11-12 10:24,Average_Complexity,1.000000" + EOL +
                       @"9,FAILURE,2015-11-12 10:24,Maximum_Complexity,1.000000" + EOL +
                       @"9,FAILURE,2015-11-12 10:24,Average_Block_Depth,0.430000" + EOL +
                       @"9,FAILURE,2015-11-12 10:24,Maximum_Block_Depth,1.000000" + EOL +
                       @"10,FAILURE,2015-11-12 10:44,Average_Complexity,1.000000" + EOL +
                       @"10,FAILURE,2015-11-12 10:44,Maximum_Complexity,1.000000" + EOL +
                       @"10,FAILURE,2015-11-12 10:44,Average_Block_Depth,0.430000" + EOL +
                       @"10,FAILURE,2015-11-12 10:44,Maximum_Block_Depth,1.000000" + EOL;
            }
        }

        private readonly string EOL = "\r\n";

        private string MultiMetricResult2
        {
            get
            {
                return @"Build number,Build status,Start time,Coverage level,Value" + EOL +
                       @"6,SUCCESS,2015-12-08 12:37,Line,97.2332000732421875" + EOL +
                       @"6,SUCCESS,2015-12-08 12:37,Method,93.02326202392578125" + EOL +
                       @"6,SUCCESS,2015-12-08 12:37,Class,86.36363983154296875" + EOL +
                       @"7,FAILURE,2015-12-08 15:03,Line,97.2332000732421875" + EOL +
                       @"7,FAILURE,2015-12-08 15:03,Method,93.02326202392578125" + EOL +
                       @"7,FAILURE,2015-12-08 15:03,Class,86.36363983154296875" + EOL +
                       @"8,SUCCESS,2015-12-08 15:04,Line,97.2332000732421875" + EOL +
                       @"8,SUCCESS,2015-12-08 15:04,Method,93.02326202392578125" + EOL +
                       @"8,SUCCESS,2015-12-08 15:04,Class,86.36363983154296875" + EOL +
                       @"9,SUCCESS,2015-12-18 15:20,Line,97.2332000732421875" + EOL +
                       @"9,SUCCESS,2015-12-18 15:20,Method,93.02326202392578125" + EOL +
                       @"9,SUCCESS,2015-12-18 15:20,Class,86.36363983154296875" + EOL;
            }
        }

        #endregion Example metric results

        [TestMethod]
        public void Setting_the_server_and_login_details_via_the_configuration()
        {
            // Given
            string expectedUserName = "byran";
            string expectedPassword = "usualPassword";
            var configuration = CreateConfiguration("http://boo", expectedUserName, expectedPassword);
            CreateMockFactory();

            // When
            var target = new TeamCityMetricsPresenter(configuration);

            // Then
            Assert.AreEqual(expectedUserName, _factory._metricsUserName);
            Assert.AreEqual(expectedPassword, _factory._metricsPassword);
            Assert.AreEqual(10 * 60 * 1000, _factory._interval);
        }

        [TestMethod]
        public void No_server_and_login_details_in_the_configuration()
        {
            // Given
            string expectedUserName = "guest";
            string expectedPassword = "password";
            var configuration = new InformationRadiatorItemConfiguration();
            CreateMockFactory();

            // When
            var target = new TeamCityMetricsPresenter(configuration);

            // Then
            Assert.AreEqual(expectedUserName, _factory._metricsUserName);
            Assert.AreEqual(expectedPassword, _factory._metricsPassword);
            Assert.AreEqual(10 * 60 * 1000, _factory._interval);
        }

        [TestMethod]
        public void If_no_max_graph_y_is_specified_then_the_presenter_indicates_the_graph_should_auto_scale()
        {
            // Given
            var configuration = new InformationRadiatorItemConfiguration();
            CreateMockFactory();

            // When
            var target = new TeamCityMetricsPresenter(configuration);

            // Then
            Assert.IsTrue(target.AutoscaleGraphYAxis);
        }

        [TestMethod]
        public void If_a_max_graph_y_is_specified_then_the_presenter_indicates_the_graph_should_not_auto_scale_and_reports_the_required_may_y()
        {
            // Given
            var configuration = new InformationRadiatorItemConfiguration();
            configuration.Add(new InformationRadiatorItemConfigurationField() {ID = "MaxY", Value = "10"});
            CreateMockFactory();

            // When
            var target = new TeamCityMetricsPresenter(configuration);

            // Then
            Assert.IsFalse(target.AutoscaleGraphYAxis);
            Assert.AreEqual(10m, target.GraphYAxisMax);
        }

        [TestMethod]
        public void If_an_invalid_max_graph_y_is_specified_then_the_presenter_indicates_the_graph_should_auto_scale()
        {
            // Given
            var configuration = new InformationRadiatorItemConfiguration();
            configuration.Add(new InformationRadiatorItemConfigurationField() { ID = "MaxY", Value = "1x0" });
            CreateMockFactory();

            // When
            var target = new TeamCityMetricsPresenter(configuration);

            // Then
            Assert.IsTrue(target.AutoscaleGraphYAxis);
        }

        [TestMethod]
        public void The_view_is_updated_when_the_timer_fires_and_a_single_data_series_is_downloaded()
        {
            // Given
            const string uRL = @"http://hello.world/boo";
            CreateMockFactory();
            var target = new TeamCityMetricsPresenter(CreateConfiguration(uRL: uRL));

            _mockDownload.Setup(m => m.DownloadMetrics(uRL))
                .Returns(SingleMetricResult);

            TeamCityMetricsEventArgs actualArguments = null;
            var wait = new System.Threading.AutoResetEvent(false);
            target.MetricsUpdated += (s, e) => {
                actualArguments = e;
                wait.Set();
            };

            // When
            _mockTimer.Raise(m => m.Tick += null, _mockTimer.Object, EventArgs.Empty);

            // Then
            Assert.IsTrue(wait.WaitOne(5000), "No event raised when the timer fired");

            Assert.AreEqual(1, actualArguments.Series.Count);
            var series = actualArguments.Series[0];
            Assert.AreEqual("OpenCppCoverage", series.Name);
            Assert.AreEqual(7, series.Points.Count);

            Assert.AreEqual(new DateTime(2015, 11, 12, 9, 59, 0), series.Points[0].X);
            Assert.AreEqual(82.352941, series.Points[0].Y, 0.00001);

            Assert.AreEqual(new DateTime(2015, 11, 12, 10, 2, 0), series.Points[1].X);
            Assert.AreEqual(82.352941, series.Points[1].Y, 0.00001);

            Assert.AreEqual(new DateTime(2015, 11, 12, 10, 11, 0), series.Points[3].X);
            Assert.AreEqual(51.851852, series.Points[3].Y, 0.00001);

            _mockDownload.VerifyAll();
            _mockTimer.VerifyAll();
        }

        [TestMethod]
        public void The_view_is_updated_when_the_timer_fires_and_multiple_data_series_are_downloaded()
        {
            // Given
            const string uRL = @"http://hello.world/boo";
            CreateMockFactory();
            var target = new TeamCityMetricsPresenter(CreateConfiguration(uRL: uRL));

            _mockDownload.Setup(m => m.DownloadMetrics(uRL))
                .Returns(MultiMetricResult);

            TeamCityMetricsEventArgs actualArguments = null;
            var wait = new System.Threading.AutoResetEvent(false);
            target.MetricsUpdated += (s, e) =>
            {
                actualArguments = e;
                wait.Set();
            };

            // When
            _mockTimer.Raise(m => m.Tick += null, _mockTimer.Object, EventArgs.Empty);

            // Then
            Assert.IsTrue(wait.WaitOne(5000), "No event raised when the timer fired");

            Assert.AreEqual(4, actualArguments.Series.Count);

            // Check the Average_Complexity series
            var series = actualArguments.Series.First(s => s.Name == "Average_Complexity");
            Assert.AreEqual(6, series.Points.Count);

            Assert.AreEqual(new DateTime(2015, 11, 12, 9, 21, 0), series.Points[0].X);
            Assert.AreEqual(1, series.Points[0].Y, 0.00001);

            Assert.AreEqual(new DateTime(2015, 11, 12, 10, 8, 0), series.Points[1].X);
            Assert.AreEqual(1, series.Points[1].Y, 0.00001);

            // Check the Maximum_Complexity series
            series = actualArguments.Series.First(s => s.Name == "Maximum_Complexity");
            Assert.AreEqual(6, series.Points.Count);

            Assert.AreEqual(new DateTime(2015, 11, 12, 9, 21, 0), series.Points[0].X);
            Assert.AreEqual(1, series.Points[0].Y, 0.00001);

            Assert.AreEqual(new DateTime(2015, 11, 12, 10, 8, 0), series.Points[1].X);
            Assert.AreEqual(1, series.Points[1].Y, 0.00001);

            // Check the Average_Block_Depth series
            series = actualArguments.Series.First(s => s.Name == "Average_Block_Depth");
            Assert.AreEqual(6, series.Points.Count);

            Assert.AreEqual(new DateTime(2015, 11, 12, 9, 21, 0), series.Points[0].X);
            Assert.AreEqual(0.43, series.Points[0].Y, 0.00001);

            Assert.AreEqual(new DateTime(2015, 11, 12, 10, 8, 0), series.Points[1].X);
            Assert.AreEqual(0.43, series.Points[1].Y, 0.00001);

            // Check the Maximum_Block_Depth series
            series = actualArguments.Series.First(s => s.Name == "Maximum_Block_Depth");
            Assert.AreEqual(6, series.Points.Count);

            Assert.AreEqual(new DateTime(2015, 11, 12, 9, 21, 0), series.Points[0].X);
            Assert.AreEqual(1, series.Points[0].Y, 0.00001);

            Assert.AreEqual(new DateTime(2015, 11, 12, 10, 8, 0), series.Points[1].X);
            Assert.AreEqual(1, series.Points[1].Y, 0.00001);

            _mockDownload.VerifyAll();
            _mockTimer.VerifyAll();
        }

        [TestMethod]
        public void The_view_is_updated_when_the_timer_fires_and_no_data_is_downloaded()
        {
            // Given
            const string uRL = @"http://hello.world/boo";
            CreateMockFactory();
            var target = new TeamCityMetricsPresenter(CreateConfiguration(uRL: uRL));

            _mockDownload.Setup(m => m.DownloadMetrics(uRL))
                .Returns("");

            TeamCityMetricsEventArgs actualArguments = null;
            var wait = new System.Threading.AutoResetEvent(false);
            target.MetricsUpdated += (s, e) =>
            {
                actualArguments = e;
                wait.Set();
            };

            // When
            _mockTimer.Raise(m => m.Tick += null, _mockTimer.Object, EventArgs.Empty);

            // Then
            Assert.IsTrue(wait.WaitOne(5000), "No event raised when the timer fired");

            Assert.AreEqual(0, actualArguments.Series.Count);

            _mockDownload.VerifyAll();
            _mockTimer.VerifyAll();
        }

        [TestMethod]
        public void The_view_is_updated_when_the_timer_fires_and_invalid_data_is_downloaded()
        {
            // Given
            const string uRL = @"http://hello.world/boo";
            CreateMockFactory();
            var target = new TeamCityMetricsPresenter(CreateConfiguration(uRL: uRL));

            _mockDownload.Setup(m => m.DownloadMetrics(uRL))
                .Returns(@"Build number,Build status,Start time,Series,Value
4,SUCCESS,2015-11-12 09:59,OpenCppCoverage");

            var wait = new System.Threading.AutoResetEvent(false);
            target.MetricsError += (s, e) =>
            {
                wait.Set();
            };

            // When
            _mockTimer.Raise(m => m.Tick += null, _mockTimer.Object, EventArgs.Empty);

            // Then
            Assert.IsTrue(wait.WaitOne(5000), "No event raised when the timer fired");

            _mockDownload.VerifyAll();
            _mockTimer.VerifyAll();
        }


        [TestMethod]
        public void The_view_is_updated_when_the_timer_fires_and_multiple_data_series_are_downloaded_from_multiple_urls()
        {
            // Given
            const string uRL = @"http://hello.world/boo";
            const string uRL2 = @"http://hello.world/hoo";
            CreateMockFactory();
            var target = new TeamCityMetricsPresenter(CreateConfiguration(uRL: uRL, uRL2: uRL2));

            _mockDownload.Setup(m => m.DownloadMetrics(uRL))
                .Returns(MultiMetricResult);

            _mockDownload.Setup(m => m.DownloadMetrics(uRL2))
                .Returns(MultiMetricResult2);

            TeamCityMetricsEventArgs actualArguments = null;
            var wait = new System.Threading.AutoResetEvent(false);
            target.MetricsUpdated += (s, e) =>
            {
                actualArguments = e;
                wait.Set();
            };

            // When
            _mockTimer.Raise(m => m.Tick += null, _mockTimer.Object, EventArgs.Empty);

            // Then
            Assert.IsTrue(wait.WaitOne(5000), "No event raised when the timer fired");

            Assert.AreEqual(7, actualArguments.Series.Count);

            // Check the Average_Complexity series
            var series = actualArguments.Series.First(s => s.Name == "Average_Complexity");
            Assert.AreEqual(6, series.Points.Count);

            // Check the Maximum_Complexity series
            series = actualArguments.Series.First(s => s.Name == "Maximum_Complexity");
            Assert.AreEqual(6, series.Points.Count);

            // Check the Average_Block_Depth series
            series = actualArguments.Series.First(s => s.Name == "Average_Block_Depth");
            Assert.AreEqual(6, series.Points.Count);

            // Check the Maximum_Block_Depth series
            series = actualArguments.Series.First(s => s.Name == "Maximum_Block_Depth");
            Assert.AreEqual(6, series.Points.Count);

            // Check the Line series
            series = actualArguments.Series.First(s => s.Name == "Line");
            Assert.AreEqual(4, series.Points.Count);

            // Check the Method series
            series = actualArguments.Series.First(s => s.Name == "Method");
            Assert.AreEqual(4, series.Points.Count);

            // Check the Class series
            series = actualArguments.Series.First(s => s.Name == "Class");
            Assert.AreEqual(4, series.Points.Count);
            
            _mockDownload.VerifyAll();
            _mockTimer.VerifyAll();
        }
    }
}
