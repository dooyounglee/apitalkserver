using Npgsql;
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

        public DataTable? ExecuteSelect(string sql, Dictionary<string, object>? parameters = null)
        {
            using var conn = new NpgsqlConnection(_connStr);
            try
            {
                conn.Open();
                using var cmd = new NpgsqlCommand(sql, conn);

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
        }

        public DataTable? ExecuteSelect(string sql, object? parameters = null)
        {
            using var conn = new NpgsqlConnection(_connStr);
            try
            {
                conn.Open();
                using var cmd = new NpgsqlCommand(sql, conn);

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
        }

        public int ExecuteNonQuery(string sql, Dictionary<string, object>? parameters = null)
        {
            using var conn = new NpgsqlConnection(_connStr);
            try
            {
                conn.Open();
                using var cmd = new NpgsqlCommand(sql, conn);

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
        }

        public int ExecuteNonQuery(string sql, object? parameters = null)
        {
            using var conn = new NpgsqlConnection(_connStr);
            try
            {
                conn.Open();
                using var cmd = new NpgsqlCommand(sql, conn);

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
                return -1;
            }
        }
    }
}
