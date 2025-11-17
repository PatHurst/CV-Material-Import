using System.Windows;

namespace CV_Material_Import;

/// <summary>
/// Interaction logic for OverwriteMaterialsWindow.xaml
/// </summary>
public partial class OverwriteMaterialsWindow : Window
{
	private bool _overwrite;

	public string Material { get; }

	public OverwriteMaterialsWindow(string materialName)
	{
		Material = materialName;
		InitializeComponent();
	}

	public void ShowDialog(ref bool update, ref bool repeatChoice)
	{
		ShowDialog();
		update = _overwrite;
		repeatChoice = DoForAllMaterialsCheckBox.IsChecked ?? false;
	}


	private void OverwriteButton_Click(object sender, RoutedEventArgs e)
	{
		_overwrite = true;
		this.Close();
	}

	private void SkipButton_Click(object sender, RoutedEventArgs e)
	{
		_overwrite = false;
		this.Close();
	}
}
