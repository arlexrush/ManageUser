namespace ManageUser.Application.Specification
{
    public class PaginationBaseQuery
    {
        public string? Search { get; set; }
        public string? Sort { get; set; }
        public int? PageIndex { get; set; } = 1;
        private int _pageSize = 3;
        private const int MaxPagesSize = 50;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPagesSize) ? MaxPagesSize : value;
        }
    }
}
