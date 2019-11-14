using System;
using System.Collections.Generic;
using System.Linq;

namespace GitVisualiser.Model
{
    public interface IIslandCollisionDetector
    {
        System.Windows.Point GetNearestFreeLocation(IIslandShape possibleCollidingIslandShape, 
            IIslandShape releaseArchipelagoIslandShape);
    }
}
