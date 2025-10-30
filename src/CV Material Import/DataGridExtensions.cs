using System.Windows.Controls;

namespace CV_Material_Import;

/// <summary>
/// Stores methods to operate on the datagrid data.
/// </summary>
/// <remarks>
/// I'm pretty unhappy with this class. Needs to be cleaned up and simplified. 
/// </remarks>
internal static class DataGridExtensions
{
	/// <summary>
	/// Loads the datagrid from a 2 dimensional IEnumerable.
	/// </summary>
	/// <param name="grid"></param>
	/// <param name="table"></param>
	public static void LoadFromGrid(this DataGrid grid, IEnumerable<IEnumerable<string>> table)
	{
		var rows = table.ToList();
		int columnCount = rows.First().Count();

		grid.Columns.Clear();
		grid.Items.Clear();

		for (int i = 0; i < columnCount; i++)
		{
			grid.Columns.Add(new DataGridTextColumn
			{
				Header = CreateMaterialComboBox(),
				Binding = new System.Windows.Data.Binding($"Column{i}"),
				Width = 150
			});
		}

		foreach (IEnumerable<string> line in rows)
		{
			var obj = new System.Dynamic.ExpandoObject() as IDictionary<string, object>;
			int colIndex = 0;
			foreach (string cell in line)
			{
				obj[$"Column{colIndex}"] = cell;
				colIndex++;
			}
			grid.Items.Add(obj);
		}
	}

	/// <summary>
	/// Returns the materials from the grid using the selections from headers. 
	/// </summary>
	/// <param name="grid"></param>
	/// <returns></returns>
	public static IEnumerable<Material> GetMaterials(this DataGrid grid)
	{
		List<Material> materials = [];
		Dictionary<MaterialFields, int> indexes = grid.LoadIndexes();
		foreach (IDictionary<string, object> row in grid.Items)
		{
			Material material = new();

			if (indexes.TryGetValue(MaterialFields.Name, out int nameIndex))
				material.Name = row[$"Column{nameIndex}"]?.ToString();

			if (indexes.TryGetValue(MaterialFields.Description, out int descIndex))
				material.Description = row[$"Column{descIndex}"]?.ToString();

			//if (indexes.TryGetValue(MaterialFields.SKU, out int skuIndex))
			//	material.SKU = row[$"Column{skuIndex}"]?.ToString();

			if (indexes.TryGetValue(MaterialFields.DefaultCost, out int costIndex) &&
				decimal.TryParse(row[$"Column{costIndex}"]?.ToString(), out var cost))
				material.DefaultCost = cost;

			if (indexes.TryGetValue(MaterialFields.SellPrice, out int priceIndex) &&
				decimal.TryParse(row[$"Column{priceIndex}"]?.ToString(), out var price))
				material.SellPrice = price;

			if (indexes.TryGetValue(MaterialFields.Width, out int widthIndex) &&
				double.TryParse(row[$"Column{widthIndex}"]?.ToString(), out var width))
				material.Width = Math.Round(Settings.Unit == CSVReader.CSVUnit.Metric ? width : width * 25.4, 3);

			if (indexes.TryGetValue(MaterialFields.Length, out int lengthIndex) &&
				double.TryParse(row[$"Column{lengthIndex}"]?.ToString(), out var length))
				material.Length = Math.Round(Settings.Unit == CSVReader.CSVUnit.Metric ? length : length * 25.4);

			if (indexes.TryGetValue(MaterialFields.Thickness, out int thickIndex) &&
				double.TryParse(row[$"Column{thickIndex}"]?.ToString(), out var thickness))
				material.Thickness = Math.Round(Settings.Unit == CSVReader.CSVUnit.Metric ? thickness : thickness * 25.4);

			materials.Add(material);
		}

		return materials;
	}

	private static Dictionary<MaterialFields, int> LoadIndexes(this DataGrid grid)
	{
		Dictionary<MaterialFields, int> dict = [];

		for (int i = 0; i < grid.Columns.Count; i++)
		{
			if (grid.Columns[i].Header is ComboBox comboBox &&
				comboBox.SelectedItem is string selectedItem &&
				Enum.TryParse(selectedItem, out MaterialFields field) &&
				field != MaterialFields.Ignore)
			{
				if (!dict.TryAdd(field, i))
				{
					throw new InvalidOperationException("Duplicate Fields!");
				}
			}
		}

		return dict;
	}

	private static ComboBox CreateMaterialComboBox()
	{
		return new ComboBox
		{
			ItemsSource = Enum.GetNames(typeof(MaterialFields)),
			SelectedItem = MaterialFields.Ignore.ToString(),
			Width = 120
		};
	}
}
