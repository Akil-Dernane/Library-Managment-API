
using Library_API.Auditing;
using Library_API.Authorization;
using Library_API.Data;
using Library_API.Models;
using Library_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library_API.Controllers.Admin;

[ApiController]
[Route("api/admin/audit-logs")]
[HasPermission(AppPermissions.AuditLogs.ReadAll)]
public class AdminAuditLogsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IAuditService _audit;
    private readonly ICurrentUserService _actor;

    public AdminAuditLogsController(
        AppDbContext context,
        IAuditService audit,
        ICurrentUserService actor)
    {
        _context = context;
        _audit = audit;
        _actor = actor;
    }

    [HttpGet]
    public async Task<ActionResult<AuditLogPageDto>> GetAll(
        [FromQuery] AuditLogQuery q,
        CancellationToken cancellationToken)
    {
        if (!int.TryParse(_actor.UserId, out int actorUserId))
            return Unauthorized();

        if (q.FromUtc.HasValue &&
            q.ToUtc.HasValue &&
            q.FromUtc >= q.ToUtc)
        {
            return BadRequest("FromUtc must be earlier than ToUtc.");
        }

        int page = Math.Max(q.Page, 1);
        int pageSize = Math.Clamp(q.PageSize, 1, 200);

        IQueryable<AuditLog> query = _context.AuditLogs.AsNoTracking();

        if (q.ActorUserId.HasValue)
            query = query.Where(a => a.ActorUserId == q.ActorUserId);

        if (!string.IsNullOrWhiteSpace(q.EventType))
            query = query.Where(a => a.EventType == q.EventType);

        if (q.Outcome.HasValue)
            query = query.Where(a => a.Outcome == q.Outcome);

        if (q.FromUtc.HasValue)
            query = query.Where(a => a.TimestampUtc >= q.FromUtc);

        if (q.ToUtc.HasValue)
            query = query.Where(a => a.TimestampUtc < q.ToUtc);

        int totalCount = await query.CountAsync(cancellationToken);

        int skip = (int)Math.Min(
            ((long)page - 1) * pageSize,
            int.MaxValue);

        var logs = await query
            .OrderByDescending(a => a.TimestampUtc)
            .ThenByDescending(a => a.Id)
            .Skip(skip)
            .Take(pageSize)
            .Select(a => new AuditLogDto(
                a.Id,
                a.TimestampUtc,
                a.EventType,
                a.Outcome.ToString(),
                a.ActorUserId,
                a.ActorIdentifier,
                a.TargetType,
                a.TargetId,
                a.IpAddress,
                a.TraceId,
                a.DetailsJson
            ))
            .ToListAsync(cancellationToken);

        await _audit.WriteAsync(new AuditEntry(
            AuditEvents.AuditLogsRead,
            AuditOutcome.Success,
            ActorUserId: actorUserId,
            ActorIdentifier: _actor.Email,
            Details: new
            {
                page,
                pageSize,
                totalCount,
                resultsCount = logs.Count,
                filters = new
                {
                    actorUserId = q.ActorUserId,
                    eventType = q.EventType,
                    outcome = q.Outcome?.ToString(),
                    fromUtc = q.FromUtc,
                    toUtc = q.ToUtc
                }
            }
        ));

        return Ok(new AuditLogPageDto(
            logs,
            totalCount,
            page,
            pageSize
        ));
    }
}
