using Microsoft.EntityFrameworkCore;
using System;
using TestWorkService.DataBase.Model;

namespace TestWorkService.DataBase
{
    public class SearchEngineContext : DbContext
    {
        public SearchEngineContext()
        {
        }

        public SearchEngineContext(DbContextOptions<SearchEngineContext> options)
            : base(options)
        {

        }

        public DbSet<LinkPositionTracker> LinkTracker { get; set; }
        public DbSet<PositonAndDate> PositonAndDates { get; set; }
        public DbSet<LinkDetails> LinkDetails { get; set; }
        public DbSet<Keywords> Keywords { get; set; }
        public DbSet<ExternalLinks> ExternalLinks { get; set; }
        public DbSet<KeywordsInMeaningfulText> KeywordsInText { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=C:\\USERS\\ADMINISTRATOR\\KIRIL\\SEARCHENGINE.MDF; Integrated Security=False; User ID=Koko;Password=koko2403; Trusted_Connection=False; Connection Timeout=120; MultipleActiveResultSets=true", 
                sqlServerOptionsAction: sqlAction => 
                {
                    sqlAction.EnableRetryOnFailure(
                           maxRetryCount: 10,
                           maxRetryDelay: TimeSpan.FromSeconds(30),
                           errorNumbersToAdd: null
                        );
                });
            base.OnConfiguring(builder);

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<LinkPositionTracker>()
                .HasKey(k => new { k.Keywords, k.Link});
        }
    }
}
