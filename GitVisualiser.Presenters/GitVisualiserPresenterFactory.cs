using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GitVisualiser.Presenters
{
    public class GitVisualiserPresenterFactory
    {
        private static GitVisualiserPresenterFactory _instance;

        public static GitVisualiserPresenterFactory Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GitVisualiserPresenterFactory();
                return _instance;
            }
            internal set
            {
                _instance = value;
            }
        }

        public virtual GitVisualiser.Model.IIslands CreateIslands(
            GitVisualiser.Model.ViewControlData viewControlData, 
            GitVisualiser.Model.Branch releaseArchipelago)
        {
            return new GitVisualiser.Model.Islands(viewControlData, releaseArchipelago);
        }

        public virtual PresenterCommon.ITimer CreateTimer(double interval)
        {
            return new PresenterCommon.DotNetTimer(interval);
        }

        public virtual GitVisualiser.Model.IPythonScriptRunner CreatePythonScriptRunner(
            string scriptDirectory,
            string pythonExeLocation)
        {
            return new GitVisualiser.Model.PythonScriptRunner(scriptDirectory, pythonExeLocation);
        }
    }
}
