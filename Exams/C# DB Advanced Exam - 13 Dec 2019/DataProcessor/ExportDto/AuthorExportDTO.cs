namespace BookShop.DataProcessor.ExportDto
{
    public class AuthorExportDTO
    {
        public string AuthorName { get; set; }

        public AuthorBooksExportDTO[] Books { get; set; }
    }
}
