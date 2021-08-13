using System.Collections.Generic;
using ClickHouse.NetCore.Entities;

namespace ClickHouse.NetCore
{
    public interface IClickHouseCommandFormatter
    {
        /// <summary>
        /// Check table exists
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        string TableExists(string databaseName, string tableName);

        /// <summary>
        /// Check database exists
        /// </summary>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        string DatabaseExists(string databaseName);

        /// <summary>
        /// Compose a query that performs bulk insert
        /// </summary>
        /// <param name="bulk"></param>
        /// <returns></returns>
        (List<object[]>, List<string> columns) BulkInsert<T>(IEnumerable<T> bulk);

        /// <summary>
        /// Compose a query that creates a table
        /// </summary>
        /// <param name="database"></param>
        /// <param name="table">table schema definition</param>
        /// <param name="options">create options</param>
        /// <returns></returns>
        string CreateTable(string database, Table table, CreateOptions options = null);
        
        /// <summary>
        /// Compose a query that creates an empty database
        /// </summary>
        /// <param name="databaseName">database name</param>
        /// <param name="options">create options</param>
        /// <returns></returns>
        string CreateDatabase(string databaseName, CreateOptions options = null);

        /// <summary>
        /// Compose a query that drops a table
        /// </summary>
        /// <param name="name">table name</param>
        /// <param name="options">drop options</param>
        /// <returns></returns>
        string DropTable(string name, DropOptions options = null);

        /// <summary>
        /// Compose a query DESCRIBE TABLE
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string DescribeTable(string name);
    }
}