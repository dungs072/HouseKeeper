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
        bool SendEmailForIdentityApproval(string receiverEmail, string receiverName, NHANVIEN staff);
        bool SendEmailForIdentityDisapproval(string receiverEmail, string receiverName, string disapprovalReason, NHANVIEN staff);
        bool SendEmail(string receiverEmail, string subject, string body);

    }
}
