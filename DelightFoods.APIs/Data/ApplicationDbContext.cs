using DelightFoods.APIs.Model;
using DelightFoods.APIs.Model.DTO;
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

        public DbSet<ProductModel> Product { get; set; }
        public DbSet<CategoryModel> Category{ get; set; }

        public DbSet<CustomerModel> Customers { get; set; }
        public DbSet<OrderModel> Orders { get; set; }
        public DbSet<MediaGalleryModel> MediaGallery{ get; set; }
        public DbSet<SaleOrderModel> SaleOrder { get; set; } = default!;
        public DbSet<CustomerAddress> CustomerAddress { get; set; }
        public DbSet<CartDTO> CartDTO { get; set; } = default!;
        public DbSet<ShippingModel> Shipping { get; set; } = default!;
        public DbSet<PaymentTransaction> PaymentTransaction { get; set; } = default!;
        public DbSet<SaleOrderProductMappingModel> SaleOrderProductMapping { get; set; } = default!;


    }
}
