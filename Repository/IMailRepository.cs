using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mcshippers_Task.Models
{
    public interface IMailRepository
    {

        Task SendEmailAsync(string toEmail, string subject, string content);
    }

   
}
