using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace MiniAccounts.Pages.Accounts
{
    public class DeleteModel : PageModel
    {
        private readonly IConfiguration _config;

        public DeleteModel(IConfiguration config)
        {
            _config = config;
        }

        [BindProperty]
        public string AccountName { get; set; }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public IActionResult OnGet()
        {
            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("SELECT AccountName FROM ChartOfAccounts WHERE AccountId = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", Id);
            conn.Open();
            var result = cmd.ExecuteScalar();
            if (result == null)
            {
                return NotFound();
            }

            AccountName = result.ToString();
            return Page();
        }

        public IActionResult OnPost()
        {
            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_ManageChartOfAccounts", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Mode", "Delete");
            cmd.Parameters.AddWithValue("@AccountId", Id);

            conn.Open();
            cmd.ExecuteNonQuery();

            return RedirectToPage("Index");
        }
    }
}