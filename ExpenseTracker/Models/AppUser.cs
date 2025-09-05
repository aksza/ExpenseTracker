using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Models
{
    public class AppUser : IdentityUser
    {
        public bool IsDeleted { get; set; } = false;

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
