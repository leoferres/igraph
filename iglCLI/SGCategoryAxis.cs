using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace IGraph.StatGraph
{
  public struct Category
  {
    public string text;
    public int cat_pos;
    public double axis_pos; /*where it appears visually in the axis*/

    public Category(string t, int cp, double ap)
    {
      text = t;
      cat_pos = cp;
      axis_pos = ap;
    }
  }

  public struct KnownCategory
  {
    public string category;
    public int position;

    public KnownCategory(string cat, int pos)
    {
      category = cat;
      position = pos;
    }
  }

  public enum CategoryType 
  { 
    PRIMARY, 
    SECONDARY, 
    UNDEF 
  }

  public enum CategoryUnit
  {
    UNDEF,
    MONTH,
    QUARTER,
    YEAR,
    CANADIAN_PROVINCE,
    MISC,
    DIRTY
  }
  public enum TypeAxis
  {
      CONTINUOS,
      DISCRETE
  }

  public class SGCategoryAxis
  {

    // Class data
    public string Title { get; set; }
    public double Origin { get; set; }
    public double Width { get; set; }
    public double PosX { get; set; }

    public List<string> Categories { get; set; }
    public List<string> PrimaryCategories { get; set; }
    public List<string> SecondaryCategories { get; set; }
    public CategoryUnit PrimaryCategoryType { get; set; }
    public CategoryUnit SecondaryCategoryType { get; set; }
    public TypeAxis CategoriesTypeAxis { get; set; }
    public bool PrimaryCategoryNulls { get; set; }

    public string getPrimaryCategoryType()
    {
      return PrimaryCategoryType.ToString();
    }

    public string getSecondaryCategoryType()
    {
      return SecondaryCategoryType.ToString();
    }

    public string getPrimaryCategoryAt(int idx)
    {
      return PrimaryCategories.ElementAt(idx);
    }

    public string getSecondaryCategoryAt(int idx)
    {
      if (SecondaryCategories != null)
      {
        return SecondaryCategories.ElementAt(idx);
      }
      else
      {
        return "";
      }
    }

    public string getLastPrimaryCategory()
    {
      return PrimaryCategories.Last();
    }

    public string getFirstPrimaryCategory()
    {
      return PrimaryCategories.First();
    }

    public string getLastSecondaryCategory()
    {
      return SecondaryCategories.Last();
    }

    public string getFirstSecondaryCategory()
    {
      return SecondaryCategories.First();
    }

    // Class methods
    public override string ToString()
    {
      return String.Format("Legal category axis found: "
        + "title is {0}, origin={1}, posX={2}, Width={3} ({4} categories).",
        this.Title, this.Origin, this.PosX, this.Width, this.Categories.Count);
    }

    public string RawCategoriesToString()
    {
      string cats = "";
      foreach (string s in this.Categories)
      {
        cats += "[" + s.Trim() + "] ";
      }
      return cats;
    }

    public List<KnownCategory> GetKnownCategories()
    {
      List<KnownCategory> kc = new List<KnownCategory>();
      
      int ctr = 0;
      
      foreach (string s in this.Categories)
      {
        if (!String.IsNullOrEmpty(s.Trim()))
        {
          kc.Add(new KnownCategory(s.Trim(), ctr));
        }
        ctr++;
      }

      if (kc.Count == 0)
      {
          kc.Add(new KnownCategory("undefined", 0));
      }
      return kc;
    }

    public string CategoriesToString(CategoryType c)
    {
      string cats = "";
      if (c == CategoryType.PRIMARY)
      {
        for (int i = 0; i < this.PrimaryCategories.Count; i++)
        {
          cats += "[" + this.PrimaryCategories[i] + "] ";
        }
      } else
      {
        for (int i = 0; i < this.SecondaryCategories.Count; i++)
        {
          cats += "[" + this.PrimaryCategories[i] + ","
            + this.SecondaryCategories[i] + "] ";
        }
      }
      return cats;
    }

    public bool HasNullOrEmptyCategories()
    {
      foreach (string s in this.Categories)
      {
        if (String.IsNullOrEmpty(s.Trim()))
          return true;
      }
      return false;
    }

    public Category getClosestPrimaryAxisCategory(double pos, double offset)
    {
      double space_between_ticks = this.Width
        / this.PrimaryCategories.Count;
      for (int i = 0; i < this.PrimaryCategories.Count; i++)
      {
        double prev = offset + space_between_ticks * (i - 1);
        double curr = offset + space_between_ticks * i;
        /// If it is the first position, that is, if the box starts before the
        /// x-axis starts.
        if (pos < curr && i == 0)
        {
          return new Category(this.PrimaryCategories.First().ToString(), 0, 0d);
        }
        /// if position of a category is less than the current one being 
        /// analyzed then
        if (pos < curr)
        {
          // if the current category is closer than the previous one then
          if (curr - pos < (pos - prev))
          {
            /// return the current category.
            return new Category(this.PrimaryCategories[i].ToString(), i, curr);
          } else
          {
            /// If the current category is NOT closer than the previous one,
            /// then return the previous category.
            return new Category(this.PrimaryCategories[i - 1].ToString(),
              i - 1, prev);
          }
        }
      }
      /// If none of this works, then it is the final category.
      return new Category(this.PrimaryCategories.Last().ToString(),
              this.PrimaryCategories.Count - 1,
              (space_between_ticks * (this.PrimaryCategories.Count)));
    }
  }
}
