using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebSocketSharp;
using WebSocketSharp.Server;

namespace GSF
{
    using Auth;

    public class Server
    {
        private WebSocketServer WebSocket;

        internal AuthHandler AuthHandler;

        /// <summary>
        /// 새로운 서버 인스턴스를 생성합니다. <para/>
        /// 생성된 인스턴스는 <see cref="Run"/> 메소드로 실행합니다.
        /// </summary>
        /// <param name="port">서버 포트</param>
        /// <returns>서버 인스턴스</returns>
        public static Server Create(int port)
        {
            return new Server(port);
        }

        private Server(int port)
        {
            WebSocket = new WebSocketServer(port);
            AuthHandler = new AuthHandler();
        }

        /// <summary>
        /// 서버에 서비스를 등록합니다. <para/>
        /// 이 작업은 반드시 <see cref="Run"/> 실행 이전에 호출되어야 합니다.
        /// </summary>
        /// <typeparam name="T">서비스 타입</typeparam>
        /// <param name="path">서비스 주소 (상대경로)</param>
        /// <returns>서버 인스턴스</returns>
        public Server WithService<T>(string path)
            where T : Service<T>, new()
        {
            WebSocket.AddWebSocketService<T>(path, () =>
            {
                var session = new T();
                session.Server = this;
                return session;
            });

            return this;
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="userType"></param>
        /// <returns></returns>
        /// <example>
        /// <code>
        /// Server.Create(9916)
        ///     .Accepts<KakaoTalkIDProvider>("kakao")
        ///     /* your service goes here */
        ///     .Run();
        /// </code>
        /// </example>
        public Server Accepts<T>(string userType)
            where T : Auth.IDP.IDProvider, new()
        {
            AuthHandler.AddIDProvider<T>(userType);
            return this;
        }

        public void Run()
        {
            WebSocket.Start();
        }
    }
}
