using System;
using MySql.Data.MySqlClient;
using System.Data;

public class Database
{
    // Static property for the connection string
    string connectionString = "Server=localhost;Port=3306;Database=real_state_db;User Id=root;Password=root;";

    // Method to open a connection
    private MySqlConnection GetConnection()
    {
        return new MySqlConnection(connectionString);
    }

    // Method to execute a query (INSERT, UPDATE, DELETE)
    public int ExecuteNonQuery(string query, params MySqlParameter[] parameters)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            using (MySqlCommand command = new MySqlCommand(query, connection))
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
    public DataTable ExecuteQuery(string query, params MySqlParameter[] parameters)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            }
        }
    }

    // Method to execute a scalar query (e.g., get single value)
    public object ExecuteScalar(string query, params MySqlParameter[] parameters)
    {
        using (MySqlConnection connection = GetConnection())
        {
            connection.Open();
            using (MySqlCommand command = new MySqlCommand(query, connection))
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
