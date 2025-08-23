using Npgsql;

namespace rest1.Controllers
{
    public class DbTransactionManager
    {
        [ThreadStatic]
        private static NpgsqlTransaction? _currentTransaction;

        [ThreadStatic]
        private static NpgsqlConnection? _connection;

        private static string? _connStr;

        public static void Initialize(string connStr)
        {
            _connStr = connStr;
        }

        public static void Begin()
        {
            _connection = new NpgsqlConnection(_connStr);
            _connection.Open();
            _currentTransaction = _connection.BeginTransaction();
        }

        public static void Commit()
        {
            _currentTransaction?.Commit();
            _connection?.Close();
        }

        public static void Rollback()
        {
            _currentTransaction?.Rollback();
            _connection?.Close();
        }

        public static NpgsqlTransaction? Current => _currentTransaction;
        public static NpgsqlConnection? Connection => _connection;
    }
}
