using System.Collections.Generic;

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
        public string secret { get; set; }

        public string issuer { get; set; }

        public string audience { get; set; }
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
            set { this._type = value; }
        }

        public string server {
            get { return this._server; }
            set { this._server = value; }
        }

        public string port {
            get { return this._port; }
            set { this._port = value; }
        }

        public string user_id {
            get { return this._user_id; }
            set { this._user_id = value; }
        }

        public string password {
            get { return this._password; }
            set { this._password = value; }
        }

        public string db_name {
            get { return this._db_name; }
            set { this._db_name = value; }
        }

        public string get_connection_string {
            get {
                //here need to add decode 
                return "Server="+ this._server + "; Port=" + this._port + ";User Id=" + this._user_id + ";Password=" + this._password + ";Database=" + this.db_name;
            }
        }
    }
}
