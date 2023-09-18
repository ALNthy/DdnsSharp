using DdnsSharp.Core;
using DdnsSharp.Core.DdnsClient;
using DdnsSharp.IServices;
using DdnsSharp.Model;
using Microsoft.AspNetCore.Components;

namespace DdnsSharp.Pages
{
    partial class Index
    {
        [Inject]
        IDdnsConfigService DdnsConfigService { get; set; }

        string ServiceName = ServiceType.DnsPod.ToString();
        class Person
        {
            public int? Value { get; set; }
            public string Name { get; set; }
        }

        List<Person> _persons;
        int? _selectedValue = null;

        protected override async Task OnInitializedAsync()
        {
            _persons = new List<Person>
        {
            new Person { Value = null, Name = "自动" },
            new Person { Value = 1, Name = "1秒" },
            new Person { Value = 5 , Name = "5秒" },
            new Person { Value = 10 , Name = "10秒" },
            new Person { Value = 60 , Name = "1分钟" },
            new Person { Value = 120 , Name = "2分钟" },
            new Person { Value = 600 , Name = "10分钟" },
            new Person { Value = 1800 , Name = "30分钟" },
            new Person { Value = 3600 , Name = "1小时" },
        };
        }
    }
}
