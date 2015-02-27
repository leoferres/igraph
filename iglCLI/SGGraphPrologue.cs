using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace IGraph.StatGraph
{
  public class GraphPrologue
  {

    private string file,origin;
    private int id, sheet, type;
    double gheight, gwidth;

    public GraphPrologue(string fileName,
                         string graphOrigin,
                         int graphID,
                         int sheetName,
                         int graphType,
                         double graphHeight,
                         double graphWidth)
    {
      file = fileName;
      origin = graphOrigin;
      id = graphID;
      type = graphType;
      sheet = sheetName;
      gheight = graphHeight;
      gwidth = graphWidth;
    }

    public string GetOrigin()
    {
      return origin;
    }

    public int GetGraphType()
    {
      return type;
    }

    public double GetGraphWidth()
    {
      return gwidth;
    }

    public double GetGraphHeight()
    {
      return gheight;
    }

    public string GetGraphName()
    {
      return Path.GetFileName(file).Split('.')[0] + "_" + sheet + "_" + id;
    }

    public string GetLanguageByFilename()
    {
      if (GetOriginalExcelFile().StartsWith("c"))
      {
        return "English";
      }

      if (GetOriginalExcelFile().StartsWith("g"))
      {
        return "French";
      }

      return "None";
    }

    public string GetAbsolutePathFile()
    {
      return Path.GetFullPath(file);
    }

    public string GetOriginalExcelFile()
    {
      return Path.GetFileName(file);
    }

    public string GetOriginalExcelFilenNoExtension()
    {
      return Path.GetFileName(file).Split('.')[0];
    }

    public string GetGraphOriginalDirectory()
    {
      return Path.GetDirectoryName(GetAbsolutePathFile()) + @"\";
    }

    public string GetGifName()
    {
      return this.GetGraphOriginalDirectory() + this.GetGraphName() + ".gif";
    }

    public string PrologueToString()
    {
      string s = String.Format("Graph name is {0}, created with {1}, ",
        this.GetGraphName(), this.origin);
      s += String.Format("embedded in sheet {0}", this.sheet);
      return s;
    }

  }
}
