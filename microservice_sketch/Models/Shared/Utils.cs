using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using microservice_sketch.Models.DataTransferObjects;
using microservice_sketch.Models.DataTransferObjects.Response;
using microservice_sketch.Models.DataTransferObjects.Request;

namespace microservice_sketch.Models.Shared
{
    public class Utils
    {
        public const string _slackwebhook = "https://hooks.slack.com/services/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";

        public static BaseResponseDTO PostJsonMessageToSlackChannel(SlackNotifyDTO para)
        {
            //string json = JsonConvert.SerializeObject(para.Json);

            string json = para.Json.ToString();

            SlackPayload payload = new SlackPayload()
            {
                Channel = para.Channel,
                Username = "KoocoApiCenter",
                Text = $"=> *`{json}`*"
            };
            string payloadJson = JsonConvert.SerializeObject(payload);

            using (WebClient wc = new WebClient())
            {
                try
                {
                    wc.Encoding = Encoding.UTF8;

                    NameValueCollection nc = new NameValueCollection();
                    nc["payload"] = payloadJson;

                    byte[] bResult = wc.UploadValues(_slackwebhook, nc);

                    string result = Encoding.UTF8.GetString(bResult);

                    return new BaseResponseDTO()
                    {
                        ReturnCode = Enums.ReturnCodes.Succeed
                    };
                }
                catch (WebException ex)
                {
                    //throw new Exception("無法連接遠端伺服器");
                    //Console.WriteLine($"CallApi catch:Url={url},Value={nc},ErrorMessage={ex.Message}");
                    return new BaseResponseDTO()
                    {
                        ReturnCode = Enums.ReturnCodes.InternalServerError,
                        ErrorMessage = "InternalServerError",
                        Data = ex.Message
                    };
                }
            }
        }
    }
}