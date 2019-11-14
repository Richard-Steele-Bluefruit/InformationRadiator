using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TeamCitySharp.DomainEntities;
using TeamCitySharp.Locators;

namespace TeamCity.Model
{
    public class TeamCityServer : ITeamCityServer
    {
        private TeamCitySharp.ITeamCityClient _client;
        private bool _useDefault;
        private object _lock;

        #region ReadBuildStatusError

        public event EventHandler<ReadBuildStatusErrorEventArgs> ReadBuildStatusError;

        protected void OnReadBuildStatusError(Exception throwException, string buildConfigurationId)
        {
            var ev = ReadBuildStatusError;
            if (ev != null)
                ev(this, new ReadBuildStatusErrorEventArgs(throwException, buildConfigurationId));
        }

        #endregion ReadBuildStatusError

        #region ReadBuildStatusComplete event

        public event EventHandler<ReadBuildStatusCompleteEventArgs> ReadBuildStatusComplete;

        protected void OnReadBuildStatusComplete(BuildStatus status, string buildConfigurationId, string name)
        {
            var ev = ReadBuildStatusComplete;
            if (ev != null)
                ev(this, new ReadBuildStatusCompleteEventArgs(status, buildConfigurationId, name));
        }

        #endregion ReadBuildStatusComplete event

        public TeamCityServer(string hostName, string userName, string password)
        {
            _lock = new object();
            _useDefault = true;
            _client = TeamCitySharpFactory.Instance.Create(hostName, false);
            _client.Connect(userName, password);
        }

        public bool UseDefault
        {
            get
            {
                lock(_lock)
                {
                    return _useDefault;
                }
            }
            set
            {
                lock(_lock)
                {
                    _useDefault = value;
                }
            }
        }

        private void ReadBuildStatus(object buildConfigurationIdObject)
        {
            var buildConfigurationId = buildConfigurationIdObject as string;

            try
            {
                Build buildInformation;
                string name;
                lock (_lock)
                {
                    if (_useDefault)
                    {
                        buildInformation = _client.Builds.LastBuildByBuildConfigId(buildConfigurationId);
                    }
                    else
                    {
                        var locator = BuildLocator.WithDimensions(buildType: BuildTypeLocator.WithId(buildConfigurationId));
                        
                        var builds = _client.Builds.ByBuildLocator(locator);
                        buildInformation = builds != null ? builds.FirstOrDefault() : new Build();
                    }

                    var configuration = _client.BuildConfigs.ByConfigurationId(buildConfigurationId);
                    name = configuration.Name ?? buildConfigurationId;
                }

                OnReadBuildStatusComplete(buildInformation.Status == "SUCCESS" ? BuildStatus.Success : BuildStatus.Failed, buildConfigurationId, name);
            }
            catch(Exception ex)
            {
                OnReadBuildStatusError(ex, buildConfigurationId);
            }
        }

        public void ReadBuildStatusAsync(string buildConfigurationId)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(ReadBuildStatus, buildConfigurationId.Clone());
        }
    }
}
