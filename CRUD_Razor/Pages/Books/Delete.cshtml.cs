using CRUD_Razor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CRUD_Razor.Data;

namespace CRUD_Razor
{
    public class DeleteModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public DeleteModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Book Book { get; set; } = new();

        public IActionResult OnGet(int id)
        {
            using var con = BookDatabase.CreateConnection(_configuration);
            con.Open();

            string query = "SELECT * FROM dbo.books WHERE Id = @Id";

            using var cmd = BookDatabase.CreateCommand(con, query);
            cmd.AddParameter("@Id", System.Data.DbType.Int32, id);

            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
                return NotFound();

            Book = new Book
            {
                Id = (int)reader["Id"],
                title = reader["title"] as string,
                author = reader["author"] as string,
                description = reader["description"] as string,
                publisher = reader["publisher"] as string,
                status = reader["status"] as string,
                category = reader["category"] as string
            };

            return Page();
        }

        public IActionResult OnPost(int id)
        {
            using var con = BookDatabase.CreateConnection(_configuration);
            con.Open();

            string query = "DELETE FROM dbo.books WHERE Id = @Id";

            using var cmd = BookDatabase.CreateCommand(con, query);
            cmd.AddParameter("@Id", System.Data.DbType.Int32, id);

            int rows = cmd.ExecuteNonQuery();

            if (rows == 0)
                return NotFound();

            TempData["SuccessMessage"] = "Book deleted.";
            return RedirectToPage("Index");
        }
    }
}
