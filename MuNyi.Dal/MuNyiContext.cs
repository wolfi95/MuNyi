using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MuNyi.Dal.Entities;
using MuNyi.Dal.Entities.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace MuNyi.Dal
{
    public class MuNyiContext : IdentityDbContext<User>
    {
        private readonly IConfiguration configuration;

        public DbSet<Project> Projects { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Work> WorkItems { get; set; }

        public MuNyiContext(DbContextOptions options, IConfiguration configuration) : base(options)
        {
            this.configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
            builder.UseMySQL("server=localhost;database=MuNyiContext");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Task>()
                .HasOne<Project>(t => t.Project);
            modelBuilder.Entity<Task>()
                .HasMany<Work>(t => t.WorkItems);
        }
    }
}
