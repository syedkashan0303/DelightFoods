using DelightFoods_Live.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DelightFoods_Live.Models.DTO;

namespace DelightFoods_Live.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<CustomerModel> Customers { get; set; }
        public DbSet<CustomerAddress> CustomerAddress { get; set; }
        public DbSet<CategoryModel> Category { get; set; } = default!;
        public DbSet<ProductModel> Product { get; set; } = default!;
        public DbSet<MediaGalleryModel> MediaGallery { get; set; } = default!;
        public DbSet<DelightFoods_Live.Models.DTO.CustomerDTO> CustomerDTO { get; set; }
        public DbSet<CartModel> Cart { get; set; }
        public DbSet<CartDTO> CartDTO { get; set; } 
        public DbSet<PaymentModel> Payment { get; set; } 
        public DbSet<SaleOrderModel> SaleOrder { get; set; }
        public DbSet<SaleOrderProductMappingModel> SaleOrderProductMapping { get; set; }

    }
}
