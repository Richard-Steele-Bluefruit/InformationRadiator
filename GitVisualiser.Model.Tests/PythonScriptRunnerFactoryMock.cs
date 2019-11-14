using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GitVisualiser.Model.Tests
{
    public class PythonScriptRunnerFactoryMock : PythonScriptRunnerFactory
    {
        public IProcessWrapper _processWrapper;
        public IJsonReaderAndDeleter _jsonReader;

        public override IProcessWrapper CreateProcess(string fileName, string workingDirectory, string arguments)
        {
            var processWrapper = _processWrapper;
            _processWrapper = null;
            return processWrapper;
        }

        public override IJsonReaderAndDeleter CreateJsonReader()
        {
            var jsonReader = _jsonReader;
            _jsonReader = null;
            return jsonReader;
        }
    }
}
