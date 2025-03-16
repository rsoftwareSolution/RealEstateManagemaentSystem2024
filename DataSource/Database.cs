using System;
using MySql.Data.MySqlClient;
using System.Data;

public class Database
{
    // Static property for the connection string
    string connectionString = "Server=localhost;Port=3306;Database=real_state_db;User Id=root;Password=root;";
    private MySqlConnection connection;
    private MySqlTransaction transaction; // Transaction object

    // Method to open a connection
    private MySqlConnection GetConnection()
    {
        return new MySqlConnection(connectionString);
    }

    // 🔹 Start a Transaction
    public void BeginTransaction()
    {
        if (connection.State != ConnectionState.Open)
        {
            connection.Open();
        }
        transaction = connection.BeginTransaction();
    }

    // 🔹 Commit the Transaction
    public void CommitTransaction()
    {
        transaction?.Commit();
        transaction = null;
        connection.Close();
    }

    // 🔹 Rollback the Transaction
    public void RollbackTransaction()
    {
        transaction?.Rollback();
        transaction = null;
        connection.Close();
    }

    // Method to execute a query (INSERT, UPDATE, DELETE)
    public int ExecuteNonQuery(string query, params MySqlParameter[] parameters)
    {
        using (connection = new MySqlConnection(connectionString))
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
        using (connection = new MySqlConnection(connectionString))
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
        using (connection = GetConnection())
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
