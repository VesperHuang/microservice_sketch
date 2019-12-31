using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kooco.common.models

{
    public class api_settings
    {
       public bool memory_switch { get; set; }

       public bool dataBase_switch { get; set; }

       public memory_catch memory_catch { get; set; }
    }

    public class memory_catch { 
        public int size_limit { get; set; }

        public int words_limit { get; set; }

        public int expiration { get; set; }

        public List<string> actions { get; set; }

        public dataBase dataBase { get; set; }
    }

    public class dataBase { 
    
        public string server { get; set; }

        public string user_id { get; set; }

        public string password { get; set; }

        public string db_name { get; set; }
    }
}
