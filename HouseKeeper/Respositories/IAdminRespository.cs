using HouseKeeper.Models.DB;

namespace HouseKeeper.Respositories
{
    public interface IAdminRespository
    {
        Task<List<LOAICONGVIEC>> GetJobTypes();
    }
}
