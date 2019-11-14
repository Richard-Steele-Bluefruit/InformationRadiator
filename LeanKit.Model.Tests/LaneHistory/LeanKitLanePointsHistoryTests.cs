using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;

using System.IO;
using System.Collections.Generic;

using Moq;
using LeanKit.API.Client.Library;
using LeanKit.API.Client.Library.TransferObjects;
using LeanKit.API.Client.Library.Enumerations;

using LeanKit.Model.LaneHistory;

namespace LeanKit.Model.Tests.LaneHistory
{
    [TestClass]
    public class LeanKitLanePointsHistoryTests
    {
        private Mock<ILeanKitApi> mockApi;
        private Board mockBoard;
        private Lane mockBacklogLane;
        private Lane mockToDoLane;
        private Lane mockDoingLane;
        private Lane mockDoneLane;
        private Lane mockArchiveLane;
        private long? invalidLaneId;
        private List<List<CardView>> mockArchiveCardLists;

        private LeanKitLanePointsHistory target;

        #region Setup and Teardown

        [TestInitialize]
        public void Setup()
        {
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
            mockBoard = new Board();
            mockBoard.Id = 100;
            mockBoard.Backlog = new List<Lane>();
            mockBoard.Lanes = new List<Lane>();
            mockBoard.Archive = new List<Lane>();
            mockArchiveCardLists = new List<List<CardView>>();
            mockArchiveCardLists.Add(new List<CardView>());
        }

        private void ApiSetupGetBoard(long id, Board board = null, bool allowNullBoard = false)
        {
            if (board == null && !allowNullBoard)
                board = mockBoard;
            mockApi.Setup(m => m.GetBoard(id)).Returns(board);
            var listsReturned = mockApi.SetupSequence(m => m.SearchCards(board.Id, It.IsAny<SearchOptions>()));
            foreach(var archiveList in mockArchiveCardLists)
            {
                listsReturned = listsReturned.Returns(archiveList);
            }
            listsReturned = listsReturned.Returns(new List<CardView>());
        }

        private List<CardEvent> AddCardToLane(long boardId, Lane lane, CardView card)
        {
            lane.Cards.Add(card);

            var history = new List<CardEvent>();
            mockApi.Setup(m => m.GetCardHistory(boardId, card.Id)).Returns(history);
            return history;
        }

        private void RunTest(long boardId = 100, int numberOfDaysHistory = 10)
        {
            target = new LeanKitLanePointsHistory(mockApi.Object, boardId);
            target.Update(numberOfDaysHistory);
        }

        private void AddDefaultLanesToBoard()
        {
            mockBacklogLane = new Lane { Index = 0, Id = 1, Title = "Backlog", ClassType = LaneClassType.Backlog, Type = LaneType.Untyped };
            mockBacklogLane.Cards = new List<CardView>();
            mockBoard.Backlog.Add(mockBacklogLane);

            mockToDoLane = new Lane { Index = 0, Id = 2, Title = "To Do", ClassType = LaneClassType.Active, Type = LaneType.Ready };
            mockToDoLane.Cards = new List<CardView>();
            mockBoard.Lanes.Add(mockToDoLane);

            mockDoingLane = new Lane { Index = 1, Id = 3, Title = "Doing", ClassType = LaneClassType.Active, Type = LaneType.InProcess };
            mockDoingLane.Cards = new List<CardView>();
            mockBoard.Lanes.Add(mockDoingLane);

            mockDoneLane = new Lane { Index = 2, Id = 4, Title = "Done", ClassType = LaneClassType.Active, Type = LaneType.Completed };
            mockDoneLane.Cards = new List<CardView>();
            mockBoard.Lanes.Add(mockDoneLane);

            mockArchiveLane = new Lane { Index = 0, Id = 5, Title = "Archive", ClassType = LaneClassType.Archive, Type = LaneType.Untyped };
            mockBoard.Archive.Add(mockArchiveLane);

            invalidLaneId = 6;
        }

        #endregion Helper Functions

        #region Checking Functions

        public List<int> AllAre(int value = 0, int numberOfElements = 10)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < numberOfElements; i++)
            {
                result.Add(value);
            }
            return result;
        }

        private LanePointsHistory ActualLane(Lane lane)
        {
            var result = target.LaneHistory.First(m => m.Id == lane.Id);
            Assert.IsNotNull(result, "Unable to find actual lane " + lane.Title);
            return result;
        }

        #endregion Checking Functions

        [TestMethod]
        public void A_default_board_with_no_cards()
        {
            // Given
            AddDefaultLanesToBoard();
            ApiSetupGetBoard(mockBoard.Id);

            // When
            RunTest(boardId: 100, numberOfDaysHistory: 10);

            // Then
            Assert.AreEqual(5, target.LaneHistory.Count,"Incorrect lane count");

            ActualLane(mockBacklogLane).AssertLaneHistoryIs(AllAre(0), "Backlog", laneType: TypeOfLane.Untyped);
            ActualLane(mockToDoLane).AssertLaneHistoryIs(AllAre(0), "To Do", laneType: TypeOfLane.Ready);
            ActualLane(mockDoingLane).AssertLaneHistoryIs(AllAre(0), "Doing", laneType: TypeOfLane.InProcess);
            ActualLane(mockDoneLane).AssertLaneHistoryIs(AllAre(0), "Done", laneType: TypeOfLane.Completed);
            ActualLane(mockArchiveLane).AssertLaneHistoryIs(AllAre(0), "Archive", laneType: TypeOfLane.Untyped);
        }

        [TestMethod]
        public void Two_cards_in_the_ToDo_lane_both_with_no_history()
        {
            // Given
            AddDefaultLanesToBoard();
            ApiSetupGetBoard(mockBoard.Id);

            var card = new CardView { LaneId = mockToDoLane.Id ?? 0, Size = 5 };
            mockToDoLane.Cards.Add(card);
            card = new CardView { LaneId = mockToDoLane.Id ?? 0, Size = 10 };
            mockToDoLane.Cards.Add(card);

            // When
            RunTest(boardId: 100, numberOfDaysHistory: 5);

            // Then
            ActualLane(mockBacklogLane).AssertLaneHistoryIs(AllAre(value: 0, numberOfElements: 5), "Backlog");
            ActualLane(mockToDoLane).AssertLaneHistoryIs(AllAre(value: 15, numberOfElements: 5), "To Do");
            ActualLane(mockDoingLane).AssertLaneHistoryIs(AllAre(value: 0, numberOfElements: 5), "Doing");
            ActualLane(mockDoneLane).AssertLaneHistoryIs(AllAre(value: 0, numberOfElements: 5), "Done");
            ActualLane(mockArchiveLane).AssertLaneHistoryIs(AllAre(value: 0, numberOfElements: 5), "Archive");
        }

        [TestMethod]
        public void Cards_in_the_Backlog_and_Archive_lanes_with_no_history()
        {
            // Given
            AddDefaultLanesToBoard();
            ApiSetupGetBoard(mockBoard.Id);

            var card = new CardView { LaneId = mockBacklogLane.Id ?? 0, Size = 5 };
            mockBacklogLane.Cards.Add(card);
            card = new CardView { LaneId = mockArchiveLane.Id ?? 0, Size = 10 };
            mockArchiveCardLists[0].Add(card);

            // When
            RunTest(boardId: 100, numberOfDaysHistory: 5);

            // Then
            ActualLane(mockBacklogLane).AssertLaneHistoryIs(AllAre(value: 5, numberOfElements: 5), "Backlog");
            ActualLane(mockToDoLane).AssertLaneHistoryIs(AllAre(value: 0, numberOfElements: 5), "To Do");
            ActualLane(mockDoingLane).AssertLaneHistoryIs(AllAre(value: 0, numberOfElements: 5), "Doing");
            ActualLane(mockDoneLane).AssertLaneHistoryIs(AllAre(value: 0, numberOfElements: 5), "Done");
            ActualLane(mockArchiveLane).AssertLaneHistoryIs(AllAre(value: 10, numberOfElements: 5), "Archive");
        }

        [TestMethod]
        public void A_card_in_the_ToDo_lane_with_history_and_a_card_in_the_archive_with_no_history()
        {
            // Given
            AddDefaultLanesToBoard();
            ApiSetupGetBoard(mockBoard.Id);

            var card = new CardView { Id = 350, LaneId = mockDoingLane.Id ?? 0, Size = 5 };
            var history = AddCardToLane(mockBoard.Id, mockDoingLane, card);

            var cardEvent = new CardEvent
            {
                Type = "CardMoveEvent",
                DateTime = LeanKitConversions.DateTimeToCardEventDateTime(DateTime.Now.AddDays(-1)),
                FromLaneId = mockToDoLane.Id ?? 0,
                ToLaneId = mockDoingLane.Id ?? 0
            };
            history.Add(cardEvent);

            card = new CardView { Id = 200, LaneId = mockDoingLane.Id ?? 0, Size = 10 };
            AddCardToLane(mockBoard.Id, mockDoingLane, card);

            // When
            RunTest(boardId: mockBoard.Id, numberOfDaysHistory: 5);

            // Then
            ActualLane(mockBacklogLane).AssertLaneHistoryIs(AllAre(value: 0, numberOfElements: 5), "Backlog");
            ActualLane(mockToDoLane).AssertLaneHistoryIs(new List<int> { 0, 5, 5, 5, 5 }, "To Do");
            ActualLane(mockDoingLane).AssertLaneHistoryIs(new List<int> { 15, 10, 10, 10, 10 }, "Doing");
            ActualLane(mockDoneLane).AssertLaneHistoryIs(AllAre(value: 0, numberOfElements: 5), "Done");
            ActualLane(mockArchiveLane).AssertLaneHistoryIs(AllAre(value: 0, numberOfElements: 5), "Archive");
        }


        [TestMethod]
        public void A_card_in_the_ToDo_lane_with_history_that_refers_to_a_lane_that_no_longer_exists()
        {
            // Given
            AddDefaultLanesToBoard();
            ApiSetupGetBoard(mockBoard.Id);

            var card = new CardView { Id = 350, LaneId = mockDoingLane.Id ?? 0, Size = 5 };
            var history = AddCardToLane(mockBoard.Id, mockDoingLane, card);

            var cardEvent = new CardEvent
            {
                Type = "CardMoveEvent",
                DateTime = LeanKitConversions.DateTimeToCardEventDateTime(DateTime.Now.AddDays(-1)),
                FromLaneId = invalidLaneId,
                ToLaneId = mockDoingLane.Id ?? 0
            };
            history.Add(cardEvent);

            card = new CardView { Id = 200, LaneId = mockDoingLane.Id ?? 0, Size = 10 };
            AddCardToLane(mockBoard.Id, mockDoingLane, card);

            // When
            RunTest(boardId: mockBoard.Id, numberOfDaysHistory: 5);

            // Then
            ActualLane(mockBacklogLane).AssertLaneHistoryIs(AllAre(value: 0, numberOfElements: 5), "Backlog");
            ActualLane(mockToDoLane).AssertLaneHistoryIs(AllAre(value: 0, numberOfElements: 5), "To Do");
            ActualLane(mockDoingLane).AssertLaneHistoryIs(new List<int> { 15, 10, 10, 10, 10 }, "Doing");
            ActualLane(mockDoneLane).AssertLaneHistoryIs(AllAre(value: 0, numberOfElements: 5), "Done");
            ActualLane(mockArchiveLane).AssertLaneHistoryIs(AllAre(value: 0, numberOfElements: 5), "Archive");
        }

        [TestMethod]
        public void Cards_in_the_Archive_lane_that_have_not_been_modified_in_the_required_number_of_days_do_not_have_their_history_downloaded_from_leankit()
        {
            // Given
            AddDefaultLanesToBoard();
            ApiSetupGetBoard(mockBoard.Id);

            var card = new CardView { LaneId = mockArchiveLane.Id ?? 0, Size = 10,
                LastActivity = DateTime.Now.AddDays(-6).ToLongDateString() };
            mockArchiveCardLists[0].Add(card);

            // When
            RunTest(boardId: 100, numberOfDaysHistory: 5);

            // Then
            ActualLane(mockBacklogLane).AssertLaneHistoryIs(AllAre(value: 0, numberOfElements: 5), "Backlog");
            ActualLane(mockToDoLane).AssertLaneHistoryIs(AllAre(value: 0, numberOfElements: 5), "To Do");
            ActualLane(mockDoingLane).AssertLaneHistoryIs(AllAre(value: 0, numberOfElements: 5), "Doing");
            ActualLane(mockDoneLane).AssertLaneHistoryIs(AllAre(value: 0, numberOfElements: 5), "Done");
            ActualLane(mockArchiveLane).AssertLaneHistoryIs(AllAre(value: 0, numberOfElements: 5), "Archive");
        }

    }
}
