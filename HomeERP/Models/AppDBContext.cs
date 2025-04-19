using Microsoft.EntityFrameworkCore;
using Object = HomeERP.Models.EAV.Domain.Object;
using Attribute = HomeERP.Models.EAV.Domain.Attribute;
using HomeERP.Models.EAV.Domain;
using HomeERP.Models.Chore.Domain;

namespace Logistics.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options){}

        public DbSet<Entity> Entities { get; set; }
        public DbSet<Object> Objects { get; set; }
        public DbSet<Attribute> Attributes { get; set; }
        public DbSet<LinkAttribute> LinkAttributes { get; set; }

        public DbSet<IntegerAttributeValue> IntegerAttributeValues { get; set; }
        public DbSet<StringAttributeValue> StringAttributeValues { get; set; }
        public DbSet<DateAttributeValue> DateAttributeValues { get; set; }
        public DbSet<LinkAttributeValue> LinkAttributeValues { get; set; }
        public DbSet<FileAttributeValue> FileAttributeValues { get; set; }
        public DbSet<FloatAttributeValue> FloatAttributeValues { get; set; }

        public DbSet<Chore> Chores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Object>().HasKey(Object => Object.Id);
            modelBuilder.Entity<Object>().HasOne(Object => Object.Entity).WithMany(Entity => Entity.Objects);
            

            modelBuilder.Entity<Attribute>().HasKey(Attribute => Attribute.Id);
            modelBuilder.Entity<Attribute>().HasOne(Attribute => Attribute.Entity).WithMany(Entity => Entity.Attributes);
            modelBuilder.Entity<Attribute>().UseTptMappingStrategy();

            modelBuilder.Entity<LinkAttribute>().HasOne<Entity>().WithMany().HasForeignKey(LinkAttribute => LinkAttribute.LinkedEntityId);
            modelBuilder.Entity<LinkAttribute>().Ignore(LinkAttribute => LinkAttribute.EntityObjects);

            modelBuilder.Entity<AttributeValue>().UseTpcMappingStrategy();
            modelBuilder.Entity<AttributeValue>().HasKey(AttributeValue => new { AttributeValue.AttributeId, AttributeValue.ObjectId });
            modelBuilder.Entity<AttributeValue>().HasOne(AttributeValue => AttributeValue.Object).WithMany(Object => Object.AttributeValues);
            modelBuilder.Entity<AttributeValue>().HasOne(AttributeValue => AttributeValue.Attribute).WithMany();

            modelBuilder.Entity<LinkAttributeValue>().HasOne<Object>().WithMany().HasForeignKey(LinkAttributeValue => LinkAttributeValue.Value);

            modelBuilder.Entity<Chore>().HasOne(Chore => Chore.Attribute).WithMany();
        }
    }
}
