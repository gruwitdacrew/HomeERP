using Microsoft.EntityFrameworkCore;
using HomeERP.Domain.EAV.Models;
using HomeERP.Domain.Chores.Models;
using Object = HomeERP.Domain.EAV.Models.Object;
using Attribute = HomeERP.Domain.EAV.Models.Attribute;
using HomeERP.Domain.Common.Models;
using Task = HomeERP.Domain.Chores.Models.Task;
using HomeERP.Domain.Product.Models;

namespace HomeERP.Domain.Common.Contexts
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

        public DbSet<Entity> Entities { get; set; }
        public DbSet<Object> Objects { get; set; }
        public DbSet<Attribute> Attributes { get; set; }

        public DbSet<IntegerAttributeValue> IntegerAttributeValues { get; set; }
        public DbSet<StringAttributeValue> StringAttributeValues { get; set; }
        public DbSet<DateAttributeValue> DateAttributeValues { get; set; }
        public DbSet<LinkAttributeValue> LinkAttributeValues { get; set; }
        public DbSet<FileAttributeValue> FileAttributeValues { get; set; }
        public DbSet<FloatAttributeValue> FloatAttributeValues { get; set; }

        public DbSet<Chore> Chores { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<ProductCollection> Bundles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Object>().HasKey(Object => Object.Id);
            modelBuilder.Entity<Object>().HasOne(Object => Object.Entity).WithMany(Entity => Entity.Objects);


            modelBuilder.Entity<Attribute>().HasKey(Attribute => Attribute.Id);
            modelBuilder.Entity<Attribute>().HasOne(Attribute => Attribute.Entity).WithMany(Entity => Entity.Attributes);
            modelBuilder.Entity<Attribute>().UseTphMappingStrategy();

            modelBuilder.Entity<LinkAttribute>().HasOne(linkAttribute => linkAttribute.LinkedEntity).WithMany().HasForeignKey("LinkedEntityId");

            modelBuilder.Entity<AttributeValue>().UseTpcMappingStrategy();
            modelBuilder.Entity<AttributeValue>().HasKey(AttributeValue => new { AttributeValue.ObjectId, AttributeValue.AttributeId, AttributeValue.ChangeDate });
            modelBuilder.Entity<AttributeValue>().HasOne(AttributeValue => AttributeValue.Object).WithMany(Object => Object.AttributeValues);
            modelBuilder.Entity<AttributeValue>().HasOne(AttributeValue => AttributeValue.Attribute).WithMany();

            modelBuilder.Entity<LinkAttributeValue>().HasOne<Object>().WithMany().HasForeignKey(LinkAttributeValue => LinkAttributeValue.Value);

            modelBuilder.Entity<Chore>().UseTphMappingStrategy()
                .HasDiscriminator(chore => chore.Type)
                .HasValue<RepetitiveChore>(ChoreType.Repetitive)
                .HasValue<PlanChore>(ChoreType.Plan);
            modelBuilder.Entity<Chore>().HasMany(Chore => Chore.Tasks).WithOne(Task => Task.Chore).HasForeignKey("ChoreId");

            modelBuilder.Entity<Task>().HasOne(task => task.User).WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.SetNull);


            modelBuilder.Entity<ProductInCollection>().HasKey("ProductId", "ProductCollectionId");
            modelBuilder.Entity<ProductInCollection>().HasOne(product => product.Product).WithMany().HasForeignKey("ProductId");

            modelBuilder.Entity<ProductCollection>().HasDiscriminator()
                .HasValue<Inventory>("Inventory")
                .HasValue<ShoppingList>("ShoppingList")
                .HasValue<ProductBundle>("ProductBundle");
            modelBuilder.Entity<ProductCollection>().HasMany<Object>().WithMany().UsingEntity<ProductInCollection>();

            modelBuilder.Entity<User>().HasData(new User()
            {
                Id = new Guid("d2503327-15ec-4b67-96f0-be16467a9dbe"),
                Name = "Demo User"
            });

            modelBuilder.Entity<Inventory>().HasData(new Inventory() { Id = new Guid("f6703327-15ec-4b67-96f0-be16467a9dbf") });
            modelBuilder.Entity<ShoppingList>().HasData(new ShoppingList() { Id = new Guid("c2703327-15ec-4b67-96f0-be16467a9dbf") });
            modelBuilder.Entity<Entity>().HasData(new Entity()
            {
                Id = new Guid("e2603327-15ec-4b67-96f0-be16467a9dbf"),
                Name = "Товары"
            });
        }
    }
}
