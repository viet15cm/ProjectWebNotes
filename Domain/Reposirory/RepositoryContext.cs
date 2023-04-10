using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace Domain.Reposirory
{
    public class RepositoryContext : DbContext
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

        public virtual DbSet<Image> Images { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>(entity => {
                entity.HasIndex(p => p.Slug).IsUnique();
            });


            modelBuilder.Entity<Post>(entity => {
                entity.HasIndex(p => p.Slug).IsUnique();
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
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);


        }
    }
}

