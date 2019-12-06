using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whateverthefuckserver.storage
{
    interface IStorage
    {
        void AddEntry(string collectionName, IStorable entry);
        void AddJson(string collectionName, object o);
        void AddBson(string collectionName, object o);
    }

    interface IStorable
    {
        Dictionary<string, string> toStorageFormat();
    }
}
