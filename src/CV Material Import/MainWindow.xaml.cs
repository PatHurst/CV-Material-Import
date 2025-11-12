using System.ComponentModel;
using System.Data;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using CV_Material_Import.CSVReader;
using CV_Material_Import.Database;

using Microsoft.Win32;

namespace CV_Material_Import;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, INotifyPropertyChanged
{
	private string _openedFile = string.Empty; // path to currently opened file
	private Dictionary<string, int> _materialFolders; //the CV material folders

	// binding sources
	public Dictionary<string, int> MaterialFolders => _materialFolders;

	public List<string> Delimiters => [..Enum.GetNames<CSVDelimiter>()];

	public List<string> TextQualifiers => [.. Enum.GetNames<CSVTextQualifier>()];

	private DataView _dataView = new();
	public DataView DataView
	{
		get => _dataView;
		set
		{
			if (_dataView != value)
			{
				_dataView = value;
				OnPropertyChanged(nameof(DataView));
			}
		}
	}

	public MainWindow()
	{
		_materialFolders = App.Database.GetMaterialFolders();
		InitializeComponent();
		InitializeControls();

		App.Database.ConnectionChanged += OnDatabaseConnectionChanged;
	}

	private void InitializeControls()
	{
		switch (Settings.Unit)
		{
			case CSVUnit.Metric:
				MetricRadioButton.IsChecked = true;
				break;
			case CSVUnit.Imperial:
			default:
				ImperialRadioButton.IsChecked = true;
				break;
		}
		DelimiterComboBox.SelectedIndex = Settings.Delimiter switch
		{
			CSVDelimiter.Comma => 2,
			CSVDelimiter.Space => 1,
			CSVDelimiter.Semicolon => 4,
			CSVDelimiter.Colon => 3,
			CSVDelimiter.Tab => 0,
			_ => 0
		};
		TextQualifierComboBox.SelectedIndex = Settings.TextQualifier switch
		{
			CSVTextQualifier.None => 0,
			CSVTextQualifier.Quotation => 1,
			CSVTextQualifier.Apostrophe => 2,
			_ => 0
		};
		SetLabel(App.Database.IsConnected, App.Database.ToString());
	}

	private void SetLabel(bool isConnected, string databaseConnection)
	{
		ConnectionStatusLabel.Content = (isConnected ? "Database Connected : " : "Database Disconnected... : ") + databaseConnection;
		ConnectionStatusLabel.Foreground = isConnected ? Brushes.Green : Brushes.Red;
	}

	private async void OpenCSVFile(object sender, RoutedEventArgs e)
	{
		OpenFileDialog ofd = new()
		{
			CheckFileExists = true,
			CheckPathExists = true,
			Filter = "Text Files|*.csv;*.txt;"
		};
		if (ofd.ShowDialog() == true)
		{
			_openedFile = ofd.FileName;
			await Load();
		}
	}

	private async Task Load()
	{
		if (_openedFile == string.Empty)
			return;
		CSVReader.CSVReader reader = new(_openedFile, Settings.Delimiter, Settings.TextQualifier, (bool)HasHeadersCheckBox.IsChecked!);
		var table = await reader.ReadAsTable();
		DataView = table.DefaultView;
		CreateHeaders();
	}

	private async void TextQualifierComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		Settings.TextQualifier = TextQualifierComboBox.SelectedIndex switch
		{
			0 => CSVTextQualifier.None,
			1 => CSVTextQualifier.Quotation,
			2 => CSVTextQualifier.Apostrophe,
			_ => CSVTextQualifier.None
		};
		await Load();
	}

	private async void DelimiterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		Settings.Delimiter = DelimiterComboBox.SelectedIndex switch
		{
			2 => CSVDelimiter.Comma,
			1 => CSVDelimiter.Space,
			4 => CSVDelimiter.Semicolon,
			3 => CSVDelimiter.Colon,
			0 => CSVDelimiter.Tab,
			_ => CSVDelimiter.Comma
		};
		await Load();
	}

	private void RadioButton_Checked(object sender, RoutedEventArgs e) => Settings.Unit = MetricRadioButton.IsChecked == true ? CSVUnit.Metric : CSVUnit.Imperial;

	private void DatabaseSettings_Click(object sender, RoutedEventArgs e)
	{
		SettingsWindow settingsWindow = new();
		settingsWindow.ShowDialog();
		_materialFolders = App.Database.GetMaterialFolders();
	}

	private void OnDatabaseConnectionChanged(object? sender, bool connectionOpen)
	{
		if (sender is Database.Database db)
			SetLabel(connectionOpen, db.ToString());
	}

	private async void HasHeadersCheckBox_Checked(object sender, RoutedEventArgs e) => await Load();

	private void MaterialGrid_Loaded(object sender, RoutedEventArgs e) => CreateHeaders();

	private void CreateHeaders()
	{
		List<string> properties = [.. Enum.GetNames<MaterialFields>()];
		foreach (var column in MaterialGrid.Columns)
		{
			string columnName = column.Header.ToString() ?? string.Empty;
			if (properties.Contains(columnName))
			{
				column.Header = CreatePropertyComboBox(columnName);
			}
			else
			{
				column.Header = CreatePropertyComboBox("Ignore");
			}
		}
		ParseHeaders();
	}

	private ComboBox CreatePropertyComboBox(string selectedItem)
	{
		var box = new ComboBox
		{
			ItemsSource = Enum.GetNames<MaterialFields>(),
			SelectedItem = selectedItem
		};
		box.SelectionChanged += On_PropertyComboBoxSelectionChanged;
		return box;
	}

	private void ParseHeaders()
	{
		List<string> selectedValues = [];
		foreach (var column in MaterialGrid.Columns)
		{
			if (column.Header is ComboBox box)
			{
				string selectedText = (string)box.SelectedItem;
				if (selectedValues.Contains(selectedText) || selectedText == "Ignore")
				{
					box.Foreground = Brushes.Red;
				}
				else
				{
					box.Foreground = Brushes.Green;
				}
				selectedValues.Add(selectedText);
			}
		}
		((InsertMaterialsCommand)InsertMaterialsCommand).RaiseCanExecuteChanged();
	}

	private void On_PropertyComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		ParseHeaders();
	}

	private InsertMaterialsCommand? _insertMaterialsCommand;
	public ICommand InsertMaterialsCommand
	{
		get
		{
			_insertMaterialsCommand ??=
				new InsertMaterialsCommand(() => MaterialGrid.GetMaterials(), () => ((KeyValuePair<string, int>)MaterialFoldersComboBox.SelectedItem).Value);
			_insertMaterialsCommand.CommandProgressChanged += InsertMaterialsCommand_CommandProgressChanged;
			_insertMaterialsCommand.CommandCompleted += InsertMaterialsCommand_CommandCompleted;
			return _insertMaterialsCommand;
		}
	}

	private void InsertMaterialsCommand_CommandCompleted(object? sender, int e)
	{
		RowsInsertedStatus.Content = $"{e} rows inserted successfully";
		RowsInsertedStatus.Foreground = e > 0
			? Brushes.Green 
			: Brushes.Red;
	}

	private void InsertMaterialsCommand_CommandProgressChanged(object? sender, float e) => ProgressBar.Value = e * 100;

	public event PropertyChangedEventHandler? PropertyChanged;

	protected void OnPropertyChanged(string propertyName) =>
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

	private void Exit_Click(object sender, RoutedEventArgs e)
	{
		Application.Current.Shutdown(0);
	}
}