using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IGraph.StatGraph
{
  public class SGTextBoxCollection : IEnumerable<SGTextBox>
  {
    private List<SGTextBox> tb_collection;
    
    public SGTextBoxCollection()
    {
      tb_collection = new List<SGTextBox>();
    }

    public SGTextBoxCollection(List<SGTextBox> tbxlist)
    {
      tb_collection = tbxlist;
    }

    public void AddTextBox(SGTextBox tb)
    {
      tb_collection.Add(tb);
    }

    public void RemoveTextBox(SGTextBox tb)
    {
        tb_collection.Remove(tb);
    }

    public SGTextBox GetTextBox(int i)
    {
      return tb_collection[i];
    }

    public IEnumerator<SGTextBox> GetEnumerator()
    {
      return this.tb_collection.GetEnumerator();
    }

    public void SortHorizontally()
    {
      SGTextBox.SortOrder = SGTextBox.sortMethod.POS_X;
      tb_collection.Sort();
    }

    public int NumberOfCategories
    {
      get { return tb_collection.Count; }
    }

    public int CountFootnotes()
    {
        int i = 0;

        foreach (var item in tb_collection)
        {
            if ((item.Function).ToString() == "FOOTNOTE")
                i++;
        }
        return i;
    }
    public void SortVertically()
    {
      SGTextBox.SortOrder = SGTextBox.sortMethod.POS_Y;
      tb_collection.Sort();
    }

    public override string ToString()
    {
      string rtn = "";
      foreach (SGTextBox t in this.tb_collection)
      {
        rtn += String.Format("[{0}] ", t.Text);
      }
      return rtn;
    }

    System.Collections.IEnumerator 
      System.Collections.IEnumerable.GetEnumerator()
    {
      return tb_collection.GetEnumerator();
    }
  }
}
