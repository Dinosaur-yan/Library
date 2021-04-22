﻿using Library.API.Extensions;
using Microsoft.EntityFrameworkCore;
using System;

namespace Library.API.Entities
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
            : base(options)
        {

        }

        public DbSet<Author> Authors { get; set; }

        public DbSet<Book> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.SendData();

            base.OnModelCreating(modelBuilder);
        }
    }
}
