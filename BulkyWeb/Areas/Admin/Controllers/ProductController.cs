using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers {
    [Area("Admin")]
    public class ProductController : Controller {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment) {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index() {
            var objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category");

            return View(objProductList);
        }

        //GET
        public IActionResult Upsert(int? id) {
            //IEnumerable<SelectListItem> categoryList = _unitOfWork.Category.GetAll().Select(c => new SelectListItem {
            //    Text = c.Name,
            //    Value = c.Id.ToString()
            //});
            //ViewBag.CategoryList = categoryList;
            //ViewData["CategoryList"] = categoryList;

            ProductVM productVM = new() { 
                Product = new Product(),
                CategoryList = _unitOfWork.Category.GetAll().Select(c => new SelectListItem {
                    Text = c.Name,
                    Value = c.Id.ToString()
                })
            };

            if (id != null && id != 0) {
                productVM.Product = _unitOfWork.Product.GetByFilter(p => p.Id == id);    
            }

            return View(productVM);
        }

        //POST
        [HttpPost]
        public IActionResult Upsert(ProductVM obj, IFormFile? file) {
            if (ModelState.IsValid) {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null) {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");
                    
                    if (!string.IsNullOrEmpty(obj.Product.ImageUrl)) {
                        //Delete the old img
                        var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath)) {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var filestream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create)) {
                        file.CopyTo(filestream);
                    }

                    obj.Product.ImageUrl = @"\images\product\" + fileName;
                }

                if (obj.Product.Id == 0) {
                    _unitOfWork.Product.Add(obj.Product);
                    TempData["success"] = "Product created successfully!";
                } else {
                    _unitOfWork.Product.Update(obj.Product);
                    TempData["success"] = "Product updated successfully!";
                }
                _unitOfWork.Save();

                return RedirectToAction("Index");
            } else {
                obj.CategoryList = _unitOfWork.Category.GetAll().Select(c => new SelectListItem {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });

                return View(obj);
            }
        }

        #region API CALLS
        
        [HttpGet]
        public IActionResult GetAll() {
            var objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category");

            return Json(new { data = objProductList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id) {
            var productToDelete = _unitOfWork.Product.GetByFilter(p => p.Id == id);

            if (productToDelete == null) {
                return Json(new { success = false, message = "Error while deleting" });
            }

            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToDelete.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagePath)) {
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.Product.Remove(productToDelete);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete successful" });
        }

        #endregion
    }
}