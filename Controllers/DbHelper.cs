using Npgsql;
using OTILib.Util;
using System.Data;

namespace rest1.Controllers
{
    public class DbHelper
    {
        private readonly string _connStr;

        public DbHelper(IConfiguration config)
        {
            _connStr = config.GetConnectionString("DefaultConnection");
        }

        private NpgsqlConnection GetConnection()
        {
            return DbTransactionManager.Connection ?? new NpgsqlConnection(_connStr);
        }

        private NpgsqlTransaction? GetTransaction()
        {
            return DbTransactionManager.Current;
        }

        public DataTable? ExecuteSelect(string sql, Dictionary<string, object>? parameters = null)
        {
            var useTransaction = DbTransactionManager.Connection != null;
            using var conn = useTransaction ? null : new NpgsqlConnection(_connStr);
            var connection = conn ?? DbTransactionManager.Connection!;
            try
            {
                if (!useTransaction) conn.Open();
                using var cmd = new NpgsqlCommand(sql, connection);
                if (GetTransaction() != null) cmd.Transaction = GetTransaction();

                if (parameters != null)
                {
                    foreach (var param in parameters)
                        cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }

                using var adapter = new NpgsqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
            finally
            {
                if (!useTransaction) connection.Close();
            }
        }

        public DataTable? ExecuteSelect(string sql, object? parameters = null)
        {
            var useTransaction = DbTransactionManager.Connection != null;
            using var conn = useTransaction ? null : new NpgsqlConnection(_connStr);
            var connection = conn ?? DbTransactionManager.Connection!;
            try
            {
                if (!useTransaction) conn.Open();
                using var cmd = new NpgsqlCommand(sql, connection);
                if (GetTransaction() != null) cmd.Transaction = GetTransaction();

                if (parameters != null)
                {
                    var props = parameters.GetType().GetProperties();
                    foreach (var prop in props)
                    {
                        var name = prop.Name;
                        var value = prop.GetValue(parameters) ?? DBNull.Value;
                        cmd.Parameters.AddWithValue(name, value);
                    }
                }

                using var adapter = new NpgsqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
            finally
            {
                if (!useTransaction) connection.Close();
            }
        }

        public int ExecuteNonQuery(string sql, Dictionary<string, object>? parameters = null)
        {
            var useTransaction = DbTransactionManager.Connection != null;
            using var conn = useTransaction ? null : new NpgsqlConnection(_connStr);
            var connection = conn ?? DbTransactionManager.Connection!;
            try
            {
                if (!useTransaction) conn.Open();
                using var cmd = new NpgsqlCommand(sql, connection);
                if (GetTransaction() != null) cmd.Transaction = GetTransaction();

                if (parameters != null)
                {
                    foreach (var param in parameters)
                        cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }

                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return -1;
            }
            finally
            {
                if (!useTransaction) connection.Close();
            }
        }

        public int ExecuteNonQuery(string sql, object? parameters = null)
        {
            var useTransaction = DbTransactionManager.Connection != null;
            using var conn = useTransaction ? null : new NpgsqlConnection(_connStr);
            var connection = conn ?? DbTransactionManager.Connection!;
            try
            {
                if (!useTransaction) conn.Open();
                using var cmd = new NpgsqlCommand(sql, connection);
                if (GetTransaction() != null) cmd.Transaction = GetTransaction();

                if (parameters != null)
                {
                    var props = parameters.GetType().GetProperties();
                    foreach (var prop in props)
                    {
                        var name = prop.Name;
                        var value = prop.GetValue(parameters) ?? DBNull.Value;
                        cmd.Parameters.AddWithValue(name, value);
                    }
                }

                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                OtiLogger.log1(ex);
                return -1;
            }
            finally
            {
                if (!useTransaction) connection.Close();
            }
        }
    }
}
