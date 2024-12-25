using Microsoft.AspNetCore.Mvc;
using ProductStore_MVC_.Models;
using ProductStore_MVC_.Services;

namespace ProductStore_MVC_.Controllers
{
    public class ProductController : Controller
    {
        private readonly Application_Db_Context context;
        private readonly IWebHostEnvironment environment;



        public ProductController(Application_Db_Context context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
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
                ModelState.AddModelError("ImageFile", "The image file is required");

            }
            if (!ModelState.IsValid)
            {
                return View(productDto);
            }

            //Save the Image File
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(productDto.ImageFile!.FileName);

            string imageFullPath = environment.WebRootPath + "/images/" + newFileName;
            using (var stream = System.IO.File.OpenWrite(imageFullPath))
            {
                productDto.ImageFile.CopyTo(stream);
            }

            //save the New Product in the Data base
            Product product = new Product()
            {
                Name = productDto.Name,
                Brand = productDto.Brand,
                Category = productDto.Category,
                Price = productDto.Price,
                Description = productDto.Description,
                ImageFileName = newFileName,
                CreatedAt = DateTime.Now,
            };

            context.Products.Add(product);
            context.SaveChanges();

            return RedirectToAction("Index", "Product");

        }

        public IActionResult Edit(int id)

        {
            var product = context.Products.Find(id);

            if (product == null)

            {
                return RedirectToAction("Index", "Product");
            }
            //Create product Dto Form Product
            var productDto = new ProductDto()
            {
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
        public IActionResult Edit(int id, Product productDto)

        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Product");

            }
            if (!ModelState.IsValid)
            {
                ViewData["ProductId"] = product.Id;
                ViewData["ImageFileName"] = product.ImageFileName;
                ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");
               
                return View(productDto);
            }
        
            //Isses
            //update the image file if we have a new image file
            string newFileName= product.ImageFileName;

            if(productDto.ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName += Path.GetExtension(productDto.ImageFile.FileName);
                string imageFullPath=environment.WebRootPath +"/images/"+ newFileName;
                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    productDto.ImageFile.CopyTo(stream);
                }

                //delete the old iamges
                string oldImageFullPath = environment.WebRootPath + "/images/" + product.ImageFileName;
                System.IO.File.Delete(oldImageFullPath);
            }

            //Update the product in Database
            product.Name=productDto.Name;
            product.Brand = productDto.Brand;
            product.Category = productDto.Category;
            product.Price = productDto.Price;
            product.Description=productDto.Description;
            product.ImageFileName = newFileName;

            context.SaveChanges();
            return RedirectToAction("Index","Product");

        }
    }
}