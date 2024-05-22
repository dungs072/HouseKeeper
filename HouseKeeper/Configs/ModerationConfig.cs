namespace HouseKeeper.Configs
{
    public class ModerationConfig
    {
        public static readonly int HoursAllowForEditRejectionNotes = 2;

        public static readonly string RecruimentNotFoundNotification = "Recruitment not found or is deleted!";
        public static readonly string ServerErrorNotification = "Server Error, Try again!";
        public static readonly string RecruitmentHandledByOtherNotification = "Recruitment is handled by other staff!";
        public static string EditRecruitmentSuccessNotification(int recruitmentId) => $"Edit rejection notes for recruitment with Id = {recruitmentId} successfully!";
        public static string RejectRecruitmentSuccessNotification(int recruitmentId) => $"Reject recruitment with Id = {recruitmentId} successfully!";
        public static string ApproveRecruitmentSuccessNotification(int recruitmentId) => $"Approve recruitment with Id = {recruitmentId} successfully!";
        public static string ApproveEmployeeSuccessNotification(int employeeId) => $"Approve employee with Id = {employeeId} successfully!";
        public static string DisapproveEmployeeSuccessNotification(int employeeId) => $"Disapprove employee with Id = {employeeId} successfully!";
        public static string ApproveEmployerSuccessNotification(int employerId) => $"Approve employer with Id = {employerId} successfully!";
        public static string DisapproveEmployerSuccessNotification(int employerId) => $"Disapprove employer with Id = {employerId} successfully!";
    }
}
