using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace CardMessageWebSocketClient
{
    class Program
    {
        private static string serverAddress = "ws://127.0.0.1:5884";

        static void Main(string[] args)
        {
            using (var ws = new WebSocket(serverAddress + "/CardReader"))
            {
                Console.WriteLine("Press ENTER to exit ...");
                ws.OnMessage += (sender, e) =>
                {
                    Console.WriteLine(e.Data);
                };
                ws.Connect();
                Console.Read();
            }
        }
    }
}
