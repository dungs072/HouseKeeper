using HouseKeeper.Models.DB;
using System.Net.Mail;
using System.Net;
using HouseKeeper.IServices;

namespace HouseKeeper.Services
{
    public class EmailService : IEmailService
    {
        private string _credentialsUsername = "dungoc1235@gmail.com";
        private string _credentialsPassword = "kkacjdpsrwwwiuur";
        private string _smtpHost = "smtp.gmail.com";
        private int _smtpPort = 587;
        private string _senderEmail = "noreply@housekeeper.com";
        private string _senderName = "HouseKeeper";

        private string GetSalutation(string name)
        {
            return "Dear <b>" + name + "</b>,\r\n";
        }

        private string GetClosing(string staffName, string staffEmail, string staffPhone)
        {
            return "\r\n<p>Thanks,<br>\r\n" +
                "<b>" + staffName + "</b><br>\r\n" +
                "<b>Website Staff</b>\r\n" +

                "<p><b>Email:</b> " + staffEmail + "<br>\r\n" +
                "<b>Tell:</b> " + staffPhone + "</p>\r\n";
        }


        public bool SendEmailForRecruitmentApproval(TINTUYENDUNG recruitment)
        {
            var staff = recruitment.Staff;
            var employer = recruitment.Employer;
            string staffName = staff.FirstName + " " + staff.LastName;
            string staffEmail = staff.Account.Gmail;
            string staffPhone = staff.Account.PhoneNumber;

            string receiverName = employer.FirstName;
            string receiverEmail = employer.Account.Gmail;
            string recruitmentId = recruitment.RecruitmentId.ToString();

            string subject = "Recruitment Approval";
            string body = GetSalutation(receiverName) +

                "<p>Your recruitment (with ID: " + recruitmentId + ") approved successfully.<br>\r\n" +
                "Your recruitment will be displayed on the Website.<br>\r\n" +
                "Let me know if you need more any information.</p>\r\n" +

                GetClosing(staffName, staffEmail, staffPhone);
            return SendEmail(receiverEmail, subject, body);
        }

        public bool SendEmailForRecruitmentDisapproval(TINTUYENDUNG recruitment)
        {
            var staff = recruitment.Staff;
            var employer = recruitment.Employer;
            string staffName = staff.FirstName + " " + staff.LastName;
            string staffEmail = staff.Account.Gmail;
            string staffPhone = staff.Account.PhoneNumber;

            string receiverName = employer.FirstName;
            string receiverEmail = employer.Account.Gmail;
            string recruitmentId = recruitment.RecruitmentId.ToString();

            string subject = "Recruitment Disapproval";
            string body = GetSalutation(receiverName) +

                "<p>Your recruitment (with ID: " + recruitmentId + ") was rejected,<br>\r\n" +
                "Please correct the information in the recruitment form and post it again,<br>\r\n" +
                "The reason for rejection will be displayed in the recruitment form.<br>\r\n" +
                "Let me know if you need more any information.</p>\r\n" +

                GetClosing(staffName, staffEmail, staffPhone);
            return SendEmail(receiverEmail, subject, body);
        }

        public bool SendEmailForIdentityApproval(string receiverEmail, string receiverName, NHANVIEN staff)
        {
            string staffName = staff.FirstName + " " + staff.LastName;
            string staffEmail = staff.Account.Gmail;
            string staffPhone = staff.Account.PhoneNumber;

            string subject = "Account Approval";
            string body = GetSalutation(receiverName) +

                "<p>Your account has been approved.<br>\r\n" +
                "Let me know if you need more any information.</p>\r\n" +
            GetClosing(staffName, staffEmail, staffPhone);
            return SendEmail(receiverEmail, subject, body);
        }

        public bool SendEmailForIdentityDisapproval(string receiverEmail, string receiverName, string disapprovalReason, NHANVIEN staff)
        {
            string staffName = staff.FirstName + " " + staff.LastName;
            string staffEmail = staff.Account.Gmail;
            string staffPhone = staff.Account.PhoneNumber;

            string subject = "Account Disapproval";
            string body = GetSalutation(receiverName) +

                "<p>Your account has been disapproved because:\"" + disapprovalReason + "\",<br>\r\n" +
                "Please correct the information in the profile form and edit it again,<br>\r\n" +
                "Let me know if you need more any information.</p>\r\n" +
            GetClosing(staffName, staffEmail, staffPhone) + "<br>\r\n";
                
            return SendEmail(receiverEmail, subject, body);
        }


        public bool SendEmail(string receiverEmail, string subject, string body)
        {
            try
            {
                MailMessage mail = new MailMessage();

                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                mail.From = new MailAddress(_senderEmail, _senderName);
                mail.To.Add(new MailAddress(receiverEmail));
                mail.Priority = MailPriority.High;

                SmtpClient smtp = new SmtpClient(_smtpHost, _smtpPort);
                smtp.Credentials = new NetworkCredential(_credentialsUsername, _credentialsPassword);
                smtp.EnableSsl = true;
                smtp.Send(mail);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
