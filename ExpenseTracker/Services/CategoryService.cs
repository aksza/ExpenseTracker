using ExpenseTracker.Data;
using ExpenseTracker.Models;
using ExpenseTracker.Models.Enums;
using ExpenseTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetCategoriesAsync(TransactionType type)
        {
            return await _context.Categories
                .Where(c => c.TransactionType == type)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int categoryId)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == categoryId);
        }

        public async Task<Category> AddCategoryAsync(string name, TransactionType type)
        {
            var category = new Category
            {
                Name = name,
                TransactionType = type
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }
    }
}
