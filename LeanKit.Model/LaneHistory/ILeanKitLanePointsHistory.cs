using System;

using LeanKit.API.Client.Library.TransferObjects;

namespace LeanKit.Model.LaneHistory
{
    public interface ILeanKitLanePointsHistory
    {
        System.Collections.ObjectModel.ReadOnlyCollection<LanePointsHistory> LaneHistory { get; }
        Board Board { get; }
        void Update(int numberOfDaysHistory = 10);
    }
}
