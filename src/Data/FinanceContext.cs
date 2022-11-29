using Financial.Models;
using Microsoft.EntityFrameworkCore;

namespace Financial.Data
{
    public class FinanceContext : DbContext
    {

        public DbSet<BaseMoneyModel> finances { get; set; }
        public DbSet<UserModel> users { get; set; }

        protected readonly IConfiguration Configuration;

        /*public FinanceContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public FinanceContext()
        { }*/
        public FinanceContext(DbContextOptions<FinanceContext> options) : base(options) { }

        public FinanceContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to postgres with connection string from app settings
            //options.UseNpgsql(Configuration.GetConnectionString("FinanceDB"));//.UseLowerCaseNamingConvention();
        }
    }
}