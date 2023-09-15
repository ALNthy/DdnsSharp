using DdnsSharp.EFCore;
using DdnsSharp.IRepository;
using DdnsSharp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
