using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSF.Concurrency
{
    public class BoundTask
    {
        public int Id { get; set; }

        public BoundTask()
        {
            Id = BoundTaskPool.GenerateNextId();
        }

        [ThreadSafe]
        public Task<T> Run<T>(Func<T> task)
        {
            return BoundTaskPool.Enqueue<T>(Id, task);
        }

        [ThreadSafe]
        public Task Run(Action task)
        {
            return BoundTaskPool.Enqueue(Id, task);
        }
    }
}
