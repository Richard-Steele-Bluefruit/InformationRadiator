using System;
using System.Collections.Generic;
using System.Linq;
using PresenterCommon.Configuration;
using LeanKit.Model;

namespace LeanKit.Presenters
{
    public class LeanKitPresenter
    {
        private const double updateInterval = 60000;

        private object _apiLock;
        private ILeanKitPoints _points;
        private LeanKitConfigurationParser _configurationParser;
        private PresenterCommon.ITimer _timer;


        public event EventHandler<ProgressUpdateEventArgs> ProgressUpdate;

        protected void OnProgressUpdate(int readyPoints, int inProgressPoints, int completePoints, int untypedPoints)
        {
            var ev = ProgressUpdate;
            if (ev != null)
                ev(this, new ProgressUpdateEventArgs(readyPoints, inProgressPoints, completePoints, untypedPoints));
        }

        public LeanKitPresenter(InformationRadiatorItemConfiguration configuration)
        {
            _configurationParser = new LeanKitConfigurationParser();
            _configurationParser.ParseConfiguration(configuration);

            _apiLock = new object();

            _points = LeanKitFactory.Instance.CreateLeanKitPoints(_configurationParser.HostName, _configurationParser.UserName, _configurationParser.Password);

            _timer = LeanKitFactory.Instance.CreateTimer(updateInterval);
            _timer.Tick += _timer_Tick;
        }


        private void _timer_Tick(object sender, EventArgs e)
        {
            int readyPoints;
            int inProgressPoints;
            int completePoints;
            int untypedPoints;

            lock (_apiLock)
            {
                _points.Update(_configurationParser.BoardId, _configurationParser.IgnoredLanes);
                readyPoints = _points.ReadyPoints;
                inProgressPoints = _points.InProgressPoints;
                completePoints = _points.CompletePoints;
                untypedPoints = _points.UntypedPoints;
            }

            OnProgressUpdate(readyPoints, inProgressPoints, completePoints, untypedPoints);
        }

        public void ManualUpdate()
        {
            _timer_Tick(this, EventArgs.Empty);
        }

    }
}
