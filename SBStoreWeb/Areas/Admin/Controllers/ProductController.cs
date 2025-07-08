using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SBStore.DataAccess.Data;
using SBStore.DataAccess.Repository.IRepository;
using SBStore.Models;
using SBStore.Models.ViewModels;
using SBStore.Utility;
using System.Net.WebSockets;


namespace SBStoreWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();

            return View(objProductList);
        }
        public IActionResult Upsert(int? id)
        {
            ProductViewModel productViewModel = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select
               (u => new SelectListItem
               {
                   Text = u.Name,
                   Value = u.Id.ToString()
               }),
                Product = new Product()
            };
            if (id == null || id == 0)
            {
                //create
                return View(productViewModel);
            }
            else
            {
                //update
                productViewModel.Product = _unitOfWork.Product.Get(u => u.Id == id, includeProperties: "ProductImages");
                return View(productViewModel);
            }
        }
        [HttpPost]
        public IActionResult Upsert(ProductViewModel productViewModel, List<IFormFile> files)
        {

            if (ModelState.IsValid)
            {

                if (productViewModel.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productViewModel.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(productViewModel.Product);
                }

                _unitOfWork.Save();
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (files != null)
                {

                    foreach (IFormFile file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName); //random name for our file
                        string productPath = @"images\products\product-" + productViewModel.Product.Id;
                        string finalPath = Path.Combine(wwwRootPath, productPath);

                        if (!Directory.Exists(finalPath))
                        
                            Directory.CreateDirectory(finalPath);

                            //uploading a new image
                            using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                            {
                                file.CopyTo(fileStream);
                            }
                            ProductImage productImage = new()
                            {
                                ImageUrl = @"\" + productPath + @"\" + fileName,
                                ProductId = productViewModel.Product.Id,
                            };

                            if (productViewModel.Product.ProductImages == null)
                                productViewModel.Product.ProductImages = new List<ProductImage>();

                            productViewModel.Product.ProductImages.Add(productImage);

                        }
                        _unitOfWork.Product.Update(productViewModel.Product);
                        _unitOfWork.Save();
                    }
                    TempData["success"] = "Product has beean created successfully";
                    return RedirectToAction("Index", "Product");
                }
                else
                {
                    productViewModel.CategoryList = _unitOfWork.Category.GetAll().Select
                  (u => new SelectListItem
                  {
                      Text = u.Name,
                      Value = u.Id.ToString()
                  });
                    return View(productViewModel);
                }
            }
            public IActionResult DeleteImage(int imageId)
            {
                var imgToDelete = _unitOfWork.ProductImage.Get(u=>u.Id == imageId);
                int productId = imgToDelete.ProductId;
                if (imgToDelete != null)
                {
                    if(!string.IsNullOrEmpty(imgToDelete.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, imgToDelete.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    _unitOfWork.ProductImage.Delete(imgToDelete);
                    _unitOfWork.Save();

                    TempData["success"] = "Deleted successfully";
                }
                return RedirectToAction(nameof(Upsert), new { id = productId });
            }

            #region API CALLS

            [HttpGet]
            public IActionResult GetAll()
            {
                List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
                return Json(new { data = objProductList });
            }
            [HttpDelete]
            public IActionResult Delete(int? id)
            {
                var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);

                if (productToBeDeleted == null)
                {
                    return Json(new { success = false, message = "Error while removing" });
                }
           
            string productPath = @"images\products\product-" + id;
            string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);

            if (Directory.Exists(finalPath))
            {
                string[] filePaths = Directory.GetFiles(finalPath);
                foreach (string filePath in filePaths)
                {
                    System.IO.File.Delete(filePath);
                }

                Directory.Delete(finalPath);
            }
            _unitOfWork.Product.Delete(productToBeDeleted);
            _unitOfWork.Save();


            return Json(new { success = true, message = "Delete Successfull" });
            }


            #endregion
        }
    }
