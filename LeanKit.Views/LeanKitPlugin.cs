using System;
using PresenterCommon.Plugin;

namespace LeanKit.Views
{
    public class LeanKitPlugin : IInformationRadiatorItemPlugin
    {
        public string ItemType
        {
            get { return "LeanKit"; }
        }

        public Type PresenterType
        {
            get { return typeof(LeanKit.Presenters.LeanKitPresenter); }
        }

        public Type ViewType
        {
            get { return typeof(LeanKitView); }
        }
    }
}
