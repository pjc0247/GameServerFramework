using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSF.Concurrency
{
    public class ConcurrentSet<T>
    {
        private ConcurrentDictionary<T, T> Dic { get; set; }

        public int Count
        {
            get
            {
                return Dic.Count;
            }
        }

        public ConcurrentSet()
        {
            Dic = new ConcurrentDictionary<T, T>();
        }

        public bool TryAdd(T obj)
        {
            return Dic.TryAdd(obj, obj);
        }
        public bool Contains(T obj)
        {
            return Dic.ContainsKey(obj);
        }
        public bool TryRemove(T obj)
        {
            T trash;
            return Dic.TryRemove(obj, out trash);
        }

        public void Clear()
        {
            Dic.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var pair in Dic)
                yield return pair.Key;
        }
    }
}
