using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;
using System.Collections.Generic;

using Moq;
using LeanKit.API.Client.Library.TransferObjects;

using LeanKit.Model.LaneHistory;

namespace LeanKit.Model.Tests.LaneHistory
{
    [TestClass]
    public class LeanKitFilteredLanePointsHistoryTests
    {
        private Mock<ILeanKitLanePointsHistory> mockPointsHistory;
        private Board mockBoard;
        private List<LanePointsHistory> mockHistory;
        private List<long> ignoredLanes;

        [TestInitialize]
        public void Setup()
        {
            mockPointsHistory = new Mock<ILeanKitLanePointsHistory>(MockBehavior.Loose);

            mockBoard = new Board
            {
                Lanes = new List<Lane>(),
                Backlog = new List<Lane>(),
                Archive = new List<Lane>()
            };

            mockPointsHistory.Setup(m => m.Board).Returns(mockBoard);

            mockHistory = new List<LanePointsHistory>();

            ignoredLanes = new List<long>();
        }

        [TestMethod]
        public void Filtering_the_entire_backlog_lane()
        {
            // Given
            mockBoard.Backlog.Add(new Lane { Id = 1 });
            mockBoard.Lanes.Add(new Lane { Id = 2 });
            mockBoard.Lanes.Add(new Lane { Id = 3 });
            mockBoard.Archive.Add(new Lane { Id = 4 });

            mockHistory.Add(new LanePointsHistory { Id = 1 });
            mockHistory.Add(new LanePointsHistory { Id = 2 });
            mockHistory.Add(new LanePointsHistory { Id = 3 });
            mockHistory.Add(new LanePointsHistory { Id = 4 });
            mockPointsHistory.Setup(m => m.LaneHistory).Returns(mockHistory.AsReadOnly());

            ignoredLanes.Add(1);

            var target = new LeanKitFilteredLanePointsHistory(mockPointsHistory.Object, ignoredLanes);

            // When
            target.Update(10);

            // Then
            Assert.AreEqual(3, target.LaneHistory.Count);
            Assert.IsNotNull(target.LaneHistory.FirstOrDefault(m => m.Id == 2), "Unable to find lane history with Id 2");
            Assert.IsNotNull(target.LaneHistory.FirstOrDefault(m => m.Id == 3), "Unable to find lane history with Id 3");
            Assert.IsNotNull(target.LaneHistory.FirstOrDefault(m => m.Id == 4), "Unable to find lane history with Id 4");
        }

        [TestMethod]
        public void Filtering_a_sublane()
        {
            // Given
            mockBoard.Backlog.Add(new Lane { Id = 1 });
            mockBoard.Lanes.Add(new Lane { Id = 2 });
            mockBoard.Lanes.Add(new Lane { Id = 3 });
            mockBoard.Lanes.Add(new Lane { Id = 5, ParentLaneId = 3 });
            mockBoard.Archive.Add(new Lane { Id = 4 });

            mockHistory.Add(new LanePointsHistory { Id = 1 });
            mockHistory.Add(new LanePointsHistory { Id = 2 });
            mockHistory.Add(new LanePointsHistory { Id = 3 });
            mockHistory.Add(new LanePointsHistory { Id = 5 });
            mockHistory.Add(new LanePointsHistory { Id = 4 });
            mockPointsHistory.Setup(m => m.LaneHistory).Returns(mockHistory.AsReadOnly());

            ignoredLanes.Add(3);

            var target = new LeanKitFilteredLanePointsHistory(mockPointsHistory.Object, ignoredLanes);

            // When
            target.Update(10);

            // Then
            Assert.AreEqual(3, target.LaneHistory.Count);
            Assert.IsNotNull(target.LaneHistory.FirstOrDefault(m => m.Id == 1), "Unable to find lane history with Id 1");
            Assert.IsNotNull(target.LaneHistory.FirstOrDefault(m => m.Id == 2), "Unable to find lane history with Id 2");
            Assert.IsNotNull(target.LaneHistory.FirstOrDefault(m => m.Id == 4), "Unable to find lane history with Id 4");
        }

    }
}
