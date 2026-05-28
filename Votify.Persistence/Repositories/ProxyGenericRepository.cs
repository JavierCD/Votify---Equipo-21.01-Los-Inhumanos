using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using Votify.Core.Interfaces;

namespace Votify.Persistence.Repositories
{
    public class ProxyGenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly IGenericRepository<T> _realSubject;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _defaultExpiration;
        private CancellationTokenSource _resetTokenSource;

        public ProxyGenericRepository(IGenericRepository<T> realSubject, IMemoryCache cache, TimeSpan? expiration = null)
        {
            _realSubject = realSubject;
            _cache = cache;
            _defaultExpiration = expiration ?? TimeSpan.FromMinutes(5);
            _resetTokenSource = new CancellationTokenSource();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            string cacheKey = $"{typeof(T).Name}_ById_{id}";

            if (_cache.TryGetValue(cacheKey, out T? cached))
                return cached;

            var entity = await _realSubject.GetByIdAsync(id);

            if (entity != null)
            {
                var options = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _defaultExpiration
                };
                options.AddExpirationToken(new CancellationChangeToken(_resetTokenSource.Token));
                _cache.Set(cacheKey, entity, options);
            }

            return entity;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            string cacheKey = $"{typeof(T).Name}_All";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<T>? cached))
                return cached!;

            var entities = await _realSubject.GetAllAsync();

            var list = entities.ToList();
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _defaultExpiration
            };
            options.AddExpirationToken(new CancellationChangeToken(_resetTokenSource.Token));
            _cache.Set(cacheKey, list, options);

            return list;
        }

        public async Task<T?> GetWithIncludesAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            string cacheKey = $"{typeof(T).Name}_WithIncludes_{predicate.Body}_{string.Join(",", includes.Select(i => i.Body.ToString()))}";

            if (_cache.TryGetValue(cacheKey, out T? cached))
                return cached;

            var entity = await _realSubject.GetWithIncludesAsync(predicate, includes);

            if (entity != null)
            {
                var options = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _defaultExpiration
                };
                options.AddExpirationToken(new CancellationChangeToken(_resetTokenSource.Token));
                _cache.Set(cacheKey, entity, options);
            }

            return entity;
        }

        public async Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includes)
        {
            string cacheKey = $"{typeof(T).Name}_AllWithIncludes_{string.Join(",", includes.Select(i => i.Body.ToString()))}";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<T>? cached))
                return cached!;

            var entities = await _realSubject.GetAllWithIncludesAsync(includes);

            var list = entities.ToList();
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _defaultExpiration
            };
            options.AddExpirationToken(new CancellationChangeToken(_resetTokenSource.Token));
            _cache.Set(cacheKey, list, options);

            return list;
        }

        public async Task<T> AddAsync(T entity)
        {
            var result = await _realSubject.AddAsync(entity);
            ClearCache();
            return result;
        }

        public async Task UpdateAsync(T entity)
        {
            await _realSubject.UpdateAsync(entity);
            ClearCache();
        }

        public async Task DeleteAsync(int id)
        {
            await _realSubject.DeleteAsync(id);
            ClearCache();
        }

        private void ClearCache()
        {
            var oldTokenSource = Interlocked.Exchange(ref _resetTokenSource, new CancellationTokenSource());
            oldTokenSource.Cancel();
            oldTokenSource.Dispose();
        }
    }
}
