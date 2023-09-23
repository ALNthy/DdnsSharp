using DdnsSharp.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DdnsSharp.EFCore.Configs
{
    internal class DdnsConfigEFConfig : IEntityTypeConfiguration<DdnsConfig>
    {
        public void Configure(EntityTypeBuilder<DdnsConfig> builder)
        {
            builder.ToTable($"T_{nameof(DdnsConfig)}s"); 
            builder.HasKey(x=>x.Guid);
        }
    }
}
