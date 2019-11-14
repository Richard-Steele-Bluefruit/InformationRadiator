using System;
using System.Collections.Generic;

namespace GitVisualiser.Presenters
{
    public class DisplayAddDrawableDataEventArgs : EventArgs
    {
        public string Name { get; private set; }
        public List<System.Windows.Shapes.Shape> Drawables { get; private set; }

        public DisplayAddDrawableDataEventArgs(string name, List<System.Windows.Shapes.Shape> newDrawables)
        {
            Name = name;
            Drawables = newDrawables;
        }
    }
}
