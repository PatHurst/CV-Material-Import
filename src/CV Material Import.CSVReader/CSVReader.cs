using System.Data;
using System.Text;

namespace CV_Material_Import.CSVReader;

/// <summary>
/// Represents an object to read data from CSV files.
/// </summary>
public class CSVReader
{
	private string _file;
	private CSVDelimiter _delimiter;
	private CSVTextQualifier _textQualifier;

	/// <summary>
	/// Initialize a new instance of the reader. Default delimiter is comma, default qualifier is none. 
	/// </summary>
	/// <param name="filePath">The path to the CSV file.</param>
	/// <param name="delimiter">The field delimiter.</param>
	/// <param name="textQualifier">The character surrounding text.</param>
	public CSVReader(string filePath, CSVDelimiter delimiter = CSVDelimiter.Comma, CSVTextQualifier textQualifier = CSVTextQualifier.None)
	{
		_file = filePath;
		_delimiter = delimiter;
		_textQualifier = textQualifier;
	}

	public async Task<DataTable> Read()
	{
		return await Task.Run(() =>
		{
			DataTable dataTable = new("Materials");

			string[] lines = File.ReadAllLines(_file);
			for (int i = 0; i < lines.Length; i++)
			{
				switch (_textQualifier)
				{
					case CSVTextQualifier.DoubleQuotation:
						lines[i] = lines[i].Replace('"', (char)0);
						break;
					case CSVTextQualifier.SingleQuotation:
						lines[i] = lines[i].Replace('\'', (char)0);
						break;
				}
			}

			int countOfColumns = lines.First().Count(c => c == (char)_delimiter) + 1;
			for (int i = 0; i < countOfColumns; i++)
				dataTable.Columns.Add($"Column{i}");

			foreach (string line in lines)
			{
				string[] fields = line.Split((char)_delimiter);
				dataTable.Rows.Add(fields);
			}

			StringBuilder stringBuilder = new StringBuilder();
			foreach (DataRow row in dataTable.Rows)
			{
				for (int i = 0; i < dataTable.Columns.Count; i++)
				{
					stringBuilder.Append(row[i].ToString());
				}
				stringBuilder.Append(Environment.NewLine);
			}

			return dataTable;
		});
	}
}
