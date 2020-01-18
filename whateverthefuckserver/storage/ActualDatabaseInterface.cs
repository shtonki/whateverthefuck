using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using whateverthefuck.src.util;

namespace whateverthefuckserver.storage
{
    interface ActualDatabaseInterface
    {
        void StoreUserInfo(UserInfo info);
        UserInfo GetUserInfo(string username);
    }

    class SebasLocalDatabase : ActualDatabaseInterface
    {
        private const string FilePath = "database.txt";

        private Dictionary<string, UserInfo> UserInfos { get; }

        public static SebasLocalDatabase Instance { get; } = new SebasLocalDatabase();

        public SebasLocalDatabase()
        {
            UserInfos = new Dictionary<string, UserInfo>();

            var bs = File.ReadAllBytes(FilePath);
            if (bs != null && bs.Length > 0)
            {
                try
                {
                    Read(bs);
                    Logging.Log("Dank user database");
                }
                catch (Exception e)
                {
                    Logging.Log("Corrupt user database");
                }
            }
        }

        public void StoreUserInfo(UserInfo info)
        {
            UserInfos[info.Username] = info;
            Write();
        }

        public UserInfo GetUserInfo(string username)
        {
            if (!UserInfos.ContainsKey(username))
            {
                UserInfos[username] = new UserInfo(username);
            }

            return UserInfos[username];
        }

        private void Write()
        {
            WhateverEncoder encoder = new WhateverEncoder();

            encoder.Encode(UserInfos.Count);

            foreach (var info in UserInfos.Values)
            {
                EncodeUserInfo(info, encoder);
                Logging.Log("Writing inventory " + info.Inventory.AllItems.Count());
            }

            var bytes = encoder.GetBytes();
            File.WriteAllBytes(FilePath, bytes);
        }

        private void Read(byte[] bs)
        {
            WhateverDecoder decoder = new WhateverDecoder(bs);

            var userCount = decoder.DecodeInt();

            for (int i = 0; i < userCount; i++)
            {
                var userInfo = DecodeUserInfo(decoder);
                UserInfos[userInfo.Username] = userInfo;
                Logging.Log("Reading inventory " + userInfo.Inventory.AllItems.Count());
            }
        }

        private void EncodeUserInfo(UserInfo info, WhateverEncoder encoder)
        {
            encoder.Encode(info.Username);
            info.Inventory.Encode(encoder);
            encoder.Encode(info.Equipment.Items);
        }

        private UserInfo DecodeUserInfo(WhateverDecoder decoder)
        {
            var username = decoder.DecodeString();
            
            var userInfo = new UserInfo(username);
            userInfo.Inventory.Decode(decoder);
            userInfo.Equipment.Decode(decoder);

            return userInfo;
        }
    }
}
