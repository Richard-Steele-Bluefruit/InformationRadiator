using System;
using System.Collections.Generic;

namespace GitVisualiser.Model
{
    public class PythonDataEventArgs : EventArgs
    {
        public List<Tuple<string, double>> ArchipelagoData { get; private set; }

        public PythonDataEventArgs(List<Tuple<string, double>> archipelagoData)
        {
            ArchipelagoData = archipelagoData;
        }
    }
}
