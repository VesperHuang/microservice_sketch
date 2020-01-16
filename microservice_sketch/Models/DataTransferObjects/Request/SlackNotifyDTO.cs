using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace microservice_sketch.Models.DataTransferObjects.Request
{
    public class SlackNotifyDTO
    {
        public string Channel { get; set; }
        public object Json { get; set; }
    }
}
