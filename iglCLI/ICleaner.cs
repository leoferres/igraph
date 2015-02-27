using System;
using IGraph.StatGraph;

namespace IGraph.Cleaners
{
  public interface ICleaner
  {
    void Clean(StatisticalGraph graph);
  }
}
