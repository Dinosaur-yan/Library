using AutoMapper;
using Library.API.Entities;
using Library.API.Helpers;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Library.API.Controllers
{
    /// <summary>
    /// author
    /// </summary>
    [Route("api/authors")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        public AuthorController(IMapper mapper, IDistributedCache distributedCache, IAuthorRepository authorRepository)
        {
            Mapper = mapper;
            DistributedCache = distributedCache;
            AuthorRepository = authorRepository;
        }

        public IMapper Mapper { get; }
        public IDistributedCache DistributedCache { get; }
        public IAuthorRepository AuthorRepository { get; }

        /// <summary>
        /// 获取所有author资源
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = nameof(GetAuthors))]
        [ResponseCache(Duration = 15, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new string[] { "searchQuery" })]
        public async Task<ActionResult<List<AuthorDto>>> GetAuthors([FromQuery] AuthorResourceParameters parameters)
        {
            PagedList<Author> pagedList = null; //= await AuthorRepository.GetAllAsync(parameters);

            // 为了简单，仅当其不包含过滤和搜索时进行缓存
            if (string.IsNullOrWhiteSpace(parameters.Name) && string.IsNullOrWhiteSpace(parameters.SearchQuery))
            {
                string cacheKey = $"authors_page_{parameters.PageNumber}_pageSize_{parameters.PageSize}_sort_{parameters.SortBy}";
                string cachedContent = await DistributedCache.GetStringAsync(cacheKey);

                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.Converters.Add(new PagedListConverter<Author>());
                settings.Formatting = Formatting.Indented;

                if (string.IsNullOrWhiteSpace(cachedContent))
                {
                    pagedList = await AuthorRepository.GetAllAsync(parameters);
                    DistributedCacheEntryOptions options = new DistributedCacheEntryOptions
                    {
                        // 缓存超过2分钟没有被访问，则被移除
                        SlidingExpiration = TimeSpan.FromMinutes(2)
                    };

                    var serializedContent = JsonConvert.SerializeObject(pagedList, settings);
                    await DistributedCache.SetStringAsync(cacheKey, serializedContent);
                }
                else
                {
                    pagedList = JsonConvert.DeserializeObject<PagedList<Author>>(cachedContent, settings);
                }
            }
            else
            {
                pagedList = await AuthorRepository.GetAllAsync(parameters);
            }

            var paginationMetadata = new
            {
                totalCount = pagedList.TotalCount,
                pageSize = pagedList.PageSize,
                currentPage = pagedList.CurrentPage,
                totalPages = pagedList.TotalPages,
                previousPageLink = pagedList.HasPrevious ? Url.Link(nameof(GetAuthors), new
                {
                    pageNumber = pagedList.CurrentPage - 1,
                    pageSize = pagedList.PageSize,
                    name = parameters.Name,
                    searchQuery = parameters.SearchQuery,
                    sortBy = parameters.SortBy
                }) : null,
                nextPageLink = pagedList.HasNext ? Url.Link(nameof(GetAuthors), new
                {
                    pageNumber = pagedList.CurrentPage + 1,
                    pageSize = pagedList.PageSize,
                    name = parameters.Name,
                    searchQuery = parameters.SearchQuery,
                    sortBy = parameters.SortBy
                }) : null
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));

            var authorDtoList = Mapper.Map<IEnumerable<AuthorDto>>(pagedList);
            return authorDtoList.ToList();
        }

        /// <summary>
        /// 获取author资源
        /// </summary>
        /// <param name="authorId"></param>
        /// <returns></returns>
        [HttpGet("{authorId}", Name = nameof(GetAuthor))]
        [ResponseCache(CacheProfileName = "Never")]
        public async Task<ActionResult<AuthorDto>> GetAuthor([FromRoute] Guid authorId)
        {
            var author = await AuthorRepository.GetByIdAsync(authorId);
            if (author == null)
            {
                return NotFound();
            }

            var entityHash = HashFactory.GetHash(author);
            Response.Headers[HeaderNames.ETag] = entityHash;

            if (Request.Headers.TryGetValue(HeaderNames.IfNoneMatch, out var requestETag) && entityHash == requestETag)
            {
                return StatusCode((int)HttpStatusCode.NotModified);
            }

            return Mapper.Map<AuthorDto>(author);
        }

        /// <summary>
        /// 创建author资源
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateAuthor([FromBody] AuthorForCreationDto authorForCreationDto)
        {
            var author = Mapper.Map<Author>(authorForCreationDto);

            AuthorRepository.Create(author);
            var result = await AuthorRepository.SaveAsync();
            if (!result)
            {
                throw new Exception("创建资源author失败");
            }

            var authorDto = Mapper.Map<AuthorDto>(author);
            return CreatedAtRoute(nameof(GetAuthor), new { authorId = authorDto.Id }, authorDto);
        }

        /// <summary>
        /// 删除author资源
        /// </summary>
        /// <param name="authorId"></param>
        /// <returns></returns>
        [HttpDelete("{authorId}")]
        public async Task<ActionResult> DeleteAuthor([FromRoute] Guid authorId)
        {
            var author = await AuthorRepository.GetByIdAsync(authorId);
            if (author == null)
            {
                return NotFound();
            }

            AuthorRepository.Delete(author);
            var result = await AuthorRepository.SaveAsync();
            if (!result)
            {
                throw new Exception("删除资源author失败");
            }

            return NoContent();
        }
    }
}
