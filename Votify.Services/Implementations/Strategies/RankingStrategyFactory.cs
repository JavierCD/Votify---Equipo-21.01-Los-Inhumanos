using System;
using System.Collections.Generic;
using Votify.Core.Models;
using Votify.Services.Interfaces;

namespace Votify.Services.Implementations.Strategies
{
    public class RankingStrategyFactory
    {
        private readonly Dictionary<Type, IRankingStrategy> _strategies;

        public RankingStrategyFactory(
            MulticriterioRankingStrategy multicriterioStrategy,
            PopularRankingStrategy popularStrategy,
            PuntuacionRankingStrategy puntuacionStrategy)
        {
            _strategies = new Dictionary<Type, IRankingStrategy>
            {
                { typeof(Multicriterio), multicriterioStrategy },
                { typeof(Popular), popularStrategy },
                { typeof(Puntuacion), puntuacionStrategy }
            };
        }

        public IRankingStrategy GetStrategy(Votacion votacion)
        {
            var tipo = votacion.GetType();

            if (_strategies.TryGetValue(tipo, out var strategy))
                return strategy;

            throw new InvalidOperationException($"No hay estrategia de ranking configurada para el tipo de votación: {tipo.Name}");
        }
    }
}
