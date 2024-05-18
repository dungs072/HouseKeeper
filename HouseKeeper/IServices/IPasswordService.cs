namespace HouseKeeper.IServices
{
    public interface IPasswordService
    {
        public string HashPassword(string password);
    }
}
