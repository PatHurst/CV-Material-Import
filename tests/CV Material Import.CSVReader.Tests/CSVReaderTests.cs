using System.Data;

using CV_Material_Import.CSVReader;

using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace CV_Material_Import.CSVReader.Tests;

[TestClass]
public sealed class CSVReaderTests
{
	[TestMethod]
	public void TestReadCSVFile()
	{
		CSVReader reader = new(@"C:\Users\patri\source\CV Material Import\tests\CV Material Import.CSVReader.Tests\Material.csv");
		DataTable dataTable = reader.Read().Result;

		foreach (DataRow row in dataTable.Rows)
		{
			for (int i = 0; i < dataTable.Columns.Count; i++)
			{
				Console.Write(row[i].ToString());
			}
			Console.WriteLine(Environment.NewLine);
		}
	}
}
