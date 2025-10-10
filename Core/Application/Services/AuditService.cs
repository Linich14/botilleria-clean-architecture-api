// Servicio para registrar operaciones de auditoría de seguridad
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using botilleria_clean_architecture_api.Core.Domain.Entities;
using botilleria_clean_architecture_api.Core.Domain.Interfaces;

namespace botilleria_clean_architecture_api.Core.Application.Services;

public class AuditService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task LogOperationAsync(string action, string entityType, int entityId, object? oldValues = null, object? newValues = null)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var userId = httpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Anonymous";

        var jsonOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            WriteIndented = false
        };

        var auditLog = new AuditLog
        {
            UserId = userId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            OldValues = oldValues != null ? JsonSerializer.Serialize(oldValues, jsonOptions) : null,
            NewValues = newValues != null ? JsonSerializer.Serialize(newValues, jsonOptions) : null,
            IpAddress = GetClientIpAddress(httpContext),
            UserAgent = httpContext?.Request.Headers["User-Agent"].ToString(),
            Timestamp = DateTime.UtcNow
        };

        await _unitOfWork.AuditLogs.AddAsync(auditLog);
        await _unitOfWork.SaveChangesAsync();
    }

    private string? GetClientIpAddress(HttpContext? context)
    {
        if (context == null) return null;

        // Check for IP in various headers (useful when behind proxy/load balancer)
        var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault()
               ?? context.Request.Headers["X-Real-IP"].FirstOrDefault()
               ?? context.Connection.RemoteIpAddress?.ToString();

        return ip;
    }
}