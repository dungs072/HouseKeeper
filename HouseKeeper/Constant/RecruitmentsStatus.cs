using HouseKeeper.DBContext;

namespace HouseKeeper.Constant
{
    public static class RecruitmentsStatus
    {
        // get status string for recruitment
        public readonly static string Displayed = "Displayed";
        public readonly static string PendingApproval = "Pending approval";
        public readonly static string RejectApproval = "Reject approval";
        public readonly static string Hidden = "Hidden";
        public readonly static string Expired = "Expired";


        public readonly static string[] status = new string[] { PendingApproval, RejectApproval,
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
