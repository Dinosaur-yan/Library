using System.ComponentModel.DataAnnotations;

namespace Library.API.Models
{
    public class AuthorForCreationDto
    {
        /// <summary>
        /// 姓名
        /// </summary>
        [Required(ErrorMessage = "必须提供姓名")]
        [MaxLength(20, ErrorMessage = "姓名的最大长度为20个字符")]
        public string Name { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// 电子邮箱
        /// </summary>
        [EmailAddress(ErrorMessage = "邮箱格式不正确")]
        public string Email { get; set; }
    }
}
