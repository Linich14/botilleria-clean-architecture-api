// Registro de todas las operaciones realizadas en el sistema para auditoría
using System.Text.Json;

namespace botilleria_clean_architecture_api.Core.Domain.Entities;

public class AuditLog
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!; // Usuario que realizó la operación
    public string Action { get; set; } = null!; // CREATE, UPDATE, DELETE
    public string EntityType { get; set; } = null!; // Product, Brand, etc.
    public int EntityId { get; set; } // ID de la entidad afectada
    public string? OldValues { get; set; } // Valores anteriores (JSON)
    public string? NewValues { get; set; } // Valores nuevos (JSON)
    public string? IpAddress { get; set; } // Dirección IP del cliente
    public string? UserAgent { get; set; } // Navegador/dispositivo
    public DateTime Timestamp { get; set; } // Momento exacto de la operación
}