using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace IGraph.LanguageGeneration
{
  public class FrenchGenerator
  {
    Dictionary<string, string> dict = new Dictionary<string, string>()
    {
      {"point","points"},
    };

    public string FrenchifyNumber(string n)
    {
      if (n.Contains("."))
      {
        return n.Replace(".",",");
      }
      return n;
    }
    public string FrenchifyWord(string w)
    {
      string r;
      
      if (w == "MONTH")
        r = "Mois";
      else if (w == "YEAR")
        r = "Année";
      else if (w == "QUARTER")
        r = "Quart";
      else
        r = w;

      return FrenchifyQuarter(FrenchifyMonth(r));
    }
    public string FrenchifyQuarter(string m)
    {
        string r;
        if (m == "First")
            r = "Premier";
        else if (m == "Second")
            r = "Deuxième";
        else if (m == "Third")
            r = "Troisième";
        else if (m == "Fourth")
            r = "Quatrième";
        else
            r = m;

        return r;

    }

    public string FrenchifyMonth(string m)
    {
      string r;
      if (m == "January")
        r = "janvier";
      else if (m == "February")
        r = "février";
      else if (m == "March")
        r = "mars";
      else if (m == "April")
        r = "avril";
      else if (m == "May")
        r = "mai";
      else if (m == "June")
        r = "juin";
      else if (m == "July")
        r = "juillet";
      else if (m == "August")
        r = "août";
      else if (m == "September")
        r = "septembre";
      else if (m == "October")
        r = "octobre";
      else if (m == "November")
        r = "novembre";
      else if (m == "December")
        r = "décembre";
      else
        r = m;
      return r;                       
    }

    public string PluralizeWord(string wd, Double num)
    {
      if(num>1)
      {
        return dict[wd];
      }
      return wd;
    }

    public string CapitalizeWord(string w)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(w);
    }

  }
}
