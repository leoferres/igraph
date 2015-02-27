#region Using directives

using System;
using System.IO;
using System.Text;

using LumenWorks.Framework.IO.Csv;

#endregion

namespace CsvReaderDemo
{
	public sealed class CachedCsvReaderBenchmark
	{
		private CachedCsvReaderBenchmark()
		{
		}

		public static CachedCsvReader Run1(string path)
		{
			return Run1(path, -1);
		}

		public static CachedCsvReader Run1(string path, int field)
		{
			CachedCsvReader csv = new CachedCsvReader(new StreamReader(path), false);

			string s;

			if (field == -1)
			{
				while (csv.ReadNextRecord())
				{
					for (int i = 0; i < csv.FieldCount; i++)
						s = csv[i];
				}
			}
			else
			{
				while (csv.ReadNextRecord())
					s = csv[field];
			}

			return csv;
		}

		public static void Run2(CachedCsvReader csv)
		{
			Run2(-1, csv);
		}

		public static void Run2(int field, CachedCsvReader csv)
		{
			using (csv)
			{
				string s;

				if (field == -1)
				{
					while (csv.ReadNextRecord())
					{
						for (int i = 0; i < csv.FieldCount; i++)
							s = csv[i];
					}
				}
				else
				{
					while (csv.ReadNextRecord())
						s = csv[field];
				}
			}
		}
		
	}
}
