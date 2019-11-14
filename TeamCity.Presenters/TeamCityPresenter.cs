using System;
using System.Collections.Generic;
using System.Linq;

using PresenterCommon;
using PresenterCommon.Configuration;

namespace TeamCity.Presenters
{
    public class TeamCityPresenter
    {
        private string _hostName;
        private string _userName;
        private string _password;
        private bool _onlyDefaultBranch;
        private object _configurationsLock;
        private List<string> _configurations;

        private TeamCity.Model.ITeamCityServer _server;
        private PresenterCommon.ITimer _timer;

        #region BuildStatusUpdate event

        public event EventHandler<BuildStatusUpdateEventArgs> BuildStatusUpdate;

        protected void OnBuildStatusUpdate(string buildConfiguration, BuildStatus status, string name)
        {
            var ev = BuildStatusUpdate;
            if (ev != null)
                ev(this, new BuildStatusUpdateEventArgs(buildConfiguration, status, name));
        }

        #endregion BuildStatusUpdate event

        public TeamCityPresenter(InformationRadiatorItemConfiguration configuration)
        {
            _onlyDefaultBranch = true;
            _configurationsLock = new object();
            ParseConfiguration(configuration);

            _server = TeamCityFactory.Instance.CreateServer(_hostName, _userName, _password);
            _server.ReadBuildStatusComplete += _server_ReadBuildStatusComplete;
            _server.ReadBuildStatusError += _server_ReadBuildStatusError;
            _server.UseDefault = _onlyDefaultBranch;

            _timer = TeamCityFactory.Instance.CreateTimer(30000);
            _timer.Tick += _timer_Tick;
        }

        private void SetDefaultConfiguration()
        {
            _configurations = new List<string>();
            _hostName = "localhost";
            _userName = "guest";
            _password = "password";
        }

        private void ParseConfiguration(InformationRadiatorItemConfiguration configuration)
        {
            SetDefaultConfiguration();

            foreach(var item in configuration)
            {
                switch(item.ID.ToLower())
                {
                    case "hostname":
                        _hostName = item.Value;
                        break;
                    case "username":
                        _userName = item.Value;
                        break;
                    case "password":
                        _password = item.Value;
                        break;
                    case "buildconfiguration":
                        _configurations.Add(item.Value);
                        break;
                    case "onlydefaultbranch":
                        bool onlyDefaultBranch;
                        if(bool.TryParse(item.Value, out onlyDefaultBranch))
                        {
                            _onlyDefaultBranch = onlyDefaultBranch;
                        }
                        break;
                }
            }
        }

        public List<string> Configurations
        {
            get
            {
                List<string> clone;
                lock(_configurationsLock)
                {
                    clone = _configurations.Select(item => (string)item.Clone()).ToList();
                }
                return clone;
            }
        }

        public void ManualUpdate()
        {
            _timer_Tick(this, EventArgs.Empty);
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            lock (_configurationsLock)
            {
                foreach(var configuration in _configurations)
                {
                    _server.ReadBuildStatusAsync(configuration);
                }
            }
        }

        private void _server_ReadBuildStatusComplete(object sender, Model.ReadBuildStatusCompleteEventArgs e)
        {
            var status = e.Status == Model.BuildStatus.Success ? BuildStatus.Success : BuildStatus.Failed;
            OnBuildStatusUpdate(e.BuildConfigurationId, status, e.Name);
        }

        private void _server_ReadBuildStatusError(object sender, Model.ReadBuildStatusErrorEventArgs e)
        {
            OnBuildStatusUpdate(e.BuildConfigurationId, BuildStatus.Unknown, e.BuildConfigurationId);
        }

    }
}
