using whateverthefuck.src.model;
using whateverthefuck.src.util;
using System.Threading;
using whateverthefuckserver.network;
using System.Collections.Generic;
using whateverthefuck.src.network.messages;

namespace whateverthefuckserver
{
    class GameServer
    {
        private GameState GameState;
        private Timer TickTimer;

        public GameServer()
        {
            GameState = new GameState();
            TickTimer = new Timer((_) => Tick(), null, 0, 10);
        }
        int i = 0;
        public void Tick()
        {
            Program.ServerConnection.SendMessageToEveryone(new LogMessage("we tickers now " + i++));
        }
    }
}
