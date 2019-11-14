using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamCity.Model
{
    public class ReadBuildStatusErrorEventArgs : EventArgs
    {
        public Exception ThrownException { get; private set; }
        public string BuildConfigurationId { get; private set; }
        public ReadBuildStatusErrorEventArgs(Exception thrownException, string buildConfigurationId)
        {
            ThrownException = thrownException;
            BuildConfigurationId = buildConfigurationId;
        }
    }

}
