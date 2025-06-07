using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;

namespace MiniAccounts.Pages.Accounts
{
    [Authorize(Roles = "Admin,Accountant")]
    public class CreateModel : PageModel
    {
        private readonly IConfiguration _config;

        public CreateModel(IConfiguration config)
        {
            _config = config;
        }

        [BindProperty]
        public AccountInputModel Account { get; set; }

        public List<AccountDropdownItem> AccountDropdown { get; set; } = new();

        public class AccountInputModel
        {
            [Required]
            public string AccountName { get; set; }

            [Required]
            public string AccountType { get; set; }

            public int? ParentId { get; set; }
        }

        public class AccountDropdownItem
        {
            public int Id { get; set; }
            public string AccountName { get; set; }
        }

        public void OnGet()
        {
            LoadParentAccounts();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                LoadParentAccounts();
                return Page();
            }

            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_ManageChartOfAccounts", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            // ? Updated parameter names to match your stored procedure
            cmd.Parameters.AddWithValue("@Mode", "Add");
            cmd.Parameters.AddWithValue("@AccountName", Account.AccountName);
            cmd.Parameters.AddWithValue("@AccountType", Account.AccountType);
            cmd.Parameters.AddWithValue("@ParentAccountId", (object?)Account.ParentId ?? DBNull.Value);

            conn.Open();
            cmd.ExecuteNonQuery();

            return RedirectToPage("Index");
        }

        private void LoadParentAccounts()
        {
            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("SELECT AccountId, AccountName FROM ChartOfAccounts", conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                AccountDropdown.Add(new AccountDropdownItem
                {
                    Id = reader.GetInt32(0),
                    AccountName = reader.GetString(1)
                });
            }
        }
    }
}
