using System.Net;
using System.Text;

namespace CoreQualityMetrics.Model
{
    public class WebsiteConnection : IWebsiteConnection
    {
        private WebClient client;
        private string url;

        public WebsiteConnection(string websiteUrl)
        {
            url = websiteUrl;
            if (!url.EndsWith("/"))
            {
                url += "/";
            }
            url += "api/v2.0.0/";
            client = new WebClient();
        }

        public string DownloadProjectsAndMetricsJson()
        {
            var data = client.DownloadData(url + "projects_and_metrics.php");
            return ASCIIEncoding.ASCII.GetString(data);
        }

        public string DownloadReleaseReportsListJson()
        {
            var data = client.DownloadData(url + "list_release_reports.php");
            return ASCIIEncoding.ASCII.GetString(data);
        }

        public string DownloadReleaseReportJson(string project, string version)
        {
            project = System.Web.HttpUtility.UrlEncode(project);
            version = System.Web.HttpUtility.UrlEncode(version);
            var data = client.DownloadData(url + "release_report.php?project=" + project + "&version=" + version);
            return ASCIIEncoding.ASCII.GetString(data);
        }
    }
}
