using System;
using System.Windows;
using System.Diagnostics;
using System.Windows.Threading;
using System.Collections.Generic;

namespace GitVisualiser.Model
{
    public class PythonScriptRunner : IPythonScriptRunner
    {
        public double Distance;
        public string BranchName;

        public event EventHandler<PythonDataEventArgs> FinishedEventHandler;

        private readonly string _scriptDirectory;
        private readonly string _fileToRun;
        private readonly string _programArguments;
        private readonly string _exeLocation;
        private bool _isRunning;
        private System.Windows.Threading.Dispatcher _threadDispatcherOnConstruction;
        private readonly string _jsonDirectory;
        private IJsonReaderAndDeleter _jsonReader;

        public PythonScriptRunner(string scriptDirectory, string exeLocation)
        {
            _scriptDirectory = scriptDirectory;
            _fileToRun = @"\run_no_gui.py";
            _programArguments = _scriptDirectory + @"\config.txt";
            _exeLocation = exeLocation;
            _isRunning = false;
            _jsonDirectory = _scriptDirectory + @"\json\";
            _jsonReader = PythonScriptRunnerFactory.Instance.CreateJsonReader();

            // hopefully nobody creates this guy outside of the main thread
            _threadDispatcherOnConstruction = System.Windows.Threading.Dispatcher.CurrentDispatcher;

            // later I want to use c# processes instead of python proceses:
            // branchList = run_cmd(pythonScriptDirectory+"\run_get_branches.py", "");
            // for branchList:
            //      run_cmd(pythonScriptDirectory+"\run_no_gui.py", "branchName");
            // for processList:
            //      if process.processHasFinished:
            //          send back data to parent (the view) that it needs to update that branch's visualisation
        }

        public void Go()
        {
            if (_isRunning)
            {
                return;
            }

            Run();
        }

        private void Run()
        {
            _isRunning = true;

            string arguments = _scriptDirectory + _fileToRun + " " + _programArguments;
            var pythonProcess = PythonScriptRunnerFactory.Instance.CreateProcess(_exeLocation, _scriptDirectory, arguments);

            pythonProcess.Start();

            pythonProcess.Exited += _pythonProcess_Exited;

            // The following is just in case pythonProcess finished before it could add "_pythonProcess_Exited(...)" to the event handler
            // This is unlikely in the case of our current python script (unless debugging or it fails...)
            var timeout = 1;
            var pythonCodeFinished = pythonProcess.WaitForExit(timeout);
            if (pythonCodeFinished)
            {
                _pythonProcess_Exited(this, EventArgs.Empty);
            }
        }

        private void _pythonProcess_Exited(object sender, EventArgs e)
        {
            _threadDispatcherOnConstruction.Invoke(new Action(() =>
            {
                ReadJsonAndSendPythonDataEvent();
                RemoveJsonFilesJustGenerated();

                _isRunning = false;
            }));
        }

        private void ReadJsonAndSendPythonDataEvent()
        {
            if (FinishedEventHandler == null)
            {
                return;
            }

            List<Tuple<string, double>> archipelagoData = _jsonReader.GetArchipelagoDataFromDirectory(_jsonDirectory);

            if (archipelagoData.Count > 0)
            {
                FinishedEventHandler(this, new PythonDataEventArgs(archipelagoData));
            }
        }

        private void RemoveJsonFilesJustGenerated()
        {
            _jsonReader.RemoveJsonFilesFromDirectory(_jsonDirectory);
        }
    }
}