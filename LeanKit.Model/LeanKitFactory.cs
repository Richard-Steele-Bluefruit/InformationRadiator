using LeanKit.API.Client.Library;
using LeanKit.API.Client.Library.TransferObjects;

namespace LeanKit.Model
{
    public class LeanKitFactory
    {
        private static LeanKitFactory _instance;

        public static LeanKitFactory Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LeanKitFactory();
                return _instance;
            }
            internal set
            {
                _instance = value;
            }
        }

        public virtual ILeanKitApi CreateApi(string hostName, string userName, string password)
        {
            var factory = new LeanKitClientFactory();
            var authorisation = new LeanKitBasicAuth
            {
                Hostname = hostName,
                Username = userName,
                Password = password
            };
            return factory.Create(authorisation);
        }

    }
}
