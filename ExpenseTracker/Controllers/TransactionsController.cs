using ExpenseTracker.Models;
using ExpenseTracker.Models.Enums;
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

    public async Task<IActionResult> Index( TransactionType? type,
                             int? categoryId,
                             DateTime? startDate,
                             DateTime? endDate,
                             int page = 1,
                             string sortColumn = "Date",
                             string sortOrder = "desc")
    {
        var selectedType = type ?? TransactionType.Expense;
        var userId = _userManager.GetUserId(User);

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
}
