using System;
using System.Linq;

namespace GitVisualiser.Model
{
    public interface IProcessWrapper
    {
        event EventHandler<EventArgs> Exited;
        void Start();
        bool WaitForExit(int timeout);
    }
}
