﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Coffee_PryStore.Migrations
{
    [DbContext(typeof(DataBaseHome))]
    [Migration("20241011135213_PhotoADDAgain")]
    partial class PhotoADDAgain
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Coffee_PryStore.Models.CartItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("CartItems");
                });

            modelBuilder.Entity("Coffee_PryStore.Models.CategoryTable", b =>
                {
                    b.Property<string>("CategID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CategDescript")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CategName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CategID");

                    b.ToTable("CategoryTable");
                });

            modelBuilder.Entity("Coffee_PryStore.Models.HomeDataModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ReleaseDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("HomeDataModels");
                });

            modelBuilder.Entity("Coffee_PryStore.Models.Table", b =>
                {
                    b.Property<int>("CofId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CofId"));

                    b.Property<int>("CofAmount")
                        .HasColumnType("int");

                    b.Property<string>("CofCateg")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CofDuration")
                        .HasColumnType("datetime2");

                    b.Property<string>("CofName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("CofPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<byte[]>("ImageData")
                        .HasColumnType("varbinary(max)");

                    b.HasKey("CofId");

                    b.HasIndex("CofCateg");

                    b.ToTable("Table");
                });

            modelBuilder.Entity("Coffee_PryStore.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Coffee_PryStore.Models.Table", b =>
                {
                    b.HasOne("Coffee_PryStore.Models.CategoryTable", "Category")
                        .WithMany("Table")
                        .HasForeignKey("CofCateg")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("Coffee_PryStore.Models.CategoryTable", b =>
                {
                    b.Navigation("Table");
                });
#pragma warning restore 612, 618
        }
    }
}
