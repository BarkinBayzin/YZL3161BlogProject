using Microsoft.EntityFrameworkCore;
using YZL3161BlogProject.Models.Entity;

namespace YZL3161BlogProject.Models.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Article> Articles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<User>().HasMany(x => x.Articles).HasOne(x => x.User).HasForeignKey(x => x.UserId); burada annotaionlar ile ilişkilendirme gerçekleştirdiğimiz için FluentAPI ile yazmak zorunda kalmadık fakat unutmayalımki olur da ikisini de çalıştırırsak, her zaman Fluent Api içeriisnde yazılan kodlar diğer kodları ezecektir.
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasData(new User(1, "bayzin", "1234"));
            modelBuilder.Entity<Article>().Property(x => x.CreatedTime).HasDefaultValueSql("getutcdate()");
        }
    }
}
