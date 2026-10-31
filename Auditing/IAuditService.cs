
namespace Library_API.Auditing
{
    public interface IAuditService
    {
        /// <summary>Adds the entry to the CALLER'S DbContext. It is saved together with the business change (atomic).</summary>
        void Record(AuditEntry entry);

        /// <summary>Writes the entry on its own, immediately, independent of whatever the caller is doing.</summary>
        Task WriteAsync(AuditEntry entry);
    }
}
