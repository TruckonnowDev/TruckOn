using MonkeyCache.SQLite;
using System;

namespace MDispatch.Service.Cache
{
    public class CacheService : ICacheService
    {


        public void Add(string key, string data, TimeSpan expireIn, string eTag = null)
        {
            Barrel.Current.Add<string>(key, data, expireIn, eTag);
        }

        public void Add<T>(string key, T data, TimeSpan expireIn, string eTag = null)
        {
            Barrel.Current.Add<T>(key, data, expireIn, eTag);
        }

        public void Empty(params string[] key)
        {
            Barrel.Current.Empty(key);
        }

        public void EmptyAll()
        {
            Barrel.Current.EmptyAll();
        }

        public void EmptyExpired()
        {
            Barrel.Current.EmptyExpired();
        }

        public bool Exists(string key)
        {
            return Barrel.Current.Exists(key);
        }

        public string Get(string key)
        {
            return Barrel.Current.Get<string>(key);
        }

        public T Get<T>(string key)
        {
            return Barrel.Current.Get<T>(key);
        }

        public string GetETag(string key)
        {
            return Barrel.Current.GetETag(key);
        }

        public bool IsExpired(string key)
        {
            return Barrel.Current.IsExpired(key);
        }
    }
}
