using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using General.Presenters;
using PresenterCommon.Configuration;

namespace General.Presenters.Tests
{
    [TestClass]
    public class SprintDaysPresenterTests
    {
        [TestMethod]
        public void Reads_the_days_left_in_the_sprint()
        {
            var mockDayUpdateMonitor = new DayUpdateMonitorMock();
            var mockSprintDays = new SprintDaysMock();
            mockSprintDays._sprintDay = 5;
            
            var target = new SprintDaysPresenter(mockSprintDays, mockDayUpdateMonitor, new InformationRadiatorItemConfiguration());

            DateTime expectedCurrentDate = DateTime.Now.Date;
            Assert.AreEqual("6", target.SprintDayText);

            // This is checked because this test will fail incorrectly if
            // the day changes during the test
            if (DateTime.Now.Date != expectedCurrentDate)
                Assert.Inconclusive("Day changed during test");
            
            Assert.AreEqual(expectedCurrentDate, mockSprintDays._currentDate, "Current date mismatch");

            var expectedStartDate = new DateTime(2014, 8, 19);
            Assert.AreEqual(expectedStartDate, mockSprintDays._startDate);
            Assert.IsTrue(mockSprintDays._sprintDayRead);

            // Check a different day
            mockSprintDays._sprintDay = 7;
            Assert.AreEqual("4", target.SprintDayText);
        }

        [TestMethod]
        public void Reading_the_days_left_in_the_sprint_with_a_sprint_length_that_is_not_10_days()
        {
            // Given
            var mockDayUpdateMonitor = new DayUpdateMonitorMock();
            var mockSprintDays = new SprintDaysMock();
            var configuration = new InformationRadiatorItemConfiguration();

            mockSprintDays.DaysInSprint = 10;
            configuration.Add(new InformationRadiatorItemConfigurationField() { ID = "DaysInSprint", Value = "20" });

            var target = new SprintDaysPresenter(mockSprintDays, mockDayUpdateMonitor, configuration);

            mockSprintDays._sprintDay = 3;

            // When
            var actual = target.SprintDayText;

            // Then
            Assert.AreEqual(20, mockSprintDays.DaysInSprint);
            Assert.AreEqual("18", actual);
        }

        [TestMethod]
        public void The_view_is_informed_of_the_sprint_day_changing_when_the_day_changes()
        {
            var mockSprintDays = new SprintDaysMock();
            var mockDayUpdateMonitor = new DayUpdateMonitorMock();
            var target = new SprintDaysPresenter(mockSprintDays, mockDayUpdateMonitor, new InformationRadiatorItemConfiguration());

            bool eventRaised = false;
            target.SprintDayUpdated += (sender, e) => { eventRaised = true; };

            mockDayUpdateMonitor.OnDayChanged();

            Assert.IsTrue(eventRaised);
        }

        [TestMethod]
        public void Setting_a_sprint_start_date_from_the_configuration()
        {
            // Given
            var mockDayUpdateMonitor = new DayUpdateMonitorMock();
            var mockSprintDays = new SprintDaysMock();
            var configuration = new InformationRadiatorItemConfiguration();

            configuration.Add(new InformationRadiatorItemConfigurationField() { ID = "StartDate", Value = "22/10/2014" });

            var target = new SprintDaysPresenter(mockSprintDays, mockDayUpdateMonitor, configuration);

            // When
            var notUsed = target.SprintDayText;

            // Then
            var expectedDate = new DateTime(2014, 10, 22);
            Assert.AreEqual(expectedDate, mockSprintDays._startDate);
        }

        [TestMethod]
        public void Setting_the_Days_to_be_displayed_inverted_from_the_configuration()
        {
            // Given
            var mockDayUpdateMonitor = new DayUpdateMonitorMock();
            var mockSprintDays = new SprintDaysMock();
            var configuration = new InformationRadiatorItemConfiguration();

            configuration.Add(new InformationRadiatorItemConfigurationField() { ID = "DayOfSprint", Value = "true" });

            var target = new SprintDaysPresenter(mockSprintDays, mockDayUpdateMonitor, configuration);

            mockSprintDays._sprintDay = 3;

            // When
            var actual = target.SprintDayText;

            // Then
            Assert.AreEqual("3", actual);
        }

    }
}
