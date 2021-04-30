using AutoMapper;
using Library.API.Entities;
using Library.API.Filters;
using Library.API.Helpers;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Library.API.Controllers
{
    /// <summary>
    /// book
    /// </summary>
    [Route("api/authors/{authorId}/books")]
    [ApiController]
    [ServiceFilter(typeof(CheckAuthorExistFilterAttribute))]
    public class BookController : ControllerBase
    {
        public BookController(IMapper mapper, IMemoryCache memoryCache, IRepositoryWrapper repositoryWrapper)
        {
            Mapper = mapper;
            MemoryCache = memoryCache;
            RepositoryWrapper = repositoryWrapper;
        }

        public IMapper Mapper { get; }
        public IMemoryCache MemoryCache { get; }
        public IRepositoryWrapper RepositoryWrapper { get; }

        /// <summary>
        /// 获取所有book资源
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = nameof(GetBooksAsync))]
        public async Task<ActionResult<ResourceCollect<BookDto>>> GetBooksAsync([FromRoute] Guid authorId)
        {
            string key = $"{authorId}_books";
            if (!MemoryCache.TryGetValue(key, out ResourceCollect<BookDto> resourceList))
            {
                var books = await RepositoryWrapper.Book.GetBooksAsync(authorId);
                if (books == null || !books.Any())
                {
                    return NotFound();
                }

                var bookDtoList = Mapper.Map<IEnumerable<BookDto>>(books).ToList();
                bookDtoList = bookDtoList.Select(book => CreateLinkForBook(authorId, book)).ToList();

                resourceList = new ResourceCollect<BookDto>(bookDtoList);

                //MemoryCache.Set(key, bookDtoList);

#pragma warning disable CS1587 // XML 注释没有放在有效语言元素上
                /**
                 * 使用MemoryCacheEntryOptions对象可控制缓存时间和优先级等属性
                 *      1>. 合理使用有效时间，不仅能确保资源被及时更新，也能使当资源不再使用时，所占用的内存能自动恢复
                 *      2>. 使用优先级属性，决定了当服务器内存不足时是否先将该项移除，当优先级为低时，将会被先移除。
                 *          如果不希望缓存项被移除，则应设置Priority属性为CacheItemPriority.NeverRemove。
                 */
#pragma warning restore CS1587 // XML 注释没有放在有效语言元素上
                MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
                options.AbsoluteExpiration = DateTime.Now.AddMinutes(10);   // 有效时间为10分钟
                options.Priority = CacheItemPriority.Normal;    // 优先级为默认
                MemoryCache.Set(key, resourceList, options);
            }
            return resourceList;
        }

        /// <summary>
        /// 获取book资源
        /// </summary>
        /// <param name="authorId"></param>
        /// <param name="bookId"></param>
        /// <returns></returns>
        [HttpGet("{bookId}", Name = nameof(GetBookAsync))]
        public async Task<ActionResult<BookDto>> GetBookAsync([FromRoute] Guid authorId, Guid bookId)
        {
            var book = await RepositoryWrapper.Book.GetBookAsync(authorId, bookId);
            if (book == null)
            {
                return NotFound();
            }

            var entityHash = HashFactory.GetHash(book);
            Response.Headers[HeaderNames.ETag] = entityHash;

            if (Request.Headers.TryGetValue(HeaderNames.IfNoneMatch, out var requestETag) && entityHash == requestETag)
            {
                return StatusCode((int)HttpStatusCode.NotModified);
            }

            var bookDto = Mapper.Map<BookDto>(book);
            return CreateLinkForBook(authorId, bookDto);
        }

        /// <summary>
        /// 创建book资源
        /// </summary>
        /// <returns></returns>
        [HttpPost(Name = nameof(CreateBookAsync))]
        public async Task<ActionResult> CreateBookAsync([FromRoute] Guid authorId, [FromBody] BookForCreationDto bookForCreationDto)
        {
            var book = Mapper.Map<Book>(bookForCreationDto);
            book.AuthorId = authorId;

            RepositoryWrapper.Book.Create(book);
            var result = await RepositoryWrapper.Book.SaveAsync();
            if (!result)
            {
                throw new Exception("创建资源book失败");
            }

            var bookDto = Mapper.Map<BookDto>(book);
            return CreatedAtRoute(nameof(GetBookAsync), new { authorId, bookId = bookDto.Id }, bookDto);
        }

        /// <summary>
        /// 删除book资源
        /// </summary>
        /// <param name="authorId"></param>
        /// <param name="bookId"></param>
        /// <returns></returns>
        [HttpDelete("{bookId}", Name = nameof(DeleteBookAsync))]
        public async Task<ActionResult> DeleteBookAsync([FromRoute] Guid authorId, Guid bookId)
        {
            var book = await RepositoryWrapper.Book.GetBookAsync(authorId, bookId);
            if (book == null)
            {
                return NotFound();
            }

            RepositoryWrapper.Book.Delete(book);
            var result = await RepositoryWrapper.Book.SaveAsync();
            if (!result)
            {
                throw new Exception("删除资源book失败");
            }

            return NoContent();
        }

        /// <summary>
        /// 全量更新book资源
        /// </summary>
        /// <param name="authorId"></param>
        /// <param name="bookId"></param>
        /// <param name="updateBook"></param>
        /// <returns></returns>
        [HttpPut("{bookId}", Name = nameof(UpdateBookAsync))]
        [CheckIfMatchHeaderFilter]
        public async Task<ActionResult> UpdateBookAsync([FromRoute] Guid authorId, Guid bookId, [FromBody] BookForUpdateDto updateBook)
        {
            var book = await RepositoryWrapper.Book.GetBookAsync(authorId, bookId);
            if (book == null)
            {
                return NotFound();
            }

            var entityHash = HashFactory.GetHash(book);
            if (Request.Headers.TryGetValue(HeaderNames.IfMatch, out var requestETag) && entityHash != requestETag)
            {
                return StatusCode(StatusCodes.Status412PreconditionFailed);
            }

            Mapper.Map(updateBook, book, typeof(BookForUpdateDto), typeof(Book));
            RepositoryWrapper.Book.Update(book);
            if (!await RepositoryWrapper.Book.SaveAsync())
            {
                throw new Exception("更新资源book失败");
            }

            var entityNewHash = HashFactory.GetHash(book);
            Response.Headers[HeaderNames.ETag] = entityNewHash;

            return NoContent();
        }

        /// <summary>
        /// 局部更新book资源
        /// </summary>
        /// <param name="authorId"></param>
        /// <param name="bookId"></param>
        /// <param name="patchDocument"></param>
        /// <returns></returns>
        [HttpPatch("{bookId}", Name = nameof(PartiallyUpdateBookAsync))]
        [CheckIfMatchHeaderFilter]
        public async Task<ActionResult> PartiallyUpdateBookAsync([FromRoute] Guid authorId, Guid bookId, [FromBody] JsonPatchDocument<BookForUpdateDto> patchDocument)
        {
            var book = await RepositoryWrapper.Book.GetBookAsync(authorId, bookId);
            if (book == null)
            {
                return NotFound();
            }

            var entityHash = HashFactory.GetHash(book);
            if (Request.Headers.TryGetValue(HeaderNames.IfMatch, out var requestETag) && entityHash != requestETag)
            {
                return StatusCode(StatusCodes.Status412PreconditionFailed);
            }

            var bookUpdateDto = Mapper.Map<BookForUpdateDto>(book);
            patchDocument.ApplyTo(bookUpdateDto, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Mapper.Map(bookUpdateDto, book, typeof(BookForUpdateDto), typeof(Book));

            RepositoryWrapper.Book.Update(book);
            if (!await RepositoryWrapper.Book.SaveAsync())
            {
                throw new Exception("更新资源book失败");
            }

            var entityNewHash = HashFactory.GetHash(book);
            Response.Headers[HeaderNames.ETag] = entityNewHash;

            return NoContent();
        }

        private BookDto CreateLinkForBook(Guid authorId, BookDto book)
        {
            book.Links.Clear();

            book.Links.Add(new Link(HttpMethods.Get.ToString(),
                "self",
                Url.Link(nameof(GetBookAsync), new { authorId, bookId = book.Id })));

            book.Links.Add(new Link(HttpMethods.Delete.ToString(),
                "delete book",
                Url.Link(nameof(DeleteBookAsync), new { authorId, bookId = book.Id })));

            book.Links.Add(new Link(HttpMethods.Put.ToString(),
                "update book",
                Url.Link(nameof(UpdateBookAsync), new { authorId, bookId = book.Id })));

            book.Links.Add(new Link(HttpMethods.Patch.ToString(),
                "partially update book",
                Url.Link(nameof(PartiallyUpdateBookAsync), new { authorId, bookId = book.Id })));

            return book;
        }

        private ResourceCollect<BookDto> CreateLinksForBooks(Guid authorId, ResourceCollect<BookDto> books)
        {
            books.Links.Clear();

            books.Links.Add(new Link(HttpMethods.Get.ToString(),
                "self",
                Url.Link(nameof(GetBookAsync), new { authorId })));

            books.Links.Add(new Link(HttpMethods.Delete.ToString(),
                "create book",
                Url.Link(nameof(CreateBookAsync), null)));

            return books;
        }
    }
}
