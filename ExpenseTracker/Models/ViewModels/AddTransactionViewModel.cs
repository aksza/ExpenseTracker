namespace ExpenseTracker.Models.ViewModels
{
    public class AddTransactionViewModel
    {
        public Transaction Transaction { get; set; } = new Transaction();
        public List<Category> Categories { get; set; } = new List<Category>();

        public int? CategoryId { get; set; }
        public string? NewCategoryName { get; set; }
    }

}
