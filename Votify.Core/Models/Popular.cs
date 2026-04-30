using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Models
{
    public class Popular:Votacion{
        public int MaxSelection { get; set; }
        public bool PermiteAutoVoto { get; set; }
        
    }
}
