using Microsoft.EntityFrameworkCore.Scaffolding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Persistence.Context;
using Votify.Persistence.Entities;

namespace Votify.Persistence.Repositories
{
    public class VotanteRepository : IVotanteRepository
    {
        private readonly VotifyContext _context;

        public VotanteRepository(VotifyContext context)
        {
            _context = context;
        }

        /**
         * Insertar(Votante votante) es una función que convierte un objeto del modelo (Core) a entidad de EF
         */
        public void Insertar(Votante votante)
        {
            var entity = new VotanteEntity
            {
                Email = votante.Email,
                Anonimo = votante.Anonimo,
            };

            _context.Votantes.Add(entity);
            _context.SaveChanges();

            votante.Id = entity.Id;

        }

        public Votante ObternerPorId(int id)
        {
            var entidad = _context.Votantes.Find(id);
            if (entidad == null) return null;

            return new Votante
            {
                Id = entidad.Id,
                Email = entidad.Email,
                Anonimo = entidad.Anonimo,
            };
        }
    }
}
