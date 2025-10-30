using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CV_Material_Import;
/// <summary>
/// Interaction logic for SettingsWindow.xaml
/// </summary>
public partial class SettingsWindow : Window
{
	public SettingsWindow()
	{
		InitializeComponent();
	}

	private void InitializeControls()
	{
		// if the servername is not stored in registry, try to read it from hexagons registry key.
		if (Settings.ServerName is null or "")
		{
			ReadConfigFromDefaultRegistry();
		}
		else
		{
			ServerNameTextBox.Text = Settings.ServerName;
			DatabaseNameTextBox.Text = Settings.DatabaseName;
		}
		VersionComboBox.SelectedIndex = Settings.CurrentVersion switch
		{
			2022 => 0,
			2023 => 1,
			2024 => 2,
			2025 => 3,
			_ => 0
		};
	}

	private void ReadConfigFromDefaultRegistry()
	{
		string version = VersionComboBox.SelectedItem is null ? "2022" : (string)(VersionComboBox.SelectedItem as ComboBoxItem)!.Tag;
		if (ServerNameTextBox is not null)
			ServerNameTextBox.Text = Utilities.GetValueFromRegistry<string>($"HKEY_LOCAL_MACHINE\\SOFTWARE\\Hexagon\\CABINET VISION\\Common {version}\\CVData", "CVDataSQLPath");
		if (DatabaseNameTextBox is not null)
			DatabaseNameTextBox.Text = Utilities.GetValueFromRegistry<string>($"HKEY_LOCAL_MACHINE\\SOFTWARE\\Hexagon\\CABINET VISION\\Common {version}\\CVData", "CVDataCatalog");
	}

	private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		ConnectionStringTextBox.Text = string.Format("Server={0};Database={1};Trusted_Connection=True;TrustServerCertificate=True", ServerNameTextBox.Text, DatabaseNameTextBox.Text);
	}

	private void VersionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		ReadConfigFromDefaultRegistry();
	}

	private void Window_Loaded(object sender, EventArgs e)
	{
		InitializeControls();
	}

	private void TestConnectionButton_Click(object sender, RoutedEventArgs e)
	{
		ConnectionStatusLabel.Content = "Trying to Connect...";
		Database.Database db = new(ConnectionStringTextBox.Text);
		if (db.IsConnected)
		{
			ConnectionStatusLabel.Content = "Connected Successfully!";
			ConnectionStatusLabel.Foreground = Brushes.Green;
		}
		else
		{
			ConnectionStatusLabel.Content = "Connection Failed...";
			ConnectionStatusLabel.Foreground = Brushes.Red;
		}
	}

	private void CancelButton_Click(object sender, RoutedEventArgs e)
	{
		this.Close();
	}

	private void SaveButton_Click(object sender, RoutedEventArgs e)
	{
		Settings.ServerName = ServerNameTextBox.Text;
		Settings.DatabaseName = DatabaseNameTextBox.Text;
		Settings.CurrentVersion = Convert.ToInt32((VersionComboBox.SelectedItem as ComboBoxItem)!.Tag);
		App.Database.ChangeConnectionn(ConnectionStringTextBox.Text);
		this.Close();
	}
}
