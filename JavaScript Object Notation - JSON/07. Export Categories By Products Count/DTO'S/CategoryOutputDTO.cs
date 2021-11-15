namespace ProductShop.DTO_S
{
    public class CategoryOutputDTO
    {
        public string Category { get; set; }

        public int ProductsCount { get; set; }

        public string AveragePrice { get; set; }
        public string TotalRevenue { get; set; }
    }
}
