// Contrato para persistir registros de auditoría
using botilleria_clean_architecture_api.Core.Domain.Entities;

namespace botilleria_clean_architecture_api.Core.Domain.Interfaces;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog auditLog);
}