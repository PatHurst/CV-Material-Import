using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;

using Microsoft.Data.SqlClient;

namespace CV_Material_Import.Database;

/// <summary>
/// Represents a database object.
/// </summary>
public class Database
{
	private string _connectionString;
	private SqlConnection _connection;
	private SqlCommand _command;

	/// <summary>
	/// Database connection state.
	/// </summary>
	public bool IsConnected => _connection.State == ConnectionState.Open;

	/// <summary>
	/// Invoked when the connection changes.
	/// </summary>
	public event EventHandler<bool>? ConnectionChanged;

	/// <summary>
	/// Create an SQL database from a connection string.
	/// </summary>
	/// <param name="connString">The connection string to connect to the database</param>
	public Database(string connString)
	{
		_connectionString = connString;
		_connection = new SqlConnection(connString);
		_command = new SqlCommand()
		{
			Connection = _connection
		};
		Connect();
	}

	/// <summary>
	/// Create an SQL database from server and database name.
	/// </summary>
	/// <param name="serverName">The server name. eg. {Machine Name}\CV</param>
	/// <param name="databaseName">The database name. eg. CVData_2023</param>
	public Database(string serverName, string databaseName)
	{
		_connectionString = $"Server={serverName};Database={databaseName};Trusted_Connection=True;TrustServerCertificate=True";
		_connection = new SqlConnection(_connectionString);
		_command = new SqlCommand()
		{
			Connection = _connection
		};
		Connect();
	}

	~Database()
	{
		ConnectionChanged?.Invoke(this, false);
		if (IsConnected)
			_connection?.Close();
		_connection?.Dispose();
		_command?.Dispose();
	}

	/// <summary>
	/// Reconnect the database with a new connection string.
	/// </summary>
	/// <param name="connString"></param>
	public void ChangeConnectionn(string connString)
	{
		_connectionString = connString;
		_connection = new SqlConnection(connString);
		_command = new SqlCommand()
		{
			Connection = _connection
		};
		Connect();
	}

	/// <summary>
	/// Execute sql text against the database.
	/// </summary>
	/// <param name="sql">The sql command text.</param>
	/// <returns>The number of rows affected of -1 if the command did not succeed.</returns>
	public int Execute(string sql)
	{
		if (!IsConnected)
		{
			MessageBox.Show("Connection is Closed!");
			return -1;
		}
		_command.CommandText = sql;
		_command.CommandType = CommandType.Text;
		try
		{
			return _command.ExecuteNonQuery();
		}
		catch (Exception ex)
		{
			StringBuilder sb = new($"The sql command \"{sql}\" did not execute successfully:");
			sb.AppendLine(ex.Message);
			MessageBox.Show(sb.ToString(), "Command Execution Error");
			return -1;
		}
	}

	public T ExecuteScalar<T>(string sql)
	{
		if (!IsConnected)
		{
			MessageBox.Show("Connection is Closed!");
			return default!;
		}
		_command.CommandText = sql;
		_command.CommandType = CommandType.Text;
		try
		{
			return (T)_command.ExecuteScalar();
		}
		catch (Exception ex)
		{
			StringBuilder sb = new($"The sql command \"{sql}\" did not execute successfully:");
			sb.AppendLine(ex.Message);
			MessageBox.Show(sb.ToString(), "Command Execution Error");
			return default!;
		}
	}

	public Dictionary<string, int> GetMaterialFolders()
	{
		Dictionary<string, int> folders = [];
		_command.CommandText = "SELECT TOP(1000)[ID], [Name] FROM[CVData_2023].[dbo].[MaterialMenuTree] WHERE[ParentID] = 0";
		using SqlDataReader reader = _command.ExecuteReader();
		while (reader.Read())
		{
			folders.TryAdd((string)reader["Name"], Convert.ToInt32(reader["ID"]));
		}
		return folders;
	}

	private void Connect()
	{
		try
		{
			_connection.Open();
			ConnectionChanged?.Invoke(this, true);
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message, "Error Opening Database");
			ConnectionChanged?.Invoke(this, false);
		}
	}

}

