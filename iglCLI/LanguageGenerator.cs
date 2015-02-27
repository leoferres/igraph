using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IGraph.StatGraph;
using IGraph.Utils;

using Commons.Collections;
using NVelocity;
using NVelocity.App;
using NVelocity.Context;
using System.IO;
using log4net;

namespace IGraph.LanguageGeneration
{
  public class LanguageGenerator
  {
    private StatisticalGraph g;
    private bool error;


    private static readonly ILog log = LogManager.
      GetLogger(typeof(LanguageGenerator));

    public LanguageGenerator()
    {
      IGraphConsole.WriteLine("This is Alpha-Lexis, Version 1.0.");
    }

    public bool Generate(StatisticalGraph graph)
    {
      g = graph;
      error = false;
      FrenchGenerator fr_t = new FrenchGenerator();
      EnglishGenerator en_t = new EnglishGenerator();

      #region NVelocity setup
      VelocityEngine velocity = new VelocityEngine();

      ExtendedProperties props = new ExtendedProperties();
      velocity.Init(props);

      //Template template;
      string strTemplate;

      // This nested if can be better...
      if (g.GraphLanguage == null || g.GraphLanguage.Length == 0)
      {
        if (g.Prologue.GetLanguageByFilename() != "French")
        {
          g.GraphLanguage = IGraphConstants.LANG_ENG;
        }
        else
        {
          g.GraphLanguage = IGraphConstants.LANG_FRA;
        }
      }

      if (g.GraphLanguage == IGraphConstants.LANG_ENG)
      {

        //template = velocity.GetTemplate(@"./English.nv");
        byte[] NVtemplate = igl.Properties.Resources.NVenglish;
        System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
        strTemplate = enc.GetString(NVtemplate);
      }
      else
      {

        //template = velocity.GetTemplate(@"./French.nv");
        byte[] NVtemplate = igl.Properties.Resources.NVfrench;
        System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
        strTemplate = enc.GetString(NVtemplate);
      }


      VelocityContext context = new VelocityContext();
      #endregion

      context.Put("graph", g);
      context.Put("french",fr_t);
      context.Put("english", en_t);
      context.Put("date", DateTime.Now);

      // run template matching
      StringWriter writer = new StringWriter();

      try
      {
        //setting Culture
        if (g.GraphLanguage == IGraphConstants.LANG_ENG)
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-CA", false);
        else
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("fr-CA", false);

        //template.Merge(context, writer);
        velocity.Evaluate(context, writer, "NVlocity", strTemplate);
        SaveDescription(writer.GetStringBuilder().ToString());
      } catch (Exception e)
      {
        log.Error("NVelocity error." + e.Message);
        error = true;
      }

      //Restoring culture
      System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US", false);

      return error;
    }

    private void SaveDescription(string desc)
    {
      string filename = g.Prologue.GetGraphOriginalDirectory()
          + g.Prologue.GetGraphName() + ".html";

      TextWriter tw =
        new StreamWriter(filename);

      if (File.Exists(filename))
      {
        log.Warn("Overwritting an HTML description: \"" + filename + "\".");
      }

      tw.Write(desc);
      tw.Close();
    }

  }
}
