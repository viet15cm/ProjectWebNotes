using Domain.IdentityModel;
using Entities.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection.Metadata;

namespace Domain.Reposirory
{
    public class RepositoryContext : IdentityDbContext<AppUser>
    {
        public RepositoryContext(DbContextOptions options) : base(options)
        {
        }
        protected RepositoryContext()
        {
        }

        public virtual DbSet<Post> Posts { get; set; }

        public virtual DbSet<PostCategory> PostCategories { get; set; }

        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<Content> Contents { get; set; }

        public virtual DbSet<Image> Images { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var item in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = item.GetTableName();

                if (tableName.StartsWith("AspNet"))
                {
                    item.SetTableName(tableName.Substring(6));
                }

            }
            modelBuilder.Entity<Category>(entity => {
                entity.HasIndex(p => p.Slug).IsUnique();
            });


            modelBuilder.Entity<Post>(entity => {
                entity.HasIndex(p => p.Slug).IsUnique();
            });

            modelBuilder.Entity<Content>(entity => {
                entity.HasIndex(p => p.KeyTitleId).IsUnique();
            });

            // cấu hình mối quan hệ many to many

            modelBuilder.Entity<PostCategory>()
            .HasKey(bc => new { bc.CategoryID, bc.PostID });
            modelBuilder.Entity<PostCategory>()
                .HasOne(bc => bc.Post)
                .WithMany(b => b.PostCategories)
                .HasForeignKey(bc => bc.PostID);
            modelBuilder.Entity<PostCategory>()
                .HasOne(bc => bc.Category)
                .WithMany(c => c.PostCategories)
                .HasForeignKey(bc => bc.CategoryID);

            modelBuilder.Entity<AppUser>()
           .HasMany(e => e.Posts)
           .WithOne()
           .HasForeignKey(e => e.AuthorId);
           
        }



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);


        }
    }
}

