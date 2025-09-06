using ExpenseTracker.Models;
using ExpenseTracker.Models.Enums;
using ExpenseTracker.Models.ViewModels;
using ExpenseTracker.Services.Interfaces;
using ExpenseTracker.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

[Authorize]
public class TransactionsController : Controller
{
    private readonly ITransactionService _transactionService;
    private readonly ICategoryService _categoryService;
    private readonly ITransactionValidator _transactionValidator;
    private readonly UserManager<AppUser> _userManager;

    public TransactionsController(ITransactionService transactionService,
        ICategoryService categoryService,
        ITransactionValidator transactionValidator,
        UserManager<AppUser> userManager)
    {
        _categoryService = categoryService;
        _transactionValidator = transactionValidator;
        _transactionService = transactionService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(TransactionType? type,
                             int? categoryId,
                             DateTime? startDate,
                             DateTime? endDate,
                             int page = 1,
                             string sortColumn = "Date",
                             string sortOrder = "desc")
    {
        var selectedType = type ?? TransactionType.Expense;
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return RedirectToLogin();
        }

        var model = await _transactionService.GetUserTransactionsAsync(
            userId,
            selectedType,
            categoryId,
            startDate,
            endDate,
            page,
            10,
            sortColumn,
            sortOrder);

        model.Categories = await _categoryService.GetCategoriesAsync(selectedType);

        ViewBag.TransactionTypes = GetTransactionTypeSelectList(selectedType);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AddTransactionViewModel model, string? CategoryName)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToLogin();
            }

            var validationResult = _transactionValidator.ValidateTransaction(model.Transaction);
            if (!validationResult.IsValid)
            {
                TempData["ErrorMessage"] = validationResult.ErrorMessage;
                return RedirectToIndex(model.Transaction.TransactionType);
            }

            var categoryId = await GetOrCreateCategoryIdAsync(model, CategoryName);
            if (categoryId == null)
            {
                TempData["ErrorMessage"] = "Category is required";
                return RedirectToIndex(model.Transaction.TransactionType);
            }

            var transaction = CreateTransactionFromModel(model, userId, categoryId.Value);
            await _transactionService.AddTransactionAsync(transaction);

            TempData["SuccessMessage"] = "Transaction added successfully!";
            return RedirectToIndex(model.Transaction.TransactionType);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error saving transaction: " + ex.Message;
            return RedirectToIndex(model.Transaction.TransactionType);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var transaction = await _transactionService.GetTransactionByIdAsync(id, userId);
        if (transaction == null)
        {
            TempData["ErrorMessage"] = "Transaction not found";
            return RedirectToAction("Index");
        }

        var categories = await _categoryService.GetCategoriesAsync(transaction.TransactionType);

        var model = new AddTransactionViewModel
        {
            Transaction = transaction,
            Categories = categories,
            CategoryId = transaction.CategoryId
        };

        return PartialView("_EditTransactionModal", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(AddTransactionViewModel model, string? CategoryName)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToLogin();
            }

            var existingTransaction = await _transactionService.GetTransactionByIdAsync(model.Transaction.Id, userId);
            if (existingTransaction == null)
            {
                TempData["ErrorMessage"] = "Transaction not found";
                return RedirectToAction("Index");
            }

            var validationResult = _transactionValidator.ValidateTransaction(model.Transaction);
            if (!validationResult.IsValid)
            {
                TempData["ErrorMessage"] = validationResult.ErrorMessage;
                return RedirectToIndex(model.Transaction.TransactionType);
            }

            var categoryId = await GetOrCreateCategoryIdAsync(model, CategoryName);
            if (categoryId == null)
            {
                TempData["ErrorMessage"] = "Category is required";
                return RedirectToIndex(model.Transaction.TransactionType);
            }

            UpdateExistingTransaction(existingTransaction, model, categoryId.Value);
            await _transactionService.UpdateTransactionAsync(existingTransaction);

            TempData["SuccessMessage"] = "Transaction updated successfully!";
            return RedirectToIndex(model.Transaction.TransactionType);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error updating transaction: " + ex.Message;
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Json(new { success = false, message = "User not authenticated" });
            }

            var transaction = await _transactionService.GetTransactionByIdAsync(id, userId);
            if (transaction == null)
            {
                return Json(new { success = false, message = "Transaction not found" });
            }

            await _transactionService.DeleteTransactionAsync(id, userId);

            return Json(new { success = true, message = "Transaction deleted successfully" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error deleting transaction: " + ex.Message });
        }
    }

    #region Private Helper Methods

    private string? GetCurrentUserId() => _userManager.GetUserId(User);

    private IActionResult RedirectToLogin() => RedirectToAction("Login", "Account");

    private IActionResult RedirectToIndex(TransactionType type) =>
        RedirectToAction("Index", new { type });

    private List<SelectListItem> GetTransactionTypeSelectList(TransactionType selectedType)
    {
        return Enum.GetValues(typeof(TransactionType))
            .Cast<TransactionType>()
            .Select(t => new SelectListItem
            {
                Value = ((int)t).ToString(),
                Text = t.ToString(),
                Selected = (t == selectedType)
            }).ToList();
    }

    private async Task<int?> GetOrCreateCategoryIdAsync(AddTransactionViewModel model, string? categoryName)
    {
        if (model.CategoryId.HasValue && model.CategoryId > 0)
        {
            return model.CategoryId.Value;
        }

        var effectiveCategoryName = model.NewCategoryName ?? categoryName;
        if (string.IsNullOrEmpty(effectiveCategoryName))
        {
            return null;
        }

        var existingCategories = await _categoryService.GetCategoriesAsync(model.Transaction.TransactionType);
        var existingCategory = existingCategories.FirstOrDefault(c =>
            c.Name.Equals(effectiveCategoryName, StringComparison.OrdinalIgnoreCase));

        if (existingCategory != null)
        {
            return existingCategory.Id;
        }

        var newCategory = await _categoryService.AddCategoryAsync(
            effectiveCategoryName,
            model.Transaction.TransactionType);

        return newCategory.Id;
    }

    private static Transaction CreateTransactionFromModel(AddTransactionViewModel model, string userId, int categoryId)
    {
        return new Transaction
        {
            UserId = userId,
            TransactionType = model.Transaction.TransactionType,
            CategoryId = categoryId,
            Amount = model.Transaction.Amount,
            Date = model.Transaction.Date,
            Description = model.Transaction.Description ?? string.Empty
        };
    }

    private static void UpdateExistingTransaction(Transaction existingTransaction,
        AddTransactionViewModel model, int categoryId)
    {
        existingTransaction.CategoryId = categoryId;
        existingTransaction.Amount = model.Transaction.Amount;
        existingTransaction.Date = model.Transaction.Date;
        existingTransaction.Description = model.Transaction.Description ?? string.Empty;
    }

    #endregion
}