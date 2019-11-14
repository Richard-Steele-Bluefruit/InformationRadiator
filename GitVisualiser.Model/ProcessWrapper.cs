using System;
using System.Windows;
using System.Diagnostics;

namespace GitVisualiser.Model
{
    public class ProcessWrapper : IProcessWrapper
    {
        public event EventHandler<EventArgs> Exited;

        private Process _process;

        public ProcessWrapper(string fileName, string workingDirectory, string arguments)
        {
            _process = new Process();
            _process.StartInfo.FileName = fileName;
            _process.StartInfo.WorkingDirectory = workingDirectory;
            _process.StartInfo.Arguments = arguments;
            _process.EnableRaisingEvents = true;
            _process.StartInfo.UseShellExecute = false;

            // comment these two lines for easier debugging
            _process.StartInfo.RedirectStandardOutput = false;
            _process.StartInfo.CreateNoWindow = true;

            _process.Exited += _process_Exited;
        }

        public void Start()
        {
            _process.Start();
        }

        public bool WaitForExit(int timeout)
        {
            return _process.WaitForExit(timeout);
        }

        private void _process_Exited(object sender, EventArgs e)
        {
            Exited(this, EventArgs.Empty);
        }
    }
}
