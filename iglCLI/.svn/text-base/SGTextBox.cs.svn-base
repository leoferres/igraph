using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IGraph.Utils;

namespace IGraph.StatGraph
{
  public class SGTextBox : IComparable
  {

    public enum sortMethod
    {
      POS_X = 0,
      POS_Y = 1
    };
    
    public enum SemanticFunction
    {
      MAIN_TITLE_PRIMARY,
      MAIN_TITLE_SECONDARY,
      Y_AXIS_TITLE_PRIMARY,
      Y_AXIS_TITLE_SECONDARY,
      X_AXIS_TITLE_PRIMARY,
      X_AXIS_TITLE_SECONDARY,
      FOOTNOTE,
      INFORMATION_NOTE,
      YEAR,
      PROVINCE,
      ORPHAN_BOX
    };
    
    public enum HorizontalAlignment
    {
      LEFT,
      CENTER,
      RIGHT,
      JUSTIFIED
    };
    
    private static sortMethod sm;

    public Size BoxSize
    {
      get;
      set;
    }
    
    public string Text { get; set; }

    public SemanticFunction Function { get; set; }

    public SGGeometry Geometry { get; set; }

    public int ID { get; set; }

    public HorizontalAlignment Justification { get; set; }

    public static sortMethod SortOrder
    {
      get { return sm; }
      set { sm = value; }
    }

    public string ReturnAsString()
    {
      return String.Format("Textbox found: "
        + "id={0}, halign={1}, ({2}).",
        this.ID, this.Justification, this.Geometry.showAsString());
    }

    /* I need to be able to sort textboxes according to where they appear,
     * see the enum above for the sorting methods.
     */
    public int CompareTo(object tbox)
    {
      switch (sm)
      {
        case sortMethod.POS_Y:
          return this.Geometry.PosY.CompareTo(((SGTextBox)tbox).Geometry.PosY);
        case sortMethod.POS_X:
          return this.Geometry.PosX.CompareTo(((SGTextBox)tbox).Geometry.PosX);
        default:
          return this.Geometry.PosY.CompareTo(((SGTextBox)tbox).Geometry.PosY);
      }
    }

  }
}
