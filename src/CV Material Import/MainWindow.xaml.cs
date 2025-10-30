using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using CV_Material_Import.CSVReader;

using Microsoft.Win32;

namespace CV_Material_Import;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	private string _openedFile = string.Empty;
	private Dictionary<string, int> _materialFolders = App.Database.GetMaterialFolders();

	public MainWindow()
	{
		InitializeComponent();
		this.DelimiterComboBox.ItemsSource = Enum.GetNames<CSVDelimiter>();
		this.TextQualifierComboBox.ItemsSource = Enum.GetNames<CSVTextQualifier>();
		this.MaterialFoldersComboBox.ItemsSource = _materialFolders.Keys;
		this.MaterialFoldersComboBox.SelectedIndex = 0;
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
			CSVTextQualifier.Apostrophe => 2
		};
		SetLabel(App.Database.IsConnected);
	}

	private void SetLabel(bool isConnected)
	{
		ConnectionStatusLabel.Content = isConnected ? "Database Connected" : "Database Disconnected...";
		ConnectionStatusLabel.Foreground = isConnected ? Brushes.Green : Brushes.Red;
	}

	private async void OpenCSVFile(object sender, RoutedEventArgs e)
	{
		OpenFileDialog ofd = new()
		{
			CheckFileExists = true,
			CheckPathExists = true,
			DefaultExt = "Text Files|*.csv;*.txt;"
		};
		if (ofd.ShowDialog() == true)
		{
			_openedFile = ofd.FileName;
			await Reload();
		}
	}

	private async Task Reload()
	{
		if (_openedFile == string.Empty)
			return;
		CSVReader.CSVReader reader = new(_openedFile, Settings.Delimiter, Settings.TextQualifier);
		IEnumerable<IEnumerable<string>> grid = await reader.ReadAsList();
		MaterialGrid.LoadFromGrid(grid);
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
		await Reload();
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
		await Reload();
	}

	private void RadioButton_Checked(object sender, RoutedEventArgs e)
	{
		Settings.Unit = MetricRadioButton.IsChecked == true ? CSVUnit.Metric : CSVUnit.Imperial;
	}

	private void DatabaseSettings_Click(object sender, RoutedEventArgs e)
	{
		SettingsWindow settingsWindow = new();
		settingsWindow.ShowDialog();
	}

	private void OnDatabaseConnectionChanged(object? sender, bool connectionOpen)
	{
		SetLabel(connectionOpen);
	}

	private void InsertMaterialButton_Click(object sender, RoutedEventArgs e)
	{
		try
		{
			var materials = MaterialGrid.GetMaterials();
			int rowsInserted = new InsertMaterialsCommand(materials, _materialFolders[(string)MaterialFoldersComboBox.SelectedItem]).Execute();
			RowsInsertedStatus.Content = $"{rowsInserted} rows inserted successfully";
			RowsInsertedStatus.Foreground = rowsInserted > 0 ? Brushes.Green : Brushes.Red;
		}
		catch (Exception ex)
		{
			MessageBox.Show("Operation Canceled: " + ex.Message);
		}
	}
}