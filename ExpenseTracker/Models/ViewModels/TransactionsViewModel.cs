using ExpenseTracker.Models;
using ExpenseTracker.Models.Enums;
using System;
using System.Collections.Generic;

public class TransactionsViewModel
{
    public List<Transaction> Transactions { get; set; } = new();
    public TransactionType SelectedType { get; set; }
    public int? SelectedCategoryId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public string SortColumn { get; set; } = "Date";
    public string SortOrder { get; set; } = "desc"; // "asc" or "desc"

    public List<Category> Categories { get; set; } = new(); // dropdown list
}
