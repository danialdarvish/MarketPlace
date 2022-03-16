using System.IO;

namespace MarketPlace.Application.Utils
{
    public static class PathExtension
    {
        #region Default images

        public static string DefaultAvatar = "/img/defaults/avatar.jpg";

        #endregion

        #region Uploader

        public static string UploadImage = "/img/upload/";
        public static string UploadImageServer = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/upload/");

        #endregion

        #region Products

        public static string ProductImage = "/content/images/product/origin/";
        public static string ProductImageServer = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/content/images/product/origin/");

        public static string ProductThumbnailImage = "/content/images/product/thumb/";
        public static string ProductImageThumbnailServer = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/content/images/product/thumb/");

        #endregion

        #region Slider

        public static string SliderOrigin = "/img/slider/";

        #endregion

        #region Banner

        public static string BannerOrigin = "/img/bg/";

        #endregion

        #region User avatar

        public static string UserAvatarOrigin = "/Content/Images/UserAvatar/origin/";
        public static string UserAvatarOriginServer = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Content/Images/UserAvatar/origin/");

        public static string UserAvatarThumb = "/Content/Images/UserAvatar/Thump/";
        public static string UserAvatarThumbServer = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Content/Images/UserAvatar/Thump/");

        #endregion
    }
}
