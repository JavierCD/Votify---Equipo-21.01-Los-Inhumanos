using System;
using System.Threading.Tasks;
using Votify.Core.Models;

namespace Votify.Core.Models
{
    public class SingleVoteRestrictionHandler : AbstractVotoValidationHandler
    {
        private readonly Func<int, string, Task<bool>> _emailYaVoto;

        public SingleVoteRestrictionHandler(Func<int, string, Task<bool>> emailYaVoto)
        {
            _emailYaVoto = emailYaVoto;
        }

        public override async Task HandleAsync(VotoValidationContext context)
        {
            if (context.Votacion == null)
                throw new InvalidOperationException("No se puede validar restricción de voto sin votación.");

            if (!string.IsNullOrWhiteSpace(context.Email) && context.Votacion.RestriccionVotoUnico)
            {
                bool yaVoto = await _emailYaVoto(context.VotacionId, context.Email);
                if (yaVoto)
                    throw new InvalidOperationException("Este correo electrónico ya ha emitido su voto en esta votación.");
            }

            await HandleNextAsync(context);
        }
    }
}
