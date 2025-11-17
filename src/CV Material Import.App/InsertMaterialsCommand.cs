using System.Data.Common;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

using Microsoft.Data.SqlClient;

namespace CV_Material_Import;

/// <summary>
///		Command handles the insertion of materials into CV.
/// </summary>
internal class InsertMaterialsCommand : ICommand
{
	private Func<IEnumerable<Material>> _getMaterials;
	private Func<int> _getFolderID;
	private int _totalMaterials;

	/// <summary>
	///		Create a new instance of the InsertMaterialsCommand class.
	/// </summary>
	/// <remarks>
	///		The material list and folder ID are passed as functions to retrieve the values rather than the actual values.
	///		This is because this constructor is called when the ICommand is bound to the button. We need to pass the values as functions, or this constructor would be called with 
	///		null or empty data.
	/// </remarks>
	/// <param name="getMaterials">The function to return the list of the materials to be inserted.</param>
	/// <param name="getFolderID">The function to return the database ID of the folder in which they will be inserted.</param>
	public InsertMaterialsCommand(Func<IEnumerable<Material>> getMaterials, Func<int> getFolderID)
	{
		_getMaterials = getMaterials;
		_getFolderID = getFolderID;
	}

	/// <summary>
	///		Inserts the materials into the database.
	/// </summary>
	/// <remarks>
	///		If a material is already in the database, ask the user whether he wants to cancel or update the existing material.
	/// </remarks>
	/// <param name="obj">Ignored</param>
	public void Execute(object? obj)
	{
		var _materials = _getMaterials();
		var _folderID = _getFolderID();
		_totalMaterials = _materials.Count();

		int countOfRowsInserted = 0;
		bool repeatForAllItems = false, update = false;
		foreach (Material material in _materials)
		{
			// if [repeatForAllItems] is true, use the value of [update] from the last run. Else [update] will be set to false.
			update = repeatForAllItems && update;
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
				if (!repeatForAllItems)
				{
					OverwriteMaterialsWindow window = new(material.Name ?? "Null Material");
					window.ShowDialog(ref update, ref repeatForAllItems);
				}

				//var result = MessageBox.Show($"You already have a material named {material.Name}!\r\nWould you like to overwrite it?", "Duplicate Material!",
				//	MessageBoxButton.YesNo, MessageBoxImage.Warning);

				switch (update)
				{
					// if they don't want to update, simply move on to the next material.
					case false:
						continue;
					// if they do want to update, run an update command instead and set the update flag to true for later reference.
					case true:
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
					CommandCompleted?.Invoke(this, countOfRowsInserted);
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
					$"	Values ({matID}, {material.Width}, {material.Length}, {material.Thickness}); " +
					$"Insert Into MaterialExtraCNCInfo ([MaterialID])" +
					$"	Values ({matID}); " +
					$"INSERT INTO layer ([Name], [Description], [MaterialID], [LayerTypeID]) " +
					$"	VALUES ('Face', 'Face', {matID}, 1), ('Back', 'Back', {matID}, 2), ('Edge', 'Edge', {matID}, 3), ('End', 'End', {matID}, 4); " +
					$"Insert Into [MaterialMenuTreeItem] ([MenuID], [MenuTreeID], [MaterialID])" +
					$"	Values (1, {_folderID}, {matID}); ",
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
			CommandProgressChanged?.Invoke(this, countOfRowsInserted / (float)_totalMaterials);
		}
		CommandCompleted?.Invoke(this, countOfRowsInserted);
	}

	public event EventHandler? CanExecuteChanged;

	/// <summary>
	///		Raise the CanExecuteChanged event.
	/// </summary>
	public void RaiseCanExecuteChanged()
	{
		CanExecuteChanged?.Invoke(this, EventArgs.Empty);
	}

	/// <summary>
	///		Invoked when the command has completed execution, returning the number of rows affected.
	/// </summary>
	public event EventHandler<int>? CommandCompleted;

	/// <summary>
	///		Invoked when the command's progress has changed, with the float value indicating percentage of completion.
	/// </summary>
	public event EventHandler<float>? CommandProgressChanged;

	/// <summary>
	///		Determines whether the materials grid is a valid candidate for insertion.
	/// </summary>
	/// <param name="parameter">The datagrid</param>
	/// <returns>A boolean indicating whether the Execute method can be invoked.</returns>
	public bool CanExecute(object? parameter)
	{
		if (parameter is null)
			return false;
		if (parameter is DataGrid grid)
		{
			if (grid.Items.Count == 0 || grid.Columns.Count == 0)
				return false;
			foreach (var column in grid.Columns)
			{
				if (column.Header is ComboBox box)
				{
					string selectedText = (string)box.SelectedItem;

					if (selectedText == "Ignore")
						continue;
					if (box.Foreground == Brushes.Red)
						return false;
				}
			}
		}
		return true; // this shouldn't  happen
	}
}
