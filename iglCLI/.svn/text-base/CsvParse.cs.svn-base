using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using log4net;

using IGraph.Utils;

using LumenWorks.Framework.IO.Csv;

namespace IGraph
{
  class CsvParse
  {
    private static readonly ILog log =
      LogManager.GetLogger(typeof(CsvParse));

    public static CsvFile[] parse(string filename)
    {
      log.Debug("Attempting to parse CSV file.");
      CsvFile field = null;
      List<CsvFile> listValues = new List<CsvFile>();

      CsvReader csv =
        new CsvReader(new StreamReader(
          filename, System.Text.Encoding.GetEncoding("iso-8859-1"),true),
          false, 
          ',', 
          1024);

      int malformed = 0;
      while (csv.ReadNextRecord())
      {
        try
        {
          field = new CsvFile();
          field.Filename = csv[0];
          field.Language = csv[1].ToUpper();
          field.Title = csv[2];

          listValues.Add(field);
        } catch
        {
          malformed++;
        }
      }
      log.Debug(String.Format("Parsed {0} entries, {1} were ill-formed.",
        listValues.Count, malformed));
      return listValues.ToArray();
    }
  }
}
