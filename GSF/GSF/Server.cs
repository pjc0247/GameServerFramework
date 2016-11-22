using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebSocketSharp;
using WebSocketSharp.Server;

namespace GSF
{
    public class Server
    {
        private WebSocketServer WebSocket;

        public static Server Create(int port)
        {
            return new Server(port);
        }

        private Server(int port)
        {
            WebSocket = new WebSocketServer(port);
        }

        public Server WithService<T>(string path)
            where T : Service, new()
        {
            WebSocket.AddWebSocketService<T>(path);

            return this;
        }

        public void Run()
        {
            WebSocket.Start();
            Console.WriteLine("AFTER RUN");
        }
    }
}
