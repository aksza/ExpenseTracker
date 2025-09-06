using ExpenseTracker.Data;
using ExpenseTracker.Models;
using ExpenseTracker.Models.Enums;
using Microsoft.EntityFrameworkCore;

public class TransactionService : ITransactionService
{
    private readonly ApplicationDbContext _context;

    public TransactionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TransactionsViewModel> GetUserTransactionsAsync(
            string userId,
            TransactionType type,
            int? categoryId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int pageNumber = 1,
            int pageSize = 10,
            string sortColumn = "Date",
            string sortOrder = "desc")
    {
        var query = _context.Transactions
            .Include(t => t.Category)
            .Where(t => !t.IsDeleted && t.UserId == userId && t.TransactionType == type);

        if (categoryId.HasValue)
            query = query.Where(t => t.CategoryId == categoryId.Value);

        if (startDate.HasValue)
            query = query.Where(t => t.Date >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(t => t.Date <= endDate.Value);

        bool asc = sortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase);
        query = sortColumn switch
        {
            "Amount" => asc ? query.OrderBy(t => t.Amount) : query.OrderByDescending(t => t.Amount),
            "Category" => asc ? query.OrderBy(t => t.Category.Name) : query.OrderByDescending(t => t.Category.Name),
            "Date" or _ => asc ? query.OrderBy(t => t.Date) : query.OrderByDescending(t => t.Date)
        };

        var totalCount = await query.CountAsync();
        var transactions = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new TransactionsViewModel
        {
            Transactions = transactions,
            SelectedType = type,
            SelectedCategoryId = categoryId,
            StartDate = startDate,
            EndDate = endDate,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            SortColumn = sortColumn,
            SortOrder = sortOrder
        };
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

    public async Task<Transaction?> GetTransactionByIdAsync(int transactionId, string userId)
    {
        return await _context.Transactions
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == transactionId && t.UserId == userId && !t.IsDeleted);
    }

    public async Task AddTransactionAsync(Transaction transaction)
    {
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateTransactionAsync(Transaction transaction)
    {
        transaction.UpdatedAt = DateTime.UtcNow;
        _context.Transactions.Update(transaction);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTransactionAsync(int transactionId, string userId)
    {
        var transaction = await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == transactionId && t.UserId == userId && !t.IsDeleted);

        if (transaction != null)
        {
            transaction.IsDeleted = true;
            await _context.SaveChangesAsync();
        }
    }
}