using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamCity.Presenters.Tests
{
    class TeamCityFactoryMock : TeamCityFactory
    {
        public TeamCity.Model.ITeamCityServer _server;

        public string _hostName;
        public string _userName;
        public string _password;


        public override TeamCity.Model.ITeamCityServer CreateServer(string hostName, string userName, string password)
        {
            var result = _server;
            _server = null;
            _hostName = hostName;
            _userName = userName;
            _password = password;
            return result;
        }

        public PresenterCommon.ITimer _timer;
        public double _interval;

        public override PresenterCommon.ITimer CreateTimer(double interval)
        {
            var result = _timer;
            _timer = null;
            _interval = interval;
            return result;
        }

        public TeamCity.Model.ITeamCityMetricsDownload _metricsDownload;

        public string _metricsUserName;
        public string _metricsPassword;

        public override TeamCity.Model.ITeamCityMetricsDownload CreateMetricsDownload(string userName, string password)
        {
            _metricsUserName = userName;
            _metricsPassword = password;

            return _metricsDownload;
        }

    }
}
