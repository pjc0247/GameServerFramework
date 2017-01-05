using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GSF;
using GSF.Concurrency;

namespace GSF.MatchMaking
{
    public class MatchPlayer
    {
        public string UserId;
    }

    public class MatchGroup
    {
        public MatchPlayer[] Players;
    }

    public class MatchData
    {
        public MatchGroup[] Groups;
    }

    public interface IMatchMaker
    {
        /// <summary>
        /// 매치 메이커에 유저를 넣는다.
        /// 
        /// 매치 메이커는 유저의 데이터를 기반으로 적절한 MatchQueue에
        /// 다시 넣어야 한다.
        /// </summary>
        /// <param name="player">유저</param>
        /// <param name="queueType">큐 힌트</param>
        void Enqueue(MatchPlayer player, int queueType);

        void Enqueue(MatchGroup group, int queueType);

        IEnumerable<MatchData> Poll();
    }

    public interface IMatchQueue
    {
        void Reset(int maxPlayerCount);

        /// <summary>
        /// 매치 큐에 유저를 넣는다.
        /// </summary>
        /// <param name="group">유저 목록</param>
        void Enqueue(MatchGroup group);

        bool TryDequeue(int playerCount, out MatchData result);
    }
}
