using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamCity.Model.Tests
{
    internal class TeamCitySharpFactoryMock : TeamCitySharpFactory
    {
        public TeamCitySharp.ITeamCityClient _client;
        public string _createHost;
        public bool? _creaseUseSsl = null;

        public override TeamCitySharp.ITeamCityClient Create(string hostName, bool useSsl)
        {
            var result = _client;
            _client = null;
            _createHost = hostName;
            _creaseUseSsl = useSsl;
            return result;
        }
    }
}
