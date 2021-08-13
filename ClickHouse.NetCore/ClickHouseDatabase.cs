using ClickHouse.Client.ADO;
using ClickHouse.Client.Copy;
using ClickHouse.NetCore.Entities;
using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace ClickHouse.NetCore
{
    public class ClickHouseDatabase : IClickHouseDatabase
    {
        private readonly ClickHouseConnection _clickHouseConnection;
        private readonly IClickHouseCommandFormatter _commandFormatter;
        
        public ClickHouseDatabase(ClickHouseConnection clickHouseConnection, IClickHouseCommandFormatter commandFormatter)
        {
            _clickHouseConnection = clickHouseConnection;
            _commandFormatter = commandFormatter;
        }

        public async Task<bool> TableExists(string tableName)
        {
            var commandText = _commandFormatter.TableExists(_clickHouseConnection.Database, tableName);
            var result = await ExecuteExists(commandText);
            return result;
        }

        public async Task CreateTable(Table table, CreateOptions options = null)
        {
            var commandText = _commandFormatter.CreateTable(_clickHouseConnection.Database, table, options);
            await ExecuteNonQueryAsync(commandText);
        }

        public async Task CreateDatabase(Database db, CreateOptions options = null)
        {
            await CreateDatabase(db.Name, options);
            foreach (var table in db.Tables)
            {
                await CreateTable(table, options);
            }
        }

        public async Task CreateDatabase(string databaseName, CreateOptions options = null)
        {
            var commandText = _commandFormatter.CreateDatabase(databaseName, options);
            await ExecuteNonQueryAsync(commandText);
        }

        public async Task<bool> DatabaseExists(string databaseName)
        {
            var commandText = _commandFormatter.DatabaseExists(databaseName);
            var result = await ExecuteExists(commandText);
            return result;
        }

        public async Task DropTable(string tableName, DropOptions options = null)
        {
            var commandText = _commandFormatter.DropTable(tableName, options);
            await ExecuteNonQueryAsync(commandText);
        }
        
        public async Task<IEnumerable<T>> ExecuteQueryMapping<T>(string query) where T : new()
        {
            return await _clickHouseConnection.QueryAsync<T>(query);
        }

        public async Task<long> BulkInsert<T>(string tableName, List<T> bulk)
        {
            using var bulkCopyInterface = new ClickHouseBulkCopy(_clickHouseConnection)
            {
                DestinationTableName = tableName,
                BatchSize = bulk.Count
            };
            var (items, columns) = _commandFormatter.BulkInsert(bulk);
            await bulkCopyInterface.WriteToServerAsync(items, columns);
            return bulkCopyInterface.RowsWritten;
        }

        public async Task ExecuteNonQueryAsync(string sqlQuery)
        {
            await _clickHouseConnection.ExecuteAsync(sqlQuery);
        }

        public async Task<bool> ExecuteExists(string sqlQuery)
        {
            return (ulong?)await _clickHouseConnection.ExecuteScalarAsync(sqlQuery) > 0;
        }

        public async Task<ulong> ExecuteCountExists(string sqlQuery)
        {
            var count = await _clickHouseConnection.ExecuteScalarAsync(sqlQuery);
            return (ulong)count;
        }
    }
}