using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LeanKit.API.Client.Library;
using LeanKit.API.Client.Library.TransferObjects;
using LeanKit.API.Client.Library.Enumerations;

namespace LeanKit.Model.LaneHistory
{
    internal class LeanKitCardViewHistory
    {
        private List<CardEvent> history;
        private int historyPosition;
        private DateTime historyDate;

        private long cardLaneId;
        private int cardSize;
        

        public IList<CardPointsHistory> PointsPerDay { get; private set; }

        public LeanKitCardViewHistory(ILeanKitApi api, long boardId, CardView card, int numberOfDays, DateTime date)
        {
            try
            {
                var unorderedHistory = api.GetCardHistory(boardId, card.Id);
                var orderedEvents = unorderedHistory.OrderBy(c => c.ConvertedDateTime()).Reverse();
                history = orderedEvents.ToList();
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("***** Error download history:\r\n" + ex.Message);
                history = new List<CardEvent>();
            }

            historyDate = date.Date;
            historyPosition = 0;

            PointsPerDay = new List<CardPointsHistory>();

            cardLaneId = card.LaneId;
            cardSize = card.Size != 0 ? card.Size : 1;

            PointsPerDay.Add(new CardPointsHistory(cardLaneId, cardSize));
            for (int i = 1; i < numberOfDays; i++)
            {
                historyDate = historyDate.AddDays(-1);
                ApplyHistory();
                PointsPerDay.Add(new CardPointsHistory(cardLaneId, cardSize));
            }
        }

        private void ApplyHistory()
        {
            while((historyPosition < history.Count) &&
                (history[historyPosition].ConvertedDateTime() > historyDate))
            {
                switch(history[historyPosition].CardEventType())
                {
                    case EventType.CardMove:
                        cardLaneId = history[historyPosition].FromLaneId ?? 0;
                        break;
                    case EventType.CardFieldsChanged:
                        ApplyFieldChanges(history[historyPosition].Changes);
                        break;
                    case EventType.CardCreation:
                        cardSize = 0;
                        break;
#if DEBUG
                    case EventType.BoardCreation:
                    case EventType.BoardEdit:
                    case EventType.CardBlocked:
//                    case EventType.CardDeleted:
                    case EventType.CommentPost:
                    case EventType.UserAssignment:
                    case EventType.UserWipOverride:
                    case EventType.WipOverride:
                    case EventType.AttachmentChange:
//                    case EventType.CardMoveToBoard:
//                    case EventType.CardMoveFromBoard:
                    case EventType.BoardCardTypesChanged:
                    case EventType.BoardClassOfServiceChanged:
                        break;


                    default:
                        System.Diagnostics.Debug.WriteLine("Unknown card operation : " + history[historyPosition].Type);
                        break;
#endif
                }

                historyPosition++;
            }
        }

        private void ApplyFieldChanges(IList<CardEvent.FieldChange> changes)
        {
            foreach(var change in changes)
            {
                switch (change.FieldName.ToLower())
                {
                    case "size":
                        int size;
                        if (int.TryParse(change.OldValue, out size))
                        {
                            cardSize = size != 0 ? size : 1;
                        }
                        break;
#if DEBUG
                    case "priority":
                    case "type":
                    case "title":
                    case "description":
                    case "parentcard":
                    case "class of service":
                    case "tags":
                        break;

                    default:
                        System.Diagnostics.Debug.WriteLine("Unknown card change : " + change.FieldName);
                        break;
#endif
                }
            }
        }
    }
}
