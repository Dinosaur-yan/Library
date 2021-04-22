using System;

namespace Library.API.Models
{
    /// <summary>
    /// 书
    /// </summary>
    public class BookDto
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public int Pages { get; set; }

        /// <summary>
        /// 作者Id
        /// </summary>
        public Guid AuthorId { get; set; }
    }
}
