using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Globalization;

namespace UdeC.iglUI
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
      static void Main(string[] args)
    {
        string lang = "";

        // Setting lang of the application
        if (args.Length > 0)
        {
            if (args[0] == "fr-CA")
                lang = args[0];
        }

      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new Form1(lang));
    }
  }
}
