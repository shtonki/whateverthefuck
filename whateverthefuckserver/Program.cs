using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck;

namespace whateverthefuckserver
{
    class Program
    {
        public static void Main(string[] args)
        {
            WhateverServerConnection wsc = new WhateverServerConnection();
            wsc.StartListening();
        }
    }
}
