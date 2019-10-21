namespace NoSocNet.BLL.Models
{
    using NoSocNet.BLL.Enums;
    public class Paginator
    {
        public string TailId { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string SortColumn { get; set; }

        public SortOrder SortOrder { get; set; }
    }
}
