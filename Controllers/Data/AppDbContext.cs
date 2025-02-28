﻿using Microsoft.EntityFrameworkCore;
using UpriseMidLevel.Models;

namespace UpriseMidTask.Controllers.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

    }

}
