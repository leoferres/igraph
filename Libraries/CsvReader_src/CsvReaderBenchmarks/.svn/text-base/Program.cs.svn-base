#region Using directives

using System;
using System.Diagnostics;

using LumenWorks.Framework.IO.Csv;

#endregion

namespace CsvReaderDemo
{
	class Program
	{
		[System.Runtime.InteropServices.DllImport("Kernel32.dll")]
		private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

		[System.Runtime.InteropServices.DllImport("Kernel32.dll")]
		private static extern bool QueryPerformanceFrequency(out long lpFrequency);

		[STAThread()]
		static void Main(string[] args)
		{
			const string TestFile1 = @"..\..\test1.csv";
			const string TestFile2 = @"..\..\test2.csv";
			const string TestFile3 = @"..\..\test3.csv";

			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

			if (args.Length > 0)
			{
				if (args.Length == 1)
				{
					string s = args[0].ToUpper();

					switch (s)
					{
						case "CSVREADER":
							CsvReaderBenchmark.Run(TestFile3);
							return;
						case "OLEDB":
							OleDbBenchmark.Run(TestFile3);
							return;
						case "REGEX":
							RegexBenchmark.Run(TestFile3);
							return;
					}
				}

				Console.WriteLine("Possible values : CsvReader, OleDb, Regex");
				return;
			}

			const int Field = 72;
			long fileSize = new System.IO.FileInfo(TestFile2).Length / 1024 / 1024;

			CachedCsvReader csv;

			long start;
			long end;
			long frequency;
			long clocks;
			double time;
			double rate;

			QueryPerformanceFrequency(out frequency);

			for (int i = 1; i < 4; i++)
			{
				Console.WriteLine("Test pass #{0} - All fields\n", i);
			
				QueryPerformanceCounter(out start);
				CsvReaderBenchmark.Run(TestFile2);
				QueryPerformanceCounter(out end);
				GetStats(start, end, frequency, fileSize, out clocks, out time, out rate);
				Console.WriteLine("CsvReader - No cache :\t\t {0} clocks, {1:f4} sec., {2:f4} MB/sec.", clocks, time, rate);
				GC.Collect();

				QueryPerformanceCounter(out start);
				csv = CachedCsvReaderBenchmark.Run1(TestFile2);
				QueryPerformanceCounter(out end);
				GetStats(start, end, frequency, fileSize, out clocks, out time, out rate);
				Console.WriteLine("CachedCsvReader - Run 1 :\t {0} clocks, {1:f4} sec., {2:f4} MB/sec.", clocks, time, rate);
				GC.Collect();

				QueryPerformanceCounter(out start);
				CachedCsvReaderBenchmark.Run2(csv);
				QueryPerformanceCounter(out end);
				GetStats(start, end, frequency, fileSize, out clocks, out time, out rate);
				Console.WriteLine("CachedCsvReader - Run 2 :\t {0} clocks, {1:f4} sec., {2:f4} MB/sec.", clocks, time, rate);
				csv = null;
				GC.Collect();

				QueryPerformanceCounter(out start);
				OleDbBenchmark.Run(TestFile2);
				QueryPerformanceCounter(out end);
				GetStats(start, end, frequency, fileSize, out clocks, out time, out rate);
				Console.WriteLine("OleDb :\t\t\t\t {0} clocks, {1:f4} sec., {2:f4} MB/sec.", clocks, time, rate);

				QueryPerformanceCounter(out start);
				RegexBenchmark.Run(TestFile2);
				QueryPerformanceCounter(out end);
				GetStats(start, end, frequency, fileSize, out clocks, out time, out rate);
				Console.WriteLine("Regex :\t\t\t\t {0} clocks, {1:f4} sec., {2:f4} MB/sec.", clocks, time, rate);

				Console.WriteLine();

				Console.WriteLine("Test pass #{0} - Field #{1} (middle)\n", i, Field);

				QueryPerformanceCounter(out start);
				CsvReaderBenchmark.Run(TestFile2, Field);
				QueryPerformanceCounter(out end);
				GetStats(start, end, frequency, fileSize, out clocks, out time, out rate);
				Console.WriteLine("CsvReader - No cache :\t\t {0} clocks, {1:f4} sec., {2:f4} MB/sec.", clocks, time, rate);
				GC.Collect();

				QueryPerformanceCounter(out start);
				csv = CachedCsvReaderBenchmark.Run1(TestFile2, Field);
				QueryPerformanceCounter(out end);
				GetStats(start, end, frequency, fileSize, out clocks, out time, out rate);
				Console.WriteLine("CachedCsvReader - Run 1 :\t {0} clocks, {1:f4} sec., {2:f4} MB/sec.", clocks, time, rate);
				GC.Collect();

				QueryPerformanceCounter(out start);
				CachedCsvReaderBenchmark.Run2(Field, csv);
				QueryPerformanceCounter(out end);
				GetStats(start, end, frequency, fileSize, out clocks, out time, out rate);
				Console.WriteLine("CachedCsvReader - Run 2 :\t {0} clocks, {1:f4} sec., {2:f4} MB/sec.", clocks, time, rate);
				csv = null;
				GC.Collect();

				QueryPerformanceCounter(out start);
				OleDbBenchmark.Run(TestFile2, Field);
				QueryPerformanceCounter(out end);
				GetStats(start, end, frequency, fileSize, out clocks, out time, out rate);
				Console.WriteLine("OleDb :\t\t\t\t {0} clocks, {1:f4} sec., {2:f4} MB/sec.", clocks, time, rate);

				QueryPerformanceCounter(out start);
				RegexBenchmark.Run(TestFile2, Field);
				QueryPerformanceCounter(out end);
				GetStats(start, end, frequency, fileSize, out clocks, out time, out rate);
				Console.WriteLine("Regex :\t\t\t\t {0} clocks, {1:f4} sec., {2:f4} MB/sec.", clocks, time, rate);

				Console.WriteLine();
				Console.WriteLine();
			}

			Console.WriteLine("Done");
			Console.ReadLine();
		}

		static void GetStats(long start, long end, long frequency, long fileSize, out long clocks, out double time, out double rate)
		{
			clocks = end - start;
			time = (double) clocks / frequency;
			rate = fileSize / time;
		}

		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			if (e.ExceptionObject != null)
				Console.WriteLine("Unhandled exception :\n\n'{0}'.", e.ExceptionObject.ToString());
			else
				Console.WriteLine("Unhandled exception occured.");

			Console.ReadLine();
		}
	}
}
