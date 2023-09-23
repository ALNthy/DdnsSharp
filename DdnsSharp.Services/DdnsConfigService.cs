using DdnsSharp.IRepository;
using DdnsSharp.IServices;
using DdnsSharp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DdnsSharp.Services
{
    public class DdnsConfigService:BaseService<DdnsConfig>,IDdnsConfigService
    {
        private readonly IDdnsConfigRepository _service;
        public DdnsConfigService(IDdnsConfigRepository service):base(service)
        { 
            _service = service;
        }

        public async Task<bool> FindAnyAsync(Guid id)
        {
            return await _service.FindAnyAsync(x=>x.Guid == id);
        }
    }
}
