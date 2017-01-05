using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

using WebSocketSharp;
using WebSocketSharp.Server;

namespace GSF
{
    using Packet;
    using Auth;

    public partial class Service<T> : WebSocketBehavior
    {
        internal Server Server;

        private static bool HasICheckable;
        private static Dictionary<Type, MethodInfo> Handlers;

        private int LastPacketId = 0;

        //private static ConcurrentDictionary<int, T> Sessions { get; set; }

        private int _UserId;
        /// <summary>
        /// 현재 세션의 유저 아이디
        /// </summary>
        public int UserId
        {
            get { return _UserId; }
            set
            {
                //Sessions[value] = (T)(object)this;
                _UserId = value;
            }
        }

        public virtual bool IsAlive
        {
            get
            {
                return State == WebSocketState.Open;
            }
        }

        static Service()
        {
            Handlers = new Dictionary<Type, MethodInfo>();
            //Sessions = new ConcurrentDictionary<int, T>();

            var handlerCandidates = typeof(T).GetMethods(
                BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.GetParameters().Length == 1)
                .Where(x => x.GetParameters().First().ParameterType.IsSubclassOf(typeof(PacketBase)))
                .Where(x => x.ReturnType == typeof(void));

            foreach (var candidate in handlerCandidates)
            {
                var packetType = candidate
                    .GetParameters().First()
                    .ParameterType;

                Handlers[packetType] = candidate;
            }

            if (typeof(T).GetInterfaces().Contains(typeof(ICheckable)))
                HasICheckable = true;
        }

        /// <summary>
        /// 숫자로된 플레이어 아이디로 세션을 가져온다.
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns>세션</returns>
        /// <exception cref="KeyNotFoundException">
        /// 주어진 playerId로 찾을 수 없을 때
        /// </exception>
        /*
        [ThreadSafe]
        protected static T GetSessionById(int playerId)
        {
            return Sessions[playerId];
        }
        */

        protected void ErrorClose(CloseStatusCode code, string reason)
        {
            if (State != WebSocketState.Open)
            {
                Console.WriteLine("State != Open");
                return;
            }

            base.Sessions.CloseSession(this.ID, code, reason);
        }

        protected internal virtual void SendRawPacket(string packet)
        {
            try
            {
                Send(packet);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }
        protected internal virtual void SendPacket(PacketBase packet)
        {
            try
            {
                if (packet.PacketId == 0)
                    packet.PacketId = Interlocked.Increment(ref LastPacketId);

                var json = PacketSerializer.Serialize(packet);

                SendRawPacket(json);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        protected void SendReplyPacket(PacketBase received, PacketBase packet)
        {
            packet.PacketId = received.PacketId;

            SendPacket(packet);
        }

        protected async Task ProcessLogin(
            string userType, string userId, string accessToken)
        {
            Console.WriteLine($"ProcessLogin({userType}, {userId}, {accessToken}");

            try
            {
                if (string.IsNullOrEmpty(userType))
                    throw new ArgumentException(nameof(userType));
                if (string.IsNullOrEmpty(userId))
                    throw new ArgumentException(nameof(userId));

                var loggedIn = await Server.AuthHandler.Login(
                    userType, userId, accessToken);

                if (loggedIn == false)
                    throw new InvalidOperationException();

                UserId = int.Parse(userId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorClose(CloseStatusCode.InvalidData, "login error");
            }
        }

        public void OnDispose()
        {
            ErrorClose(CloseStatusCode.ServerError, "healthcheck failure");
        }

        protected override async void OnOpen()
        {
            // 클라이언트 버전 체크
            #region CHECK_VERSION
            Version clientVersion = null;
            if (Version.TryParse(
                Context.QueryString.Get("version"),
                out clientVersion) == false)
            {
                ErrorClose(CloseStatusCode.InvalidData, "invalid version string");
                return;
            }

            if (new Version("1.0.0") // TODO
                != clientVersion)
            {
                ErrorClose(CloseStatusCode.ProtocolError, "serverVersion != clientVersion");
                return;
            }
            #endregion

            // 로그인
            #region LOGIN
            var userType = Context.QueryString.Get("userType");
            var userId = Context.QueryString.Get("userId");
            var accessToken = Context.QueryString.Get("accessToken");

            await ProcessLogin(userType, userId, accessToken);
            #endregion

            if (HasICheckable)
                HealthChecker.Add((ICheckable)this);
        }
        protected override void OnClose(CloseEventArgs e)
        {
            //T trash;
            // Sessions.TryRemove(UserId, out trash);

            if (HasICheckable)
                HealthChecker.Remove((ICheckable)this);
        }
        protected override void OnMessage(MessageEventArgs e)
        {
            object packet = null;

            /*
            if (e.IsText == false)
            {
                ErrorClose(CloseStatusCode.ProtocolError, "only json data accepted");
                return;
            }
            */

            try
            {
                packet = PacketSerializer.Deserialize(e.RawData);
            }
            catch (Exception ex)
            {
            }

            if (packet == null)
            {
                Console.WriteLine($"Parsing Error : {e.Data}");
                ErrorClose(CloseStatusCode.InvalidData, "parsing error");
            }
            else if (Handlers.ContainsKey(packet.GetType()) == false)
            {
                Console.WriteLine($"Unkown Packet : {e.Data}");
                ErrorClose(CloseStatusCode.InvalidData, "unknown packet");
            }
            else
            {
                var handler = Handlers[packet.GetType()];

                try
                {
                    handler.Invoke(this, new object[] { packet });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    ErrorClose(CloseStatusCode.ServerError, "internal server error");
                }
            }
        }
    }
}
