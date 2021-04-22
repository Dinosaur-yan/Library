using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.API.Controllers
{
    /// <summary>
    /// book
    /// </summary>
    [Route("api/authors/{authorId}/books")]
    [ApiController]
    public class BookController : ControllerBase
    {
        public BookController(IAuthorRepository authorRepository, IBookRepository bookRepository)
        {
            AuthorRepository = authorRepository;
            BookRepository = bookRepository;
        }

        public IAuthorRepository AuthorRepository { get; }
        public IBookRepository BookRepository { get; }

        /// <summary>
        /// 获取所有book资源
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<List<BookDto>> GetBooks([FromRoute] Guid authorId)
        {
            if (!AuthorRepository.IsAuthorExists(authorId))
            {
                return NotFound();
            }

            return BookRepository.GetBooksForAuthor(authorId).ToList();
        }

        /// <summary>
        /// 获取book资源
        /// </summary>
        /// <param name="authorId"></param>
        /// <param name="bookId"></param>
        /// <returns></returns>
        [HttpGet("{bookId}", Name = nameof(GetBook))]
        public ActionResult<BookDto> GetBook([FromRoute] Guid authorId, Guid bookId)
        {
            if (!AuthorRepository.IsAuthorExists(authorId))
            {
                return NotFound();
            }

            var targetBook = BookRepository.GetBookForAuthor(authorId, bookId);
            if (targetBook == null)
            {
                return NotFound();
            }

            return targetBook;
        }

        /// <summary>
        /// 创建book资源
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateBook([FromRoute] Guid authorId, [FromBody] BookForCreationDto bookForCreationDto)
        {
            if (!AuthorRepository.IsAuthorExists(authorId))
            {
                return NotFound();
            }

            var bookDto = new BookDto
            {
                Id = Guid.NewGuid(),
                Title = bookForCreationDto.Title,
                Description = bookForCreationDto.Description,
                Pages = bookForCreationDto.Pages,
                AuthorId = authorId
            };

            BookRepository.AddBook(bookDto);
            return CreatedAtRoute(nameof(GetBook), new { authorId, bookId = bookDto.Id }, bookDto);
        }

        /// <summary>
        /// 删除book资源
        /// </summary>
        /// <param name="authorId"></param>
        /// <param name="bookId"></param>
        /// <returns></returns>
        [HttpDelete("{bookId}")]
        public ActionResult<string[]> DeleteBook([FromRoute] Guid authorId, Guid bookId)
        {
            if (!AuthorRepository.IsAuthorExists(authorId))
            {
                return NotFound();
            }

            var book = BookRepository.GetBookForAuthor(authorId, bookId);
            if (book == null)
            {
                return NotFound();
            }

            BookRepository.DeleteBook(book);
            return NoContent();
        }

        /// <summary>
        /// 全量更新book资源
        /// </summary>
        /// <param name="authorId"></param>
        /// <param name="bookId"></param>
        /// <param name="updateBook"></param>
        /// <returns></returns>
        [HttpPut("{bookId}")]
        public ActionResult UpdateBook([FromRoute] Guid authorId, Guid bookId, [FromBody] BookForUpdateDto updateBook)
        {
            if (!AuthorRepository.IsAuthorExists(authorId))
            {
                return NotFound();
            }

            var book = BookRepository.GetBookForAuthor(authorId, bookId);
            if (book == null)
            {
                return NotFound();
            }

            BookRepository.UpdateBook(authorId, bookId, updateBook);
            return NoContent();
        }

        /// <summary>
        /// 局部更新book资源
        /// </summary>
        /// <param name="authorId"></param>
        /// <param name="bookId"></param>
        /// <param name="patchDocument"></param>
        /// <returns></returns>
        [HttpPatch("{bookId}")]
        public ActionResult PartiallyUpdateBook([FromRoute] Guid authorId, Guid bookId, [FromBody] JsonPatchDocument<BookForUpdateDto> patchDocument)
        {
            if (!AuthorRepository.IsAuthorExists(authorId))
            {
                return NotFound();
            }

            var book = BookRepository.GetBookForAuthor(authorId, bookId);
            if (book == null)
            {
                return NotFound();
            }

            var bookToPatch = new BookForUpdateDto
            {
                Title = book.Title,
                Description = book.Description,
                Pages = book.Pages
            };

            patchDocument.ApplyTo(bookToPatch, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            BookRepository.UpdateBook(authorId, bookId, bookToPatch);
            return NoContent();
        }
    }
}
