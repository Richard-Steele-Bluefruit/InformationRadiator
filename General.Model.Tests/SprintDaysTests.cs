using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace General.Model.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class SprintDaysTests
    {
        public SprintDaysTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void First_day()
        {
            // Given
            SprintDays target = new SprintDays();

            target.StartDate = new DateTime(2012, 12, 03);
            target.CurrentDate = new DateTime(2012, 12, 03);

            // When, Then
            Assert.AreEqual(1, target.SprintDay);
        }

        [TestMethod]
        public void Second_day()
        {
            // Given
            SprintDays target = new SprintDays();

            target.StartDate = new DateTime(2012, 12, 03);
            target.CurrentDate = new DateTime(2012, 12, 04);

            // When, Then
            Assert.AreEqual(2, target.SprintDay);
        }

        [TestMethod]
        public void Weekend_day()
        {
            // Given
            SprintDays target = new SprintDays();

            target.StartDate = new DateTime(2012, 12, 03);
            target.CurrentDate = new DateTime(2012, 12, 08);

            // When, Then
            Assert.AreEqual(5, target.SprintDay);
        }

        [TestMethod]
        public void First_day_in_following_sprint()
        {
            // Given
            SprintDays target = new SprintDays();

            target.StartDate = new DateTime(2012, 12, 04);
            target.CurrentDate = new DateTime(2012, 12, 18);

            // When, Then
            Assert.AreEqual(1, target.SprintDay);
        }

        [TestMethod]
        public void Second_day_in_following_sprint()
        {
            // Given
            SprintDays target = new SprintDays();

            target.StartDate = new DateTime(2012, 12, 04);
            target.CurrentDate = new DateTime(2012, 12, 19);

            // When, Then
            Assert.AreEqual(2, target.SprintDay);
        }

        [TestMethod]
        public void Weekend_day_in_following_sprint()
        {
            // Given
            SprintDays target = new SprintDays();

            target.StartDate = new DateTime(2012, 12, 04);
            target.CurrentDate = new DateTime(2012, 12, 23);

            // When, Then
            Assert.AreEqual(4, target.SprintDay);
        }
    }
}
