using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Application.Interfaces.Repositories;
using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure.Persistance.Respositories
{
    public class AuditLogRespository : IAuditLogRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public AuditLogRespository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        public async Task CreateAsync(AuditLog auditLog,CancellationToken cancellationToken)
        {
            _applicationDbContext.AuditLogs.Add(auditLog);
            await _applicationDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
