using System;
using PresenterCommon.Plugin;

namespace TeamCity.Views
{
    public class TeamCityPlugin : IInformationRadiatorItemPlugin
    {
        public string ItemType
        {
            get { return "TeamCity"; }
        }

        public Type PresenterType
        {
            get { return typeof(TeamCity.Presenters.TeamCityPresenter); }
        }


        public Type ViewType
        {
            get { return typeof(TeamCityView); }
        }
    }
}
