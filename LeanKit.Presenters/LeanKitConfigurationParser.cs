using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PresenterCommon.Configuration;

namespace LeanKit.Presenters
{
    public class LeanKitConfigurationParser
    {
        public string HostName { get; private set; }
        public string UserName { get; private set; }
        public string Password { get; private set; }
        public long BoardId { get; private set; }

        public List<long> IgnoredLanes { get; private set; }

        public class UnknownConfigurationParameterEventArgs : EventArgs
        {
            public UnknownConfigurationParameterEventArgs(string id, string value)
            {
                ID = id;
                Value = value;
            }

            public string ID { get; private set; }
            public string Value { get; private set; }
        }

        public event EventHandler<UnknownConfigurationParameterEventArgs> UnknownConfigurationParameter;

        private void OnUnknownConfigurationParameter(string id, string value)
        {
            var ev = UnknownConfigurationParameter;
            if(ev != null)
            {
                ev(this, new UnknownConfigurationParameterEventArgs(id, value));
            }
        }

        public LeanKitConfigurationParser()
        {
            IgnoredLanes = new List<long>();
        }

        public void ParseConfiguration(InformationRadiatorItemConfiguration configuration)
        {
            foreach (var item in configuration)
            {
                switch (item.ID.ToLower())
                {
                    case "hostname":
                        HostName = item.Value;
                        break;
                    case "username":
                        UserName = item.Value;
                        break;
                    case "password":
                        Password = item.Value;
                        break;
                    case "boardid":
                        long id;
                        if (long.TryParse(item.Value, out id))
                        {
                            BoardId = id;
                        }
                        break;
                    case "ignorelaneid":
                        long laneId;
                        if (long.TryParse(item.Value, out laneId))
                        {
                            IgnoredLanes.Add(laneId);
                        }
                        break;
                    default:
                        OnUnknownConfigurationParameter(item.ID, item.Value);
                        break;
                }
            }
        }

    }
}
