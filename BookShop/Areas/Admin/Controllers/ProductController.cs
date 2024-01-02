using Book.DataAccess.Data;
using Book.DataAccess.Repository.IRepository;
using Book.Models;
using Book.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Drawing.Drawing2D;

namespace BookShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ROLE_ADMIN)]
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
            List<Product> products = _unitOfWork.Product.GetAll(includeProp:"Category").ToList();
            return View(products);
        }
        public IActionResult Upsert(int? id)
        {
            IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });

            ViewBag.CategoryList = CategoryList;
            Product product = new Product();

            if (id == null || id == 0)
            {
                //Create
                return View(product);
            }
            else
            {
                //Update
                product = _unitOfWork.Product.Get(u => u.Id == id);
                return View(product);
            }
        }
        [HttpPost]
        public IActionResult Upsert(Product obj, IFormFile? file)
        {
            ModelState.Remove("Category");
            ModelState.Remove("ImageUrl");
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");
                    if (!string.IsNullOrEmpty(obj.ImageUrl))
                    {
                        var oldImage = Path.Combine(wwwRootPath, obj.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImage)) 
                        {
                            System.IO.File.Delete(oldImage);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    obj.ImageUrl = @"\images\product\" + fileName;
                }
                if (obj.Id == 0)
                {
                    _unitOfWork.Product.Add(obj);
                }
                else
                {
                    _unitOfWork.Product.Update(obj);
                    
                }
                _unitOfWork.Save();
                TempData["success"] = "Product updated successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                ViewBag.CategoryList = CategoryList;
                return RedirectToAction("Upsert");
            }
        }
        #region API
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> products = _unitOfWork.Product.GetAll(includeProp: "Category").ToList();
            return Json(new { data = products });
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productDel = _unitOfWork.Product.Get(u => u.Id == id);
            if (productDel == null) 
            {
                return Json(new { success = false, massage = "Error" });
            }
            var oldImage = Path.Combine(_webHostEnvironment.WebRootPath, productDel.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImage))
            {
                System.IO.File.Delete(oldImage);
            }
            _unitOfWork.Product.Remove(productDel);
            _unitOfWork.Save();
            return Json(new { success = true, massage = "Succesfully" });
        }
        #endregion
    }
}
