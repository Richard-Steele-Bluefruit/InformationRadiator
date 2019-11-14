using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

using LeanKit.Model.Ticker;
using Moq;
using LeanKit.API.Client.Library.TransferObjects;

namespace LeanKit.Model.Tests.Ticker
{
    [TestClass]
    public class LeanKitTickerTests
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
            var target = new LeanKitTicker("absw", "informationradiator", "Cornwall1#", 0, 0);

            // Then
            Assert.AreEqual("absw", mockFactory._hostName);
            Assert.AreEqual("informationradiator", mockFactory._userName);
            Assert.AreEqual("Cornwall1#", mockFactory._password);
        }

        [TestMethod]
        public void Reading_messages_from_the_board()
        {
            // Given
            var mockBoard = LeanKitHelper.Board_with_ToDo_Doing_and_Complete_Lanes();

            // ToDo Lane Cards
            mockBoard.Lanes[0].AddCard(title: "A");

            // Messages Lane Cards
            mockBoard.Lanes[1].AddCard(title: "B");
            mockBoard.Lanes[1].AddCard(title: "C", startDate: new DateTime(2015, 02, 01), dueDate: new DateTime(2015, 02, 25));
            mockBoard.Lanes[1].AddCard(title: "D");

            // Complete Lane Cards
            mockBoard.Lanes[2].AddCard(title: "E");

            const long boardId = 1256;
            const long laneId = 478239;
            var target = new LeanKitTicker("a", "i", "C", boardId, laneId);

            mockBoard.Lanes[1].Id = laneId;
            mockApi.Setup(m => m.GetBoard(boardId)).Returns(mockBoard);

            // When
            var result = target.GetMessages();

            // Then
            Assert.AreEqual(3, result.Count);
            Assert.IsNotNull(result.FirstOrDefault(s => (s.Message == "B") && (s.DueDate == null)), "No 'B' message found");
            var actualCMessage = result.FirstOrDefault(s => (s.Message == "C"));
            Assert.IsNotNull(actualCMessage, "No 'C' message found");
            Assert.AreEqual(2015, actualCMessage.DueDate.Value.Year);
            Assert.AreEqual(02, actualCMessage.DueDate.Value.Month);
            Assert.AreEqual(25, actualCMessage.DueDate.Value.Day);
            Assert.AreEqual(2015, actualCMessage.StartDate.Value.Year);
            Assert.AreEqual(02, actualCMessage.StartDate.Value.Month);
            Assert.AreEqual(01, actualCMessage.StartDate.Value.Day);
            Assert.IsNotNull(result.FirstOrDefault(s => (s.Message == "D") && (s.DueDate == null)), "No 'D' message found");
        }

        [TestMethod]
        public void Reading_from_a_non_existent_lane()
        {
            // Given
            var mockBoard = LeanKitHelper.Board_with_ToDo_Doing_and_Complete_Lanes();

            // ToDo Lane Cards
            mockBoard.Lanes[0].AddCard(title: "A");

            // Messages Lane Cards
            mockBoard.Lanes[1].AddCard(title: "B");
            mockBoard.Lanes[1].AddCard(title: "C");
            mockBoard.Lanes[1].AddCard(title: "D");

            // Complete Lane Cards
            mockBoard.Lanes[2].AddCard(title: "E");

            const long boardId = 1256;
            const long laneId = 478239;
            var target = new LeanKitTicker("a", "i", "C", boardId, 100); // Invlaid lane Id

            mockBoard.Lanes[1].Id = laneId;
            mockApi.Setup(m => m.GetBoard(boardId)).Returns(mockBoard);

            // When
            var result = target.GetMessages();

            // Then
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Reading_from_a_non_existent_board()
        {
            // Given

            const long boardId = 1256;
            const long laneId = 478239;
            var target = new LeanKitTicker("a", "i", "C", boardId, laneId);

            mockApi.Setup(m => m.GetBoard(boardId)).Returns<Board>(null);

            // When
            var result = target.GetMessages();

            // Then
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void An_exception_is_thrown_from_LeanKitAPI()
        {
            // Given

            const long boardId = 1256;
            const long laneId = 478239;
            var target = new LeanKitTicker("a", "i", "C", boardId, laneId);

            mockApi.Setup(m => m.GetBoard(boardId)).Throws(new Exception());

            // When
            var result = target.GetMessages();

            // Then
            Assert.IsNull(result);
        }
    }
}
