using System;
using System.Collections.Generic;

namespace GitVisualiser.Presenters
{
    public class DisplayRemoveDrawableDataEventArgs : EventArgs
    {
        public string Name { get; private set; }

        public DisplayRemoveDrawableDataEventArgs(string name)
        {
            Name = name;
        }
    }
}
