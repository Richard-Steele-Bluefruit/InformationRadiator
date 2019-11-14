using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LeanKit.API.Client.Library;
using LeanKit.API.Client.Library.TransferObjects;

namespace LeanKit.Model.Ticker
{
    public class LeanKitTicker : ILeanKitTicker
    {
        private ILeanKitApi _api;
        private long _boardId;
        private long _laneId;

        public LeanKitTicker(string hostName, string userName, string password, long boardId, long laneId)
        {
            _api = LeanKitFactory.Instance.CreateApi(hostName, userName, password);
            _boardId = boardId;
            _laneId = laneId;
        }

        public IList<LeanKitTickerMessage> GetMessages()
        {
            var messages = new List<LeanKitTickerMessage>();
            try
            {
                var board = _api.GetBoard(_boardId);
                if (board != null)
                {
                    var lane = board.Lanes.FirstOrDefault(p => ((p.Id != null) && (p.Id == _laneId)));
                    if (lane != null)
                    {
                        messages.AddRange(from c in lane.Cards
                                          select new LeanKitTickerMessage { Message = c.Title, StartDate = c.ConvertedStartDate(), DueDate = c.ConvertedDueDate() });

                    }
                }

                return messages;
            }
            catch
            {
                return null;
            }
        }
    }
}
