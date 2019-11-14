using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LeanKit.API.Client.Library;
using LeanKit.API.Client.Library.TransferObjects;
using LeanKit.API.Client.Library.Enumerations;

namespace LeanKit.Model.LaneHistory
{
    public class LeanKitLanePointsHistory : ILeanKitLanePointsHistory
    {
        private ILeanKitApi _api;
        private long _boardId;
        private int _numberOfDaysHistory;
        private DateTime _date;

        public System.Collections.ObjectModel.ReadOnlyCollection<LanePointsHistory> LaneHistory { get; private set; }
        public Board Board { get; private set; }
        private List<LanePointsHistory> Lanes { get; set; }

        public LeanKitLanePointsHistory(ILeanKitApi api, long boardId)
        {
            _api = api;
            _boardId = boardId;
        }

        public LeanKitLanePointsHistory(string hostName, string userName, string password, long boardId)
        {
            _api = LeanKitFactory.Instance.CreateApi(hostName, userName, password);
            _boardId = boardId;
        }

        private TypeOfLane ConvertLaneType(LaneType type)
        {
            switch(type)
            {
               case LaneType.Completed:
                    return TypeOfLane.Completed;
               case LaneType.InProcess:
                    return TypeOfLane.InProcess;
               case LaneType.Ready:
                    return TypeOfLane.Ready;
                default:
                    return TypeOfLane.Untyped;
            }
        }

        private void AddLanes(IList<Lane> boardLanes, long parentId = 0)
        {
            var filteredLanes = from lane in boardLanes
                                where lane.ParentLaneId == parentId
                                select lane;
            var orderedBoardLanes = filteredLanes.OrderBy((p) => { return p.Index; });
            var points = new int[_numberOfDaysHistory];
            foreach (var lane in orderedBoardLanes)
            {
                Lanes.Add(new LanePointsHistory { Id = lane.Id ?? 0, Title = lane.Title, PointsPerDay = new List<int>(points), Type = ConvertLaneType(lane.Type) });
                AddLanes(boardLanes, lane.Id ?? -1);
            }
        }

        private void AddCard(CardView card)
        {
            var cardHistory = new LeanKitCardViewHistory(_api, _boardId, card, _numberOfDaysHistory, _date);
            for (int i = 0; i < _numberOfDaysHistory; i++)
            {
                var lane = Lanes.FirstOrDefault(l => l.Id == cardHistory.PointsPerDay[i].LaneId);
                if(lane != null)
                {
                    lane.PointsPerDay[i] += cardHistory.PointsPerDay[i].Points;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Unable to find lane " + cardHistory.PointsPerDay[i].LaneId);
                }
            }
        }

        private int AddCardsFromLaneList(IEnumerable<Lane> boardLanes)
        {
            int numberOfCards = 0;
            foreach (var lane in boardLanes)
            {
                foreach (var card in lane.Cards)
                {
                    numberOfCards++;
                    AddCard(card);
                }
            }
            return numberOfCards;
        }

        private List<CardView> BuildArchiveCardList()
        {
            var archive = new List<CardView>();

            SearchOptions options = new SearchOptions();
            options.IncludeBacklogOnly = true;
            options.SearchInOldArchive = true;
            options.SearchInRecentArchive = true;

            bool keepRunning = true;
            
            do
            {
                var cards = _api.SearchCards(_boardId, options).ToList();
                foreach (var card in cards)
                {
                    DateTime lastActivity = card.ConvertedLastActivity() ?? DateTime.Now;

                    if (lastActivity >= _date.AddDays(-_numberOfDaysHistory))
                    {
                        archive.Add(card);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Discarding card '" + card.Title + "' as it wasn't modified in the time period requested");
                    }
                }
                if (cards.Count == 0)
                {
                    keepRunning = false;
                }
                options.Page++;
            } while (keepRunning);

            return archive;

        }

        private int AddArchiveCards()
        {
            var archiveCards = BuildArchiveCardList();
            int numberOfCards = 0;
            foreach (var card in archiveCards)
            {
                numberOfCards++;
                AddCard(card);
            }
            return numberOfCards;
        }

        public void Update(int numberOfDaysHistory = 10)
        {
#if DEBUG
            DateTime startTime = DateTime.Now;
#endif
            Board = _api.GetBoard(_boardId);
            Lanes = new List<LanePointsHistory>();
            _date = DateTime.Now.Date;

            _numberOfDaysHistory = numberOfDaysHistory;

            AddLanes(Board.Backlog);
            AddLanes(Board.Lanes);
            AddLanes(Board.Archive);

            int backlogCardCount = AddCardsFromLaneList(Board.Backlog);
            int lanesCardCount = AddCardsFromLaneList(Board.Lanes);
            int archiveCardCount = AddArchiveCards();

            System.Diagnostics.Debug.WriteLine("Number of backlog cards : " + backlogCardCount);
            System.Diagnostics.Debug.WriteLine("Number of lanes cards   : " + lanesCardCount);
            System.Diagnostics.Debug.WriteLine("Number of archive cards : " + archiveCardCount);
#if DEBUG
            System.Diagnostics.Debug.WriteLine("Time to download cards  : " + (DateTime.Now - startTime).TotalMinutes.ToString("0.0") + " minutes");
#endif

            LaneHistory = Lanes.AsReadOnly();
        }

    }
}
