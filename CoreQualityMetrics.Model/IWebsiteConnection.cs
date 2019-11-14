namespace CoreQualityMetrics.Model
{
    public interface IWebsiteConnection
    {
        string DownloadProjectsAndMetricsJson();
        string DownloadReleaseReportsListJson();
        string DownloadReleaseReportJson(string project, string version);
    }
}
