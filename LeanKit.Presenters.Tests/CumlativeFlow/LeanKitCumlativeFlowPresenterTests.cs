using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Drawing;

using Moq;
using PresenterCommon.Configuration;

using LeanKit.API.Client.Library.TransferObjects;
using LeanKit.Model.LaneHistory;

using LeanKit.Presenters.CumlativeFlow;

namespace LeanKit.Presenters.Tests.CumlativeFlow
{
    [TestClass]
    public class LeanKitCumlativeFlowPresenterTests
    {
        private LeanKitFactoryMock mockFactory;
        private Mock<ILeanKitLanePointsHistory> mockHistory;
        private Mock<PresenterCommon.IDayUpdateMonitor> mockDayUpdateMonitor;

        [TestInitialize]
        public void Setup()
        {
            mockFactory = new LeanKitFactoryMock();
            LeanKitFactory.Instance = mockFactory;

            mockHistory = new Mock<ILeanKitLanePointsHistory>(MockBehavior.Strict);
            mockFactory._laneHistory = mockHistory.Object;

            mockDayUpdateMonitor = new Mock<PresenterCommon.IDayUpdateMonitor>(MockBehavior.Strict);
        }

        [TestCleanup]
        public void CleanUp()
        {
            LeanKitFactory.Instance = null;

            mockHistory.VerifyAll();
            mockDayUpdateMonitor.VerifyAll();
        }

        #region Helper Functions

        private InformationRadiatorItemConfiguration CreateDefaultConfiguration(long boardId, int numberOfDays = 10)
        {
            var configuration = new InformationRadiatorItemConfiguration();
            configuration.Add(new InformationRadiatorItemConfigurationField { ID = "HostName", Value = "absw" });
            configuration.Add(new InformationRadiatorItemConfigurationField { ID = "UserName", Value = "ir" });
            configuration.Add(new InformationRadiatorItemConfigurationField { ID = "Password", Value = "password" });
            configuration.Add(new InformationRadiatorItemConfigurationField { ID = "BoardId", Value = boardId.ToString() });
            configuration.Add(new InformationRadiatorItemConfigurationField { ID = "NumberOfDays", Value = numberOfDays.ToString() });
            return configuration;
        }

        #endregion Helper Functions


        [TestMethod]
        public void Specifying_the_server_configuration()
        {
            // Given
            string expectedHostName = "absw";
            string expectedUserName = "informationRadiator";
            string expectedPassword = "passWord";
            long expectedBoardId = 152;

            var configuration = new InformationRadiatorItemConfiguration();
            configuration.Add(new InformationRadiatorItemConfigurationField { ID = "HostName", Value = expectedHostName });
            configuration.Add(new InformationRadiatorItemConfigurationField { ID = "UserName", Value = expectedUserName });
            configuration.Add(new InformationRadiatorItemConfigurationField { ID = "Password", Value = expectedPassword });
            configuration.Add(new InformationRadiatorItemConfigurationField { ID = "BoardId", Value = expectedBoardId.ToString() });
            configuration.Add(new InformationRadiatorItemConfigurationField { ID = "IgnoreLaneID", Value = "356" });
            configuration.Add(new InformationRadiatorItemConfigurationField { ID = "IgnoreLaneID", Value = "879" });

            // When
            var target = new LeanKitCumlativeFlowPresenter(configuration, mockDayUpdateMonitor.Object);

            // Then
            Assert.AreEqual(expectedHostName, mockFactory._historyHostName);
            Assert.AreEqual(expectedUserName, mockFactory._historyUserName);
            Assert.AreEqual(expectedPassword, mockFactory._historyPassword);
            Assert.AreEqual(expectedBoardId, mockFactory._historyBoardId);

            Assert.AreEqual(2, mockFactory._ignoredLanes.Count);
            Assert.IsTrue(mockFactory._ignoredLanes.Exists(p => p == 356));
            Assert.IsTrue(mockFactory._ignoredLanes.Exists(p => p == 879));
        }

        private List<LanePointsHistory> SetupUpdateHistory(int numberOfDays)
        {
            var laneHistory = new List<LanePointsHistory>();
            mockHistory.Setup(m => m.Update(numberOfDays));
            mockHistory.Setup(m => m.LaneHistory).Returns(laneHistory.AsReadOnly());
            return laneHistory;
        }

        [TestMethod]
        public void Updating_the_CumlativeFlow()
        {
            // Given
            var configuration = CreateDefaultConfiguration(100);
            var target = new LeanKitCumlativeFlowPresenter(configuration, mockDayUpdateMonitor.Object);

            var laneHistory = SetupUpdateHistory(10);
            laneHistory.Add(new LanePointsHistory { Id = 3, Title = "Simple", PointsPerDay = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, Type = TypeOfLane.InProcess });
            laneHistory.Add(new LanePointsHistory { Id = 5, Title = "Second", PointsPerDay = new List<int> { 20, 30, 40, 50, 60, 70, 80, 90, 100, 110 }, Type = TypeOfLane.Ready });

            AutoResetEvent wait = new AutoResetEvent(false);
            List<CumlativeFlowLaneData> actualLanes = null;
            target.CumaltiveFlowDataUpdate += (o, e) =>
            {
                actualLanes = e.Lanes;
                wait.Set();
            };

            // When
            mockDayUpdateMonitor.Raise(m => m.DayChanged += null, EventArgs.Empty);
            wait.WaitOne(5000);

            // Then
            var lane = actualLanes.First(o => o.Title == "Simple"); // This throws if the element cannot be found
            Assert.AreEqual(10, lane.PointsPerDay.Count);
            Assert.AreEqual(CumlativeFlowLaneType.InProcess, lane.Type);
            int expected = 10;
            foreach(var point in lane.PointsPerDay)
            {
                Assert.AreEqual(expected, point);
                expected--;
            }

            lane = actualLanes.First(o => o.Title == "Second"); // This throws if the element cannot be found
            Assert.AreEqual(10, lane.PointsPerDay.Count);
            Assert.AreEqual(CumlativeFlowLaneType.Ready, lane.Type);
            expected = 110;
            foreach (var point in lane.PointsPerDay)
            {
                Assert.AreEqual(expected, point);
                expected-=10;
            }
        }

        [TestMethod]
        public void Configuring_the_Number_of_days()
        {
            // Given
            var configuration = CreateDefaultConfiguration(100, 20);
            var target = new LeanKitCumlativeFlowPresenter(configuration, mockDayUpdateMonitor.Object);

            var laneHistory = SetupUpdateHistory(20);
            AutoResetEvent wait = new AutoResetEvent(false);
            target.CumaltiveFlowDataUpdate += (o, e) =>
            {
                wait.Set();
            };

            // When
            mockDayUpdateMonitor.Raise(m => m.DayChanged += null, EventArgs.Empty);
            wait.WaitOne(5000);

            // Then, the mock will check if the correct number of days has been requested
        }

        [TestMethod]
        public void Configuring_the_lane_type_colour_ranges()
        {
            // Given
            var configuration = CreateDefaultConfiguration(100, 10);
            configuration.Add(new InformationRadiatorItemConfigurationField() { ID = "ReadyStartColour", Value = "FF0000"});
            configuration.Add(new InformationRadiatorItemConfigurationField() { ID = "ReadyEndColour", Value = "0F0000" });
            configuration.Add(new InformationRadiatorItemConfigurationField() { ID = "InProcessStartColour", Value = "00FF00" });
            configuration.Add(new InformationRadiatorItemConfigurationField() { ID = "InProcessEndColour", Value = "000F00" });
            configuration.Add(new InformationRadiatorItemConfigurationField() { ID = "CompleteStartColour", Value = "0000FF" });
            configuration.Add(new InformationRadiatorItemConfigurationField() { ID = "CompleteEndColour", Value = "00000F" });

            var target = new LeanKitCumlativeFlowPresenter(configuration, mockDayUpdateMonitor.Object);

            CumaltiveFlowDataUpdateEventArgs eventArguments = null;
            SetupUpdateHistory(10);
            AutoResetEvent wait = new AutoResetEvent(false);
            target.CumaltiveFlowDataUpdate += (o, e) =>
            {
                eventArguments = e;
                wait.Set();
            };

            // When
            mockDayUpdateMonitor.Raise(m => m.DayChanged += null, EventArgs.Empty);
            Assert.IsTrue(wait.WaitOne(5000));
            
            // Then
            Assert.AreEqual(Color.FromArgb(0xFF, 0x00, 0x00), eventArguments.ReadyStartColour);
            Assert.AreEqual(Color.FromArgb(0x0F, 0x00, 0x00), eventArguments.ReadyEndColour);
            Assert.AreEqual(Color.FromArgb(0x00, 0xFF, 0x00), eventArguments.InProcessStartColour);
            Assert.AreEqual(Color.FromArgb(0x00, 0x0F, 0x00), eventArguments.InProcessEndColour);
            Assert.AreEqual(Color.FromArgb(0x00, 0x00, 0xFF), eventArguments.CompleteStartColour);
            Assert.AreEqual(Color.FromArgb(0x00, 0x00, 0x0F), eventArguments.CompleteEndColour);
        }

        [TestMethod]
        public void There_are_default_lane_type_colour_ranges_if_they_are_not_specified_in_the_configuration()
        {
            // Given
            var configuration = CreateDefaultConfiguration(100, 10);
            var target = new LeanKitCumlativeFlowPresenter(configuration, mockDayUpdateMonitor.Object);

            CumaltiveFlowDataUpdateEventArgs eventArguments = null;
            SetupUpdateHistory(10);
            AutoResetEvent wait = new AutoResetEvent(false);
            target.CumaltiveFlowDataUpdate += (o, e) =>
            {
                eventArguments = e;
                wait.Set();
            };

            // When
            mockDayUpdateMonitor.Raise(m => m.DayChanged += null, EventArgs.Empty);
            Assert.IsTrue(wait.WaitOne(5000));

            // Then
            Assert.AreEqual(Color.FromArgb(200, 0x00, 0x00), eventArguments.ReadyStartColour);
            Assert.AreEqual(Color.FromArgb(150, 0x00, 0x00), eventArguments.ReadyEndColour);
            Assert.AreEqual(Color.FromArgb(0x00, 0x00, 200), eventArguments.InProcessStartColour);
            Assert.AreEqual(Color.FromArgb(0x00, 0x00, 150), eventArguments.InProcessEndColour);
            Assert.AreEqual(Color.FromArgb(0x00, 200, 0x00), eventArguments.CompleteStartColour);
            Assert.AreEqual(Color.FromArgb(0x00, 150, 0x00), eventArguments.CompleteEndColour);
        }

    }
}
