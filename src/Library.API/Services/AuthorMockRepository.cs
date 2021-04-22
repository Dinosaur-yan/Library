using Library.API.Data;
using Library.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.API.Services
{
    public class AuthorMockRepository : IAuthorRepository
    {
        public AuthorDto GetAuthor(Guid authorId)
        {
            return LibraryMockData.Current.Authors.FirstOrDefault(w => w.Id == authorId);
        }

        public IEnumerable<AuthorDto> GetAuthors()
        {
            return LibraryMockData.Current.Authors;
        }

        public bool IsAuthorExists(Guid authorId)
        {
            return LibraryMockData.Current.Authors.Any(w => w.Id == authorId);
        }

        public void AddAuthor(AuthorDto author)
        {
            author.Id = Guid.NewGuid();
            LibraryMockData.Current.Authors.Add(author);
        }

        public void DeleteAuthor(AuthorDto author)
        {
            LibraryMockData.Current.Books.RemoveAll(book => book.AuthorId == author.Id);
            LibraryMockData.Current.Authors.Remove(author);
        }
    }
}
