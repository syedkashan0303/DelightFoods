using DelightFoods.APIs.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DelightFoods.APIs.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<ProductModel> Products { get; set; }
        public DbSet<CategoryModel> Category{ get; set; }

        public DbSet<CustomerModel> Customers { get; set; }
        public DbSet<OrderModel> Orders { get; set; }
        public DbSet<MediaGalleryModel> MediaGallery{ get; set; }
        public DbSet<CustomerAddress> CustomerAddress { get; set; }

    }
}
