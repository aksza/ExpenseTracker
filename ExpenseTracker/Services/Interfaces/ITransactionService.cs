using ExpenseTracker.Models;
using ExpenseTracker.Models.Enums;

public interface ITransactionService
{
    Task<TransactionsViewModel> GetUserTransactionsAsync(
        string userId,
        TransactionType type,
        int? categoryId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int pageNumber = 1,
        int pageSize = 10,
        string sortColumn = "Date",
        string sortOrder = "desc");

    Task<List<Category>> GetCategoriesAsync(TransactionType type);
}
