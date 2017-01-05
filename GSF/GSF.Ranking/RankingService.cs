using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StackExchange.Redis;

namespace GSF.Ranking
{
    public class RankingService : Service<RankingService>
    {
        private static ConnectionMultiplexer Connection { get; }
        private static IDatabase Database { get; }

        static RankingService()
        {
            Connection = ConnectionMultiplexer.Connect("127.0.0.1:6379");
            Database = Connection.GetDatabase();
        }

        public void OnAddScore(AddScore packet)
        {
            Database.SortedSetAddAsync(packet.RankingKey, packet.Player.PlayerId, packet.Score);
        }
        public void OnDeleteScore(DeleteScore packet)
        {
            Database.SortedSetRemoveAsync(packet.RankingKey, packet.Player.PlayerId);
        }
        public async void OnQueryPlayerRanking(QueryPlayerRank packet)
        {
            var rank = await Database.SortedSetRankAsync(packet.RankingKey, packet.Player.PlayerId);

            if (rank.HasValue == false)
                Console.WriteLine("NoValue");

            SendReplyPacket(packet, new QueryPlayerRankResponse()
            {
                Rank = rank.Value
            });
        }

        public static void Test()
        {
            Database.ScriptEvaluate("redis.call('flushall')");

            Database.SortedSetAdd("A", "ASDF1", 100);
            Database.SortedSetAdd("A", "ASDF2", 100);
            Database.SortedSetAdd("A", "ASDF3", 100);

            var values = Database.SortedSetRangeByRank("A", 0, -1);

            foreach(var value in values)
            {
                Console.WriteLine(value);
            }
        }
    }
}
