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
	private bool _hasHeaders;

	/// <summary>
	/// Initialize a new instance of the reader. Default delimiter is comma, default qualifier is none. 
	/// </summary>
	/// <param name="filePath">The path to the CSV file.</param>
	/// <param name="delimiter">The field delimiter.</param>
	/// <param name="textQualifier">The character surrounding text.</param>
	public CSVReader(string filePath, CSVDelimiter delimiter = CSVDelimiter.Comma, CSVTextQualifier textQualifier = CSVTextQualifier.None, bool hasHeaders = true)
	{
		_file = filePath;
		_delimiter = delimiter;
		_textQualifier = textQualifier;
		_hasHeaders = hasHeaders;
	}

	/// <summary>
	/// Return the CSV file as a 2 dimensional IEnumerable.
	/// </summary>
	/// <returns></returns>
	public async Task<DataTable> ReadAsTable()
	{
		return await Task.Run(() =>
		{
			DataTable table = new("CSVData");
			try
			{
				List<string> lines = [.. File.ReadAllLines(_file)];

				foreach (string header in lines.First().Split((char)_delimiter))
				{
					if (_hasHeaders)
					{
						table.Columns.Add(Clean(header), typeof(string));
					}
					else
						table.Columns.Add();
				}

				int i = _hasHeaders ? 1 : 0;
				for (; i < lines.Count; i++)
				{
					if (lines[i].Length == 0)
						continue;

					List<string> fields = [.. lines[i].Split((char)_delimiter)];
					DataRow row = table.NewRow();
					int j = 0;
					fields.ForEach(f => row[j++] = Clean(f));
					table.Rows.Add(row);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error Reading CSV File");
			}

			return table;
		});
	}

	/// <summary>
	/// Remove text qualifiers and trim spaces.
	/// </summary>
	/// <param name="input"></param>
	/// <returns></returns>
	private string Clean(string input)
	{
		return _textQualifier switch
		{
			CSVTextQualifier.Quotation => input.Replace((char)CSVTextQualifier.Quotation, ' ').Trim(),
			CSVTextQualifier.Apostrophe => input.Replace((char)CSVTextQualifier.Apostrophe, ' ').Trim(),
			_ => input.Trim()
		};
	}
}
