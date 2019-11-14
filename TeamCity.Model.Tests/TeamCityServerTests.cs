using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using TeamCitySharp.DomainEntities;
using TeamCitySharp.Locators;

namespace TeamCity.Model.Tests
{
    [TestClass]
    public class TeamCityServerTests
    {
        [TestCleanup]
        public void CleanUp()
        {
            TeamCitySharpFactory.Instance = null;
        }

        private TeamCitySharpFactoryMock mockFactory;
        private Mock<TeamCitySharp.ITeamCityClient> clientMock;

        #region CreateClient

        private void CreateClient(string userName = "Username", string password = "Password")
        {
            mockFactory = new TeamCitySharpFactoryMock();
            TeamCitySharpFactory.Instance = mockFactory;

            clientMock = new Mock<TeamCitySharp.ITeamCityClient>(MockBehavior.Strict);
            clientMock.Setup(m => m.Connect(userName, password));

            mockFactory._client = clientMock.Object;
        }

        #endregion CreateClient

        [TestMethod]
        public void Connecting_to_a_server()
        {
            // Given
            string hostName = "localhost";
            string userName = "byran";
            string password = "HelloWorld";

            CreateClient(userName, password);

            // When
            var server = new TeamCityServer(hostName, userName, password);

            // Then
            Assert.AreEqual(hostName, mockFactory._createHost);
            Assert.AreEqual(false, mockFactory._creaseUseSsl);

            clientMock.VerifyAll();
        }

        #region ExpectReadOfBuildStatus

        private Mock<TeamCitySharp.ActionTypes.IBuilds> ExpectReadOfBuildStatus(string expectedBuildConfigurationId,
            Build expectedBuildInfo, Exception thrownException = null,
            BuildLocator locator = null)
        {
            var mockBuilds = new Mock<TeamCitySharp.ActionTypes.IBuilds>(MockBehavior.Strict);

            // The builds property will be accessed to get the build status
            clientMock.Setup(m => m.Builds).Returns(mockBuilds.Object);

            // This will read the build status
            if (locator == null)
            {
                if (thrownException == null)
                    mockBuilds.Setup(m => m.LastBuildByBuildConfigId(expectedBuildConfigurationId)).Returns(expectedBuildInfo);
                else
                    mockBuilds.Setup(m => m.LastBuildByBuildConfigId(expectedBuildConfigurationId)).Throws(thrownException);
            }
            else
            {
                mockBuilds.Setup(m =>
                    m.ByBuildLocator(It.Is<BuildLocator>(data => data.ToString() == locator.ToString())))
                    .Returns(new List<Build>() { expectedBuildInfo });
            }

            return mockBuilds;
        }

        #endregion ExpectReadOfBuildStatus

        #region ExpectReadOfBuildConfiguration

        void ExpectReadOfBuildConfiguration(string expectedBuildConfigurationId, string buildName)
        {
            var mockBuildConfigs = new Mock<TeamCitySharp.ActionTypes.IBuildConfigs>(MockBehavior.Strict);

            clientMock.Setup(m => m.BuildConfigs).Returns(mockBuildConfigs.Object);

            TeamCitySharp.DomainEntities.BuildConfig configuration = new BuildConfig();
            configuration.Name = buildName;

            mockBuildConfigs.Setup(m => m.ByConfigurationId(expectedBuildConfigurationId)).Returns(configuration);
        }

        #endregion ExpectReadOfBuildConfiguration

        private void Reading_the_status_of_a_XYZ_build(string status, BuildStatus expectedStatus,
            string expectedBuildConfigurationId = "BuildServer_Branch",
            string returnedName = "Name", string expectedName = "Name")
        {
            // Given
            CreateClient();
            var expectedBuildInfo = new Build() { Status = status };
            var mockBuilds = ExpectReadOfBuildStatus(expectedBuildConfigurationId, expectedBuildInfo);

            ExpectReadOfBuildConfiguration(expectedBuildConfigurationId, returnedName);

            var server = new TeamCityServer("localhost", "Username", "Password");

            var wait = new System.Threading.AutoResetEvent(false);

            BuildStatus? actualStatus = null;
            string actualBuildConfigurationId = null;
            System.Threading.Thread actualThread = null;
            string actualBuildName = null;

            server.ReadBuildStatusComplete += (sender, e) =>
            {
                actualThread = System.Threading.Thread.CurrentThread;
                actualStatus = e.Status;
                actualBuildConfigurationId = e.BuildConfigurationId;
                actualBuildName = e.Name;
                wait.Set();
            };

            // When
            server.ReadBuildStatusAsync(expectedBuildConfigurationId);

            Assert.IsTrue(wait.WaitOne(5000));

            // Then
            clientMock.VerifyAll();
            mockBuilds.VerifyAll();
            Assert.AreEqual(expectedStatus, actualStatus);
            Assert.AreEqual(expectedBuildConfigurationId, actualBuildConfigurationId);
            Assert.AreEqual(expectedName, actualBuildName);
            Assert.AreNotEqual(System.Threading.Thread.CurrentThread, actualThread);
        }

        [TestMethod]
        public void Reading_the_status_of_a_successful_build()
        {
            Reading_the_status_of_a_XYZ_build("SUCCESS", BuildStatus.Success);
        }

        [TestMethod]
        public void A_null_build_name_returns_the_build_configuration_id_instead()
        {
            Reading_the_status_of_a_XYZ_build("SUCCESS", BuildStatus.Success, "BuildID", null, "BuildID");
        }


        [TestMethod]
        public void Reading_the_status_of_a_failed_build()
        {
            Reading_the_status_of_a_XYZ_build("FAILED", BuildStatus.Failed);
        }

        [TestMethod]
        public void An_exception_during_the_reading_the_status()
        {
            // Given
            CreateClient();
            var expectedBuildInfo = new Build() { Status = "FAILED" };
            string expectedBuildConfigurationId = "BuildServer_Branch";
            Exception expectedException = new Exception();
            var mockBuilds = ExpectReadOfBuildStatus(expectedBuildConfigurationId, expectedBuildInfo, expectedException);

            var server = new TeamCityServer("localhost", "Username", "Password");

            var wait = new System.Threading.AutoResetEvent(false);

            Exception actualException = null;
            string actualBuildConfigurationId = null;
            System.Threading.Thread actualThread = null;

            server.ReadBuildStatusError += (sender, e) =>
            {
                actualThread = System.Threading.Thread.CurrentThread;
                actualException = e.ThrownException;
                actualBuildConfigurationId = e.BuildConfigurationId;
                wait.Set();
            };

            // When
            server.ReadBuildStatusAsync(expectedBuildConfigurationId);

            Assert.IsTrue(wait.WaitOne(5000));

            // Then
            clientMock.VerifyAll();
            mockBuilds.VerifyAll();
            Assert.AreEqual(expectedException, actualException);
            Assert.AreEqual(expectedBuildConfigurationId, actualBuildConfigurationId);
            Assert.AreNotEqual(System.Threading.Thread.CurrentThread, actualThread);
        }

        [TestMethod]
        public void Reading_the_status_of_a_non_default_build()
        {
            // Given
            CreateClient();
            var expectedBuildInfo = new Build() { Status = "SUCCESS" };
            string expectedBuildConfigurationId = "BuildServer_Branch";

            var locator = BuildLocator.WithDimensions(
                buildType: BuildTypeLocator.WithId(expectedBuildConfigurationId),
                maxResults: 1,
                branch: "branched:true");

            var mockBuilds = ExpectReadOfBuildStatus(expectedBuildConfigurationId, expectedBuildInfo, locator: locator);

            ExpectReadOfBuildConfiguration(expectedBuildConfigurationId, expectedBuildConfigurationId + "_hello");


            var server = new TeamCityServer("localhost", "Username", "Password");

            var wait = new System.Threading.AutoResetEvent(false);

            BuildStatus? actualStatus = null;
            string actualBuildConfigurationId = null;
            System.Threading.Thread actualThread = null;
            string actualName = null;

            server.ReadBuildStatusComplete += (sender, e) =>
            {
                actualThread = System.Threading.Thread.CurrentThread;
                actualStatus = e.Status;
                actualBuildConfigurationId = e.BuildConfigurationId;
                actualName = e.Name;
                wait.Set();
            };

            server.UseDefault = false; // Set the server to not use the builds default branch

            // When
            server.ReadBuildStatusAsync(expectedBuildConfigurationId);

            Assert.IsTrue(wait.WaitOne(5000));

            // Then
            clientMock.VerifyAll();
            mockBuilds.VerifyAll();
            Assert.AreEqual(BuildStatus.Success, actualStatus);
            Assert.AreEqual(expectedBuildConfigurationId, actualBuildConfigurationId);
            Assert.AreEqual(expectedBuildConfigurationId + "_hello", actualName);
            Assert.AreNotEqual(System.Threading.Thread.CurrentThread, actualThread);
        }

    }
}
