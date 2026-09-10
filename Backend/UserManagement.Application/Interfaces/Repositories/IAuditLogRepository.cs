using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Interfaces.Repositories
{
    public interface IAuditLogRepository
    {
        Task CreateAsync(AuditLog auditLog,CancellationToken cancellationToken);
    }
}
