using whateverthefuck.src.model;
using whateverthefuck.src.util;
using System.Threading;
using whateverthefuckserver.network;
using System.Collections.Generic;

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

        public void Tick()
        {

        }
    }
}
