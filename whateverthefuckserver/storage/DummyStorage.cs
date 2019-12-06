using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whateverthefuckserver.storage
{
    class DummyStorage : IStorage
    {
        public void AddEntry(string collectionName, IStorable entry)
        {

        }

        public void AddJson(string collectionName, object o)
        {

        }

        public void AddBson(string collectionName, object o)
        {
            
        }
    }
}
