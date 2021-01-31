using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClickHouse.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ClickHouseService _clickHouseService;

        public WeatherForecastController(ClickHouseService clickHouseService)
        {
            _clickHouseService = clickHouseService;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            var humanTable = new HumanTable<HumanModel>();
            await _clickHouseService.CreateTable(humanTable);

            await _clickHouseService.BulkInsert(HumanTable<HumanModel>.TableName, new List<HumanModel>
            {
                new HumanModel
                {
                    Id = 1,
                    Name = "Ivan",
                    BirthDay = DateTime.UtcNow
                },
                new HumanModel
                {
                    Id = 2,
                    Name = "Oleg",
                    BirthDay = DateTime.Now
                },
            });

            await _clickHouseService.Delete($"alter table {HumanTable<HumanModel>.TableName} delete where Id = 1");

            var result = await _clickHouseService.Select<HumanModel>($"select * from {HumanTable<HumanModel>.TableName}");

            var json = JsonConvert.SerializeObject(result);

            await _clickHouseService.DropTable(HumanTable<HumanModel>.TableName);

            return json;
        }
    }
}
