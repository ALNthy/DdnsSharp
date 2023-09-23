using DdnsSharp.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DdnsSharp.EFCore.Configs
{
    internal class NetworkConfigEFConfig: IEntityTypeConfiguration<NetworkConfig>
    {
        public void Configure(EntityTypeBuilder<NetworkConfig> builder)
        {
            builder.ToTable($"T_{nameof(NetworkConfig)}s");
            builder.HasKey(x => x.Guid);
            builder.Property(x => x.Netinterface).HasConversion<Netinterface>().HasConversion(
        v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
        v => JsonSerializer.Deserialize<Netinterface>(v, (JsonSerializerOptions)null));
        }
    }
}
