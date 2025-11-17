using CV_Material_Import.CSVReader;

namespace CV_Material_Import;

/// <summary>
/// Represents the settings to import a CSV file. Setting these property values will update the registry.
/// </summary>
internal static class Settings
{
	private static CSVDelimiter _delimiter;
	private static CSVTextQualifier _textQualifier;
	private static CSVUnit _unit;
	private static string _serverName;
	private static string _databaseName;
	private static int _currentVersion;
	private static readonly string _keyPath = @"HKEY_CURRENT_USER\Software\CV Material Import";

	static Settings()
	{
		_delimiter = (CSVDelimiter)Utilities.GetValueFromRegistry<int>(_keyPath, nameof(Delimiter));
		_textQualifier = (CSVTextQualifier)Utilities.GetValueFromRegistry<int>(_keyPath, nameof(TextQualifier));
		_unit = (CSVUnit)Utilities.GetValueFromRegistry<int>(_keyPath, nameof(Unit));
		_serverName = Utilities.GetValueFromRegistry<string>(_keyPath, nameof(ServerName));
		_databaseName = Utilities.GetValueFromRegistry<string>(_keyPath, nameof(DatabaseName));
		_currentVersion = Utilities.GetValueFromRegistry<int>(_keyPath, nameof(CurrentVersion));
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

	/// <summary>
	/// The name of the database server.
	/// </summary>
	public static string ServerName
	{
		get => _serverName;
		set
		{
			_serverName = value;
			Utilities.SetValueInRegistry(_keyPath, nameof(ServerName), value);
		}
	}

	/// <summary>
	/// The name of the database.
	/// </summary>
	public static string DatabaseName
	{
		get => _databaseName;
		set
		{
			_databaseName = value;
			Utilities.SetValueInRegistry(_keyPath, nameof(DatabaseName), value);
		}
	}

	/// <summary>
	/// The current version of CV.
	/// </summary>
	public static int CurrentVersion
	{
		get => _currentVersion;
		set
		{
			_currentVersion = value;
			Utilities.SetValueInRegistry(_keyPath, nameof(CurrentVersion), value);
		}
	}
}
