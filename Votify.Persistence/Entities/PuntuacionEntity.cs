using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Votify.Core.Models;
using Votify.Core.Interfaces;

namespace Votify.Persistence.Entities
{
    public class PuntuacionEntity:VotacionEntity
    {
        public int ValorMax { get; set; }
    }
}
