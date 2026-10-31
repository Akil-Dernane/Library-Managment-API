namespace Library_API.Auditing
{
    public record AuditEntry(
       string EventType,
       AuditOutcome Outcome,
       int? ActorUserId = null,
       string? ActorIdentifier = null,
       string? TargetType = null,
       string? TargetId = null,
       object? Details = null);
}
