using System.Data;
using System.IO;
using System.Windows;

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
			try
			{
				string[] lines = File.ReadAllLines(_file);
				for (int i = 0; i < lines.Length; i++)
				{
					switch (_textQualifier)
					{
						case CSVTextQualifier.Quotation:
							lines[i] = lines[i].Replace((char)CSVTextQualifier.Quotation, (char)0);
							break;
						case CSVTextQualifier.Apostrophe:
							lines[i] = lines[i].Replace((char)CSVTextQualifier.Apostrophe, (char)0);
							break;
					}
				}

				int countOfColumns = lines.First().Count(c => c == (char)_delimiter) + 1;
				for (int i = 0; i < countOfColumns; i++)
					dataTable.Columns.Add($"Column{i}");

				foreach (string line in lines)
				{
					string[] fields = line.Split((char)_delimiter);
					for (int i = 0; i < fields.Length; i++)
						fields[i] = fields[i].Trim();
					dataTable.Rows.Add(fields);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error Reading CSV File");
			}

			return dataTable;
		});
	}

	public async Task<IEnumerable<IEnumerable<string>>> ReadAsList()
	{
		return await Task.Run(() =>
		{
			List<List<string>> grid = [];
			try
			{
				List<string> lines = File.ReadAllLines(_file).ToList();
				for (int i = 0; i < lines.Count; i++)
				{
					switch (_textQualifier)
					{
						case CSVTextQualifier.Quotation:
							lines[i] = lines[i].Replace((char)CSVTextQualifier.Quotation, (char)0);
							break;
						case CSVTextQualifier.Apostrophe:
							lines[i] = lines[i].Replace((char)CSVTextQualifier.Apostrophe, (char)0);
							break;
					}
				}

				foreach (string line in lines)
				{
					List<string> fields = [.. line.Split((char)_delimiter)];
					grid.Add(fields);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error Reading CSV File");
			}

			return grid;
		});
	}
}
