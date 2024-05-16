using HouseKeeper.Enum;

namespace HouseKeeper.IServices
{
    public interface IGeneral
    {
        Task<bool> HasRightPassword(string password, int userId);
        Task<bool> ChangePassword(string password, int userId);
    }
}
