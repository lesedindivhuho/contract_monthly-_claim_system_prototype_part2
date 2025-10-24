
using contract_monthly__claim_system_prototype_part2.Models;
using Microsoft.EntityFrameworkCore;

namespace contract_monthly__claim_system_prototype_part2.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) { }
        public DbSet<Claim> Claims { get; set; }
    }
}
