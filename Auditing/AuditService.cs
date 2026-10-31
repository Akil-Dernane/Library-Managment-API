using Library_API.Data;
using Library_API.Models;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace Library_API.Auditing
{
    public class AuditService : IAuditService
    {
        private readonly AppDbContext _context;
        private readonly IServiceScopeFactory _scopes;
        private readonly IHttpContextAccessor _http;
        private readonly ILogger<AuditService> _logger;

        public AuditService(
             AppDbContext context,
             IServiceScopeFactory scopes,
             IHttpContextAccessor http,
             ILogger<AuditService> logger)
        {
            _context = context;
            _scopes = scopes;
            _http = http;
            _logger = logger;
        }
        public void Record(AuditEntry entry)
        {
            AuditLog log = Build(entry);
            _context.AuditLogs.Add(log);
            Emit(log);
        }

        public async Task WriteAsync(AuditEntry entry)
        {
            AuditLog log = Build(entry);
            try
            {
                using IServiceScope scope = _scopes.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.AuditLogs.Add(log);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to persist audit entry {EventType}", entry.EventType);
            }
            Emit(log);
        }

        private AuditLog Build(AuditEntry e)
        {
            HttpContext? http = _http.HttpContext;

            int? actorId = e.ActorUserId;
            if (actorId is null && int.TryParse(http?.User.FindFirstValue(JwtRegisteredClaimNames.Sub), out int id))
                actorId = id;

            return new AuditLog
            {
                TimestampUtc = DateTime.UtcNow,
                EventType = e.EventType,
                Outcome = e.Outcome,
                ActorUserId = actorId,
                ActorIdentifier = e.ActorIdentifier,
                TargetType = e.TargetType,
                TargetId = e.TargetId,
                IpAddress = http?.Connection.RemoteIpAddress?.ToString(),
                UserAgent = Truncate(http?.Request.Headers.UserAgent.ToString(), 256),
                TraceId = Activity.Current?.TraceId.ToString() ?? http?.TraceIdentifier,
                DetailsJson = e.Details is null ? null : JsonSerializer.Serialize(e.Details)
            };
        }

        private void Emit(AuditLog l)
        {
            _logger.LogInformation(
            "AUDIT {EventType} {Outcome} actor={ActorUserId} target={TargetType}:{TargetId} ip={Ip} trace={TraceId}",
            l.EventType, l.Outcome, l.ActorUserId, l.TargetType, l.TargetId, l.IpAddress, l.TraceId);
        }

        private static string? Truncate(string? s, int max)
        {
            return s is null || s.Length <= max ? s : s[..max];
        }
    }
}
