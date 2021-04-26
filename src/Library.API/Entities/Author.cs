using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.API.Entities
{
    public class Author
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        [Required]
        public DateTimeOffset BirthDate { get; set; }

        /// <summary>
        /// 出生地址
        /// </summary>
        public string BirthPlace { get; set; }

        /// <summary>
        /// 电子邮箱
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
