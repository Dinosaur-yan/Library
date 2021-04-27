using AutoMapper;
using Library.API.Entities;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Controllers
{
    /// <summary>
    /// book
    /// </summary>
    [Route("api/authors/{authorId}/books")]
    [ApiController]
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
        [HttpGet]
        public async Task<ActionResult<List<BookDto>>> GetBooks([FromRoute] Guid authorId)
        {
            string key = $"{authorId}_books";
            if (!MemoryCache.TryGetValue(key, out List<BookDto> bookDtoList))
            {
                var books = await RepositoryWrapper.Book.GetBooksAsync(authorId);
                bookDtoList = Mapper.Map<IEnumerable<BookDto>>(books).ToList();
                //MemoryCache.Set(key, bookDtoList);

                /**
                 * 使用MemoryCacheEntryOptions对象可控制缓存时间和优先级等属性
                 *      1>. 合理使用有效时间，不仅能确保资源被及时更新，也能使当资源不再使用时，所占用的内存能自动恢复
                 *      2>. 使用优先级属性，决定了当服务器内存不足时是否先将该项移除，当优先级为低时，将会被先移除。
                 *          如果不希望缓存项被移除，则应设置Priority属性为CacheItemPriority.NeverRemove。
                 */
                MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
                options.AbsoluteExpiration = DateTime.Now.AddMinutes(10);   // 有效时间为10分钟
                options.Priority = CacheItemPriority.Normal;    // 优先级为默认
                MemoryCache.Set(key, bookDtoList, options);
            }
            return bookDtoList;
        }

        /// <summary>
        /// 获取book资源
        /// </summary>
        /// <param name="authorId"></param>
        /// <param name="bookId"></param>
        /// <returns></returns>
        [HttpGet("{bookId}", Name = nameof(GetBook))]
        public async Task<ActionResult<BookDto>> GetBook([FromRoute] Guid authorId, Guid bookId)
        {
            var book = await RepositoryWrapper.Book.GetBookAsync(authorId, bookId);
            if (book == null)
            {
                return NotFound();
            }

            var bookDto = Mapper.Map<BookDto>(book);
            return bookDto;
        }

        /// <summary>
        /// 创建book资源
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateBook([FromRoute] Guid authorId, [FromBody] BookForCreationDto bookForCreationDto)
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
            return CreatedAtRoute(nameof(GetBook), new { authorId, bookId = bookDto.Id }, bookDto);
        }

        /// <summary>
        /// 删除book资源
        /// </summary>
        /// <param name="authorId"></param>
        /// <param name="bookId"></param>
        /// <returns></returns>
        [HttpDelete("{bookId}")]
        public async Task<ActionResult> DeleteBook([FromRoute] Guid authorId, Guid bookId)
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
        [HttpPut("{bookId}")]
        public async Task<ActionResult> UpdateBook([FromRoute] Guid authorId, Guid bookId, [FromBody] BookForUpdateDto updateBook)
        {
            var book = await RepositoryWrapper.Book.GetBookAsync(authorId, bookId);
            if (book == null)
            {
                return NotFound();
            }

            Mapper.Map(updateBook, book, typeof(BookForUpdateDto), typeof(Book));
            RepositoryWrapper.Book.Update(book);
            if (!await RepositoryWrapper.Book.SaveAsync())
            {
                throw new Exception("更新资源book失败");
            }

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
        public async Task<ActionResult> PartiallyUpdateBook([FromRoute] Guid authorId, Guid bookId, [FromBody] JsonPatchDocument<BookForUpdateDto> patchDocument)
        {
            var book = await RepositoryWrapper.Book.GetBookAsync(authorId, bookId);
            if (book == null)
            {
                return NotFound();
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
            return NoContent();
        }
    }
}
