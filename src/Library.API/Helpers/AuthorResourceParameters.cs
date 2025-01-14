﻿using Library.API.Entities;

namespace Library.API.Helpers
{
    public class AuthorResourceParameters
    {
        public const int MaxPageSize = 50;

        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;
        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                _pageSize = value > MaxPageSize ? MaxPageSize : value;
            }
        }

        public string Name { get; set; }

        public string SearchQuery { get; set; }

        public string SortBy { get; set; } = nameof(Author.Name);
    }
}
