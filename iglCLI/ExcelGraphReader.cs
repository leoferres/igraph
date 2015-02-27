using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using log4net;

using Excel;

using IGraph.StatGraph;
using System.Collections;
using IGraph.Utils;

namespace IGraph.GraphReaders
{
 
  class ExcelGraphReader : IIGraphReader
  {

    private static readonly ILog log = LogManager.
      GetLogger(typeof(ExcelGraphReader));

    private Excel.Application xl_app;
    private StatisticalGraph sg;
    private bool problem;

    public ExcelGraphReader()
    {
      try
      {
        xl_app = new Excel.Application();
        xl_app.DisplayAlerts = false;
        log.Debug(
          "Communication with Excel started successfully.");
      } catch
      {
        IGraphConsole.WriteError(
          "Couldn't find the Excel libraries. Exiting...");
        Environment.Exit(IGraphConstants.EXCEL_NOT_AVAILABLE);
      }
    }

    public List<StatisticalGraph> BuildGraphList(string file, bool gif)
    {
      List<StatisticalGraph> sg_collection = new List<StatisticalGraph>();
      Excel.Workbook xl_workbook;

      try
      {
        xl_workbook = this.xl_app.Workbooks.Open(file, 0, false, 5,
          "", "", false, XlPlatform.xlWindows, "", true, false, 0, true, false,
          false);
      } catch
      {
        return null; /*returning the null list; handled in iGraphMain*/
      }

      /** Loop over every worksheet and every ChartObject in the worksheet.
       * The ChartObject object acts as a container for a Chart object. 
       * Properties and methods for the ChartObject object control the 
       * appearance and size of the embedded chart on the worksheet,
       * see: http://msdn.microsoft.com/en-us/library/aa173258(office.11).aspx 
       */
      foreach (Worksheet sheet in xl_workbook.Worksheets)
      {
        foreach (ChartObject curr_chart in
          (ChartObjects)sheet.ChartObjects(Type.Missing))
        {
          sg = new StatisticalGraph();
          problem = false;
          Chart c = curr_chart.Chart; // c is the chart in ChartObject

          if ((int)c.ChartType == 68 || (int)c.ChartType == 71)
          {
              log.Warn("iGraph does not process pie charts... Skipping.");
              continue;
          }

          sg.Prologue = new GraphPrologue(file,
                                           "MS Excel",
                                            curr_chart.Index,
                                            sheet.Index,
                                            (int)c.ChartType,
                                            curr_chart.Height,
                                            curr_chart.Width);

          log.Debug("Graph ID: " + sg.Prologue.GetGraphName());

          sg.PlotArea = GetXLPlotArea(c);

          sg.Textboxes = GetXLTextBoxes(c);

          sg.Series = GetXLSeries(c);

          sg.MainTitle = GetXLChartTitle(c);

          sg.ValueAxis = GetXLValueAxis(c);

          sg.CategoryAxis = GetXLCategoryAxis(c);

          // add to the returned statgraph collection
          if (!problem)
          {
            sg_collection.Add(sg);
            // if flagged, export graph as gif in same argument directory
            if (gif)
            {
              c.Export(sg.Prologue.GetGifName(),"GIF", false);
              log.Info("Saved " + sg.Prologue.GetGifName());
            }
          }
        }
      }
      // Excel has to be closed.
      xl_app.Workbooks.Close();
      xl_app.Quit();

      // the return value if all goes well...
      return sg_collection;
    }

    /***************************************************************************
     * Text boxes are dealt with here
     **************************************************************************/
    private SGTextBoxCollection GetXLTextBoxes(Chart chart)
    {
      SGTextBoxCollection sg_textbox_collection = new SGTextBoxCollection();
      IEnumerator text_box_enumerator = ((TextBoxes)chart.TextBoxes
                                         (Type.Missing)).GetEnumerator();
      int ctr = 0;

      if (chart.HasTitle)
      {
        ChartTitle ct = chart.ChartTitle;
        SGTextBox tibox = new SGTextBox();
        tibox.ID = ctr;
        tibox.Geometry = new SGGeometry(0F, 0F, 0F, 0F);
        tibox.Text = ct.Text;
        sg_textbox_collection.AddTextBox(tibox);
        ctr++;
      }
      
      while (text_box_enumerator.MoveNext())
      {
        SGTextBox sg_textbox = new SGTextBox();
        sg_textbox.ID = ctr;
        TextBox xl_textbox = (TextBox)text_box_enumerator.Current;

        // TextBox without text :)
        if (xl_textbox.Text.Length == 0)
          continue;

        sg_textbox.Text = xl_textbox.Text.Trim().Replace("\n", "");
        sg_textbox.BoxSize = new Size(xl_textbox.Width, xl_textbox.Height);
        SGGeometry txtbox_geom = new SGGeometry(xl_textbox.Left,
          xl_textbox.Top, xl_textbox.Height, xl_textbox.Width);
        sg_textbox.Geometry = txtbox_geom;
        /**
         * Excel constants for horizontal alignment are found at
         * \latexonly
         * \url{http://msdn.microsoft.com/en-us/library/aa221100(office.11).spx}
         * \endlatexonly
         */

        switch (xl_textbox.HorizontalAlignment.ToString())
        {
          case "-4131":
            sg_textbox.Justification = SGTextBox.HorizontalAlignment.LEFT;
            break;
          case "-4108":
            sg_textbox.Justification = SGTextBox.HorizontalAlignment.CENTER;
            break;
          case "-4152":
            sg_textbox.Justification = SGTextBox.HorizontalAlignment.RIGHT;
            break;
          case "-4130":
            sg_textbox.Justification = SGTextBox.HorizontalAlignment.JUSTIFIED;
            break;
        }
        sg_textbox_collection.AddTextBox(sg_textbox);
        log.Debug(sg_textbox.ReturnAsString());
        ctr++;

      }
      log.Debug(ctr + " legal text box(es) found.");
      return sg_textbox_collection;
    }

    /***************************************************************************
     * Series are dealt with here. This is a very error prone method.
     **************************************************************************/
    private SGSeriesCollection GetXLSeries(Chart chart)
    {
      SGSeriesCollection sg_series_collection = new SGSeriesCollection();
      IEnumerator series_enumerator = ((SeriesCollection)
        chart.SeriesCollection(Type.Missing)).GetEnumerator();
      int ctr = 0;

      // iterate through all the boxes in the chart area.
      while (series_enumerator.MoveNext())
      {
        SGSeries sg_series = new SGSeries();
        Series xl_series = (Series)series_enumerator.Current;
        sg_series.ID = ctr;

        ///////////////////////////////////////
        // \internal find if there is a type (line, bar, etc.)
        //////////////////////////////////////
        try
        {
          sg_series.Type = xl_series.Type;
        } catch
        {
          sg_series.Type = -1;
          log.Warn("Couldn't get the type of the series."
            + "\n\tThis must be fixed!");
          continue;
        }

        ///////////////////////////////////////
        // find if there is a name
        //////////////////////////////////////
        if (!String.IsNullOrEmpty(xl_series.Name))
        {
          sg_series.Name = xl_series.Name;
        } else
        {
          log.Warn("Couldn't find the name of the series."
            + "\n\tSetting it to: \"none\".");
          sg_series.Name = "none";
        }

        ///////////////////////////////////////
        // find if there are values
        //////////////////////////////////////

        try
        {
          List<object> vals = new List<object>();
          //vals.AddRange(((Array)xl_series.Values).Cast<object>());

          foreach (object c in (Array)xl_series.Values)
          {
              if (c != null && c.ToString().Length != 0)
              {
                  vals.Add(c);
              }
              else
              {
                  vals.Add("");
              }
          }

          sg_series.Values = vals;
        } catch
        {
          log.Warn("Couldn't find the values for the series."
            + "\n\tSetting it to: \"none\".");
          continue;
        }

        sg_series_collection.Add(sg_series);
        log.Debug(sg_series.ToString() + ".");
        ctr++;
      }

      log.Debug(ctr + " legal series found and added to the list.");
      return sg_series_collection;
    }

    /***************************************************************************
     * Plot area properties
     **************************************************************************/
    private SGPlotArea GetXLPlotArea(Chart chart)
    {
      SGPlotArea pa = new SGPlotArea();
      SGGeometry geom = new SGGeometry(chart.PlotArea.Left,
                                                 chart.PlotArea.Top,
                                                 chart.PlotArea.InsideHeight,
                                                 chart.PlotArea.InsideWidth);
      pa.Geometry = geom;
      log.Debug("Plot area found, properties are " + geom.showAsString() + ".");

      return pa;
    }

    /***************************************************************************
     * Category axis properties
     **************************************************************************/
    private SGCategoryAxis GetXLCategoryAxis(Chart chart)
    {
      SGCategoryAxis sg_category_axis = new SGCategoryAxis();
      Axis xl_category_axis = (Axis)chart.Axes(XlAxisType.xlCategory,
                                               XlAxisGroup.xlPrimary);
      Axis xl_value_axis = (Axis)chart.Axes(XlAxisType.xlValue,
                                            XlAxisGroup.xlPrimary);
      bool isContinuos = false;
      
      ///////////////////////////////////////
      //  Category axis position and width
      //////////////////////////////////////
      try
      {
        sg_category_axis.Width = xl_category_axis.Width;
        sg_category_axis.PosX = xl_category_axis.Left;
      } catch
      {
        sg_category_axis.Width = sg.PlotArea.Geometry.Width;
        sg_category_axis.PosX = sg.PlotArea.Geometry.PosX;
        log.Warn("There is a double axis here. Setting category axis to the"
          + " width and  position of the plot area.");
      }

      ///////////////////////////////////////
      // Category axis title
      //////////////////////////////////////
      if (xl_category_axis.HasTitle)
      {
        sg_category_axis.Title = xl_category_axis.AxisTitle.Text;
      } else
      {
        sg_category_axis.Title = "none";
        log.Warn("Category axis name not found."
          + "\n\tSetting it to: \"none\".");
      }

      ///////////////////////////////////////
      // Category axis crosses at property
      //////////////////////////////////////
      try
      {
        sg_category_axis.Origin = xl_value_axis.CrossesAt;
      } catch
      {
        //throw new Exception(IGraphConstants.kGraphErrorCrossesAt);
      }

      ///////////////////////////////////////
      // Category axis categories
      ////////////////////////////////////// 
      /*
       * Old Method to Get Categories
       */

      try
      {
        List<string> cats = new List<string>();
        foreach (object c in (Array)xl_category_axis.CategoryNames)
        {
          if(c!=null)
          {
            cats.Add(c.ToString());
          }
          else
          {
            sg_category_axis.PrimaryCategoryNulls = true;
            cats.Add("");
          }
        }
        sg_category_axis.Categories = cats;
        sg_category_axis.CategoriesTypeAxis = TypeAxis.DISCRETE;
      } 
      catch 
      {
      //  log.Error("Couldn't get the categories values.");
          isContinuos = true;
      }
      

      /**
        * We are using the Series object to get the categories :)
        * http://msdn.microsoft.com/en-us/library/microsoft.office.interop.excel.series.xvalues%28v=office.14%29.aspx
        */
      if (isContinuos)
      {
          IEnumerator series_enumerator = ((SeriesCollection)
          chart.SeriesCollection(Type.Missing)).GetEnumerator();
          series_enumerator.MoveNext();
          Series xl_series = (Series)series_enumerator.Current;

          List<string> cats = new List<string>();
          foreach (object c in (Array)xl_series.XValues)
          {
              if (c != null && c.ToString().Length != 0)
              {
                  cats.Add(c.ToString());
              }
              else
              {
                  sg_category_axis.PrimaryCategoryNulls = true;
                  cats.Add("");
              }
          }
          sg_category_axis.Categories = cats;
          sg_category_axis.CategoriesTypeAxis = TypeAxis.CONTINUOS;
      }

      return sg_category_axis;
    }

    /***************************************************************************
     * Value axis properties
     **************************************************************************/
    private SGValueAxis GetXLValueAxis(Chart chart)
    {
      SGValueAxis sg_value_axis = new SGValueAxis();
      Axis xl_value_axis = (Axis)chart.Axes(XlAxisType.xlValue,
                                            XlAxisGroup.xlPrimary);
      
      // unit of axis value, we must to divide values by this
      try
      {
          sg_value_axis.ScaleUnit = xl_value_axis.DisplayUnitCustom;
      }
      catch (Exception e)
      {
          sg_value_axis.ScaleUnit = 1;
          log.Warn("Couldn't find the display unit."
            + "\n\tSetting it to: \"1\".");
      }
        

      // value axis title
      if (xl_value_axis.HasTitle)
      {
        sg_value_axis.Title = xl_value_axis.AxisTitle.Text;
      } else
      {
        sg_value_axis.Title = "none";
        log.Warn("Couldn't find the title of the value axis."
          + "\n\tSetting it to: \"none\".");
      }

      sg_value_axis.StartsAt = xl_value_axis.MinimumScale;
      sg_value_axis.EndsAt = xl_value_axis.MaximumScale;
      sg_value_axis.Stepping = xl_value_axis.MajorUnit;

      log.Debug(sg_value_axis.ReturnAsString());
      return sg_value_axis;
    }

    private string GetXLChartTitle(Chart chart)
    {
      if (chart.HasTitle)
      {
        return chart.ChartTitle.Caption;
      } else
      {
        log.Warn("Couldn't find the title of the graph."
          + "\n\tSetting it to: \"none\".");
        return "none";

      }
    }
  }
}
