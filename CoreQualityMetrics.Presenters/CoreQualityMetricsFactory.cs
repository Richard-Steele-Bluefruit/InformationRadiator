using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreQualityMetrics.Presenters
{
    public class CoreQualityMetricsFactory
    {
        private static CoreQualityMetricsFactory _instance;

        public static CoreQualityMetricsFactory Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CoreQualityMetricsFactory();
                return _instance;
            }
            internal set
            {
                _instance = value;
            }
        }

        public virtual CoreQualityMetrics.Model.IWebsiteConnection CreateWebsiteConnection(string websiteUrl)
        {
            return new CoreQualityMetrics.Model.WebsiteConnection(websiteUrl);
        }
    }
}
