namespace Library_API.Auditing
{
    public record AuditLogDto(
        long Id, 
        DateTime TimestampUtc, 
        string EventType, 
        string Outcome,
        int? ActorUserId, 
        string? ActorIdentifier, 
        string? TargetType, 
        string? TargetId,
        string? IpAddress, 
        string? TraceId, 
        string? DetailsJson
    );
}
