using microservice_sketch.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace microservice_sketch.Models.DataTransferObjects.Response
{
    public class BaseResponseDTO
    {
        public ReturnCodes ReturnCode { get; set; }
        public string ErrorMessage { get; set; }
        public object Data { get; set; }
    }
}
