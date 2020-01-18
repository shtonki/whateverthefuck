using whateverthefuck.src.model;
using whateverthefuck.src.util;

namespace whateverthefuckserver.storage
{
    class UserInfo
    {
        public UserInfo(string username)
        {
            Username = username;
            Inventory = new Inventory();
            Equipment = new Equipment();
        }

        public UserInfo(string username, Inventory inventory, Equipment equipment) : this(username)
        {
            Inventory = inventory;
            Equipment = equipment;
        }

        public string Username { get; }

        public Inventory Inventory { get; }

        public Equipment Equipment { get; }
    }
}
