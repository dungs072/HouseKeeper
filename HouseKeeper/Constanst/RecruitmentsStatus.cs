using HouseKeeper.DBContext;

namespace HouseKeeper.Contanst
{
    public static class RecruitmentsStatus
    {
        // get status string for recruitment
        public static string Displayed = "Displayed";
        public static string PendingApproval = "Pending approval";
        public static string RejectApproval = "Reject approval";
        public static string Hidden = "Hidden";
        public static string Expired = "Expired";


        public static string[] status = new string[] { PendingApproval, RejectApproval,
                                                    Displayed, Hidden, Expired };
        


        // get status id by status name
        public static int GetStatusId(string statusName)
        {
            for (int i = 0; i < status.Length; i++)
            {
                if (status[i] == statusName)
                {
                    return i + 1;
                }
            }
            return 0;
        }

    }
}
