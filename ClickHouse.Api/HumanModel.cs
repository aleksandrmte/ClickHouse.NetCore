using ClickHouse.NetCore;
using System;
using System.Collections;

namespace ClickHouse.Api
{
    public class HumanModel: IChModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDay { get; set; }

        public IEnumerator GetEnumerator()
        {
            yield return Id;
            yield return Name;
            yield return BirthDay;
        }
    }
}
