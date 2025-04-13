using BestStoreMVC.Models;
using BestStoreMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BestStoreMVC.Controllers
{
    public class Products : Controller
    {
        private readonly AplicationDbContext context;
        private readonly IWebHostEnvironment enviroment; // Fixed spelling and type

        public Products(AplicationDbContext context, IWebHostEnvironment enviroment) // Fixed parameter type and name
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context)); // Ensures context is not null
            this.enviroment = enviroment ?? throw new ArgumentNullException(nameof(enviroment)); // Ensures enviroment is not null
        }

        public IActionResult Index()
        {
            var products = context.Products.OrderByDescending(p => p.Id).ToList();
            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ProductDto productDto)
        {
            if (productDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "Please select an image file");
            }

            if (!ModelState.IsValid)
            {
                return View(productDto);
            }

            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmss");
            newFileName += Path.GetExtension(productDto.ImageFile!.FileName);

            string imageFullPath = Path.Combine(enviroment.WebRootPath, "products", newFileName);

            using (var stream = System.IO.File.Create(imageFullPath))
            {
                productDto.ImageFile.CopyTo(stream);
            }


            Product product = new Product
            {
                Name = productDto.Name,
                Brand = productDto.Brand,
                Category = productDto.Category,
                Price = productDto.Price,
                Description = productDto.Description,
                ImageFileName = newFileName,
                CreatedAt = DateTime.Now
            };

            context.Products.Add(product);
            context.SaveChanges();
            TempData["Success"] = "Product created successfully";

            return RedirectToAction("Index", "Products");
        }

        public IActionResult Edit(int id)
        {
            var product = context.Products.Find(id);

            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            var productDto = new ProductDto()
            {
                Id = product.Id,
                Name = product.Name,
                Brand = product.Brand,
                Category = product.Category,
                Price = product.Price,
                Description = product.Description,
            };

            ViewData["ProductId"] = product.Id;

            ViewData["ImageFileName"] = product.ImageFileName;

            ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");

            return View(productDto);
        }

        [HttpPost]
        public IActionResult Edit(int id, ProductDto productDto)
        {
            var product = context.Products.Find(id);

            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            if (ModelState.IsValid)
            {
                product.Name = productDto.Name;
                product.Brand = productDto.Brand;
                product.Category = productDto.Category;
                product.Price = productDto.Price;
                product.Description = productDto.Description;

                // Si hay una nueva imagen, procesarla
                if (productDto.ImageFile != null)
                {
                    string newFileName = DateTime.Now.ToString("yyyyMMddHHmmss");
                    newFileName += Path.GetExtension(productDto.ImageFile.FileName);

                    string imageFullPath = Path.Combine(enviroment.WebRootPath, "products", newFileName);

                    using (var stream = System.IO.File.Create(imageFullPath))
                    {
                        productDto.ImageFile.CopyTo(stream);
                    }

                    product.ImageFileName = newFileName;
                }

                context.Update(product);
                context.SaveChanges();
                TempData["Success"] = "Producto actualizado correctamente";

                return RedirectToAction("Index", "Products");
            }

            ViewData["ProductId"] = id;
            ViewData["ImageFileName"] = product.ImageFileName;
            ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");

            return View(productDto);
        }

        public IActionResult Delete(int id)
        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            string imageFullPath = Path.Combine(enviroment.WebRootPath, "products", product.ImageFileName);
            if (System.IO.File.Exists(imageFullPath))
            {
                System.IO.File.Delete(imageFullPath);
            }


            // Eliminar el producto de la base de datos
            context.Products.Remove(product);
            context.SaveChanges();
            TempData["Success"] = "Product deleted successfully";
            return RedirectToAction("Index", "Products");
        }

    }
}
