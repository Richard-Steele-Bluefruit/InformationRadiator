using System;
using PresenterCommon.Plugin;


namespace TeamCity.Views
{
    public class TeamCityMetricsPlugin : IInformationRadiatorItemPlugin
    {
        public string ItemType
        {
            get { return "TeamCityMetrics"; }
        }

        public Type PresenterType
        {
            get { return typeof(TeamCity.Presenters.TeamCityMetricsPresenter); }
        }


        public Type ViewType
        {
            get { return typeof(TeamCityMetricsView); }
        }
    }
}
