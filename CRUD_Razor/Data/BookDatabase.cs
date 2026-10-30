using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using MySqlConnector;

namespace CRUD_Razor.Data;

public static class BookDatabase
{
    public static bool UsesMySql(IConfiguration configuration) =>
        string.Equals(configuration["Database:Provider"], "MySql", StringComparison.OrdinalIgnoreCase);

    public static DbConnection CreateConnection(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        return UsesMySql(configuration)
            ? new MySqlConnection(connectionString)
            : new SqlConnection(connectionString);
    }

    public static DbCommand CreateCommand(DbConnection connection, string sql)
    {
        var command = connection.CreateCommand();
        command.CommandText = connection is MySqlConnection
            ? sql.Replace("dbo.books", "books").Replace("CHARINDEX(", "LOCATE(")
            : sql;
        return command;
    }

    public static void AddParameter(this DbCommand command, string name, DbType type, object? value, int size = 0)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.DbType = type;
        if (size > 0)
            parameter.Size = size;
        parameter.Value = value ?? DBNull.Value;
        command.Parameters.Add(parameter);
    }
}
