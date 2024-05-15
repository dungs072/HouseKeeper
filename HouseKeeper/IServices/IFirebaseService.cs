using HouseKeeper.Enum;

namespace HouseKeeper.IServices
{
    public interface IFirebaseService
    {
        Task<string?> UploadImage(IFormFile file, AccountEnum.AccountType accountType, ImageEnum.ImageType imageType);
    }
}
