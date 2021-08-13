using ClickHouse.NetCore.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClickHouse.NetCore
{
    /// <summary>
    /// Интерфес управления структурой базы данных
    /// </summary>
    public interface IClickHouseDatabase
    {
        /// <summary>
        /// Create a new database
        /// </summary>
        /// <param name="db">Object representing database structure</param>
        /// <param name="options"></param>
        Task CreateDatabase(Database db, CreateOptions options = null);

        /// <summary>
        /// Create a new empty database
        /// </summary>
        /// <param name="databaseName">database name</param>
        /// <param name="options">create options</param>
        Task CreateDatabase(string databaseName, CreateOptions options = null);

        /// <summary>
        /// Проверяет существование базы данных
        /// </summary>
        /// <param name="databaseName"></param>
        Task<bool> DatabaseExists(string databaseName);

        /// <summary>
        /// Check if table exists in current database
        /// </summary>
        /// <param name="tableName">Table name</param>
        Task<bool> TableExists(string tableName);

        /// <summary>
        /// Drop a table
        /// </summary>
        /// <param name="tableName">table name</param>
        /// <param name="options">drop options</param>
        Task DropTable(string tableName, DropOptions options = null);
       
        /// <summary>
        /// Execute a query that produces no results
        /// </summary>
        /// <param name="commandText">Text of query</param>
        Task ExecuteNonQueryAsync(string commandText);

        /// <summary>
        /// Execute a query that returns a count
        /// </summary>
        /// <param name="commandText">Text of query</param>
        Task<ulong> ExecuteCountExists(string commandText);

        /// <summary>
        /// Bulk insert rows into table
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="tableName">Table name</param>
        /// <param name="bulk">Data to insert</param>
        Task<long> BulkInsert<T>(string tableName, List<T> bulk);

        /// <summary>
        /// Create a new table
        /// </summary>
        /// <param name="table">table schema</param>
        /// <param name="options">create options</param>
        Task CreateTable(Table table, CreateOptions options = null);

        /// <summary>
        /// Execute a query that maps its result to object collection
        /// </summary>
        /// <typeparam name="T">object type</typeparam>
        /// <param name="query">query text</param>
        /// <returns></returns>
        Task<IEnumerable<T>> ExecuteQueryMapping<T>(string query) where T : new();
    }
}