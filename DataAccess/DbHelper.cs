using System;
using System.Data;
using System.Data.SqlClient;

namespace DataAccess
{
    public class DBHelper
    {
        private string connectionString;

        public DBHelper(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        public async Task OpenConnectionAsync(SqlConnection connection)
        {
            try
            {
                await connection.OpenAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening connection: {ex.Message}");
                throw;
            }
        }
        public async Task<int> ExecuteNonQueryAsync(string query, SqlParameter[] parameters)
        {
            using (var connection = GetConnection())
            {
                await OpenConnectionAsync(connection);

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddRange(parameters);
                    int affectedRows = await command.ExecuteNonQueryAsync();
                    return affectedRows;
                }
            }
            // Not: CloseConnection çağırılmadı çünkü using bloğu bağlantıyı zaten kapatacak.
        }
        public void CloseConnection(SqlConnection connection)
        {
            try
            {
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error closing connection: {ex.Message}");
                throw;
            }
        }

        public async Task<DataTable> SelectAsync(string query)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand(query, connection))
            using (var adapter = new SqlDataAdapter(command))
            {
                var dataTable = new DataTable();
                await OpenConnectionAsync(connection);
                adapter.Fill(dataTable);
                CloseConnection(connection);
                return dataTable;
            }
        }

        public async Task<DataTable> SelectAsyncSafe(string query, SqlParameter[] parameters = null)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand(query, connection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                using (var adapter = new SqlDataAdapter(command))
                {
                    var dataTable = new DataTable();
                    await OpenConnectionAsync(connection);
                    adapter.Fill(dataTable);
                    CloseConnection(connection);
                    return dataTable;
                }
            }
        }
    }
}
