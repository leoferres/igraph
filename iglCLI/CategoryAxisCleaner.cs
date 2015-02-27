using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using log4net;

using IGraph.Cleaners;
using IGraph.StatGraph;

namespace IGraph.Cleaners
{
  class CategoryAxisCleaner : ICleaner
  {

    private static readonly ILog log = LogManager.
      GetLogger(typeof(CategoryAxisCleaner));

    StatisticalGraph g;
    SGCategoryAxis ca;

    public void Clean(StatisticalGraph graph)
    {
      log.Debug("Applying category axis cleaning algorithms.");

      g = graph;
      ca = graph.CategoryAxis;

      CleanNullsCategory();

      CleanPrimaryCategories();

      CleanSecondaryCategories();

    }

    private CategoryUnit GuessPrimaryCategoryUnit(KnownCategory kc)
    {
      /**
      * We do lazy matching here as well. We know that months are the most 
       * common horizontal axis categories, so we test for them first, then for
       * and finally for year. */

      Regex month_pattern = new Regex("^[JFMASOND]$");
      Regex year_pattern = new Regex(@"^(19|20)?\d\d$");
      Regex quarter_pattern = new Regex(@"^(I|II|III|IV)$");

      if (month_pattern.IsMatch(kc.category))
      {
        return CategoryUnit.MONTH;

      } else if (quarter_pattern.IsMatch(kc.category))
      {
        return CategoryUnit.QUARTER;

      } else if (year_pattern.IsMatch(kc.category))
      {
        return CategoryUnit.YEAR;

      }
      return CategoryUnit.UNDEF;
    }

    #region PrimaryAxis
    private void CleanPrimaryCategories()
    {
      CategoryUnit tu =
        GuessPrimaryCategoryUnit(g.CategoryAxis.GetKnownCategories()[0]);
      log.Debug(g.CategoryAxis.RawCategoriesToString());
      log.Debug("Probable primary category: " + tu);

      switch (tu)
      {
        case CategoryUnit.MONTH:
          log.Debug("Attempting to clean months...");
          g.CategoryAxis.PrimaryCategoryType = CategoryUnit.MONTH;
          CleanMonths();
          break;
        case CategoryUnit.QUARTER:
          log.Debug("Attempting to clean quarters...");
          g.CategoryAxis.PrimaryCategoryType = CategoryUnit.QUARTER;
          CleanQuarters();
          break;
        case CategoryUnit.YEAR:
          log.Debug("Attempting to clean years...");
          g.CategoryAxis.PrimaryCategoryType = CategoryUnit.YEAR;
          CleanYears();
          break;
        default:
          g.CategoryAxis.PrimaryCategoryType = CategoryUnit.UNDEF;
          g.CategoryAxis.PrimaryCategories = g.CategoryAxis.Categories;
          break;
      }
    }
    #endregion

    #region SecondaryAxis

    private IEnumerable<SGTextBox> RecognizeSecondaryAxis()
    {
      // for now we only recognize years as secondary categories, but we should
      // be able to extend this, that's why I'm returning and IEnumerable

      IEnumerable<SGTextBox> it =
        from s in g.Textboxes
        where (s.Function == SGTextBox.SemanticFunction.YEAR)
        select s;

      return it;
    }

    private void CleanSecondaryCategories()
    {
      SGTextBoxCollection txbxcoll = 
        new SGTextBoxCollection(RecognizeSecondaryAxis().ToList<SGTextBox>());

      if (txbxcoll.NumberOfCategories != 0)
      {
        if (ca.PrimaryCategoryType != CategoryUnit.DIRTY && ca.PrimaryCategoryType != CategoryUnit.YEAR) 
        {
        log.Debug("Secondary category axis found. Attempting cleanup...");
        log.Debug("Attempting to clean years...");
        g.CategoryAxis.SecondaryCategoryType = CategoryUnit.YEAR;
        CleanYearsAsBoxes(txbxcoll);
        } else
        {
          log.Warn("Sorry, I see a secondary axis, but I can't fix it."
            + " Primary categories should be fixed first.");
          ca.SecondaryCategories = null;
        }
      } else
      {
        log.Debug("No secondary axis.");
        ca.SecondaryCategories = null;
      }
    }

    #endregion

    #region Cleaning functions: Months

    private void CleanMonths()
    {
      Regex pattern = new Regex(GetMonthRegEx());
      MatchCollection mc = pattern.Matches(GetMinimalMonthString());
      // at some point we might want to deal with more ambiguous lists,
      // (where mc.Count > 1) but not now...
      if (mc.Count >= 1)
      {
        WriteList(mc[0].Index);
      } else
      {
        ca.PrimaryCategories = ca.Categories;
        log.Error("Categories are either ambiguous or misaligned."
          + " Cannot clean them. Please fix this.");
        log.Debug("Categories remain the same.");
        ca.PrimaryCategoryType = CategoryUnit.DIRTY;
      }
    }

    private string GetMinimalMonthString()
    {
      char[] months = "JFMAMJJASOND".ToCharArray();
      string input_str = "";
      for (int i = 0; i < ca.Categories.Count + 11; i++)
      {
        input_str += months.ElementAt(i % 12);
      }
      return input_str;
    }

    private void WriteList(int first_category)
    {
      List<string> prim_axis = new List<string>();
      prim_axis.Add(GetMonthFullName(first_category));
      for (int i = 0; i < ca.Categories.Count - 1; i++)
      {
        prim_axis.Insert(i + 1,
          GetMonthFullName(((i + first_category) + 1) % 12));
      }
      ca.PrimaryCategories = prim_axis;
      log.Debug("Primary category list has been inferred as: "
        + ca.CategoriesToString(CategoryType.PRIMARY));
    }

    private string GetMonthFullName(int month)
    {
      Hashtable MonthNames = new Hashtable() {
          {0, "January"},  
          {1, "February"}, 
          {2, "March"}, 
          {3, "April"}, 
          {4, "May"},
          {5, "June"}, 
          {6, "July"}, 
          {7, "August"}, 
          {8, "September"}, 
          {9, "October"}, 
          {10,"November"}, 
          {11,"December"}
      };
      return (string)MonthNames[month];
    }

    private string GetMonthRegEx()
    {
      string rex = "";
      List<string> lst_cats = ca.Categories;
      for (int i = 0; i < lst_cats.Count; i++)
      {
        if (String.IsNullOrEmpty((string)lst_cats[i].Trim()))
        {
          rex += ".";
        } else
        {
          rex += lst_cats[i];
        }
      }
      return rex;
    }

    #endregion

    #region Cleaning functions: Quarter

    private void CleanQuarters()
    {
      List<string> qtr_cat_axis = new List<string>();

      if (!ca.HasNullOrEmptyCategories())
      {
        foreach (string s in ca.Categories)
        {
          qtr_cat_axis.Add(StrQuarterToName(s));
        }
      } else
      {
        KnownCategory kc = ca.GetKnownCategories()[0];
        int qtr_first_cat = (QuarterToInteger(kc.category) - kc.position) % 4;
        if (qtr_first_cat < 0)
        {
          qtr_first_cat += 4;
        }
        qtr_cat_axis.Add(IntQuarterToName(qtr_first_cat));
        for (int i = 0; i < ca.Categories.Count - 1; i++)
        {
          qtr_cat_axis.Insert(i + 1,
            IntQuarterToName(((i + qtr_first_cat) + 1) % 4));
        }
      }
/*
      if (g.Series[0].Status == (int)SeriesNulls.FIRST_LAST_NULL)
      {
        qtr_cat_axis.RemoveAt(0);
        qtr_cat_axis.RemoveAt(qtr_cat_axis.Count - 1);
      }*/

      g.CategoryAxis.PrimaryCategories = qtr_cat_axis;

      log.Debug("Primary category list has been inferred as: "
         + g.CategoryAxis.CategoriesToString(CategoryType.PRIMARY));
    }

    private int QuarterToInteger(string q)
    {
      Hashtable quarter_int = new Hashtable() 
      {
      {"I",0},
      {"II",1},
      {"III",2},
      {"IV",3}
      };
      return (int)quarter_int[q];
    }

    private string StrQuarterToName(string q)
    {
      Hashtable quarter_int = new Hashtable() 
      {
      {"I","First"},
      {"II","Second"},
      {"III","Third"},
      {"IV","Fourth"}
      };
      return (string)quarter_int[q];
    }

    private string IntQuarterToName(int i)
    {
      Hashtable quarter_name = new Hashtable() 
      {
      {0,"First"},
      {1,"Second"},
      {2,"Third"},
      {3,"Fourth"}
      };
      return (string)quarter_name[i];
    }

    # endregion

    #region Cleaning functions: Year

    private void CleanYears()
    {

      // I got to refactor this, maybe with a switch statement
      List<string> yr_cat_axis = new List<string>();
      List<KnownCategory> cy_known_cats = ca.GetKnownCategories();
      int cy_tot_num_cats = ca.Categories.Count;

      /**
       * OPTION 1:
       * Lazy matching: if there are less than two known categories, we assume
       * that there is only one year being displayed. I have not seen this 
       * particular example in the graphs I have, but it is defininitely 
       * possible to have one in the future. If there is only one known category
       * recognized as YEAR, then we fill all the categories with the label of
       * this year.
       */
      if (cy_known_cats.Count < 2)
      {
        for (int i = 0; i != cy_tot_num_cats; i++)
        {
          yr_cat_axis.Add(cy_known_cats[0].category);
        }
        ca.PrimaryCategories = yr_cat_axis;
        log.Debug("Primary category list has been inferred as: "
           + ca.CategoriesToString(CategoryType.PRIMARY)
           + " (Using option: 1 (Known Categories < 2))");
        return;
      }

      /**
       * OPTION 2:
       * Tests for year empty categories whose
       * last known category name, parsed to an integer and normalized, equals 
       * the first known category plus the last category position. If so, this
       * means that years are one after the other, irrespective of empty
       * categories.
       */
      int first_cat = NormalizeYear(cy_known_cats.First().category);
      int last_cat = NormalizeYear(cy_known_cats.Last().category);
      int last_cat_pos = cy_known_cats.Last().position;

      if (last_cat == first_cat + last_cat_pos)
      {
        for (int i = 0; i != cy_tot_num_cats; i++)
        {
          yr_cat_axis.Add((first_cat + i).ToString());
        }
        ca.PrimaryCategories = yr_cat_axis;
        log.Debug("Primary category list has been inferred as: "
           + ca.CategoriesToString(CategoryType.PRIMARY)
           + " (Using option: 2 (LastCateg = FirstCateg+LastCateg position))");
        return;
      }

      /**
       * OPTION DEFAULT:
       * This simply says that whenever we discover a category that is not null,
       * empty or none, we should repeat it until we find a new one. This is
       * somewhat dangerous (maybe the empties are not very regular), 
       * but it will have to do for now.
       */
      string tmp_category = "";
      foreach (string s in ca.Categories)
      {
        if (!String.IsNullOrEmpty(s) && s != "none")
        {
          tmp_category = s;
          yr_cat_axis.Add(tmp_category);
        } else
        {
          yr_cat_axis.Add(tmp_category);
        }
      }
      ca.PrimaryCategories = yr_cat_axis;
      log.Debug("Primary category list has been inferred as: "
         + ca.CategoriesToString(CategoryType.PRIMARY)
         + " (Using option: Default (Repeat LasKnownCateg))");
      return;
    }

    private int NormalizeYear(string m)
    {
      /** Weird case in which there is a footnote in the year, see graph
       * c080601b. Makes sense to deal with it as a string because we only have
       * to strip the last character, we will assume footnotes from 1 to 99.
       */
      if (m.Length >= 5)
      {
        m = m.Remove(4);
      }

      int yr = Int16.Parse(m);
      switch (m.Length)
      {
        case 4:
          return yr;
        case 2:
          if (yr > 50)
          {
            return yr + 1900;
          } else
          {
            return yr + 2000;
          }
        case 1:
          return 2000 + yr;
      }
      return -1;
    }

    #endregion


    /**
 * Relatedly, recognizing textboxes first as years, and then as secondary
 * axis categories is not a simple matter. Consider, for example the 
 * categories $c_1$
 */
    private void CleanYearsAsBoxes(SGTextBoxCollection y)
    {
      SGTextBoxCollection _tbcol = y;
      _tbcol.SortHorizontally();

      List<string> sec_cats = new List<string>();
      sec_cats.Add(((SGTextBox)_tbcol.ElementAt(0)).Text);

      int fst_secondary_categ = int.Parse(sec_cats[0]);

      for (int i = 1; i < ca.PrimaryCategories.Count; i++)
      {
        string c_cat = ca.PrimaryCategories[i];
        if (c_cat == "January" || c_cat == "First")
        {
          fst_secondary_categ++;
        }
        //Console.WriteLine(i + "," + _cat_axis_sem.PrimaryCategories[i]
        //  + "," + fst_secondary_categ);
        sec_cats.Insert(i, fst_secondary_categ.ToString());
      }

      ca.SecondaryCategories = sec_cats;
      log.Debug("Secondary category list inferred as: "
        + ca.CategoriesToString(CategoryType.SECONDARY));


    }

    private void CleanNullsCategory()
    {
        /* This method clean a Category when doesn't have a value.
         *  In some cases, when the axis is CONTINUOS, some values doesn't have a point, in that case
         *  the category must be removed.
         *  In other cases, when the all series are dirty (for example, when the first and last value are zeros)
         *  the first and last category must be removed.
         */

        int i;
        int n=ca.Categories.Count;

        if (ca.CategoriesTypeAxis == TypeAxis.CONTINUOS)
        {

            for (i = 0; i < n; i++)
            {
                if (ca.Categories[i].Length == 0)
                {
                    ca.Categories.RemoveAt(i);
                    i--;
                    n--;
                }
            }
        }

        if (g.Series.Dirty)
        {
            for (i = 0; i < g.Series.Count; i++)
            {
                if (g.Series[i].Status == (int)SeriesNulls.FIRST_LAST_NULL)
                {
                    ca.Categories.RemoveAt(0);
                    ca.Categories.RemoveAt(g.CategoryAxis.Categories.Count - 1);
                    break;
                }
                if (g.Series[i].Status == (int)SeriesNulls.FIRST_NULL)
                {
                    ca.Categories.RemoveAt(0);
                    break;
                }
                if (g.Series[i].Status == (int)SeriesNulls.LAST_NULL)
                {
                    ca.Categories.RemoveAt(g.CategoryAxis.Categories.Count - 1);
                    break;
                }
            }
        }

    }
  }



}
