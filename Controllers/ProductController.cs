using Microsoft.AspNetCore.Mvc;
using ProductStore_MVC_.Services;

namespace ProductStore_MVC_.Controllers
{
    public class ProductController : Controller
    {
        private Application_Db_Context context;

        public ProductController(Application_Db_Context context)
        {
            this.context = context;
        }
        public IActionResult Index()
        { 
            var products=context.Products.ToList();
            return View(products);
        }
    }
}
