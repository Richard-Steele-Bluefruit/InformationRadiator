using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GitVisualiser.Model.Tests
{
    public class IslandShapeFactoryMock : IslandShapeFactory
    {
        public IUniqueFractalPolygon _uniqueFractalPolygon;

        public override IUniqueFractalPolygon CreateUniqueFractalPolygon()
        {
            var uniqueFractalPolygon = _uniqueFractalPolygon;
            _uniqueFractalPolygon = null;
            return uniqueFractalPolygon;
        }
    }
}
