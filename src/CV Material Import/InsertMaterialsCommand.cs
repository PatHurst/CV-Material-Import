using System.Data.Common;
using System.Windows;

using Microsoft.Data.SqlClient;

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

	/// <summary>
	///		Inserts the materials into the database.
	/// </summary>
	/// <remarks>
	///		If a material is already in the database, ask the user whether he wants to cancel or update the existing material.
	/// </remarks>
	/// <returns></returns>
	public int Execute()
	{
		int countOfRowsInserted = 0;
		foreach (Material material in _materials)
		{
			bool update = false;
			string insertCommand =
				@"INSERT INTO [Material] ([Name], [Description], [DefaultCost], [SellPrice], [rGUID]) " +
				$"VALUES ('{material.Name}', '{material.Description}', {material.DefaultCost}, {material.SellPrice}, NEWID()); ";
			try
			{
				App.Database.Execute(insertCommand);
			}
			// Catch when the material already exists in the database. Ask they user how they want to proceed.
			catch (SqlException ex) when (ex.Number == 2601)
			{
				var result = MessageBox.Show($"You already have a material named {material.Name}!\r\nWould you like to overwrite it?", "Duplicate Material!",
					MessageBoxButton.YesNo, MessageBoxImage.Warning);
				switch (result)
				{
					// if they don't want to update, simply move on to the next material.
					case MessageBoxResult.No:
						continue;
					// if they do want to update, run an update command instead and set the update flag to true for later reference.
					case MessageBoxResult.Yes:
					{
						string updateCommand =
							@"UPDATE [Material] " +
							$"SET [Description] = '{material.Description}', " +
							$"    [DefaultCost] = {material.DefaultCost}, " +
							$"    [SellPrice] = {material.SellPrice} " +
							$"WHERE [Name] = '{material.Name}';";
						App.Database.Execute(updateCommand);
						update = true;
						break;
					}

				}
			}
			// All other exceptions.
			catch (SqlException ex)
			{
				if (MessageBox.Show(ex.Message + "\r\nCancel all insertions?", "Cancel", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
					return countOfRowsInserted;
				countOfRowsInserted--;
			}

			long matID = update switch
			{
				true => App.Database.ExecuteScalar<long>($"Select TOP 1 [ID] From [Material] WHERE [Name] = '{material.Name}';"),
				false => App.Database.ExecuteScalar<long>("Select TOP 1 [ID] From [Material] Order By [ID] Desc")
			};

			insertCommand = update switch
			{
				true =>
					$"UPDATE MaterialExtraSizeInfo " +
					$"SET [Width] = {material.Width}, " +
					$"    [Length] = {material.Length}, " +
					$"    [Thickness] = {material.Thickness} " +
					$"WHERE [MaterialID] = {matID};",
				false =>
					$"Insert Into MaterialExtraSizeInfo ([MaterialID], [Width], [Length], [Thickness]) " +
					$"Values ({matID}, {material.Width}, {material.Length}, {material.Thickness}); " +
					$"Insert Into MaterialExtraCNCInfo ([MaterialID]) Values ({matID}); " +
					$"INSERT INTO layer ([Name], [Description], [MaterialID], [LayerTypeID]) " +
					$"VALUES ('Face', 'Face', {matID}, 1), ('Back', 'Back', {matID}, 2), ('Edge', 'Edge', {matID}, 3), ('End', 'End', {matID}, 4); " +
					$"Insert Into [MaterialMenuTreeItem] ([MenuID], [MenuTreeID], [MaterialID]) Values (1, {_folderID}, {matID}); ",
			};

			try
			{
				App.Database.Execute(insertCommand);
			}
			catch (SqlException ex)
			{
				MessageBox.Show("An error occurred inserting material info:\r\n" + ex.Message);
			}
				
			countOfRowsInserted++;
		}
		return countOfRowsInserted;
	}
}
