using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Models
{
    public class Votante
    {
        public int Id { get; set; }

        // El email es obligatorio para saber quién es
        public string Email { get; set; }

        // Para saber si su identidad debe ocultarse al público/organizador
        public bool Anonimo { get; set; }
        //public string Rol {  get; set; }

        // Relación MUCHOS A MUCHOS: Un votante participa en varios eventos
        // y un evento tiene varios votantes.
        public List<Evento> Eventos { get; set; } = new List<Evento>();

        // Relación UNO A MUCHOS: Un votante emite varios votos 
        // (Corregido a lista para que pueda votar en distintas categorías)
        public List<Voto> Votos { get; set; } = new List<Voto>();
    }
}
