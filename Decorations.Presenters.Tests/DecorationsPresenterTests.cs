using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

using PresenterCommon;
using PresenterCommon.Configuration;
using PresenterCommon.TestDoubles;
using Moq;

namespace Decorations.Presenters.Tests
{
    [TestClass]
    public class DecorationsPresenterTests
    {
        private Mock<IDayUpdateMonitor> dayUpdateMock;
        private InformationRadiatorItemConfiguration configuration;

        private Mock<IDotNetWrapper> mockDotNet;

        private ITimerFake fakeTimer;
        private PresenterCommonFactorySpy factorySpy;

        [TestInitialize]
        public void Setup()
        {
            dayUpdateMock = new Mock<IDayUpdateMonitor>(MockBehavior.Strict);
            configuration = new InformationRadiatorItemConfiguration();

            mockDotNet = new Mock<IDotNetWrapper>();

            DotNet.Instance = mockDotNet.Object;

            factorySpy = new PresenterCommonFactorySpy();
            fakeTimer = new ITimerFake();
            factorySpy.createTimerReturn = fakeTimer;
            factorySpy.InstallSpy();
        }

        [TestCleanup]
        public void TearDown()
        {
            DotNet.Instance = null;
            PresenterCommonFactorySpy.RemoveSpy();
        }

        [TestMethod]
        public void The_view_is_updated_on_a_day_change()
        {
            // Given
            var presenter = new DecorationsPresenter(dayUpdateMock.Object, configuration);

            DecorationsPresenter eventPresenter = null;
            presenter.Update += (sender, e) =>
                {
                    eventPresenter = sender as DecorationsPresenter;
                };

            // When
            dayUpdateMock.Raise(m => m.DayChanged += null, dayUpdateMock.Object, EventArgs.Empty);

            // Then
            Assert.AreEqual(presenter, eventPresenter, "Update event was not raised");
        }

        [TestMethod]
        public void Christmas_is_only_from_December_3rd_to_December_31()
        {
            // Given
            var presenter = new DecorationsPresenter(dayUpdateMock.Object, configuration);

            DateTime November_30th = new DateTime(2015, 11, 30);
            DateTime December_1st = new DateTime(2015, 12, 1);
            DateTime December_3rd = new DateTime(2014, 12, 3);
            DateTime December_31st = new DateTime(2016, 12, 31);
            DateTime January_1st = new DateTime(2016, 1, 1);

            // When, Then
            mockDotNet.Setup(m => m.Now).Returns(November_30th);
            Assert.IsFalse(presenter.IsChristmas);

            mockDotNet.Setup(m => m.Now).Returns(December_1st);
            Assert.IsFalse(presenter.IsChristmas);

            mockDotNet.Setup(m => m.Now).Returns(December_3rd);
            Assert.IsTrue(presenter.IsChristmas);

            mockDotNet.Setup(m => m.Now).Returns(December_31st);
            Assert.IsTrue(presenter.IsChristmas);

            mockDotNet.Setup(m => m.Now).Returns(January_1st);
            Assert.IsFalse(presenter.IsChristmas);
        }

        [TestMethod]
        public void Christmas_can_be_forced_by_a_configuration_item()
        {
            // Given
            configuration.Add(new InformationRadiatorItemConfigurationField()
            {
                ID = "ForceChristmas",
                Value = "true"
            });
            var presenter = new DecorationsPresenter(dayUpdateMock.Object, configuration);

            DateTime November_30th = new DateTime(2015, 11, 30);
            mockDotNet.Setup(m => m.Now).Returns(November_30th);

            // When, Then
            Assert.IsTrue(presenter.IsChristmas);
        }

        private DecorationsPresenter PresenterWithForcedChristmas()
        {
            configuration.Add(new InformationRadiatorItemConfigurationField()
            {
                ID = "ForceChristmas",
                Value = "true"
            });
            return new DecorationsPresenter(dayUpdateMock.Object, configuration);
        }

        [TestMethod]
        public void A_Steve_Quote_Angel_is_shown_after_an_initial_delay_and_then_hidden_after_another_delay()
        {
            // Given
            var presenter = PresenterWithForcedChristmas();

            bool show = false;
            string quote = null;
            presenter.SteveQuoteUpdate += (sender, e) => {
                show = e.ShowSteveAngel;
                quote = e.Quote;
            };

            // When
            fakeTimer.OnTick(times: 14);

            // Then
            Assert.AreEqual(20000, factorySpy.createTimerInterval);
            Assert.IsTrue(show, "SteveQuoteUpdate event did not indicate that the Steve Quote Angel should be shown");
            Assert.IsFalse(string.IsNullOrEmpty(quote), "No Steve quote passed");

            // When
            fakeTimer.OnTick(times: 1);

            // Then
            Assert.IsFalse(show, "SteveQuoteUpdate event did not indicate that the Steve Quote Angel should be hidden");
        }


        [TestMethod]
        public void A_Steve_Quote_Angel_is_shown_if_it_is_Christmas()
        {
            // Given
            DateTime December_3rd = new DateTime(2014, 12, 3);
            mockDotNet.Setup(m => m.Now).Returns(December_3rd);

            var presenter = new DecorationsPresenter(dayUpdateMock.Object, configuration);

            bool show = false;
            presenter.SteveQuoteUpdate += (sender, e) =>
            {
                show = e.ShowSteveAngel;
            };

            // When
            fakeTimer.OnTick(times: 14);

            // Then
            Assert.IsTrue(show, "SteveQuoteUpdate event did not indicate that the Steve Quote Angel should be shown during Christmas");
        }

        [TestMethod]
        public void A_Steve_Quote_Angel_is_not_shown_if_it_is_not_Christmas()
        {
            // Given
            DateTime November_30th = new DateTime(2015, 11, 30);
            mockDotNet.Setup(m => m.Now).Returns(November_30th);

            var presenter = new DecorationsPresenter(dayUpdateMock.Object, configuration);

            bool show = true;
            presenter.SteveQuoteUpdate += (sender, e) =>
            {
                show = e.ShowSteveAngel;
            };

            // When
            fakeTimer.OnTick(times: 14);

            // Then
            Assert.IsFalse(show, "SteveQuoteUpdate event indicated that the Steve Quote Angel should be shown when it isn't Christmas");
        }


        [TestMethod]
        public void Steve_Quotes_change_every_time_the_Steve_Quote_Angel_is_shown_and_loop_at_the_end_of_the_list_of_quotes()
        {
            // Given
            var presenter = PresenterWithForcedChristmas();

            bool show = false;
            string quote = null;
            presenter.SteveQuoteUpdate += (sender, e) =>
            {
                show = e.ShowSteveAngel;
                quote = e.Quote;
            };

            var quotesShown = new List<string>();

            for (int i = 0; i < presenter.SteveQuotes.Length; i++)
            {
                string run = " (Run: " + i.ToString() + ")";
                // When
                fakeTimer.OnTick(times: 14);

                // Then
                Assert.IsFalse(quotesShown.Exists(s => s == quote), "Steve Quote did not change" + run);
                Assert.IsTrue(show, "SteveQuoteUpdate event did not indicate that the Steve Quote Angel should be shown" + run);
                quotesShown.Add(quote);

                fakeTimer.OnTick(times: 1);
                Assert.IsFalse(show, "SteveQuoteUpdate event did not indicate that the Steve Quote Angel should be hidden" + run);
            }

            // When
            fakeTimer.OnTick(times: 14);

            // Then
            Assert.IsTrue(quotesShown.Exists(s => s == quote), "Steve Quote did not change");
        }

        [TestMethod]
        public void Easter_is_only_from_March_20th_to_April_8th()
        {
            // Given
            var presenter = new DecorationsPresenter(dayUpdateMock.Object, configuration);

            // When, Then
            mockDotNet.Setup(m => m.Now).Returns(new DateTime(2015, 3, 19));
            Assert.IsFalse(presenter.IsEaster);

            mockDotNet.Setup(m => m.Now).Returns(new DateTime(2015, 3, 20));
            Assert.IsTrue(presenter.IsEaster);

            mockDotNet.Setup(m => m.Now).Returns(new DateTime(2015, 4, 8));
            Assert.IsTrue(presenter.IsEaster);

            mockDotNet.Setup(m => m.Now).Returns(new DateTime(2015, 4, 9));
            Assert.IsFalse(presenter.IsEaster);
        }

        [TestMethod]
        public void Easter_can_be_forced_by_a_configuration_item()
        {
            // Given
            configuration.Add(new InformationRadiatorItemConfigurationField()
            {
                ID = "ForceEaster",
                Value = "true"
            });
            var presenter = new DecorationsPresenter(dayUpdateMock.Object, configuration);

            mockDotNet.Setup(m => m.Now).Returns(new DateTime(2015, 4, 9));

            // When, Then
            Assert.IsTrue(presenter.IsEaster);
        }
    }
}
