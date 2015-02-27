using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using log4net;

using IGraph.StatGraph;
using System.Collections;

namespace IGraph.Cleaners
{
  class TextboxCleaner : ICleaner
  {
    private static readonly ILog log =
      LogManager.GetLogger(typeof(TextboxCleaner));

    private StatisticalGraph g;
    private SGTextBoxCollection txtbox_collection;

    public void Clean(StatisticalGraph graph)
    {
      log.Debug("Applying textbox cleaning algorithms.");

      g = graph;
      txtbox_collection = graph.Textboxes;
      removeEmpty(); //remove empty textboxes
      txtbox_collection.SortVertically();

      int ctr = 0;
      foreach (SGTextBox txtbx in txtbox_collection)
      {
        RecognizeFunction(txtbx, ctr);
        ctr++;
      }
    }

    private void RecognizeFunction(SGTextBox tb, int idx)
    {
      string tb_txt = cleanString(tb.Text);

      // If graph have a title (MainTitle != "none" and != "", don't run this code
      if ( (g.MainTitle=="none" || g.MainTitle=="") && isAbovePlotArea(tb) && idx == 0 && !isRightOfPlotArea(tb))
      {
        tb.Function = SGTextBox.SemanticFunction.MAIN_TITLE_PRIMARY;
        log.Debug("Probable main title recognized: " + chopString(tb_txt));
        g.MainTitle = tb.Text;
        return;
      }

      if (isAbovePlotArea(tb)
        && idx != 0 && !isRightOfPlotArea(tb)
        && !g.Series.existsSeriesType(57)
        && g.ValueAxis.Title == "none")
      {
        tb.Function = SGTextBox.SemanticFunction.Y_AXIS_TITLE_PRIMARY;
        log.Debug("Probable value axis title recognized: " 
          + chopString(tb_txt));
        g.ValueAxis.Title = tb.Text;
        return;
      }

      // is it a year?
      Regex year_pattern = new Regex(@"^(19|20)?\d\d$");
      if (isBelowXAxis(tb)
        && year_pattern.IsMatch(tb_txt))
      {
        log.Debug("Probable year recognized: " + tb.Text);
        tb.Function = SGTextBox.SemanticFunction.YEAR;
        return;
      }

      // is it a footnote?
      Regex footnote_pattern = new Regex(@"\d\.");
      if (isBelowXAxis(tb) && footnote_pattern.IsMatch(tb_txt))
      {
        tb.Function = SGTextBox.SemanticFunction.FOOTNOTE;
        log.Info("Probable footnote recognized: " + chopString(tb.Text));
        return;
      }

      // is it a note?
      if (isBelowXAxis(tb) && tb_txt.StartsWith("Note:"))
      {
        tb.Function = SGTextBox.SemanticFunction.INFORMATION_NOTE;
        log.Debug("Probable note recognized: " + chopString(tb.Text));
        return;
      }

      // is it the value axis title of a horizontal bar graph?
      if (isBelowXAxis(tb) && g.Prologue.GetGraphType() == 57)
      {
        tb.Function = SGTextBox.SemanticFunction.Y_AXIS_TITLE_PRIMARY;
        log.Debug("Probable value axis title recognized: "
          + chopString(tb.Text));
        g.ValueAxis.Title = tb.Text;
        return;
      }

      // @FIX why this?
      tb.Function = SGTextBox.SemanticFunction.ORPHAN_BOX;
      log.Warn("Orphan textbox: " + chopString(tb.Text));
    }

    private string cleanString(string txt)
    {
      return Regex.Replace(txt, @"\s+", " ").Trim();
    }

    private string chopString(string s)
    {
      if (s.Length > 50)
      {
        return s.Substring(0, 50).Trim() + "...";
      }
      return s;
    }

    private bool isAbovePlotArea(SGTextBox tb)
    {
      return tb.Geometry.PosY <= g.PlotArea.Geometry.PosY;
    }

    private bool isBelowXAxis(SGTextBox tb)
    {
      return tb.Geometry.PosY > (g.PlotArea.Geometry.Height
        + g.PlotArea.Geometry.PosY);
    }

    private bool isRightOfPlotArea(SGTextBox tb)
    {
      return tb.Geometry.PosX > g.PlotArea.Geometry.Width / 2;
    }

    private void removeEmpty()
    {
        ArrayList removals = new ArrayList();
        foreach (SGTextBox txtbx in txtbox_collection)
        {
            string tb_txt = cleanString(txtbx.Text);
            if (tb_txt.Length == 0)
                removals.Add(txtbx);
        }

        foreach(Object o in removals) {
            txtbox_collection.RemoveTextBox((SGTextBox)o);
        }
    }

  }
}
