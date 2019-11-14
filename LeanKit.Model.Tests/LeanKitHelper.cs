using System;
using System.Collections.Generic;
using System.Linq;

using LeanKit.API.Client.Library.TransferObjects;

namespace LeanKit.Model.Tests
{
    public static class LeanKitHelper
    {
        public static long lastId = 1;

        public static Board CreateEmptyBoard()
        {
            var board = new Board
                {
                    Lanes = new List<Lane>(),
                    Backlog = new List<Lane>(),
                    Archive = new List<Lane>()
                };
            return board;
        }

        public static Lane CreateEmptyLane(LaneType type)
        {
            var lane = new Lane
                {
                    Cards = new List<CardView>(),
                    Type = type
                };
            return lane;
        }

        public static CardView CreateEmptyCardView()
        {
            var card = new CardView { };
            return card;
        }

        public static Lane AddLane(this Board o, LaneType type = LaneType.Untyped, long id = -1, long parentId = 0)
        {
            var lane = CreateEmptyLane(type);
            if(id == -1)
            {
                id = lastId;
                lastId++;
            }
            lane.Id = id;
            lane.ParentLaneId = parentId;
            o.Lanes.Add(lane);
            return lane;
        }

        public static Board Board_with_ToDo_Doing_and_Complete_Lanes()
        {
            var board = CreateEmptyBoard();
            board.AddLane(type: LaneType.Ready); // ToDo Lane
            board.AddLane(type: LaneType.InProcess); // Doing Lane
            board.AddLane(type: LaneType.Completed); // Complete Lane
            return board;
        }

        public static CardView AddCard(this Lane o, int size = 1, string title = "", DateTime? startDate = null, DateTime? dueDate = null)
        {
            var card = new CardView
                {
                    Size = size,
                    Title = title,
                    StartDate = (startDate != null) ? (startDate ?? DateTime.Now).ToShortDateString() : "",
                    DueDate = (dueDate != null) ? (dueDate ?? DateTime.Now).ToShortDateString() : ""
                };
            o.Cards.Add(card);
            return card;
        }
    }
}
