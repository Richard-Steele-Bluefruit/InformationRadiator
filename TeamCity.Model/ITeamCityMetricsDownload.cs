using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamCity.Model
{
    public interface ITeamCityMetricsDownload
    {
        string DownloadMetrics(string url);
    }
}
