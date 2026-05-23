using System.Threading.Tasks;
using Votify.Core.Models;

namespace Votify.Core.Interfaces
{
    public interface IVotoValidationHandler
    {
        IVotoValidationHandler SetNext(IVotoValidationHandler next);
        Task HandleAsync(VotoValidationContext context);
    }
}
