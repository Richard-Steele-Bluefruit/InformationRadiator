using System;
using PresenterCommon.Plugin;

namespace LeanKit.Views.CumlativeFlow
{
    public class LeanKitCumlativeFlowPlugin : IInformationRadiatorItemPlugin
    {
        public string ItemType
        {
            get { return "LeanKitCumlativeFlow"; }
        }

        public Type PresenterType
        {
            get { return typeof(LeanKit.Presenters.CumlativeFlow.LeanKitCumlativeFlowPresenter); }
        }

        public Type ViewType
        {
            get { return typeof(LeanKitCumlativeFlowView); }
        }
    }
}
