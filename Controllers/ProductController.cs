using Microsoft.AspNetCore.Mvc;
using ProductStore_MVC_.Models;
using ProductStore_MVC_.Services;

namespace ProductStore_MVC_.Controllers
{
    public class ProductController : Controller
    {
        private readonly Application_Db_Context context;
        private readonly IWebHostEnvironment environment;

       

        public ProductController(Application_Db_Context context,IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }
        public IActionResult Index()
        { 
            var products=context.Products.OrderByDescending(p=>p.Id).ToList();
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

            string imageFullPath = environment.WebRootPath + "/images/" +newFileName;
            using (var stream = System.IO.File.OpenWrite(imageFullPath))
            {
                productDto.ImageFile.CopyTo(stream);
            }

            //save the New Product in the Data base
            Product product = new Product()
            {
                Name =productDto.Name,
                Brand=productDto.Brand,
                Category =productDto.Category,
                Price=productDto.Price,
                Description=productDto.Description,
                ImageFileName=newFileName,
                CreatedAt=DateTime.Now,
            };

            context.Products.Add(product);
            context.SaveChanges();
   
         return RedirectToAction("Index","Product");

        }

    }
}
