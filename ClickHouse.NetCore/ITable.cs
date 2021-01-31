using ClickHouse.NetCore.Entities;

namespace ClickHouse.NetCore
{
    public interface ITable
    {
        Table GetTableScheme();
        string GetTableName();
    }
}
