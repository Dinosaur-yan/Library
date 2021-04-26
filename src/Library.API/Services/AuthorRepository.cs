using Library.API.Entities;
using Library.API.Extensions;
using Library.API.Helpers;
using Library.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Library.API.Services
{
    public class AuthorRepository : RepositoryBase<Author, Guid>, IAuthorRepository
    {
        private Dictionary<string, PropertyMapping> mappingDict = null;

        public AuthorRepository(LibraryDbContext dbContext) : base(dbContext)
        {
            mappingDict = new Dictionary<string, PropertyMapping>(StringComparer.OrdinalIgnoreCase);
            mappingDict.Add(nameof(AuthorDto.Name), new PropertyMapping(nameof(Author.Name)));
            mappingDict.Add(nameof(AuthorDto.Age), new PropertyMapping(nameof(Author.BirthDate), true));
            mappingDict.Add(nameof(AuthorDto.BirthPlace), new PropertyMapping(nameof(Author.BirthPlace)));
        }

        public async Task<PagedList<Author>> GetAllAsync(AuthorResourceParameters parameters)
        {
            IQueryable<Author> queryableAuthors = DbContext.Set<Author>();

            if (!string.IsNullOrWhiteSpace(parameters.Name))
            {
                queryableAuthors = queryableAuthors.Where(m => m.Name.ToLower() == parameters.Name.ToLower());
            }
            if (!string.IsNullOrWhiteSpace(parameters.SearchQuery))
            {
                queryableAuthors = queryableAuthors.Where(m => m.Name.ToLower().Contains(parameters.SearchQuery.ToLower()));
            }

            var orderedAuthors = queryableAuthors.Sort(parameters.SortBy, mappingDict);
            return await PagedList<Author>.CreateAsync(queryableAuthors, parameters.PageNumber, parameters.PageSize);
        }
    }
}
