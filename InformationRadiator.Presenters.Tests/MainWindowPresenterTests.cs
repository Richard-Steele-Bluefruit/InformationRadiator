using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using PresenterCommon;
using PresenterCommon.Configuration;

namespace InformationRadiatorPresentersTests
{
    [TestClass]
    public class MainWindowPresenterTests
    {
        [TestMethod]
        public void Parsing_a_configuration_with_a_single_item()
        {
            // Given
            var mockPresenterFactory = new ItemFactoryMock();
            object expectedPresenterObject = new object();
            mockPresenterFactory.returnObjects.Add(expectedPresenterObject);

            var configuration = new InformationRadiatorConfiguration();
            string expectedItemType = "SprintDays";
            int expectedWidth = 400;
            int expectedHeight = 500;
            string expectedTitle = "Carina";
            configuration.Items.Add(new InformationRadiatorItem()
                {
                    ItemType = expectedItemType,
                    Width = expectedWidth,
                    Height = expectedHeight,
                    Title = expectedTitle,
                    Left = "20",
                    Top = "5",
                    Screen = "2"
                });

            object actualPresenterObject = null;
            string actualItemType = null;
            int actualWidth = 0;
            int actualHeight = 0;
            int? actualLeft = null;
            int? actualTop = null;
            string actualTitle = null;
            int actualScreen = -1;

            var target = new MainWindowPresenter();

            target.CreateView += (sender, e) =>
            {
                actualPresenterObject = e.Presenter;
                actualItemType = e.ItemType;
                actualWidth = e.Width;
                actualHeight = e.Height;
                actualTitle = e.Title;
                actualLeft = e.Left;
                actualTop = e.Top;
                actualScreen = e.Screen;
            };

            // When
            target.ParseConfiguration(configuration, mockPresenterFactory);

            // Then
            Assert.AreEqual(expectedPresenterObject, actualPresenterObject);
            Assert.AreEqual(expectedItemType, actualItemType);
            Assert.AreEqual(expectedWidth, actualWidth);
            Assert.AreEqual(expectedHeight, actualHeight);
            Assert.AreEqual(20, actualLeft);
            Assert.AreEqual(5, actualTop);
            Assert.AreEqual(expectedTitle, actualTitle);
            Assert.AreEqual(2, actualScreen);
        }

        [TestMethod]
        public void The_default_screen_width_height_left_and_top_of_a_configuration_item_are_0_300x300_and_null_null()
        {
            // Given
            var mockPresenterFactory = new ItemFactoryMock();
            object expectedPresenterObject = new object();
            mockPresenterFactory.returnObjects.Add(expectedPresenterObject);

            var configuration = new InformationRadiatorConfiguration();
            string expectedItemType = "SprintDays";
            configuration.Items.Add(new InformationRadiatorItem()
            {
                ItemType = expectedItemType
                // The Width and Height are not set
            });

            int actualWidth = 0;
            int actualHeight = 0;
            int? actualLeft = 1;
            int? actualTop = 1;
            int actualScreen = -1;

            var target = new MainWindowPresenter();

            target.CreateView += (sender, e) =>
            {
                actualWidth = e.Width;
                actualHeight = e.Height;
                actualLeft = e.Left;
                actualTop = e.Top;
                actualScreen = e.Screen;
            };

            // When
            target.ParseConfiguration(configuration, mockPresenterFactory);

            // Then
            Assert.AreEqual(0, actualScreen);
            Assert.AreEqual(300, actualWidth);
            Assert.AreEqual(300, actualHeight);
            Assert.AreEqual(null, actualLeft);
            Assert.AreEqual(null, actualTop);
        }

        [TestMethod]
        public void Parsing_a_configuration_with_a_multiple_item()
        {
            // Given
            var mockPresenterFactory = new ItemFactoryMock();
            object[] expectedPresenterObject = new object[] { new object(),
                new object(), new object() };
            foreach(object o in expectedPresenterObject)
                mockPresenterFactory.returnObjects.Add(o);

            var configuration = new InformationRadiatorConfiguration();
            string[] expectedItemType = new string[] { "SprintDays1",
                "SprintDays2", "SprintDays3" };
            foreach(string s in expectedItemType)
                configuration.Items.Add(new InformationRadiatorItem() { ItemType = s });

            List<object> actualPresenterObjects = new List<object>();
            List<string> actualItemTypes = new List<string>();

            var target = new MainWindowPresenter();

            target.CreateView += (sender, e) =>
            {
                actualPresenterObjects.Add(e.Presenter);
                actualItemTypes.Add(e.ItemType);
            };

            // When
            target.ParseConfiguration(configuration, mockPresenterFactory);

            // Then
            Assert.AreEqual(expectedPresenterObject.Length, actualPresenterObjects.Count);
            for (int i = 0; i < expectedPresenterObject.Length; ++i)
                Assert.AreEqual(expectedPresenterObject[i], actualPresenterObjects[i]);

            Assert.AreEqual(expectedItemType.Length, actualItemTypes.Count);
            for (int i = 0; i < expectedItemType.Length; ++i)
                Assert.AreEqual(expectedItemType[i], actualItemTypes[i]);
        }

        [TestMethod]
        public void The_configuration_is_passed_to_the_factory_when_creating_a_presenter()
        {
            // Given
            var mockPresenterFactory = new ItemFactoryMock();
            object expectedPresenterObject = new object();
            mockPresenterFactory.returnObjects.Add(expectedPresenterObject);

            var configuration = new InformationRadiatorConfiguration();
            string expectedItemType = "SprintDays";
            configuration.Items.Add(new InformationRadiatorItem()
            {
                ItemType = expectedItemType
            });

            configuration.Items[0].Configuration.Add(new InformationRadiatorItemConfigurationField()
            {
                ID = "StartDate",
                Value = "Tomorrow"
            });

            var target = new MainWindowPresenter();

            // When
            target.ParseConfiguration(configuration, mockPresenterFactory);

            // Then
            // Get the parameters passed to the first presenter created as an object[]
            object[] firstParameters = mockPresenterFactory.requestedSpecificParameters[0];
            // Find the parameters that are the configuration
            object configurationParameter = Array.Find(firstParameters, o => o is InformationRadiatorItemConfiguration);
            Assert.AreEqual(configurationParameter, configuration.Items[0].Configuration);
        }

    }
}
