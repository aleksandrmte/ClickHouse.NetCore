using ClickHouse.NetCore.Entities;
using System.Collections.Generic;
using System.Linq;

namespace ClickHouse.NetCore
{
    public class ClickHouseCommandFormatter : IClickHouseCommandFormatter
    {
        public string TableExists(string databaseName, string tableName)
        {
            return $"SELECT COUNT(*) FROM system.tables WHERE database='{databaseName}' AND name='{tableName}'";
        }

        public string DatabaseExists(string databaseName)
        {
            return $"SELECT COUNT(*) FROM system.databases WHERE name='{databaseName}'";
        }

        public string CreateTable(string database, Table table, CreateOptions options = null)
        {           
            return Create("TABLE", Table(database, table), options);
        }

        public string DropTable(string name, DropOptions options = null)
        {
            return Drop("TABLE", name, options);
        }
        
        public string CreateDatabase(string databaseName, CreateOptions options = null)
        {
            return Create("DATABASE", databaseName, options);
        }

        public string DescribeTable(string name)
        {
            return $"DESCRIBE TABLE {name}";
        }

        private static string Table(string databaseName, Table table)
        {
            return $"{databaseName}.{table.Name}{TableSchema(table)} ENGINE = {table.Engine}{TableSelect(table)}";
        }

        private static string TableSchema(Table table)
        {
            return table.Columns != null && table.Columns.Any() ? $" {Columns(table.Columns)}" : string.Empty;
        }

        private static string TableSelect(Table table)
        {
            return string.IsNullOrWhiteSpace(table.Select) ? string.Empty : $" AS {table.Select}";
        }
        
        private static string Columns(IEnumerable<Column> columns)
        {
            return $"({string.Join(", ", columns)})";
        }

        private static string Create(string subject, string rest, CreateOptions options = null)
        {
            return $"CREATE {subject}{IfNotExists(options)} {rest}";
        }

        private static string Drop(string subject, string name, DropOptions options = null)
        {
            return $"DROP {subject}{IfExists(options)} {name}";
        }

        private static string IfNotExists(CreateOptions options = null)
        {
            var ifNotExists = options?.IfNotExists ?? true;
            return ifNotExists ? " IF NOT EXISTS" : "";
        }

        private static string IfExists(DropOptions options = null)
        {
            var ifExists = options?.IfExists ?? true;
            return ifExists ? " IF EXISTS" : "";
        }

        public List<object[]> BulkInsert<T>(IEnumerable<T> bulk)
        {
            var fields = typeof(T).GetProperties();
            var items = bulk.Select(x => Enumerable.Range(0, fields.Length).Select(i => x.GetType().GetProperty(fields[i].Name)?.GetValue(x, null)).ToArray()).ToList();
            return items;
        }
    }
}
