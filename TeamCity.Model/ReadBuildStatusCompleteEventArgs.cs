using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamCity.Model
{
    public class ReadBuildStatusCompleteEventArgs : EventArgs
    {
        public BuildStatus Status { get; private set; }
        public string BuildConfigurationId { get; private set; }
        public string Name { get; private set; }
        public ReadBuildStatusCompleteEventArgs(BuildStatus status, string buildConfigurationId, string name)
        {
            Status = status;
            BuildConfigurationId = buildConfigurationId;
            Name = name;
        }
    }

}
