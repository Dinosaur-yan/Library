using Library.API.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace Library.API.Extensions
{
    public static class ModelBuilderExtension
    {
        public static void SendData(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>().HasData(
                new Author
                {
                    Id = new Guid("48556951-e6b7-44fa-a24b-448cfc4c8c4c"),
                    Name = "Author 1",
                    BirthDate = new DateTimeOffset(new DateTime(1960, 11, 16)),
                    Email = "author1@xxx.com"
                },
                new Author
                {
                    Id = new Guid("597e9f16-a810-4c34-98ff-92a53bbebcb9"),
                    Name = "Author 2",
                    BirthDate = new DateTimeOffset(new DateTime(1973, 6, 21)),
                    Email = "author2@xxx.com"
                }
            );

            modelBuilder.Entity<Book>().HasData(
                new Book
                {
                    Id = new Guid("a87f37d4-70c1-4d8a-bd34-76ca4194982f"),
                    Title = "Book 1",
                    Description = "Description of Book 1",
                    Pages = 281,
                    AuthorId = new Guid("48556951-e6b7-44fa-a24b-448cfc4c8c4c")
                },
                new Book
                {
                    Id = new Guid("c3357824-6d12-4a41-a544-76659e848263"),
                    Title = "Book 2",
                    Description = "Description of Book 2",
                    Pages = 370,
                    AuthorId = new Guid("48556951-e6b7-44fa-a24b-448cfc4c8c4c")
                },
                new Book
                {
                    Id = new Guid("e1108d59-2bc3-4f2f-b643-f3d0cb3b7e0b"),
                    Title = "Book 3",
                    Description = "Description of Book 3",
                    Pages = 229,
                    AuthorId = new Guid("597e9f16-a810-4c34-98ff-92a53bbebcb9")
                },
                new Book
                {
                    Id = new Guid("f1ea8f47-aed7-4c33-a1c7-8a96abfc89aa"),
                    Title = "Book 4",
                    Description = "Description of Book 4",
                    Pages = 440,
                    AuthorId = new Guid("597e9f16-a810-4c34-98ff-92a53bbebcb9")
                }
            );
        }
    }
}
