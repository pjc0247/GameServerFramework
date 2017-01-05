using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSF.MatchMaking
{
    public class MatchQueueLocal : IMatchQueue
    {
        private object LockObject = new object();

        private int MaxPlayerCount;
        private List<MatchGroup>[] Slots;

        public void Reset(int maxPlayerCount)
        {
            MaxPlayerCount = maxPlayerCount;

            Slots = new List<MatchGroup>[maxPlayerCount + 1];
            for (int i = 0; i < maxPlayerCount + 1; i++)
                Slots[i] = new List<MatchGroup>();
        }

        public void Enqueue(MatchGroup group)
        {
            if (group.Players.Length <= 0)
                throw new ArgumentException($"{nameof(group)}.Length <= 0");

            lock (LockObject)
            {
                Slots[group.Players.Length].Add(group);
            }
        }

        public bool TryDequeue(int playerCount, out MatchData result)
        {
            var resultList = new List<MatchGroup>();

            lock (LockObject)
            {
                if (TryDequeue(playerCount, resultList))
                {
                    result = new MatchData()
                    {
                        Groups = resultList.ToArray()
                    };
                    return true;
                }
            }

            result = null;
            return false;
        }
        private bool TryDequeue(int playerCount, List<MatchGroup> result)
        {
            if (playerCount == 0)
                return true;

            for (int i= playerCount; i > 0; i--)
            {
                if (Slots[i].Count > 0)
                {
                    result.Add(Slots[i].Last());
                    Slots[i].RemoveAt(Slots[i].Count - 1);

                    if (TryDequeue(playerCount - i, result))
                        return true;

                    Slots[i].Add(result.Last());
                    result.RemoveAt(result.Count - 1);
                }
            }

            return false;
        }
    }
}
