namespace System.Data.LibCore
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public static class UtlConnectionPool
    {
        private static Dictionary<string, Pool> _connectionPools = new Dictionary<string, Pool>(StringComparer.OrdinalIgnoreCase);
        private static int _poolVersion = 1;

        public static void Add(string connectionString, UtlConnectionProxy hdl, int version)
        {
            lock (_connectionPools)
            {
                Pool pool;
                if ((_connectionPools.TryGetValue(connectionString, out pool) && (version == pool.PoolVersion)) && ((hdl.Created + hdl.LifeTime) > DateTime.UtcNow.Ticks))
                {
                    ResizePool(pool, true);
                    hdl.IsPooled = true;
                    pool.Queue.Enqueue(new WeakReference(hdl, false));
                    GC.KeepAlive(hdl);
                }
                else
                {
                    try
                    {
                        hdl.Close();
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        public static void ClearAllPools()
        {
            lock (_connectionPools)
            {
                foreach (KeyValuePair<string, Pool> pair in _connectionPools)
                {
                    while (pair.Value.Queue.Count > 0)
                    {
                        UtlConnectionProxy target = pair.Value.Queue.Dequeue().Target as UtlConnectionProxy;
                        if (target != null)
                        {
                            try
                            {
                                target.Close();
                                continue;
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                        }
                    }
                    if (_poolVersion <= pair.Value.PoolVersion)
                    {
                        _poolVersion = pair.Value.PoolVersion + 1;
                    }
                }
                _connectionPools.Clear();
            }
        }

        public static void ClearPool(string connectionString)
        {
            lock (_connectionPools)
            {
                Pool pool;
                if (_connectionPools.TryGetValue(connectionString, out pool))
                {
                    pool.PoolVersion++;
                    while (pool.Queue.Count > 0)
                    {
                        UtlConnectionProxy target = pool.Queue.Dequeue().Target as UtlConnectionProxy;
                        if (target != null)
                        {
                            try
                            {
                                target.Close();
                                continue;
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                        }
                    }
                }
            }
        }

        public static UtlConnectionProxy Remove(string connectionString, int maxPoolSize, int minPoolSize, out int version)
        {
            lock (_connectionPools)
            {
                Pool pool;
                version = _poolVersion;
                if (!_connectionPools.TryGetValue(connectionString, out pool))
                {
                    pool = new Pool(_poolVersion, connectionString, maxPoolSize, minPoolSize);
                    _connectionPools.Add(connectionString, pool);
                    return null;
                }
                version = pool.PoolVersion;
                pool.MaxPoolSize = maxPoolSize;
                ResizePool(pool, false);
                while (pool.Queue.Count > 0)
                {
                    UtlConnectionProxy target = pool.Queue.Dequeue().Target as UtlConnectionProxy;
                    if ((target != null) && ((target.Created + target.LifeTime) > DateTime.UtcNow.Ticks))
                    {
                        target.IsPooled = false;
                        return target;
                    }
                }
                return null;
            }
        }

        private static void ResizePool(Pool queue, bool forAdding)
        {
            int maxPoolSize = queue.MaxPoolSize;
            if (!forAdding && (maxPoolSize > 0))
            {
                maxPoolSize++;
            }
            else if (forAdding && (maxPoolSize > 0))
            {
                maxPoolSize--;
            }
            while (queue.Queue.Count > maxPoolSize)
            {
                UtlConnectionProxy target = queue.Queue.Dequeue().Target as UtlConnectionProxy;
                if (target != null)
                {
                    try
                    {
                        target.Close();
                        continue;
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
            maxPoolSize = queue.MinPoolSize;
            if (!forAdding && (maxPoolSize > 0))
            {
                maxPoolSize++;
            }
            else if (forAdding && (maxPoolSize > 0))
            {
                maxPoolSize--;
            }
            while (queue.Queue.Count < maxPoolSize)
            {
                UtlConnectionProxy target = new UtlConnectionProxy(new UtlConnectionOptions(queue.ConnectionString));
                target.Open();
                target.IsPooled = true;
                queue.Queue.Enqueue(new WeakReference(target, false));
                GC.KeepAlive(target);
            }
        }

        public class Pool
        {
            public readonly Queue<WeakReference> Queue = new Queue<WeakReference>();
            public int PoolVersion;
            public int MaxPoolSize;
            public int MinPoolSize;
            public string ConnectionString;

            public Pool(int version, string connectionString, int maxSize, int minSize)
            {
                this.PoolVersion = version;
                this.ConnectionString = connectionString;
                this.MaxPoolSize = maxSize;
                this.MinPoolSize = minSize;
            }
        }
    }
}

