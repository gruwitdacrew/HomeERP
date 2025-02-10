using Microsoft.EntityFrameworkCore;
using HomeERP.Models.Domain;
using Object = HomeERP.Models.Domain.Object;
using Attribute = HomeERP.Models.Domain.Attribute;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Object>().HasKey(Object => Object.Id);
            modelBuilder.Entity<Object>().HasOne(Object => Object.Entity).WithMany();

            modelBuilder.Entity<Attribute>().HasKey(Attribute => Attribute.Id);
            modelBuilder.Entity<Attribute>().HasOne(Attribute => Attribute.Entity).WithMany();
            modelBuilder.Entity<Attribute>().UseTptMappingStrategy();

            modelBuilder.Entity<LinkAttribute>().HasOne<Entity>().WithMany().HasForeignKey(LinkAttribute => LinkAttribute.LinkedEntityId);
            modelBuilder.Entity<LinkAttribute>().Ignore(LinkAttribute => LinkAttribute.EntityObjects);

            modelBuilder.Entity<AttributeValue>().UseTpcMappingStrategy();
            modelBuilder.Entity<AttributeValue>().HasKey(AttributeValue => new { AttributeValue.AttributeId, AttributeValue.ObjectId });
            modelBuilder.Entity<AttributeValue>().HasOne(AttributeValue => AttributeValue.Object).WithMany();
            modelBuilder.Entity<AttributeValue>().HasOne(AttributeValue => AttributeValue.Attribute).WithMany();

            modelBuilder.Entity<LinkAttributeValue>().HasOne<Object>().WithMany().HasForeignKey(LinkAttributeValue => LinkAttributeValue.Value);
        }
    }
}
