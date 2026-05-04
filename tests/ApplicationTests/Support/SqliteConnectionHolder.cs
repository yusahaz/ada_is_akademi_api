namespace Azoxia.AdaIsAkademi.Application.Tests.Support
{
    using Microsoft.Data.Sqlite;

    /// <summary>
    /// Owns an open SQLite connection so <see cref="Microsoft.Extensions.DependencyInjection.ServiceProvider"/> disposal closes the shared in-memory database.
    /// </summary>
    internal sealed class SqliteConnectionHolder :
        IDisposable
    {
        #region Fields

        private readonly SqliteConnection _connection;

        #endregion Fields

        #region Ctors

        /// <summary>
        /// Opens a new shared-cache in-memory SQLite database.
        /// </summary>
        public SqliteConnectionHolder()
        {
            string dataSource = "test_" + Guid.NewGuid().ToString("N");
            _connection = new SqliteConnection($"Data Source={dataSource};Mode=Memory;Cache=Shared");
            _connection.Open();
        }

        #endregion Ctors

        #region Properties

        /// <summary>
        /// Gets the open connection passed to <c>UseSqlite</c>.
        /// </summary>
        public SqliteConnection Connection => _connection;

        #endregion Properties

        #region Methods

        /// <inheritdoc />
        public void Dispose()
            => _connection.Dispose();

        #endregion Methods
    }
}
