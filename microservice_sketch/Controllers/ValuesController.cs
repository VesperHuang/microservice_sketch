using kooco.common.models;
using microservice_sketch.Models;
using microservice_sketch.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace microservice_sketch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly api_settings _api_settings;
        private HealthCheckService _health_check_service;

        public ValuesController(IEnumerable<IApiService> apiServices,IOptionsSnapshot<api_settings> options) {
            _api_settings = options.Value;

            #region example how to get instance in IEnumerable<IApiService>
            //get service instance
            //IApiService instance = service_data_repository.get_service(apiServices, "AopService");
            //instance.info("get instance from apiServices");

            //INlogService nLog_instance = (INlogService)service_data_repository.get_service(apiServices, "NlogService");
            //nLog_instance.info("NLog use info test here is values controller comstruct");
            //nLog_instance.fatal("NLog use fatal test here is values controller comstruct");
            #endregion

            _health_check_service = (HealthCheckService)service_data_repository.get_service(apiServices, "HealthCheckService");
        }

        /// <summary>
        ///     make exception to test HealthCheckService
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("/make_exception")]
        public IActionResult make_exceptin()
        {
            try
            {
                throw new Exception("from make_exception");
            }
            catch (Exception ex)
            {               
                this._health_check_service.write_exception("exception_level_1",ex.Message.ToString());
                return BadRequest("service exception");
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
