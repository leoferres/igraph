using System;
using System.Collections.Generic;
using System.IO;

using log4net;

using IGraph.Utils;
using IGraph.StatGraph;
using IGraph.GraphReaders;
using IGraph.Cleaners;
using IGraph.LanguageGeneration;
using IGraph.GraphWriters;

namespace IGraph
{
  public static class IGraphLite
  {

    private static readonly ILog log = LogManager.
      GetLogger(typeof(IGraphLite));

    private static ExcelGraphReader xl_reader;
    private static LanguageGenerator nlg;
    private static IGraphOptions ops;

    public static void Execute(IGraphOptions _ops)
    {
      // Atributes :)
      xl_reader = new ExcelGraphReader();
      nlg = new LanguageGenerator();

      ops = _ops;

      string dir = ops.DirectoryName;
      string csvFilename = ops.CsvFilename;
      string inputFilename = ops.InputFilename;

      if (ops.isFile)
      {
        processFile(inputFilename, null, null);
      }
      else if (ops.isCsv)
      {
        CsvFile[] xl_csvfiles = CsvParse.parse(csvFilename);
        string path_csvfile =
          Path.GetDirectoryName(Path.GetFullPath(csvFilename));

        if (xl_csvfiles.Length == 0)
        {
          IGraphConsole.WriteError("CSV file found, but no Excel files in it."
            + "\n\tYou said: " + csvFilename);
          Environment.Exit(IGraphConstants.NO_GRAPH_FILES_IN_CSV);
        }

        // All's well so far, process each graph in each file in the dir
        log.Info(xl_csvfiles.Length + " Excel file(s) in the csv file.");
        foreach (CsvFile csvfile in xl_csvfiles)
        {

          string file = csvfile.Filename;
          string f = Path.Combine(path_csvfile, file); // full path of "file"

          if (File.Exists(f))
          {
            processFile(f, csvfile.Language, csvfile.Title);
          }
          else
          {
            log.Warn("Excel file not found."
                      + "\n\tThe Excel file must be in: " + f);
          }
        }
      }
      else
      {
        // Do Excel files exist in the directory?
        string[] xl_files = Directory.GetFiles(dir, "*.xls");
        
        if (xl_files.Length == 0)
        {

          IGraphConsole.WriteError("Directory found, but no Excel files in it."
            + "\n\tYou said: " + (dir == "." ? ". (current directory)" : dir));
          Environment.Exit(IGraphConstants.NO_GRAPH_FILES_IN_DIR);
        }

        log.Info(xl_files.Length + " Excel file(s) in the directory.");

        // All's well so far, process each graph in each file in the dir
        foreach (string file in xl_files)
        {
          processFile(file, null, null);
        }
      }

      log.Info("Successfully released the Excel handle. This is awesome.");
      IGraphConsole.WriteLine("Done.");
      Environment.Exit(IGraphConstants.EXIT_SUCCESS);
    }

    private static void processFile(string file, string lang, string title)
    {
      string f = Path.GetFullPath(file); // the full path of "file"

      IGraphConsole.WriteLine("Processing file: " + f);

      List<StatisticalGraph> sgList =
        xl_reader.BuildGraphList(f, ops.writeGIF);

      // was there at least one graph in  that Excel file?
      if (sgList == null || sgList.Count == 0 )
      {
        log.Info("No valid graph to process here.");
      }
      else
      {
        foreach (StatisticalGraph sg in sgList)
        {
          if (ops.isCsv)
          {
            if (title != null && title.Length > 0)
              sg.MainTitle = title;
            if (lang != null && lang.Length > 0)
              sg.GraphLanguage = lang;
          }

          // Graph cleaning goes first.
          CleaningManager.CleanGraph(sg);

          // Writers go second
          if (ops.writeXML)
          {
            log.Debug("Writing graph as XML file.");
            XMLGraphWriter.write(sg);
          }

          // NLG goes last.
          if (nlg.Generate(sg))
          {
            log.Error("This file has wounded me deeply. Please fix it.");
          }
        }
      }
    }
  }
}