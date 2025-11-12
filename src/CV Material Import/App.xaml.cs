using System.Windows;

namespace CV_Material_Import;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		if (Settings.ServerName is null or "")
		{
			SettingsWindow settingsWindow = new();
			settingsWindow.ShowDialog();
		}
		Database ??= new Database.Database(Settings.ServerName!, Settings.DatabaseName);
		InitializeScreens(Database.IsConnected);
	}

	/// <summary>
	/// Persist the settings screen until database successfully connects.
	/// </summary>
	/// <param name="databaseConnected"></param>
	private void InitializeScreens(bool databaseConnected)
	{
		if (!databaseConnected)
		{
			SettingsWindow settingsWindow = new();
			settingsWindow.ShowDialog();
		}
		if (Database.IsConnected)
		{
			CV_Material_Import.MainWindow mainWindow = new();
			mainWindow.Show();
		}
		else
		{
			InitializeScreens(Database.IsConnected);
		}
	}

	/// <summary>
	///		Global access point to the database.
	/// </summary>
	internal static Database.Database Database { get; set; }

	private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
	{
		MessageBox.Show(e.Exception.Message, "Unhandled Exception!");
	}
}
