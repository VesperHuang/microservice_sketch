using System;
using System.Collections.Generic;
using System.Linq;

//using System.Text.Json;
//using System.Text.Json.Serialization;
using System.Threading.Tasks;

using System.Net.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace microservice_sketch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IpInfoController : ControllerBase
    {
        private const string _access_key = "?access_key=c6d8d5419ce287d3587c3ac487533b98";
        private readonly IHttpClientFactory _httpClientFactory;

        public IpInfoController(IHttpClientFactory httpClientFactory) {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("/api/[controller]/GetIpInfo/{ip}")]
        public async Task<string> get_ip_info(string ip)
        {
            //need move to service_data_repository
            var parameter = ip + _access_key;
            var client = _httpClientFactory.CreateClient("ipstack");
            var request = new HttpRequestMessage(HttpMethod.Get, parameter);

            request.Headers.Add("cache-control", "no-cache");
            request.Headers.Add("Connection", "keep-alive");
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            request.Headers.Add("Host", "api.ipstack.com");
            request.Headers.Add("Cache-Control", "no-cache");
            request.Headers.Add("Accept", "*/*");

            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
            else
            {
                return "faild";
            }
        }
    }
}
