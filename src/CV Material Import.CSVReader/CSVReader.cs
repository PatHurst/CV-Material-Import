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

	/// <summary>
	/// Return the CSV file as a 2 dimensional IEnumerable.
	/// </summary>
	/// <returns></returns>
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
							lines[i] = lines[i].Replace((char)CSVTextQualifier.Quotation, ' ');
							break;
						case CSVTextQualifier.Apostrophe:
							lines[i] = lines[i].Replace((char)CSVTextQualifier.Apostrophe, ' ');
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
