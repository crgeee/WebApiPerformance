using WebApiPerformance.Helpers;

namespace WebApiPerformance.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<WebApiPerformance.DAL.TestContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(WebApiPerformance.DAL.TestContext context)
        {
            var testViewModels = StaticHelper.CreateTestData(80000);
            context.TestViewModels.AddRange(testViewModels);
            context.SaveChanges();
            base.Seed(context);
        }
    }
}
