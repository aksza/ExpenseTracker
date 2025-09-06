using ExpenseTracker.Models;
using ExpenseTracker.Models.Enums;
using ExpenseTracker.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

[Authorize]
public class TransactionsController : Controller
{
    private readonly ITransactionService _transactionService;
    private readonly UserManager<AppUser> _userManager;

    public TransactionsController(ITransactionService transactionService, UserManager<AppUser> userManager)
    {
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
        var userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
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

        model.Categories = await _transactionService.GetCategoriesAsync(selectedType);

        ViewBag.TransactionTypes = Enum.GetValues(typeof(TransactionType))
            .Cast<TransactionType>()
            .Select(t => new SelectListItem
            {
                Value = ((int)t).ToString(),
                Text = t.ToString(),
                Selected = (t == selectedType)
            }).ToList();

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AddTransactionViewModel model, string? CategoryName)
    {
        try
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Basic validation
            if (model.Transaction.Amount <= 0)
            {
                TempData["ErrorMessage"] = "Amount must be greater than 0";
                return RedirectToAction("Index", new { type = model.Transaction.TransactionType });
            }

            if (model.Transaction.Date == default(DateTime))
            {
                TempData["ErrorMessage"] = "Date is required";
                return RedirectToAction("Index", new { type = model.Transaction.TransactionType });
            }

            int categoryId;

            if (model.CategoryId.HasValue && model.CategoryId > 0)
            {
                categoryId = model.CategoryId.Value;
            }
            else if (!string.IsNullOrEmpty(model.NewCategoryName))
            {
                var newCategory = await _transactionService.AddCategoryAsync(
                    model.NewCategoryName,
                    model.Transaction.TransactionType);
                categoryId = newCategory.Id;
            }
            else if (!string.IsNullOrEmpty(CategoryName))
            {
                var existingCategories = await _transactionService.GetCategoriesAsync(model.Transaction.TransactionType);
                var existingCategory = existingCategories.FirstOrDefault(c =>
                    c.Name.Equals(CategoryName, StringComparison.OrdinalIgnoreCase));

                if (existingCategory != null)
                {
                    categoryId = existingCategory.Id;
                }
                else
                {
                    var newCategory = await _transactionService.AddCategoryAsync(
                        CategoryName,
                        model.Transaction.TransactionType);
                    categoryId = newCategory.Id;
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Category is required";
                return RedirectToAction("Index", new { type = model.Transaction.TransactionType });
            }

            var transaction = new Transaction
            {
                UserId = userId,
                TransactionType = model.Transaction.TransactionType,
                CategoryId = categoryId,
                Amount = model.Transaction.Amount,
                Date = model.Transaction.Date,
                Description = model.Transaction.Description ?? string.Empty
            };

            await _transactionService.AddTransactionAsync(transaction);

            TempData["SuccessMessage"] = "Transaction added successfully!";
            return RedirectToAction("Index", new { type = model.Transaction.TransactionType });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error saving transaction: " + ex.Message;
            return RedirectToAction("Index", new { type = model.Transaction.TransactionType });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        var transaction = await _transactionService.GetTransactionByIdAsync(id, userId);
        if (transaction == null)
        {
            TempData["ErrorMessage"] = "Transaction not found";
            return RedirectToAction("Index");
        }

        var categories = await _transactionService.GetCategoriesAsync(transaction.TransactionType);

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
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var existingTransaction = await _transactionService.GetTransactionByIdAsync(model.Transaction.Id, userId);
            if (existingTransaction == null)
            {
                TempData["ErrorMessage"] = "Transaction not found";
                return RedirectToAction("Index");
            }

            if (model.Transaction.Amount <= 0)
            {
                TempData["ErrorMessage"] = "Amount must be greater than 0";
                return RedirectToAction("Index", new { type = model.Transaction.TransactionType });
            }

            if (model.Transaction.Date == default(DateTime))
            {
                TempData["ErrorMessage"] = "Date is required";
                return RedirectToAction("Index", new { type = model.Transaction.TransactionType });
            }

            int categoryId;

            if (model.CategoryId.HasValue && model.CategoryId > 0)
            {
                categoryId = model.CategoryId.Value;
            }
            else if (!string.IsNullOrEmpty(model.NewCategoryName))
            {
                var newCategory = await _transactionService.AddCategoryAsync(
                    model.NewCategoryName,
                    model.Transaction.TransactionType);
                categoryId = newCategory.Id;
            }
            else if (!string.IsNullOrEmpty(CategoryName))
            {
                var existingCategories = await _transactionService.GetCategoriesAsync(model.Transaction.TransactionType);
                var existingCategory = existingCategories.FirstOrDefault(c =>
                    c.Name.Equals(CategoryName, StringComparison.OrdinalIgnoreCase));

                if (existingCategory != null)
                {
                    categoryId = existingCategory.Id;
                }
                else
                {
                    var newCategory = await _transactionService.AddCategoryAsync(
                        CategoryName,
                        model.Transaction.TransactionType);
                    categoryId = newCategory.Id;
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Category is required";
                return RedirectToAction("Index", new { type = model.Transaction.TransactionType });
            }

            existingTransaction.CategoryId = categoryId;
            existingTransaction.Amount = model.Transaction.Amount;
            existingTransaction.Date = model.Transaction.Date;
            existingTransaction.Description = model.Transaction.Description ?? string.Empty;

            await _transactionService.UpdateTransactionAsync(existingTransaction);

            TempData["SuccessMessage"] = "Transaction updated successfully!";
            return RedirectToAction("Index", new { type = model.Transaction.TransactionType });
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
            var userId = _userManager.GetUserId(User);
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
}