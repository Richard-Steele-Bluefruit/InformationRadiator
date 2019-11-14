using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GitVisualiser.Model
{
    public class IslandsFactory
    {
        private static IslandsFactory _instance;

        public static IslandsFactory Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new IslandsFactory();
                return _instance;
            }
            internal set
            {
                _instance = value;
            }
        }

        public virtual IIslandCollisionDetector CreateIslandCollisionDetector(
            GitVisualiser.Model.ViewControlData viewControlData)
        {
            return new IslandCollisionDetector(viewControlData);
        }
    }
}
