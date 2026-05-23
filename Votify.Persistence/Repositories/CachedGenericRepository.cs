using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Votify.Core.Interfaces;

namespace Votify.Persistence.Repositories
{
    public class CachedGenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly IGenericRepository<T> _inner;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _defaultExpiration;

        public CachedGenericRepository(IGenericRepository<T> inner, IMemoryCache cache, TimeSpan? expiration = null)
        {
            _inner = inner;
            _cache = cache;
            _defaultExpiration = expiration ?? TimeSpan.FromMinutes(5);
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            string cacheKey = $"{typeof(T).Name}_ById_{id}";

            if (_cache.TryGetValue(cacheKey, out T? cached))
                return cached;

            var entity = await _inner.GetByIdAsync(id);

            if (entity != null)
            {
                _cache.Set(cacheKey, entity, _defaultExpiration);
            }

            return entity;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            string cacheKey = $"{typeof(T).Name}_All";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<T>? cached))
                return cached!;

            var entities = await _inner.GetAllAsync();

            var list = entities.ToList();
            _cache.Set(cacheKey, list, _defaultExpiration);

            return list;
        }

        public async Task<T?> GetWithIncludesAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            string cacheKey = $"{typeof(T).Name}_WithIncludes_{predicate.Body}_{string.Join(",", includes.Select(i => i.Body.ToString()))}";

            if (_cache.TryGetValue(cacheKey, out T? cached))
                return cached;

            var entity = await _inner.GetWithIncludesAsync(predicate, includes);

            if (entity != null)
            {
                _cache.Set(cacheKey, entity, _defaultExpiration);
            }

            return entity;
        }

        public async Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includes)
        {
            string cacheKey = $"{typeof(T).Name}_AllWithIncludes_{string.Join(",", includes.Select(i => i.Body.ToString()))}";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<T>? cached))
                return cached!;

            var entities = await _inner.GetAllWithIncludesAsync(includes);

            var list = entities.ToList();
            _cache.Set(cacheKey, list, _defaultExpiration);

            return list;
        }

        public async Task<T> AddAsync(T entity)
        {
            var result = await _inner.AddAsync(entity);
            ClearCache();
            return result;
        }

        public async Task UpdateAsync(T entity)
        {
            await _inner.UpdateAsync(entity);
            ClearCache();
        }

        public async Task DeleteAsync(int id)
        {
            await _inner.DeleteAsync(id);
            ClearCache();
        }

        private void ClearCache()
        {
        }
    }
}
