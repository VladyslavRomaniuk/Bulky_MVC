using Bulky.DataAccess.Data;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers {
    public class CategoryController : Controller {
        private readonly ApplicationDbContext _db;
        
        public CategoryController(ApplicationDbContext db) {
            _db = db;
        }

        public IActionResult Index() {
            var objCategoryList = _db.Categories.ToList();
            
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
                _db.Categories.Add(obj);
                _db.SaveChanges();
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

            Category? categoryToEdit = _db.Categories.FirstOrDefault(c => c.Id == id);
            if (categoryToEdit == null) {
                return NotFound();
            }

            return View(categoryToEdit);
        }

        //POST
        [HttpPost]
        public IActionResult Edit(Category obj) {
            if (ModelState.IsValid) {
                _db.Categories.Update(obj);
                _db.SaveChanges();
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

            Category? categoryToDelete = _db.Categories.FirstOrDefault(c => c.Id == id);
            if (categoryToDelete == null) {
                return NotFound();
            }

            return View(categoryToDelete);
        }

        //POST
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id) {
            Category? categoryToDelete = _db.Categories.FirstOrDefault(c => c.Id == id);
            if (categoryToDelete == null) {
                return NotFound();
            }

            _db.Categories.Remove(categoryToDelete);
            _db.SaveChanges();
            TempData["success"] = "Category deleted successfully!";

            return RedirectToAction("Index");
        }
    }
}
