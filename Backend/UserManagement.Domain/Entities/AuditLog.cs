using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Domain.Entities
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public ActionType actionType { get; set; }
        public int EntityId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public string IpAddress { get; set; }
        public string? OldEntityValues { get; set; }
        public string? NewEntityValues { get; set; }
        public string? ChangeDetails { get; set; } = String.Empty;

    }

    public enum ActionType
    {
        Add,
        Update,
        Delete
    }
}
