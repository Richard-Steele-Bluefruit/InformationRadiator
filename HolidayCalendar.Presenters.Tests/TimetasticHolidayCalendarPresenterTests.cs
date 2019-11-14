using System;
using System.Collections.Generic;
using System.Threading;
using HolidayCalendar.Presenters.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PresenterCommon;
using PresenterCommon.Configuration;

namespace HolidayCalendar.Presenters.Tests
{
    [TestClass]
    public class TimetasticHolidayCalendarPresenterTests
    {
        private HolidayCalendarFactoryFake factory;
        private Mock<Model.ITimetasticHolidayDownloader> timetastic;
        private Mock<ITimer> timer;
        private TimetasticHolidayCalendarPresenter presenter;

        private List<HolidayCalendarDay> HolidayCalendarUpdate_Days;
        private ManualResetEvent HolidayCalendarUpdate_Event;

        [TestInitialize]
        public void Setup()
        {
            factory = new HolidayCalendarFactoryFake {date = new DateTime(2017, 03, 23)};


            timer = new Mock<ITimer>(MockBehavior.Strict);
            factory.timer = timer.Object;

            timetastic = new Mock<Model.ITimetasticHolidayDownloader>(MockBehavior.Strict);
            factory.timetasticHolidayDownloader = timetastic.Object;
            
            HolidayCalendarFactory.Instance = factory;

            HolidayCalendarUpdate_Days = null;
            HolidayCalendarUpdate_Event = new ManualResetEvent(false);

            presenter = new TimetasticHolidayCalendarPresenter(new InformationRadiatorItemConfiguration());
            presenter.HolidayCalendarUpdate += (sender, args) =>
            {
                HolidayCalendarUpdate_Days = args.Days;
                HolidayCalendarUpdate_Event.Set();
            };
        }

        [TestMethod]
        public void No_attempts_to_download_holidays_are_made_if_there_are_no_users()
        {
            timetastic.Setup(m => m.DownloadUsers()).Returns(new List<TimetasticUser>());

            presenter.UpdateHolidayCalendar();

            Assert.IsTrue(HolidayCalendarUpdate_Event.WaitOne(1000), "HolidayCalendarUpdate event fired");
        }

        [TestMethod]
        public void The_downloaded_days_represent_the_next_14_days()
        {

            timetastic.Setup(m => m.DownloadUsers()).Returns(new List<TimetasticUser>());

            presenter.UpdateHolidayCalendar();

            Assert.IsTrue(HolidayCalendarUpdate_Event.WaitOne(1000), "HolidayCalendarUpdate event fired");

            var expectedDate = factory.date;
            Assert.AreEqual(expectedDate.AddDays(0), HolidayCalendarUpdate_Days[0].Date);
            Assert.AreEqual(expectedDate.AddDays(1), HolidayCalendarUpdate_Days[1].Date);
            Assert.AreEqual(expectedDate.AddDays(2), HolidayCalendarUpdate_Days[2].Date);
            Assert.AreEqual(expectedDate.AddDays(3), HolidayCalendarUpdate_Days[3].Date);
            Assert.AreEqual(expectedDate.AddDays(4), HolidayCalendarUpdate_Days[4].Date);
            Assert.AreEqual(expectedDate.AddDays(5), HolidayCalendarUpdate_Days[5].Date);
            Assert.AreEqual(expectedDate.AddDays(6), HolidayCalendarUpdate_Days[6].Date);
            Assert.AreEqual(expectedDate.AddDays(7), HolidayCalendarUpdate_Days[7].Date);
            Assert.AreEqual(expectedDate.AddDays(8), HolidayCalendarUpdate_Days[8].Date);
            Assert.AreEqual(expectedDate.AddDays(9), HolidayCalendarUpdate_Days[9].Date);
            Assert.AreEqual(expectedDate.AddDays(10), HolidayCalendarUpdate_Days[10].Date);
            Assert.AreEqual(expectedDate.AddDays(11), HolidayCalendarUpdate_Days[11].Date);
            Assert.AreEqual(expectedDate.AddDays(12), HolidayCalendarUpdate_Days[12].Date);
            Assert.AreEqual(expectedDate.AddDays(13), HolidayCalendarUpdate_Days[13].Date);
        }

        [TestMethod]
        public void A_single_users_one_day_long_holiday_entry_appears_on_the_correct_day()
        {
            var users = new List<TimetasticUser>();
            var byransHoliday = new List<TimetasticHolidayEntry>();

            users.Add(new TimetasticUser() { firstname = "Byran", surname = "Wills-Heath", id="265" });
            byransHoliday.Add(new TimetasticHolidayEntry() { startDate = factory.date, endDate = factory.date });

            timetastic.Setup(m => m.DownloadUsers()).Returns(users);
            timetastic.Setup(m => m.DownloadAUsersHoliday("265", factory.date)).Returns(byransHoliday);

            presenter.UpdateHolidayCalendar();

            Assert.IsTrue(HolidayCalendarUpdate_Event.WaitOne(1000), "HolidayCalendarUpdate event fired");

            Assert.AreEqual(1, HolidayCalendarUpdate_Days[0].PeopleOnLeave.Count);
            Assert.AreEqual("Byran Wills-Heath", HolidayCalendarUpdate_Days[0].PeopleOnLeave[0]);
        }

        [TestMethod]
        public void A_single_users_multiple_day_holiday_entry_appears_on_several_day()
        {
            var users = new List<TimetasticUser>();
            var byransHoliday = new List<TimetasticHolidayEntry>();

            users.Add(new TimetasticUser() { firstname = "Byran", surname = "Wills-Heath", id = "265" });
            byransHoliday.Add(new TimetasticHolidayEntry() { startDate = factory.date.AddDays(3), endDate = factory.date.AddDays(5) });

            timetastic.Setup(m => m.DownloadUsers()).Returns(users);
            timetastic.Setup(m => m.DownloadAUsersHoliday("265", factory.date)).Returns(byransHoliday);

            presenter.UpdateHolidayCalendar();

            Assert.IsTrue(HolidayCalendarUpdate_Event.WaitOne(1000), "HolidayCalendarUpdate event fired");

            Assert.AreEqual(1, HolidayCalendarUpdate_Days[3].PeopleOnLeave.Count);
            Assert.AreEqual("Byran Wills-Heath", HolidayCalendarUpdate_Days[3].PeopleOnLeave[0]);
            Assert.AreEqual(1, HolidayCalendarUpdate_Days[4].PeopleOnLeave.Count);
            Assert.AreEqual("Byran Wills-Heath", HolidayCalendarUpdate_Days[4].PeopleOnLeave[0]);
            Assert.AreEqual(1, HolidayCalendarUpdate_Days[5].PeopleOnLeave.Count);
            Assert.AreEqual("Byran Wills-Heath", HolidayCalendarUpdate_Days[5].PeopleOnLeave[0]);
        }

        [TestMethod]
        public void Multiple_users_multiple_day_holidays_appears_on_relevant_day()
        {
            var users = new List<TimetasticUser>();
            var byransHoliday = new List<TimetasticHolidayEntry>();
            var paulsHoliday = new List<TimetasticHolidayEntry>();

            users.Add(new TimetasticUser() { firstname = "Byran", surname = "Wills-Heath", id = "265" });
            users.Add(new TimetasticUser() { firstname = "Paul", surname = "Massey", id = "2" });
            timetastic.Setup(m => m.DownloadUsers()).Returns(users);

            byransHoliday.Add(new TimetasticHolidayEntry() { startDate = factory.date.AddDays(3), endDate = factory.date.AddDays(4) });
            timetastic.Setup(m => m.DownloadAUsersHoliday("265", factory.date)).Returns(byransHoliday);

            paulsHoliday.Add(new TimetasticHolidayEntry() { startDate = factory.date.AddDays(2), endDate = factory.date.AddDays(3) });
            timetastic.Setup(m => m.DownloadAUsersHoliday("2", factory.date)).Returns(paulsHoliday);

            presenter.UpdateHolidayCalendar();

            Assert.IsTrue(HolidayCalendarUpdate_Event.WaitOne(1000), "HolidayCalendarUpdate event fired");

            Assert.AreEqual(1, HolidayCalendarUpdate_Days[2].PeopleOnLeave.Count);
            Assert.IsTrue(HolidayCalendarUpdate_Days[2].PeopleOnLeave.Contains("Paul Massey"));

            Assert.AreEqual(2, HolidayCalendarUpdate_Days[3].PeopleOnLeave.Count);
            Assert.IsTrue(HolidayCalendarUpdate_Days[3].PeopleOnLeave.Contains("Byran Wills-Heath"));
            Assert.IsTrue(HolidayCalendarUpdate_Days[3].PeopleOnLeave.Contains("Paul Massey"));

            Assert.AreEqual(1, HolidayCalendarUpdate_Days[4].PeopleOnLeave.Count);
            Assert.IsTrue(HolidayCalendarUpdate_Days[4].PeopleOnLeave.Contains("Byran Wills-Heath"));
        }
    }
}
