using LeanKit.API.Client.Library;

namespace LeanKit.Model.Tests
{
    class LeanKitFactoryMock : LeanKitFactory
    {
        public ILeanKitApi _api;

        public string _hostName;
        public string _userName;
        public string _password;

        public override ILeanKitApi CreateApi(string hostName, string userName, string password)
        {
            var api = _api;
            _api = null;
            _hostName = hostName;
            _userName = userName;
            _password = password;
            return api;
        }
    }
}
