using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DdnsSharp.Core.Model
{
    public class RecordInfoListItem
    {
        //
        // 摘要:
        //     记录Id
        [JsonProperty("RecordId")]
        public ulong? RecordId { get; set; }

        //
        // 摘要:
        //     记录值
        [JsonProperty("Value")]
        public string Value { get; set; }

        //
        // 摘要:
        //     记录状态，启用：ENABLE，暂停：DISABLE
        [JsonProperty("Status")]
        public string Status { get; set; }

        //
        // 摘要:
        //     更新时间
        [JsonProperty("UpdatedOn")]
        public string UpdatedOn { get; set; }

        //
        // 摘要:
        //     主机名
        [JsonProperty("Name")]
        public string Name { get; set; }

        //
        // 摘要:
        //     记录类型
        [JsonProperty("Type")]
        public string Type { get; set; }

        //
        // 摘要:
        //     记录权重，用于负载均衡记录 注意：此字段可能返回 null，表示取不到有效值。
        [JsonProperty("Weight")]
        public ulong? Weight { get; set; }

        //
        // 摘要:
        //     记录备注说明
        [JsonProperty("Remark")]
        public string Remark { get; set; }

        //
        // 摘要:
        //     记录缓存时间
        [JsonProperty("TTL")]
        public ulong? TTL { get; set; }
    }
}
