using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Persistence.Context;
using Votify.Persistence.Repositories;

namespace Votify.Persistence.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly VotifyContext _context;
        private IDbContextTransaction? _transaction;

        private IGenericRepository<Evento>? _eventos;
        private IGenericRepository<Proyecto>? _proyectos;
        private IGenericRepository<Categoria>? _categorias;
        private IGenericRepository<Voto>? _votos;
        private IGenericRepository<Premio>? _premios;
        private IGenericRepository<Criterio>? _criterios;
        private IGenericRepository<DetalleVoto>? _detallesVoto;
        private IGenericRepository<Miembro>? _miembros;
        private IGenericRepository<Votacion>? _votaciones;
        private IGenericRepository<Votante>? _votantes;
        private IGenericRepository<Participante>? _participantes;
        private IGenericRepository<Juez>? _jueces;
        private IGenericRepository<Organizador>? _organizadores;
        private IGenericRepository<Notificacion>? _notificaciones;

        private IEventoRepository? _eventoRepository;
        private ICategoriaRepository? _categoriaRepository;
        private IParticipanteRepository? _participanteRepository;
        private IPopularRepository? _popularRepository;
        private IPuntuacionRepository? _puntuacionRepository;
        private IVotoExpertoRepository? _votoExpertoRepository;
        private IVotoPopularRepository? _votoPopularRepository;
        private IVotoPuntuacionRepository? _votoPuntuacionRepository;
        private IVotoMulticriterioRepository? _votoMulticriterioRepository;
        private IAuditoriaRepository? _auditoriaRepository;

        public UnitOfWork(VotifyContext context)
        {
            _context = context;
        }

        public IGenericRepository<Evento> Eventos => _eventos ??= new GenericRepository<Evento>(_context);
        public IGenericRepository<Proyecto> Proyectos => _proyectos ??= new GenericRepository<Proyecto>(_context);
        public IGenericRepository<Categoria> Categorias => _categorias ??= new GenericRepository<Categoria>(_context);
        public IGenericRepository<Voto> Votos => _votos ??= new GenericRepository<Voto>(_context);
        public IGenericRepository<Premio> Premios => _premios ??= new GenericRepository<Premio>(_context);
        public IGenericRepository<Criterio> Criterios => _criterios ??= new GenericRepository<Criterio>(_context);
        public IGenericRepository<DetalleVoto> DetallesVoto => _detallesVoto ??= new GenericRepository<DetalleVoto>(_context);
        public IGenericRepository<Miembro> Miembros => _miembros ??= new GenericRepository<Miembro>(_context);
        public IGenericRepository<Votacion> Votaciones => _votaciones ??= new GenericRepository<Votacion>(_context);
        public IGenericRepository<Votante> Votantes => _votantes ??= new GenericRepository<Votante>(_context);
        public IGenericRepository<Participante> Participantes => _participantes ??= new GenericRepository<Participante>(_context);
        public IGenericRepository<Juez> Jueces => _jueces ??= new GenericRepository<Juez>(_context);
        public IGenericRepository<Organizador> Organizadores => _organizadores ??= new GenericRepository<Organizador>(_context);
        public IGenericRepository<Notificacion> Notificaciones => _notificaciones ??= new GenericRepository<Notificacion>(_context);

        public IEventoRepository EventoRepository => _eventoRepository ??= new EventoRepository(_context);
        public ICategoriaRepository CategoriaRepository => _categoriaRepository ??= new CategoriaRepository(_context);
        public IParticipanteRepository ParticipanteRepository => _participanteRepository ??= new ParticipanteRepository(_context);
        public IPopularRepository PopularRepository => _popularRepository ??= new PopularRepository(_context);
        public IPuntuacionRepository PuntuacionRepository => _puntuacionRepository ??= new PuntuacionRepository(_context);
        public IVotoExpertoRepository VotoExpertoRepository => _votoExpertoRepository ??= new VotoExpertoRepository(_context);
        public IVotoPopularRepository VotoPopularRepository => _votoPopularRepository ??= new VotoPopularRepository(_context);
        public IVotoPuntuacionRepository VotoPuntuacionRepository => _votoPuntuacionRepository ??= new VotoPuntuacionRepository(_context);
        public IVotoMulticriterioRepository VotoMulticriterioRepository => _votoMulticriterioRepository ??= new VotoMulticriterioRepository(_context);
        public IAuditoriaRepository AuditoriaRepository => _auditoriaRepository ??= new AuditoriaRepository(_context);

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                throw new InvalidOperationException("Ya existe una transacción activa. Llama a CommitTransactionAsync o RollbackTransactionAsync primero.");
            }
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No hay una transacción activa. Llama a BeginTransactionAsync primero.");
            }

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                await _transaction.CommitAsync(cancellationToken);
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No hay una transacción activa. Llama a BeginTransactionAsync primero.");
            }

            try
            {
                await _transaction.RollbackAsync(cancellationToken);
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
        }
    }
}
