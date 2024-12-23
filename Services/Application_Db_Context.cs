using Microsoft.EntityFrameworkCore;
using ProductStore_MVC_.Models;

namespace ProductStore_MVC_.Services
{
    public class Application_Db_Context : DbContext
    {
        public Application_Db_Context(DbContextOptions options) : base (options)
        {
        }
        public DbSet    <Product> Products {  get; set; }

    }
}
