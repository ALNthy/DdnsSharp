﻿using DdnsSharp.Core;
using DdnsSharp.Core.DdnsClient;
using DdnsSharp.IServices;
using Microsoft.AspNetCore.Components;

namespace DdnsSharp.Pages
{
    partial class Index
    {
        [Inject]
        IDdnsConfigService DdnsConfigService { get; set; }

    }
}