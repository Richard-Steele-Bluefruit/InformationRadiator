using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GitVisualiser.Model
{
    public class PythonScriptRunnerFactory
    {
        private static PythonScriptRunnerFactory _instance;

        public static PythonScriptRunnerFactory Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new PythonScriptRunnerFactory();
                return _instance;
            }
            internal set
            {
                _instance = value;
            }
        }

        public virtual IProcessWrapper CreateProcess(string fileName, string workingDirectory, string arguments)
        {
            return new ProcessWrapper(fileName, workingDirectory, arguments);
        }

        public virtual IJsonReaderAndDeleter CreateJsonReader()
        {
            return new JsonReaderAndDeleter();
        }
    }
}
