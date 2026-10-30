using CRUD_Razor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CRUD_Razor.Data;

namespace CRUD_Razor
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<Book> Books { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        public void OnGet()
        {
            using var con = BookDatabase.CreateConnection(_configuration);
            con.Open();

            Search = Search?.Trim();
            if (Search?.Length > 50)
                Search = Search[..50];

            string query = @"SELECT Id, title, author, description, publisher, status, category
                FROM dbo.books
                WHERE @search = N'' OR CHARINDEX(@search, title) > 0 OR CHARINDEX(@search, author) > 0
                ORDER BY Id DESC";

            using var cmd = BookDatabase.CreateCommand(con, query);
            cmd.AddParameter("@search", System.Data.DbType.String, Search ?? "", 50);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Books.Add(new Book
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    title = reader["title"] as string,
                    author = reader["author"] as string,
                    description = reader["description"] as string,
                    publisher = reader["publisher"] as string,
                    status = reader["status"] as string,
                    category = reader["category"] as string
                });
            }
        }
    }
}
