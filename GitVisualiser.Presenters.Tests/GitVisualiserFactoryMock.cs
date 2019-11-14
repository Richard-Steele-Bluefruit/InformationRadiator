using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitVisualiser.Presenters.Tests
{
    class GitVisualiserFactoryMock : GitVisualiserPresenterFactory
    {
        public GitVisualiser.Model.IIslands _islands;
        public GitVisualiser.Model.ViewControlData _viewControlData;
        public GitVisualiser.Model.IPythonScriptRunner _pythonScriptRunner;
        public List<PresenterCommon.ITimer> _timers = new List<PresenterCommon.ITimer>();
        public Dictionary<PresenterCommon.ITimer, double> _timersToIntervals = new Dictionary<PresenterCommon.ITimer, double>();
        public string _scriptDirectory;
        public string _pythonExeLocation;

        public override GitVisualiser.Model.IIslands CreateIslands(
            GitVisualiser.Model.ViewControlData viewControlData, 
            GitVisualiser.Model.Branch releaseArchipelago)
        {
            var result = _islands;
            _islands = null;
            _viewControlData = viewControlData;
            return result;
        }
        
        public override PresenterCommon.ITimer CreateTimer(double interval)
        {
            var mockTimer = _timers[_timers.Count - 1];
            _timers.Remove(mockTimer);
            _timersToIntervals.Add(mockTimer, interval);

            return mockTimer;
        }

        public override GitVisualiser.Model.IPythonScriptRunner CreatePythonScriptRunner(
            string scriptDirectory,
            string pythonExeLocation)
        {
            var scriptRunner = _pythonScriptRunner;
            _pythonScriptRunner = null;

            _scriptDirectory = scriptDirectory;
            _pythonExeLocation = pythonExeLocation;

            return scriptRunner;
        }
    }
}
