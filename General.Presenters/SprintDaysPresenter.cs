using System;
using PresenterCommon;
using PresenterCommon.Configuration;

namespace General.Presenters
{
    public class SprintDaysPresenter
    {
        private DateTime _startDate;
        private bool _dayOfSprint;
        private int _daysInSprint;

        private General.Model.ISprintDays _sprintDays;
        private object _sprintDaysLock;

        #region SprintDayUpdated event

        public class SprintDayUpdatedEventArgs : EventArgs
        {
            public string SprintDayText { get; private set; }
            public SprintDayUpdatedEventArgs(string text) { SprintDayText = text; }
        }

        public event EventHandler<SprintDayUpdatedEventArgs> SprintDayUpdated;

        protected void OnSprintDayUpdated()
        {
            EventHandler<SprintDayUpdatedEventArgs> ev = SprintDayUpdated;
            if (ev != null)
                ev(this, new SprintDayUpdatedEventArgs(SprintDayText));
        }

        #endregion SprintDayUpdated event

        private void ParseConfiguration(InformationRadiatorItemConfiguration configuration)
        {
            foreach(var item in configuration)
            {
                switch(item.ID.ToLower())
                {
                    case "startdate":
                        DateTime date;
                        if(DateTime.TryParse(item.Value, out date))
                            _startDate = date.Date;
                        break;
                    case "dayofsprint":
                        bool inverted;
                        if (bool.TryParse(item.Value, out inverted))
                            _dayOfSprint = inverted;
                        break;
                    case "daysinsprint":
                        int days;
                        if (int.TryParse(item.Value, out days))
                            _daysInSprint = days;
                        break;
                }
            }
        }

        private void Initialise(IDayUpdateMonitor updateMonitor, InformationRadiatorItemConfiguration configuration)
        {
            _startDate = new DateTime(2014, 8, 19);
            _dayOfSprint = false;
            _daysInSprint = 10;
            ParseConfiguration(configuration);
            updateMonitor.DayChanged += updateMonitor_DayChanged;
            _sprintDaysLock = new object();
        }

        [PresenterCommon.ItemFactory.ItemFactoryConstructor]
        public SprintDaysPresenter(IDayUpdateMonitor updateMonitor, InformationRadiatorItemConfiguration configuration)
        {
            _sprintDays = new General.Model.SprintDays();
            Initialise(updateMonitor, configuration);
        }

        public SprintDaysPresenter(General.Model.ISprintDays sprintDays, IDayUpdateMonitor updateMonitor, InformationRadiatorItemConfiguration configuration)
        {
            _sprintDays = sprintDays;
            Initialise(updateMonitor, configuration);
        }

        void updateMonitor_DayChanged(object sender, EventArgs e)
        {
            OnSprintDayUpdated();
        }

        public string SprintDayText
        {
            get
            {
                lock(_sprintDaysLock)
                {
                    int day;
                    _sprintDays.StartDate = _startDate;
                    _sprintDays.CurrentDate = DateTime.Now.Date;
                    _sprintDays.DaysInSprint = _daysInSprint;
                    if (_dayOfSprint)
                        day = _sprintDays.SprintDay;
                    else
                        day = _sprintDays.DaysInSprint - _sprintDays.SprintDay + 1;
                    return day.ToString();
                }
            }
        }
    }
}
