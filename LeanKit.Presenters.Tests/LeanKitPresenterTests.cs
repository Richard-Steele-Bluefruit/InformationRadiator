using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using PresenterCommon.Configuration;

using LeanKit.API.Client.Library.TransferObjects;
using LeanKit.Model;


namespace LeanKit.Presenters.Tests
{
    [TestClass]
    public class LeanKitPresenterTests
    {
        private LeanKitFactoryMock mockFactory;
        private Mock<ILeanKitPoints> mockPoints;
        private Mock<PresenterCommon.ITimer> mockTimer;

        [TestInitialize]
        public void Setup()
        {
            mockFactory = new LeanKitFactoryMock();
            LeanKitFactory.Instance = mockFactory;

            mockPoints = new Mock<ILeanKitPoints>(MockBehavior.Strict);
            mockFactory._points = mockPoints.Object;

            mockTimer = new Mock<PresenterCommon.ITimer>(MockBehavior.Strict);
            mockFactory._timer = mockTimer.Object;
        }

        [TestCleanup]
        public void CleanUp()
        {
            LeanKitFactory.Instance = null;

            mockPoints.VerifyAll();
            mockTimer.VerifyAll();
        }

        [TestMethod]
        public void Specifying_the_server_configuration()
        {
            // Given
            string expectedHostName = "absw";
            string expectedUserName = "informationRadiator";
            string expectedPassword = "passWord";

            var configuration = new InformationRadiatorItemConfiguration();
            configuration.Add(new InformationRadiatorItemConfigurationField { ID = "HostName", Value = expectedHostName });
            configuration.Add(new InformationRadiatorItemConfigurationField { ID = "UserName", Value = expectedUserName });
            configuration.Add(new InformationRadiatorItemConfigurationField { ID = "Password", Value = expectedPassword });

            // When
            var target = new LeanKitPresenter(configuration);

            // Then
            Assert.AreEqual(expectedHostName, mockFactory._pointsHostName);
            Assert.AreEqual(expectedUserName, mockFactory._pointsUserName);
            Assert.AreEqual(expectedPassword, mockFactory._pointsPassword);
        }

        private InformationRadiatorItemConfiguration CreateDefaultConfiguration(long boardId)
        {
            var configuration = new InformationRadiatorItemConfiguration();
            configuration.Add(new InformationRadiatorItemConfigurationField { ID = "HostName", Value = "absw" });
            configuration.Add(new InformationRadiatorItemConfigurationField { ID = "UserName", Value = "ir" });
            configuration.Add(new InformationRadiatorItemConfigurationField { ID = "Password", Value = "password" });
            configuration.Add(new InformationRadiatorItemConfigurationField { ID = "BoardId", Value = boardId.ToString() });
            return configuration;
        }

        [TestMethod]
        public void Getting_an_update_of_progress_of_a_board()
        {
            // Given
            long expectedBoardId = 100;
            var configuration = CreateDefaultConfiguration(expectedBoardId);

            mockPoints.Setup(m => m.Update(expectedBoardId, It.Is<IList<long>>(il => il.Count == 0)));
            mockPoints.Setup(m => m.ReadyPoints).Returns(5);
            mockPoints.Setup(m => m.InProgressPoints).Returns(3);
            mockPoints.Setup(m => m.CompletePoints).Returns(11);
            mockPoints.Setup(m => m.UntypedPoints).Returns(150);

            var target = new LeanKitPresenter(configuration);

            int actualReadyPoints = 0;
            int actualInProgressPoints = 0;
            int actualCompletePoints = 0;
            int actualUntypedPoints = 0;
            target.ProgressUpdate += (sender, e) =>
                {
                    actualReadyPoints = e.ReadyPoints;
                    actualInProgressPoints = e.InProgressPoints;
                    actualCompletePoints = e.CompletePoints;
                    actualUntypedPoints = e.UntypedPoints;
                };

            // When
            mockTimer.Raise(o => o.Tick += null, EventArgs.Empty);

            // Then
            Assert.AreEqual(60000, mockFactory._interval, 0.1);
            Assert.AreEqual(5, actualReadyPoints);
            Assert.AreEqual(3, actualInProgressPoints);
            Assert.AreEqual(11, actualCompletePoints);
            Assert.AreEqual(150, actualUntypedPoints);
        }

        [TestMethod]
        public void Filtering_lanes_that_are_not_required()
        {
            // Given
            long expectedBoardId = 100;
            var configuration = CreateDefaultConfiguration(expectedBoardId);

            // Add the lanes to be ignored
            configuration.Add(new InformationRadiatorItemConfigurationField() { ID = "IgnoreLaneID", Value = "4" });
            configuration.Add(new InformationRadiatorItemConfigurationField() { ID = "IgnoreLaneID", Value = "8" });

            var expectedIgnoreList = new List<long>() { 4, 8 };

            Func<IList<long>, bool> checkIgnoreList = (il) =>
            {
                foreach (long search in expectedIgnoreList)
                {
                    if (expectedIgnoreList.Count(v => v == search) != il.Count<long>(v => v == search))
                        return false;
                }
                return true;
            };

            mockPoints.Setup(m => m.Update(expectedBoardId, It.Is<IList<long>>(il => checkIgnoreList(il))));

            // These need to be setup but the results will not be checked
            mockPoints.Setup(m => m.ReadyPoints).Returns(3);
            mockPoints.Setup(m => m.InProgressPoints).Returns(4);
            mockPoints.Setup(m => m.CompletePoints).Returns(5);
            mockPoints.Setup(m => m.UntypedPoints).Returns(6);

            var target = new LeanKitPresenter(configuration);

            // When
            mockTimer.Raise(o => o.Tick += null, EventArgs.Empty);

            // Then, everything is checked in the CleanUp
        }

    }
}
