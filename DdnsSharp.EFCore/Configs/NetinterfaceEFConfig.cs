using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DdnsSharp.Model;

namespace DdnsSharp.EFCore.Configs
{
    internal class NetinterfaceEFConfig : IEntityTypeConfiguration<Netinterface>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Netinterface> builder)
        {
            builder.ToTable($"T_{nameof(Netinterface)}s");
            builder.HasKey(x=>x.Guid);
        }
    }
}
