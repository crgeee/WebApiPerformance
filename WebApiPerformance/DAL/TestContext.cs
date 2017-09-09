using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using WebApiPerformance.Models;

namespace WebApiPerformance.DAL
{
    public class TestContext : DbContext
    {
        public TestContext() : base("TestContext") { }

        public DbSet<TestViewModel> TestViewModels { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}