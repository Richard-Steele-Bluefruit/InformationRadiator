using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using LeanKit.Model.Ticker;

using LeanKit.Presenters.Ticker;
using PresenterCommon.Configuration;

namespace LeanKit.Presenters.Tests.Ticker
{
    [TestClass]
    public class LeanKitTickerPresenterTests
    {
        private LeanKitFactoryMock mockFactory;
        private Mock<ILeanKitTicker> mockTicker;
        private Mock<PresenterCommon.ITimer> mockTimer;
        private string actualMessage;
        private System.Threading.AutoResetEvent wait;

        [TestInitialize]
        public void Setup()
        {
            mockFactory = new LeanKitFactoryMock();
            LeanKitFactory.Instance = mockFactory;

            mockTicker = new Mock<ILeanKitTicker>(MockBehavior.Strict);
            mockFactory._ticker = mockTicker.Object;

            mockTimer = new Mock<PresenterCommon.ITimer>(MockBehavior.Strict);
            mockFactory._timer = mockTimer.Object;

            actualMessage = null;
            wait = new System.Threading.AutoResetEvent(false);
        }

        [TestCleanup]
        public void CleanUp()
        {
            LeanKitFactory.Instance = null;

            mockTicker.VerifyAll();
        }
        
        [TestMethod]
        public void Specifying_the_server_configuration()
        {
            // Given
            string expectedHostName = "absw";
            string expectedUserName = "informationRadiator";
            string expectedPassword = "passWord";
            long expectedBoardId = 152;
            long expectedLaneId = 12;

            var configuration = new InformationRadiatorItemConfiguration();
            configuration.Add(new InformationRadiatorItemConfigurationField { ID = "HostName", Value = expectedHostName });
            configuration.Add(new InformationRadiatorItemConfigurationField { ID = "UserName", Value = expectedUserName });
            configuration.Add(new InformationRadiatorItemConfigurationField { ID = "Password", Value = expectedPassword });
            configuration.Add(new InformationRadiatorItemConfigurationField { ID = "BoardId", Value = expectedBoardId.ToString() });
            configuration.Add(new InformationRadiatorItemConfigurationField { ID = "LaneId", Value = expectedLaneId.ToString() });

            // When
            var target = new LeanKitTickerPresenter(configuration);

            // Then
            Assert.AreEqual(expectedHostName, mockFactory._tickerHostName);
            Assert.AreEqual(expectedUserName, mockFactory._tickerUserName);
            Assert.AreEqual(expectedPassword, mockFactory._tickerPassword);
            Assert.AreEqual(expectedBoardId, mockFactory._tickerBoardId);
            Assert.AreEqual(expectedLaneId, mockFactory._tickerLaneId);
            Assert.AreEqual(1000, mockFactory._interval);
        }

        #region Support Functions

        private InformationRadiatorItemConfiguration AddDefaultConfiguration(int displayUpdatedInterval = 10, int fetchUpdateInterval = 360)
        {
            string expectedHostName = "absw";
            string expectedUserName = "informationRadiator";
            string expectedPassword = "passWord";
            long expectedBoardId = 152;
            long expectedLaneId = 12;

            var configuration = new InformationRadiatorItemConfiguration();
            configuration.Add(new InformationRadiatorItemConfigurationField { ID = "HostName", Value = expectedHostName });
            configuration.Add(new InformationRadiatorItemConfigurationField { ID = "UserName", Value = expectedUserName });
            configuration.Add(new InformationRadiatorItemConfigurationField { ID = "Password", Value = expectedPassword });
            configuration.Add(new InformationRadiatorItemConfigurationField { ID = "BoardId", Value = expectedBoardId.ToString() });
            configuration.Add(new InformationRadiatorItemConfigurationField { ID = "LaneId", Value = expectedLaneId.ToString() });
            configuration.Add(new InformationRadiatorItemConfigurationField { ID = "DisplayUpdateInterval", Value = displayUpdatedInterval.ToString() });
            configuration.Add(new InformationRadiatorItemConfigurationField { ID = "FetchUpdateInterval", Value = fetchUpdateInterval.ToString() });
            return configuration;
        }

        private LeanKitTickerPresenter TargetWithDefaultConfiguration(int displayUpdatedInterval = 10, int fetchUpdateInterval = 360)
        {
            var configuration = AddDefaultConfiguration(displayUpdatedInterval: displayUpdatedInterval, fetchUpdateInterval: fetchUpdateInterval);
            return new LeanKitTickerPresenter(configuration);
        }

        private void MessagesFromLeanKit(params string[] messages)
        {
            var expectedMessages = new List<LeanKitTickerMessage>();
            var messageList = new List<LeanKitTickerMessage>();
            foreach (var m in messages)
                messageList.Add(new LeanKitTickerMessage { Message = m });
            expectedMessages.AddRange(messageList);
            mockTicker.Setup(m => m.GetMessages()).Returns(expectedMessages);            
        }

        private void MessagesFromLeanKit(params LeanKitTickerMessage[] messages)
        {
            var expectedMessages = new List<LeanKitTickerMessage>();
            expectedMessages.AddRange(messages);
            mockTicker.Setup(m => m.GetMessages()).Returns(expectedMessages);
        }

        private void AttachTickerUpdate(LeanKitTickerPresenter target)
        {
            target.TickerUpdate += (o, e) =>
            {
                actualMessage = e.Message;
                wait.Set();
            };
        }

        private void WaitForTickerUpdate()
        {
            Assert.IsTrue(wait.WaitOne(5000), "TickerUpdate event did not fire");
        }

        private void ResetTickerUpdate()
        {
            wait.Reset();
        }

        #endregion Support Functions

        [TestMethod]
        public void Forcing_a_read_of_the_messages()
        {
            // Given
            var target = TargetWithDefaultConfiguration();
            MessagesFromLeanKit("Hello", "World");
            
            // When
            target.TickerUpdate += (o, e) =>
                {
                    actualMessage = e.Message;
                    wait.Set();
                };

            target.Update();
            Assert.IsTrue(wait.WaitOne(5000), "TickerUpdate event did not fire");

            // Then
            Assert.AreEqual("Hello", actualMessage);
        }

        [TestMethod]
        public void Forcing_a_read_of_the_messages_when_there_are_no_messages()
        {
            // Given
            var target = TargetWithDefaultConfiguration();
            MessagesFromLeanKit(new string[0]);

            AttachTickerUpdate(target);

            // When
            target.Update();
            WaitForTickerUpdate();

            // Then
            Assert.AreEqual("No messages", actualMessage);
        }

        [TestMethod]
        public void The_message_changes_after_a_specified_time()
        {
            // Given
            var target = TargetWithDefaultConfiguration(displayUpdatedInterval: 5);
            MessagesFromLeanKit("Hello", "World");

            AttachTickerUpdate(target);

            // Force an update, the case where there are
            // no message will be covered in another test
            target.Update();
            WaitForTickerUpdate();

            // When
            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual("Hello", actualMessage);
                mockTimer.Raise(m => m.Tick += null, EventArgs.Empty);
            }

            // Then
            Assert.AreEqual("World", actualMessage);
        }

        [TestMethod]
        public void The_Message_change_is_blank_if_no_messages_have_been_downloaded()
        {
            // Given
            var target = TargetWithDefaultConfiguration(displayUpdatedInterval: 5);

            string actualMessage = null;
            target.TickerUpdate += (o, e) =>
            {
                actualMessage = e.Message;
            };

            // When
            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(null, actualMessage);
                mockTimer.Raise(m => m.Tick += null, EventArgs.Empty);
            }

            // Then
            Assert.AreEqual("Error: Messages have not been downloaded", actualMessage);
        }


        [TestMethod]
        public void Message_change_after_a_specified_time_is_blank_if_there_are_no_messages()
        {
            // Given
            var target = TargetWithDefaultConfiguration(displayUpdatedInterval: 10);
            MessagesFromLeanKit(new string[0]);

            AttachTickerUpdate(target);

            // Force an update
            target.Update();
            WaitForTickerUpdate();

            // When
            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual("No messages", actualMessage);
                mockTimer.Raise(m => m.Tick += null, EventArgs.Empty);
            }

            // Then
            Assert.AreEqual("No messages", actualMessage);
        }

        [TestMethod]
        public void Fetching_new_messages_from_the_server()
        {
            // Given
            var target = TargetWithDefaultConfiguration(displayUpdatedInterval: 10, fetchUpdateInterval: 4);
            MessagesFromLeanKit("Hello", "World");

            AttachTickerUpdate(target);

            // Force an update
            target.Update();
            WaitForTickerUpdate();

            var expectedMessages2 = new List<LeanKitTickerMessage>
            {
                new LeanKitTickerMessage { Message = "Mars1" },
                new LeanKitTickerMessage { Message = "Mars2" },
                new LeanKitTickerMessage { Message = "Mars3" }
            };

            mockTicker.Setup(m => m.GetMessages())
                .Returns(expectedMessages2);

            // When
            ResetTickerUpdate();
            for (int i = 0; i < 4; i++)
            {
                mockTimer.Raise(m => m.Tick += null, EventArgs.Empty);
            }
            WaitForTickerUpdate();

            // Then
            Assert.AreEqual("Mars", actualMessage.Substring(0, 4));
        }

        [TestMethod]
        public void Fetching_new_messages_and_an_error_occurs()
        {
            // Given
            var target = TargetWithDefaultConfiguration(displayUpdatedInterval: 2, fetchUpdateInterval: 4);
            MessagesFromLeanKit("Hello", "World");

            AttachTickerUpdate(target);

            // Force an update
            target.Update();
            WaitForTickerUpdate();

            var updateWait = new System.Threading.AutoResetEvent(false);

            mockTicker.Setup(m => m.GetMessages())
                .Returns<List<string>>(null)
                .Callback(() => updateWait.Set());

            // When
            for (int i = 0; i < 4; i++)
            {
                mockTimer.Raise(m => m.Tick += null, EventArgs.Empty);
            }
            updateWait.WaitOne(5000);

            // Then
            Assert.AreEqual("Error: Messages have not been downloaded", actualMessage);
        }

        [TestMethod]
        public void The_messages_loop()
        {
            // Given
            var target = TargetWithDefaultConfiguration(displayUpdatedInterval: 2);
            MessagesFromLeanKit("Hello", "World");

            AttachTickerUpdate(target);

            // Force an update, the case where there are
            // no message will be covered in another test
            target.Update();
            WaitForTickerUpdate();

            // When
            for (int i = 0; i < 4; i++)
            {
                mockTimer.Raise(m => m.Tick += null, EventArgs.Empty);
            }

            // Then
            Assert.AreEqual("Hello", actualMessage);
        }

        [TestMethod]
        public void Messages_with_formatted_Dates()
        {
            // Given
            var target = TargetWithDefaultConfiguration();
            MessagesFromLeanKit(
                new LeanKitTickerMessage { Message = "Hello <days_until> World", DueDate = DateTime.Now.AddDays(1).AddMinutes(-1) }
                );

            // When
            target.TickerUpdate += (o, e) =>
            {
                actualMessage = e.Message;
                wait.Set();
            };

            target.Update();
            Assert.IsTrue(wait.WaitOne(5000), "TickerUpdate event did not fire");

            // Then
            Assert.AreEqual("Hello 1 World", actualMessage);
        }

        [TestMethod]
        public void Messages_with_due_dates_in_the_past_are_not_displayed()
        {
            // Given
            var target = TargetWithDefaultConfiguration();
            MessagesFromLeanKit(
                new LeanKitTickerMessage { Message = "Hello <days_until> World", DueDate = DateTime.Now.AddDays(-1) },
                new LeanKitTickerMessage { Message = "Boo", DueDate = DateTime.Now.AddMinutes(-1) }
                );

            // When
            target.TickerUpdate += (o, e) =>
            {
                actualMessage = e.Message;
                wait.Set();
            };

            target.Update();
            Assert.IsTrue(wait.WaitOne(5000), "TickerUpdate event did not fire");

            // Then
            Assert.AreEqual("Boo", actualMessage);
        }

        [TestMethod]
        public void Messages_with_formatted_days_since_start_date()
        {
            // Given
            var target = TargetWithDefaultConfiguration();
            MessagesFromLeanKit(
                new LeanKitTickerMessage { Message = "Day <day_number> of sprint", StartDate = DateTime.Now.AddDays(-2) }
                );

            // When
            target.TickerUpdate += (o, e) =>
            {
                actualMessage = e.Message;
                wait.Set();
            };

            target.Update();
            Assert.IsTrue(wait.WaitOne(5000), "TickerUpdate event did not fire");

            // Then
            Assert.AreEqual("Day 3 of sprint", actualMessage);
        }

        [TestMethod]
        public void Messages_with_start_dates_in_the_future_are_not_displayed()
        {
            // Given
            var target = TargetWithDefaultConfiguration();
            MessagesFromLeanKit(
                new LeanKitTickerMessage { Message = "Day <day_number> of sprint", StartDate = DateTime.Now.AddDays(2) },
                new LeanKitTickerMessage { Message = "Boo", StartDate = DateTime.Now }
                );

            // When
            target.TickerUpdate += (o, e) =>
            {
                actualMessage = e.Message;
                wait.Set();
            };

            target.Update();
            Assert.IsTrue(wait.WaitOne(5000), "TickerUpdate event did not fire");

            // Then
            Assert.AreEqual("Boo", actualMessage);
        }
    }
}
