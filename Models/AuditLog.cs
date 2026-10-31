using Library_API.Auditing;

namespace Library_API.Models
{
    public class AuditLog
    {
        public long Id { get; set; }
        public DateTime TimestampUtc { get; set; }

        public string EventType { get; set; } = string.Empty;
        public AuditOutcome Outcome { get; set; }

        public int? ActorUserId { get; set; }
        public string? ActorIdentifier { get; set; }

        public string? TargetType { get; set; }
        public string? TargetId { get; set; }

        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? TraceId { get; set; }

        public string? DetailsJson { get; set; }
    }
}
