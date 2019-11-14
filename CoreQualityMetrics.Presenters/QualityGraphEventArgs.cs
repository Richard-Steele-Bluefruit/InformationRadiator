using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreQualityMetrics.Presenters
{
    public class QualityGraphEventArgs : EventArgs
    {
        public QualityGraphEventArgs(List<QualityGraph> graphs)
        {
            Graphs = graphs;
        }

        public List<QualityGraph> Graphs { get; private set; }
    }

    public class QualityGraph
    {
        public QualityGraph(string project, List<QualityGraphPoint> dataPoints)
        {
            ProjectName = project;
            Points = dataPoints;
        }

        public string ProjectName { get; private set; }
        public List<QualityGraphPoint> Points { get; private set; }
    }

    public class QualityGraphPoint
    {
        public QualityGraphPoint(DateTime xPoint, int yPoint)
        {
            x = xPoint;
            y = yPoint;
        }

        public DateTime x { get; private set; }
        public int y { get; private set; }
    }
}
