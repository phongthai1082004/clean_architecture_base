using Domain.Abstractions.Entities;
using Domain.Enums.Users;

namespace Domain.Entities.Users
{
    public class AuditLog(
        Guid? userId,
        AuditAction action,
        string tableName,
        string recordId,
        string message,
        string? oldValues,
        string? newValues) : BaseEntity<int>
    {
        public Guid? UserId { get; private set; } = userId;
        public AuditAction Action { get; private set; } = action;
        public string TableName { get; private set; } = tableName;
        public string RecordId { get; private set; } = recordId;
        public string Message { get; private set; } = message;
        public string? OldValues { get; private set; } = oldValues;
        public string? NewValues { get; private set; } = newValues;
    }
}
