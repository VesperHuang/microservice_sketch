using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


using kooco.common.models;
using microservice_sketch.Services;

namespace microservice_sketch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IApiService _service;
        private readonly api_settings _api_settings;
        public ValuesController(IEnumerable<IApiService> apiServices,IOptionsSnapshot<api_settings> options) {
            _api_settings = options.Value;

            foreach (var serrvice in apiServices)
            {
                switch (serrvice)
                {
                    case AopService aop:
                        _service = aop;
                        _service.info("use aop service");
                        break;
                    case DataBase database:
                        _service = database;
                        _service.info("use database service");
                        break;
                }
            }
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
