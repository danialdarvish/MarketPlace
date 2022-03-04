using MarketPlace.Application.Services.Interfaces;
using MarketPlace.DataLayer.Entities.Site;
using MarketPlace.DataLayer.Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketPlace.Application.Services.Implementations
{
    public class SiteService : ISiteService
    {
        #region Constructor

        private readonly IGenericRepository<SiteBanner> _siteBanners;
        private readonly IGenericRepository<Slider> _sliderRepository;
        private readonly IGenericRepository<SiteSetting> _siteSettingRepository;
        public SiteService(IGenericRepository<SiteSetting> siteSettingRepository, IGenericRepository<Slider> sliderRepository, IGenericRepository<SiteBanner> siteBanners)
        {
            _siteBanners = siteBanners;
            _sliderRepository = sliderRepository;
            _siteSettingRepository = siteSettingRepository;
        }

        #endregion

        #region Site Setting
        public async Task<SiteSetting> GetDefaultSiteSetting()
        {
            return await _siteSettingRepository.GetQuery().FirstOrDefaultAsync(x => x.IsDefault && !x.IsDelete);
        }
        #endregion

        #region Slider

        public async Task<List<Slider>> GetAllActiveSliders()
        {
            return await _sliderRepository.GetQuery().Where(x => x.IsActive && !x.IsDelete).ToListAsync();
        }

        #endregion

        #region Site Banners

        public async Task<List<SiteBanner>> GetSiteBannersByPlacement(List<BannerPlacement> placements)
        {
            return await _siteBanners.GetQuery()
                .Where(x => placements.Contains(x.BannerPlacement)).ToListAsync();
        }

        #endregion

        #region Dispose
        public async ValueTask DisposeAsync()
        {
            if (_siteSettingRepository != null)
                await _siteSettingRepository.DisposeAsync();
            if (_sliderRepository != null)
                await _sliderRepository.DisposeAsync();
            if (_siteBanners != null)
                await _siteBanners.DisposeAsync();
        }
        #endregion
    }
}
 