namespace Library.API.Models
{
    public class BookForCreationDto
    {
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
    }
}
