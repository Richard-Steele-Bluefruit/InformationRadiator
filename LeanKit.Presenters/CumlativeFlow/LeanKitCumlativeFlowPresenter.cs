using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Globalization;
using PresenterCommon.Configuration;
using LeanKit.Model;
using LeanKit.Model.LaneHistory;

namespace LeanKit.Presenters.CumlativeFlow
{
    public class LeanKitCumlativeFlowPresenter
    {
        private LeanKitConfigurationParser _configurationParser;
        private ILeanKitLanePointsHistory _history;

        public int NumberOfDaysHistory { get; private set; }

        private Color _readyStartColour = Color.FromArgb(200, 0x00, 0x00);
        private Color _readyEndColour = Color.FromArgb(150, 0x00, 0x00);
        private Color _inProcessStartColour = Color.FromArgb(0x00, 0x00, 200);
        private Color _inProcessEndColour = Color.FromArgb(0x00, 0x00, 150);
        private Color _completeStartColour = Color.FromArgb(0x00, 200, 0x00);
        private Color _completeEndColour = Color.FromArgb(0x00, 150, 0x00);

        public event EventHandler<CumaltiveFlowDataUpdateEventArgs> CumaltiveFlowDataUpdate;

        private void OnCumaltiveFlowDataUpdate(CumaltiveFlowDataUpdateEventArgs eventArgs)
        {
            var ev = CumaltiveFlowDataUpdate;
            if(ev!=null)
            {
                ev(this, eventArgs);
            }
        }

        public LeanKitCumlativeFlowPresenter(InformationRadiatorItemConfiguration configuration, PresenterCommon.IDayUpdateMonitor updateMonitor)
        {
            NumberOfDaysHistory = 10;
            _configurationParser = new LeanKitConfigurationParser();
            _configurationParser.UnknownConfigurationParameter += _configurationParser_UnknownConfigurationParameter;
            _configurationParser.ParseConfiguration(configuration);

            _history = LeanKitFactory.Instance.CreateLanePointsHistory(_configurationParser.HostName, _configurationParser.UserName, _configurationParser.Password, _configurationParser.BoardId, _configurationParser.IgnoredLanes);

            updateMonitor.DayChanged += updateMonitor_DayChanged;
        }

        private Color ConvertColour(string configurationValue, Color originalColour)
        {
            int argb;
            Color color = originalColour;
            if (Int32.TryParse("FF" + configurationValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out argb))
            {
                color = Color.FromArgb(argb);
            }
            return color;
        }

        private void _configurationParser_UnknownConfigurationParameter(object sender, LeanKitConfigurationParser.UnknownConfigurationParameterEventArgs e)
        {
            switch(e.ID.ToLower())
            {
                case "numberofdays":
                    int value;
                    if (int.TryParse(e.Value, out value))
                    {
                        NumberOfDaysHistory = value;
                    }
                    break;
                case "readystartcolour":
                    _readyStartColour = ConvertColour(e.Value, _readyStartColour);
                    break;
                case "readyendcolour":
                    _readyEndColour = ConvertColour(e.Value, _readyEndColour);
                    break;
                case "inprocessstartcolour":
                    _inProcessStartColour = ConvertColour(e.Value, _inProcessStartColour);
                    break;
                case "inprocessendcolour":
                    _inProcessEndColour = ConvertColour(e.Value, _inProcessEndColour);
                    break;
                case "completestartcolour":
                    _completeStartColour = ConvertColour(e.Value, _completeStartColour);
                    break;
                case "completeendcolour":
                    _completeEndColour = ConvertColour(e.Value, _completeEndColour);
                    break;
            }
        }

        private CumlativeFlowLaneType ConvertLaneType(TypeOfLane type)
        {
            switch (type)
            {
                case TypeOfLane.Ready:
                    return CumlativeFlowLaneType.Ready;
                case TypeOfLane.Completed:
                    return CumlativeFlowLaneType.Completed;
                case TypeOfLane.InProcess:
                    return CumlativeFlowLaneType.InProcess;
                default:
                    return CumlativeFlowLaneType.Untyped;
            }
        }

        private void Update(object notUsed)
        {
            _history.Update(NumberOfDaysHistory);
            var eventArgs = new CumaltiveFlowDataUpdateEventArgs(_readyStartColour, _readyEndColour,
                                                                 _inProcessStartColour, _inProcessEndColour,
                                                                 _completeStartColour, _completeEndColour);

            foreach (var lane in _history.LaneHistory)
            {
                var details = new CumlativeFlowLaneData(lane.Title, new List<int>(), type: ConvertLaneType(lane.Type));
                foreach (var point in lane.PointsPerDay)
                {
                    details.PointsPerDay.Insert(0, point);
                }
                eventArgs.Lanes.Add(details);
            }

            OnCumaltiveFlowDataUpdate(eventArgs);

        }

        private void updateMonitor_DayChanged(object sender, EventArgs e)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(Update);
        }

        public void ForceUpdate()
        {
            updateMonitor_DayChanged(this, EventArgs.Empty);
        }
    }
}
