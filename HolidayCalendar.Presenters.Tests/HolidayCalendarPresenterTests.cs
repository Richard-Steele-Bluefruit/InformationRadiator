using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PresenterCommon.Configuration;
using Moq;
using PresenterCommon;

namespace HolidayCalendar.Presenters.Tests
{
    [TestClass]
    public class HolidayCalendarPresenterTests
    {
        private Mock<ITimer> timer;
        private LeaveDetails leave;
        private HolidayCalendarFactoryFake factory;

        [TestInitialize]
        public void Setup()
        {
            timer = new Mock<ITimer>(MockBehavior.Strict);
            leave = new LeaveDetails();
            factory = new HolidayCalendarFactoryFake();
            factory.timer = timer.Object;
            HolidayCalendarFactory.Instance = factory;
        }

        private void AddPersonsLeave(DateTime date, string name)
        {
            leave.LeaveDays.Add(new LeaveDetails.LeaveDay() { Date = date, Name = name });
        }

        private void SaveLeaveToFakeDisk(LeaveDetails leaveDetails = null)
        {
            if (leaveDetails == null)
                leaveDetails = leave;

            factory.leaveDetails = leaveDetails;
        }

        [TestMethod]
        public void The_holiday_information_for_a_single_person_is_loaded_from_disk_and_sent_to_the_view_when_the_day_changes()
        {
            leave.DownloadDateTime = new DateTime(2016, 10, 26);
            AddPersonsLeave(new DateTime(2016, 10, 28), "Byran");
            SaveLeaveToFakeDisk();

            var configuration = new InformationRadiatorItemConfiguration();
            var presenter = new HolidayCalendarPresenter(configuration);


            List<HolidayCalendarDay> days = null;
            DateTime downloadedDate = DateTime.MinValue;
            presenter.HolidayCalendarUpdate += (sender, e) =>
            {
                days = e.Days;
                downloadedDate = e.DownloadedTime;
            };

            timer.Raise(t => t.Tick += null, timer.Object, EventArgs.Empty);

            Assert.IsNotNull(days);
            Assert.AreEqual(1, days.Count);
            Assert.AreEqual(new DateTime(2016, 10, 28), days[0].Date);

            Assert.AreEqual(1, days[0].PeopleOnLeave.Count);
            Assert.AreEqual("Byran", days[0].PeopleOnLeave[0]);

            Assert.AreEqual(new DateTime(2016, 10, 26), downloadedDate);
        }

        [TestMethod]
        public void The_holiday_information_for_a_multiple_people_is_loaded_from_disk_and_sent_to_the_view_when_the_day_changes()
        {
            AddPersonsLeave(new DateTime(2016, 10, 28), "Paul");
            AddPersonsLeave(new DateTime(2016, 10, 28), "Steve");
            SaveLeaveToFakeDisk();

            var configuration = new InformationRadiatorItemConfiguration();
            var presenter = new HolidayCalendarPresenter(configuration);


            List<HolidayCalendarDay> days = null;
            presenter.HolidayCalendarUpdate += (sender, e) =>
            {
                days = e.Days;
            };

            timer.Raise(t => t.Tick += null, timer.Object, EventArgs.Empty);

            Assert.IsNotNull(days);
            Assert.AreEqual(1, days.Count);
            Assert.AreEqual(new DateTime(2016, 10, 28), days[0].Date);

            Assert.AreEqual(2, days[0].PeopleOnLeave.Count);
            Assert.IsTrue(days[0].PeopleOnLeave.Contains("Paul"));
            Assert.IsTrue(days[0].PeopleOnLeave.Contains("Steve"));
        }

        [TestMethod]
        public void Only_holiday_information_for_the_next_two_weeks_is_sent_to_the_view()
        {
            AddPersonsLeave(factory.Date().AddDays(-1), "Paul");
            AddPersonsLeave(factory.Date(), "Steve");
            AddPersonsLeave(factory.Date().AddDays(13), "Jenny");
            AddPersonsLeave(factory.Date().AddDays(14), "Byran");
            SaveLeaveToFakeDisk();

            var configuration = new InformationRadiatorItemConfiguration();
            var presenter = new HolidayCalendarPresenter(configuration);


            List<HolidayCalendarDay> days = null;
            presenter.HolidayCalendarUpdate += (sender, e) =>
            {
                days = e.Days;
            };

            timer.Raise(t => t.Tick += null, timer.Object, EventArgs.Empty);

            Assert.IsNotNull(days);
            Assert.AreEqual(2, days.Count);

            Assert.AreEqual(factory.Date(), days[0].Date);
            Assert.AreEqual(1, days[0].PeopleOnLeave.Count);
            Assert.IsTrue(days[0].PeopleOnLeave.Contains("Steve"));

            Assert.AreEqual(factory.Date().AddDays(13), days[1].Date);
            Assert.AreEqual(1, days[1].PeopleOnLeave.Count);
            Assert.IsTrue(days[1].PeopleOnLeave.Contains("Jenny"));
        }

        public class ErroringHolidayCalendarFactory : HolidayCalendarFactory
        {
            public override DateTime Date()
            {
                throw new Exception();
            }

            public override LeaveDetails LoadLeaveDetailsFromDisk()
            {
                throw new Exception();
            }
        }

        [TestMethod]
        public void The_view_is_informed_about_an_error_during_processing_the_holidays()
        {
            var configuration = new InformationRadiatorItemConfiguration();
            var presenter = new HolidayCalendarPresenter(configuration);

            HolidayCalendarFactory.Instance = new ErroringHolidayCalendarFactory();

            bool errorEventRaised = false;
            presenter.ErrorLoadingHolidayInformation += (sender, e) =>
            {
                errorEventRaised = true;
            };

            timer.Raise(t => t.Tick += null, timer.Object, EventArgs.Empty);

            Assert.IsTrue(errorEventRaised);
        }

        [TestMethod]
        public void The_update_timer_is_requested_at_a_5_minute_interval()
        {
            var configuration = new InformationRadiatorItemConfiguration();
            var presenter = new HolidayCalendarPresenter(configuration);


            // The interval is in milliseconds
            Assert.AreEqual(300000, factory.timerInterval);
        }
    }
}
