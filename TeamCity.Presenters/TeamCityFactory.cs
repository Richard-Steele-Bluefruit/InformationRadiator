using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamCity.Presenters
{
    public class TeamCityFactory
    {
        private static TeamCityFactory _instance;

        public static TeamCityFactory Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TeamCityFactory();
                return _instance;
            }
            internal set
            {
                _instance = value;
            }
        }

        public virtual TeamCity.Model.ITeamCityServer CreateServer(string hostName, string userName, string password)
        {
            return new TeamCity.Model.TeamCityServer(hostName, userName, password);
        }

        public virtual PresenterCommon.ITimer CreateTimer(double interval)
        {
            return new PresenterCommon.DotNetTimer(interval);
        }

        public virtual TeamCity.Model.ITeamCityMetricsDownload CreateMetricsDownload(string userName, string password)
        {
            return new TeamCity.Model.TeamCityMetricsDownload(userName, password);
        }
    }
}
