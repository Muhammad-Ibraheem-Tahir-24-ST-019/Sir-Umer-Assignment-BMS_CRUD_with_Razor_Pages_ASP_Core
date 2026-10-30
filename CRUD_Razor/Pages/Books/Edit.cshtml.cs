using CRUD_Razor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CRUD_Razor.Data;

namespace CRUD_Razor
{
    public class EditModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public EditModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
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
            if (id != Book.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return Page();

            using var con = BookDatabase.CreateConnection(_configuration);
            con.Open();

            string query = @"
                UPDATE dbo.books SET
                    title = @title,
                    author = @author,
                    description = @description,
                    publisher = @publisher,
                    status = @status,
                    category = @category
                WHERE Id = @Id";

            using var cmd = BookDatabase.CreateCommand(con, query);

            cmd.AddParameter("@Id", System.Data.DbType.Int32, Book.Id);
            cmd.AddParameter("@title", System.Data.DbType.String, (object?)Book.title?.Trim() ?? DBNull.Value, 50);
            cmd.AddParameter("@author", System.Data.DbType.String, (object?)Book.author?.Trim() ?? DBNull.Value, 50);
            cmd.AddParameter("@description", System.Data.DbType.String, (object?)Book.description?.Trim() ?? DBNull.Value, 50);
            cmd.AddParameter("@publisher", System.Data.DbType.String, (object?)Book.publisher?.Trim() ?? DBNull.Value, 50);
            cmd.AddParameter("@status", System.Data.DbType.String, (object?)Book.status?.Trim() ?? DBNull.Value, 50);
            cmd.AddParameter("@category", System.Data.DbType.String, (object?)Book.category?.Trim() ?? DBNull.Value, 50);

            int rows = cmd.ExecuteNonQuery();

            if (rows == 0)
                return NotFound();

            TempData["SuccessMessage"] = "Book updated.";
            return RedirectToPage("Index");
        }
    }
}
