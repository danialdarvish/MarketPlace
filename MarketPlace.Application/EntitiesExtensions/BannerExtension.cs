using MarketPlace.Application.Utils;
using MarketPlace.DataLayer.Entities.Site;

namespace MarketPlace.Application.EntitiesExtensions
{
    public static class BannerExtension
    {
        public static string GetBannerMainImageAddress(this SiteBanner banner)
        {
            return PathExtension.BannerOrigin + banner.ImageName;
        }
    }
}
