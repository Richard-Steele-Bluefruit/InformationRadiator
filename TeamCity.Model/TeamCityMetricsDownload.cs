using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EasyHttp.Http;

namespace TeamCity.Model
{
    public class TeamCityMetricsDownload : ITeamCityMetricsDownload
    {
        private string _userName;
        private string _password;

        public TeamCityMetricsDownload(string userName, string password)
        {
            _userName = userName;
            _password = password;
        }

        private EasyHttp.Http.HttpClient CreateEasyHttpClient()
        {
            var httpClient = new EasyHttp.Http.HttpClient();
            httpClient.Request.Accept = HttpContentTypes.TextCsv;
            httpClient.Request.SetBasicAuthentication(_userName, _password);
            httpClient.Request.ForceBasicAuth = true;

            return httpClient;
        }

        public string DownloadMetrics(string url)
        {
            try
            {
                var client = CreateEasyHttpClient();
                var response = client.Get(url);
                if(response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return response.RawText;
                }
                return "";
            }
            catch
            {
                return "";
            }
        }
    }
}
