using System;
using System.Collections.Generic;

namespace LeanKit.Model
{
    public interface ILeanKitPoints
    {
        int CompletePoints { get; }
        int InProgressPoints { get; }
        int ReadyPoints { get; }
        int UntypedPoints { get; }
        void Update(long boardId, IList<long> ignoresLanes);
    }
}
