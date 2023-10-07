using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers {
    [Area("Admin")]
    public class CategoryController : Controller {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index() {
            var objCategoryList = _unitOfWork.Category.GetAll();

            return View(objCategoryList);
        }

        //GET
        public IActionResult Create() {
            return View();
        }

        //POST
        [HttpPost]
        public IActionResult Create(Category obj) {
            if (ModelState.IsValid) {
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category created successfully!";

                return RedirectToAction("Index");
            }

            return View();
        }

        //GET
        public IActionResult Edit(int? id) {
            if (id == null || id == 0) {
                return NotFound();
            }

            Category? categoryToEdit = _unitOfWork.Category.GetByFilter(c => c.Id == id);
            if (categoryToEdit == null) {
                return NotFound();
            }

            return View(categoryToEdit);
        }

        //POST
        [HttpPost]
        public IActionResult Edit(Category obj) {
            if (ModelState.IsValid) {
                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category edited successfully!";

                return RedirectToAction("Index");
            }

            return View();
        }

        //GET
        public IActionResult Delete(int? id) {
            if (id == null || id == 0) {
                return NotFound();
            }

            Category? categoryToDelete = _unitOfWork.Category.GetByFilter(c => c.Id == id);
            if (categoryToDelete == null) {
                return NotFound();
            }

            return View(categoryToDelete);
        }

        //POST
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id) {
            Category? categoryToDelete = _unitOfWork.Category.GetByFilter(c => c.Id == id);
            if (categoryToDelete == null) {
                return NotFound();
            }

            _unitOfWork.Category.Remove(categoryToDelete);
            _unitOfWork.Save();
            TempData["success"] = "Category deleted successfully!";

            return RedirectToAction("Index");
        }
    }
}