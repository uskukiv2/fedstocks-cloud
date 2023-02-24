﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using fed.cloud.menu.infrastructure;

#nullable disable

namespace fed.cloud.menu.infrastructure.Migrations
{
    [DbContext(typeof(MenuContext))]
    [Migration("20230209054548_AddMaterializedViewsForRecipe")]
    partial class AddMaterializedViewsForRecipe
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("fed.cloud.menu.data.Entities.AuditLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("AffectedColumns")
                        .HasColumnType("text")
                        .HasColumnName("affectedcolumns");

                    b.Property<int>("Change")
                        .HasColumnType("integer")
                        .HasColumnName("change");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<string>("EntityName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("entityname");

                    b.Property<DateTime>("ModifiedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("modifieddate");

                    b.Property<string>("NewValues")
                        .HasColumnType("text")
                        .HasColumnName("newvalues");

                    b.Property<string>("OldValues")
                        .HasColumnType("text")
                        .HasColumnName("oldvalues");

                    b.Property<string>("PrimaryKey")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("primarykey");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("userid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("auditlogs", (string)null);
                });

            modelBuilder.Entity("fed.cloud.menu.data.Entities.Ingredient", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<int>("ProductNumber")
                        .HasColumnType("integer")
                        .HasColumnName("productnumber");

                    b.Property<int>("UnitId")
                        .HasColumnType("integer")
                        .HasColumnName("unitid");

                    b.HasKey("Id");

                    b.HasIndex("UnitId");

                    b.ToTable("ingredients", (string)null);
                });

            modelBuilder.Entity("fed.cloud.menu.data.Entities.Recipe", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("content");

                    b.Property<TimeSpan>("CookingTime")
                        .HasColumnType("interval")
                        .HasColumnName("cookingtime");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uuid")
                        .HasColumnName("ownerid");

                    b.Property<string>("Tags")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("tags");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("recipes", (string)null);
                });

            modelBuilder.Entity("fed.cloud.menu.data.Entities.RecipeIngredient", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("IngredientId")
                        .HasColumnType("uuid")
                        .HasColumnName("ingredientid");

                    b.Property<double>("Quantity")
                        .HasColumnType("double precision")
                        .HasColumnName("quantity");

                    b.Property<Guid>("RecipeId")
                        .HasColumnType("uuid")
                        .HasColumnName("recipeid");

                    b.HasKey("Id");

                    b.HasIndex("IngredientId");

                    b.HasIndex("RecipeId");

                    b.ToTable("recipeingredients", (string)null);
                });

            modelBuilder.Entity("fed.cloud.menu.data.Entities.Unit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<double>("Rate")
                        .HasColumnType("double precision")
                        .HasColumnName("rate");

                    b.Property<int>("TypeNumber")
                        .HasColumnType("integer")
                        .HasColumnName("typenumber");

                    b.HasKey("Id");

                    b.ToTable("units", (string)null);
                });

            modelBuilder.Entity("fed.cloud.menu.data.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("AuthenticationId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("authenticationid");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("isactive");

                    b.HasKey("Id");

                    b.HasIndex("AuthenticationId")
                        .IsUnique();

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("fed.cloud.menu.data.Models.Read.RecipeIngredientModel", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<long>("ProductNumber")
                        .HasColumnType("bigint")
                        .HasColumnName("productnumber");

                    b.Property<double>("Quantity")
                        .HasColumnType("double precision")
                        .HasColumnName("quantity");

                    b.Property<double>("Rate")
                        .HasColumnType("double precision")
                        .HasColumnName("rate");

                    b.Property<Guid>("RecipeId")
                        .HasColumnType("uuid")
                        .HasColumnName("recipeid");

                    b.ToView("mview_recipeingredients");
                });

            modelBuilder.Entity("fed.cloud.menu.data.Models.Read.RecipeModel", b =>
                {
                    b.Property<string>("Content")
                        .HasColumnType("text")
                        .HasColumnName("content");

                    b.Property<TimeSpan>("CookingTime")
                        .HasColumnType("interval")
                        .HasColumnName("cookingtime");

                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uuid")
                        .HasColumnName("ownerid");

                    b.Property<string>("Tags")
                        .HasColumnType("text")
                        .HasColumnName("tags");

                    b.ToView("mview_recipes");
                });

            modelBuilder.Entity("fed.cloud.menu.data.Entities.AuditLog", b =>
                {
                    b.HasOne("fed.cloud.menu.data.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("fed.cloud.menu.data.Entities.Ingredient", b =>
                {
                    b.HasOne("fed.cloud.menu.data.Entities.Unit", "Unit")
                        .WithMany()
                        .HasForeignKey("UnitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Unit");
                });

            modelBuilder.Entity("fed.cloud.menu.data.Entities.Recipe", b =>
                {
                    b.HasOne("fed.cloud.menu.data.Entities.User", "Owner")
                        .WithMany("Recipes")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("fed.cloud.menu.data.Entities.RecipeIngredient", b =>
                {
                    b.HasOne("fed.cloud.menu.data.Entities.Ingredient", "Ingredient")
                        .WithMany()
                        .HasForeignKey("IngredientId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("fed.cloud.menu.data.Entities.Recipe", null)
                        .WithMany("Ingredients")
                        .HasForeignKey("RecipeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Ingredient");
                });

            modelBuilder.Entity("fed.cloud.menu.data.Entities.Recipe", b =>
                {
                    b.Navigation("Ingredients");
                });

            modelBuilder.Entity("fed.cloud.menu.data.Entities.User", b =>
                {
                    b.Navigation("Recipes");
                });
#pragma warning restore 612, 618
        }
    }
}
