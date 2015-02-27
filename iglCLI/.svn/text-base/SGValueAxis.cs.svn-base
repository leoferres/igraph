using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IGraph.StatGraph
{
  public class SGValueAxis
  {
    public string Title
    {
      get;
      set;
    }
    public double EndsAt
    {
      get;
      set;
    }

    public double StartsAt
    {
      get;
      set;
    }

    public double Stepping
    {
      get;
      set;
    }
    public double ScaleUnit { get; set; }
    public string ReturnAsString()
    {
      return String.Format("Legal value axis found: "
        + "title is {0}, min={1}, max={2}, step={3}, visualitationunit={4}.",
        this.Title, this.StartsAt, this.EndsAt, this.Stepping, this.ScaleUnit);
    }

  }
}
