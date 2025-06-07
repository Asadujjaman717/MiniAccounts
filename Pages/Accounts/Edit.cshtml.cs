using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;

namespace MiniAccounts.Pages.Accounts
{
    public class EditModel : PageModel
    {
        private readonly IConfiguration _config;

        public EditModel(IConfiguration config)
        {
            _config = config;
        }

        [BindProperty]
        public AccountInputModel Account { get; set; }

        public List<AccountDropdownItem> AccountDropdown { get; set; } = new();

        public class AccountInputModel
        {
            public int AccountId { get; set; }

            [Required]
            public string AccountName { get; set; }

            [Required]
            public string AccountType { get; set; }

            public int? ParentAccountId { get; set; }
        }

        public class AccountDropdownItem
        {
            public int Id { get; set; }
            public string AccountName { get; set; }
        }

        public IActionResult OnGet(int id)
        {
            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("SELECT AccountId, AccountName, AccountType, ParentAccountId FROM ChartOfAccounts WHERE AccountId = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                Account = new AccountInputModel
                {
                    AccountId = reader.GetInt32(0),
                    AccountName = reader.GetString(1),
                    AccountType = reader.GetString(2),
                    ParentAccountId = reader.IsDBNull(3) ? null : reader.GetInt32(3)
                };
            }
            else
            {
                return NotFound();
            }

            LoadParentAccounts();
            return Page();
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

            cmd.Parameters.AddWithValue("@Mode", "Edit");
            cmd.Parameters.AddWithValue("@AccountId", Account.AccountId);
            cmd.Parameters.AddWithValue("@AccountName", Account.AccountName);
            cmd.Parameters.AddWithValue("@AccountType", Account.AccountType);
            cmd.Parameters.AddWithValue("@ParentAccountId", (object?)Account.ParentAccountId ?? DBNull.Value);

            conn.Open();
            cmd.ExecuteNonQuery();

            return RedirectToPage("Index");
        }

        private void LoadParentAccounts()
        {
            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("SELECT AccountId, AccountName FROM ChartOfAccounts WHERE AccountId != @CurrentId", conn);
            cmd.Parameters.AddWithValue("@CurrentId", Account?.AccountId ?? 0);
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
