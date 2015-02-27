namespace IGraph
{
  using System;
  using System.Collections.Generic;
  using NDesk.Options;
  using IGraph.Utils;

  public class IGraphOptions
  {

    OptionSet opset;
    String[] args;
    List<string> param;

    // these are the default options
    bool _gif_file = false;
    bool _xml_file = false;
    bool _owl_file = false;
    bool _show_help = false;
    int log_level = 2;
    string _csv_file = null;
    string _input_file = null;

    public IGraphOptions(string[] a)
    {
      args = a;
      opset = new OptionSet();

      // iGraph-Lite Options
      opset.Add("g|gif", "Export graph to gif file",
        v => _gif_file = v != null);

      opset.Add("?|h|help", "Show this message and exit",
        v => _show_help = v != null);

      opset.Add("c|csv=", "Load a csv file",
        v => _csv_file = v);

      opset.Add("f|file=", "Load a Excel file",
        v => _input_file = v);

      opset.Add("l|log-level=", "How much iGraph will say (0 to 6)",
        (int v) => log_level = v);

      opset.Add("x|xml", "Export graph to XML file",
        v => _xml_file = v != null);

      opset.Add("o|owl", "Export graph to OWL 2.0 file",
        v => _owl_file = v != null);

      param = opset.Parse(args);
    }

    public bool Validate()
    {
      if (param.Count == 0)
      {
        return true;
      }
      else if (param.Count == 1 && !param[0].StartsWith("-"))
      {
        return true;
      }
      return false;
    }

    public string DirectoryName
    {
      get
      {
        if (param.Count == 1)
        {
          return param[0];
        }
        return ".";
      }
    }

    public string CsvFilename
    {
      get
      {
        return _csv_file;
      }
    }

    public string InputFilename
    {
      get
      {
        return _input_file;
      }
    }

    public int LogLevel
    {
      get
      {
        return log_level;
      }
    }
    public bool writeGIF
    {
      get { return _gif_file; }
    }

    public bool writeXML
    {
      get { return _xml_file; }
    }

    public bool writeOWL
    {
      get { return _owl_file; }
    }

    public bool isHelp
    {
      get { return _show_help; }
    }

    public bool isCsv
    {
      get
      {
        return (_csv_file == null ? false : true);
      }
    }

    public bool isFile
    {
      get
      {
        return (_input_file == null ? false : true);
      }
    }

    public void Help()
    {
      IGraphConsole.WriteLine("\nUsage: igl [OPTIONS] Directory");
      IGraphConsole.WriteLine("\nOptions:");
      opset.WriteOptionDescriptions(Console.Out);
      IGraphConsole.WriteLine("\nOptions that take values may use an equal"
                              + " sign, a colon");
      IGraphConsole.WriteLine("or a space to separate the option from its"
                              + " value.");
      IGraphConsole.WriteLine("\nIf no directory is specified, the current"
                        + " directory is used.");
    }
  }
}
