using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using Votify.Core.Interfaces;
using Votify.Core.Models;

namespace Votify.Persistence.Repositories
{
    public class CachedEventoRepository : IEventoRepository
    {
        private readonly IEventoRepository _inner;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _defaultExpiration;
        private CancellationTokenSource _resetTokenSource;

        public CachedEventoRepository(IEventoRepository inner, IMemoryCache cache, TimeSpan? expiration = null)
        {
            _inner = inner;
            _cache = cache;
            _defaultExpiration = expiration ?? TimeSpan.FromMinutes(5);
            _resetTokenSource = new CancellationTokenSource();
        }

        public async Task<Evento?> GetByIdAsync(int id)
        {
            string cacheKey = $"Evento_ById_{id}";

            if (_cache.TryGetValue(cacheKey, out Evento? cached))
                return cached;

            var entity = await _inner.GetByIdAsync(id);

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

        public async Task<IEnumerable<Evento>> GetAllAsync()
        {
            string cacheKey = "Evento_All";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<Evento>? cached))
                return cached!;

            var entities = await _inner.GetAllAsync();
            var list = entities.ToList();
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _defaultExpiration
            };
            options.AddExpirationToken(new CancellationChangeToken(_resetTokenSource.Token));
            _cache.Set(cacheKey, list, options);
            return list;
        }

        public async Task<Evento?> GetWithIncludesAsync(System.Linq.Expressions.Expression<Func<Evento, bool>> predicate, params System.Linq.Expressions.Expression<Func<Evento, object>>[] includes)
        {
            string cacheKey = $"Evento_WithIncludes_{predicate.Body}_{string.Join(",", includes.Select(i => i.Body.ToString()))}";

            if (_cache.TryGetValue(cacheKey, out Evento? cached))
                return cached;

            var entity = await _inner.GetWithIncludesAsync(predicate, includes);
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

        public async Task<IEnumerable<Evento>> GetAllWithIncludesAsync(params System.Linq.Expressions.Expression<Func<Evento, object>>[] includes)
        {
            string cacheKey = $"Evento_AllWithIncludes_{string.Join(",", includes.Select(i => i.Body.ToString()))}";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<Evento>? cached))
                return cached!;

            var entities = await _inner.GetAllWithIncludesAsync(includes);
            var list = entities.ToList();
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _defaultExpiration
            };
            options.AddExpirationToken(new CancellationChangeToken(_resetTokenSource.Token));
            _cache.Set(cacheKey, list, options);
            return list;
        }

        public async Task<Evento> AddAsync(Evento entity)
        {
            var result = await _inner.AddAsync(entity);
            ClearCache();
            return result;
        }

        public async Task UpdateAsync(Evento entity)
        {
            await _inner.UpdateAsync(entity);
            ClearCache();
        }

        public async Task DeleteAsync(int id)
        {
            await _inner.DeleteAsync(id);
            ClearCache();
        }

        public async Task<Evento?> ObtenerEventoConDetallesAsync(int id)
        {
            string cacheKey = $"Evento_Detalles_{id}";

            if (_cache.TryGetValue(cacheKey, out Evento? cached))
                return cached;

            var entity = await _inner.ObtenerEventoConDetallesAsync(id);
            if (entity != null)
            {
                var options = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                };
                options.AddExpirationToken(new CancellationChangeToken(_resetTokenSource.Token));
                _cache.Set(cacheKey, entity, options);
            }

            return entity;
        }

        public async Task<IEnumerable<Evento>> ObtenerEventosPorJuezAsync(int juezId)
        {
            string cacheKey = $"Evento_PorJuez_{juezId}";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<Evento>? cached))
                return cached!;

            var entities = await _inner.ObtenerEventosPorJuezAsync(juezId);
            var list = entities.ToList();
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _defaultExpiration
            };
            options.AddExpirationToken(new CancellationChangeToken(_resetTokenSource.Token));
            _cache.Set(cacheKey, list, options);
            return list;
        }

        public async Task<IEnumerable<Evento>> ObtenerEventosDisponiblesAsync()
        {
            string cacheKey = "Evento_Disponibles";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<Evento>? cached))
                return cached!;

            var entities = await _inner.ObtenerEventosDisponiblesAsync();
            var list = entities.ToList();
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
            };
            options.AddExpirationToken(new CancellationChangeToken(_resetTokenSource.Token));
            _cache.Set(cacheKey, list, options);
            return list;
        }

        public async Task<IEnumerable<Evento>> ObtenerEventosPorParticipanteAsync(int participanteId)
        {
            string cacheKey = $"Evento_PorParticipante_{participanteId}";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<Evento>? cached))
                return cached!;

            var entities = await _inner.ObtenerEventosPorParticipanteAsync(participanteId);
            var list = entities.ToList();
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _defaultExpiration
            };
            options.AddExpirationToken(new CancellationChangeToken(_resetTokenSource.Token));
            _cache.Set(cacheKey, list, options);
            return list;
        }

        private void ClearCache()
        {
            var oldTokenSource = Interlocked.Exchange(ref _resetTokenSource, new CancellationTokenSource());
            oldTokenSource.Cancel();
            oldTokenSource.Dispose();
        }
    }
}
