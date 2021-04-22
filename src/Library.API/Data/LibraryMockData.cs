using Library.API.Models;
using System;
using System.Collections.Generic;

namespace Library.API.Data
{
    public class LibraryMockData
    {
        public static LibraryMockData Current { get; } = new LibraryMockData();

        public List<AuthorDto> Authors { get; set; }

        public List<BookDto> Books { get; set; }

        public LibraryMockData()
        {
            Authors = new List<AuthorDto>
            {
                new AuthorDto{ Id = new Guid("48556951-e6b7-44fa-a24b-448cfc4c8c4c"), Name = "Author 1", Age = 46, Email = "author1@xxx.com" },
                new AuthorDto{ Id = new Guid("597e9f16-a810-4c34-98ff-92a53bbebcb9"), Name = "Author 2", Age = 38, Email = "author2@xxx.com" },
            };

            Books = new List<BookDto>
            {
                new BookDto{ Id = new Guid("a87f37d4-70c1-4d8a-bd34-76ca4194982f"), Title = "Book 1", Description = "Description of Book 1", Pages = 281, AuthorId = new Guid("48556951-e6b7-44fa-a24b-448cfc4c8c4c") },
                new BookDto{ Id = new Guid("c3357824-6d12-4a41-a544-76659e848263"), Title = "Book 2", Description = "Description of Book 2", Pages = 370, AuthorId = new Guid("48556951-e6b7-44fa-a24b-448cfc4c8c4c") },
                new BookDto{ Id = new Guid("e1108d59-2bc3-4f2f-b643-f3d0cb3b7e0b"), Title = "Book 3", Description = "Description of Book 3", Pages = 229, AuthorId = new Guid("597e9f16-a810-4c34-98ff-92a53bbebcb9") },
                new BookDto{ Id = new Guid("f1ea8f47-aed7-4c33-a1c7-8a96abfc89aa"), Title = "Book 4", Description = "Description of Book 4", Pages = 440, AuthorId = new Guid("597e9f16-a810-4c34-98ff-92a53bbebcb9") },
            };
        }
    }
}
