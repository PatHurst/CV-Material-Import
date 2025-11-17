using System.Data;
using System.Windows.Controls;

using CV_Material_Import.CSVReader;

namespace CV_Material_Import;

/// <summary>
/// Stores methods to operate on the datagrid data.
/// </summary>
internal static class DataGridExtensions
{
	/// <summary>
	/// Returns the materials from the grid using the selections from headers. 
	/// </summary>
	/// <param name="grid">The Datagrid</param>
	/// <returns>An <see cref="IEnumerable{T}">IEnumberable</see> of <see cref="Material">Material</see></returns>
	public static IEnumerable<Material> GetMaterials(this DataGrid grid)
	{
		List<Material> materials = [];
		Dictionary<MaterialFields, int> indexes = grid.GetColumnIndexes();
		foreach (var item in grid.Items)
		{
			if (item is not DataRowView row)
				continue;
			Material m = new();
			if (indexes[MaterialFields.Name] is not -1)
				m.Name = Convert.ToString(row[indexes[MaterialFields.Name]]);
			if (indexes[MaterialFields.Description] is not -1)
				m.Description = Convert.ToString(row[indexes[MaterialFields.Description]]);
			if (indexes[MaterialFields.DefaultCost] is not -1)
				m.DefaultCost = Convert.ToDecimal(row[indexes[MaterialFields.DefaultCost]]);
			if (indexes[MaterialFields.SellPrice] is not -1)
				m.SellPrice = Convert.ToDecimal(row[indexes[MaterialFields.SellPrice]]);
			if (indexes[MaterialFields.Width] is not -1)
				m.Width = Convert.ToDouble(row[indexes[MaterialFields.Width]]) * (Settings.Unit == CSVUnit.Metric ? 1 : 25.4);
			if (indexes[MaterialFields.Length] is not -1)
				m.Length = Convert.ToDouble(row[indexes[MaterialFields.Length]]) * (Settings.Unit == CSVUnit.Metric ? 1 : 25.4);
			if (indexes[MaterialFields.Thickness] is not -1)
				m.Thickness = Convert.ToDouble(row[indexes[MaterialFields.Thickness]]) * (Settings.Unit == CSVUnit.Metric ? 1 : 25.4);
			materials.Add(m);
		}

		return materials;
	}

	/// <summary>
	/// Return a field and it's matching index. Indexes are initialized to -1.
	/// </summary>
	/// <param name="grid"></param>
	/// <returns></returns>
	private static Dictionary<MaterialFields, int> GetColumnIndexes(this DataGrid grid)
	{
		// Initialize the Dictionary with -1 for skipping in case not all values are provided.
		Dictionary<MaterialFields, int> dict = new()
		{
			{ MaterialFields.Name, -1 },
			{ MaterialFields.Description, -1 },
			{ MaterialFields.DefaultCost, -1 },
			{ MaterialFields.SellPrice, -1 },
			{ MaterialFields.Width, -1 },
			{ MaterialFields.Length, -1 },
			{ MaterialFields.Thickness, -1 }
		};

		int index = 0;
		foreach (var column in grid.Columns)
		{
			if (column.Header is ComboBox box)
			{
				string selectedText = (string)box.SelectedItem;
				dict[Enum.Parse<MaterialFields>(selectedText)] = index;
			}
			index++;
		}
		return dict;
	}
}
