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
    }

    interface IStorable
    {
        Dictionary<string, string> toStorageFormat();
    }
}
