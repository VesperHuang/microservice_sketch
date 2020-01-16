using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using microservice_sketch.Models.DataTransferObjects.Request;
using microservice_sketch.Models.Shared;
using Microsoft.AspNetCore.Mvc;

namespace microservice_sketch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SlackNotifyController : ControllerBase
    {
        // POST api/values
        [HttpPost]
        public void Post(SlackNotifyDTO para)
        {
            var response = Utils.PostJsonMessageToSlackChannel(para);
        }
    }
}