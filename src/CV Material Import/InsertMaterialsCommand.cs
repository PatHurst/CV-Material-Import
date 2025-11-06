using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CV_Material_Import;

internal class InsertMaterialsCommand
{
	private IEnumerable<Material> _materials = [];
	private int _folderID;

	public InsertMaterialsCommand(IEnumerable<Material> materials, int folderID)
	{
		_materials = materials;
		_folderID = folderID;
	}

	public int Execute()
	{
		int countOfRowsInserted = 0;
		foreach (Material material in _materials)
		{
			string insertCommand =
				@"INSERT INTO [Material] ([Name], [Description], [DefaultCost], [SellPrice], [rGUID]) " +
				$"VALUES ('{material.Name}', '{material.Description}', {material.DefaultCost}, {material.SellPrice}, NEWID()); ";
			if (App.Database.Execute(insertCommand) is -1)
			{
				if (MessageBox.Show("Cancel Operation?", "Cancel", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
					return countOfRowsInserted;
				countOfRowsInserted--;
			}

			long matID = App.Database.ExecuteScalar<long>("Select [ID] From [Material] Order By [ID] Desc");

			if (matID == 0)
				continue;

			insertCommand =
				$"Insert Into MaterialExtraSizeInfo ([MaterialID], [Width], [Length], [Thickness]) " +
				$"Values ({matID}, {material.Width}, {material.Length}, {material.Thickness}); " +
				$"Insert Into MaterialExtraCNCInfo ([MaterialID]) Values ({matID}); " +
				$"INSERT INTO layer ([Name], [Description], [MaterialID], [LayerTypeID]) " +
				$"VALUES ('Face', 'Face', {matID}, 1), ('Back', 'Back', {matID}, 2), ('Edge', 'Edge', {matID}, 3), ('End', 'End', {matID}, 4); " + 
				$"Insert Into [MaterialMenuTreeItem] ([MenuID], [MenuTreeID], [MaterialID]) Values (1, {_folderID}, {matID}) ";

			if (App.Database.Execute(insertCommand) is -1)
				if (MessageBox.Show("Cancel Operation?", "Cancel", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
					return countOfRowsInserted;

			countOfRowsInserted++;
		}
		return countOfRowsInserted;
	}
}
