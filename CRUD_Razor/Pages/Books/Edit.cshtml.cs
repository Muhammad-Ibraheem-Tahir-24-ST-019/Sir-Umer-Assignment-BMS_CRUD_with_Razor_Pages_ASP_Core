using CRUD_Razor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

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
            string cs = _configuration
                .GetConnectionString("DefaultConnection")!;

            using SqlConnection con = new SqlConnection(cs);
            con.Open();

            string query = "SELECT * FROM dbo.books WHERE Id = @Id";

            using SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.Add("@Id", System.Data.SqlDbType.Int).Value = id;

            using SqlDataReader reader = cmd.ExecuteReader();

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

            string cs = _configuration
                .GetConnectionString("DefaultConnection")!;

            using SqlConnection con = new SqlConnection(cs);
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

            using SqlCommand cmd = new SqlCommand(query, con);

            cmd.Parameters.Add("@Id", System.Data.SqlDbType.Int).Value = Book.Id;
            cmd.Parameters.Add("@title", System.Data.SqlDbType.NVarChar, 50).Value = (object?)Book.title?.Trim() ?? DBNull.Value;
            cmd.Parameters.Add("@author", System.Data.SqlDbType.NVarChar, 50).Value = (object?)Book.author?.Trim() ?? DBNull.Value;
            cmd.Parameters.Add("@description", System.Data.SqlDbType.NVarChar, 50).Value = (object?)Book.description?.Trim() ?? DBNull.Value;
            cmd.Parameters.Add("@publisher", System.Data.SqlDbType.NVarChar, 50).Value = (object?)Book.publisher?.Trim() ?? DBNull.Value;
            cmd.Parameters.Add("@status", System.Data.SqlDbType.NVarChar, 50).Value = (object?)Book.status?.Trim() ?? DBNull.Value;
            cmd.Parameters.Add("@category", System.Data.SqlDbType.NVarChar, 50).Value = (object?)Book.category?.Trim() ?? DBNull.Value;

            int rows = cmd.ExecuteNonQuery();

            if (rows == 0)
                return NotFound();

            TempData["SuccessMessage"] = "Book updated.";
            return RedirectToPage("Index");
        }
    }
}


