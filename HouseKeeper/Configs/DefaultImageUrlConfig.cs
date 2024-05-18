using static System.Net.WebRequestMethods;

namespace HouseKeeper.Configs
{
    public class DefaultImageUrlConfig
    {
        private const string BaseUrl = "https://firebasestorage.googleapis.com/v0/b/housekeeperimage.appspot.com/o/";

        public readonly static string DefaultAvatarStaff = $"{BaseUrl}Avatar%2FStaff%2FDefault%2Fdefault-avatar-staff.png?alt=media&token=6c20c712-5667-4e6e-90db-cf20ba27bdb1";
        public readonly static string FrontImage = $"{BaseUrl}Identity%2FDefault%2FFront%2Fdefault-front-image.jpg?alt=media&token=7f79c43e-2a83-4e45-8bc2-39130bc1e5bc";
        public readonly static string BackImage = $"{BaseUrl}Identity%2FDefault%2FBack%2Fdefault-back-image.jpg?alt=media&token=7f79c43e-2a83-4e45-8bc2-39130bc1e5bc";
        public readonly static string DefaultAvatarEmployee = $"{BaseUrl}Avatar%2FEmployee%2FDefault%2Fhousekeeping-male-worker-with-mop-avatar-character_18591-67781.avif?alt=media&token=74dd9818-f4e0-4629-b302-63869e04896b";
        public readonly static string DefaultAvatarEmployer = $"{BaseUrl}Avatar%2FEmployer%2FDefault%2Favatar-default-icon.png?alt=media&token=7c98ec49-9d07-4c9a-b932-f189658fec62";
    }
}
