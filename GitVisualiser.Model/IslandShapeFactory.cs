using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GitVisualiser.Model
{
    public class IslandShapeFactory
    {
        private static IslandShapeFactory _instance;

        public static IslandShapeFactory Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new IslandShapeFactory();
                return _instance;
            }
            internal set
            {
                _instance = value;
            }
        }

        public virtual IUniqueFractalPolygon CreateUniqueFractalPolygon()
        {
            return new UniqueFractalPolygon();
        }
    }
}
