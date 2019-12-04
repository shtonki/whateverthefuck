using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.model;
using whateverthefuck.src.util;
using whateverthefuck.src.view;

namespace whateverthefuck
{
    class Program
    {
        public static GameState GameState = new GameState();

        public static void Main(String[] args)
        {
            Logger.AddLoggingOutput(new ConsoleOutput(Logger.LoggingLevel.All, true));
            Logger.Log("Started Logger");
            GUI.CreateGameWindow();
        }
    }
}
