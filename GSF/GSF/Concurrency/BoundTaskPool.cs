using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GSF.Concurrency
{
    class BoundTaskPool
    {
        private static readonly int MaxPoolSize = 4;

        private static BlockingCollection<Task>[] TaskQueue { get; set; }

        static BoundTaskPool()
        {
            TaskQueue = new BlockingCollection<Task>[MaxPoolSize];
            for (int i = 0; i < TaskQueue.Length; i++)
            {
                var localQueue = TaskQueue[i] = new BlockingCollection<Task>();

                var thread = new Thread(() => {
                    PoolThread(localQueue);
                });
                thread.Start();
            }
        }

        private static void PoolThread(BlockingCollection<Task> q)
        {
            while (true)
            {
                q.Take().RunSynchronously();
            }
        }

        /// <summary>
        /// 동기 작업을 비동기 작업 Task`T로 변환한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="poolId"></param>
        /// <param name="task"></param>
        /// <returns>생성된 비동기 작업</returns>
        public static Task<T> Enqueue<T>(int poolId, Func<T> task)
        {
            if (poolId < 0 || poolId >= MaxPoolSize)
                throw new ArgumentOutOfRangeException(nameof(poolId));

            var t = new Task<T>(task);
            TaskQueue[poolId].Add(t);
            return t;
        }

        /// <summary>
        /// 동기 작업을 비동기 작업 Task로 변환한다.
        /// </summary>
        /// <param name="poolId"></param>
        /// <param name="task"></param>
        /// <returns>생성된 비동기 작업</returns>
        public static Task Enqueue(int poolId, Action task)
        {
            if (poolId < 0 || poolId >= MaxPoolSize)
                throw new ArgumentOutOfRangeException(nameof(poolId));

            var t = new Task(task);
            TaskQueue[poolId].Add(t);
            return t;
        }

        public static int GenerateNextId()
        {
            return new Random().Next(MaxPoolSize);
        }
    }
}
