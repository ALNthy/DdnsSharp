﻿using DdnsSharp.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DdnsSharp.EFCore.Configs
{
    internal class DdnsConfigEFConfig : IEntityTypeConfiguration<DdnsConfig>
    {
        public void Configure(EntityTypeBuilder<DdnsConfig> builder)
        {
            builder.ToTable($"T_{nameof(DdnsConfig)}s"); 
            builder.HasKey(x=>x.Guid);
            builder.Property(x => x.IPV4).HasConversion<NetworkConfig>().HasConversion(
v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
v => JsonSerializer.Deserialize<NetworkConfig>(v, (JsonSerializerOptions)null));
            builder.Property(x => x.IPV6).HasConversion<NetworkConfig>().HasConversion(
v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
v => JsonSerializer.Deserialize<NetworkConfig>(v, (JsonSerializerOptions)null));
        }
    }
}
