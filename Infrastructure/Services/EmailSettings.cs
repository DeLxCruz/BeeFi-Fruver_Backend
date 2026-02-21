using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class EmailSettings
    {
        public const string SectionName = "EmailSettings";

        public string SmtpServer { get; init; } = null!;
        public int SmtpPort { get; init; }
        public string SenderEmail { get; init; } = null!;
        public string SenderName { get; init; } = null!;
        public string Username { get; init; } = null!;
        public string Password { get; init; } = null!;
        public bool EnableSsl { get; init; }
    }
}