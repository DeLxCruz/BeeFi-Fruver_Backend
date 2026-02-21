using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendConfirmationEmailAsync(string email, string firstName, string confirmationLink);
        Task SendPasswordResetEmailAsync(string email, string firstName, string resetLink);
        Task SendOrderConfirmationEmailAsync(string email, string orderNumber);
        
        /// <summary>
        /// Envía un email genérico
        /// </summary>
        Task SendEmailAsync(string to, string subject, string htmlBody);
    }
}