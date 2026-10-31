namespace Library_API.Auditing
{
    public record AuditLogPageDto(
        IReadOnlyList<AuditLogDto> Items,
        int TotalCount,
        int Page,
        int PageSize
    );
}
