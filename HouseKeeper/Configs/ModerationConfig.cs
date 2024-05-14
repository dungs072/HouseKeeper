namespace HouseKeeper.Configs
{
    public class ModerationConfig
    {
        public static readonly int HoursAllowForEditRejectionNotes = 2;

        public static readonly string NotFoundNotification = "Recruitment not found or is deleted!";
        public static readonly string ServerErrorNotification = "Server Error, Try again!";
        public static readonly string HandledByOtherNotification = "Recruitment is handled by other staff!";
        public static string EditSuccessNotification(int recruitmentId) => $"Edit rejection notes for recruitment with Id = {recruitmentId} successfully!";
        public static string RejectSuccessNotification(int recruitmentId) => $"Reject recruitment with Id = {recruitmentId} successfully!";
        public static string AcceptSuccessNotification(int recruitmentId) => $"Accept recruitment with Id = {recruitmentId} successfully!";
    }
}
