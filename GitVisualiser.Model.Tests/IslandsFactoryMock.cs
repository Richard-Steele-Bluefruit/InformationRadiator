using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GitVisualiser.Model.Tests
{
    public class IslandsFactoryMock : IslandsFactory
    {
        public IIslandCollisionDetector _collisionDetector;

        public override IIslandCollisionDetector CreateIslandCollisionDetector(
            GitVisualiser.Model.ViewControlData viewControlData)
        {
            var collisionDetector = _collisionDetector;
            _collisionDetector = null;
            return collisionDetector;
        }
    }
}
