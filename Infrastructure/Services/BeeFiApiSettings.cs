using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class BeeFiApiSettings
    {
        public const string SectionName = "BeeFiApi";

        public string BaseUrl { get; init; } = null!;
        public string ApiKey { get; init; } = null!;
        public int TimeoutSeconds { get; init; } = 30;
    }
}