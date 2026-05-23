using System.Threading.Tasks;
using Votify.Core.Interfaces;
using Votify.Core.Models;

namespace Votify.Core.Models
{
    public abstract class AbstractVotoValidationHandler : IVotoValidationHandler
    {
        private IVotoValidationHandler? _next;

        public IVotoValidationHandler SetNext(IVotoValidationHandler next)
        {
            _next = next;
            return next;
        }

        public abstract Task HandleAsync(VotoValidationContext context);

        protected async Task HandleNextAsync(VotoValidationContext context)
        {
            if (_next != null)
                await _next.HandleAsync(context);
        }
    }
}
