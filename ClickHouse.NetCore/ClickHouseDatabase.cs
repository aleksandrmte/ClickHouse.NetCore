using ClickHouse.Client.ADO;
using ClickHouse.Client.ADO.Adapters;
using ClickHouse.Client.Copy;
using ClickHouse.NetCore.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;


namespace ClickHouse.NetCore
{
    public class ClickHouseDatabase : IClickHouseDatabase
    {
        private readonly ClickHouseConnection _clickHouseConnection;
        private readonly IClickHouseCommandFormatter _commandFormatter;
        private readonly IPropertyBinder _propertyBinder;

        public ClickHouseDatabase(ClickHouseConnection clickHouseConnection, IClickHouseCommandFormatter commandFormatter, IPropertyBinder propertyBinder)
        {
            _clickHouseConnection = clickHouseConnection;
            _commandFormatter = commandFormatter;
            _propertyBinder = propertyBinder;
        }

        public async Task<bool> TableExists(string tableName)
        {
            var commandText = _commandFormatter.TableExists(_clickHouseConnection.Database, tableName);
            var result = await Execute(async cmd => await ExecuteExists(cmd), commandText);
            return result;
        }

        public async Task CreateTable(Table table, CreateOptions options = null)
        {
            var commandText = _commandFormatter.CreateTable(_clickHouseConnection.Database, table, options);
            await Execute(cmd => cmd.ExecuteNonQueryAsync(), commandText);
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
            await Execute(cmd => cmd.ExecuteNonQueryAsync(), commandText);
        }

        public async Task<bool> DatabaseExists(string databaseName)
        {
            var commandText = _commandFormatter.DatabaseExists(databaseName);
            var result = await Execute(async cmd => await ExecuteExists(cmd), commandText);
            return result;
        }

        public async Task DropTable(string tableName, DropOptions options = null)
        {
            var commandText = _commandFormatter.DropTable(tableName, options);
            await Execute(async cmd => await cmd.ExecuteNonQueryAsync(), commandText);
        }
        
        public async Task<IEnumerable<T>> ExecuteQueryMapping<T>(string commandText, IColumnNamingConvention convention = null) where T : new()
        {
            var data = new List<T>();
            await Execute(async cmd =>
            {
                using var adapter = new ClickHouseDataAdapter();
                await using var command = cmd;

                command.CommandText = commandText;
                adapter.SelectCommand = command;

                var dataTable = new DataTable();
                adapter.Fill(dataTable);

                foreach (DataRow row in dataTable.Rows)
                {
                    var obj = new T();
                    for (var i = 0; i < dataTable.Columns.Count; i++)
                    {
                        var propertyName = convention?.GetPropertyName(dataTable.Columns[i].ColumnName) ?? dataTable.Columns[i].ColumnName;
                        _propertyBinder.BindProperty(obj, propertyName, row.ItemArray[i]);
                    }
                    data.Add(obj);
                }
            }, commandText);
            return data;
        }

        public async Task BulkInsert<T>(string tableName, List<T> bulk)
        {
            using var bulkCopyInterface = new ClickHouseBulkCopy(_clickHouseConnection)
            {
                DestinationTableName = tableName,
                BatchSize = bulk.Count
            };
            var items = _commandFormatter.BulkInsert(bulk);
            await bulkCopyInterface.WriteToServerAsync(items);
        }

        public async Task ExecuteNonQueryAsync(string commandText)
        {
            await Execute(cmd => cmd.ExecuteNonQueryAsync(), commandText);
        }

        public async Task<bool> ExecuteExists(ClickHouseCommand command)
        {
            return (ulong?)await command.ExecuteScalarAsync() > 0;
        }

        public async Task<ulong> ExecuteCountExists(string commandText)
        {
            var count = await Execute(async cmd => await cmd.ExecuteScalarAsync(), commandText);

            return (ulong)count;
        }

        public void Execute(Action<ClickHouseCommand> body, string commandText)
        {
            using var command = _clickHouseConnection.CreateCommand();
            command.CommandText = commandText;
            body(command);
        }

        private T Execute<T>(Func<ClickHouseCommand, T> body, string commandText)
        {
            var result = default(T);
            Execute(cmd =>
            {
                result = body(cmd);
            }, commandText);
            return result;
        }
    }
}