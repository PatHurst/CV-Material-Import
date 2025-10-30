using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using CV_Material_Import.CSVReader;

using Microsoft.Win32;

namespace CV_Material_Import;

/// <summary>
/// Represents the settings to import a CSV file. Setting these property values will update the registry.
/// </summary>
internal static class Settings
{
	private static CSVDelimiter _delimiter;
	private static CSVTextQualifier _textQualifier;
	private static CSVUnit _unit;
	private static readonly string _keyPath = @"HKEY_CURRENT_USER\Software\CV Material Import";

	static Settings()
	{
		_delimiter = (CSVDelimiter)Utilities.GetValueFromRegistry<int>(_keyPath, nameof(Delimiter));
		_textQualifier = (CSVTextQualifier)Utilities.GetValueFromRegistry<int>(_keyPath, nameof(TextQualifier));
		_unit = (CSVUnit)Utilities.GetValueFromRegistry<int>(_keyPath, nameof(Unit));
	}

	/// <summary>
	/// The field delimiter.
	/// </summary>
	public static CSVDelimiter Delimiter
	{
		get => _delimiter;
		set
		{
			_delimiter = value;
			Utilities.SetValueInRegistry(_keyPath, nameof(Delimiter), (int)value);
		}
	}

	/// <summary>
	/// The text qualifier.
	/// </summary>
	public static CSVTextQualifier TextQualifier
	{
		get => _textQualifier;
		set
		{
			_textQualifier = value;
			Utilities.SetValueInRegistry(_keyPath, nameof(TextQualifier), (int)value);
		}
	}

	/// <summary>
	/// The measurement unit.
	/// </summary>
	public static CSVUnit Unit
	{
		get => _unit;
		set
		{
			_unit = value;
			Utilities.SetValueInRegistry(_keyPath, nameof(Unit), (int)value);
		}
	}

}
