using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using PresenterCommon.Configuration;

namespace TeamCity.Presenters.Tests
{
    [TestClass]
    public class TeamCityPresenterTests
    {
        TeamCityFactoryMock _factory;
        Mock<TeamCity.Model.ITeamCityServer> _mockServer;
        Mock<PresenterCommon.ITimer> _mockTimer;

        #region Helper Methods

        [TestCleanup]
        public void CleanUp()
        {
            TeamCityFactory.Instance = null;
        }

        private InformationRadiatorItemConfiguration CreateConfiguration(string hostName = "Host", string userName = "User", string password = "password")
        {
            var configuration = new InformationRadiatorItemConfiguration();
            configuration.Add(new InformationRadiatorItemConfigurationField() { ID = "HostName", Value = hostName });
            configuration.Add(new InformationRadiatorItemConfigurationField() { ID = "UserName", Value = userName });
            configuration.Add(new InformationRadiatorItemConfigurationField() { ID = "Password", Value = password });
            return configuration;
        }

        private void CreateMockFactory(bool useDefaultBranch = true)
        {
            _factory = new TeamCityFactoryMock();
            TeamCityFactory.Instance = _factory;
            
            _mockServer = new Mock<TeamCity.Model.ITeamCityServer>(MockBehavior.Strict);
            _factory._server = _mockServer.Object;

            _mockTimer = new Mock<PresenterCommon.ITimer>(MockBehavior.Strict);
            _factory._timer = _mockTimer.Object;

            _mockServer.SetupSet(m => m.UseDefault = It.Is<bool>(v => v == useDefaultBranch));
        }

        #endregion Helper Methods

        [TestMethod]
        public void Setting_the_server_and_login_details_via_the_configuration()
        {
            // Given
            string expectedHostName = "Gladys";
            string expectedUserName = "byran";
            string expectedPassword = "usualPassword";
            var configuration = CreateConfiguration(expectedHostName, expectedUserName, expectedPassword);
            CreateMockFactory();

            // When
            var target = new TeamCityPresenter(configuration);

            // Then
            Assert.AreEqual(expectedHostName, _factory._hostName);
            Assert.AreEqual(expectedUserName, _factory._userName);
            Assert.AreEqual(expectedPassword, _factory._password);
            _mockServer.VerifyAll();
        }

        [TestMethod]
        public void No_server_and_login_details_in_the_configuration()
        {
            // Given
            string expectedHostName = "localhost";
            string expectedUserName = "guest";
            string expectedPassword = "password";
            var configuration = new InformationRadiatorItemConfiguration();
            CreateMockFactory();

            // When
            var target = new TeamCityPresenter(configuration);

            // Then
            Assert.AreEqual(expectedHostName, _factory._hostName);
            Assert.AreEqual(expectedUserName, _factory._userName);
            Assert.AreEqual(expectedPassword, _factory._password);
            _mockServer.VerifyAll();
        }

        [TestMethod]
        public void Setting_the_build_configuration_details_in_the_configuration()
        {
            // Given
            string[] expectedConfigurations = new string[] { "Demo_A", "Hemo_Matt", "Test_XYZ" };
            var configuration = CreateConfiguration();
            CreateMockFactory();
            foreach(var configurationId in expectedConfigurations)
            {
                configuration.Add(new InformationRadiatorItemConfigurationField() { ID = "BuildConfiguration", Value = configurationId });
            }

            // When
            var target = new TeamCityPresenter(configuration);

            // Then
            foreach (var configurationId in expectedConfigurations)
            {
                Assert.IsTrue(target.Configurations.Any(s => s == configurationId));
            }
            _mockServer.VerifyAll();
        }

        private void Reading_the_build_status_of_a_single_XYZ_build_configuration(
            string expectedConfiguration, string expectedName,
            Model.BuildStatus serverStatus, BuildStatus expectedStatus)
        {
            // Given
            var configuration = CreateConfiguration();
            CreateMockFactory();
            configuration.Add(new InformationRadiatorItemConfigurationField() { ID = "BuildConfiguration", Value = expectedConfiguration });

            var target = new TeamCityPresenter(configuration);

            string actualConfiguration = null;
            BuildStatus actualStatus = BuildStatus.Unknown;
            string actualName = null;

            target.BuildStatusUpdate += (sender, e) =>
            {
                actualConfiguration = e.BuildConfiguration;
                actualStatus = e.Status;
                actualName = e.Name;
            };

            _mockServer.Setup(m => m.ReadBuildStatusAsync(expectedConfiguration));

            // When
            _mockTimer.Raise(m => m.Tick += null, EventArgs.Empty);

            // Then
            // All calls to the server should be done by now
            _mockServer.VerifyAll();

            // When
            // Fake the server returning the build status
            _mockServer.Raise(m => m.ReadBuildStatusComplete += null,
                new TeamCity.Model.ReadBuildStatusCompleteEventArgs(
                    serverStatus, expectedConfiguration, expectedName));

            Assert.AreEqual((double)30000, _factory._interval, (double)0.1);
            Assert.AreEqual(expectedConfiguration, actualConfiguration);
            Assert.AreEqual(expectedStatus, actualStatus);
            Assert.AreEqual(expectedName, actualName);
            _mockTimer.VerifyAll();
        }

        [TestMethod]
        public void Reading_the_build_status_of_a_single_Successful_build_configuration()
        {
            Reading_the_build_status_of_a_single_XYZ_build_configuration("Hello_World", "Boo",
                Model.BuildStatus.Success, BuildStatus.Success);
        }

        [TestMethod]
        public void Reading_the_build_status_of_a_single_Failed_build_configuration()
        {
            Reading_the_build_status_of_a_single_XYZ_build_configuration("Simple_Test", "Hoo",
                Model.BuildStatus.Failed, BuildStatus.Failed);
        }

        private void Reading_the_build_status_of_a_multiple_build_configuration(
            string[] expectedConfigurations,
            string[] expectedNames,
            Model.BuildStatus[] serverStatus,
            BuildStatus[] expectedStatus,
            Exception[] throwException = null)
        {
            // Given
            var configuration = CreateConfiguration();
            CreateMockFactory();
            foreach (var config in expectedConfigurations)
                configuration.Add(new InformationRadiatorItemConfigurationField() { ID = "BuildConfiguration", Value = config });

            var target = new TeamCityPresenter(configuration);

            string actualConfiguration = null;
            BuildStatus actualStatus = BuildStatus.Unknown;
            string actualName = null;

            target.BuildStatusUpdate += (sender, e) =>
            {
                actualConfiguration = e.BuildConfiguration;
                actualStatus = e.Status;
                actualName = e.Name;
            };

            foreach (var config in expectedConfigurations)
                _mockServer.Setup(m => m.ReadBuildStatusAsync(config));

            // When
            _mockTimer.Raise(m => m.Tick += null, EventArgs.Empty);

            // Then
            // All calls to the server should be done by now
            _mockServer.VerifyAll();

            // When
            // Fake the server returning the build status
            for (int i = 0; i < expectedConfigurations.Length; i++)
            {
                actualConfiguration = null;
                actualStatus = BuildStatus.Unknown;

                if (throwException == null || throwException[i] == null)
                {
                    _mockServer.Raise(m => m.ReadBuildStatusComplete += null,
                        new TeamCity.Model.ReadBuildStatusCompleteEventArgs(
                            serverStatus[i], expectedConfigurations[i], expectedNames[i]));
                }
                else
                {
                    _mockServer.Raise(m => m.ReadBuildStatusError += null,
                        new TeamCity.Model.ReadBuildStatusErrorEventArgs(
                            throwException[i], expectedConfigurations[i]));
                }

                Assert.AreEqual(expectedStatus[i], actualStatus);
                Assert.AreEqual(expectedConfigurations[i], actualConfiguration);
                Assert.AreEqual(expectedNames[i], actualName);
            }

            _mockTimer.VerifyAll();
        }

        [TestMethod]
        public void Reading_the_build_status_of_a_multiple_build_configuration()
        {
            Reading_the_build_status_of_a_multiple_build_configuration(
                expectedConfigurations: new string[] { "Hello_World", "Test_Build" },
                expectedNames: new string[] {"Left", "Right" },
                serverStatus: new Model.BuildStatus[] { Model.BuildStatus.Success, Model.BuildStatus.Failed },
                expectedStatus: new BuildStatus[] { BuildStatus.Success, BuildStatus.Failed });
        }

        [TestMethod]
        public void Reading_the_build_status_with_an_error_from_the_server()
        {
            Reading_the_build_status_of_a_multiple_build_configuration(
                expectedConfigurations: new string[] { "Hello_World", "Test_Build", },
                expectedNames: new string[] { "Hello_World", "Test_Build" },
                serverStatus: new Model.BuildStatus[] { Model.BuildStatus.Success, Model.BuildStatus.Failed },
                expectedStatus: new BuildStatus[] { BuildStatus.Success, BuildStatus.Unknown },
                throwException: new Exception[] { null, new Exception() });
        }

        [TestMethod]
        public void Setting_to_not_use_the_default_branch_in_the_build_configuration()
        {
            // Given
            var configuration = CreateConfiguration();
            CreateMockFactory(useDefaultBranch: false);

            configuration.Add(new InformationRadiatorItemConfigurationField() { ID = "OnlyDefaultBranch", Value = "false" });

            // When
            var target = new TeamCityPresenter(configuration);

            // Then
            _mockServer.VerifyAll();
        }
    }
}
