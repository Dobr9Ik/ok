using System.Data.Entity;
using Ok.Domain.Core;

namespace Ok.Infrastructure.Data
{
    class ApplicationContext : DbContext
    {
        static ApplicationContext()
        {
            Database.SetInitializer(new DbInitializer());
        }

        public ApplicationContext():base("DbConnection") { }

        public DbSet<News> News { get; set; }
        public DbSet<Source> Sources { get; set; }
    }

    class DbInitializer : CreateDatabaseIfNotExists<ApplicationContext>
    {
        protected override void Seed(ApplicationContext context)
        {
            var sources = new[]
            {
                new Source {Name = "Хабрахабр", Url = "http://habrahabr.ru/rss/"},
                new Source {Name = "Интерфакс", Url = "http://www.interfax.by/news/feed"}
            };

            context.Sources.AddRange(sources);
            context.SaveChanges();
        }
    }
}
