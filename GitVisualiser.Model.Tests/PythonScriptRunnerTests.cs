using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Windows.Media;

using GitVisualiser.Model;

namespace GitVisualiser.Model.Tests
{
    [TestClass]
    public class PythonScriptRunnerTests
    {
        PythonScriptRunnerFactoryMock _factory;
        Mock<GitVisualiser.Model.IProcessWrapper> _mockProcessWrapper;
        Mock<GitVisualiser.Model.IJsonReaderAndDeleter> _mockJsonReader;

        #region Helper Methods

        [TestCleanup]
        public void CleanUp()
        {
            PythonScriptRunnerFactoryMock.Instance = null;
        }

        private void CreateMockFactory(bool useDefaultBranch = true)
        {
            _factory = new PythonScriptRunnerFactoryMock();
            PythonScriptRunnerFactoryMock.Instance = _factory;

            _mockProcessWrapper = new Mock<IProcessWrapper>(MockBehavior.Strict);
            _factory._processWrapper = _mockProcessWrapper.Object;

            _mockJsonReader = new Mock<IJsonReaderAndDeleter>(MockBehavior.Strict);
            _factory._jsonReader = _mockJsonReader.Object;
        }

        #endregion Helper Methods

        [TestMethod]
        public void Running_the_script_and_exiting_the_process_raises_an_event_which_gives_the_arhcipelago_data()
        {
            // Given
            CreateMockFactory();
            _mockProcessWrapper.Setup(m => m.Start());
            _mockProcessWrapper.Setup(m => m.WaitForExit(It.IsAny<int>())).Returns(false);

            List<Tuple<string, double>> expectedArchipelagoData = new List<Tuple<string, double>>();
            expectedArchipelagoData.Add(new Tuple<string, double>("branch name 1", 10));
            expectedArchipelagoData.Add(new Tuple<string, double>("branch name 2", 20));
            expectedArchipelagoData.Add(new Tuple<string, double>("branch name 3", 30));
            _mockJsonReader.Setup(m => m.GetArchipelagoDataFromDirectory(It.IsAny<string>())).Returns(expectedArchipelagoData);
            _mockJsonReader.Setup(m => m.RemoveJsonFilesFromDirectory(It.IsAny<string>()));

            var scriptRunner = new PythonScriptRunner("scriptDirectory", "pythonExeLocation");

            List<Tuple<string, double>> actualArchipelagoData = new List<Tuple<string, double>>();
            scriptRunner.FinishedEventHandler += (sender, eventArgs) => 
            { 
                actualArchipelagoData = eventArgs.ArchipelagoData; 
            };
            
            // When
            scriptRunner.Go();

            // fake process saying that it has finished
            _mockProcessWrapper.Raise(m => m.Exited += null, EventArgs.Empty);

            // Then
            for (var i = 0; i < actualArchipelagoData.Count; i++)
            {
                var expected = expectedArchipelagoData[i];
                var actual = actualArchipelagoData[i];

                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void Running_the_script_and_exiting_the_process_does_not_raise_an_event_which_gives_the_arhcipelago_data_when_there_was_no_data()
        {
            // Given
            CreateMockFactory();
            _mockProcessWrapper.Setup(m => m.Start());
            _mockProcessWrapper.Setup(m => m.WaitForExit(It.IsAny<int>())).Returns(false);

            List<Tuple<string, double>> emptyArchipelagoData = new List<Tuple<string, double>>(); // no data
            _mockJsonReader.Setup(m => m.GetArchipelagoDataFromDirectory(It.IsAny<string>())).Returns(emptyArchipelagoData);
            _mockJsonReader.Setup(m => m.RemoveJsonFilesFromDirectory(It.IsAny<string>()));

            var scriptRunner = new PythonScriptRunner("scriptDirectory", "pythonExeLocation");

            int actualFinishedEventHandlerCallCount = 0;
            scriptRunner.FinishedEventHandler += (sender, eventArgs) =>
            {
                actualFinishedEventHandlerCallCount++;
            };

            // When
            scriptRunner.Go();

            // fake process saying that it has finished
            _mockProcessWrapper.Raise(m => m.Exited += null, EventArgs.Empty);

            // Then
            int expectedFinishedEventHandlerCallCount = 0;
            Assert.AreEqual(expectedFinishedEventHandlerCallCount, actualFinishedEventHandlerCallCount);
        }

        [TestMethod]
        public void Running_checks_for_exit_happening_before_adding_exit_delegate_to_exit_event_handler()
        {
            // Given
            CreateMockFactory();
            _mockProcessWrapper.Setup(m => m.Start());
            // this should force the script to read the json
            _mockProcessWrapper.Setup(m => m.WaitForExit(It.IsAny<int>())).Returns(true);

            List<Tuple<string, double>> archipelagoData = new List<Tuple<string, double>>();
            archipelagoData.Add(new Tuple<string, double>("branch name 1", 10));
            archipelagoData.Add(new Tuple<string, double>("branch name 2", 20));
            archipelagoData.Add(new Tuple<string, double>("branch name 3", 30));
            _mockJsonReader.Setup(m => m.GetArchipelagoDataFromDirectory(It.IsAny<string>())).Returns(archipelagoData);
            _mockJsonReader.Setup(m => m.RemoveJsonFilesFromDirectory(It.IsAny<string>()));

            var scriptRunner = new PythonScriptRunner("scriptDirectory", "pythonExeLocation");

            int actualFinishedEventHandlerCallCount = 0;
            scriptRunner.FinishedEventHandler += (sender, eventArgs) =>
            {
                actualFinishedEventHandlerCallCount++;
            };

            // When
            scriptRunner.Go();

            // not faking the process saying that it has finished

            // Then
            int expectedFinishedEventHandlerCallCount = 1;
            Assert.AreEqual(expectedFinishedEventHandlerCallCount, actualFinishedEventHandlerCallCount);
        }

        [TestMethod]
        public void Running_twice_without_completing_the_first_one_does_not_create_the_process_again()
        {
            // Given
            CreateMockFactory();

            int actualStartCalledCount = 0;
            _mockProcessWrapper.Setup(m => m.Start()).Callback(() =>
            {
                actualStartCalledCount++;
            });
            _mockProcessWrapper.Setup(m => m.WaitForExit(It.IsAny<int>())).Returns(false);

            var scriptRunner = new PythonScriptRunner("scriptDirectory", "pythonExeLocation");

            // When
            scriptRunner.Go();
            scriptRunner.Go(); // should do nothing

            // Then
            int expectedStartCalledCount = 1;
            Assert.AreEqual(expectedStartCalledCount, actualStartCalledCount);
        }

        [TestMethod]
        public void Json_files_are_deleted_after_a_run_completes()
        {
            // Given
            CreateMockFactory();
            _mockProcessWrapper.Setup(m => m.Start());
            _mockProcessWrapper.Setup(m => m.WaitForExit(It.IsAny<int>())).Returns(true);

            var actualRemoveJsonFilesFromDirectoryCallCount = 0;
            _mockJsonReader.Setup(m => m.RemoveJsonFilesFromDirectory(It.IsAny<string>())).Callback(() =>
            {
                actualRemoveJsonFilesFromDirectoryCallCount++;
            });

            var scriptRunner = new PythonScriptRunner("scriptDirectory", "pythonExeLocation");

            // When
            scriptRunner.Go();

            // Then
            var expectedRemoveJsonFilesFromDirectoryCallCount = 1;
            Assert.AreEqual(expectedRemoveJsonFilesFromDirectoryCallCount, actualRemoveJsonFilesFromDirectoryCallCount);
        }
    }
}
