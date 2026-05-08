using Votify.Core.Enums;

namespace Votify.Core.Models
{
    public class VotacionStateChangedArgs
    {
        public Votacion Votacion { get; set; } = null!;
        public Evento Evento { get; set; } = null!;
        public VotacionStateEventType EventType { get; set; }
        public DateTime TriggeredAt { get; set; } = DateTime.UtcNow;
    }
}
