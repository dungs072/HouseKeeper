using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using HouseKeeper.Models.DB;

namespace HouseKeeper.IServices
{
    public interface IEmailService
    {
        bool SendEmailForRecruitmentApproval(TINTUYENDUNG recruitment);
        bool SendEmailForRecruitmentDisapproval(TINTUYENDUNG recruitment);
        bool SendEmail(string receiverEmail, string subject, string body);

    }
}
