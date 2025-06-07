using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace MiniAccounts.Pages.Vouchers
{
    [Authorize(Roles = "Admin,Accountant")]
    public class CreateVoucherModel : PageModel
    {
        private readonly IConfiguration _config;

        public CreateVoucherModel(IConfiguration config)
        {
            _config = config;
        }

        [BindProperty]
        public VoucherInputModel Voucher { get; set; } = new();

        public List<AccountDropdownItem> Accounts { get; set; } = new();

        public class VoucherInputModel
        {
            [Required]
            public string VoucherType { get; set; }

            [Required]
            [DataType(DataType.Date)]
            public DateTime VoucherDate { get; set; }

            [Required]
            public string ReferenceNo { get; set; }

            public List<VoucherDetailItem> Details { get; set; } = new();
        }

        public class VoucherDetailItem
        {
            [Required]
            public int AccountId { get; set; }

            public decimal DebitAmount { get; set; }

            public decimal CreditAmount { get; set; }
        }

        public class AccountDropdownItem
        {
            public int AccountId { get; set; }
            public string AccountName { get; set; }
        }

        public void OnGet()
        {
            LoadAccounts();
            // Add 2 blank rows for initial UI
            Voucher.Details.Add(new VoucherDetailItem());
            Voucher.Details.Add(new VoucherDetailItem());
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                LoadAccounts();
                return Page();
            }

            var dt = new DataTable();
            dt.Columns.Add("AccountId", typeof(int));
            dt.Columns.Add("DebitAmount", typeof(decimal));
            dt.Columns.Add("CreditAmount", typeof(decimal));

            foreach (var item in Voucher.Details)
            {
                if (item.AccountId > 0 && (item.DebitAmount > 0 || item.CreditAmount > 0))
                {
                    dt.Rows.Add(item.AccountId, item.DebitAmount, item.CreditAmount);
                }
            }

            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_SaveVoucher", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@VoucherType", Voucher.VoucherType);
            cmd.Parameters.AddWithValue("@VoucherDate", Voucher.VoucherDate);
            cmd.Parameters.AddWithValue("@ReferenceNo", Voucher.ReferenceNo);

            var tvpParam = cmd.Parameters.AddWithValue("@Details", dt);
            tvpParam.SqlDbType = SqlDbType.Structured;
            tvpParam.TypeName = "VoucherDetailType";

            conn.Open();
            cmd.ExecuteNonQuery();

            return RedirectToPage("Index");
        }

        private void LoadAccounts()
        {
            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("SELECT AccountId, AccountName FROM ChartOfAccounts", conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Accounts.Add(new AccountDropdownItem
                {
                    AccountId = reader.GetInt32(0),
                    AccountName = reader.GetString(1)
                });
            }
        }
    }
}