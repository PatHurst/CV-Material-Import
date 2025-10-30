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
		Database ??= new Database.Database(Settings.ServerName, Settings.DatabaseName);
		if (Database.IsConnected)
		{
			CV_Material_Import.MainWindow mainWindow = new();
			mainWindow.Show();
		}
	}

	/// <summary>
	/// Global access point to the database.
	/// </summary>
	internal static Database.Database Database { get; set; }

	private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
	{
		MessageBox.Show(e.Exception.Message, "Unhandled Exception!");
		e.Handled = true;
	}
}
