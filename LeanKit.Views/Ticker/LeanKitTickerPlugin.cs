using System;
using PresenterCommon.Plugin;

namespace LeanKit.Views.Ticker
{
    public class LeanKitTickerPlugin : IInformationRadiatorItemPlugin
    {
        public string ItemType
        {
            get { return "LeanKitTicker"; }
        }

        public Type PresenterType
        {
            get { return typeof(LeanKit.Presenters.Ticker.LeanKitTickerPresenter); }
        }

        public Type ViewType
        {
            get { return typeof(LeanKitTickerView); }
        }
    }
}
