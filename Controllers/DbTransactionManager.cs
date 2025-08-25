using Npgsql;

namespace rest1.Controllers
{
    public class DbTransactionManager
    {
        [ThreadStatic]
        private static NpgsqlTransaction? _currentTransaction;

        [ThreadStatic]
        private static NpgsqlConnection? _connection;

        [ThreadStatic]
        private static int _transactionDepth;

        private static string? _connStr;

        public static void Initialize(string connStr)
        {
            _connStr = connStr;
        }

        public static void Begin(bool requiresNew = false)
        {
            if (requiresNew || _currentTransaction == null)
            {
                _connection = new NpgsqlConnection(_connStr);
                _connection.Open();
                _currentTransaction = _connection.BeginTransaction();
                _transactionDepth = 1;
            }
            else
            {
                _transactionDepth++;
            }
        }

        public static void Commit()
        {
            if (--_transactionDepth == 0)
            {
                _currentTransaction?.Commit();
                _connection?.Close();
                _currentTransaction = null;
                _connection = null;
            }
        }

        public static void Rollback()
        {
            if (_transactionDepth > 0)
            {
                _currentTransaction?.Rollback();
                _transactionDepth = 0;
                _connection?.Close();
                _currentTransaction = null;
                _connection = null;
            }
        }

        public static NpgsqlTransaction? Current => _currentTransaction;
        public static NpgsqlConnection? Connection => _connection;
    }
}
