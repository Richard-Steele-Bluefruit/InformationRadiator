using System;

namespace TeamCity.Model
{
    public class TeamCitySharpFactory
    {
        private static TeamCitySharpFactory _instance;

        public static TeamCitySharpFactory Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TeamCitySharpFactory();
                return _instance;
            }
            internal set
            {
                _instance = value;
            }
        }

        public virtual TeamCitySharp.ITeamCityClient Create(string hostName, bool useSsl)
        {
            return new TeamCitySharp.TeamCityClient(hostName, useSsl);
        }
    }
}
