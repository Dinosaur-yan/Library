using AutoMapper;
using Library.API.Entities;
using Library.API.Helpers;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
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
        [HttpGet(Name = nameof(GetAuthorsAsync))]
        [ResponseCache(Duration = 15, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new string[] { "searchQuery" })]
        public async Task<ActionResult<ResourceCollect<AuthorDto>>> GetAuthorsAsync([FromQuery] AuthorResourceParameters parameters)
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
                previousPageLink = pagedList.HasPrevious ? Url.Link(nameof(GetAuthorsAsync), new
                {
                    pageNumber = pagedList.CurrentPage - 1,
                    pageSize = pagedList.PageSize,
                    name = parameters.Name,
                    searchQuery = parameters.SearchQuery,
                    sortBy = parameters.SortBy
                }) : null,
                nextPageLink = pagedList.HasNext ? Url.Link(nameof(GetAuthorsAsync), new
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
            authorDtoList = authorDtoList.Select(author => CreateLinkForAuthor(author));

            var resourceList = new ResourceCollect<AuthorDto>(authorDtoList.ToList());
            return CreateLinksForAuthors(resourceList);
        }

        /// <summary>
        /// 获取author资源
        /// </summary>
        /// <param name="authorId"></param>
        /// <returns></returns>
        [HttpGet("{authorId}", Name = nameof(GetAuthorAsync))]
        [ResponseCache(CacheProfileName = "Never")]
        public async Task<ActionResult<AuthorDto>> GetAuthorAsync([FromRoute] Guid authorId)
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

            var authorDto = Mapper.Map<AuthorDto>(author);
            return CreateLinkForAuthor(authorDto);
        }

        /// <summary>
        /// 创建author资源
        /// </summary>
        /// <returns></returns>
        [HttpPost(Name = nameof(CreateAuthorAsync))]
        public async Task<ActionResult> CreateAuthorAsync([FromBody] AuthorForCreationDto authorForCreationDto)
        {
            var author = Mapper.Map<Author>(authorForCreationDto);

            author.Id = Guid.NewGuid();
            AuthorRepository.Create(author);
            var result = await AuthorRepository.SaveAsync();
            if (!result)
            {
                throw new Exception("创建资源author失败");
            }

            var authorDto = Mapper.Map<AuthorDto>(author);
            return CreatedAtRoute(nameof(GetAuthorAsync), new { authorId = authorDto.Id }, authorDto);
        }

        /// <summary>
        /// 删除author资源
        /// </summary>
        /// <param name="authorId"></param>
        /// <returns></returns>
        [HttpDelete("{authorId}", Name = nameof(DeleteAuthorAsync))]
        public async Task<ActionResult> DeleteAuthorAsync([FromRoute] Guid authorId)
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

        private AuthorDto CreateLinkForAuthor(AuthorDto author)
        {
            author.Links.Clear();
            author.Links.Add(new Link(HttpMethod.Get.ToString(),
                "self",
                Url.Link(nameof(GetAuthorAsync), new { authorId = author.Id })));

            author.Links.Add(new Link(HttpMethod.Delete.ToString(),
                "delete author",
                Url.Link(nameof(DeleteAuthorAsync), new { authorId = author.Id })));

            author.Links.Add(new Link(HttpMethod.Get.ToString(),
                "author's books",
                Url.Link(nameof(BookController.GetBooksAsync), new { authorId = author.Id })));

            return author;
        }

        private ResourceCollect<AuthorDto> CreateLinksForAuthors(ResourceCollect<AuthorDto> authors,
            AuthorResourceParameters parameters = null, dynamic paginationData = null)
        {
            authors.Links.Clear();
            authors.Links.Add(new Link(HttpMethod.Get.ToString(),
                "self",
                Url.Link(nameof(GetAuthorsAsync), parameters)));

            authors.Links.Add(new Link(HttpMethod.Post.ToString(),
                "create author",
                Url.Link(nameof(CreateAuthorAsync), null)));

            if (paginationData != null)
            {
                if (paginationData.previousePageLink != null)
                {
                    authors.Links.Add(new Link(HttpMethod.Get.ToString(),
                        "previous page",
                        paginationData.previousePageLink));
                }

                if (paginationData.nextPageLink != null)
                {
                    authors.Links.Add(new Link(HttpMethod.Get.ToString(),
                        "next page",
                        paginationData.nextPageLink));
                }
            }

            return authors;
        }
    }
}
