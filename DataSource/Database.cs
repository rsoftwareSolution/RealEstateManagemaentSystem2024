using System;
using System.Data;
using System.Data.SqlClient;

public class Database
{
    // Connection string (replace with your actual credentials)
    private readonly string connectionString;

    // Static connection parameters (can be set directly)
    private static string Server = "DESKTOP-S7NF3LU";
    private static string DatabaseName = "real_state_db";
    private static string Username = "root";
    private static string Password = "root";

    // Static property for the connection string
    private static string ConnectionString =>
        $"Server={Server};Database={DatabaseName};User Id={Username};Password={Password};";

    // Method to open a connection
    private SqlConnection GetConnection()
    {
        return new SqlConnection(connectionString);
    }

    // Method to execute a query (INSERT, UPDATE, DELETE)
    public int ExecuteNonQuery(string query, params SqlParameter[] parameters)
    {
        using (SqlConnection connection = GetConnection())
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                return command.ExecuteNonQuery(); // Returns rows affected
            }
        }
    }

    // Method to execute a query and return a DataTable (for SELECT)
    public DataTable ExecuteQuery(string query, params SqlParameter[] parameters)
    {
        using (SqlConnection connection = GetConnection())
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            }
        }
    }

    // Method to execute a scalar query (e.g., get single value)
    public object ExecuteScalar(string query, params SqlParameter[] parameters)
    {
        using (SqlConnection connection = GetConnection())
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                return command.ExecuteScalar(); // Returns the first column of the first row
            }
        }
    }
}
