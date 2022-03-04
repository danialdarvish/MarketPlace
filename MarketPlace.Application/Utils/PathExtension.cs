using System.IO;

namespace MarketPlace.Application.Utils
{
    public static class PathExtension
    {
        #region Default images

        public static string DefaultAvatar = "/img/defaults/avatar.jpg";

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
