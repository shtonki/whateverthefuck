using whateverthefuck.src.model;
using whateverthefuck.src.util;
using System.Threading;
using whateverthefuckserver.network;
using System.Collections.Generic;
using whateverthefuck.src.network.messages;
using System.Linq;

namespace whateverthefuckserver
{
    class GameServer
    {
        private GameState GameState;
        private Timer TickTimer;

        public GameServer()
        {
            GameState = new GameState(true);
            TickTimer = new Timer((_) => Tick(), null, 0, 10);
        }

        public void Tick()
        {
            var es = GameState.AllEntities.Where(e => e.ControlInfo != ControlInfo.NoControl);
            Program.ServerConnection.SendMessageToEveryone(new UpdateEntityLocationsMessage(es));
        }
    }
}
