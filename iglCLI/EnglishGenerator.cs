using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IGraph.LanguageGeneration
{
  public class EnglishGenerator
  {
    Dictionary<string, string> dict = new Dictionary<string, string>()
    {
      {"point","points"},
    };

    public string PluralizeWord(string wd, Double num)
    {
      if (num > 1)
      {
        return dict[wd];
      }
      return wd;
    }

    public string EnglishWord(string w)
    {
      string r;
      if (w == "MONTH")
        r = "Month";
      else if (w == "YEAR")
        r = "Year";
      else if (w == "QUARTER")
        r = "Quarter";
      else
        r = w;

      return r;
    }
  }
}
