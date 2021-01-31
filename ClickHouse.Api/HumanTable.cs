using ClickHouse.NetCore;
using ClickHouse.NetCore.Entities;
using System.Collections.Generic;
using System.Linq;

namespace ClickHouse.Api
{
    public class HumanTable<T> : ChTable<T> where T : IChModel, new()
    {
        public static string TableName = "Humans";
        public static readonly List<string> Fields = new T().GetType().GetProperties().Select(x => x.Name).ToList();

        public override Table GetTableScheme()
        {
            return new Table
            {
                Engine = "MergeTree() PARTITION BY toYYYYMM(BirthDay) ORDER BY (Id) ",
                Name = TableName,
                Columns = new List<Column>
                {
                    new Column("Id", DataTypes.Int),
                    new Column("Name", DataTypes.String),
                    new Column("BirthDay", DataTypes.DateTime)
                }
            };
        }

        public override string GetTableName() => TableName;
    }
}
