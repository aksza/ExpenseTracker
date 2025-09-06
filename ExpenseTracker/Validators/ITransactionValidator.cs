using ExpenseTracker.Models;

namespace ExpenseTracker.Validators
{
    public interface ITransactionValidator
    {
        ValidationResult ValidateTransaction(Transaction transaction);
        ValidationResult ValidateAmount(decimal amount);
        ValidationResult ValidateDate(DateTime date);
    }
}
