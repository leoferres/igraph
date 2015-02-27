using System;
using System.Collections.Generic;

using log4net;

using IGraph.StatGraph;

namespace IGraph.Cleaners
{
  
  public enum SeriesNulls
  {
    NO_NULL,
    SOME_NULL,
    ALL_NULL,
    FIRST_NULL,
    LAST_NULL,
    FIRST_LAST_NULL
  }

  class SeriesCleaner : ICleaner
  {

    private static readonly ILog log =
      LogManager.GetLogger(typeof(SeriesCleaner));

    public void Clean(StatisticalGraph graph)
    {
      log.Debug("Applying series cleaning algorithms.");
      int num_ser = graph.Series.Count;
      bool problem = true;
      
      for (int i = 0; i < num_ser; i++)
      {

        SGSeries curr = graph.Series[i];
        
        switch (GetSeriesNullState(curr))
        {
          case SeriesNulls.ALL_NULL:
            curr.Status = (int)SeriesNulls.ALL_NULL;
            graph.Series.RemoveAt(i);
            i--;
            num_ser--;
            log.Error("Empty series. Why?");
            break;
          
          case SeriesNulls.FIRST_LAST_NULL:
            curr.Status = (int)SeriesNulls.FIRST_LAST_NULL;
            graph.Series.Dirty = true;
            curr.Values.RemoveAt(0);
            curr.Values.RemoveAt(curr.Values.Count-1);
            log.Error("Empty first and last elements. Why?");
            break;
            case SeriesNulls.FIRST_NULL:
            curr.Status = (int)SeriesNulls.FIRST_NULL;
            graph.Series.Dirty = true;
            curr.Values.RemoveAt(0);
            log.Error("Empty first element. Why?");
            break;
            case SeriesNulls.LAST_NULL:
            curr.Status = (int)SeriesNulls.LAST_NULL;
            graph.Series.Dirty = true;
            curr.Values.RemoveAt(curr.Values.Count - 1);
            log.Error("Empty last element. Why?");
            break;
          case SeriesNulls.SOME_NULL:
            curr.Status = (int)SeriesNulls.SOME_NULL;
            CleanSomeNullValues(curr);
            break;
          default:
            curr.Status = (int)SeriesNulls.NO_NULL;
            problem = false;
            break;
        }
      }

      //Scale Values ;)
      ScaleValues(graph);
      
      //Round to 2 decimals
      RoundDecimals(graph);

      if (!problem)
      {
        log.Debug("All series seem OK. Bravo!");
      }
    }

    private SeriesNulls GetSeriesNullState(SGSeries s)
    {
      bool first = false;
      bool last = false;
      bool _hasnull = false;
      int num_nulls = 0;
      int vals_length = s.Values.Count;


      for (int i = 0; i < vals_length; i++)
      {
          _hasnull = false;

          //When value is null, setted by string
          if (s.Values[i].GetType().ToString() == "System.String") 
          {
              if (s.Values[i] == null ||
                  ((String)s.Values[i]).Length == 0 ||
                  (String)s.Values[i] == "none")
              {
                  _hasnull = true;
              }

          }
          //Double values
          else 
          {
              if (s.Values[i] == null ||
                  (Double)s.Values[i] == 0 ||
                  s.Values[i].ToString() == "none" )
              {
                  _hasnull = true;
              }
          }

          
          if (_hasnull)
          {
            if (i == 0)
              first = true;

            if (i == vals_length - 1)
              last = true;
            
            num_nulls++;
           }

      }

      if (num_nulls == vals_length)
      {
          return SeriesNulls.ALL_NULL;
      }
      else if (first && last)
      {
          return SeriesNulls.FIRST_LAST_NULL; // see graph c080229c
      }
      else if (first)
          return SeriesNulls.FIRST_NULL;
      else if (last)
          return SeriesNulls.LAST_NULL;
      else if (num_nulls > 0)
      {
          return SeriesNulls.SOME_NULL;
      }
      return SeriesNulls.NO_NULL;
    }

    private void ScaleValues(StatisticalGraph graph)
    {
      int num_ser = graph.Series.Count;

      // Scaling data by unit of visualization

      graph.ValueAxis.EndsAt = Convert.ToDouble(graph.ValueAxis.EndsAt) / graph.ValueAxis.ScaleUnit;
      graph.ValueAxis.StartsAt = Convert.ToDouble(graph.ValueAxis.StartsAt) / graph.ValueAxis.ScaleUnit;
      graph.ValueAxis.Stepping = Convert.ToDouble(graph.ValueAxis.Stepping) / graph.ValueAxis.ScaleUnit;

      for (int i = 0; i < num_ser; i++)
      {
        SGSeries curr = graph.Series[i];
        if (graph.ValueAxis.ScaleUnit > 0)
        {
          for (int j = 0; j < curr.Values.Count; j++)
          {
            curr.Values[j] = Convert.ToDouble(curr.Values[j]) / graph.ValueAxis.ScaleUnit;
          }
        }
      }


    }

    private void RoundDecimals(StatisticalGraph graph)
    {
        int num_ser = graph.Series.Count;
        int decimals2show = 2;

        for (int i = 0; i < num_ser; i++)
        {
            SGSeries curr = graph.Series[i];
            if (graph.ValueAxis.ScaleUnit > 0)
            {
                for (int j = 0; j < curr.Values.Count; j++)
                {
                    curr.Values[j] = Math.Round((Double) curr.Values[j], decimals2show);
                }
            }
        }


    }

    private void CleanSomeNullValues(SGSeries s)
    {
          int n = s.Values.Count;
          for (int i = 0; i < n; i++) {
              if (s.Values[i].GetType().ToString() == "System.String")
              {
                  if (s.Values[i] == null ||
                      ((String)s.Values[i]).Length == 0 ||
                      (String)s.Values[i] == "none")
                  {
                      s.Values.RemoveAt(i);
                      i--;
                      n--;
                  }
              }

          }

      }
  }
}
