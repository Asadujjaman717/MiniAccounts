using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace MiniAccounts.Pages.Vouchers
{
    public class DetailsModel : PageModel
    {
        private readonly IConfiguration _config;

        public DetailsModel(IConfiguration config)
        {
            _config = config;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public VoucherHeader Voucher { get; set; }
        public List<VoucherEntry> Entries { get; set; } = new();

        public class VoucherHeader
        {
            public int VoucherId { get; set; }
            public string VoucherType { get; set; }
            public DateTime VoucherDate { get; set; }
            public string ReferenceNo { get; set; }
        }

        public class VoucherEntry
        {
            public string AccountName { get; set; }
            public decimal DebitAmount { get; set; }
            public decimal CreditAmount { get; set; }
        }

        public void OnGet()
        {
            var connStr = _config.GetConnectionString("DefaultConnection");
            using var conn = new SqlConnection(connStr);
            conn.Open();

            // Fetch voucher
            using (var cmd = new SqlCommand("SELECT VoucherId, VoucherType, VoucherDate, ReferenceNo FROM Vouchers WHERE VoucherId = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", Id);
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    Voucher = new VoucherHeader
                    {
                        VoucherId = reader.GetInt32(0),
                        VoucherType = reader.GetString(1),
                        VoucherDate = reader.GetDateTime(2),
                        ReferenceNo = reader.GetString(3)
                    };
                }
            }

            // Fetch entries
            using (var cmd = new SqlCommand(@"
                SELECT c.AccountName, vd.DebitAmount, vd.CreditAmount
                FROM VoucherDetails vd
                INNER JOIN ChartOfAccounts c ON vd.AccountId = c.AccountId
                WHERE vd.VoucherId = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", Id);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Entries.Add(new VoucherEntry
                    {
                        AccountName = reader.GetString(0),
                        DebitAmount = reader.GetDecimal(1),
                        CreditAmount = reader.GetDecimal(2)
                    });
                }
            }
        }
    }
}
