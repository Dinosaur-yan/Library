using System;

namespace Library.API.Models
{
    /// <summary>
    /// 作者
    /// </summary>
    public class AuthorDto
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string Email { get; set; }
    }
}
