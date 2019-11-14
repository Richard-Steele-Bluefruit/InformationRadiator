using System;
using System.Linq;
using System.Collections.Generic;

namespace GitVisualiser.Model
{
    public interface IIslands
    {
        void Add(Branch newIsland);
        void Delete(string branchName);
        bool Contains(string branchName);
        void SetBranchDistance(string branchName, double distance);
        void MoveAll();
        System.Windows.Point GetLocation(string branchName);
        void Highlight(string branchName);
        double GetDistance(string branchName);
        List<string> GetAllNames();
    }
}
