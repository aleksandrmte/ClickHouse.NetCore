using ClickHouse.NetCore.Entities;

namespace ClickHouse.NetCore
{
    public abstract class ChTable<T> : ITable where T : IChModel, new()
    {
        public abstract Table GetTableScheme();
        public abstract string GetTableName();
    }
}
