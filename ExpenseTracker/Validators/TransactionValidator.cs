using ExpenseTracker.Models;

namespace ExpenseTracker.Validators
{
    public class TransactionValidator : ITransactionValidator
    {
        public ValidationResult ValidateTransaction(Transaction transaction)
        {
            var result = new ValidationResult();

            if (transaction.Amount <= 0)
                result.AddError("Amount must be greater than 0");

            if (transaction.Date == default(DateTime))
                result.AddError("Date is required");

            return result;
        }

        public ValidationResult ValidateAmount(decimal amount)
        {
            var result = new ValidationResult();
            if (amount <= 0)
                result.AddError("Amount must be greater than 0");
            return result;
        }

        public ValidationResult ValidateDate(DateTime date)
        {
            var result = new ValidationResult();
            if (date == default(DateTime))
                result.AddError("Date is required");
            return result;
        }
    }
}
