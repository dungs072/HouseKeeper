using System.Net.NetworkInformation;

namespace HouseKeeper.Constant
{
    public class NotificationDuringModeration
    {
        public static string NotFoundNotification()
        {
            return $"Recruitment not found or is deleted!";
        }

        public static string ServerErrorNotification()
        {
            return $"Server Error, Try again!";
        }
        
        public static string HandledByOtherNotification()
        {
            return $"Recruitment is handled by other staff!";
        }

        public static string EditSuccessNotification(int recruitmentId)
        {
            return $"Edit rejection notes for recruitment with Id = {recruitmentId} successfully!";
        }
        public static string RejectSuccessNotification(int recruitmentId)
        {
            return $"Reject recruitment with Id = {recruitmentId} successfully!";
        }
        public static string AcceptSuccessNotification(int recruitmentId)
        {
            return $"Accept recruitment with Id = {recruitmentId} successfully!";
        }

    }
}
