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

        public override async Task<List<DdnsConfig>> FindAllAsync(Expression<Func<DdnsConfig, bool>> del)
        {
            return await _dbContext.ddnsConfigs.Where(del).Include(x=>x.IPV4).Include(x=>x.IPV6).ToListAsync();
        }
        public override async Task<List<DdnsConfig>> FindAllAsync()
        {
            return await this.FindAllAsync(x=>x.Enable);
        }
        public override async Task<DdnsConfig> FindOneAsync(Expression<Func<DdnsConfig, bool>> del)
        {
            return await _dbContext.ddnsConfigs.Include(x => x.IPV4).Include(x => x.IPV6).FirstOrDefaultAsync(del);
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
