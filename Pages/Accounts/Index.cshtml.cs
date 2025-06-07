using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MiniAccounts.Pages.Accounts
{
    [Authorize(Roles = "Admin,Accountant")]
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<Account> AccountList { get; set; }

        public async Task OnGetAsync()
        {
            AccountList = new List<Account>();

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_ManageChartOfAccounts", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Mode", "Get");

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                AccountList.Add(new Account
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    ParentId = reader.IsDBNull(2) ? null : reader.GetInt32(2),
                    Type = reader.GetString(3)
                });
            }
        }

        public class Account
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int? ParentId { get; set; }
            public string Type { get; set; }
        }

        //public class AccountItem
        //{
        //    public int Id { get; set; }
        //    public string AccountName { get; set; }
        //    public string AccountType { get; set; }
        //    public int? ParentId { get; set; }
        //}

        //public class AccountTreeModel
        //{
        //    public List<AccountItem> AllAccounts { get; set; }
        //    public int? ParentId { get; set; }
        //}
    }
}
