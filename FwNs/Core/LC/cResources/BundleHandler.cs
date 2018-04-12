namespace FwNs.Core.LC.cResources
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Resources;

    public static class BundleHandler
    {
        private const string Prefix = "LibCore.Resources.";
        public static readonly object SetGlobalLock = new object();
        private static readonly object Mutex = new object();
        private static CultureInfo _locale = CultureInfo.CurrentCulture;
        private static readonly Dictionary<string, int> BundleHandleMap = new Dictionary<string, int>();
        private static readonly List<ResourceManager> BundleList = new List<ResourceManager>();

        public static ResourceManager GetBundle(string name, Assembly cl)
        {
            if (cl != null)
            {
                return new ResourceManager(name, cl);
            }
            return new ResourceManager(name, Assembly.GetExecutingAssembly());
        }

        public static int GetBundleHandle(string name, Assembly cl)
        {
            return GetBundleHandle2("LibCore.Resources." + name, cl);
        }

        public static int GetBundleHandle2(string name, Assembly cl)
        {
            int num;
            lock (Mutex)
            {
                if (BundleHandleMap.TryGetValue(name, out num))
                {
                    return num;
                }
                try
                {
                    ResourceManager bundle = GetBundle(name, cl);
                    BundleList.Add(bundle);
                    num = BundleList.Count - 1;
                    BundleHandleMap.Add(name, num);
                    return num;
                }
                catch (Exception)
                {
                    return -1;
                }
            }
            return num;
        }

        public static CultureInfo GetLocale()
        {
            lock (Mutex)
            {
                return _locale;
            }
        }

        public static string GetString(int handle, string key)
        {
            ResourceManager manager;
            lock (Mutex)
            {
                if (((handle < 0) || (handle >= BundleList.Count)) || (key == null))
                {
                    manager = null;
                }
                else
                {
                    manager = BundleList[handle];
                }
            }
            if (manager == null)
            {
                return null;
            }
            try
            {
                return ((_locale == null) ? manager.GetString(key) : manager.GetString(key, _locale));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void SetLocale(CultureInfo l)
        {
            lock (Mutex)
            {
                if (l == null)
                {
                    throw new ArgumentException("null locale");
                }
                _locale = l;
            }
        }
    }
}

