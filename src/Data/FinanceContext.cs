using Financial.Models;
using Microsoft.EntityFrameworkCore;

namespace Financial.Data
{
    public class FinanceContext : DbContext
    {

        public DbSet<BaseMoneyModel> finances { get; set; }
        public DbSet<UserModel> users { get; set; }

        protected readonly IConfiguration Configuration;

        public FinanceContext(DbContextOptions<FinanceContext> options) : base(options) { }
    }
}