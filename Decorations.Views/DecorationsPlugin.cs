using System;
using PresenterCommon.Plugin;

namespace Decorations.Views
{
    public class SprintDaysPlugin : IInformationRadiatorItemPlugin
    {
        public string ItemType
        {
            get { return "Decorations"; }
        }

        public Type PresenterType
        {
            get { return typeof(Decorations.Presenters.DecorationsPresenter); }
        }


        public Type ViewType
        {
            get { return typeof(DecorationsView); }
        }
    }
}
