using System.Threading.Tasks;
using MarketPlace.Application.Services.Interfaces;
using MarketPlace.DataLayer.DTOs.SellerWallet;
using MarketPlace.Web.PresentationExtensions;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlace.Web.Areas.Seller.Controllers
{
    public class SellerWalletController : SellerBaseController
    {
        #region Constructor
        
        private readonly ISellerService _sellerService;
        private readonly ISellerWalletService _sellerWalletService;

        public SellerWalletController(ISellerService sellerService, ISellerWalletService sellerWalletService)
        {
            _sellerService = sellerService;
            _sellerWalletService = sellerWalletService;
        }

        #endregion

        #region Index

        [HttpGet("seller-wallet")]
        public async Task<IActionResult> Index(FilterSellerWalletDto filter)
        {
            filter.TakeEntity = 5;
            var seller = await _sellerService.GetLastActiveSellerByUserId(User.GetUserId());
            if (seller == null) return NotFound();

            filter.SellerId = seller.Id;
            return View(await _sellerWalletService.FilterSellerWalletDto(filter));
        }

        #endregion
    }
}
