using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IGraph.StatGraph
{

  public class SGSeriesCollection : List<SGSeries> {

    public bool Dirty { get; set; }

    public bool existsSeriesType(int type) {
      foreach (SGSeries ser in this) {
        if (ser.Type == type) {
          return true;
        }
      }
      return false;
    }
  }
}
