using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using wapp.Models;

namespace waap.Data

{ 

	public class ApplicationDbContext : IdentityDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

        public DbSet<Category> Categories { get; set; } = default;
        public DbSet<Client> Clients { get; set; } = default;
        public DbSet<Product> Products{ get; set; } = default;
        public DbSet<Sale> Sales { get; set; } = default;
        public DbSet<SaleProduct> SaleProducts { get; set; } = default;
        
    }
}
