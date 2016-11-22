using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using WebSocketSharp;
using WebSocketSharp.Server;

namespace GSF
{
    using Concurrency;

    public class HealthChecker
    {
        public static readonly int Interval = 5000;

        private static ConcurrentSet<ICheckable> Objects { get; set; }
        private static Thread CheckThread { get; set; }

        static HealthChecker()
        {
            Objects = new ConcurrentSet<ICheckable>();

            CheckThread = new Thread(() =>
            {
                Checker();
            });
            CheckThread.Start();
        }

        private static void Checker()
        {
            Console.WriteLine("[HealthChecker] " +
                Thread.CurrentThread.ManagedThreadId);

            while (true)
            {
                foreach(var obj in Objects)
                {
                    var result = false;

                    try
                    {
                        result = obj.OnHealthCheck();
                    }
                    catch(Exception e) {
                        Console.WriteLine(
                            "HealthCheck Failure due to an exception\r\n"
                            + e.ToString());
                    }
                    
                    if (result == false)
                    {
                        obj.OnDispose();
                        Objects.TryRemove(obj);
                    }                    
                }

                Thread.Sleep(Interval);
            }
        }

        /// <summary>
        /// 관리 목록에 추가한다.
        /// 주기적으로 OnHealthCheck가 호출된다.
        /// </summary>
        /// <param name="obj">추가할 대상</param>
        public static void Add(ICheckable obj)
        {
            Objects.TryAdd(obj);
        }

        /// <summary>
        /// 관리 목록에서 제거한다.
        /// </summary>
        /// <param name="obj">제거할 대상</param>
        public static void Remove(ICheckable obj)
        {
            Objects.TryRemove(obj);
        }
    }
}
