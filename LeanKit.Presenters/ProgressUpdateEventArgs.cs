using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeanKit.Presenters
{
    public class ProgressUpdateEventArgs : EventArgs
    {
        public int ReadyPoints { get; private set; }
        public int InProgressPoints { get; private set; }
        public int CompletePoints { get; private set; }
        public int UntypedPoints { get; private set; }
        internal ProgressUpdateEventArgs(int readyPoints, int inProgressPoints, int completePoints, int untypedPoints)
        {
            ReadyPoints = readyPoints;
            InProgressPoints = inProgressPoints;
            CompletePoints = completePoints;
            UntypedPoints = untypedPoints;
        }
    }
}
