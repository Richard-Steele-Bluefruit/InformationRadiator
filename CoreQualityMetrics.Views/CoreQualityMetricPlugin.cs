using System;
using PresenterCommon.Plugin;

namespace CoreQualityMetrics.Views
{
    public class CoreQualityMetricPlugin : IInformationRadiatorItemPlugin
    {
        public string ItemType
        {
            get { return "CoreQualityMetrics"; }
        }

        public Type PresenterType
        {
            get { return typeof(CoreQualityMetrics.Presenters.CoreQualityMetricsPresenter); }
        }


        public Type ViewType
        {
            get { return typeof(CoreQualityMetricView); }
        }
    }
}
