using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace microservice_sketch.Models
{
    public class exception_collection
    {
        private Dictionary<string, List<string>> _collection = new Dictionary<string, List<string>>();

        public Dictionary<string, List<string>> collection {
            get {
                return this._collection;
            }
        }
    }
}
