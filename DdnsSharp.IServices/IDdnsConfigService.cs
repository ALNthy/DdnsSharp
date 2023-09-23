using DdnsSharp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DdnsSharp.IServices
{
    public interface IDdnsConfigService:IBaseService<DdnsConfig>
    {
        public Task<bool> FindAnyAsync(Guid id);
    }
}
