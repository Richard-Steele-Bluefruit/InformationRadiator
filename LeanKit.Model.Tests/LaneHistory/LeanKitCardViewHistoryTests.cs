using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

//using System.Linq;

//using System.IO;
using System.Collections.Generic;

using Moq;
using LeanKit.API.Client.Library;
using LeanKit.API.Client.Library.TransferObjects;
using LeanKit.API.Client.Library.Enumerations;

using LeanKit.Model.LaneHistory;

namespace LeanKit.Model.Tests.LaneHistory
{
    [TestClass]
    public class LeanKitCardViewHistoryTests
    {
        private Mock<ILeanKitApi> mockApi;

        private List<CardEvent> history;
        private DateTime currentDate;
        private CardView card;
        private long boardId;

        private LeanKitCardViewHistory target;

        #region Setup and Teardown

        [TestInitialize]
        public void Setup()
        {
            history = new List<CardEvent>();
            currentDate = DateTime.Now;
            card = new CardView { Id = 1, LaneId = 1, Size = 10 };
            boardId = 5;
            CreateMockApi(MockBehavior.Strict);
        }

        [TestCleanup]
        public void TearDown()
        {
            mockApi.VerifyAll();
        }

        #endregion Setup and Teardown

        #region Helper Functions

        private void CreateMockApi(MockBehavior behavior)
        {
            mockApi = new Mock<ILeanKitApi>(behavior);
        }

        private void ApiSetupGetCardHistory(long cardId)
        {
            mockApi.Setup(m => m.GetCardHistory(boardId, cardId)).Returns(history);
        }

        #endregion Helper Functions

        [TestMethod]
        public void A_card_which_moves_lane()
        {
            // Given
            var cardEvent = new CardEvent
            {
                Type = "CardMoveEvent",
                DateTime = LeanKitConversions.DateTimeToCardEventDateTime(currentDate.AddDays(-2)),
                FromLaneId = 5,
                ToLaneId = card.LaneId
            };
            history.Add(cardEvent);

            ApiSetupGetCardHistory(card.Id);

            // When
            target = new LeanKitCardViewHistory(mockApi.Object, boardId, card, 5, currentDate);

            // Then
            Assert.AreEqual(5, target.PointsPerDay.Count);

            Assert.AreEqual(card.LaneId, target.PointsPerDay[0].LaneId, "Incorrect lane id for day 0");
            Assert.AreEqual(card.LaneId, target.PointsPerDay[1].LaneId, "Incorrect lane id for day 1");
            Assert.AreEqual(cardEvent.FromLaneId, target.PointsPerDay[2].LaneId, "Incorrect lane id for day 2");
            Assert.AreEqual(cardEvent.FromLaneId, target.PointsPerDay[3].LaneId, "Incorrect lane id for day 3");
            Assert.AreEqual(cardEvent.FromLaneId, target.PointsPerDay[4].LaneId, "Incorrect lane id for day 4");
        }

        [TestMethod]
        public void A_card_which_changes_size()
        {
            // Given
            int oldSize = 2;
            card.Size = 5;
            var cardEvent = new CardEvent
            {
                Type = "CardFieldsChangedEvent",
                DateTime = LeanKitConversions.DateTimeToCardEventDateTime(currentDate.AddDays(-3)),
                Changes = new List<CardEvent.FieldChange>
                {
                    new CardEvent.FieldChange { FieldName = "Description", OldValue = "XYZ" },
                    new CardEvent.FieldChange { FieldName = "Size", OldValue = oldSize.ToString() }
                }
            };
            history.Add(cardEvent);

            ApiSetupGetCardHistory(card.Id);

            // When
            var target = new LeanKitCardViewHistory(mockApi.Object, boardId, card, 5, currentDate);

            // Then
            Assert.AreEqual(5, target.PointsPerDay.Count);

            Assert.AreEqual(card.Size, target.PointsPerDay[0].Points, "Incorrect points for day 0");
            Assert.AreEqual(card.Size, target.PointsPerDay[1].Points, "Incorrect points for day 1");
            Assert.AreEqual(card.Size, target.PointsPerDay[2].Points, "Incorrect points for day 2");
            Assert.AreEqual(oldSize, target.PointsPerDay[3].Points, "Incorrect points for day 3");
            Assert.AreEqual(oldSize, target.PointsPerDay[4].Points, "Incorrect points for day 4");
        }

        [TestMethod]
        public void A_card_which_has_a_size_of_0()
        {
            // Given
            card.Size = 0;

            ApiSetupGetCardHistory(card.Id);

            // When
            var target = new LeanKitCardViewHistory(mockApi.Object, boardId, card, 5, currentDate);

            // Then
            Assert.AreEqual(5, target.PointsPerDay.Count);

            Assert.AreEqual(1, target.PointsPerDay[0].Points, "Incorrect points for day 0");
            Assert.AreEqual(1, target.PointsPerDay[1].Points, "Incorrect points for day 1");
            Assert.AreEqual(1, target.PointsPerDay[2].Points, "Incorrect points for day 2");
            Assert.AreEqual(1, target.PointsPerDay[3].Points, "Incorrect points for day 3");
            Assert.AreEqual(1, target.PointsPerDay[4].Points, "Incorrect points for day 4");
        }

        [TestMethod]
        public void A_card_which_used_to_have_a_size_of_0()
        {
            // Given
            int oldSize = 0;
            card.Size = 5;
            var cardEvent = new CardEvent
            {
                Type = "CardFieldsChangedEvent",
                DateTime = LeanKitConversions.DateTimeToCardEventDateTime(currentDate.AddDays(-2)),
                Changes = new List<CardEvent.FieldChange>
                {
                    new CardEvent.FieldChange { FieldName = "Description", OldValue = "XYZ" },
                    new CardEvent.FieldChange { FieldName = "Size", OldValue = oldSize.ToString() }
                }
            };
            history.Add(cardEvent);

            ApiSetupGetCardHistory(card.Id);

            // When
            var target = new LeanKitCardViewHistory(mockApi.Object, boardId, card, 5, currentDate);

            // Then
            Assert.AreEqual(5, target.PointsPerDay.Count);

            Assert.AreEqual(card.Size, target.PointsPerDay[0].Points, "Incorrect points for day 0");
            Assert.AreEqual(card.Size, target.PointsPerDay[1].Points, "Incorrect points for day 1");
            Assert.AreEqual(1, target.PointsPerDay[2].Points, "Incorrect points for day 2");
            Assert.AreEqual(1, target.PointsPerDay[3].Points, "Incorrect points for day 3");
            Assert.AreEqual(1, target.PointsPerDay[4].Points, "Incorrect points for day 4");
        }

        [TestMethod]
        public void A_card_which_was_created_during_the_period()
        {
            // Given
            card.Size = 5;
            var cardEvent = new CardEvent
            {
                Type = "CardCreationEvent",
                DateTime = LeanKitConversions.DateTimeToCardEventDateTime(currentDate.AddDays(-2))
            };
            history.Add(cardEvent);

            ApiSetupGetCardHistory(card.Id);

            // When
            var target = new LeanKitCardViewHistory(mockApi.Object, boardId, card, 5, currentDate);

            // Then
            Assert.AreEqual(5, target.PointsPerDay.Count);

            Assert.AreEqual(card.Size, target.PointsPerDay[0].Points, "Incorrect points for day 0");
            Assert.AreEqual(card.Size, target.PointsPerDay[1].Points, "Incorrect points for day 1");
            Assert.AreEqual(0, target.PointsPerDay[2].Points, "Incorrect points for day 2");
            Assert.AreEqual(0, target.PointsPerDay[3].Points, "Incorrect points for day 3");
            Assert.AreEqual(0, target.PointsPerDay[4].Points, "Incorrect points for day 4");
        }
    }
}
