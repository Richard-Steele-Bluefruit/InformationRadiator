using System;
using PresenterCommon.Plugin;

namespace General.Views
{
    public class SprintDaysPlugin : IInformationRadiatorItemPlugin
    {
        public string ItemType
        {
            get { return "SprintDays"; }
        }

        public Type PresenterType
        {
            get { return typeof(General.Presenters.SprintDaysPresenter); }
        }


        public Type ViewType
        {
            get { return typeof(SprintDaysView); }
        }
    }
}
