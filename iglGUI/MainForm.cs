using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Globalization;

namespace UdeC.iglUI
{
  public partial class Form1 : Form
  {
    List<string> lstOfFiles;
    int idxOfFiles;
    string beginmark = "<!--descriptionbegin-->";
    string endmark = "<!--descriptionend-->";
    string htmlFile;
    string gifFile;

    public Form1(string lang)
    {
      if (lang.Length > 0) 
        System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);

      InitializeComponent();
      TranslateComponent();
    }

    private void button1_Click(object sender, EventArgs e)
    {
      fd.ShowDialog();
      dirbox.Text = fd.SelectedPath;
      showListOfFiles();
    }

    private void Generate_Click(object sender, EventArgs e)
    {
      if (dirbox.Text != "" && Directory.Exists(dirbox.Text))
      {
        statusBar.Text = Strings.T("Running iGraph, please wait a bit");

        if (RunCommandLine(dirbox.Text))
        {  //igraph has been loaded succesfull

            showListOfFiles();

            if (lstOfFiles.Count == 0)
            {
                MessageBox.Show(Strings.T("Chart files not found"),
                 "iglUI Message",
                  MessageBoxButtons.OK,
                  MessageBoxIcon.Exclamation);
            }
        }
        else
        { // error on igraph
            MessageBox.Show(Strings.T("Error on iGraph, see the log window"),
                 "iglUI Message",
                  MessageBoxButtons.OK,
                  MessageBoxIcon.Exclamation);
        }
        

      }
      else if (!Directory.Exists(dirbox.Text))
      {
          MessageBox.Show(Strings.T("Directory not found"),
          "iglUI Message",
          MessageBoxButtons.OK,
          MessageBoxIcon.Exclamation);
      }
      else
      {
        MessageBox.Show(Strings.T("No directory was specified"), 
          "iglUI Message", 
          MessageBoxButtons.OK, 
          MessageBoxIcon.Exclamation);
      }
    }

    private List<string> GetListOFiles(string p)
    {
      List<string> files = new List<string>();

      DirectoryInfo di = new DirectoryInfo(p);
      FileInfo[] rgFiles = di.GetFiles("*.gif");

      foreach (FileInfo f in rgFiles)
      {
        files.Add(f.Name.Substring(0, f.Name.Length - 4));
      }

      return files;
    }

    private bool RunCommandLine(string dir)
    {
        try
        {
            Process proc = new Process();
            string logtext;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            //proc.StartInfo.FileName = @"C:\Users\diegocaro\Documents\igraph\bin\Debug\igl.exe ";
            proc.StartInfo.FileName = @".\igl.exe ";
            proc.StartInfo.Arguments = "-g -l=" + numericLogLevel.Value + " \"" + dir + "\"";
            proc.Start();

            logtext = "C:\\> igl.exe -g -l=" + numericLogLevel.Value + " \"" + dir + "\"\r\n";
            logtext += proc.StandardOutput.ReadToEnd().Replace("\n", "\r\n");
            logtext += proc.StandardError.ReadToEnd().Replace("\n", "\r\n");
            
            infobox.Text += logtext;
            saveLogfile(dir + "\\igraph.log", logtext);

            proc.WaitForExit();
            infobox.Refresh();

            
            statusBar.Text = Strings.T("Done.");
            return true;
        }
        catch (Exception ex)
        {
          statusBar.Text = Strings.T("Error on iGraph, see the log window.");
          infobox.Text = ex.Message;
          return false;
        }
    }

    private void showListOfFiles()
    {
      FileInfo gifFileInfo;
      bool wrongFiles;
      if (dirbox.Text != "" && Directory.Exists(dirbox.Text))
      {
        statusBar.Text = Strings.T("Loading graph files from") + " \"" + dirbox.Text + "\".";

        lstOfFiles = GetListOFiles(dirbox.Text);
        idxOfFiles = -1;
        //pictureBox1.Load(lstOfFiles.First()+".gif");

        statusBar.Text = Strings.T("Done.");

        //verifing if html files has one gif file
        for(int i=0; i<lstOfFiles.Count; i++)
        {
            wrongFiles = false;
            gifFile = dirbox.Text + "\\" + lstOfFiles[i] + ".gif";
            htmlFile = dirbox.Text + "\\" + lstOfFiles[i] + ".html";

            if (!File.Exists(gifFile) || !File.Exists(htmlFile))
            {
                wrongFiles = true;
            }
            else if (File.Exists(gifFile))
            {
                gifFileInfo = new FileInfo(gifFile);
                if (gifFileInfo.Length == 0)
                {
                    wrongFiles = true;
                }
            }


            if (wrongFiles)
            {
                lstOfFiles.RemoveAt(i);
                i--;
            }
        }


        if (lstOfFiles.Count > 0)
        {
          saveButton.Enabled = true;
          goNext();
        }
      }
    }

    private void goNext()
    {
        idxOfFiles++;
        showGraph();

        
    }
    private void goPrev()
    {
        idxOfFiles--;
        showGraph();

        
    }

    private void showGraph()
    {

        gifFile = dirbox.Text + "\\" + lstOfFiles[idxOfFiles] + ".gif";
        htmlFile = dirbox.Text + "\\" + lstOfFiles[idxOfFiles] + ".html";

        if (File.Exists(gifFile) && File.Exists(htmlFile))
        {
            gifBox.Load(gifFile);
            htmlBox.Text = loadhtmlFile(htmlFile);

            statusBar.Text = Strings.T("Viewing file") + " \"" + htmlFile + "\".";
        }


        // last file
        if (idxOfFiles == lstOfFiles.Count - 1)
        {
            nextButton.Enabled = false;
            prevButton.Enabled = true;
        }
         //there is not files
 /*       else if (lstOfFiles.Count == 0)
        {
            prevButton.Enabled = false;
            nextButton.Enabled = false;
        }*/
        //first file
        else if (idxOfFiles == 0)
        {
            prevButton.Enabled = false;
            nextButton.Enabled = true;
        }
        //2nd to last -1 files
        else
        {
            nextButton.Enabled = true;
            prevButton.Enabled = true;
        }



    }

    private string loadhtmlFile(string f)
    {
        System.IO.StreamReader reader;
        string html;
        string text;
        int begin;
        int end;

        reader = new System.IO.StreamReader(f);

        html = reader.ReadToEnd();
        reader.Close();

        begin = html.IndexOf(beginmark);
        end = html.IndexOf(endmark);
        text = html.Substring(begin + beginmark.Length, end - begin - beginmark.Length);

        text = Regex.Replace(text, @"[\n\r]+", "", RegexOptions.Multiline);
        text = Regex.Replace(text, @"<br[ ]*/?>", "\r\n", RegexOptions.Multiline);
        text = Regex.Replace(text, @"<[^>]*>", "", RegexOptions.Multiline);

        text = Regex.Replace(text, @"[ \t]+", " ", RegexOptions.Multiline);
        text = Regex.Replace(text, @"^ ", "", RegexOptions.Multiline);

        text = "<p>" + text + "</p>";
        return text;
    }

    private void savehtmlFile(string f, string content)
    {
        System.IO.StreamReader reader;
        System.IO.StreamWriter writer;
        int begin;
        int end;
        string html;
        string newhtml;

        reader = new System.IO.StreamReader(f);
        html = reader.ReadToEnd();
        reader.Close();

        begin = html.IndexOf(beginmark);
        end = html.IndexOf(endmark);

        content = content.Replace("\n", "\n<br />");

        writer = new System.IO.StreamWriter(f);

        newhtml = html.Substring(0, begin + beginmark.Length ) + "\n";
        newhtml += content;
        newhtml += html.Substring(end);

        writer.Write(newhtml);

        writer.Close();
    }

    private void prevButton_Click(object sender, EventArgs e)
    {
        goPrev();
    }

    private void nextButton_Click(object sender, EventArgs e)
    {
        goNext();
    }

    private void saveButton_Click(object sender, EventArgs e)
    {
        savehtmlFile(htmlFile, htmlBox.Text);
        statusBar.Text = Strings.T("Description saved on") + " \"" + htmlFile + "\".";
    }

    private void saveLogfile(string filename, string logtext)
    {
        StreamWriter logfile = new StreamWriter(filename, true); //append
        logfile.Write(logtext);
        logfile.Close();
    }

    private void TranslateComponent()
    {
        // 
        // groupBox1
        // 
        this.groupBox1.Text = Strings.T("Step 1: Choose directory and generate");
        // 
        // Generate
        // 
        this.Generate.AccessibleDescription = Strings.T("Generates iGraph descriptions for Excel files in folder.");
        this.Generate.AccessibleName = Strings.T("Generate descriptions.");
        this.Generate.Text = Strings.T("Run");

        // 
        // dirbox
        // 
        this.dirbox.AccessibleDescription = Strings.T("Text displaying the chosen directory.");
        this.dirbox.AccessibleName = Strings.T("Chosen directory.");

        // 
        // opendir
        // 
        this.opendir.Text = Strings.T("Browse");
        this.opendir.AccessibleDescription = Strings.T("Browse and select the directory that contains the excel graphs.");
        this.opendir.AccessibleName = Strings.T("Browse.");

        // 
        // mn
        // 

        // 
        // fileToolStripMenuItem
        // 
        this.fileToolStripMenuItem.Text = Strings.T("File");

        // 
        // exitToolStripMenuItem
        // 
        this.exitToolStripMenuItem.Text = Strings.T("Exit");

        // 
        // helpToolStripMenuItem
        // 
        this.helpToolStripMenuItem.Text = Strings.T("Help");

        // 
        // aboutToolStripMenuItem
        // 
        this.aboutToolStripMenuItem.Text = Strings.T("About...");
        // 
        // gifBox
        // 
        this.gifBox.AccessibleDescription = Strings.T("This space displays the gif file being analyzed.");
        this.gifBox.AccessibleName = Strings.T("Picture viewer.");

        // 
        // htmlBox
        // 
        this.htmlBox.AccessibleDescription = Strings.T("This space shows the generated text.");
        this.htmlBox.AccessibleName = Strings.T("Description box.");

        // 
        // saveButton
        // 
        this.saveButton.AccessibleDescription = Strings.T("Accept and save the generated description.");
        this.saveButton.AccessibleName = Strings.T("Accept and save.");
        this.saveButton.Text = Strings.T("Ok, Save");

        // 
        // nextButton
        // 
        this.nextButton.AccessibleDescription = Strings.T("Go to the previous graph and description.");
        this.nextButton.AccessibleName = Strings.T("Next graph.");
        this.nextButton.Text = Strings.T("Next");

        // 
        // prevButton
        // 
        this.prevButton.AccessibleDescription = Strings.T("Go to the previous graph and description.");
        this.prevButton.AccessibleName = Strings.T("Previous graph.");
        this.prevButton.Text = Strings.T("Previous");

        // 
        // bugButton
        // 

        // 
        // groupBox2
        // 
        this.groupBox2.Text = Strings.T("Step 2: Check descriptions");

        // 
        // groupBox3
        // 
        this.groupBox3.Text = Strings.T("Information");

        // 
        // labelLogLevel
        // 
        this.labelLogLevel.Text = Strings.T("Log Level:");

        // 
        // numericLogLevel
        // 
        this.numericLogLevel.AccessibleDescription =  Strings.T("Select the log level for igraph, where the value 1 is quite and 6 is verbose.");
        this.numericLogLevel.AccessibleName =  Strings.T("Log level.");

        // 
        // infobox
        // 
        this.infobox.AccessibleDescription = Strings.T("This space displays textual information to the user");
        this.infobox.AccessibleName = Strings.T("Information box");
        // 
        // statusStripBar
        // 

        // 
        // statusBar
        // 

        // 
        // Form1
        // 
        this.Text = Strings.T("iglUI - Statistics Canada");
    }

  }
}
