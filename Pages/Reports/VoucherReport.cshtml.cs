using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using OfficeOpenXml;
using OfficeOpenXml;
using System.Data;
using System.IO;

namespace MiniAccounts.Pages.Reports
{
    [Authorize(Roles = "Admin,Accountant,Viewer")]
    public class VoucherReportModel : PageModel
    {
        private readonly IConfiguration _config;

        public VoucherReportModel(IConfiguration config)
        {
            _config = config;
        }

        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? VoucherType { get; set; }

        public List<VoucherReportItem> ReportItems { get; set; } = new();

        public class VoucherReportItem
        {
            public int VoucherId { get; set; }
            public string VoucherType { get; set; }
            public DateTime VoucherDate { get; set; }
            public string ReferenceNo { get; set; }
            public string AccountName { get; set; }
            public decimal DebitAmount { get; set; }
            public decimal CreditAmount { get; set; }
        }

        public void OnGet()
        {
            LoadReport();
        }

        public IActionResult OnPostExport()
        {
            // Set EPPlus license
            ExcelPackage.License.SetNonCommercialPersonal("Your Name or Organization");

            // Load the report data before exporting
            LoadReport();

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Vouchers");

            // Add headers
            worksheet.Cells[1, 1].Value = "Voucher No";
            worksheet.Cells[1, 2].Value = "Date";
            worksheet.Cells[1, 3].Value = "Type";
            worksheet.Cells[1, 4].Value = "Reference";
            worksheet.Cells[1, 5].Value = "Account";
            worksheet.Cells[1, 6].Value = "Debit";
            worksheet.Cells[1, 7].Value = "Credit";

            // Add data rows
            for (int i = 0; i < ReportItems.Count; i++)
            {
                var item = ReportItems[i];
                worksheet.Cells[i + 2, 1].Value = item.VoucherId;
                worksheet.Cells[i + 2, 2].Value = item.VoucherDate.ToShortDateString();
                worksheet.Cells[i + 2, 3].Value = item.VoucherType;
                worksheet.Cells[i + 2, 4].Value = item.ReferenceNo;
                worksheet.Cells[i + 2, 5].Value = item.AccountName;
                worksheet.Cells[i + 2, 6].Value = item.DebitAmount;
                worksheet.Cells[i + 2, 7].Value = item.CreditAmount;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            return File(stream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"VoucherReport_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }

        private void LoadReport()
        {
            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_GetVoucherReport", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@StartDate", (object?)StartDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@EndDate", (object?)EndDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@VoucherType", (object?)VoucherType ?? DBNull.Value);

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ReportItems.Add(new VoucherReportItem
                {
                    VoucherId = reader.GetInt32(0),
                    VoucherType = reader.GetString(1),
                    VoucherDate = reader.GetDateTime(2),
                    ReferenceNo = reader.GetString(3),
                    AccountName = reader.GetString(4),
                    DebitAmount = reader.GetDecimal(5),
                    CreditAmount = reader.GetDecimal(6),
                });
            }
        }
    }
}