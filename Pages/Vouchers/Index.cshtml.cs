using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace MiniAccounts.Pages.Vouchers
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _config;

        public IndexModel(IConfiguration config)
        {
            _config = config;
        }

        public List<VoucherItem> VoucherList { get; set; } = new();

        public class VoucherItem
        {
            public int VoucherId { get; set; }
            public string VoucherType { get; set; }
            public DateTime VoucherDate { get; set; }
            public string ReferenceNo { get; set; }
        }

        public void OnGet()
        {
            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("SELECT VoucherId, VoucherType, VoucherDate, ReferenceNo FROM Vouchers ORDER BY VoucherDate DESC", conn);
            conn.Open();

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                VoucherList.Add(new VoucherItem
                {
                    VoucherId = reader.GetInt32(0),
                    VoucherType = reader.GetString(1),
                    VoucherDate = reader.GetDateTime(2),
                    ReferenceNo = reader.GetString(3)
                });
            }
        }
    }
}
