using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamCity.Presenters
{
    public class BuildStatusUpdateEventArgs : EventArgs
    {
        public string BuildConfiguration { get; private set; }
        public BuildStatus Status { get; private set; }
        public string Name { get; private set; }
        internal BuildStatusUpdateEventArgs(string buildConfiguration, BuildStatus status, string name)
        {
            BuildConfiguration = buildConfiguration;
            Status = status;
            Name = name;
        }
    }
}
