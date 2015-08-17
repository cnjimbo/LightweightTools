using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace TSharp.DatabaseLog.EF6
{
    public class ClassInstanceCounter<T> where T : class
    {
        class E : IEqualityComparer<WeakReference<T>>, IEqualityComparer<WeakReference>
        {
            public bool Equals(WeakReference x, WeakReference y)
            {
                return x.IsAlive && y.IsAlive && object.ReferenceEquals(x.Target, y.Target);
            }

            public int GetHashCode(WeakReference obj)
            {
                return RuntimeHelpers.GetHashCode(obj.Target);
            }

            public bool Equals(WeakReference<T> x, WeakReference<T> y)
            {
                T x1, y1;
                if (x.TryGetTarget(out x1) && y.TryGetTarget(out y1))
                {
                    return object.ReferenceEquals(x1, y1);
                }
                return false;
            }

            public int GetHashCode(WeakReference<T> obj)
            {
                T obj1;
                if (obj.TryGetTarget(out obj1))
                    return obj1.GetHashCode();
                return 0;
            }
        }
        private HashSet<WeakReference<T>> holder = new HashSet<WeakReference<T>>(new E());

        public int CheckAndGetInstanceCount()
        {
            lock (holder)
            {
                holder.RemoveWhere(item =>
                {
                    T t;
                    return !(item.TryGetTarget(out t));
                });
                return holder.Count;
            }
        }

        public List<T> GetAllInstance()
        {
            lock (holder)
            {
                List<T> list = new List<T>();
                holder.RemoveWhere(item =>
                {
                    T t;
                    if (item.TryGetTarget(out t))
                    {
                        list.Add(t);
                        return false;
                    }
                    return true;
                });
                return list;
            }
        }

        public void Monitor(T o)
        {
            lock (holder)
            {
                holder.Add(new WeakReference<T>(o));
            }
        }
    }
}