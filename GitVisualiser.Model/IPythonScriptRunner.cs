using System;

namespace GitVisualiser.Model
{
    public interface IPythonScriptRunner
    {
        event EventHandler<PythonDataEventArgs> FinishedEventHandler;

        void Go();
    }
}