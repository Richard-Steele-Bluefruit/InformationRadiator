using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using LeanKit.API.Client.Library.TransferObjects;

namespace LeanKit.Model.Tests
{
    [TestClass]
    public class LeanKitPointsTests
    {
        private LeanKitFactoryMock mockFactory;
        private Mock<LeanKit.API.Client.Library.ILeanKitApi> mockApi;

        [TestInitialize]
        public void Setup()
        {
            LeanKitHelper.lastId = 1;

            mockFactory = new LeanKitFactoryMock();
            LeanKitFactory.Instance = mockFactory;

            mockApi = new Mock<LeanKit.API.Client.Library.ILeanKitApi>(MockBehavior.Strict);
            mockFactory._api = mockApi.Object;

        }

        [TestCleanup]
        public void CleanUp()
        {
            LeanKitFactory.Instance = null;

            mockApi.VerifyAll();
        }
        
        [TestMethod]
        public void Creating_an_instance_configures_the_api()
        {
            // Given, When
            var target = new LeanKitPoints("absw", "informationradiator", "Cornwall1#");

            // Then
            Assert.AreEqual("absw", mockFactory._hostName);
            Assert.AreEqual("informationradiator", mockFactory._userName);
            Assert.AreEqual("Cornwall1#", mockFactory._password);
        }

        [TestMethod]
        public void Getting_an_update_of_progress_of_a_board()
        {
            // Given
            var mockBoard = LeanKitHelper.Board_with_ToDo_Doing_and_Complete_Lanes();

            // ToDo Lane Cards
            mockBoard.Lanes[0].AddCard(size: 2);
            mockBoard.Lanes[0].AddCard(size: 3);

            // Doing Lane Cards
            mockBoard.Lanes[1].AddCard(size: 1);
            mockBoard.Lanes[1].AddCard(size: 1);
            mockBoard.Lanes[1].AddCard(size: 1);

            // Complete Lane Cards
            mockBoard.Lanes[2].AddCard(size: 10);
            mockBoard.Lanes[2].AddCard(size: 0); // Cards of size 0 should count as a size of 1

            mockApi.Setup(m => m.GetBoard(100)).Returns(mockBoard);

            // When
            var target = new LeanKitPoints("absw", "informationradiator", "Cornwall1#");
            target.Update(100, new List<long>());

            // Then
            Assert.AreEqual(5, target.ReadyPoints);
            Assert.AreEqual(3, target.InProgressPoints);
            Assert.AreEqual(11, target.CompletePoints);
        }

        [TestMethod]
        public void Getting_an_update_of_progress_of_a_board_with_multiple_lanes_of_each_type()
        {
            // Given
            var mockBoard = LeanKitHelper.Board_with_ToDo_Doing_and_Complete_Lanes();
            mockBoard.AddLane(type: LaneType.Ready);
            mockBoard.AddLane(type: LaneType.InProcess);
            mockBoard.AddLane(type: LaneType.Completed);
            mockBoard.AddLane(type: LaneType.Untyped);

            // ToDo Lane Cards
            mockBoard.Lanes[0].AddCard(size: 2);
            mockBoard.Lanes[3].AddCard(size: 30);

            // Doing Lane Cards
            mockBoard.Lanes[1].AddCard(size: 1);
            mockBoard.Lanes[1].AddCard(size: 1);
            mockBoard.Lanes[4].AddCard(size: 10);

            // Complete Lane Cards
            mockBoard.Lanes[2].AddCard(size: 10);
            mockBoard.Lanes[5].AddCard(size: 10);

            // Untyped Lane Cards
            mockBoard.Lanes[6].AddCard(size: 10);

            mockApi.Setup(m => m.GetBoard(99)).Returns(mockBoard);

            // When
            var target = new LeanKitPoints("absw", "informationradiator", "Cornwall1#");
            target.Update(99, new List<long>());

            // Then
            Assert.AreEqual(32, target.ReadyPoints);
            Assert.AreEqual(12, target.InProgressPoints);
            Assert.AreEqual(20, target.CompletePoints);
            Assert.AreEqual(10, target.UntypedPoints);
        }


        [TestMethod]
        public void Filtering_lanes_that_are_not_required()
        {
            // Given

            var mockBoard = LeanKitHelper.CreateEmptyBoard();

            mockBoard.AddLane(id: 1, type: LaneType.Untyped);
            mockBoard.AddLane(id: 2, parentId: 1, type: LaneType.Ready);
            mockBoard.AddLane(id: 3, parentId: 2, type: LaneType.Ready).AddCard(size: 3);
            // This lane should be ignored
            mockBoard.AddLane(id: 4, parentId: 2, type: LaneType.Ready).AddCard(size: 6);
            mockBoard.AddLane(id: 5, parentId: 1, type: LaneType.InProcess).AddCard(size: 4);
            mockBoard.AddLane(id: 6, parentId: 1, type: LaneType.Completed).AddCard(size: 5);
            mockBoard.AddLane(id: 7, parentId: 1, type: LaneType.Untyped).AddCard(size: 6);

            // All these lanes should be ignored
            mockBoard.AddLane(id: 8, type: LaneType.Untyped);
            mockBoard.AddLane(id: 9, parentId: 8, type: LaneType.Ready);
            mockBoard.AddLane(id: 10, parentId: 9, type: LaneType.Ready).AddCard(size: 6);
            mockBoard.AddLane(id: 11, parentId: 8, type: LaneType.InProcess).AddCard(size: 7);
            mockBoard.AddLane(id: 12, parentId: 8, type: LaneType.Completed).AddCard(size: 8);
            mockBoard.AddLane(id: 13, parentId: 8, type: LaneType.Untyped).AddCard(size: 9);

            mockApi.Setup(m => m.GetBoard(250)).Returns(mockBoard);

            // When
            var target = new LeanKitPoints("absw", "informationradiator", "Cornwall1#");

            var ignoreLanes = new List<long>() { 4, 8 };
            target.Update(250, ignoreLanes);

            // Then
            Assert.AreEqual(3, target.ReadyPoints);
            Assert.AreEqual(4, target.InProgressPoints);
            Assert.AreEqual(5, target.CompletePoints);
            Assert.AreEqual(6, target.UntypedPoints);
        }
    }
}
