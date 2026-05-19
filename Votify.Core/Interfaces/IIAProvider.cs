namespace Votify.Core.Interfaces
{
    public interface IIAProvider
    {
        Task<string> AnalizarAsync(string prompt);
    }
}
