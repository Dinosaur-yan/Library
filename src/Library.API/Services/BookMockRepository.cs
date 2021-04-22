using Library.API.Data;
using Library.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.API.Services
{
    public class BookMockRepository : IBookRepository
    {
        public BookDto GetBookForAuthor(Guid authorId, Guid bookId)
        {
            return LibraryMockData.Current.Books.FirstOrDefault(w => w.AuthorId == authorId && w.Id == bookId);
        }

        public IEnumerable<BookDto> GetBooksForAuthor(Guid authorId)
        {
            return LibraryMockData.Current.Books.Where(w => w.AuthorId == authorId);
        }

        public void AddBook(BookDto book)
        {
            LibraryMockData.Current.Books.Add(book);
        }

        public void DeleteBook(BookDto book)
        {
            LibraryMockData.Current.Books.Remove(book);
        }

        public void UpdateBook(Guid authorId, Guid bookId, BookForUpdateDto updateBook)
        {
            var originalBook = GetBookForAuthor(authorId, bookId);

            originalBook.Title = updateBook.Title;
            originalBook.Pages = updateBook.Pages;
            originalBook.Description = updateBook.Description;
        }
    }
}
