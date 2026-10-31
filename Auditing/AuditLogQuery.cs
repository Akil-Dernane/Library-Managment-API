namespace Library_API.Auditing
{
    public class AuditLogQuery
    {
        public int? ActorUserId { get; set; }
        public string? EventType { get; set; }
        public AuditOutcome? Outcome { get; set; }
        public DateTime? FromUtc { get; set; }
        public DateTime? ToUtc { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}
