using ExpenseTracker.Models;
using ExpenseTracker.Models.Enums;

namespace ExpenseTracker.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<Category>> GetCategoriesAsync(TransactionType type);
        Task<Category?> GetCategoryByIdAsync(int categoryId);
        Task<Category> AddCategoryAsync(string name, TransactionType type);
    }
}
