using CRUD_Razor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CRUD_Razor.Data;

namespace CRUD_Razor
{
    public class CreateModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public CreateModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public Book Book { get; set; } = new();

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            using var con = BookDatabase.CreateConnection(_configuration);
            con.Open();

            string query = @"
                INSERT INTO dbo.books
                (title, author, description, publisher, status, category)
                VALUES
                (@title, @author, @description, @publisher, @status, @category)";

            using var cmd = BookDatabase.CreateCommand(con, query);

            cmd.AddParameter("@title", System.Data.DbType.String, (object?)Book.title?.Trim() ?? DBNull.Value, 50);
            cmd.AddParameter("@author", System.Data.DbType.String, (object?)Book.author?.Trim() ?? DBNull.Value, 50);
            cmd.AddParameter("@description", System.Data.DbType.String, (object?)Book.description?.Trim() ?? DBNull.Value, 50);
            cmd.AddParameter("@publisher", System.Data.DbType.String, (object?)Book.publisher?.Trim() ?? DBNull.Value, 50);
            cmd.AddParameter("@status", System.Data.DbType.String, (object?)Book.status?.Trim() ?? DBNull.Value, 50);
            cmd.AddParameter("@category", System.Data.DbType.String, (object?)Book.category?.Trim() ?? DBNull.Value, 50);

            cmd.ExecuteNonQuery();

            TempData["SuccessMessage"] = "Book added.";
            return RedirectToPage("Index");
        }
    }
}
