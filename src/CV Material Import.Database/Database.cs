using System.Data;
using System.Text;
using System.Windows;

using Microsoft.Data.SqlClient;

namespace CV_Material_Import.Database;

/// <summary>
/// Represents a database object.
/// </summary>
public class Database
{
	private readonly string _connectionString;
	private readonly SqlConnection _connection;
	private readonly SqlCommand _command;

	public bool IsConnected => _connection.State == ConnectionState.Open;

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
		_connection?.Dispose();
		_command?.Dispose();
	}

	/// <summary>
	/// Execute sql text against the database.
	/// </summary>
	/// <param name="sql">The sql command text.</param>
	/// <returns>The number of rows affected of -1 if the command did not succeed.</returns>
	public async Task<int> Execute(string sql)
	{
		_command.CommandText = sql;
		_command.CommandType = CommandType.Text;
		try
		{
			return await _command.ExecuteNonQueryAsync();
		}
		catch (Exception ex)
		{
			StringBuilder sb = new($"The sql command \"{sql}\" did not execute successfully:");
			sb.AppendLine(ex.Message);
			MessageBox.Show(sb.ToString(), "Command Execution Error");
			return -1;
		}
	}

	private void Connect()
	{
		try
		{
			_connection.Open();
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message, "Error Opening Database");
		}
	}

}

