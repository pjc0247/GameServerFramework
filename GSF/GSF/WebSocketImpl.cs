using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebSocketSharp;
using WebSocketSharp.Server;

namespace GSF
{
    class WebSocketImpl : WebSocketBehavior
    {
        public virtual bool IsAlive
        {
            get
            {
                return State == WebSocketState.Open;
            }
        }
        public Action OnOpenEventHandler;

        public void Close(int code, string reason)
        {
            Sessions.CloseSession(ID, (ushort)code, reason);
        }
        public void SendData(string data)
        {
            Send(data);
        }

        protected override void OnOpen()
        {
            OnOpenEventHandler?.Invoke();
        }
        protected override void OnClose(CloseEventArgs e)
        {
            
        }
        protected override void OnError(ErrorEventArgs e)
        {
            
        }
        protected override void OnMessage(MessageEventArgs e)
        {
            
        }
    }
}
