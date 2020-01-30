using System.Collections.Generic;
using System.Security.Cryptography;

using kooco.common.utils;

/// <summary>
/// this namespace will move out be the single dll
/// </summary>
namespace kooco.common.models
{
    public class api_settings
    {
        public string memory_type { get; set; }

        public bool dataBase_switch { get; set; }

        public bool nlogger_switch { get; set; }

        public bool health_switch { get; set; }

        public bool health_publisher { get; set; }

        public bool health_slack_notify { get; set; }

        public memory_cache memory_cache { get; set; }

        public List<dataBase> storage { get; set; }

        public token token { get; set; }
    }

    public class memory_cache
    { 
        public int size_limit { get; set; }

        public int instance_size_limit { get; set; }

        public int expiration { get; set; }

        public bool initiative { get; set; }
    }

    public class token{ 
        private string _secret;
        private string _issuer;
        private string _audience;

        public string secret { 
            get{
                return this._secret;
            } 
            
            set{
                this._secret = utils.tools.DecryptAES(value);
            }
        }

        public string issuer { 
            get{
                return this._issuer;
            } 
            
            set{
                this._issuer = utils.tools.DecryptAES(value);
            } 
        }

        public string audience { 
            get{
                return this._audience;
            } 
            
            set{
                this._audience = utils.tools.DecryptAES(value);
            }  
        }
    }

    public class dataBase {
        private string _type;
        private string _server;
        private string _port;
        private string _user_id;
        private string _password;
        private string _db_name;

        public string type {
            get { return this._type; }
            set { this._type = utils.tools.DecryptAES(value); }
        }

        public string server {
            get { return this._server; }
            set { this._server = utils.tools.DecryptAES(value); }
        }

        public string port {
            get { return this._port; }
            set { this._port = utils.tools.DecryptAES(value); }
        }

        public string user_id {
            get { return this._user_id; }
            set { this._user_id = utils.tools.DecryptAES(value); }
        }

        public string password {
            get { return this._password; }
            set { this._password = utils.tools.DecryptAES(value); }
        }

        public string db_name {
            get { return this._db_name; }
            set { this._db_name = utils.tools.DecryptAES(value); }
        }

        public string get_connection_string {
            get {
                //here need to add decode 
                //return "Server="+ utils.tools.DecryptAES(this._server) + "; Port=" + utils.tools.DecryptAES(this._port) + ";User Id=" + utils.tools.DecryptAES(this._user_id) + ";Password=" + utils.tools.DecryptAES(this._password) + ";Database=" + utils.tools.DecryptAES(this.db_name);
                return "Server="+ this._server + "; Port=" + this._port + ";User Id=" + this._user_id + ";Password=" + this._password + ";Database=" + this.db_name;
            }
        }
    }    
}
