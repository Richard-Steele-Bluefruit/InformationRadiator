using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamCity.Model
{
    public interface ITeamCityServer
    {
        event EventHandler<ReadBuildStatusErrorEventArgs> ReadBuildStatusError;
        event EventHandler<ReadBuildStatusCompleteEventArgs> ReadBuildStatusComplete;
        bool UseDefault { get; set; }
        void ReadBuildStatusAsync(string buildConfigurationId);
    }
}
