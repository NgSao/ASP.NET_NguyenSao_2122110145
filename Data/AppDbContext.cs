using Microsoft.EntityFrameworkCore;
using NguyenSao_2122110145.Models;
using System.Security.Claims;
namespace NguyenSao_2122110145.Data
{
    public class AppDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Otp> Otps { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Review> Reviews { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<AuditableEntity>();

            var currentUsername = "System";
            if (_httpContextAccessor.HttpContext != null)
            {
                var userEmail = _httpContextAccessor.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                currentUsername = userEmail ?? "System";

                var claims = _httpContextAccessor.HttpContext.User?.Claims;
                if (claims != null)
                {
                    foreach (var claim in claims)
                    {
                        Console.WriteLine($"Loại Claim: {claim.Type}, Giá trị: {claim.Value}");
                    }
                }
                else
                {
                    Console.WriteLine("Không tìm thấy claims trong HttpContext.User");
                }
            }
            else
            {
                Console.WriteLine("HttpContext là null");
            }

            Console.WriteLine($"Tên người dùng hiện tại: {currentUsername}");

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedDate = DateTime.UtcNow;
                    entry.Entity.CreatedBy = currentUsername;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedDate = DateTime.UtcNow;
                    entry.Entity.UpdatedBy = currentUsername;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }


    }


}
