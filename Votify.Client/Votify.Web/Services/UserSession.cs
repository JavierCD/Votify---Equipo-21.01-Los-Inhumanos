using Votify.Core.Models;

namespace Votify.Web.Services
{
    public class UserSession
    {
        private Miembro? _usuario { get; set; }

        public event Action? OnChange;

        public Miembro? Usuario
        {
            get => _usuario;
            set
            {
                _usuario = value;
                NotifyStateChanged();
            }
        }

        private void NotifyStateChanged() => OnChange?.Invoke();

    }
}
    