﻿// <auto-generated />
using System;
using Logistics.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HomeERP.Migrations
{
    [DbContext(typeof(AppDBContext))]
    partial class AppDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("HomeERP.Models.Domain.Attribute", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("EntityId")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("EntityId");

                    b.ToTable("Attributes");

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("HomeERP.Models.Domain.AttributeValue", b =>
                {
                    b.Property<Guid>("AttributeId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ObjectId")
                        .HasColumnType("uuid");

                    b.HasKey("AttributeId", "ObjectId");

                    b.HasIndex("ObjectId");

                    b.ToTable((string)null);

                    b.UseTpcMappingStrategy();
                });

            modelBuilder.Entity("HomeERP.Models.Domain.Entity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Entities");
                });

            modelBuilder.Entity("HomeERP.Models.Domain.Object", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("EntityId")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("EntityId");

                    b.ToTable("Objects");
                });

            modelBuilder.Entity("HomeERP.Models.Domain.LinkAttribute", b =>
                {
                    b.HasBaseType("HomeERP.Models.Domain.Attribute");

                    b.Property<Guid>("LinkedEntityId")
                        .HasColumnType("uuid");

                    b.HasIndex("LinkedEntityId");

                    b.ToTable("LinkAttributes");
                });

            modelBuilder.Entity("HomeERP.Models.Domain.DateAttributeValue", b =>
                {
                    b.HasBaseType("HomeERP.Models.Domain.AttributeValue");

                    b.Property<DateTime>("Value")
                        .HasColumnType("timestamp with time zone");

                    b.ToTable("DateAttributeValues");
                });

            modelBuilder.Entity("HomeERP.Models.Domain.IntegerAttributeValue", b =>
                {
                    b.HasBaseType("HomeERP.Models.Domain.AttributeValue");

                    b.Property<int>("Value")
                        .HasColumnType("integer");

                    b.ToTable("IntegerAttributeValues");
                });

            modelBuilder.Entity("HomeERP.Models.Domain.LinkAttributeValue", b =>
                {
                    b.HasBaseType("HomeERP.Models.Domain.AttributeValue");

                    b.Property<Guid>("Value")
                        .HasColumnType("uuid");

                    b.HasIndex("Value");

                    b.ToTable("LinkAttributeValues");
                });

            modelBuilder.Entity("HomeERP.Models.Domain.StringAttributeValue", b =>
                {
                    b.HasBaseType("HomeERP.Models.Domain.AttributeValue");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text");

                    b.ToTable("StringAttributeValues");
                });

            modelBuilder.Entity("HomeERP.Models.Domain.Attribute", b =>
                {
                    b.HasOne("HomeERP.Models.Domain.Entity", "Entity")
                        .WithMany()
                        .HasForeignKey("EntityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Entity");
                });

            modelBuilder.Entity("HomeERP.Models.Domain.AttributeValue", b =>
                {
                    b.HasOne("HomeERP.Models.Domain.Attribute", "Attribute")
                        .WithMany()
                        .HasForeignKey("AttributeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HomeERP.Models.Domain.Object", "Object")
                        .WithMany()
                        .HasForeignKey("ObjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Attribute");

                    b.Navigation("Object");
                });

            modelBuilder.Entity("HomeERP.Models.Domain.Object", b =>
                {
                    b.HasOne("HomeERP.Models.Domain.Entity", "Entity")
                        .WithMany()
                        .HasForeignKey("EntityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Entity");
                });

            modelBuilder.Entity("HomeERP.Models.Domain.LinkAttribute", b =>
                {
                    b.HasOne("HomeERP.Models.Domain.Attribute", null)
                        .WithOne()
                        .HasForeignKey("HomeERP.Models.Domain.LinkAttribute", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HomeERP.Models.Domain.Entity", null)
                        .WithMany()
                        .HasForeignKey("LinkedEntityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("HomeERP.Models.Domain.LinkAttributeValue", b =>
                {
                    b.HasOne("HomeERP.Models.Domain.Object", null)
                        .WithMany()
                        .HasForeignKey("Value")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
