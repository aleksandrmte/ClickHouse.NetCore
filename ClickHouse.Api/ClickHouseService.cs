using ClickHouse.NetCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClickHouse.Api
{
    public class ClickHouseService
    {
        private readonly IClickHouseDatabase _db;

        public ClickHouseService(IClickHouseDatabase db)
        {
            _db = db;
        }

        public async Task DropTable(string tableName)
        {
            if (await _db.TableExists(tableName))
                await _db.DropTable(tableName);
        }

        public async Task CreateTable(ITable table)
        {
            if (await _db.TableExists(table.GetTableName())) 
                return;
            await _db.CreateTable(table.GetTableScheme());
        }

        public async Task BulkInsert<T>(string tableName, List<T> bulk) => await _db.BulkInsert(tableName, bulk);

        public async Task<List<T>> Select<T>(string selectQuery) where T : new()
        {
            var result = await _db.ExecuteQueryMapping<T>(selectQuery);
            return result.ToList();
        }

        public async Task Delete(string queryDelete)
        {
            await _db.ExecuteNonQueryAsync(queryDelete);
        }
    }
}
