using System.Collections.Generic;

namespace MiniAccounts.Pages.Accounts
{
    public class AccountTreeModel
    {
        public List<AccountItem> AllAccounts { get; set; }
        public int? ParentId { get; set; }
    }

    public class AccountItem
    {
        public int Id { get; set; }
        public string AccountName { get; set; }
        public string AccountType { get; set; }
        public int? ParentId { get; set; }
    }
}