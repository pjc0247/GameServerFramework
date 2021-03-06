﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSF // 편의를 위해 네임스페이스 생략
{
    public enum As
    {
        SingleConsumer,
        MultiProducer,

        ThreadLocal
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class ThreadSafe : Attribute
    {
        public ThreadSafe()
        {
        }
        public ThreadSafe(As @as)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class NotThreadSafe : Attribute
    {
        public NotThreadSafe()
        {
        }
        public NotThreadSafe(As @as)
        {
        }
    }
}
