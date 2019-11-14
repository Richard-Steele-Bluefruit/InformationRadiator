using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace GitVisualiser.Model
{
    public interface IIslandShape
    {
        IUniqueFractalPolygon MainIslandShape { get; }
        PullRequestShapeCollection PullRequestShapes { get; }
        System.Windows.Point CentrePoint { get; }
        double Width { get; }
        double Height { get; }

        void ResizeAndMove(double islandSize, System.Windows.Point islandLocation);
        void Highlight(bool enable);
        List<System.Windows.Shapes.Shape> Drawables();
    }
}