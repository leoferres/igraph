using System;
using System.Collections.Generic;
using IGraph.StatGraph;

using log4net;

namespace IGraph.Cleaners
{
  public static class CleaningManager
  {
    private static readonly ILog log = LogManager.
      GetLogger(typeof(CleaningManager));

    public static void CleanGraph(StatisticalGraph sg)
    {
      log.Debug("Attempting cleaning of graph: " 
        + sg.Prologue.GetGraphName().ToUpper());      
      new TextboxCleaner().Clean(sg);
      new SeriesCleaner().Clean(sg);
      new CategoryAxisCleaner().Clean(sg);
    }
  }
}
