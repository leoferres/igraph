using System;
using System.Collections.Generic;
using IGraph.StatGraph;


namespace IGraph.GraphReaders
{
  /**@This interface must be implemented by all reader classes. Only one 
   * such reader has been
   * implemented so far, the \c ExcelGraphReader class. It returns a list of
   * \c StatisticalGraph objects, since it is sometimes the case that there
   * are several in one file or even one worksheet.
   * 
   * This interface consists of
   * only one method the \c BuildGraphList method, which takes the absolute
   * path of the file to open and the flag of whether the graph being processed
   * will be also saved as a gif file. Of course, we can envision this method
   * passing an enum as the second parameter in which other kinds of graphical
   * output (\latexonly \) will be allowed
   * 
   * The method will take the string containing the path and file to be
   * processed, together with a flag of wheather or not to save the graph
   * being processed as a gif file.
   * <<\filename>>=**/
  interface IIGraphReader 
  {
    List<StatisticalGraph> 
      BuildGraphList(string file, bool export_as_gif);
  }

}
