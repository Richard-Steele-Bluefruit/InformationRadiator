using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Media;
using System.Windows;

using Moq;
using PresenterCommon.Configuration;

namespace GitVisualiser.Presenters.Tests
{
    [TestClass]
    public class GitVisualiserPresenterTests
    {
        GitVisualiserFactoryMock _factory;
        Mock<GitVisualiser.Model.IIslands> _mockIslands;
        Mock<PresenterCommon.ITimer> _mockTextualUpdateTimer;
        Mock<PresenterCommon.ITimer> _mockPythonScriptUpdateTimer;
        Mock<GitVisualiser.Model.IPythonScriptRunner> _mockPythonScriptRunner;

        #region Helper Methods

        [TestCleanup]
        public void CleanUp()
        {
            GitVisualiserFactoryMock.Instance = null;
        }

        private void CreateMockFactory(bool useDefaultBranch = true)
        {
            _factory = new GitVisualiserFactoryMock();
            GitVisualiserPresenterFactory.Instance = _factory;

            _mockIslands = new Mock<GitVisualiser.Model.IIslands>(MockBehavior.Strict);
            _factory._islands = _mockIslands.Object;
            
            _mockTextualUpdateTimer = new Mock<PresenterCommon.ITimer>(MockBehavior.Strict);
            _factory._timers.Add(_mockTextualUpdateTimer.Object);
            _mockPythonScriptUpdateTimer = new Mock<PresenterCommon.ITimer>(MockBehavior.Strict);
            _factory._timers.Add(_mockPythonScriptUpdateTimer.Object);

            _mockPythonScriptRunner = new Mock<GitVisualiser.Model.IPythonScriptRunner>(MockBehavior.Strict);
            _factory._pythonScriptRunner = _mockPythonScriptRunner.Object;
        }

        // copied and adapted from presenter::GetLabelWidthAndHeight(...)
        private double TestHelper_GetLabelWidthAndHeight(string text)
        {
            var defaultFontFamily = new System.Windows.Media.FontFamily();
            var defaultFontStyle = new System.Windows.FontStyle();
            var defaultFontStretch = new System.Windows.FontStretch();
            var defaultFontSizeEm = 12.0;

            var typeFace = new Typeface(defaultFontFamily,
                defaultFontStyle,
                System.Windows.FontWeights.Normal,
                defaultFontStretch);

            var formattedText = new FormattedText(
                    text,
                    System.Globalization.CultureInfo.CurrentUICulture,
                    FlowDirection.LeftToRight,
                    typeFace,
                    defaultFontSizeEm,
                    Brushes.Black);

            var Padding = 15;   // empirical
            var width = formattedText.Width + Padding;

            return width;
        }

        #endregion Helper Methods

        [TestMethod]
        public void An_empty_configuration_makes_the_timers_have_default_interval_times()
        {
            // Given
            CreateMockFactory();
            _mockPythonScriptRunner.Setup(m => m.Go());

            var emptyConfiguration = new InformationRadiatorItemConfiguration();
            var presenter = new GitVisualiserPresenter(emptyConfiguration);

            presenter.AddNewIslandToView += (sender, eventArgs) => { };
            presenter.DisplayTextualDataUpdate += (sender, eventArgs) => { };

            // When
            var controlWidth = 100.0;
            var controlHeight = 200.0;
            presenter.FinishInitialisation(controlWidth, controlHeight);

            // Then
            var actualTextualUpdateTimerInterval = _factory._timersToIntervals[_mockTextualUpdateTimer.Object];
            double defaultInterval = 6 * 1000;
            Assert.AreEqual(defaultInterval, actualTextualUpdateTimerInterval);

            var actualPythonScriptUpdateTimerInterval = _factory._timersToIntervals[_mockPythonScriptUpdateTimer.Object];
            defaultInterval = ((0.5 * 60) * 60) * 1000;
            Assert.AreEqual(defaultInterval, actualPythonScriptUpdateTimerInterval);

            _mockPythonScriptRunner.VerifyAll();
        }

        [TestMethod]
        public void Finishing_the_initialisation_adds_the_release_archipelago_to_the_view()
        {
            // Given
            CreateMockFactory();
            _mockPythonScriptRunner.Setup(m => m.Go());

            var dummyConfiguration = new InformationRadiatorItemConfiguration();
            var presenter = new GitVisualiserPresenter(dummyConfiguration);

            var wait = new System.Threading.AutoResetEvent(false);
            string nameOfIslandAddedToView = null;
            presenter.AddNewIslandToView += (sender, eventArgs) => 
            {
                nameOfIslandAddedToView = eventArgs.Name;
                wait.Set();
            };
            presenter.DisplayTextualDataUpdate += (sender, eventArgs) => { };

            // When
            var controlWidth = 100.0;
            var controlHeight = 200.0;
            presenter.FinishInitialisation(controlWidth, controlHeight);

            // Then
            Assert.IsTrue(wait.WaitOne(1000), "did not add island to view within 1 second");

            Assert.AreEqual("Release Archipelago", nameOfIslandAddedToView);

            _mockPythonScriptRunner.VerifyAll();
        }

        [TestMethod]
        public void Finishing_the_initialisation_starts_off_the_python_script()
        {
            // Given
            CreateMockFactory();
            _mockPythonScriptRunner.Setup(m => m.Go());

            var scriptDirectory = "script dir";
            var pythonExeLocation = "exe dir";
            var configurationForPythonScript = new InformationRadiatorItemConfiguration();
            configurationForPythonScript.Add(new InformationRadiatorItemConfigurationField() 
            { 
                ID = "ScriptDirectory", Value = scriptDirectory
            });
            configurationForPythonScript.Add(new InformationRadiatorItemConfigurationField() 
            { 
                ID = "PythonExeLocation", Value = pythonExeLocation 
            });

            var presenter = new GitVisualiserPresenter(configurationForPythonScript);

            presenter.AddNewIslandToView += (sender, eventArgs) => { };
            presenter.DisplayTextualDataUpdate += (sender, eventArgs) => { };

            // When
            var controlWidth = 100.0;
            var controlHeight = 200.0;
            presenter.FinishInitialisation(controlWidth, controlHeight);

            // Then
            Assert.AreEqual(scriptDirectory, _factory._scriptDirectory);
            Assert.AreEqual(pythonExeLocation, _factory._pythonExeLocation);

            _mockPythonScriptRunner.VerifyAll();
        }

        [TestMethod]
        public void When_the_python_script_timer_ticks_it_runs_the_script()
        {
            // Given
            CreateMockFactory();

            var emptyConfiguration = new InformationRadiatorItemConfiguration();
            var presenter = new GitVisualiserPresenter(emptyConfiguration);
            presenter.AddNewIslandToView += (sender, eventArgs) => { };
            presenter.DisplayTextualDataUpdate += (sender, eventArgs) => { };

            var pythonScriptStartedCount = 0;
            _mockPythonScriptRunner.Setup(m => m.Go()).Callback(() =>
            {
                pythonScriptStartedCount++;
            });

            var controlWidth = 100.0;
            var controlHeight = 200.0;
            presenter.FinishInitialisation(controlWidth, controlHeight);

            // When
            _mockPythonScriptUpdateTimer.Raise(m => m.Tick += null, EventArgs.Empty);   // force a tick

            // Then
            var expectedPythonScriptStartedCount = 1;   // for presenter.FinishInitialisation(...)
            expectedPythonScriptStartedCount++; // for _mockPythonScriptUpdateTimer ticking
            Assert.AreEqual(expectedPythonScriptStartedCount, pythonScriptStartedCount);

            _mockPythonScriptRunner.VerifyAll();
        }

        [TestMethod]
        public void When_the_textual_update_timer_ticks_it_does_nothing_when_there_are_no_islands()
        {
            // Given
            CreateMockFactory();
            _mockPythonScriptRunner.Setup(m => m.Go());

            var emptyConfiguration = new InformationRadiatorItemConfiguration();
            var presenter = new GitVisualiserPresenter(emptyConfiguration);
            presenter.AddNewIslandToView += (sender, eventArgs) => { };

            var displayTextualDataUpdateCallCount = 0;
            presenter.DisplayTextualDataUpdate += (sender, eventArgs) => 
            {
                displayTextualDataUpdateCallCount++;
            };

            var controlWidth = 100.0;
            var controlHeight = 200.0;
            presenter.FinishInitialisation(controlWidth, controlHeight);

            // When
            _mockTextualUpdateTimer.Raise(m => m.Tick += null, EventArgs.Empty);   // force a tick

            // Then
            var expectedDisplayTextualDataUpdateCallCount = 0;
            Assert.AreEqual(expectedDisplayTextualDataUpdateCallCount, displayTextualDataUpdateCallCount);
        }

        [TestMethod]
        public void When_the_textual_update_timer_ticks_and_there_is_one_island_it_tells_the_view_to_update_the_label()
        {
            // Given
            CreateMockFactory();
            _mockPythonScriptRunner.Setup(m => m.Go());
            _mockIslands.Setup(m => m.Contains(It.IsAny<string>())).Returns(false);
            _mockIslands.Setup(m => m.SetBranchDistance(It.IsAny<string>(), It.IsAny<double>()));
            _mockIslands.Setup(m => m.MoveAll());
            _mockIslands.Setup(m => m.Add(It.IsAny<GitVisualiser.Model.Branch>()));
            _mockIslands.Setup(m => m.GetLocation(It.IsAny<string>())).Returns(new System.Windows.Point(0, 0));
            _mockIslands.Setup(m => m.GetDistance(It.IsAny<string>())).Returns(100.0);
            _mockIslands.Setup(m => m.Highlight(It.IsAny<string>()));

            var emptyConfiguration = new InformationRadiatorItemConfiguration();
            var presenter = new GitVisualiserPresenter(emptyConfiguration);
            presenter.AddNewIslandToView += (sender, eventArgs) => { };

            List<string> textDisplayed = new List<string>();
            presenter.DisplayTextualDataUpdate += (sender, eventArgs) =>
            {
                textDisplayed.Add(eventArgs.TextToDisplay);
            };

            var controlWidth = 100.0;
            var controlHeight = 200.0;
            presenter.FinishInitialisation(controlWidth, controlHeight);

            // When
            // add island via faking new data from python script
            _mockPythonScriptRunner.Raise(m =>
                m.FinishedEventHandler += null,
                new GitVisualiser.Model.PythonDataEventArgs(new List<Tuple<string, double>> 
                { 
                    new Tuple<string, double>("islandName", 100)
                }
            ));

            // Then
            Assert.AreEqual(1, textDisplayed.Count);
            var expectedIslandName = "islandName\r\n100";
            var actualIslandName = textDisplayed[0];
            Assert.AreEqual(expectedIslandName, actualIslandName);

            _mockIslands.VerifyAll();
            _mockPythonScriptRunner.VerifyAll();
        }

        [TestMethod]
        public void When_the_textual_update_timer_ticks_and_there_are_three_islands_it_tells_the_view_to_update_the_label_once()
        {
            // Given
            CreateMockFactory();
            _mockPythonScriptRunner.Setup(m => m.Go());
            _mockIslands.Setup(m => m.Contains(It.IsAny<string>())).Returns(false);
            _mockIslands.Setup(m => m.SetBranchDistance(It.IsAny<string>(), It.IsAny<double>()));
            _mockIslands.Setup(m => m.MoveAll());
            _mockIslands.Setup(m => m.Add(It.IsAny<GitVisualiser.Model.Branch>()));
            _mockIslands.Setup(m => m.GetLocation(It.IsAny<string>())).Returns(new System.Windows.Point(0, 0));
            _mockIslands.Setup(m => m.GetDistance(It.IsAny<string>())).Returns(100.0);
            _mockIslands.Setup(m => m.Highlight(It.IsAny<string>()));

            var emptyConfiguration = new InformationRadiatorItemConfiguration();
            var presenter = new GitVisualiserPresenter(emptyConfiguration);
            presenter.AddNewIslandToView += (sender, eventArgs) => { };

            List<string> textDisplayed = new List<string>();
            presenter.DisplayTextualDataUpdate += (sender, eventArgs) =>
            {
                textDisplayed.Add(eventArgs.TextToDisplay);
            };

            var controlWidth = 100.0;
            var controlHeight = 200.0;
            presenter.FinishInitialisation(controlWidth, controlHeight);

            // When
            // add island via faking new data from python script
            _mockPythonScriptRunner.Raise(m =>
                m.FinishedEventHandler += null,
                new GitVisualiser.Model.PythonDataEventArgs(new List<Tuple<string, double>> 
                { 
                    new Tuple<string, double>("one", 100),
                    new Tuple<string, double>("two", 100),
                    new Tuple<string, double>("three", 100)
                }
            )); // forces 1 tick

            // Then
            Assert.AreEqual(1, textDisplayed.Count);
            
            var expectedIslandName = "one\r\n100";
            var actualIslandName = textDisplayed[0];
            Assert.AreEqual(expectedIslandName, actualIslandName);

            _mockIslands.VerifyAll();
            _mockPythonScriptRunner.VerifyAll();
        }

        [TestMethod]
        public void If_there_are_three_islands_and_we_have_ticked_thrice_then_when_we_tick_again_it_tells_the_view_to_update_the_label_to_be_for_the_first_island()
        {
            // Given
            CreateMockFactory();
            _mockPythonScriptRunner.Setup(m => m.Go());
            _mockIslands.Setup(m => m.Contains(It.IsAny<string>())).Returns(false);
            _mockIslands.Setup(m => m.SetBranchDistance(It.IsAny<string>(), It.IsAny<double>()));
            _mockIslands.Setup(m => m.MoveAll());
            _mockIslands.Setup(m => m.Add(It.IsAny<GitVisualiser.Model.Branch>()));
            _mockIslands.Setup(m => m.GetLocation(It.IsAny<string>())).Returns(new System.Windows.Point(0, 0));
            _mockIslands.Setup(m => m.GetDistance(It.IsAny<string>())).Returns(100.0);
            _mockIslands.Setup(m => m.Highlight(It.IsAny<string>()));

            var emptyConfiguration = new InformationRadiatorItemConfiguration();
            var presenter = new GitVisualiserPresenter(emptyConfiguration);
            presenter.AddNewIslandToView += (sender, eventArgs) => { };

            List<string> textDisplayed = new List<string>();
            presenter.DisplayTextualDataUpdate += (sender, eventArgs) =>
            {
                textDisplayed.Add(eventArgs.TextToDisplay);
            };

            var controlWidth = 100.0;
            var controlHeight = 200.0;
            presenter.FinishInitialisation(controlWidth, controlHeight);

            // add islands via faking new data from python script
            _mockPythonScriptRunner.Raise(m =>
                m.FinishedEventHandler += null,
                new GitVisualiser.Model.PythonDataEventArgs(new List<Tuple<string, double>> 
                { 
                    new Tuple<string, double>("one", 100),
                    new Tuple<string, double>("two", 100),
                    new Tuple<string, double>("three", 100)
                }
            )); // forces 1 tick

            _mockTextualUpdateTimer.Raise(m => m.Tick += null, EventArgs.Empty);   // force a tick
            _mockTextualUpdateTimer.Raise(m => m.Tick += null, EventArgs.Empty);   // force a tick

            // When
            _mockTextualUpdateTimer.Raise(m => m.Tick += null, EventArgs.Empty);   // force a tick

            // Then
            var expectedIslandNameLastDisplayed = "one\r\n100";
            var actualIslandNameLastDisplayed = textDisplayed[textDisplayed.Count - 1];
            Assert.AreEqual(expectedIslandNameLastDisplayed, actualIslandNameLastDisplayed);

            _mockIslands.VerifyAll();
            _mockPythonScriptRunner.VerifyAll();
        }

        [TestMethod]
        public void When_the_textual_update_timer_ticks_and_there_is_one_island_it_highlights_the_island()
        {
            // Given
            CreateMockFactory();
            _mockPythonScriptRunner.Setup(m => m.Go());
            _mockIslands.Setup(m => m.Contains(It.IsAny<string>())).Returns(false);
            _mockIslands.Setup(m => m.SetBranchDistance(It.IsAny<string>(), It.IsAny<double>()));
            _mockIslands.Setup(m => m.MoveAll());
            _mockIslands.Setup(m => m.Add(It.IsAny<GitVisualiser.Model.Branch>()));
            _mockIslands.Setup(m => m.GetLocation(It.IsAny<string>())).Returns(new System.Windows.Point(0, 0));
            _mockIslands.Setup(m => m.GetDistance(It.IsAny<string>())).Returns(0.0);

            string actualIslandLastHighlightedName = null;
            _mockIslands.Setup(m => m.Highlight(It.IsAny<string>())).Callback(
                (string nameOfBranchParameter) => actualIslandLastHighlightedName = nameOfBranchParameter
            );

            var emptyConfiguration = new InformationRadiatorItemConfiguration();
            var presenter = new GitVisualiserPresenter(emptyConfiguration);
            presenter.AddNewIslandToView += (sender, eventArgs) => { };
            presenter.DisplayTextualDataUpdate += (sender, eventArgs) => { };

            var controlWidth = 100.0;
            var controlHeight = 200.0;
            presenter.FinishInitialisation(controlWidth, controlHeight);

            // add island via faking new data from python script
            _mockPythonScriptRunner.Raise(m =>
                m.FinishedEventHandler += null,
                new GitVisualiser.Model.PythonDataEventArgs(new List<Tuple<string, double>> 
                { 
                    new Tuple<string, double>("islandName", 100)
                }
            ));

            // When
            _mockTextualUpdateTimer.Raise(m => m.Tick += null, EventArgs.Empty);   // force a tick

            // Then
            var expectedIslandName = "islandName";
            Assert.AreEqual(expectedIslandName, actualIslandLastHighlightedName);

            _mockIslands.VerifyAll();
            _mockPythonScriptRunner.VerifyAll();
        }

        [TestMethod]
        public void When_the_textual_label_is_long_it_does_not_go_outsize_of_the_control()
        {
            // Given
            CreateMockFactory();
            _mockPythonScriptRunner.Setup(m => m.Go());
            _mockIslands.Setup(m => m.Contains(It.IsAny<string>())).Returns(false);
            _mockIslands.Setup(m => m.SetBranchDistance(It.IsAny<string>(), It.IsAny<double>()));
            _mockIslands.Setup(m => m.MoveAll());
            _mockIslands.Setup(m => m.Add(It.IsAny<GitVisualiser.Model.Branch>()));
            _mockIslands.Setup(m => m.GetDistance(It.IsAny<string>())).Returns(0.0);
            _mockIslands.Setup(m => m.Highlight(It.IsAny<string>()));

            var controlWidth = 200.0;
            var controlHeight = 200.0;
            var edgeOfControlLocation = new System.Windows.Point(controlWidth - 10, controlHeight / 2);
            _mockIslands.Setup(m => m.GetLocation(It.IsAny<string>())).Returns(edgeOfControlLocation);

            var emptyConfiguration = new InformationRadiatorItemConfiguration();
            var presenter = new GitVisualiserPresenter(emptyConfiguration);
            presenter.AddNewIslandToView += (sender, eventArgs) => { };

            System.Windows.Point actualTextualLabelLocation = new System.Windows.Point();
            presenter.DisplayTextualDataUpdate += (sender, eventArgs) =>
            {
                actualTextualLabelLocation = eventArgs.Location;
            };

            presenter.FinishInitialisation(controlWidth, controlHeight);

            var longBranchName = "branch-name-that-is-really-very-long-indeed";
            // add island via faking new data from python script
            _mockPythonScriptRunner.Raise(m =>
                m.FinishedEventHandler += null,
                new GitVisualiser.Model.PythonDataEventArgs(new List<Tuple<string, double>> 
                { 
                    // with a control width of 200, we should go out of the control
                    new Tuple<string, double>(longBranchName, 500)
                }
            ));

            // When
            _mockTextualUpdateTimer.Raise(m => m.Tick += null, EventArgs.Empty);   // force a tick

            // Then
            var textWidth = TestHelper_GetLabelWidthAndHeight(longBranchName + "\r\n500");
            var rightHandSideOfTheLabelLocation = actualTextualLabelLocation.X + textWidth;
            Assert.IsTrue(rightHandSideOfTheLabelLocation < controlWidth);

            _mockIslands.VerifyAll();
            _mockPythonScriptRunner.VerifyAll();
        }

        [TestMethod]
        public void When_a_new_archipelago_is_added_all_of_the_islands_move()
        {
            // Given
            CreateMockFactory();
            _mockPythonScriptRunner.Setup(m => m.Go());
            _mockIslands.Setup(m => m.Contains(It.IsAny<string>())).Returns(false);
            _mockIslands.Setup(m => m.SetBranchDistance(It.IsAny<string>(), It.IsAny<double>()));
            var actualIslandsMoveAllCallCount = 0;
            _mockIslands.Setup(m => m.MoveAll()).Callback(() => 
            {
                actualIslandsMoveAllCallCount++;
            });
            _mockIslands.Setup(m => m.Add(It.IsAny<GitVisualiser.Model.Branch>()));
            _mockIslands.Setup(m => m.GetLocation(It.IsAny<string>())).Returns(new System.Windows.Point(0, 0));
            _mockIslands.Setup(m => m.GetDistance(It.IsAny<string>())).Returns(0.0);
            _mockIslands.Setup(m => m.Highlight(It.IsAny<string>()));

            var emptyConfiguration = new InformationRadiatorItemConfiguration();
            var presenter = new GitVisualiserPresenter(emptyConfiguration);
            presenter.AddNewIslandToView += (sender, eventArgs) => { };
            presenter.DisplayTextualDataUpdate += (sender, eventArgs) => { };

            var controlWidth = 100.0;
            var controlHeight = 200.0;
            presenter.FinishInitialisation(controlWidth, controlHeight);

            // add island via faking new data from python script
            _mockPythonScriptRunner.Raise(m =>
                m.FinishedEventHandler += null, 
                new GitVisualiser.Model.PythonDataEventArgs(new List<Tuple<string, double>> 
                { 
                    new Tuple<string, double>("old-archipelago-data", 100),
                    new Tuple<string, double>("more-old-archipelago-data", 100),
                    new Tuple<string, double>("yet-more-old-archipelago-data", 100)
                }
            )); // first move

            // When
            _mockPythonScriptRunner.Raise(m =>
                m.FinishedEventHandler += null,
                new GitVisualiser.Model.PythonDataEventArgs(new List<Tuple<string, double>> 
                { 
                    new Tuple<string, double>("never-before-added-archipelago", 100),
                    new Tuple<string, double>("another-never-before-added-archipelago", 100)
                }
            )); // second move

            // Then
            var expectedIslandsMoveAllCallCount = 2;
            Assert.AreEqual(expectedIslandsMoveAllCallCount, actualIslandsMoveAllCallCount);

            _mockIslands.VerifyAll();
            _mockPythonScriptRunner.VerifyAll();
        }

        [TestMethod]
        public void When_data_for_an_archipelago_that_has_never_been_displayed_before_is_received_then_it_is_added_to_the_view()
        {
            // Given
            CreateMockFactory();
            _mockPythonScriptRunner.Setup(m => m.Go());
            _mockIslands.Setup(m => m.Contains(It.IsAny<string>())).Returns(false);
            _mockIslands.Setup(m => m.SetBranchDistance(It.IsAny<string>(), It.IsAny<double>()));
            _mockIslands.Setup(m => m.MoveAll());
            _mockIslands.Setup(m => m.Add(It.IsAny<GitVisualiser.Model.Branch>()));
            _mockIslands.Setup(m => m.GetLocation(It.IsAny<string>())).Returns(new System.Windows.Point(0, 0));
            _mockIslands.Setup(m => m.GetDistance(It.IsAny<string>())).Returns(0.0);
            _mockIslands.Setup(m => m.Highlight(It.IsAny<string>()));

            var emptyConfiguration = new InformationRadiatorItemConfiguration();
            var presenter = new GitVisualiserPresenter(emptyConfiguration);
            var actualIslandAddedToViewCount = 0;
            presenter.AddNewIslandToView += (sender, eventArgs) => 
                {
                    actualIslandAddedToViewCount++;
                };

            presenter.DisplayTextualDataUpdate += (sender, eventArgs) => { };

            var controlWidth = 100.0;
            var controlHeight = 200.0;
            presenter.FinishInitialisation(controlWidth, controlHeight);

            // When
            // add island via faking new data from python script
            _mockPythonScriptRunner.Raise(m =>
                m.FinishedEventHandler += null,
                new GitVisualiser.Model.PythonDataEventArgs(new List<Tuple<string, double>> 
                { 
                    new Tuple<string, double>("a-new-archipelago", 100),
                }
            ));

            // Then
            var expectedIslandAddedToViewCount = 2; // 1 for the release archipelago, one for "a-new-archipelago"
            Assert.AreEqual(expectedIslandAddedToViewCount, actualIslandAddedToViewCount);

            _mockIslands.VerifyAll();
            _mockPythonScriptRunner.VerifyAll();
        }

        [TestMethod]
        public void When_data_for_multiple_archipelagos_that_have_never_been_displayed_before_are_received_then_they_are_individually_added_to_the_view()
        {
            // Given
            CreateMockFactory();
            _mockPythonScriptRunner.Setup(m => m.Go());
            _mockIslands.Setup(m => m.Contains(It.IsAny<string>())).Returns(false);
            _mockIslands.Setup(m => m.SetBranchDistance(It.IsAny<string>(), It.IsAny<double>()));
            _mockIslands.Setup(m => m.MoveAll());
            _mockIslands.Setup(m => m.Add(It.IsAny<GitVisualiser.Model.Branch>()));
            _mockIslands.Setup(m => m.GetLocation(It.IsAny<string>())).Returns(new System.Windows.Point(0, 0));
            _mockIslands.Setup(m => m.GetDistance(It.IsAny<string>())).Returns(0.0);
            _mockIslands.Setup(m => m.Highlight(It.IsAny<string>()));

            var emptyConfiguration = new InformationRadiatorItemConfiguration();
            var presenter = new GitVisualiserPresenter(emptyConfiguration);
            presenter.DisplayTextualDataUpdate += (sender, eventArgs) => { };

            var actualIslandAddedToViewCount = 0;
            presenter.AddNewIslandToView += (sender, eventArgs) =>
            {
                actualIslandAddedToViewCount++;
            };

            var controlWidth = 100.0;
            var controlHeight = 200.0;
            presenter.FinishInitialisation(controlWidth, controlHeight);

            // When
            // add island via faking new data from python script
            _mockPythonScriptRunner.Raise(m =>
                m.FinishedEventHandler += null,
                new GitVisualiser.Model.PythonDataEventArgs(new List<Tuple<string, double>> 
                { 
                    new Tuple<string, double>("a-new-archipelago", 100),
                }
            ));

            _mockPythonScriptRunner.Raise(m =>
                m.FinishedEventHandler += null,
                new GitVisualiser.Model.PythonDataEventArgs(new List<Tuple<string, double>> 
                { 
                    new Tuple<string, double>("bob", 100),
                    new Tuple<string, double>("ben", 100),
                    new Tuple<string, double>("bill", 100),
                }
            ));

            // Then
            var expectedIslandAddedToViewCount = 5; // 1 for the release archipelago
            Assert.AreEqual(expectedIslandAddedToViewCount, actualIslandAddedToViewCount);

            _mockIslands.VerifyAll();
            _mockPythonScriptRunner.VerifyAll();
        }

        [TestMethod]
        public void When_data_for_an_archipelago_that_has_never_been_displayed_before_is_received_then_the_view_is_told_to_update_the_label()
        {
            // Given
            CreateMockFactory();
            _mockPythonScriptRunner.Setup(m => m.Go());
            _mockIslands.Setup(m => m.Contains(It.IsAny<string>())).Returns(false);
            _mockIslands.Setup(m => m.SetBranchDistance(It.IsAny<string>(), It.IsAny<double>()));
            _mockIslands.Setup(m => m.MoveAll());
            _mockIslands.Setup(m => m.Add(It.IsAny<GitVisualiser.Model.Branch>()));
            _mockIslands.Setup(m => m.GetLocation(It.IsAny<string>())).Returns(new System.Windows.Point(0, 0));
            _mockIslands.Setup(m => m.GetDistance(It.IsAny<string>())).Returns(100.0);
            _mockIslands.Setup(m => m.Highlight(It.IsAny<string>()));

            var emptyConfiguration = new InformationRadiatorItemConfiguration();
            var presenter = new GitVisualiserPresenter(emptyConfiguration);
            var actualIslandAddedToViewCount = 0;
            presenter.AddNewIslandToView += (sender, eventArgs) =>
            {
                actualIslandAddedToViewCount++;
            };

            List<string> textualLabelText = new List<string>();
            presenter.DisplayTextualDataUpdate += (sender, eventArgs) =>
            {
                textualLabelText.Add(eventArgs.TextToDisplay);
            };

            var controlWidth = 100.0;
            var controlHeight = 200.0;
            presenter.FinishInitialisation(controlWidth, controlHeight);

            // When
            // add island via faking new data from python script
            _mockPythonScriptRunner.Raise(m =>
                m.FinishedEventHandler += null,
                new GitVisualiser.Model.PythonDataEventArgs(new List<Tuple<string, double>> 
                { 
                    new Tuple<string, double>("a-new-archipelago", 100),
                }
            ));

            // Then
            var expectedIslandAddedToViewCount = 2; // 1 for the release archipelago
            Assert.AreEqual(expectedIslandAddedToViewCount, actualIslandAddedToViewCount);

            var dataWasGivenToViewToDisplay = textualLabelText.Contains("a-new-archipelago\r\n100");
            Assert.IsTrue(dataWasGivenToViewToDisplay);

            _mockIslands.VerifyAll();
            _mockPythonScriptRunner.VerifyAll();
        }

        [TestMethod]
        public void When_data_for_multiple_archipelagos_that_have_never_been_displayed_before_is_received_then_the_view_is_told_to_update_the_label_once()
        {
            // Given
            CreateMockFactory();
            _mockPythonScriptRunner.Setup(m => m.Go());
            _mockIslands.Setup(m => m.Contains(It.IsAny<string>())).Returns(false);
            _mockIslands.Setup(m => m.SetBranchDistance(It.IsAny<string>(), It.IsAny<double>()));
            _mockIslands.Setup(m => m.MoveAll());
            _mockIslands.Setup(m => m.Add(It.IsAny<GitVisualiser.Model.Branch>()));
            _mockIslands.Setup(m => m.GetLocation(It.IsAny<string>())).Returns(new System.Windows.Point(0, 0));
            _mockIslands.Setup(m => m.GetDistance(It.IsAny<string>())).Returns(0.0);
            _mockIslands.Setup(m => m.Highlight(It.IsAny<string>()));

            var emptyConfiguration = new InformationRadiatorItemConfiguration();
            var presenter = new GitVisualiserPresenter(emptyConfiguration);
            presenter.AddNewIslandToView += (sender, eventArgs) => { };

            List<string> textualLabelText = new List<string>();
            presenter.DisplayTextualDataUpdate += (sender, eventArgs) =>
            {
                textualLabelText.Add(eventArgs.TextToDisplay);
            };

            var controlWidth = 100.0;
            var controlHeight = 200.0;
            presenter.FinishInitialisation(controlWidth, controlHeight);

            // When
            // add island via faking new data from python script
            _mockPythonScriptRunner.Raise(m =>
                m.FinishedEventHandler += null,
                new GitVisualiser.Model.PythonDataEventArgs(new List<Tuple<string, double>> 
                { 
                    new Tuple<string, double>("hey", 100),
                    new Tuple<string, double>("there", 100),
                    new Tuple<string, double>("I", 100),
                    new Tuple<string, double>("just", 100),
                }
            ));

            // Then
            var actualNumberOfTimeLabelHasBeenUpdated = textualLabelText.Count;
            var expectedNumberOfTimeLabelHasBeenUpdated = 1;
            Assert.AreEqual(expectedNumberOfTimeLabelHasBeenUpdated, actualNumberOfTimeLabelHasBeenUpdated);

            _mockIslands.VerifyAll();
            _mockPythonScriptRunner.VerifyAll();
        }

        [TestMethod]
        public void When_no_data_for_archipelagos_have_been_received_and_the_label_timer_ticks_then_the_view_is_not_updated()
        {
            // Given
            CreateMockFactory();
            _mockPythonScriptRunner.Setup(m => m.Go());

            var emptyConfiguration = new InformationRadiatorItemConfiguration();
            var presenter = new GitVisualiserPresenter(emptyConfiguration);
            presenter.AddNewIslandToView += (sender, eventArgs) => { };

            List<string> textualLabelText = new List<string>();
            presenter.DisplayTextualDataUpdate += (sender, eventArgs) =>
            {
                textualLabelText.Add(eventArgs.TextToDisplay);
            };

            var controlWidth = 100.0;
            var controlHeight = 200.0;
            presenter.FinishInitialisation(controlWidth, controlHeight);

            // When
            _mockTextualUpdateTimer.Raise(m => m.Tick += null, EventArgs.Empty);   // force a tick

            // Then
            var actualNumberOfTimeLabelHasBeenUpdated = textualLabelText.Count;
            var expectedNumberOfTimeLabelHasBeenUpdated = 0;
            Assert.AreEqual(expectedNumberOfTimeLabelHasBeenUpdated, actualNumberOfTimeLabelHasBeenUpdated);

            _mockIslands.VerifyAll();
            _mockPythonScriptRunner.VerifyAll();
        }

        [TestMethod]
        public void Deleted_archipelagos_are_removed_from_Islands_and_the_view()
        {
            // Given
            CreateMockFactory();
            _mockPythonScriptRunner.Setup(m => m.Go());
            _mockIslands.Setup(m => m.Contains(It.IsAny<string>())).Returns(false);
            _mockIslands.Setup(m => m.SetBranchDistance(It.IsAny<string>(), It.IsAny<double>()));
            _mockIslands.Setup(m => m.MoveAll());
            _mockIslands.Setup(m => m.GetLocation(It.IsAny<string>())).Returns(new System.Windows.Point(0, 0));
            _mockIslands.Setup(m => m.GetDistance(It.IsAny<string>())).Returns(0.0);
            _mockIslands.Setup(m => m.Highlight(It.IsAny<string>()));

            var actualIslandNames = new List<string>();
            _mockIslands.Setup(m => m.Add(It.IsAny<GitVisualiser.Model.Branch>())).Callback<GitVisualiser.Model.Branch>((branch) => 
            {
                if (actualIslandNames.Contains(branch.Name))
                {
                    return;
                }

                actualIslandNames.Add(branch.Name);
            });

            _mockIslands.Setup(m => m.GetAllNames()).Returns(actualIslandNames);

            _mockIslands.Setup(m => m.Delete(It.IsAny<string>())).Callback<string>(branchDeletedName =>
            {
                actualIslandNames.Remove(branchDeletedName);
            });

            var emptyConfiguration = new InformationRadiatorItemConfiguration();
            var presenter = new GitVisualiserPresenter(emptyConfiguration);
            presenter.AddNewIslandToView += (sender, eventArgs) => { };
            presenter.DisplayTextualDataUpdate += (sender, eventArgs) => { };

            string actualNameOfIslandRemovedFromView = null;
            presenter.RemoveIslandFromView += (sender, eventArgs) =>
            {
                actualNameOfIslandRemovedFromView = eventArgs.Name;
            };

            var controlWidth = 100.0;
            var controlHeight = 200.0;
            presenter.FinishInitialisation(controlWidth, controlHeight);

            // add island via faking new data from python script
            _mockPythonScriptRunner.Raise(m =>
                m.FinishedEventHandler += null,
                new GitVisualiser.Model.PythonDataEventArgs(new List<Tuple<string, double>> 
                { 
                    new Tuple<string, double>("one", 100),
                    new Tuple<string, double>("two", 100),
                    new Tuple<string, double>("three-going-to-be-deleted", 100),
                    new Tuple<string, double>("four", 100),
                }
            ));

            // When
            _mockPythonScriptRunner.Raise(m =>
                m.FinishedEventHandler += null,
                new GitVisualiser.Model.PythonDataEventArgs(new List<Tuple<string, double>> 
                { 
                    new Tuple<string, double>("one", 100),
                    new Tuple<string, double>("two", 100),  // no three
                    new Tuple<string, double>("four", 100),
                }
            ));

            // Then
            var expectedBranchName = "three-going-to-be-deleted";
            var deletedBranchIsInListOfBranchNames = actualIslandNames.Contains(expectedBranchName);
            Assert.IsFalse(deletedBranchIsInListOfBranchNames);

            Assert.AreEqual(expectedBranchName, actualNameOfIslandRemovedFromView);
        }

        [TestMethod]
        public void Ticking_the_textual_update_timer_does_not_highlight_deleted_branches()
        {
            // Given
            CreateMockFactory();
            _mockPythonScriptRunner.Setup(m => m.Go());
            _mockIslands.Setup(m => m.Contains(It.IsAny<string>())).Returns(false);
            _mockIslands.Setup(m => m.SetBranchDistance(It.IsAny<string>(), It.IsAny<double>()));
            _mockIslands.Setup(m => m.MoveAll());
            _mockIslands.Setup(m => m.GetLocation(It.IsAny<string>())).Returns(new System.Windows.Point(0, 0));
            _mockIslands.Setup(m => m.GetDistance(It.IsAny<string>())).Returns(0.0);

            var actualIslandNames = new List<string>();
            _mockIslands.Setup(m => m.Add(It.IsAny<GitVisualiser.Model.Branch>())).Callback<GitVisualiser.Model.Branch>((branch) =>
            {
                if (actualIslandNames.Contains(branch.Name))
                {
                    return;
                }

                actualIslandNames.Add(branch.Name);
            });
            _mockIslands.Setup(m => m.GetAllNames()).Returns(actualIslandNames);
            _mockIslands.Setup(m => m.Delete(It.IsAny<string>())).Callback<string>(branchDeletedName =>
            {
                actualIslandNames.Remove(branchDeletedName);
            });

            string actualLastBranchHighlightedName = null;
            _mockIslands.Setup(m => m.Highlight(It.IsAny<string>())).Callback<string>((branchName) =>
            {
                actualLastBranchHighlightedName = branchName;
            });

            var emptyConfiguration = new InformationRadiatorItemConfiguration();
            var presenter = new GitVisualiserPresenter(emptyConfiguration);
            presenter.AddNewIslandToView += (sender, eventArgs) => { };

            string actualLastTextualDataDisplayed = null;
            presenter.DisplayTextualDataUpdate += (sender, eventArgs) => 
            {
                actualLastTextualDataDisplayed = eventArgs.TextToDisplay;
            };
            presenter.RemoveIslandFromView += (sender, eventArgs) =>{ };

            var controlWidth = 100.0;
            var controlHeight = 200.0;
            presenter.FinishInitialisation(controlWidth, controlHeight);

            // add island via faking new data from python script
            _mockPythonScriptRunner.Raise(m =>
                m.FinishedEventHandler += null,
                new GitVisualiser.Model.PythonDataEventArgs(new List<Tuple<string, double>> 
                { 
                    new Tuple<string, double>("one", 0),
                    new Tuple<string, double>("two", 0),
                    new Tuple<string, double>("three-going-to-be-deleted", 0),
                    new Tuple<string, double>("four", 0),
                }
            )); // this causes a textual update timer tick (one is highlighted)

            // no three, therefore causes it to remove an island
            _mockPythonScriptRunner.Raise(m =>
                m.FinishedEventHandler += null,
                new GitVisualiser.Model.PythonDataEventArgs(new List<Tuple<string, double>> 
                { 
                    new Tuple<string, double>("one", 0),
                    new Tuple<string, double>("two", 0),  // no three
                    new Tuple<string, double>("four", 0),
                }
            )); // this causes a textual update timer tick (two is highlighted)

            // When
            _mockTextualUpdateTimer.Raise(m => m.Tick += null, EventArgs.Empty); // force a tick, four should be highlighted

            // Then
            var expectedLastBranchHighlightedName = "four";
            Assert.AreEqual(expectedLastBranchHighlightedName, actualLastBranchHighlightedName);

            var expectedLastTextualDataDisplayed = "four\r\n0";
            Assert.AreEqual(expectedLastTextualDataDisplayed, actualLastTextualDataDisplayed);
        }
    }
}
