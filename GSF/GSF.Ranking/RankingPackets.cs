using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GSF.Packet;

namespace GSF.Ranking
{
    public class RankingOperationResult : PacketBase
    {
    }

    public class AddScore : PacketBase
    {
        public string RankingKey { get; set; }

        public Player Player { get; set; }
        public Dictionary<string, object> Data { get; set; }

        public long Score { get; set; }
    }
    public class AddScoreResponse : RankingOperationResult { }

    public class DeleteScore : PacketBase
    {
        public string RankingKey { get; set; }

        public Player Player { get; set; }
    }
    public class DeleteScoreResponse : RankingOperationResult { }

    public class QueryPlayerRank : PacketBase
    {
        public string RankingKey { get; set; }

        public Player Player { get; set; }
    }
    public class QueryPlayerRankResponse : RankingOperationResult
    {
        public long Rank { get; set; }
        public Dictionary<string, object> Data { get; set; }
    }
}
