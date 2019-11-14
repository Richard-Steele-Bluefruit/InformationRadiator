using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LeanKit.Model.Ticker;
using PresenterCommon.Configuration;

namespace LeanKit.Presenters.Ticker
{
    public class LeanKitTickerPresenter
    {
        private const string daysUntilSearch = "<days_until>";
        private const string daysFromSearch = "<day_number>";

        private LeanKitConfigurationParser _configurationParser;
        private long _laneId;
        private int _displayUpdateInterval;
        private int _fetchUpdateInterval;

        private object _lock;

        private IList<LeanKitTickerMessage> _messages;
        private int _currentMessage;
        private uint _messageStartTicks;

        private uint _fetchStartTicks;
        
        private uint _ticks;

        private ILeanKitTicker _ticker;
        private PresenterCommon.ITimer _timer;

        public event EventHandler<TickerUpdateEventArgs> TickerUpdate;

        protected void OnTickerUpdate(string message)
        {
            var ev = TickerUpdate;
            if(ev != null)
            {
                ev(this, new TickerUpdateEventArgs(message));
            }
        }

        public LeanKitTickerPresenter(InformationRadiatorItemConfiguration configuration)
        {
            _lock = new object();

            _displayUpdateInterval = 10;
            _fetchUpdateInterval = 5 * 60;

            _configurationParser = new LeanKitConfigurationParser();
            _configurationParser.UnknownConfigurationParameter += _configurationParser_UnknownConfigurationParameter;
            _configurationParser.ParseConfiguration(configuration);

            _currentMessage = 0;
            _messageStartTicks = 0;
            _ticks = 0;

            _ticker = LeanKitFactory.Instance.CreateTicker(_configurationParser.HostName, _configurationParser.UserName, _configurationParser.Password, _configurationParser.BoardId, _laneId);
            _timer = LeanKitFactory.Instance.CreateTimer(1000);
            _timer.Tick += _timer_Tick;
        }

        private void _configurationParser_UnknownConfigurationParameter(object sender, LeanKitConfigurationParser.UnknownConfigurationParameterEventArgs e)
        {
            switch (e.ID.ToLower())
            {
                case "laneid":
                    long value;
                    if (long.TryParse(e.Value, out value))
                    {
                        _laneId = value;
                    }
                    break;
                case "displayupdateinterval":
                    int ui;
                    if (int.TryParse(e.Value, out ui))
                    {
                        _displayUpdateInterval = ui;
                    }
                    break;
                case "fetchupdateinterval":
                    int fui;
                    if (int.TryParse(e.Value, out fui))
                    {
                        _fetchUpdateInterval = fui;
                    }
                    break;
            }
        }

        private string FormatCurrentMessage()
        {
            var now = DateTime.Now;
            var dueDate = _messages[_currentMessage].DueDate ?? now;
            var span = dueDate - now;
            var days = span.Days;
            if (((double)span.Days) < span.TotalDays)
            {
                days++;
            }
            var message = _messages[_currentMessage].Message.Replace(daysUntilSearch, days.ToString());

            var startDate = _messages[_currentMessage].StartDate ?? now;
            var daysFromSpan = now - startDate;
            var daysFrom = daysFromSpan.Days + 1; // Start Date shown as Day 1
            message = message.Replace(daysFromSearch, daysFrom.ToString());
            return message;
        }

        private void ShowCurrentMessage()
        {
            if (_messages != null)
            {
                if (_currentMessage >= _messages.Count)
                {
                    _currentMessage = 0;
                }

                if (_currentMessage < _messages.Count)
                {
                    OnTickerUpdate(FormatCurrentMessage());
                }
                else
                {
                    OnTickerUpdate("No messages");
                }
            }
            else
            {
                OnTickerUpdate("Error: Messages have not been downloaded");
            }
        }

        private void CheckForMessageChange()
        {
            var timeSinceLastChange = unchecked(_ticks - _messageStartTicks);
            if(timeSinceLastChange >= _displayUpdateInterval)
            {
                _messageStartTicks = _ticks;

                _currentMessage++;
                ShowCurrentMessage();
            }
        }

        private void CheckForFetchMessages()
        {
            var timeSinceLastChange = unchecked(_ticks - _fetchStartTicks);
            if (timeSinceLastChange >= _fetchUpdateInterval)
            {
                _fetchStartTicks = _ticks;

                Update();
            }
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            lock (_lock)
            {
                unchecked
                {
                     _ticks++;
                }
                CheckForFetchMessages();
                CheckForMessageChange();
            }
        }

        public void Update()
        {
            System.Threading.ThreadPool.QueueUserWorkItem((m) =>
            {
                var messages = _ticker.GetMessages();
                List<LeanKitTickerMessage> filteredMessages = null;
                if(messages != null)
                {
                    var now = DateTime.Now.Date;
                    filteredMessages = (from e in messages
                                        where e.DueDate == null || (e.DueDate ?? now).Date >= now
                                        where e.StartDate == null || (e.StartDate ?? now).Date <= now
                                        select e).ToList();
                }
                                       
                lock(_lock)
                {
                    _messages = filteredMessages;
                    ShowCurrentMessage();
                }
            });
        }
    }
}
