namespace IGraph
{
  using System;
  using System.Reflection;
  using System.IO;

  using log4net;
  using log4net.Config;
  using log4net.Appender;
  using log4net.Layout;
  using log4net.Core;

  using IGraph.Utils;

  public class Program
  {
    private static readonly ILog log = LogManager.
      GetLogger(typeof(Program));

    public static void Main(string[] args)
    {
      System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US", false);

      WriteBanner();

      IGraphOptions opts = new IGraphOptions(args);

      #region Logger
      /// Logging setup. We do not use a configuration file.
      ColoredConsoleAppender appender = new ColoredConsoleAppender();
      appender.Layout = new
        PatternLayout("[%5level]\t%m\n");

      AddColorMapping(appender,
        Level.Error, ColoredConsoleAppender.Colors.Red
        | ColoredConsoleAppender.Colors.HighIntensity);
      AddColorMapping(appender,
        Level.Warn, ColoredConsoleAppender.Colors.Yellow
        | ColoredConsoleAppender.Colors.HighIntensity);

      switch (opts.LogLevel)
      {
        case 0:
          appender.Threshold = Level.Off;
          break;
        case 1:
          appender.Threshold = Level.Fatal;
          break;
        case 2:
          appender.Threshold = Level.Error;
          break;
        case 3:
          appender.Threshold = Level.Warn;
          break;
        case 4:
          appender.Threshold = Level.Info;
          break;
        case 5:
          appender.Threshold = Level.Debug;
          break;
        case 6:
          appender.Threshold = Level.All;
          break;
        default:
          appender.Threshold = Level.Warn;
          break;
      }
      
      appender.ActivateOptions();
      BasicConfigurator.Configure(appender);
      IGraphConsole.WriteLine("Log level set to: " + appender.Threshold + " (" + opts.LogLevel + ")");
      #endregion

      if (opts.isHelp)
      {
        opts.Help();
      }
      else if (!opts.Validate())
      {
        IGraphConsole.WriteError("There are unrecognized options."
          + "\n\tTry igl -h or igl --help for more information.");
        opts.Help();
        Environment.Exit(IGraphConstants.INVALID_OPTIONS);
      }
      else if (opts.isFile && !File.Exists(opts.InputFilename))
      {
        IGraphConsole.WriteError("The specified file was not found."
          + "\n\tYou said: " + opts.InputFilename);
        Environment.Exit(IGraphConstants.EXCEL_NOT_FOUND);
      }
      else if (opts.isCsv && !File.Exists(opts.CsvFilename))
      {
        IGraphConsole.WriteError("CSV file not found.");
        Environment.Exit(IGraphConstants.CSV_NOT_FOUND);
      }
      else if (!Directory.Exists(opts.DirectoryName))
      {
        IGraphConsole.WriteError("Directory does not exist."
          + "\n\tYou said: " + opts.DirectoryName
          + "\n\tRemember to add quotes if directory contains spaces.");
        Environment.Exit(IGraphConstants.DIR_NOT_FOUND);
      }
      else
      {
        IGraphLite.Execute(opts);
      }
    }

    private static void AddColorMapping(
      ColoredConsoleAppender appender,
      Level level,
      ColoredConsoleAppender.Colors color)
    {
      ColoredConsoleAppender.LevelColors mapping =
        new ColoredConsoleAppender.LevelColors();
      mapping.Level = level;
      mapping.ForeColor = color;
      appender.AddMapping(mapping);
    }

    private static void WriteBanner()
    {
      Assembly executingAssembly = Assembly.GetExecutingAssembly();
      System.Version version = executingAssembly.GetName().Version;

      IGraphConsole.WriteLine(
        String.Format("This is iGraph-Lite, Version {0} ({1})",
        version.ToString(4),
        System.IO.File.GetLastWriteTime(executingAssembly.Location)));
    }
  }
}
