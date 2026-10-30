using CRUD_Razor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

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

            string cs = _configuration
                .GetConnectionString("DefaultConnection")!;

            using SqlConnection con = new SqlConnection(cs);
            con.Open();

            string query = @"
                INSERT INTO dbo.books
                (title, author, description, publisher, status, category)
                VALUES
                (@title, @author, @description, @publisher, @status, @category)";

            using SqlCommand cmd = new SqlCommand(query, con);

            cmd.Parameters.Add("@title", System.Data.SqlDbType.NVarChar, 50).Value = (object?)Book.title?.Trim() ?? DBNull.Value;
            cmd.Parameters.Add("@author", System.Data.SqlDbType.NVarChar, 50).Value = (object?)Book.author?.Trim() ?? DBNull.Value;
            cmd.Parameters.Add("@description", System.Data.SqlDbType.NVarChar, 50).Value = (object?)Book.description?.Trim() ?? DBNull.Value;
            cmd.Parameters.Add("@publisher", System.Data.SqlDbType.NVarChar, 50).Value = (object?)Book.publisher?.Trim() ?? DBNull.Value;
            cmd.Parameters.Add("@status", System.Data.SqlDbType.NVarChar, 50).Value = (object?)Book.status?.Trim() ?? DBNull.Value;
            cmd.Parameters.Add("@category", System.Data.SqlDbType.NVarChar, 50).Value = (object?)Book.category?.Trim() ?? DBNull.Value;

            cmd.ExecuteNonQuery();

            TempData["SuccessMessage"] = "Book added.";
            return RedirectToPage("Index");
        }
    }
}

