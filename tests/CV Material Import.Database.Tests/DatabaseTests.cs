using CV_Material_Import.Database;

namespace CV_Material_Import.Database.Tests;

[TestClass]
public sealed class DatabaseTests
{
	private readonly string _validConnectionString = @"Server=PATRICK_DESKTOP\CV;Database=CVData_2023;Trusted_Connection=True;TrustServerCertificate=True";

	[TestMethod]
	public void AssertConnectionSucceedsWithValidConnectionString()
	{
		Database db = new(_validConnectionString);
		Assert.IsTrue(db.IsConnected);
	}

	[TestMethod]
	public void AssertConnectionSucceedsWithValidCredentials()
	{
		Database db = new(@"PATRICK_DESKTOP\CV", "CVData_2023");
		Assert.IsTrue(db.IsConnected);
	}

	[TestMethod]
	public void CanExecuteValidSQL()
	{
		Database db = new(_validConnectionString);

		db.Execute("IF OBJECT_ID('test_table', 'U') IS NULL BEGIN CREATE TABLE test_table (number INT) END;").Wait();

		Assert.AreEqual(5, db.Execute("INSERT INTO test_table (number) VALUES (1), (2), (3), (4), (5);").Result);
		Assert.AreEqual(1, db.Execute("DELETE FROM test_table WHERE [number] = 5").Result);

		db.Execute("DROP TABLE test_table;").Wait();
	}

	[TestMethod]
	public void ReturnsNegativeOneOnInvalidSQL()
	{
		Database db = new(_validConnectionString);

		Assert.IsTrue(db.Execute("Invalid SQL").Result == -1);
	}
}
