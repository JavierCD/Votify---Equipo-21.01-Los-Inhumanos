using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Votify.Core.Interfaces;
using Votify.Core.Models;

namespace Votify.Persistence.Repositories
{
    public class CachedEventoRepository : IEventoRepository
    {
        private readonly IEventoRepository _inner;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _defaultExpiration;

        public CachedEventoRepository(IEventoRepository inner, IMemoryCache cache, TimeSpan? expiration = null)
        {
            _inner = inner;
            _cache = cache;
            _defaultExpiration = expiration ?? TimeSpan.FromMinutes(5);
        }

        public async Task<Evento?> GetByIdAsync(int id)
        {
            string cacheKey = $"Evento_ById_{id}";

            if (_cache.TryGetValue(cacheKey, out Evento? cached))
                return cached;

            var entity = await _inner.GetByIdAsync(id);

            if (entity != null)
                _cache.Set(cacheKey, entity, _defaultExpiration);

            return entity;
        }

        public async Task<IEnumerable<Evento>> GetAllAsync()
        {
            string cacheKey = "Evento_All";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<Evento>? cached))
                return cached!;

            var entities = await _inner.GetAllAsync();
            var list = entities.ToList();
            _cache.Set(cacheKey, list, _defaultExpiration);
            return list;
        }

        public async Task<Evento?> GetWithIncludesAsync(System.Linq.Expressions.Expression<Func<Evento, bool>> predicate, params System.Linq.Expressions.Expression<Func<Evento, object>>[] includes)
        {
            string cacheKey = $"Evento_WithIncludes_{predicate.Body}_{string.Join(",", includes.Select(i => i.Body.ToString()))}";

            if (_cache.TryGetValue(cacheKey, out Evento? cached))
                return cached;

            var entity = await _inner.GetWithIncludesAsync(predicate, includes);
            if (entity != null)
                _cache.Set(cacheKey, entity, _defaultExpiration);

            return entity;
        }

        public async Task<IEnumerable<Evento>> GetAllWithIncludesAsync(params System.Linq.Expressions.Expression<Func<Evento, object>>[] includes)
        {
            string cacheKey = $"Evento_AllWithIncludes_{string.Join(",", includes.Select(i => i.Body.ToString()))}";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<Evento>? cached))
                return cached!;

            var entities = await _inner.GetAllWithIncludesAsync(includes);
            var list = entities.ToList();
            _cache.Set(cacheKey, list, _defaultExpiration);
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
                _cache.Set(cacheKey, entity, TimeSpan.FromMinutes(10));

            return entity;
        }

        public async Task<IEnumerable<Evento>> ObtenerEventosPorJuezAsync(int juezId)
        {
            string cacheKey = $"Evento_PorJuez_{juezId}";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<Evento>? cached))
                return cached!;

            var entities = await _inner.ObtenerEventosPorJuezAsync(juezId);
            var list = entities.ToList();
            _cache.Set(cacheKey, list, _defaultExpiration);
            return list;
        }

        public async Task<IEnumerable<Evento>> ObtenerEventosDisponiblesAsync()
        {
            string cacheKey = "Evento_Disponibles";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<Evento>? cached))
                return cached!;

            var entities = await _inner.ObtenerEventosDisponiblesAsync();
            var list = entities.ToList();
            _cache.Set(cacheKey, list, TimeSpan.FromMinutes(2));
            return list;
        }

        public async Task<IEnumerable<Evento>> ObtenerEventosPorParticipanteAsync(int participanteId)
        {
            string cacheKey = $"Evento_PorParticipante_{participanteId}";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<Evento>? cached))
                return cached!;

            var entities = await _inner.ObtenerEventosPorParticipanteAsync(participanteId);
            var list = entities.ToList();
            _cache.Set(cacheKey, list, _defaultExpiration);
            return list;
        }

        private void ClearCache()
        {
        }
    }
}
