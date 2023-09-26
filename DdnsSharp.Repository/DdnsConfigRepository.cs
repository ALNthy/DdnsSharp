using DdnsSharp.EFCore;
using DdnsSharp.IRepository;
using DdnsSharp.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DdnsSharp.Repository
{
    public class DdnsConfigRepository:BaseRepository<DdnsConfig>,IDdnsConfigRepository
    {
        private readonly SqlDbContext _dbContext;
        public DdnsConfigRepository(SqlDbContext dbContext):base(dbContext)
        {
            _dbContext = dbContext;
        }
        public override async Task<DdnsConfig> FindOneAsync(Guid id)
        {
            return await this.FindOneAsync(x=>x.Guid==id);
        }
        public override async Task<bool> DeletedAsync(DdnsConfig entity)
        {   
            entity.Enable = false;
            return await base.DeletedAsync(entity);
        }
    }
}
