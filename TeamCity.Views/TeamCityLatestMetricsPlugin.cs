using System;
using PresenterCommon.Plugin;

namespace TeamCity.Views
{
    public class TeamCityLatestMetricsPlugin : IInformationRadiatorItemPlugin
    {
        public string ItemType
        {
            get { return "TeamCityLatestMetrics"; }
        }

        public Type PresenterType
        {
            get { return typeof(TeamCity.Presenters.TeamCityMetricsPresenter); }
        }


        public Type ViewType
        {
            get { return typeof(TeamCityLatestMetricsView); }
        }
    }
}
