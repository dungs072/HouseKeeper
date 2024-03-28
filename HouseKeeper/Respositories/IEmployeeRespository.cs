using HouseKeeper.Models.DB;

namespace HouseKeeper.Respositories
{
    public interface IEmployeeRespository
    {
        Task<List<TINTUYENDUNG>> GetRecruitments(int page);
        Task<TINTUYENDUNG> GetRecruitment(int id);
    }
}
