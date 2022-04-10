using System;
using System.Threading.Tasks;
using MarketPlace.Application.Services.Interfaces;
using MarketPlace.DataLayer.DTOs.Product;
using MarketPlace.Web.Http;
using MarketPlace.Web.PresentationExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlace.Web.Areas.Seller.Controllers
{
    public class ProductController : SellerBaseController
    {
        #region Constructor

        private readonly ISellerService _sellerService;
        private readonly IProductService _productService;

        public ProductController(ISellerService sellerService, IProductService productService)
        {
            _sellerService = sellerService;
            _productService = productService;
        }

        #endregion

        #region Product

        #region List

        [HttpGet("products-list")]
        public async Task<IActionResult> Index(FilterProductDto filter)
        {
            var seller = await _sellerService.GetLastActiveSellerByUserId(User.GetUserId());
            filter.SellerId = seller.Id;
            filter.FilterProductState = FilterProductState.All;
            return View(await _productService.FilterProducts(filter));
        }

        #endregion

        #region Create product

        [HttpGet("create-product")]
        public async Task<IActionResult> CreateProduct()
        {
            ViewBag.Categories = await _productService.GetAllActiveProductCategories();
            return View();
        }

        [HttpPost("create-product"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(CreateProductDto product, IFormFile productImage)
        {
            if (ModelState.IsValid)
            {
                var seller = await _sellerService.GetLastActiveSellerByUserId(User.GetUserId());
                var result = await _productService.CreateProduct(product, seller.Id, productImage);

                switch (result)
                {
                    case CreateProductResult.Success:
                        TempData[SuccessMessage] = $"محصول مورد نظر با عنوان {product.Title} با موفقیت ثبت شد ";
                        return RedirectToAction("Index");
                    case CreateProductResult.HasNoImage:
                        TempData[WarningMessage] = "لطفا تصویر محصول را وارد نمایید";
                        break;
                    case CreateProductResult.Error:
                        TempData[ErrorMessage] = "عملیات ثبت محصول با خطا مواجه شد";
                        break;
                }
            }

            ViewBag.Categories = await _productService.GetAllProductCategoriesByParentId(null);
            return View(product);
        }

        #endregion

        #region Edit product

        [HttpGet("edit-product/{productId}")]
        public async Task<IActionResult> EditProduct(long productId)
        {
            var product = await _productService.GetProductForEdit(productId);
            if (product == null) return NotFound();
            ViewBag.Categories = await _productService.GetAllActiveProductCategories();

            return View(product);
        }

        [HttpPost("edit-product/{productId}"), ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(long productId, EditProductDto product, IFormFile productImage)
        {
            if (ModelState.IsValid)
            {
                var result = await _productService
                    .EditSellerProduct(product, User.GetUserId(), productImage);

                switch (result)
                {
                    case EditProductResult.NotFound:
                        TempData[ErrorMessage] = "اطلاعات وارد شده یافت نشد";
                        break;
                    case EditProductResult.NotForUser:
                        TempData[WarningMessage] = "در ویرایش اطلاعات خطایی رخ داد";
                        break;
                    case EditProductResult.Success:
                        TempData[SuccessMessage] = "عملیات با موفقیت انجام شد";
                        return RedirectToAction("Index");
                }
            }

            ViewBag.Categories = await _productService.GetAllActiveProductCategories();

            return View(product);
        }

        #endregion


        #endregion

        #region Product Galleries

        #region List

        [HttpGet("product-galleries/{id}")]
        public async Task<IActionResult> GetProductGalleries(long id)
        {
            ViewBag.productId = id;
            var seller = await _sellerService.GetLastActiveSellerByUserId(User.GetUserId());
            return View(await _productService.GetAllProductGalleriesInSellerPanel(id, seller.Id));
        }

        #endregion

        #region Create

        [HttpGet("create-product-gallery/{productId}")]
        public async Task<IActionResult> CreateProductGallery(long productId)
        {
            var product = await _productService.GetProductBySellerOwnerId(productId, User.GetUserId());
            if (product == null) return NotFound();
            ViewBag.product = product;

            return View();
        }

        [HttpPost("create-product-gallery/{productId}"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProductGallery(long productId, CreateOrEditProductGalleryDto gallery)
        {
            if (ModelState.IsValid)
            {
                var seller = await _sellerService.GetLastActiveSellerByUserId(User.GetUserId());
                var result = await _productService.CreateProductGallery(gallery, productId, seller.Id);

                switch (result)
                {
                    case CreateOrEditProductGalleryResult.Success:
                        TempData[SuccessMessage] = "عملیات ثبت گالری محصول با موفقیت انجام شد";
                        return RedirectToAction("GetProductGalleries", "Product", new { id = productId });
                    case CreateOrEditProductGalleryResult.NotForUserProduct:
                        TempData[ErrorMessage] = "لطفا شلوغ کاری نکنید";
                        break;
                    case CreateOrEditProductGalleryResult.ImageIsNull:
                        TempData[WarningMessage] = "تصویر مربوطه را وارد نمایید";
                        break;
                    case CreateOrEditProductGalleryResult.GalleryNotFound:
                        TempData[WarningMessage] = "محصول مورد نظر پیدا نشد";
                        break;
                }
            }

            var product = await _productService.GetProductBySellerOwnerId(productId, User.GetUserId());
            if (product == null) return NotFound();
            ViewBag.product = product;

            return View(gallery);
        }

        #endregion

        #region Edit

        [HttpGet("product_{productId}/edit-gallery/{galleryId}")]
        public async Task<IActionResult> EditGallery(long productId, long galleryId)
        {
            var seller = await _sellerService.GetLastActiveSellerByUserId(User.GetUserId());
            var mainGallery = await _productService.GetProductGalleryForEdit(galleryId, seller.Id);
            if (mainGallery == null) return NotFound();

            return View(mainGallery);
        }


        [HttpPost("product_{productId}/edit-gallery/{galleryId}")]
        public async Task<IActionResult> EditGallery(long productId, long galleryId, CreateOrEditProductGalleryDto gallery)
        {
            if (ModelState.IsValid)
            {
                var seller = await _sellerService.GetLastActiveSellerByUserId(User.GetUserId());
                var result = await _productService.EditProductGallery(galleryId, seller.Id, gallery);

                switch (result)
                {
                    case CreateOrEditProductGalleryResult.Success:
                        TempData[SuccessMessage] = "اطلاعات مورد نظر با موفقیت ویرایش شد";
                        return RedirectToAction("GetProductGalleries", "Product", new { id = productId });
                    case CreateOrEditProductGalleryResult.NotForUserProduct:
                        TempData[ErrorMessage] = "این اطلاعات برای شما غیر قابل دسترسی می باشند";
                        break;
                    case CreateOrEditProductGalleryResult.ImageIsNull:
                        TempData[ErrorMessage] = "لطفا تصویر را به درستی انتخاب کنید";
                        break;
                    case CreateOrEditProductGalleryResult.GalleryNotFound:
                        TempData[WarningMessage] = "اطلاعات مورد نظر یافت نشد";
                        break;
                }
            }
            return View();
        }

        #endregion

        #endregion

        #region Product categories

        [HttpGet("product-categories/{parentId}")]
        public async Task<IActionResult> GetProductCategoriesByParent(long parentId)
        {
            var categories = await _productService.GetAllProductCategoriesByParentId(parentId);

            return JsonResponseStatus.SendStatus(JsonResponseStatusType.Success,
                "اطلاعات دسته بندی ها", categories);
        }

        #endregion
    }
}
