using Firebase.Auth;
using Firebase.Storage;
using HouseKeeper.Enum;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using HouseKeeper.IServices;
using Microsoft.Extensions.Options;

namespace HouseKeeper.Services
{
    /// <summary>
    /// Service for handling Firebase operations.
    /// </summary>
    public class FirebaseService : IFirebaseService
    {
        private readonly HouseKeeper.Configs.FirebaseConfig _firebaseConfig;

        public FirebaseService(IOptions<HouseKeeper.Configs.FirebaseConfig> firebaseConfig)
        {
            _firebaseConfig = firebaseConfig.Value;
        }

        /// <summary>
        /// Uploads a file to Firebase.
        /// </summary>
        /// <param name="file">The file to upload.</param>
        /// <returns>The URL of the uploaded file, or null if the upload failed.</returns>
        public async Task<string?> UploadImage(IFormFile file, AccountEnum.AccountType accountType, ImageEnum.ImageType imageType)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            // Get the file name and extension
            var fileName = Path.GetFileName(file.FileName);
            var fileExtension = Path.GetExtension(fileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var folderName = GetFolderName(imageType, accountType);

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;

            return await UploadImageToFireBase(stream, folderName, uniqueFileName); ;
            
        }

        /// <summary>
        /// Uploads a file to Firebase.
        /// </summary>
        /// <param name="stream">The stream containing the file data.</param>
        /// <param name="folderName">The name of the folder to upload the file to.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>The URL of the uploaded file.</returns>
        private async Task<string> UploadImageToFireBase(MemoryStream stream, string folderName, string fileName)
        {
            var authProvider = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(_firebaseConfig.ApiKey));
            var authResult = await authProvider.SignInWithEmailAndPasswordAsync(_firebaseConfig.AuthEmail, _firebaseConfig.AuthPassword);

            var storage = new FirebaseStorage(
                _firebaseConfig.Bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(authResult.FirebaseToken),
                    ThrowOnCancel = true
                });

            return await storage
                .Child(folderName)
                .Child(fileName)
                .PutAsync(stream);
        }

        /// <summary>
        /// Gets the folder name based on the account type and image type.
        /// </summary>
        /// <param name="accountType">The type of the account.</param>
        /// <param name="imageType">The type of the image.</param>
        /// <returns>The folder name.</returns>
        private string GetFolderName(ImageEnum.ImageType imageType, AccountEnum.AccountType accountType)
        {
            return $"{GetImageFolderName(imageType)}/{GetAccountFolderName(accountType)}";
        }

        /// <summary>
        /// Gets the account folder name based on the account type.
        /// </summary>
        /// <param name="accountType">The type of the account.</param>
        /// <returns>The account folder name.</returns>
        private string GetAccountFolderName(AccountEnum.AccountType accountType)
        {
            return accountType switch
            {
                AccountEnum.AccountType.Employee => _firebaseConfig.EmployeeFolder,
                AccountEnum.AccountType.Employer => _firebaseConfig.EmployerFolder,
                AccountEnum.AccountType.Staff => _firebaseConfig.StaffFolder,
                _ => string.Empty,
            };
        }

        /// <summary>
        /// Gets the image folder name based on the image type.
        /// </summary>
        /// <param name="imageType">The type of the image.</param>
        /// <returns>The image folder name.</returns>
        private string GetImageFolderName(ImageEnum.ImageType imageType)
        {
            return imageType switch
            {
                ImageEnum.ImageType.Avatar => _firebaseConfig.IdentityFolder,
                ImageEnum.ImageType.Identity => _firebaseConfig.AvatarFolder,
                _ => string.Empty,
            };
            
        }
    }
}

