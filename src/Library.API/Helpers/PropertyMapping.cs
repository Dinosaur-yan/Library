namespace Library.API.Helpers
{
    /// <summary>
    /// 属性映射
    /// </summary>
    public class PropertyMapping
    {
        public PropertyMapping(string targetProperty, bool revert = false)
        {
            TargetProperty = targetProperty;
            IsRevert = revert;
        }

        /// <summary>
        /// 目标属性
        /// </summary>
        public string TargetProperty { get; set; }

        /// <summary>
        /// 是否顺序相反
        /// </summary>
        public bool IsRevert { get; set; }
    }
}
