using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using CV_Material_Import.CSVReader;

namespace CV_Material_Import;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.DelimiterComboBox.ItemsSource = Enum.GetNames<CSVDelimiter>();
        this.TextQualifierComboBox.ItemsSource = Enum.GetNames<CSVTextQualifier>();
        InitializeControls();

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
            CSVDelimiter.Comma => 0,
            CSVDelimiter.Space => 1,
            CSVDelimiter.Semicolon => 2,
            CSVDelimiter.Colon => 3,
            CSVDelimiter.Tab => 4,
            _ => 0
        };
        TextQualifierComboBox.SelectedIndex = Settings.TextQualifier switch
        {
            CSVTextQualifier.None => 0,
            CSVTextQualifier.Quotation => 1,
            CSVTextQualifier.Apostrophe => 2
        };

    }

	private void OpenCSVFileButton_Click(object sender, RoutedEventArgs e)
	{

	}

	private void TextQualifierComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		Settings.TextQualifier = TextQualifierComboBox.SelectedIndex switch
		{
			0 => CSVTextQualifier.None,
			1 => CSVTextQualifier.Quotation,
			2 => CSVTextQualifier.Apostrophe,
			_ => CSVTextQualifier.None
		};

	}

	private void DelimiterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		Settings.Delimiter = DelimiterComboBox.SelectedIndex switch
		{
			0 => CSVDelimiter.Comma,
			1 => CSVDelimiter.Space,
			2 => CSVDelimiter.Semicolon,
			3 => CSVDelimiter.Colon,
			4 => CSVDelimiter.Tab,
			_ => CSVDelimiter.Comma
		};

	}

	private void RadioButton_Checked(object sender, RoutedEventArgs e)
	{
        Settings.Unit = MetricRadioButton.IsChecked == true ? CSVUnit.Metric : CSVUnit.Imperial;
	}
}