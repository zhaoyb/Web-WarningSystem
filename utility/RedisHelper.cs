using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using ServiceStack.Redis;
using ServiceStack.Text;

namespace utility
{
    public class RedisHelper
    {
        private static PooledRedisClientManager _prcm;

        static RedisHelper()
        {
            if (_prcm != null)
                Init();
        }

        public static void Init()
        {
            if (_prcm != null)
                throw new Exception("连接池已存在");

            long? defaultDb = 0;
            List<string> readWriteHosts = null;
            List<string> readOnlyHosts = null;
            int maxReadConnection = 100;
            int maxWriteConnection = 50;
            var redisconfig = ConfigurationManager.GetSection("RedisConfig") as IDictionary;
            if (redisconfig != null)
            {
                if (redisconfig["DefaultDb"] != null)
                {
                    defaultDb = Convert.ToInt64(redisconfig["DefaultDb"].ToString());
                }

                if (redisconfig["ReadWriteHosts"] != null)
                {
                    readWriteHosts =
                        redisconfig["ReadWriteHosts"].ToString()
                            .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                            .ToList();
                }

                if (redisconfig["ReadOnlyHosts"] != null)
                {
                    readOnlyHosts =
                        redisconfig["ReadOnlyHosts"].ToString()
                            .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                            .ToList();
                }

                if (redisconfig["MaxReadConnection"] != null)
                {
                    maxReadConnection = Convert.ToInt32(redisconfig["MaxReadConnection"].ToString());
                }

                if (redisconfig["MaxWriteConnection"] != null)
                {
                    maxWriteConnection = Convert.ToInt32(redisconfig["MaxWriteConnection"].ToString());
                }
            }


            _prcm = new PooledRedisClientManager(readWriteHosts, readOnlyHosts, new RedisClientManagerConfig
            {
                MaxWritePoolSize = maxWriteConnection, //写操作链接数量
                MaxReadPoolSize = maxReadConnection, //读操作链接数量
                AutoStart = true,
                DefaultDb = defaultDb
            });
        }

        private static IRedisClient GetClient()
        {
            if (_prcm == null)
                throw new Exception("没有可用连接,请确认连接池是否已经加载.");
            return _prcm.GetClient();
        }


        /// <summary>
        ///     写入Redis
        /// </summary>
        public static bool Set<T>(string key, T value, bool cancelWhenExisted = false)
        {
            using (IRedisClient client = GetClient())
            {
                if (client.ContainsKey(key) && cancelWhenExisted)
                {
                    return false;
                }
                return client.Set(key, value);
            }
        }

        public static bool Set<T>(string key, T value, TimeSpan expire)
        {
            using (IRedisClient client = GetClient())
            {
                return client.Set(key, value, expire);
            }
        }

        public static long Incr(string key)
        {
            using (IRedisClient client = GetClient())
            {
                return client.IncrementValue(key);
            }
        }

        /// <summary>
        ///     根据key获取
        /// </summary>
        public static T Get<T>(string key)
        {
            using (IRedisClient client = GetClient())
            {
                return client.Get<T>(key);
            }
        }

        /// <summary>
        ///     批量获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static IDictionary<string, T> GetAll<T>(List<string> keys)
        {
            using (IRedisClient client = GetClient())
            {
                return client.GetAll<T>(keys);
            }
        }

        /// <summary>
        ///     删除指定key的项
        /// </summary>
        public static bool Del(string key)
        {
            using (IRedisClient client = GetClient())
            {
                if (client.ContainsKey(key))
                    return client.Remove(key);
            }
            return false;
        }

        /// <summary>
        ///     清空Redis
        /// </summary>
        public static bool Clear()
        {
            try
            {
                using (IRedisClient client = GetClient())
                {
                    client.RemoveAll(client.GetAllKeys());
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        ///     获取当前DB中所有key
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllKeys()
        {
            using (IRedisClient client = GetClient())
            {
                return client.GetAllKeys();
            }
        }

        /// <summary>
        ///     当前DB中是否存在指定Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Exists(string key)
        {
            using (IRedisClient client = GetClient())
            {
                return client.ContainsKey(key);
            }
        }

        #region Hash Data

        /// <summary>
        ///     以Hash形式保存一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashKey"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="cancelWhenExisted"></param>
        /// <returns></returns>
        public static bool Set<T>(string hashKey, string key, T value, bool cancelWhenExisted = false)
        {
            if (value == null)
                return false;
            string valueStr = null;
            try
            {
                valueStr = TypeSerializer.SerializeToString(value);
            }
            catch
            {
                valueStr = null;
            }
            if (string.IsNullOrEmpty(valueStr))
                return false;

            using (IRedisClient client = GetClient())
            {
                if (cancelWhenExisted)
                    return client.SetEntryInHashIfNotExists(hashKey, key, valueStr);

                return client.SetEntryInHash(hashKey, key, valueStr);
            }
        }

        /// <summary>
        ///     从Hash中获取指定对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashKey"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(string hashKey, string key)
        {
            T value = default(T);
            using (IRedisClient client = GetClient())
            {
                if (!client.HashContainsEntry(hashKey, key))
                    return default(T);
                string valueStr = client.GetValueFromHash(hashKey, key);
                if (!string.IsNullOrEmpty(valueStr))
                {
                    try
                    {
                        value = TypeSerializer.DeserializeFromString<T>(valueStr);
                    }
                    catch
                    {
                        value = default(T);
                    }
                }
            }
            return value;
        }

        /// <summary>
        ///     批量添加对象到Hash
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashKey"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool SetRangeValueInHash<T>(string hashKey, IDictionary<string, T> values)
        {
            if (values == null || values.Count == 0)
                return false;
            var strValues = new Dictionary<string, string>();
            try
            {
                foreach (var item in values)
                {
                    string valueStr = TypeSerializer.SerializeToString(item.Value);
                    if (!string.IsNullOrEmpty(valueStr))
                    {
                        strValues.Add(item.Key, valueStr);
                    }
                }
                if (strValues.Count > 0)
                {
                    using (IRedisClient client = GetClient())
                    {
                        client.SetRangeInHash(hashKey, strValues);
                    }
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        ///     从指定Hash中删除指定Key的对象
        /// </summary>
        /// <param name="hashKey"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Del(string hashKey, string key)
        {
            using (IRedisClient client = GetClient())
            {
                return client.RemoveEntryFromHash(hashKey, key);
            }
        }

        /// <summary>
        ///     获取指定Hash包含的所有子项集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashKey"></param>
        /// <returns></returns>
        public static IDictionary<string, T> GetItemFormHash<T>(string hashKey)
        {
            IDictionary<string, T> result = new Dictionary<string, T>();
            using (IRedisClient client = GetClient())
            {
                if (!client.ContainsKey(hashKey))
                    return null;
                Dictionary<string, string> strValues = client.GetAllEntriesFromHash(hashKey);

                if (strValues == null || strValues.Count == 0)
                    return null;

                foreach (var item in strValues)
                {
                    try
                    {
                        result.Add(item.Key, TypeSerializer.DeserializeFromString<T>(item.Value));
                    }
                    catch
                    {

                    }
                }
                return result;
            }
        }

        #endregion

        #region Set Data

        public static bool AddToSortedSet(string setId, string value, double? score = null)
        {
            using (IRedisClient client = GetClient())
            {
                if (score == null)
                {
                    return client.AddItemToSortedSet(setId, value);
                }
                return client.AddItemToSortedSet(setId, value, score.Value);
            }
        }

        public static List<string> GetItemsFromSortedSetByScore(string setId, double startScore, double endScore)
        {
            using (IRedisClient client = GetClient())
            {
                return client.GetRangeFromSortedSetByLowestScore(setId, startScore, endScore);
            }
        }

        public static List<string> GetItemsFromSortedSetByScore(string setId, double startScore, double endScore,
            int skip, int take)
        {
            using (IRedisClient client = GetClient())
            {
                return client.GetRangeFromSortedSetByLowestScore(setId, startScore, endScore, skip, take);
            }
        }

        public static long RemoveItemsByScore(string setId, double startScore, double endScore)
        {
            using (IRedisClient client = GetClient())
            {
                return client.RemoveRangeFromSortedSetByScore(setId, startScore, endScore);
            }
        }

        public static List<string> GetAllItemsFromSet(string setId)
        {
            using (IRedisClient client = GetClient())
            {
                return client.GetAllItemsFromSortedSet(setId);
            }
        }

        public static long GetItemIndexInSortedSet(string setId, string value)
        {
            using (IRedisClient client = GetClient())
            {
                return client.GetItemIndexInSortedSet(setId, value);
            }
        }

        public static bool RemoveRangeFromSortedSet(string setId, int startIndex, int endIndex)
        {
            int count = endIndex - startIndex;
            using (IRedisClient client = GetClient())
            {
                try
                {
                    if (client.RemoveRangeFromSortedSet(setId, startIndex, endIndex) != count)
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region List

        public static bool AddItemToList(string listid, string value)
        {
            try
            {
                using (IRedisClient client = GetClient())
                {
                    client.AddItemToList(listid, value);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool RemoveStartFromList(string listid)
        {
            try
            {
                using (IRedisClient client = GetClient())
                {
                    client.RemoveStartFromList(listid);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static long GetListCount(string listid)
        {
            try
            {
                using (IRedisClient client = GetClient())
                {
                    return client.GetListCount(listid);
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public static IList<string> GetAllItemsFromList(string listid)
        {
            try
            {
                using (IRedisClient client = GetClient())
                {
                    return client.GetAllItemsFromList(listid);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool EnqueueItemOnList(string key, string value)
        {
            try
            {
                using (IRedisClient client = GetClient())
                {
                    client.EnqueueItemOnList(key, value);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static string DequeueItemFromList(string key)
        {
            try
            {
                using (IRedisClient client = GetClient())
                {
                    return client.DequeueItemFromList(key);
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        #endregion

    }
}